<img width="2540" height="1403" alt="image" src="https://github.com/user-attachments/assets/37702208-7146-485d-8d7e-a423007c8285" />
```
dotnet add package System.IO.Ports
```


---

# WPF와 Arduino 시리얼 통신 기초 강의안

## 1. 강의 목표

이 강의에서는 **WPF 프로그램에서 버튼을 클릭하면 Arduino의 LED가 켜지고 꺼지는 구조**를 실습한다.

실습을 완료하면 다음 내용을 이해할 수 있다.

| 학습 항목 | 내용 |
|---|---|
| WPF | Windows 데스크톱 프로그램 화면 구성 |
| Arduino | 외부 장치 제어 |
| Serial 통신 | PC와 Arduino 간 USB 통신 |
| C# `SerialPort` | WPF에서 COM 포트로 데이터 전송 |
| 이벤트 처리 | 버튼 클릭 시 Arduino에 명령 전송 |

---

## 2. 전체 동작 구조

```text
사용자가 WPF 버튼 클릭
        ↓
C# 코드에서 SerialPort.Write("1") 또는 SerialPort.Write("0") 실행
        ↓
USB 케이블을 통해 Arduino로 문자 전송
        ↓
Arduino가 Serial.read()로 문자 수신
        ↓
수신 값이 '1'이면 LED ON
수신 값이 '0'이면 LED OFF
```

---

## 3. 준비물

| 준비물 | 설명 |
|---|---|
| Windows PC | WPF 실행용 |
| Visual Studio | WPF 프로젝트 개발용 |
| .NET SDK | WPF 프로젝트 빌드용 |
| Arduino Uno 또는 호환 보드 | LED 제어용 |
| USB 케이블 | PC와 Arduino 연결 |
| Arduino IDE | Arduino 코드 업로드용 |

---

## 4. 사용하는 기술과 이유

### 4.1 WPF를 사용하는 이유

WPF는 Windows 데스크톱 애플리케이션을 만들기 위한 Microsoft의 UI 프레임워크이다.

이번 예제에서는 버튼, 텍스트 표시, 이벤트 처리를 쉽게 구성할 수 있기 때문에 WPF를 사용한다.

### 4.2 Arduino를 사용하는 이유

Arduino는 센서, LED, 모터 같은 외부 장치를 제어하기 쉬운 마이크로컨트롤러 보드이다.

이번 예제에서는 PC 프로그램이 실제 하드웨어를 제어하는 구조를 이해하기 위해 Arduino를 사용한다.

### 4.3 Serial 통신을 사용하는 이유

PC와 Arduino는 USB 케이블로 연결되지만, 내부적으로는 COM 포트를 통한 Serial 통신 방식으로 데이터를 주고받는다.

WPF에서 `"1"` 또는 `"0"`이라는 간단한 문자를 보내고, Arduino가 그 문자를 해석해서 LED를 제어한다.

### 4.4 `System.IO.Ports`를 사용하는 이유

C#에서 COM 포트를 제어하려면 `SerialPort` 클래스를 사용한다.

`SerialPort` 클래스는 `System.IO.Ports` 패키지에 들어 있다.

.NET Core, .NET 6, .NET 7, .NET 8, .NET 10 계열 WPF 프로젝트에서는 기본 포함이 아닐 수 있으므로 별도로 설치해야 한다.

---

## 5. Arduino 코드

### 무엇을 위한 파일인가?

이 파일은 Arduino가 PC에서 들어오는 Serial 명령을 받아 LED를 켜고 끄는 역할을 한다.

