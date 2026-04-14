# 개발환경 구축 + WPF 구조 이해 (XAML, 이벤트 기반)

## 📌 개요
이번 강의에서는 C# WPF 개발을 위한 **개발환경을 구축**하고,
WPF 프로그램의 핵심 구조인 **XAML + 이벤트 기반 동작 방식**을 이해합니다.
이 강의의 목표는 단순 실행이 아니라
👉 **"WPF 프로그램이 어떻게 구성되는지 구조적으로 이해하는 것"** 입니다.

---

## 💻 개발환경 구축

### 1️⃣ 필수 설치 프로그램

| 프로그램 | 다운로드 경로 | 비고 |
|---|---|---|
| Visual Studio 2022 | https://visualstudio.microsoft.com | Community(무료) 선택 |
| .NET 8 SDK | VS 설치 시 자동 포함 | 별도 설치 불필요 |

---

### 2️⃣ Visual Studio 설치 시 체크 항목

설치 중 **워크로드 선택** 화면에서 반드시 체크합니다.

- ✔️ **.NET 데스크톱 개발**
- ✔️ WPF 관련 구성 요소 포함 (자동 선택됨)

> 💡 이 항목을 체크하지 않으면 WPF 프로젝트 템플릿이 목록에 나타나지 않습니다.

---

### 3️⃣ 프로젝트 생성

1. Visual Studio 실행
2. **"새 프로젝트 만들기"** 클릭
3. 검색창에 `WPF` 입력 → **WPF 앱 (.NET)** 선택
4. 프로젝트 이름 입력 (예: `WpfHelloApp`)
5. 프레임워크 **.NET 8** 선택 후 **만들기**

---

## 📁 WPF 프로젝트 구조

프로젝트를 생성하면 아래 파일들이 자동으로 만들어집니다.

```
WpfHelloApp/
├── App.xaml              ← 프로그램 시작 설정 (시작 창 지정)
├── App.xaml.cs           ← App 코드비하인드
├── MainWindow.xaml       ← 화면(UI) 정의
└── MainWindow.xaml.cs    ← 동작(로직) 코드
```

| 파일 | 설명 |
|---|---|
| `App.xaml` | 프로그램 시작 설정, 전역 리소스 정의 |
| `MainWindow.xaml` | 화면(UI) 정의 — 버튼, 텍스트 등 배치 |
| `MainWindow.xaml.cs` | 동작(로직) 코드 — 이벤트 처리 담당 |

---

## 💻 첫 번째 WPF 프로그램

### MainWindow.xaml

```xml
<!-- Window: 이 파일이 하나의 '창'임을 선언 -->
<!-- x:Class: 이 XAML과 짝을 이루는 C# 클래스 이름 지정 -->
<!-- xmlns: WPF 기본 컨트롤(Button, Grid 등)을 쓰기 위한 네임스페이스 -->
<!-- xmlns:x: x:Class, x:Name 같은 XAML 전용 키워드를 쓰기 위한 네임스페이스 -->
<!-- Title: 창 상단 제목 표시줄에 나타나는 텍스트 -->
<!-- Height / Width: 창의 세로 / 가로 크기 (단위: 픽셀) -->
<Window x:Class="WpfHelloApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Hello WPF"
        Height="200"
        Width="300">

    <!-- Grid: 창 내부를 채우는 기본 레이아웃 컨테이너 -->
    <Grid>

        <!-- Button: 클릭 가능한 버튼 컨트롤 -->
        <!-- Name="btnHello": C# 코드에서 이 버튼을 btnHello 라는 이름으로 접근 -->
        <!-- Content="클릭하세요": 버튼 위에 표시될 텍스트 -->
        <!-- Width / Height: 버튼의 가로 / 세로 크기 -->
        <!-- Click="btnHello_Click": 버튼 클릭 시 실행할 C# 메서드 이름 연결 -->
        <Button Name="btnHello"
                Content="클릭하세요"
                Width="120"
                Height="40"
                Click="btnHello_Click"/>

    </Grid>

</Window>
```

---

### MainWindow.xaml.cs

```csharp
using System;                       // 기본 시스템 기능 (Console, Exception 등) 사용 선언
using System.Windows;               // WPF의 Window, MessageBox 등을 사용하기 위한 선언

namespace WpfHelloApp               // 프로젝트 이름과 동일한 네임스페이스 (XAML x:Class와 일치)
{
    // partial: XAML 자동생성 코드와 이 파일이 합쳐져 하나의 클래스를 이룸
    // Window 상속: 창(Window) 기능을 모두 물려받음
    public partial class MainWindow : Window
    {
        // 생성자: 창이 처음 만들어질 때 한 번 자동 실행됨
        public MainWindow()
        {
            InitializeComponent();  // XAML에 선언된 모든 UI 요소를 메모리에 생성 (반드시 필요!)
        }

        // 버튼 클릭 이벤트 핸들러 — XAML의 Click="btnHello_Click"과 이름이 반드시 일치해야 함
        // sender: 이벤트를 발생시킨 컨트롤 객체 (여기서는 btnHello 버튼)
        // e     : 이벤트 관련 추가 정보를 담는 객체
        private void btnHello_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hello, WPF!");  // 화면에 팝업 메시지 창을 띄움
        }
    }
}
```

