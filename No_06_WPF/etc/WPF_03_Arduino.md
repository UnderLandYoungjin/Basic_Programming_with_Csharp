<img width="2540" height="1403" alt="image" src="https://github.com/user-attachments/assets/37702208-7146-485d-8d7e-a423007c8285" />
```
dotnet add package System.IO.Ports
```


---

# WPF와 Arduino 시리얼 통신 기초

## 1. 이번 실습에서 할 내용

WPF 프로그램에서 버튼을 누르면 Arduino의 LED가 켜지고 꺼지도록 만든다.

실습에서 확인할 흐름은 다음과 같다.

```text
WPF 버튼 클릭
→ PC에서 Arduino로 값 전송
→ Arduino가 값 확인
→ LED ON / OFF
```

---

## 2. 전체 구조

WPF와 Arduino는 USB 케이블로 연결되어 있지만, 실제 통신은 COM 포트를 통한 Serial 통신으로 이루어진다.

```text
WPF 프로그램
    ↓
SerialPort.Write("1")
또는
SerialPort.Write("0")
    ↓
USB Serial 통신
    ↓
Arduino Serial.read()
    ↓
digitalWrite()
    ↓
LED 제어
```

WPF는 명령을 보내는 쪽이고, Arduino는 받은 명령에 따라 실제 LED를 제어하는 쪽이다.

---

## 3. 준비물

| 준비물 | 설명 |
|---|---|
| Windows PC | WPF 프로그램 실행 |
| Visual Studio | WPF 프로젝트 작성 |
| .NET SDK | WPF 프로젝트 생성 및 빌드 |
| Arduino Uno 또는 호환 보드 | LED 제어 |
| USB 케이블 | PC와 Arduino 연결 |
| Arduino IDE | Arduino 코드 업로드 |

---

## 4. 사용하는 기술

### WPF

WPF는 Windows 데스크톱 프로그램을 만들기 위한 UI 프레임워크이다.

이번 실습에서는 버튼과 상태 표시 문구를 가진 간단한 프로그램을 만든다.

### Arduino

Arduino는 외부 장치를 제어하기 위한 마이크로컨트롤러 보드이다.

이번 실습에서는 Arduino의 13번 핀에 연결된 LED를 제어한다. Arduino Uno 기준으로 13번 핀은 보드 내장 LED와 연결되어 있어 별도 LED 없이도 테스트할 수 있다.

### Serial 통신

PC와 Arduino는 USB로 연결되지만, 프로그램에서는 COM 포트를 통해 데이터를 주고받는다.

이번 실습에서는 WPF에서 Arduino로 문자 하나를 보낸다.

| WPF에서 보내는 값 | Arduino 동작 |
|---|---|
| `"1"` | LED 켜기 |
| `"0"` | LED 끄기 |

### `System.IO.Ports`

C#에서 COM 포트를 사용하려면 `SerialPort` 클래스를 사용한다.

`SerialPort`는 `System.IO.Ports` 패키지에 들어 있으므로, 프로젝트에 패키지를 추가해야 한다.

---

## 5. Arduino 코드

### 파일 역할

PC에서 들어오는 Serial 값을 읽고, 값에 따라 LED를 켜거나 끈다.

```cpp
// 파일 경로: ArduinoLedControl/ArduinoLedControl.ino

// Arduino 보드에서 사용할 LED 핀 번호를 저장합니다.
const int LED_PIN = 13;

// setup 함수는 Arduino가 켜지거나 리셋될 때 한 번만 실행됩니다.
void setup()
{
    // LED_PIN으로 지정한 핀을 출력 모드로 설정합니다.
    pinMode(LED_PIN, OUTPUT);

    // PC와 Arduino 사이의 Serial 통신 속도를 9600bps로 설정합니다.
    Serial.begin(9600);
}

// loop 함수는 Arduino가 켜져 있는 동안 계속 반복 실행됩니다.
void loop()
{
    // PC에서 Arduino로 들어온 Serial 데이터가 있는지 확인합니다.
    if (Serial.available() > 0)
    {
        // Serial 통신으로 들어온 문자 하나를 읽습니다.
        char command = Serial.read();

        // 읽은 문자가 '1'이면 LED를 켭니다.
        if (command == '1')
        {
            // LED 핀에 HIGH 신호를 보내 LED를 켭니다.
            digitalWrite(LED_PIN, HIGH);
        }

        // 읽은 문자가 '0'이면 LED를 끕니다.
        else if (command == '0')
        {
            // LED 핀에 LOW 신호를 보내 LED를 끕니다.
            digitalWrite(LED_PIN, LOW);
        }
    }
}
```

