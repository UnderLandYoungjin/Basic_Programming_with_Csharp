# 아두이노 HC-SR04 초음파 센서 값을 C# WPF에 실시간 표시하기

아두이노에서 HC-SR04 초음파 센서로 거리를 측정해 시리얼(USB)로 보내고, C# WPF 프로그램이 그 값을 받아 UI에 실시간으로 띄우는 전체 과정을 다룬다. 펌웨어 → 통신 프로토콜 → WPF(MVVM) 순서로 진행하며, 그대로 따라 만들면 바로 동작하는 완성 코드를 제공한다.

---

## 1. 전체 동작 흐름

```
[HC-SR04 센서] --(Trig/Echo)--> [Arduino] --(USB Serial, 9600bps)--> [PC : C# WPF]
                                                                          │
                                                                 시리얼 수신 → 파싱 → UI 갱신
```

- 아두이노는 일정 주기(예: 100ms)마다 거리를 측정한다.
- 측정값을 `DIST:23.45\n` 형태의 텍스트 한 줄로 시리얼 전송한다.
- WPF는 `SerialPort`로 한 줄씩 받아 숫자만 뽑아내 화면에 표시한다.

문자열 한 줄(라인) 단위로 약속(프로토콜)을 정해두는 것이 핵심이다. 이렇게 하면 PC 쪽 파싱이 단순해지고, 통신이 끊겨도 다음 줄부터 정상 복구된다.

---

## 2. 하드웨어 결선

HC-SR04는 4핀(VCC, Trig, Echo, GND)이다. 아두이노 우노(UNO) 기준 결선은 다음과 같다.

| HC-SR04 | Arduino UNO | 비고 |
|---------|-------------|------|
| VCC     | 5V          | 5V 전원 |
| Trig    | D9          | 출력(트리거 펄스) |
| Echo    | D10         | 입력(에코 수신) |
| GND     | GND         | 공통 접지 |

> **주의**: HC-SR04의 Echo 출력은 5V이므로 UNO에서는 직결해도 되지만, 3.3V 보드(ESP32 등)를 쓸 경우 Echo에 분압 저항(예: 1kΩ + 2kΩ)을 넣어 3.3V로 낮춰야 보드가 손상되지 않는다.

측정 원리는 다음과 같다. Trig 핀에 10µs 펄스를 주면 센서가 초음파를 발사하고, 반사파가 돌아올 때까지의 시간을 Echo 핀의 HIGH 펄스 폭으로 알려준다. 음속(약 343m/s = 0.0343cm/µs)을 이용해 거리로 환산한다.

```
거리(cm) = (Echo 펄스 폭 µs × 0.0343) / 2
```

왕복 거리이므로 마지막에 2로 나누는 점이 포인트다.

---

## 3. 아두이노 펌웨어 (Arduino IDE)

아래 스케치를 그대로 업로드하면 된다. 측정 실패(범위 밖) 시 `-1`을 보내 PC가 구분할 수 있게 했다.

```cpp
// HC-SR04 거리 측정 후 시리얼로 전송
// 전송 포맷: "DIST:<거리cm>\n"  (예: DIST:23.45)
// 측정 실패(범위 밖) 시: "DIST:-1.00"

const int TRIG_PIN = 9;
const int ECHO_PIN = 10;

const unsigned long ECHO_TIMEOUT_US = 30000UL; // 최대 약 5m 대응 타임아웃
const int MEASURE_INTERVAL_MS = 100;           // 측정 주기 (10Hz)

void setup() {
  Serial.begin(9600);
  pinMode(TRIG_PIN, OUTPUT);
  pinMode(ECHO_PIN, INPUT);
  digitalWrite(TRIG_PIN, LOW);
}

float measureDistanceCm() {
  // 트리거 펄스 발생 (10us)
  digitalWrite(TRIG_PIN, LOW);
  delayMicroseconds(2);
  digitalWrite(TRIG_PIN, HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIG_PIN, LOW);

  // Echo HIGH 펄스 폭(us) 측정
  unsigned long duration = pulseIn(ECHO_PIN, HIGH, ECHO_TIMEOUT_US);

  if (duration == 0) {
    return -1.0; // 타임아웃 = 측정 실패
  }

  // 거리 = (시간 × 음속) / 2
  float distance = (duration * 0.0343f) / 2.0f;
  return distance;
}

void loop() {
  float distance = measureDistanceCm();

  Serial.print("DIST:");
  Serial.println(distance, 2); // 소수점 2자리, println이 끝에 \n 추가

  delay(MEASURE_INTERVAL_MS);
}
```