---

### ▶️ 실행 결과

**실행 직후** — 버튼이 있는 빈 창이 열림

```
┌──────────────────────────────┐
│  Hello WPF             - □ X │
├──────────────────────────────┤
│                              │
│       ┌──────────────┐       │
│       │  클릭하세요   │       │
│       └──────────────┘       │
│                              │
└──────────────────────────────┘
```

**버튼 클릭 후** — MessageBox 팝업이 나타남

```
┌──────────────────────────────┐
│  Hello WPF             - □ X │
├──────────────────────────────┤
│    ┌─────────────────────┐   │
│    │  ℹ️               X │   │
│    │                     │   │
│    │    Hello, WPF!      │   │
│    │                     │   │
│    │       [ 확인 ]      │   │
│    └─────────────────────┘   │
└──────────────────────────────┘
```

---

## 🔍 구조 설명

| 구성 요소 | 설명 |
|---|---|
| `XAML` | UI를 정의하는 마크업 언어 — 화면 구조를 코드 없이 선언적으로 표현 |
| `Button` | 화면에 표시되는 클릭 가능한 버튼 컨트롤 |
| `Click="btnHello_Click"` | 버튼 클릭 시 실행될 C# 메서드 이름 연결 |
| `InitializeComponent()` | XAML에 선언된 컨트롤들을 실제 메모리 객체로 생성 |
| `MessageBox.Show()` | 화면에 팝업 메시지 창 출력 |

---

## 🧠 핵심 개념

### 1️⃣ XAML (UI 정의)

화면을 코드가 아닌 **구조적(선언적)** 으로 표현하는 마크업 언어입니다.

```xml
<!-- 버튼 하나를 선언하는 것만으로 화면에 버튼이 나타남 -->
<Button Content="클릭하세요" Width="120" Height="40"/>
```

### 2️⃣ Code-Behind (동작)

XAML과 짝을 이루는 C# 파일로, **이벤트 처리 및 로직**을 담당합니다.

```csharp
// 버튼이 클릭될 때 실행할 동작을 C#으로 작성
private void btnHello_Click(object sender, RoutedEventArgs e)
{
    MessageBox.Show("Hello, WPF!");
}
```

### 3️⃣ 이벤트 기반 구조

WPF는 **사용자의 행동**에 반응하는 이벤트 기반으로 동작합니다.

```
👤 사용자 행동       →   🖱️ 버튼 클릭
        ↓
📢 이벤트 발생       →   Click 이벤트 트리거
        ↓
⚙️ 코드 실행         →   btnHello_Click() 메서드 실행
        ↓
📦 결과 출력         →   MessageBox.Show("Hello, WPF!")
```

---

## 📝 핵심 포인트

- WPF는 **XAML(화면) + C#(로직)** 의 이중 구조로 구성됩니다.
- **UI와 로직이 분리**되어 있어 유지보수가 쉽습니다.
- 프로그램은 **이벤트 기반**으로 동작합니다 — 사용자 행동 → 이벤트 → 코드 실행.
- `InitializeComponent()`는 생성자에서 **반드시** 호출해야 합니다.
- XAML의 `Click="메서드명"`과 C#의 메서드 이름이 **정확히 일치**해야 합니다.

---

## 🧪 예제

### 예제 1 — 버튼 텍스트 변경

`Content` 속성을 바꾸면 버튼에 표시되는 텍스트가 달라집니다.

```xml
<!-- Content 속성값을 원하는 텍스트로 변경 -->
<Button Content="시작하기"
        Width="120"
        Height="40"/>
```

**실행 결과**
```
┌──────────────────┐
│                  │
│  ┌────────────┐  │
│  │  시작하기  │  │
│  └────────────┘  │
│                  │
└──────────────────┘
```

---

### 예제 2 — 창 제목 변경

`Title` 속성을 바꾸면 창 상단 제목 표시줄이 바뀝니다.

```xml
<!-- Title 속성값을 원하는 제목으로 변경 -->
<Window ...
        Title="내 첫 번째 프로그램"
        Height="200"
        Width="300">
```