---

## 6. WPF 프로젝트 만들기

### 실행 위치

Windows의 **CMD**, **PowerShell**, 또는 **Developer PowerShell**에서 실행한다.

```powershell
dotnet new wpf -n ArduinoWpfLedControl
cd ArduinoWpfLedControl
```

### 명령어 설명

| 명령어 | 설명 |
|---|---|
| `dotnet new wpf -n ArduinoWpfLedControl` | WPF 프로젝트 생성 |
| `cd ArduinoWpfLedControl` | 생성된 프로젝트 폴더로 이동 |

---

## 7. `System.IO.Ports` 패키지 추가

### 실행 위치

프로젝트 폴더에서 실행한다.

```powershell
dotnet add package System.IO.Ports
```

이 패키지를 추가해야 C# 코드에서 아래 기능을 사용할 수 있다.

```csharp
using System.IO.Ports;
```

```csharp
SerialPort serialPort = new SerialPort();
```

### 콘솔 종류에 따른 명령어 차이

Visual Studio 관련 콘솔은 헷갈릴 수 있다.

| 실행 위치 | 명령 |
|---|---|
| CMD / PowerShell / Developer PowerShell | `dotnet add package System.IO.Ports` |
| Visual Studio NuGet 패키지 관리자 콘솔 | `Install-Package System.IO.Ports` |

일반 PowerShell이나 Developer PowerShell에서는 `dotnet add package System.IO.Ports`를 쓰는 것이 안전하다.

---

## 8. WPF 화면 코드

### 파일 역할

버튼과 상태 표시 문구를 배치한다.

| 화면 요소 | 역할 |
|---|---|
| 상태 표시 문구 | Arduino 연결 상태와 LED 상태 표시 |
| Arduino 연결 버튼 | COM 포트 열기 |
| LED 켜기 버튼 | Arduino로 `"1"` 전송 |
| LED 끄기 버튼 | Arduino로 `"0"` 전송 |

```xml
<!-- 파일 경로: ArduinoWpfLedControl/MainWindow.xaml -->

<Window x:Class="ArduinoWpfLedControl.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Arduino WPF LED Control"
        Height="300"
        Width="400">

    <Grid>

        <StackPanel VerticalAlignment="Center"
                    HorizontalAlignment="Center"
                    Width="250">

            <TextBlock x:Name="StatusTextBlock"
                       Text="Arduino 연결 안 됨"
                       FontSize="18"
                       TextAlignment="Center"
                       Margin="0,0,0,20"/>

            <Button Content="Arduino 연결"
                    Height="40"
                    Margin="0,0,0,10"
                    Click="ConnectButton_Click"/>

            <Button Content="LED 켜기"
                    Height="40"
                    Margin="0,0,0,10"
                    Click="LedOnButton_Click"/>

            <Button Content="LED 끄기"
                    Height="40"
                    Click="LedOffButton_Click"/>

        </StackPanel>

    </Grid>
</Window>
```

---

## 9. WPF C# 코드

### 파일 역할

버튼을 눌렀을 때 실행될 동작을 작성한다.

| 함수 | 동작 |
|---|---|
| `ConnectButton_Click` | Arduino와 연결 |
| `LedOnButton_Click` | LED 켜기 명령 전송 |
| `LedOffButton_Click` | LED 끄기 명령 전송 |
| `OnClosed` | 프로그램 종료 시 Serial 포트 닫기 |