업로드 후 Arduino IDE의 **시리얼 모니터(9600 baud)** 를 열어 `DIST:30.12` 같은 줄이 100ms마다 올라오는지 먼저 확인한다. 여기까지 정상이면 하드웨어와 펌웨어는 완료된 것이다.

> **확인 후 시리얼 모니터는 반드시 닫는다.** 시리얼 포트는 한 번에 한 프로그램만 점유할 수 있어서, 모니터를 열어둔 채 WPF에서 같은 포트를 열면 "액세스가 거부되었습니다" 오류가 난다.

---

## 4. WPF 프로젝트 생성

### 4.1 프로젝트 만들기

Visual Studio에서 **WPF 애플리케이션(.NET 6 이상)** 으로 생성한다. 프로젝트 이름은 `SR04Monitor`로 가정한다.

### 4.2 시리얼 통신 패키지 추가

.NET Core/5+ WPF에서는 `System.IO.Ports`가 기본 포함되어 있지 않으므로 NuGet 패키지를 추가한다.

```powershell
# 패키지 관리자 콘솔
Install-Package System.IO.Ports
```

또는 `.csproj`에 직접 추가한다.

```xml
<ItemGroup>
  <PackageReference Include="System.IO.Ports" Version="8.0.0" />
</ItemGroup>
```

### 4.3 폴더 구조

MVVM 패턴으로 다음과 같이 구성한다.

```
SR04Monitor/
├── App.xaml
├── App.xaml.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── ViewModels/
│   ├── ViewModelBase.cs
│   └── MainViewModel.cs
├── Commands/
│   └── RelayCommand.cs
└── Services/
    └── SerialService.cs
```

---

## 5. 공통 클래스 (MVVM 기반)

### 5.1 ViewModelBase.cs

`INotifyPropertyChanged`를 구현한 ViewModel 베이스 클래스다. 속성 변경을 UI에 알린다.

```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SR04Monitor.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 값이 실제로 바뀐 경우에만 알림 (불필요한 UI 갱신 방지)
        protected bool SetProperty<T>(ref T field, T value,
                                      [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
```

### 5.2 RelayCommand.cs

버튼 등을 `Command`로 연결하기 위한 표준 `ICommand` 구현이다.

```csharp
using System;
using System.Windows.Input;

namespace SR04Monitor.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        // 버튼 활성/비활성 상태를 강제로 재평가시키고 싶을 때 호출
        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}
```

---

## 6. 시리얼 통신 서비스

시리얼 포트의 열기/닫기/수신을 담당하는 서비스 클래스다. ViewModel과 분리해 두면 테스트와 재사용이 쉽다.

### 6.1 SerialService.cs

```csharp
using System;
using System.IO.Ports;

namespace SR04Monitor.Services
{
    public class SerialService : IDisposable
    {
        private SerialPort? _port;

        // 한 줄(라인) 수신 시 발생하는 이벤트
        public event Action<string>? LineReceived;
        // 오류 발생 시
        public event Action<string>? ErrorOccurred;

        public bool IsOpen => _port?.IsOpen ?? false;

        // 현재 PC에 연결된 시리얼 포트 목록
        public static string[] GetPortNames() => SerialPort.GetPortNames();

        public void Open(string portName, int baudRate = 9600)
        {
            Close(); // 기존 연결 정리

            _port = new SerialPort(portName, baudRate)
            {
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                NewLine = "\n",          // println의 줄바꿈과 일치
                ReadTimeout = 1000,
                Encoding = System.Text.Encoding.ASCII
            };

            _port.DataReceived += OnDataReceived;
            _port.Open();
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // 버퍼에 쌓인 줄을 가능한 만큼 모두 읽는다
                while (_port != null && _port.IsOpen && _port.BytesToRead > 0)
                {
                    string line = _port.ReadLine().Trim();
                    if (!string.IsNullOrEmpty(line))
                        LineReceived?.Invoke(line);
                }
            }
            catch (TimeoutException)
            {
                // 줄이 아직 완성되지 않음 → 무시하고 다음 수신 대기
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(ex.Message);
            }
        }

        public void Close()
        {
            if (_port == null) return;

            try
            {
                _port.DataReceived -= OnDataReceived;
                if (_port.IsOpen) _port.Close();
            }
            catch { /* 닫는 중 예외는 무시 */ }
            finally
            {
                _port.Dispose();
                _port = null;
            }
        }

        public void Dispose() => Close();
    }
}
```