**실행 결과**
```
┌──────────────────────────────────┐
│  내 첫 번째 프로그램        - □ X │
├──────────────────────────────────┤
│                                  │
└──────────────────────────────────┘
```

---

### 예제 3 — 버튼 여러 개 배치

`StackPanel`을 사용하면 컨트롤을 세로로 쌓아서 배치할 수 있습니다.

```xml
<!-- StackPanel: 자식 요소들을 위에서 아래로 순서대로 쌓아서 배치 -->
<StackPanel VerticalAlignment="Center"
            HorizontalAlignment="Center">

    <!-- Margin="0,0,0,5": 버튼 아래 5px 여백 -->
    <Button Content="1번 버튼" Width="120" Height="35" Margin="0,0,0,5"/>

    <!-- 두 번째 버튼: 첫 번째 버튼 아래에 자동 배치됨 -->
    <Button Content="2번 버튼" Width="120" Height="35"/>

</StackPanel>
```

**실행 결과**
```
┌──────────────────────┐
│                      │
│    ┌────────────┐    │
│    │  1번 버튼  │    │
│    └────────────┘    │
│    ┌────────────┐    │
│    │  2번 버튼  │    │
│    └────────────┘    │
│                      │
└──────────────────────┘
```

---

## 📝 문제

---

### 문제 1

WPF에서 UI(화면)를 정의하는 파일의 확장자는 무엇인가요?

**정답:**
<details>
<summary>정답 보기 (클릭)</summary>

`.xaml`입니다. (예: `MainWindow.xaml`)
화면에 배치할 버튼, 텍스트, 레이아웃 등을 XAML 문법으로 선언합니다.

</details>

---

### 문제 2

다음 코드에서 버튼 클릭 시 실행될 C# 메서드 이름은 무엇인가요?

```xml
<Button Content="확인" Click="OkButton_Click"/>
```

**정답:**
<details>
<summary>정답 보기 (클릭)</summary>

`OkButton_Click`입니다.
XAML의 `Click="OkButton_Click"`이 C# 코드비하인드의 아래 메서드와 연결됩니다.

```csharp
private void OkButton_Click(object sender, RoutedEventArgs e)
{
    // 버튼 클릭 시 실행할 코드 작성
}
```

</details>

---

### 문제 3

`InitializeComponent()`의 역할은 무엇인가요?

**정답:**
<details>
<summary>정답 보기 (클릭)</summary>

XAML에 선언된 모든 UI 컨트롤(Button, TextBlock 등)을 **메모리 객체로 실제 생성**하는 메서드입니다.
생성자에서 이 메서드를 호출하지 않으면 컨트롤이 생성되지 않아 **NullReferenceException** 오류가 발생합니다.

```csharp
public MainWindow()
{
    InitializeComponent();  // 이 줄이 없으면 XAML의 컨트롤에 접근 불가
}
```

</details>

---

### 문제 4

WPF의 이벤트 기반 구조를 순서대로 설명하세요.

**정답:**
<details>
<summary>정답 보기 (클릭)</summary>

```
① 사용자가 버튼을 클릭 (사용자 행동)
        ↓
② Click 이벤트 발생
        ↓
③ XAML의 Click="btnHello_Click"으로 연결된 C# 메서드 탐색
        ↓
④ btnHello_Click() 메서드 실행
        ↓
⑤ MessageBox.Show() 등 결과 출력
```

</details>

---

### 문제 5

`MessageBox.Show()`를 호출하면 화면에 어떤 결과가 나타나나요?
또한 아래 빈칸을 채워 `"WPF 완료!"` 메시지를 출력하도록 코드를 완성하세요.

```csharp
private void btnDone_Click(object sender, RoutedEventArgs e)
{
    MessageBox.________("WPF 완료!");
}
```

**정답:**
<details>
<summary>정답 보기 (클릭)</summary>

`MessageBox.Show()`는 화면 중앙에 **팝업 메시지 창**을 띄웁니다.
확인 버튼을 누르기 전까지 다른 조작이 차단됩니다.

```csharp
private void btnDone_Click(object sender, RoutedEventArgs e)
{
    MessageBox.Show("WPF 완료!");  // 팝업 메시지 창에 "WPF 완료!" 출력
}
```

**실행 결과**
```
┌─────────────────────┐
│  ℹ️               X │
│                     │
│     WPF 완료!       │
│                     │
│      [ 확인 ]       │
└─────────────────────┘
```

</details>

---

> 📌 **Tip:** WPF는 **UI + 데이터 + 이벤트 흐름**을 설계하는 기술입니다.
> XAML에서 이벤트 핸들러 이름 입력 후 **Tab 키**를 누르면 Visual Studio가 C# 메서드를 자동으로 생성해줍니다!
