# C# WPF 제1강 — Hello World와 변수

## 개요
이번 강의에서는 WPF 프로그램의 **기본 구조**를 이해하고,
첫 프로그램 **Hello World**를 만든 다음,
**변수(Variable)** 를 사용해서 값을 저장하고 출력하는 방법까지 배웁니다.

```
1단계: WPF 프로그램 구조 이해 (XAML + C#)
   ↓
2단계: Hello World 만들기      (버튼 클릭 → 메시지 출력)
   ↓
3단계: 변수에 값 저장하기      (int, double, string)
   ↓
4단계: 변수 값 변경하기        (계산해서 다시 저장)
```

이 강의의 목표는 **"WPF의 구조를 이해하고, 변수에 담은 값을 화면에 출력할 수 있다"** 입니다.

---

## 1. WPF 프로그램의 구조

WPF 프로그램은 **두 개의 파일이 한 쌍**으로 동작합니다.

```
   화면(UI)              동작(로직)
+-------------+      +------------------+
|             |      |                  |
| MainWindow  | <--> | MainWindow       |
|   .xaml     |      |   .xaml.cs       |
|             |      |                  |
+-------------+      +------------------+
   버튼, 글자,           버튼 클릭 시
   레이아웃 배치          무엇을 할지 작성
```

| 파일 | 언어 | 역할 |
|---|---|---|
| `MainWindow.xaml` | XAML | **화면(UI)** 정의 — 버튼, 텍스트 등을 어디에 배치할지 |
| `MainWindow.xaml.cs` | C# | **동작(로직)** 정의 — 버튼을 누르면 무슨 일이 일어날지 |

> **Tip:** WPF는 **화면**과 **동작**이 분리되어 있어, 각각 독립적으로 작성·수정할 수 있습니다.

---

### 두 파일이 연결되는 방식

```
MainWindow.xaml                    MainWindow.xaml.cs
-----------------------            ------------------------------
<Button                            private void b1_Click(
    Click="b1_Click"/>  ------->       object sender,
                                       RoutedEventArgs e)
                                   {
                                       MessageBox.Show("Hello!");
                                   }

  XAML에서 메서드 이름 지정 ------> C#에서 같은 이름의 메서드 작성
```

XAML의 `Click="b1_Click"` 과 C#의 메서드 이름이 **정확히 일치**해야 연결됩니다.

---

## 2. Hello World 프로그램

**목표:** 버튼을 클릭하면 `Hello, World!` 팝업이 나타나는 프로그램

### MainWindow.xaml (화면 정의)

```xml
<!-- Window      : 이 파일이 하나의 창임을 선언 -->
<!-- x:Class     : 짝이 되는 C# 클래스 이름 -->
<!-- Title       : 창 상단에 표시되는 제목 -->
<!-- Height/Width: 창의 세로/가로 크기 (픽셀) -->
<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Hello WPF"
        Height="200" Width="300">

    <!-- Grid: 창 내부의 기본 레이아웃 컨테이너 -->
    <Grid>
        <!-- Button       : 클릭 가능한 버튼 -->
        <!-- Name="b1"    : C#에서 이 버튼을 b1으로 부름 -->
        <!-- Content      : 버튼에 표시될 글자 -->
        <!-- Click        : 클릭 시 실행할 C# 메서드 이름 -->
        <Button Name="b1"
                Content="클릭"
                Width="120" Height="40"
                Click="b1_Click"/>
    </Grid>

</Window>
```

### MainWindow.xaml.cs (동작 정의)

```csharp
using System.Windows;                           // WPF의 Window, MessageBox 사용 선언

namespace WpfApp1                               // XAML의 x:Class와 일치
{
    // partial : XAML 자동생성 코드와 합쳐져 하나의 클래스가 됨
    // Window  : 창(Window) 기능을 상속받음
    public partial class MainWindow : Window
    {
        // 생성자: 창이 만들어질 때 한 번 실행됨
        public MainWindow()
        {
            InitializeComponent();              // XAML에 선언된 컨트롤들을 메모리에 생성
        }

        // 버튼 클릭 시 실행되는 메서드
        // XAML의 Click="b1_Click" 과 이름이 일치해야 함
        private void b1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hello, World!");   // 팝업 출력
        }
    }
}
```

