# WPF RS485 Modbus RTU 수신 모니터 (수정판)

## NuGet 패키지

**System.IO.Ports** 1개만 설치하면 됩니다.

```
도구 → NuGet 패키지 관리자 → 패키지 관리자 콘솔
Install-Package System.IO.Ports
```


## 프로젝트 구조

```
RS485Monitor/
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── Models/
│   └── FrameData.cs
├── ViewModels/
│   ├── BaseViewModel.cs
│   └── MainViewModel.cs
└── Services/
    └── SerialService.cs
```


---

## Models/FrameData.cs

```csharp
namespace RS485Monitor.Models
{
    public class FrameData
    {
        public int Number { get; set; }
        public string HexString { get; set; } = string.Empty;
        public int ByteCount { get; set; }
        public int SlaveId { get; set; }
        public string FuncCode { get; set; } = string.Empty;
        public string RegAddr { get; set; } = string.Empty;
        public int RegValue { get; set; }
        public string RegValueHex { get; set; } = string.Empty;
        public string Crc { get; set; } = string.Empty;
        public bool IsChanged { get; set; }
        public bool IsSizeWarning { get; set; }
    }
}
```


---

## ViewModels/BaseViewModel.cs

```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RS485Monitor.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(name);
            return true;
        }
    }
}
```


---

## Services/SerialService.cs

```csharp
using System;
using System.IO.Ports;

namespace RS485Monitor.Services
{
    public class SerialService : IDisposable
    {
        private SerialPort? _serialPort;

        public bool IsOpen => _serialPort != null && _serialPort.IsOpen;

        public void Open(string port, int baudRate, int dataBits, Parity parity, StopBits stopBits)
        {
            _serialPort = new SerialPort
            {
                PortName = port,
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parity,
                StopBits = stopBits,
                ReadTimeout = 5000
            };
            _serialPort.Open();
        }

        public byte[]? ReadFrame(int frameSize)
        {
            if (_serialPort == null || !_serialPort.IsOpen)
                return null;

            try
            {
                byte[] buffer = new byte[frameSize];
                int totalRead = 0;

                while (totalRead < frameSize)
                {
                    int read = _serialPort.Read(buffer, totalRead, frameSize - totalRead);
                    if (read == 0) return null;
                    totalRead += read;
                }

                return buffer;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Close()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        public void Dispose()
        {
            Close();
            _serialPort?.Dispose();
        }

        public static string[] GetPortNames() => SerialPort.GetPortNames();
    }
}
```


---

## ViewModels/MainViewModel.cs