```csharp
// 파일 경로: ArduinoWpfLedControl/MainWindow.xaml.cs

// Arduino와 Serial 통신을 하기 위해 필요한 네임스페이스입니다.
using System.IO.Ports;

// WPF의 Window, MessageBox, RoutedEventArgs 등을 사용하기 위한 네임스페이스입니다.
using System.Windows;

// 이 파일이 속한 프로젝트의 네임스페이스입니다.
namespace ArduinoWpfLedControl
{
    // MainWindow 클래스는 WPF의 메인 창을 의미합니다.
    public partial class MainWindow : Window
    {
        // Arduino와 통신할 SerialPort 객체를 저장할 변수입니다.
        private SerialPort? serialPort;

        // MainWindow 생성자는 프로그램 창이 만들어질 때 실행됩니다.
        public MainWindow()
        {
            // MainWindow.xaml에 작성한 화면 요소들을 초기화합니다.
            InitializeComponent();
        }

        // Arduino 연결 버튼을 클릭했을 때 실행되는 함수입니다.
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            // Serial 포트 연결 과정에서 오류가 날 수 있으므로 try-catch를 사용합니다.
            try
            {
                // SerialPort 객체를 새로 생성합니다.
                serialPort = new SerialPort();

                // Arduino가 연결된 COM 포트 이름을 지정합니다.
                serialPort.PortName = "COM3";

                // Arduino 코드의 Serial.begin(9600)과 같은 통신 속도를 지정합니다.
                serialPort.BaudRate = 9600;

                // 한 번에 전송되는 데이터 비트 수를 8비트로 설정합니다.
                serialPort.DataBits = 8;

                // 오류 검사용 패리티 비트를 사용하지 않도록 설정합니다.
                serialPort.Parity = Parity.None;

                // 정지 비트를 1비트로 설정합니다.
                serialPort.StopBits = StopBits.One;

                // 설정한 Serial 포트를 엽니다.
                serialPort.Open();

                // 화면의 상태 문구를 연결 성공으로 변경합니다.
                StatusTextBlock.Text = "Arduino 연결됨";
            }
            catch
            {
                // 연결 실패 시 사용자에게 오류 메시지를 표시합니다.
                MessageBox.Show("Arduino 연결에 실패했습니다. COM 포트를 확인하세요.");

                // 화면의 상태 문구를 연결 실패로 변경합니다.
                StatusTextBlock.Text = "Arduino 연결 실패";
            }
        }

        // LED 켜기 버튼을 클릭했을 때 실행되는 함수입니다.
        private void LedOnButton_Click(object sender, RoutedEventArgs e)
        {
            // serialPort 객체가 존재하고 포트가 열려 있는지 확인합니다.
            if (serialPort != null && serialPort.IsOpen)
            {
                // Arduino로 문자 "1"을 전송합니다.
                serialPort.Write("1");

                // 화면의 상태 문구를 LED 켜짐으로 변경합니다.
                StatusTextBlock.Text = "LED 켜짐";
            }
            else
            {
                // Arduino가 연결되지 않았을 때 사용자에게 안내 메시지를 표시합니다.
                MessageBox.Show("먼저 Arduino를 연결하세요.");
            }
        }

        // LED 끄기 버튼을 클릭했을 때 실행되는 함수입니다.
        private void LedOffButton_Click(object sender, RoutedEventArgs e)
        {
            // serialPort 객체가 존재하고 포트가 열려 있는지 확인합니다.
            if (serialPort != null && serialPort.IsOpen)
            {
                // Arduino로 문자 "0"을 전송합니다.
                serialPort.Write("0");

                // 화면의 상태 문구를 LED 꺼짐으로 변경합니다.
                StatusTextBlock.Text = "LED 꺼짐";
            }
            else
            {
                // Arduino가 연결되지 않았을 때 사용자에게 안내 메시지를 표시합니다.
                MessageBox.Show("먼저 Arduino를 연결하세요.");
            }
        }

        // WPF 창이 닫힐 때 실행되는 함수입니다.
        protected override void OnClosed(System.EventArgs e)
        {
            // serialPort 객체가 존재하고 포트가 열려 있는지 확인합니다.
            if (serialPort != null && serialPort.IsOpen)
            {
                // 열려 있는 Serial 포트를 닫습니다.
                serialPort.Close();
            }

            // WPF의 기본 창 닫기 처리를 실행합니다.
            base.OnClosed(e);
        }
    }
}
```

---

## 10. COM 포트 확인

Arduino IDE에서 다음 메뉴를 확인한다.

```text
도구 → 포트
```

예를 들어 Arduino가 `COM5`로 잡혀 있으면 WPF 코드도 아래처럼 수정한다.

```csharp
serialPort.PortName = "COM5";
```

예제 코드에는 `COM3`으로 되어 있다.

```csharp
serialPort.PortName = "COM3";
```