### 실행 결과

```
   실행 직후                 버튼 클릭 후
+--------------+         +------------------+
|              |         |                  |
|  +--------+  |  클릭   |  +------------+  |
|  |  클릭  |  | ----->  |  |Hello, World|  |
|  +--------+  |         |  |  [ 확인 ]  |  |
|              |         |  +------------+  |
+--------------+         +------------------+
```

> **Tip:** Console 프로그램은 `Console.WriteLine()` 으로 출력하지만,
> WPF에서는 **`MessageBox.Show()`** 로 팝업창에 출력합니다.

---

### 이벤트 기반 동작 흐름

WPF는 **사용자의 행동(이벤트)** 에 따라 코드가 실행되는 구조입니다.

```
1. 사용자가 버튼 클릭 (행동)
       |
       v
2. Click 이벤트 발생
       |
       v
3. XAML의 Click="b1_Click" 으로 연결된 메서드 찾기
       |
       v
4. b1_Click() 메서드 실행
       |
       v
5. MessageBox.Show("Hello, World!") 결과 출력
```

---

## 3. 변수란?

**변수(Variable)** 는 값을 담아두는 **상자**입니다.
숫자나 글자를 저장해두고 필요할 때 꺼내 쓸 수 있습니다.

```
   변수(상자)
  +---------+
  |   20    |    <- 값
  +---------+
     a          <- 변수 이름
```

### 변수 만드는 방법

```csharp
int a = 20;
//  ↑   ↑    ↑
// 자료형 이름  값
```

| 부분 | 의미 | 예시 |
|---|---|---|
| 자료형 | 어떤 종류의 값을 담을지 | `int`, `double`, `string` |
| 이름 | 상자에 붙일 이름 | `a`, `b`, `name` |
| 값 | 상자에 담을 내용 | `20`, `3.14`, `"홍길동"` |

---

## 4. 자주 쓰는 자료형 3가지

처음에는 이 **3개만** 알면 됩니다.

| 자료형 | 용도 | 예시 |
|---|---|---|
| `int` | 정수 (소수점 없음) | `int a = 20;` |
| `double` | 실수 (소수점 있음) | `double b = 3.14;` |
| `string` | 글자 (문자열) | `string s = "Hi";` |

> **Tip:** 글자(`string`)는 반드시 큰따옴표 `" "` 로 감쌉니다.

---

## 5. 변수를 메시지박스에 출력하기

변수 값을 화면에 보이려면 `MessageBox.Show()` 안에 변수를 넣으면 됩니다.
변수와 글자를 함께 출력할 때는 **`+`** 로 연결합니다.

### MainWindow.xaml

```xml
<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="변수 출력"
        Height="200" Width="300">
    <Grid>
        <Button Name="b1"
                Content="확인"
                Width="120" Height="40"
                Click="b1_Click"/>
    </Grid>
</Window>
```

### MainWindow.xaml.cs

```csharp
using System.Windows;
namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void b1_Click(object sender, RoutedEventArgs e)
        {
            int a = 20;                         // 정수형 변수 a = 20
            double b = 3.14;                    // 실수형 변수 b = 3.14
            string s = "홍길동";                // 문자열 변수 s = "홍길동"

            // 변수와 글자를 + 로 연결해서 출력
            MessageBox.Show("a = " + a + ", b = " + b + ", s = " + s);
        }
    }
}
```

### 실행 결과

```
+----------------------------------+
|                                  |
|  a = 20, b = 3.14, s = 홍길동    |
|                                  |
|             [ 확인 ]             |
+----------------------------------+
```

---

## 6. 변수 값 바꾸기

변수에 담긴 값은 나중에 **다른 값으로 바꿀 수 있습니다**.

```csharp
private void b1_Click(object sender, RoutedEventArgs e)
{
    int a = 0;                                  // 처음 값 0
    a = 10;                                     // 10으로 변경
    a = a + 5;                                  // 자기 값에 5를 더해 다시 저장 (15)

    MessageBox.Show("a = " + a);                // 결과: a = 15
}
```

