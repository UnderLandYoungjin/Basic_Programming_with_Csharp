# WPF RS485 Modbus RTU 수신 모니터 (최종)


## 1. 목적

LS XGB PLC에서 RS485 통신(P2P)으로 전송하는 데이터를 PC에서 수신하여 정상적으로 데이터가 오고 있는지 확인하기 위한 WPF 모니터링 도구이다.

PLC가 인버터(LS G100)에 보내는 데이터를 USB-RS485 컨버터를 통해 PC에서 가로채어 확인하는 용도로, 본격적인 제어가 아닌 **데이터 수신 확인용**이다.


## 2. 시스템 구성

```
PLC (XGB)  ──RS485──  USB-RS485 컨버터  ──USB──  PC (WPF C#)
```

| 구성 요소 | 설명 |
|-----------|------|
| PLC | LS XGB, 채널2 P2P 통신으로 1초 주기 데이터 전송 |
| 통신 규격 | RS485, 모드버스 RTU 클라이언트(마스터) |
| 컨버터 | USB-RS485 (COM4로 인식) |
| PC 소프트웨어 | .NET 10 WPF + System.IO.Ports |


## 3. 통신 설정

| 항목 | 설정값 |
|------|--------|
| COM 포트 | COM4 |
| Baud Rate | 9600 bps |
| Data Bit | 8 bit |
| Parity | None |
| Stop Bit | 1 |
| 프레임 크기 | 9 bytes |
| 동기화 바이트(SLAVE) | 0x00 |


## 4. PLC 통신 주소 (LS G100 기준)

LS G100 인버터 매뉴얼(7.5 통신 호환 공통 영역 파라미터) 기준 주소 맵:

| 통신 번지 | 파라미터 | 스케일 | 단위 | R/W |
|-----------|----------|--------|------|-----|
| 0h0004 | Reserved | - | - | R/W |
| 0h0005 | 목표 주파수 | 0.01 | Hz | R/W |
| 0h0006 | 운전 지령(옵션) | - | - | R/W |

### 운전 지령(0h0006) 비트 할당

| 비트 | 기능 |
|------|------|
| B0 | 정지(S) |
| B1 | 정방향 운전(F) |
| B2 | 역방향 운전(R) |
| B3 | Trip Reset |
| B4 | 프리 런 정지 |

### PLC D 레지스터 → 운전 지령 값 예시

| D 레지스터 값 | 의미 |
|--------------|------|
| 1 (0x0001) | 정지 |
| 2 (0x0002) | 정방향 운전 |
| 4 (0x0004) | 역방향 운전 |


## 5. PLC P2P 통신 설정

| 항목 | 설정값 |
|------|--------|
| 채널 | 2 (RS485) |
| 모드 | P2P 사용 |
| 설정 드라이버 | 모드버스 RTU 클라이언트 |
| P2P 기능 | WRITE |
| 기동 조건 | _TIS |
| 방식 | 1. 개별 |
| 데이터 타입 | WORD |
| 변수 개수 | 1 |
| 상대국번 | 1 |
| 변수 설정 | READ1:D00000, SAVE1:0x40005 |


## 6. 프로젝트 환경

### 개발 환경

| 항목 | 버전 |
|------|------|
| Visual Studio | 2026 |
| .NET | 10 |
| 프로젝트 템플릿 | WPF 애플리케이션 (.NET) |

### NuGet 패키지

```
Install-Package System.IO.Ports
```

이 1개만 설치하면 된다. CommunityToolkit 등 추가 패키지는 사용하지 않는다.


## 7. 프로젝트 구조

```
RS485Monitor/
├── MainWindow.xaml           ← UI (Catppuccin Mocha 테마)
├── MainWindow.xaml.cs        ← 코드비하인드 (자동스크롤, 버튼 색상)
├── Models/
│   └── FrameData.cs          ← 수신 프레임 데이터 모델
├── ViewModels/
│   ├── BaseViewModel.cs      ← INotifyPropertyChanged 기본 구현
│   └── MainViewModel.cs      ← MVVM 메인 로직 + RelayCommand
└── Services/
    └── SerialService.cs      ← 시리얼 통신 (열기/읽기/닫기)
```


## 8. 핵심 동작 흐름

1. UI에서 통신 파라미터(COM, BAUD, SLAVE 등) 설정 후 연결 버튼 클릭
2. `SerialService.Open()`으로 시리얼 포트 열기 (버퍼 비우기 포함)
3. 백그라운드 스레드에서 `ReceiveLoop()` 실행
4. `ReadFrame()`에서 동기화 바이트(SLAVE)를 찾은 후 프레임 크기만큼 읽기
5. 수신 데이터를 HEX, Modbus RTU 파싱 결과로 DataGrid에 표시
6. 이전 프레임과 비교하여 데이터 변화 시 노란색 강조
7. 연결 해제 시 CancellationToken으로 루프 종료


## 9. 트러블슈팅 기록

### 프레임 쪼개짐 현상

초기 Python 버전에서 `ser.in_waiting`으로 버퍼에 있는 만큼만 읽었더니, 9바이트 프레임이 1바이트 + 8바이트로 쪼개져서 파싱이 깨지는 현상이 발생했다. PLC에서 1초 주기로 데이터를 전송하도록 수정한 후, 정확히 프레임 크기만큼 읽는 방식으로 해결했다.