```csharp
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RS485Monitor.Models;
using RS485Monitor.Services;

namespace RS485Monitor.ViewModels
{
    // 간단한 RelayCommand 구현
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter) => _execute();
    }

    public class MainViewModel : BaseViewModel
    {
        private readonly SerialService _serial = new();
        private CancellationTokenSource? _cts;
        private byte[]? _prevFrame;
        private int _frameCount;

        // ===== 설정 프로퍼티 =====
        private string _selectedPort = "COM4";
        public string SelectedPort
        {
            get => _selectedPort;
            set => SetProperty(ref _selectedPort, value);
        }

        private int _baudRate = 9600;
        public int BaudRate
        {
            get => _baudRate;
            set => SetProperty(ref _baudRate, value);
        }

        private int _dataBits = 8;
        public int DataBits
        {
            get => _dataBits;
            set => SetProperty(ref _dataBits, value);
        }

        private string _selectedParity = "None";
        public string SelectedParity
        {
            get => _selectedParity;
            set => SetProperty(ref _selectedParity, value);
        }

        private string _selectedStopBits = "1";
        public string SelectedStopBits
        {
            get => _selectedStopBits;
            set => SetProperty(ref _selectedStopBits, value);
        }

        private int _frameSize = 9;
        public int FrameSize
        {
            get => _frameSize;
            set => SetProperty(ref _frameSize, value);
        }

        // ===== 상태 프로퍼티 =====
        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        private string _statusText = "연결 대기중";
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        private int _totalFrames;
        public int TotalFrames
        {
            get => _totalFrames;
            set => SetProperty(ref _totalFrames, value);
        }

        // ===== 컬렉션 =====
        public ObservableCollection<string> AvailablePorts { get; } = new();
        public ObservableCollection<string> ParityOptions { get; } = new() { "None", "Odd", "Even" };
        public ObservableCollection<string> StopBitsOptions { get; } = new() { "1", "1.5", "2" };
        public ObservableCollection<FrameData> Frames { get; } = new();

        // ===== 커맨드 =====
        public ICommand RefreshPortsCommand { get; }
        public ICommand ToggleConnectionCommand { get; }
        public ICommand ClearLogCommand { get; }

        public MainViewModel()
        {
            RefreshPortsCommand = new RelayCommand(RefreshPorts);
            ToggleConnectionCommand = new RelayCommand(async () => await ToggleConnectionAsync());
            ClearLogCommand = new RelayCommand(ClearLog);
            RefreshPorts();
        }

        private void RefreshPorts()
        {
            AvailablePorts.Clear();
            foreach (var port in SerialService.GetPortNames())
            {
                AvailablePorts.Add(port);
            }
            if (AvailablePorts.Count > 0 && !AvailablePorts.Contains(SelectedPort))
            {
                SelectedPort = AvailablePorts[0];
            }
        }

        private async Task ToggleConnectionAsync()
        {
            if (IsConnected)
            {
                Disconnect();
            }
            else
            {
                await ConnectAsync();
            }
        }

        private async Task ConnectAsync()
        {
            try
            {
                var parity = SelectedParity switch
                {
                    "Odd" => Parity.Odd,
                    "Even" => Parity.Even,
                    _ => Parity.None
                };

                var stopBits = SelectedStopBits switch
                {
                    "1.5" => StopBits.OnePointFive,
                    "2" => StopBits.Two,
                    _ => StopBits.One
                };

                _serial.Open(SelectedPort, BaudRate, DataBits, parity, stopBits);
                IsConnected = true;
                StatusText = $"[연결 성공] {SelectedPort} / {BaudRate}bps / {DataBits}{SelectedParity[0]}{SelectedStopBits}";
                _frameCount = 0;
                _prevFrame = null;

                _cts = new CancellationTokenSource();
                await Task.Run(() => ReceiveLoop(_cts.Token));
            }
            catch (Exception ex)
            {
                StatusText = $"[에러] {ex.Message}";
                IsConnected = false;
            }
        }

        private void Disconnect()
        {
            _cts?.Cancel();
            _serial.Close();
            IsConnected = false;
            StatusText = "연결 해제됨";
        }

        private void ReceiveLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var frame = _serial.ReadFrame(FrameSize);
                if (frame == null) continue;

                _frameCount++;
                var data = ParseFrame(frame, _frameCount);

                // 이전 프레임과 비교
                if (_prevFrame != null)
                {
                    bool changed = false;
                    if (_prevFrame.Length != frame.Length)
                    {
                        changed = true;
                    }
                    else
                    {
                        for (int i = 0; i < frame.Length; i++)
                        {
                            if (frame[i] != _prevFrame[i])
                            {
                                changed = true;
                                break;
                            }
                        }
                    }
                    data.IsChanged = changed;
                }

                _prevFrame = (byte[])frame.Clone();

                Application.Current?.Dispatcher.Invoke(() =>
                {
                    Frames.Add(data);
                    TotalFrames = _frameCount;

                    // 최대 500개까지만 유지
                    while (Frames.Count > 500)
                        Frames.RemoveAt(0);
                });
            }
        }

        private FrameData ParseFrame(byte[] raw, int count)
        {
            var hexStr = BitConverter.ToString(raw).Replace("-", " ");

            var data = new FrameData
            {
                Number = count,
                HexString = hexStr,
                ByteCount = raw.Length,
                IsSizeWarning = raw.Length != FrameSize
            };

            if (raw.Length >= 8)
            {
                data.SlaveId = raw[0];
                data.FuncCode = $"0x{raw[1]:X2}";
                data.RegAddr = $"0x{(raw[2] << 8 | raw[3]):X4}";
                data.RegValue = (raw[4] << 8) | raw[5];
                data.RegValueHex = $"0x{data.RegValue:X4}";
                int crc = (raw[raw.Length - 1] << 8) | raw[raw.Length - 2];
                data.Crc = $"0x{crc:X4}";
            }

            return data;
        }

        private void ClearLog()
        {
            Frames.Clear();
            _frameCount = 0;
            TotalFrames = 0;
        }
    }
}
```