### 동작 원리

```
   처음            대입 후         계산 후
 +-----+         +-----+         +-----+
 |  0  |   -->   | 10  |   -->   | 15  |
 +-----+         +-----+         +-----+
   a               a               a
                                a = a + 5
                              (10 + 5 = 15)
```

---

## 7. 핵심 정리

- WPF 프로그램은 **XAML(화면)** 과 **C#(동작)** 두 파일이 한 쌍으로 동작합니다.
- XAML의 `Click="메서드명"` 과 C#의 메서드 이름이 **정확히 일치**해야 연결됩니다.
- WPF에서 값을 화면에 보일 때는 **`MessageBox.Show()`** 를 사용합니다.
- **변수**는 값을 담는 상자이고, **`자료형 이름 = 값;`** 형식으로 만듭니다.
- 자주 쓰는 자료형은 **`int`(정수), `double`(실수), `string`(글자)** 세 가지입니다.
- 변수와 글자를 함께 출력할 때는 **`+`** 로 연결합니다.
- 변수 값은 **나중에 바꿀 수 있고**, 자기 값을 이용해 새 값을 만들 수도 있습니다 (`a = a + 5`).

---

## 예제

---

### 예제 1 — 두 수 더하기

두 변수의 합을 구해서 메시지박스에 출력합니다.

#### MainWindow.xaml

```xml
<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="더하기"
        Height="200" Width="300">
    <Grid>
        <Button Name="b1"
                Content="더하기"
                Width="120" Height="40"
                Click="b1_Click"/>
    </Grid>
</Window>
```

#### MainWindow.xaml.cs

```csharp
using System.Windows;
namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void b1_Click(object sender, RoutedEventArgs e)
        {
            int a = 10;                         // 첫 번째 수
            int b = 20;                         // 두 번째 수
            int c = a + b;                      // 두 수의 합을 c에 저장

            MessageBox.Show(a + " + " + b + " = " + c);
        }
    }
}
```

#### 실행 결과

```
+----------------------+
|                      |
|   10 + 20 = 30       |
|                      |
|      [ 확인 ]        |
+----------------------+
```

---

### 예제 2 — 이름과 나이 출력

문자열과 정수를 함께 사용해 봅니다.

#### MainWindow.xaml

```xml
<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="자기소개"
        Height="200" Width="300">
    <Grid>
        <Button Name="b1"
                Content="소개"
                Width="120" Height="40"
                Click="b1_Click"/>
    </Grid>
</Window>
```

#### MainWindow.xaml.cs

```csharp
using System.Windows;
namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void b1_Click(object sender, RoutedEventArgs e)
        {
            string name = "홍길동";             // 이름
            int age = 25;                       // 나이

            MessageBox.Show("이름: " + name + ", 나이: " + age);
        }
    }
}
```

#### 실행 결과

```
+--------------------------+
|                          |
|  이름: 홍길동, 나이: 25  |
|                          |
|        [ 확인 ]          |
+--------------------------+
```

---

### 예제 3 — 버튼 누를 때마다 숫자 증가

변수 값을 바꾸는 응용 예제입니다.
버튼을 누를 때마다 숫자가 1씩 늘어납니다.

#### MainWindow.xaml

```xml
<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="카운터"
        Height="200" Width="300">
    <Grid>
        <Button Name="b1"
                Content="+1"
                Width="120" Height="40"
                Click="b1_Click"/>
    </Grid>
</Window>
```

#### MainWindow.xaml.cs

```csharp
using System.Windows;
namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        // 메서드 밖에 변수를 두면 클릭할 때마다 값이 유지됨
        int n = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void b1_Click(object sender, RoutedEventArgs e)
        {
            n = n + 1;                          // 누를 때마다 1 증가
            MessageBox.Show("n = " + n);
        }
    }
}
```

#### 실행 결과

```
1번 클릭          2번 클릭          3번 클릭
+--------+       +--------+       +--------+
| n = 1  |  -->  | n = 2  |  -->  | n = 3  |
|[ 확인 ]|       |[ 확인 ]|       |[ 확인 ]|
+--------+       +--------+       +--------+
```