> **중요(스레드 주의)**: `DataReceived` 이벤트는 UI 스레드가 아닌 **별도의 스레드**에서 호출된다. 따라서 이 안에서 직접 UI 속성을 건드리면 안 되고, ViewModel에서 `Dispatcher`로 UI 스레드에 마샬링해야 한다(7장 참고). 위 서비스는 순수 문자열만 이벤트로 넘기므로 UI를 직접 만지지 않는다.

---

## 7. MainViewModel

화면 로직의 중심이다. 포트 목록, 연결 상태, 거리 값, 버튼 명령을 모두 관리한다.

### 7.1 MainViewModel.cs

```csharp
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using SR04Monitor.Commands;
using SR04Monitor.Services;

namespace SR04Monitor.ViewModels
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private readonly SerialService _serial = new();

        public MainViewModel()
        {
            ConnectCommand = new RelayCommand(_ => Connect(), _ => !IsConnected && SelectedPort != null);
            DisconnectCommand = new RelayCommand(_ => Disconnect(), _ => IsConnected);
            RefreshPortsCommand = new RelayCommand(_ => RefreshPorts());

            _serial.LineReceived += OnLineReceived;
            _serial.ErrorOccurred += OnError;

            RefreshPorts();
        }

        // ===== 포트 목록 =====
        public System.Collections.ObjectModel.ObservableCollection<string> Ports { get; }
            = new();

        private string? _selectedPort;
        public string? SelectedPort
        {
            get => _selectedPort;
            set { SetProperty(ref _selectedPort, value); }
        }

        // ===== 연결 상태 =====
        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (SetProperty(ref _isConnected, value))
                {
                    OnPropertyChanged(nameof(StatusText));
                }
            }
        }

        public string StatusText => IsConnected ? "연결됨" : "연결 끊김";

        // ===== 거리 값 =====
        private double _distanceCm;
        public double DistanceCm
        {
            get => _distanceCm;
            set
            {
                if (SetProperty(ref _distanceCm, value))
                {
                    OnPropertyChanged(nameof(DistanceText));
                    OnPropertyChanged(nameof(IsOutOfRange));
                }
            }
        }

        // 표시용 문자열 (-1이면 범위 밖)
        public string DistanceText =>
            IsOutOfRange ? "범위 밖" : $"{DistanceCm:F2} cm";

        public bool IsOutOfRange => DistanceCm < 0;

        // 게이지(ProgressBar)용: 0~400cm 범위를 0~100%로 환산
        public double DistancePercent =>
            IsOutOfRange ? 0 : Math.Clamp(DistanceCm / 400.0 * 100.0, 0, 100);

        // ===== 마지막 수신 원문(디버그용) =====
        private string _lastRaw = "";
        public string LastRaw
        {
            get => _lastRaw;
            set => SetProperty(ref _lastRaw, value);
        }

        // ===== 명령 =====
        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }
        public ICommand RefreshPortsCommand { get; }

        private void RefreshPorts()
        {
            Ports.Clear();
            foreach (var name in SerialService.GetPortNames())
                Ports.Add(name);

            // 선택된 포트가 사라졌으면 첫 번째로 대체
            if (SelectedPort == null || !Ports.Contains(SelectedPort))
                SelectedPort = Ports.Count > 0 ? Ports[0] : null;
        }

        private void Connect()
        {
            if (SelectedPort == null) return;

            try
            {
                _serial.Open(SelectedPort, 9600);
                IsConnected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"연결 실패: {ex.Message}", "오류",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                IsConnected = false;
            }
        }

        private void Disconnect()
        {
            _serial.Close();
            IsConnected = false;
        }

        // ===== 시리얼 수신 처리 (별도 스레드에서 호출됨) =====
        private void OnLineReceived(string line)
        {
            // 반드시 UI 스레드로 마샬링
            Application.Current?.Dispatcher.Invoke(() =>
            {
                LastRaw = line;

                // "DIST:23.45" 형태 파싱
                if (line.StartsWith("DIST:", StringComparison.OrdinalIgnoreCase))
                {
                    string valuePart = line.Substring("DIST:".Length).Trim();
                    if (double.TryParse(valuePart, NumberStyles.Float,
                                        CultureInfo.InvariantCulture, out double value))
                    {
                        DistanceCm = value;
                        OnPropertyChanged(nameof(DistancePercent));
                    }
                }
            });
        }

        private void OnError(string message)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                LastRaw = $"[오류] {message}";
            });
        }

        public void Dispose() => _serial.Dispose();
    }
}
```