본인 PC에서 잡힌 포트 번호로 바꿔야 한다.

---

## 11. 실행 순서

### Arduino 쪽

1. Arduino IDE 실행
2. Arduino 보드 연결
3. `ArduinoLedControl.ino` 작성
4. 보드와 포트 선택
5. 업로드 실행

### WPF 쪽

1. WPF 프로젝트 생성
2. `System.IO.Ports` 패키지 추가
3. `MainWindow.xaml` 작성
4. `MainWindow.xaml.cs` 작성
5. COM 포트 번호 확인
6. Arduino IDE의 Serial Monitor 닫기
7. WPF 실행
8. `Arduino 연결` 버튼 클릭
9. `LED 켜기`, `LED 끄기` 버튼 클릭

---

## 12. 주의할 점

### Serial Monitor는 닫아야 한다

Arduino IDE의 Serial Monitor가 열려 있으면 해당 COM 포트를 Arduino IDE가 이미 사용 중인 상태가 된다.

그러면 WPF 프로그램에서 같은 COM 포트를 열 수 없다.

WPF 실행 전에는 Arduino IDE의 Serial Monitor를 닫는다.

### BaudRate를 맞춰야 한다

Arduino 코드:

```cpp
Serial.begin(9600);
```

WPF 코드:

```csharp
serialPort.BaudRate = 9600;
```

두 값이 같아야 정상적으로 통신할 수 있다.

### COM 포트 번호는 PC마다 다르다

강의실 PC마다 Arduino가 잡히는 COM 번호가 다를 수 있다.

`COM3`으로 되지 않으면 Arduino IDE 또는 장치 관리자에서 포트 번호를 확인한다.

---

## 13. 자주 발생하는 오류

### `SerialPort`에 빨간 밑줄이 생기는 경우

`System.IO.Ports` 패키지가 설치되지 않았을 가능성이 높다.

프로젝트 폴더에서 실행한다.

```powershell
dotnet add package System.IO.Ports
```

---

### `Install-Package System.IO.Ports`가 실패하는 경우

Developer PowerShell에서 `Install-Package`를 실행하면 의도한 NuGet 설치 명령으로 동작하지 않을 수 있다.

Developer PowerShell에서는 아래 명령을 사용한다.

```powershell
dotnet add package System.IO.Ports
```

Visual Studio의 NuGet 패키지 관리자 콘솔에서는 아래 명령을 사용할 수 있다.

```powershell
Install-Package System.IO.Ports
```

---

### `InitializeComponent()`에 빨간 밑줄이 생기는 경우

`MainWindow.xaml`의 `x:Class`와 `MainWindow.xaml.cs`의 namespace/class 이름이 맞지 않을 수 있다.

`MainWindow.xaml`:

```xml
<Window x:Class="ArduinoWpfLedControl.MainWindow"
```

`MainWindow.xaml.cs`:

```csharp
namespace ArduinoWpfLedControl
{
    public partial class MainWindow : Window
```

프로젝트 이름을 다르게 만들었다면 `ArduinoWpfLedControl` 부분도 프로젝트 이름에 맞게 수정해야 한다.

---

### `IComponentConnector.Connect`가 두 번 이상 구현되었다는 오류

같은 `MainWindow.xaml` 또는 `App.xaml`이 프로젝트에 두 번 포함된 경우에 발생할 수 있다.

대표적으로 프로젝트 폴더 안에 같은 프로젝트 폴더가 한 번 더 들어간 경우다.

문제 구조 예시:

```text
ArduinoWpfLedControl
├─ MainWindow.xaml
├─ MainWindow.xaml.cs
├─ App.xaml
├─ App.xaml.cs
├─ ArduinoWpfLedControl.csproj
├─ ArduinoWpfLedControl
│  ├─ MainWindow.xaml
│  ├─ MainWindow.xaml.cs
│  ├─ App.xaml
│  ├─ App.xaml.cs
│  └─ ArduinoWpfLedControl.csproj
```

중복된 안쪽 폴더를 삭제하거나 프로젝트에서 제외한 뒤, `bin`, `obj` 폴더를 지우고 다시 빌드한다.

PowerShell:

```powershell
Remove-Item -Recurse -Force .\bin
Remove-Item -Recurse -Force .\obj
dotnet clean
dotnet build
```