### 프레임 동기화 문제

프레임 크기만큼 읽어도 시작점이 프레임 중간이면 매번 다른 데이터로 파싱되는 문제가 있었다. 동기화 바이트(프레임 첫 바이트)를 찾아서 정렬하는 로직을 추가하여 해결했다.

### 동기화 바이트 확인

PLC 설정에서 상대국번이 1이므로 슬레이브 ID 0x01로 동기화를 시도했으나, 실제 수신 데이터에 0x01이 없었다. raw 데이터 확인 결과 프레임이 `00`으로 시작하는 것을 확인하고, 동기화 바이트를 **0x00**으로 설정하여 해결했다. 이는 P2P 통신 특성상 표준 Modbus RTU 프레임과 다른 구조로 전송되기 때문이다.

### 연결 해제 시 예외 발생

연결 해제 시 `OperationCanceledException`이 발생했다. `SerialService.ReadFrame()`의 catch를 `catch (Exception)`으로 변경하고, `ReceiveLoop()` 전체를 try-catch로 감싸서 해결했다.

### 데이터가 안 들어올 때 확인 순서

1. USB-RS485 컨버터 배선 (A+/B- 연결 확인)
2. COM 포트 번호 (장치관리자에서 확인)
3. 통신 속도 (PLC와 동일한지 확인: 9600/19200)
4. PLC 통신 설정 (P2P 모드, 슬레이브 주소 등)
5. SLAVE(동기화 바이트) 값이 실제 프레임 시작 바이트와 일치하는지 확인


## 10. 참고 사항

- 본 프로그램은 데이터 수신 확인 전용이며, 인버터 제어 기능은 포함하지 않는다.
- LS 인버터의 Modbus 주소는 PLC 종류에 따라 +1 오프셋이 필요할 수 있다.
- PLC에서 워드(D 레지스터)로 데이터를 전송하며, 비트 단위 제어가 필요한 운전 지령(0h0006)도 D 레지스터에 비트 조합 값을 넣어 워드 단위로 전송한다.
- SLAVE 값을 0으로 설정하면 0x00 동기화, 0이 아닌 값이면 해당 값으로 동기화한다.
- DataGrid는 최대 500개 프레임까지 유지하며 초과 시 오래된 것부터 삭제된다.


---


## 11. 전체 소스 코드


### Models/FrameData.cs

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


### ViewModels/BaseViewModel.cs

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


### Services/SerialService.cs

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
            _serialPort.DiscardInBuffer();
        }

        public byte[]? ReadFrame(int frameSize, byte slaveId)
        {
            if (_serialPort == null || !_serialPort.IsOpen)
                return null;

            try
            {
                // 1단계: 슬레이브 주소(프레임 시작) 찾기
                while (true)
                {
                    int b = _serialPort.ReadByte();
                    if (b == slaveId) break;
                }

                // 2단계: 나머지 바이트 읽기
                byte[] buffer = new byte[frameSize];
                buffer[0] = slaveId;
                int totalRead = 1;

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


### ViewModels/MainViewModel.cs

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

        private byte _slaveId = 0;
        public byte SlaveId
        {
            get => _slaveId;
            set => SetProperty(ref _slaveId, value);
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
                StatusText = $"[연결 성공] {SelectedPort} / {BaudRate}bps / {DataBits}{SelectedParity[0]}{SelectedStopBits} / Slave:{SlaveId}";
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
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var frame = _serial.ReadFrame(FrameSize, SlaveId);
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

                        while (Frames.Count > 500)
                            Frames.RemoveAt(0);
                    });
                }
            }
            catch (Exception)
            {
                // 연결 해제 시 정상 종료
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


### MainWindow.xaml

```xml
<Window x:Class="RS485Monitor.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="clr-namespace:RS485Monitor.ViewModels"
        Title="RS485 Modbus RTU 수신 모니터"
        Width="1000" Height="650"
        MinWidth="850" MinHeight="500"
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
                    <TextBox Width="40" Margin="0,0,16,0"
                             Background="#45475A" Foreground="#CDD6F4"
                             BorderThickness="0" Padding="8,5" FontSize="13"
                             CaretBrush="#CDD6F4"
                             Text="{Binding FrameSize, UpdateSourceTrigger=PropertyChanged}"/>

                    <TextBlock Text="SLAVE" Foreground="{StaticResource TextSub}"
                               VerticalAlignment="Center" Width="48" FontSize="12"/>
                    <TextBox Width="40"
                             Background="#45475A" Foreground="#CDD6F4"
                             BorderThickness="0" Padding="8,5" FontSize="13"
                             CaretBrush="#CDD6F4"
                             Text="{Binding SlaveId, UpdateSourceTrigger=PropertyChanged}"/>
                </WrapPanel>

                <!-- 2행: 버튼 -->
                <StackPanel Orientation="Horizontal">
                    <Button Width="120" Padding="16,8" FontWeight="SemiBold" FontSize="13"
                            BorderThickness="0" Cursor="Hand"
                            Command="{Binding ToggleConnectionCommand}"
                            x:Name="BtnConnect"/>

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


### MainWindow.xaml.cs

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

            UpdateConnectButton(false);

            _vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.IsConnected))
                {
                    Dispatcher.Invoke(() => UpdateConnectButton(_vm.IsConnected));
                }
            };

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