여기서 두 가지가 핵심이다. 첫째, `Dispatcher.Invoke`로 시리얼 스레드의 데이터를 UI 스레드에 안전하게 전달한다. 둘째, `double.TryParse`에 `CultureInfo.InvariantCulture`를 지정해 소수점이 마침표(`.`)로 고정되게 한다. 한국어 로캘 등에서 쉼표를 소수점으로 해석하는 문제를 막아준다.

---

## 8. View (XAML)

### 8.1 MainWindow.xaml

```xml
<Window x:Class="SR04Monitor.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="clr-namespace:SR04Monitor.ViewModels"
        Title="HC-SR04 거리 모니터" Height="420" Width="520"
        Background="#1E1E2E" FontFamily="Segoe UI">

    <Window.DataContext>
        <vm:MainViewModel/>
    </Window.DataContext>

    <Grid Margin="20">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- 상단: 포트 선택 / 연결 버튼 -->
        <StackPanel Grid.Row="0" Orientation="Horizontal" VerticalAlignment="Center">
            <TextBlock Text="포트:" Foreground="#CDD6F4" VerticalAlignment="Center" Margin="0,0,8,0"/>
            <ComboBox Width="120"
                      ItemsSource="{Binding Ports}"
                      SelectedItem="{Binding SelectedPort}"/>
            <Button Content="새로고침" Command="{Binding RefreshPortsCommand}"
                    Margin="8,0,0,0" Padding="10,4"/>
            <Button Content="연결" Command="{Binding ConnectCommand}"
                    Margin="16,0,0,0" Padding="14,4"
                    Background="#A6E3A1" Foreground="#1E1E2E" FontWeight="Bold"/>
            <Button Content="해제" Command="{Binding DisconnectCommand}"
                    Margin="8,0,0,0" Padding="14,4"
                    Background="#F38BA8" Foreground="#1E1E2E" FontWeight="Bold"/>
        </StackPanel>

        <!-- 중앙: 거리 값 크게 표시 -->
        <Border Grid.Row="1" Margin="0,20" CornerRadius="12" Background="#313244">
            <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center">
                <TextBlock Text="측정 거리" Foreground="#A6ADC8"
                           FontSize="16" HorizontalAlignment="Center"/>
                <TextBlock Text="{Binding DistanceText}" Foreground="#89B4FA"
                           FontSize="64" FontWeight="Bold"
                           HorizontalAlignment="Center" Margin="0,4"/>
            </StackPanel>
        </Border>

        <!-- 게이지 (0~400cm) -->
        <StackPanel Grid.Row="2" Margin="0,0,0,12">
            <TextBlock Text="0 ~ 400cm 범위" Foreground="#A6ADC8" FontSize="12" Margin="0,0,0,4"/>
            <ProgressBar Height="18"
             Minimum="0"
             Maximum="100"
             Value="0"
             Foreground="#94E2D5"
             Background="#45475A"/>
        </StackPanel>

        <!-- 하단: 상태 / 원문 -->
        <DockPanel Grid.Row="3">
            <Ellipse Width="12" Height="12" VerticalAlignment="Center" Margin="0,0,8,0">
                <Ellipse.Style>
                    <Style TargetType="Ellipse">
                        <Setter Property="Fill" Value="#F38BA8"/>
                        <Style.Triggers>
                            <DataTrigger Binding="{Binding IsConnected}" Value="True">
                                <Setter Property="Fill" Value="#A6E3A1"/>
                            </DataTrigger>
                        </Style.Triggers>
                    </Style>
                </Ellipse.Style>
            </Ellipse>
            <TextBlock Text="{Binding StatusText}" Foreground="#CDD6F4" VerticalAlignment="Center"/>
            <TextBlock Text="{Binding LastRaw}" Foreground="#6C7086"
                       HorizontalAlignment="Right" VerticalAlignment="Center"/>
        </DockPanel>
    </Grid>
</Window>
```