CMD:

```cmd
rmdir /s /q bin
rmdir /s /q obj
dotnet clean
dotnet build
```

---

### Arduino 연결 실패

다음 항목을 확인한다.

| 확인 항목 | 설명 |
|---|---|
| COM 번호 | 코드의 `COM3`이 실제 Arduino 포트와 같은지 확인 |
| Serial Monitor | Arduino IDE의 Serial Monitor가 열려 있는지 확인 |
| USB 케이블 | 충전 전용 케이블인지 확인 |
| 드라이버 | CH340 계열 보드는 드라이버가 필요할 수 있음 |
| 보드 선택 | Arduino IDE에서 보드 종류가 맞는지 확인 |

---

## 14. 외부 LED를 사용하는 경우

Arduino 내장 LED 대신 외부 LED를 사용할 수도 있다.

```text
Arduino D13 ─ 220Ω 저항 ─ LED 긴 다리(+)
LED 짧은 다리(-) ─ Arduino GND
```

LED는 방향이 있다.

| LED 다리 | 연결 |
|---|---|
| 긴 다리 | + 쪽 |
| 짧은 다리 | GND 쪽 |

저항 없이 LED를 직접 연결하는 것은 피한다.

---

## 15. 실습 중 설명할 내용

이번 구조에서 중요한 점은 WPF가 LED를 직접 켜는 것이 아니라는 점이다.

WPF는 Arduino에 값을 보낸다.

Arduino는 그 값을 읽고 LED 핀을 제어한다.

```text
WPF: "1" 보냄
Arduino: '1'을 읽음
Arduino: LED 핀 HIGH
LED: 켜짐
```

```text
WPF: "0" 보냄
Arduino: '0'을 읽음
Arduino: LED 핀 LOW
LED: 꺼짐
```

처음에는 `"1"`, `"0"`처럼 단순한 값을 사용하지만, 나중에는 아래처럼 명령어를 더 명확하게 만들 수 있다.

```text
LED_ON
LED_OFF
MOTOR_START
MOTOR_STOP
```

---

## 16. 확장 방향

기본 동작이 되면 다음 기능을 추가할 수 있다.

| 확장 기능 | 설명 |
|---|---|
| COM 포트 목록 표시 | PC에 연결된 포트를 ComboBox에 표시 |
| 연결 해제 버튼 | 사용자가 직접 포트를 닫을 수 있게 구성 |
| Arduino 응답 받기 | Arduino에서 `OK`를 보내고 WPF에서 표시 |
| 센서값 표시 | Arduino 센서값을 WPF 화면에 표시 |
| 모터 제어 | 버튼으로 모터 시작/정지 제어 |
| 간단한 장비 제어 화면 | 실제 HMI 형태로 확장 |

---

## 17. 실습 체크리스트

| 확인 항목 | 완료 |
|---|---|
| Arduino 코드 업로드 | □ |
| Arduino IDE Serial Monitor 닫기 | □ |
| WPF 프로젝트 생성 | □ |
| `System.IO.Ports` 패키지 설치 | □ |
| COM 포트 번호 확인 | □ |
| WPF 코드의 `PortName` 수정 | □ |
| WPF 실행 | □ |
| Arduino 연결 버튼 확인 | □ |
| LED 켜기 버튼 확인 | □ |
| LED 끄기 버튼 확인 | □ |

---

## 18. 파일 구조

WPF 프로젝트 폴더 예시:

```text
ArduinoWpfLedControl
├─ ArduinoWpfLedControl.csproj
├─ App.xaml
├─ App.xaml.cs
├─ MainWindow.xaml
├─ MainWindow.xaml.cs
├─ bin
└─ obj
```

Arduino 스케치 폴더 예시:

```text
ArduinoLedControl
└─ ArduinoLedControl.ino
```

---

## 19. 정리

이번 실습에서는 WPF에서 버튼을 누르면 Arduino로 값이 전송되고, Arduino는 받은 값에 따라 LED를 켜고 끄도록 구성했다.

흐름은 단순하다.

```text
버튼 클릭
→ 값 전송
→ Arduino 수신
→ LED 제어
```

WPF와 Arduino를 연결할 때는 COM 포트 번호, BaudRate, Serial Monitor 점유 여부를 먼저 확인해야 한다.