- PC에서 `'1'`을 보내면 LED ON
- PC에서 `'0'`을 보내면 LED OFF

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

        // 읽은 문자가 '1'인지 확인합니다.
        if (command == '1')
        {
            // LED 핀에 HIGH 신호를 보내 LED를 켭니다.
            digitalWrite(LED_PIN, HIGH);
        }

        // 읽은 문자가 '0'인지 확인합니다.
        else if (command == '0')
        {
            // LED 핀에 LOW 신호를 보내 LED를 끕니다.
            digitalWrite(LED_PIN, LOW);
        }
    }
}
```

---

## 6. WPF 프로젝트 생성

### 실행 위치

Windows의 **Developer PowerShell**, **CMD**, 또는 **PowerShell**에서 실행한다.

```powershell
dotnet new wpf -n ArduinoWpfLedControl
cd ArduinoWpfLedControl
```

### 명령어 설명

| 명령어 | 설명 |
|---|---|
| `dotnet new wpf -n ArduinoWpfLedControl` | `ArduinoWpfLedControl`이라는 이름의 WPF 프로젝트 생성 |
| `cd ArduinoWpfLedControl` | 생성된 프로젝트 폴더로 이동 |

---

## 7. `System.IO.Ports` 패키지 설치

### 실행 위치

프로젝트 폴더에서 실행한다.

```powershell
dotnet add package System.IO.Ports
```

### 왜 이 명령을 사용하는가?

WPF에서 Arduino와 Serial 통신을 하려면 C#의 `SerialPort` 클래스가 필요하다.

이 클래스는 `System.IO.Ports` 패키지에 들어 있으므로, 프로젝트에 NuGet 패키지로 추가해야 한다.

### 주의할 점

Visual Studio에는 비슷해 보이는 콘솔이 두 종류 있다.

| 콘솔 | 사용할 명령 |
|---|---|
| Developer PowerShell | `dotnet add package System.IO.Ports` |
| NuGet 패키지 관리자 콘솔 | `Install-Package System.IO.Ports` |

Developer PowerShell에서 `Install-Package System.IO.Ports`를 실행하면 PowerShell의 다른 패키지 관리 명령으로 해석될 수 있다.  
따라서 Developer PowerShell에서는 반드시 아래 명령을 사용한다.

```powershell
dotnet add package System.IO.Ports
```

---

## 8. WPF 화면 코드: `MainWindow.xaml`

### 무엇을 위한 파일인가?

이 파일은 WPF 프로그램의 화면을 정의한다.

화면에는 다음 4개 요소가 있다.

| 요소 | 역할 |
|---|---|
| `TextBlock` | 현재 상태 표시 |
| `Arduino 연결` 버튼 | COM 포트 연결 |
| `LED 켜기` 버튼 | Arduino로 `"1"` 전송 |
| `LED 끄기` 버튼 | Arduino로 `"0"` 전송 |

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

## 9. WPF C# 코드: `MainWindow.xaml.cs`

### 무엇을 위한 파일인가?

이 파일은 WPF 화면의 버튼 클릭 동작을 처리한다.

핵심 기능은 다음과 같다.

| 함수 | 역할 |
|---|---|
| `ConnectButton_Click` | Arduino COM 포트 연결 |
| `LedOnButton_Click` | Arduino로 `"1"` 전송 |
| `LedOffButton_Click` | Arduino로 `"0"` 전송 |
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

## 10. COM 포트 확인 방법

Arduino IDE에서 다음 메뉴를 확인한다.

```text
도구 → 포트
```

예를 들어 Arduino가 `COM5`로 표시되면 WPF 코드의 아래 부분을 수정한다.

```csharp
serialPort.PortName = "COM5";
```

기본 예제에서는 아래처럼 되어 있다.

```csharp
serialPort.PortName = "COM3";
```

본인의 PC 환경에 맞게 반드시 수정해야 한다.

---

## 11. 실행 순서

### 11.1 Arduino 코드 업로드

1. Arduino IDE 실행
2. Arduino 보드 연결
3. `ArduinoLedControl.ino` 작성
4. 보드와 포트 선택
5. 업로드 실행

### 11.2 WPF 프로그램 실행

1. Visual Studio에서 WPF 프로젝트 열기
2. `System.IO.Ports` 패키지 설치 확인
3. `MainWindow.xaml` 작성
4. `MainWindow.xaml.cs` 작성
5. Arduino IDE의 Serial Monitor 닫기
6. WPF 실행
7. `Arduino 연결` 버튼 클릭
8. `LED 켜기`, `LED 끄기` 버튼 클릭

---

## 12. 중요한 주의사항

### 12.1 Serial Monitor를 닫아야 하는 이유

Arduino IDE의 Serial Monitor가 열려 있으면 해당 COM 포트를 이미 Arduino IDE가 사용 중인 상태가 된다.

이 경우 WPF 프로그램에서 같은 COM 포트를 열 수 없다.

따라서 WPF 프로그램을 실행하기 전에는 Arduino IDE의 Serial Monitor를 반드시 닫아야 한다.

### 12.2 COM 포트 번호는 PC마다 다르다

강의 예제에서는 `COM3`을 사용하지만, 실제 PC에서는 `COM4`, `COM5`, `COM6` 등으로 다를 수 있다.

반드시 Arduino IDE에서 포트 번호를 확인한 뒤 코드에 반영해야 한다.

### 12.3 BaudRate는 Arduino와 WPF가 같아야 한다

Arduino 코드:

```cpp
Serial.begin(9600);
```

WPF 코드:

```csharp
serialPort.BaudRate = 9600;
```

두 값이 다르면 통신이 정상적으로 되지 않는다.

---

## 13. 자주 발생하는 오류와 해결 방법

### 13.1 `SerialPort`에 빨간 밑줄이 생기는 경우

#### 원인

`System.IO.Ports` 패키지가 설치되지 않았을 가능성이 높다.

#### 해결

프로젝트 폴더에서 아래 명령을 실행한다.

```powershell
dotnet add package System.IO.Ports
```

---

### 13.2 `Install-Package System.IO.Ports`가 실패하는 경우

#### 원인

Developer PowerShell에서 `Install-Package`를 실행했기 때문이다.

Developer PowerShell의 `Install-Package`는 Visual Studio NuGet 패키지 관리자 콘솔의 명령과 다르게 동작할 수 있다.

#### 해결

Developer PowerShell에서는 아래 명령을 사용한다.

```powershell
dotnet add package System.IO.Ports
```

또는 Visual Studio 메뉴에서 다음 경로로 들어간다.

```text
도구 → NuGet 패키지 관리자 → 패키지 관리자 콘솔
```

그 콘솔에서는 아래 명령을 사용할 수 있다.

```powershell
Install-Package System.IO.Ports
```

---

### 13.3 `InitializeComponent()`에 빨간 밑줄이 생기는 경우

#### 원인

`MainWindow.xaml`의 `x:Class`와 `MainWindow.xaml.cs`의 namespace 또는 class 이름이 맞지 않을 수 있다.

#### 확인할 부분

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

두 파일의 이름이 반드시 일치해야 한다.

---

### 13.4 `IComponentConnector.Connect`가 두 번 이상 구현되었다는 오류

#### 원인

같은 `MainWindow.xaml` 또는 `App.xaml`이 프로젝트에 두 번 포함되었을 가능성이 높다.

예를 들어 프로젝트 안에 다시 같은 이름의 프로젝트 폴더가 들어 있으면 문제가 발생할 수 있다.

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

#### 해결

중복된 안쪽 폴더를 프로젝트에서 제외하거나 삭제한다.

그 다음 `bin`, `obj` 폴더를 삭제하고 다시 빌드한다.

```powershell
Remove-Item -Recurse -Force .\bin
Remove-Item -Recurse -Force .\obj
dotnet clean
dotnet build
```

CMD에서는 아래 명령을 사용할 수 있다.

```cmd
rmdir /s /q bin
rmdir /s /q obj
dotnet clean
dotnet build
```

---

### 13.5 COM 포트 연결 실패

#### 원인 후보

| 원인 | 설명 |
|---|---|
| COM 번호 오류 | 코드의 `COM3`이 실제 Arduino 포트와 다름 |
| Serial Monitor 열림 | Arduino IDE가 포트를 이미 사용 중 |
| USB 케이블 문제 | 충전 전용 케이블일 수 있음 |
| 드라이버 문제 | CH340 계열 보드는 드라이버가 필요할 수 있음 |
| 보드 연결 해제 | Arduino가 PC에 연결되어 있지 않음 |

#### 해결

1. Arduino IDE에서 포트 번호 확인
2. Serial Monitor 닫기
3. USB 케이블 교체
4. 장치 관리자에서 COM 포트 확인
5. WPF 코드의 `PortName` 수정

---

## 14. 외부 LED 연결 방법

Arduino 내장 LED 대신 외부 LED를 사용할 수 있다.

```text
Arduino D13 ─ 220Ω 저항 ─ LED 긴 다리(+)
LED 짧은 다리(-) ─ Arduino GND
```

LED는 극성이 있다.

| LED 다리 | 의미 |
|---|---|
| 긴 다리 | + |
| 짧은 다리 | - |

저항 없이 LED를 직접 연결하면 LED 또는 Arduino 핀에 무리가 갈 수 있으므로 220Ω 정도의 저항을 사용하는 것이 안전하다.

---

## 15. 수업용 설명 포인트

### 15.1 가장 중요한 개념

이 예제에서 가장 중요한 것은 **PC 프로그램이 하드웨어를 직접 제어하는 것이 아니라, 명령을 보내고 Arduino가 그 명령을 해석한다는 점**이다.

즉 WPF는 명령을 보내는 역할이고, Arduino는 실제 하드웨어를 제어하는 역할이다.

```text
WPF = 명령을 보내는 쪽
Arduino = 명령을 받아 실제 장치를 제어하는 쪽
```

### 15.2 왜 문자열 `"1"`, `"0"`을 쓰는가?

처음 배우는 단계에서는 가장 단순한 프로토콜이 좋다.

| 전송 문자 | 의미 |
|---|---|
| `"1"` | LED 켜기 |
| `"0"` | LED 끄기 |

실무에서는 다음처럼 더 명확한 명령을 사용하기도 한다.

```text
LED_ON
LED_OFF
MOTOR_START
MOTOR_STOP
```

---

## 16. 확장 아이디어

이 예제를 이해한 뒤에는 다음 기능으로 확장할 수 있다.

| 확장 기능 | 설명 |
|---|---|
| COM 포트 자동 검색 | PC에 연결된 COM 포트 목록을 ComboBox에 표시 |
| 연결 해제 버튼 | Serial 포트 명시적 닫기 |
| Arduino 응답 표시 | Arduino에서 `OK`를 보내고 WPF가 표시 |
| 센서값 읽기 | Arduino의 온도, 조도, 거리 센서값을 WPF에 표시 |
| 모터 제어 | 버튼으로 DC 모터 또는 서보모터 제어 |
| 장비 제어 UI | 실제 자동화 장비의 간단한 HMI 구조로 확장 |

---

## 17. 강의 마무리 요약

이번 실습의 핵심은 다음과 같다.

| 핵심 | 설명 |
|---|---|
| WPF 버튼 | 사용자의 입력을 받음 |
| `SerialPort` | PC에서 Arduino로 데이터 전송 |
| COM 포트 | PC와 Arduino가 연결되는 통신 경로 |
| Arduino `Serial.read()` | PC에서 받은 문자를 읽음 |
| `digitalWrite()` | LED를 실제로 켜거나 끔 |

최종 흐름은 다음과 같다.

```text
버튼 클릭
→ SerialPort.Write("1")
→ Arduino Serial.read()
→ digitalWrite(13, HIGH)
→ LED 켜짐
```

```text
버튼 클릭
→ SerialPort.Write("0")
→ Arduino Serial.read()
→ digitalWrite(13, LOW)
→ LED 꺼짐
```

---

## 18. 실습 체크리스트

| 확인 항목 | 완료 |
|---|---|
| Arduino 코드 업로드 완료 | □ |
| Arduino IDE Serial Monitor 닫음 | □ |
| WPF 프로젝트 생성 완료 | □ |
| `System.IO.Ports` 패키지 설치 완료 | □ |
| COM 포트 번호 확인 완료 | □ |
| WPF 코드의 `PortName` 수정 완료 | □ |
| WPF 실행 완료 | □ |
| Arduino 연결 버튼 동작 확인 | □ |
| LED 켜기 버튼 동작 확인 | □ |
| LED 끄기 버튼 동작 확인 | □ |

---

## 19. 참고용 최종 파일 구조

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

Arduino 스케치 파일은 별도 폴더에 둘 수 있다.

```text
ArduinoLedControl
└─ ArduinoLedControl.ino
```

---

## 20. 강사용 한 줄 설명

> 이 예제는 WPF가 Serial 통신으로 Arduino에 간단한 문자 명령을 보내고, Arduino가 그 명령을 해석하여 LED를 제어하는 가장 기본적인 PC-하드웨어 연동 실습이다.