### 8.2 MainWindow.xaml.cs

코드 비하인드는 거의 비어 있다. 종료 시 포트를 정리하는 정도만 처리한다.

```csharp
using System.Windows;
using SR04Monitor.ViewModels;

namespace SR04Monitor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 창이 닫힐 때 시리얼 포트 정리
            Closed += (_, _) =>
            {
                if (DataContext is MainViewModel vm)
                    vm.Dispose();
            };
        }
    }
}
```

---

## 9. 실행 및 테스트 순서

1. 아두이노에 펌웨어 업로드 → 시리얼 모니터(9600)로 `DIST:xx.xx` 수신 확인 → **모니터 닫기**.
2. WPF 실행 → 상단 콤보박스에서 아두이노가 연결된 COM 포트 선택(예: `COM3`).
3. **연결** 버튼 클릭 → 상태 표시등이 초록색, "연결됨"으로 바뀐다.
4. 센서 앞에 손을 가까이/멀리 → 큰 숫자와 게이지가 실시간으로 변한다.
5. 측정 범위를 벗어나면 "범위 밖"으로 표시된다.

COM 포트 번호는 Windows **장치 관리자 → 포트(COM & LPT)** 에서 확인할 수 있다.

---

## 10. 자주 발생하는 문제 (트러블슈팅)

| 증상 | 원인 | 해결 |
|------|------|------|
| 연결 시 "액세스가 거부되었습니다" | 시리얼 모니터/다른 프로그램이 포트 점유 | Arduino IDE 시리얼 모니터를 닫는다 |
| 값이 안 바뀜 | baudrate 불일치 | 양쪽 모두 9600으로 맞춘다 |
| 숫자가 깨지거나 0만 나옴 | 줄바꿈/인코딩 불일치 | `NewLine="\n"`, ASCII 인코딩 확인 |
| 소수점이 이상하게 파싱됨 | 로캘 문제 | `CultureInfo.InvariantCulture` 사용(이미 적용됨) |
| 값이 -1로 고정 | 센서 결선 오류 또는 측정 범위 밖 | Trig/Echo 핀, 전원(5V) 확인 |
| UI가 멈춤/오류 | 시리얼 스레드에서 UI 직접 접근 | `Dispatcher.Invoke`로 마샬링(이미 적용됨) |

---

## 11. 확장 아이디어

- **이동 평균 필터**: 초음파 값은 튀는 경우가 있으므로, 최근 N개 값의 평균을 내면 표시가 안정된다.
- **임계값 알람**: 일정 거리 이하로 들어오면 배경색을 빨강으로 바꾸거나 소리를 낸다(예: 접근 감지).
- **그래프**: `LiveCharts` 또는 `OxyPlot`으로 거리 변화를 실시간 라인 차트로 그린다.
- **CSV 로깅**: 수신값을 타임스탬프와 함께 파일로 저장해 측정 데이터를 분석한다.
- **다중 센서**: 아두이노에서 `DIST1:..,DIST2:..` 형태로 보내고 WPF에서 분리 파싱한다.

이동 평균 필터를 적용하려면 `MainViewModel`의 `OnLineReceived`에서 값을 큐에 넣고 평균을 계산해 `DistanceCm`에 대입하면 된다. 다음은 간단한 예시다.

```csharp
private readonly Queue<double> _window = new();
private const int WindowSize = 5;

private double Smooth(double value)
{
    if (value < 0) return value; // 범위 밖은 필터링하지 않음
    _window.Enqueue(value);
    if (_window.Count > WindowSize) _window.Dequeue();

    double sum = 0;
    foreach (var v in _window) sum += v;
    return sum / _window.Count;
}
```

`DistanceCm = value;` 대신 `DistanceCm = Smooth(value);`로 바꾸면 표시값이 한결 부드러워진다.