> **Tip:** 변수를 **메서드 안**이 아니라 **클래스 안(메서드 밖)** 에 선언하면,
> 클릭할 때마다 값이 사라지지 않고 **계속 유지**됩니다.

---

## 문제

---

### 문제 1

WPF에서 화면(UI)을 정의하는 파일의 확장자는 무엇인가요?

<details>
<summary>정답 보기 (클릭)</summary>

`.xaml` 입니다. (예: `MainWindow.xaml`)
짝이 되는 동작 파일은 `.xaml.cs` 입니다 (예: `MainWindow.xaml.cs`).

</details>

---

### 문제 2

다음 코드에서 메시지박스에 출력되는 값은 무엇인가요?

```csharp
private void b1_Click(object sender, RoutedEventArgs e)
{
    int a = 5;
    int b = 7;
    MessageBox.Show("결과: " + (a + b));
}
```

<details>
<summary>정답 보기 (클릭)</summary>

```
결과: 12
```

</details>

---

### 문제 3

다음 중 **올바른 변수 선언**은 무엇인가요?

```
① int 1a = 100;
② int a_1 = 100;
③ int int = 100;
④ int a 1 = 100;
```

<details>
<summary>정답 보기 (클릭)</summary>

② `int a_1 = 100;`
- 변수 이름은 **숫자로 시작할 수 없습니다** (① 틀림).
- `int`는 예약어라서 변수명으로 쓸 수 없습니다 (③ 틀림).
- 변수 이름에는 **공백이 들어갈 수 없습니다** (④ 틀림).

</details>

---

### 문제 4

소수점이 있는 숫자(예: 3.14)를 저장하려면 어떤 자료형을 써야 하나요?

```
① int
② double
③ string
④ bool
```

<details>
<summary>정답 보기 (클릭)</summary>

② `double` — 소수점이 있는 실수를 담는 자료형입니다.
- `int`는 정수만 저장 가능
- `string`은 글자(문자열)
- `bool`은 참/거짓 (true/false)

</details>

---

### 문제 5

빈칸을 채워, 버튼 클릭 시 메시지박스에 `합: 30` 이 출력되도록 완성하세요.

```csharp
private void b1_Click(object sender, RoutedEventArgs e)
{
    int a = 10;
    int b = 20;
    int c = ________;

    MessageBox.Show("합: " + ____);
}
```

<details>
<summary>정답 보기 (클릭)</summary>

```csharp
private void b1_Click(object sender, RoutedEventArgs e)
{
    int a = 10;
    int b = 20;
    int c = a + b;                              // 두 변수를 더해 c에 저장

    MessageBox.Show("합: " + c);                // 결과: 합: 30
}
```

</details>

---

### 문제 6

다음 코드에서 **잘못된 부분을 모두 찾아** 고치세요.

```csharp
private void b1_Click(object sender, RoutedEventArgs e)
{
    Int a = 10
    double b = 3.14
    MessageBox.show("a: " a + ", b: " b)
}
```

<details>
<summary>정답 보기 (클릭)</summary>

```csharp
private void b1_Click(object sender, RoutedEventArgs e)
{
    int a = 10;                                 // Int -> int (소문자), 세미콜론 추가
    double b = 3.14;                            // 세미콜론 추가
    MessageBox.Show("a: " + a + ", b: " + b);   // show -> Show, + 연산자 추가, 세미콜론 추가
}
```

수정 사항 정리:
1. `Int` → `int` (C#은 대소문자를 구분하며, 자료형은 소문자)
2. `int a = 10` 뒤에 세미콜론(`;`) 누락
3. `double b = 3.14` 뒤에 세미콜론(`;`) 누락
4. `MessageBox.show` → `MessageBox.Show` (대문자 S)
5. 변수와 글자 사이에 `+` 연산자 누락
6. 마지막 줄 끝에 세미콜론(`;`) 누락

</details>

---

> **Tip:** XAML에서 `Click="이름"` 을 입력한 뒤 **Tab 키** 를 누르면,
> Visual Studio가 같은 이름의 C# 메서드를 자동으로 생성해줍니다!