---

## MainWindow.xaml

```xml
<Window x:Class="RS485Monitor.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="clr-namespace:RS485Monitor.ViewModels"
        Title="RS485 Modbus RTU 수신 모니터"
        Width="950" Height="650"
        MinWidth="800" MinHeight="500"
        Background="#1E1E2E"
        WindowStartupLocation="CenterScreen">

    <Window.DataContext>
        <vm:MainViewModel/>
    </Window.DataContext>

    <Window.Resources>
        <SolidColorBrush x:Key="BgSurface" Color="#313244"/>
        <SolidColorBrush x:Key="BgOverlay" Color="#45475A"/>
        <SolidColorBrush x:Key="TextMain" Color="#CDD6F4"/>
        <SolidColorBrush x:Key="TextSub" Color="#A6ADC8"/>
        <SolidColorBrush x:Key="AccentGreen" Color="#A6E3A1"/>
        <SolidColorBrush x:Key="AccentRed" Color="#F38BA8"/>
        <SolidColorBrush x:Key="AccentBlue" Color="#89B4FA"/>
        <SolidColorBrush x:Key="AccentYellow" Color="#F9E2AF"/>
        <SolidColorBrush x:Key="AccentPeach" Color="#FAB387"/>

        <!-- Bool to Visibility -->
        <BooleanToVisibilityConverter x:Key="BoolToVis"/>
    </Window.Resources>

    <Grid Margin="16">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- 헤더 -->
        <TextBlock Grid.Row="0" Text="RS485 Modbus RTU 수신 모니터"
                   Foreground="{StaticResource AccentBlue}"
                   FontSize="20" FontWeight="Bold" Margin="0,0,0,12"/>

        <!-- 설정 영역 -->
        <Border Grid.Row="1" Background="{StaticResource BgSurface}"
                CornerRadius="8" Padding="16" Margin="0,0,0,12">
            <StackPanel>
                <!-- 1행: 통신 설정 -->
                <WrapPanel Margin="0,0,0,10">
                    <TextBlock Text="PORT" Foreground="{StaticResource TextSub}"
                               VerticalAlignment="Center" Width="45" FontSize="12"/>
                    <ComboBox Width="90" Margin="0,0,8,0"
                              Background="#45475A" Foreground="#CDD6F4"
                              BorderThickness="0" Padding="8,5" FontSize="13"
                              ItemsSource="{Binding AvailablePorts}"
                              SelectedItem="{Binding SelectedPort}"/>

                    <Button Content="↻" Width="30" Height="28" Margin="0,0,16,0"
                            Background="#45475A" Foreground="#CDD6F4"
                            BorderThickness="0" Cursor="Hand" FontSize="14"
                            Command="{Binding RefreshPortsCommand}"/>

                    <TextBlock Text="BAUD" Foreground="{StaticResource TextSub}"
                               VerticalAlignment="Center" Width="42" FontSize="12"/>
                    <TextBox Width="70" Margin="0,0,16,0"
                             Background="#45475A" Foreground="#CDD6F4"
                             BorderThickness="0" Padding="8,5" FontSize="13"
                             CaretBrush="#CDD6F4"
                             Text="{Binding BaudRate, UpdateSourceTrigger=PropertyChanged}"/>

                    <TextBlock Text="DATA" Foreground="{StaticResource TextSub}"
                               VerticalAlignment="Center" Width="40" FontSize="12"/>
                    <TextBox Width="40" Margin="0,0,16,0"
                             Background="#45475A" Foreground="#CDD6F4"
                             BorderThickness="0" Padding="8,5" FontSize="13"
                             CaretBrush="#CDD6F4"
                             Text="{Binding DataBits, UpdateSourceTrigger=PropertyChanged}"/>

                    <TextBlock Text="PARITY" Foreground="{StaticResource TextSub}"
                               VerticalAlignment="Center" Width="50" FontSize="12"/>
                    <ComboBox Width="75" Margin="0,0,16,0"
                              Background="#45475A" Foreground="#CDD6F4"
                              BorderThickness="0" Padding="8,5" FontSize="13"
                              ItemsSource="{Binding ParityOptions}"
                              SelectedItem="{Binding SelectedParity}"/>

                    <TextBlock Text="STOP" Foreground="{StaticResource TextSub}"
                               VerticalAlignment="Center" Width="38" FontSize="12"/>
                    <ComboBox Width="55" Margin="0,0,16,0"
                              Background="#45475A" Foreground="#CDD6F4"
                              BorderThickness="0" Padding="8,5" FontSize="13"
                              ItemsSource="{Binding StopBitsOptions}"
                              SelectedItem="{Binding SelectedStopBits}"/>

                    <TextBlock Text="FRAME" Foreground="{StaticResource TextSub}"
                               VerticalAlignment="Center" Width="48" FontSize="12"/>
                    <TextBox Width="40"
                             Background="#45475A" Foreground="#CDD6F4"
                             BorderThickness="0" Padding="8,5" FontSize="13"
                             CaretBrush="#CDD6F4"
                             Text="{Binding FrameSize, UpdateSourceTrigger=PropertyChanged}"/>
                </WrapPanel>

                <!-- 2행: 버튼 -->
                <StackPanel Orientation="Horizontal">
                    <!-- 연결 버튼 -->
                    <Button Width="120" Padding="16,8" FontWeight="SemiBold" FontSize="13"
                            BorderThickness="0" Cursor="Hand"
                            Command="{Binding ToggleConnectionCommand}"
                            x:Name="BtnConnect"/>

                    <!-- 로그 지우기 버튼 -->
                    <Button Content="로그 지우기" Margin="8,0,0,0"
                            Padding="16,8" FontWeight="SemiBold" FontSize="13"
                            Background="{StaticResource AccentPeach}" Foreground="#1E1E2E"
                            BorderThickness="0" Cursor="Hand"
                            Command="{Binding ClearLogCommand}"/>
                </StackPanel>
            </StackPanel>
        </Border>

        <!-- 데이터 그리드 -->
        <Border Grid.Row="2" Background="{StaticResource BgSurface}"
                CornerRadius="8" Padding="2">
            <DataGrid ItemsSource="{Binding Frames}"
                      AutoGenerateColumns="False"
                      IsReadOnly="True"
                      CanUserAddRows="False"
                      CanUserDeleteRows="False"
                      HeadersVisibility="Column"
                      GridLinesVisibility="Horizontal"
                      HorizontalGridLinesBrush="#45475A"
                      Background="Transparent"
                      RowBackground="#313244"
                      AlternatingRowBackground="#3B3D50"
                      Foreground="#CDD6F4"
                      FontFamily="Consolas"
                      FontSize="12.5"
                      BorderThickness="0"
                      SelectionMode="Single"
                      x:Name="DataGridLog">

                <DataGrid.ColumnHeaderStyle>
                    <Style TargetType="DataGridColumnHeader">
                        <Setter Property="Background" Value="#45475A"/>
                        <Setter Property="Foreground" Value="#A6ADC8"/>
                        <Setter Property="FontWeight" Value="SemiBold"/>
                        <Setter Property="Padding" Value="8,6"/>
                        <Setter Property="BorderThickness" Value="0,0,1,1"/>
                        <Setter Property="BorderBrush" Value="#585B70"/>
                    </Style>
                </DataGrid.ColumnHeaderStyle>

                <DataGrid.CellStyle>
                    <Style TargetType="DataGridCell">
                        <Setter Property="Padding" Value="8,4"/>
                        <Setter Property="BorderThickness" Value="0"/>
                        <Setter Property="Foreground" Value="#CDD6F4"/>
                        <Style.Triggers>
                            <DataTrigger Binding="{Binding IsChanged}" Value="True">
                                <Setter Property="Foreground" Value="#F9E2AF"/>
                                <Setter Property="FontWeight" Value="Bold"/>
                            </DataTrigger>
                            <DataTrigger Binding="{Binding IsSizeWarning}" Value="True">
                                <Setter Property="Foreground" Value="#F38BA8"/>
                            </DataTrigger>
                        </Style.Triggers>
                    </Style>
                </DataGrid.CellStyle>

                <DataGrid.Columns>
                    <DataGridTextColumn Header="#" Binding="{Binding Number}" Width="55"/>
                    <DataGridTextColumn Header="Bytes" Binding="{Binding ByteCount}" Width="50"/>
                    <DataGridTextColumn Header="HEX" Binding="{Binding HexString}" Width="*"/>
                    <DataGridTextColumn Header="Slave" Binding="{Binding SlaveId}" Width="55"/>
                    <DataGridTextColumn Header="Func" Binding="{Binding FuncCode}" Width="60"/>
                    <DataGridTextColumn Header="Addr" Binding="{Binding RegAddr}" Width="70"/>
                    <DataGridTextColumn Header="Value" Binding="{Binding RegValue}" Width="65"/>
                    <DataGridTextColumn Header="Hex" Binding="{Binding RegValueHex}" Width="65"/>
                    <DataGridTextColumn Header="CRC" Binding="{Binding Crc}" Width="70"/>
                </DataGrid.Columns>
            </DataGrid>
        </Border>

        <!-- 상태 바 -->
        <Border Grid.Row="3" Background="{StaticResource BgSurface}"
                CornerRadius="6" Padding="12,8" Margin="0,10,0,0">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="Auto"/>
                </Grid.ColumnDefinitions>

                <TextBlock Grid.Column="0" Text="{Binding StatusText}"
                           Foreground="{StaticResource AccentGreen}" FontSize="12.5"
                           FontFamily="Consolas" VerticalAlignment="Center"/>

                <StackPanel Grid.Column="1" Orientation="Horizontal">
                    <TextBlock Text="수신 프레임: " Foreground="{StaticResource TextSub}"
                               FontSize="12" VerticalAlignment="Center"/>
                    <TextBlock Text="{Binding TotalFrames}" FontWeight="Bold"
                               Foreground="{StaticResource AccentBlue}"
                               FontSize="13" VerticalAlignment="Center"/>
                </StackPanel>
            </Grid>
        </Border>
    </Grid>
</Window>
```


---

## MainWindow.xaml.cs

```csharp
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using RS485Monitor.ViewModels;

namespace RS485Monitor
{
    public partial class MainWindow : Window
    {
        private MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = (MainViewModel)DataContext;

            // 연결 버튼 초기 색상
            UpdateConnectButton(false);

            // 연결 상태 변경 시 버튼 색상 업데이트
            _vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.IsConnected))
                {
                    Dispatcher.Invoke(() => UpdateConnectButton(_vm.IsConnected));
                }
            };

            // DataGrid 자동 스크롤
            ((INotifyCollectionChanged)DataGridLog.Items).CollectionChanged += (s, e) =>
            {
                if (DataGridLog.Items.Count > 0)
                {
                    DataGridLog.ScrollIntoView(DataGridLog.Items[DataGridLog.Items.Count - 1]);
                }
            };
        }

        private void UpdateConnectButton(bool connected)
        {
            if (connected)
            {
                BtnConnect.Content = "연결 해제";
                BtnConnect.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F38BA8"));
                BtnConnect.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E2E"));
            }
            else
            {
                BtnConnect.Content = "연결";
                BtnConnect.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A6E3A1"));
                BtnConnect.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E2E"));
            }
        }
    }
}
```
