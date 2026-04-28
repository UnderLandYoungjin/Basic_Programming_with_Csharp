# C# WPF 제2강 — 문자와 문자열 (char & string)

## 개요
**문자(char)** 는 `'A'`, `'가'` 처럼 **단 하나의 문자**를 저장하는 자료형이고,
**문자열(string)** 은 `"Hello"`, `"안녕하세요"` 처럼 **문자들의 묶음**을 저장하는 자료형입니다.

이번 강의에서는 단순히 자료형만 배우는 것이 아니라,
**WPF의 입력 컨트롤(TextBox)** 과 **출력 컨트롤(TextBlock)** 을 함께 사용해서
**사용자가 입력한 문자열을 화면에 표시**하는 진짜 WPF 프로그램을 만듭니다.

```
1단계: TextBlock으로 문자열 출력하기 (StackPanel 레이아웃 사용)
   ↓
2단계: TextBox로 사용자 입력 받기 (입력 -> 출력 연결)
   ↓
3단계: 문자열 연결과 보간 ($)
   ↓
4단계: 이스케이프 시퀀스와 문자열 메서드 응용
```

이 강의의 목표는 **"TextBox로 문자열을 입력받아 가공한 뒤 TextBlock에 출력할 수 있다"** 입니다.

---

## 1. WPF의 입력/출력 컨트롤 소개

지난 시간에는 결과를 `MessageBox`로 띄웠지만,
실제 WPF 앱에서는 **창 안의 컨트롤**에 결과를 표시합니다.

| 컨트롤 | 역할 | 주요 속성 |
|---|---|---|
| `TextBlock` | **출력 전용** — 문자열을 화면에 표시 | `Text` |
| `TextBox` | **입력용** — 사용자가 문자열을 입력 | `Text` |
| `Button` | 클릭 이벤트를 받음 | `Content`, `Click` |

```
+----------------------------+
|  [TextBox]  <- 사용자 입력 |    홍길동
|                            |
|  [Button]   <- 클릭        |    [확인]
|                            |
|  [TextBlock] <- 결과 출력  |    안녕하세요, 홍길동!
+----------------------------+
```

> **Tip:** `TextBlock`과 `TextBox` 모두 **`Text`** 라는 속성으로 문자열을 다룹니다.
> C# 코드에서 `t1.Text = "..."` 처럼 값을 넣거나 읽을 수 있습니다.

---

## 2. 문자형 (char) — 단 하나의 문자

`char` 형은 **단 하나의 문자**만 저장하며, 반드시 **작은따옴표(`'`)** 로 감쌉니다.

```csharp
char c = 'A';
//  ↑   ↑   ↑
// 자료형 이름  값(작은따옴표)
```

> **주의:** 두 글자 이상이거나 큰따옴표를 쓰면 **컴파일 오류**가 발생합니다.
> ```csharp
> // char a = 'AB';     // 두 글자 이상 불가
> // char a = "A";      // 큰따옴표 사용 불가
> ```

---

### 예제 1 — TextBlock에 문자 출력하기

**목표:** 버튼을 클릭하면 `char` 변수에 담긴 값을 `TextBlock`에 출력합니다.

#### MainWindow.xaml

```xml
<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="문자 출력"
        Height="220" Width="300">

    <!-- StackPanel: 자식 컨트롤을 위에서 아래로 쌓는 레이아웃 -->
    <!-- VerticalAlignment="Center": 창 세로 가운데 정렬 -->
    <!-- Margin="20": 바깥 여백 20px -->
    <StackPanel VerticalAlignment="Center" Margin="20">

        <!-- Button: 클릭 시 b1_Click 메서드 실행 -->
        <Button Name="b1"
                Content="확인"
                Width="100" Height="35"
                Margin="0,0,0,10"
                Click="b1_Click"/>

        <!-- TextBlock: 결과를 표시할 출력용 컨트롤 -->
        <!-- Name="t1": C#에서 t1 이라는 이름으로 접근 -->
        <!-- FontSize="16": 글자 크기 -->
        <TextBlock Name="t1"
                   FontSize="16"
                   HorizontalAlignment="Center"/>

    </StackPanel>
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
            char a = 'A';                       // 영문자
            char b = '@';                       // 기호
            char c = '가';                      // 한글 한 글자

            // TextBlock의 Text 속성에 문자열 대입 -> 화면에 표시됨
            t1.Text = "a = " + a + ", b = " + b + ", c = " + c;
        }
    }
}
```

#### 실행 결과

```
   실행 직후                    버튼 클릭 후
+----------------+         +-----------------------+
|                |         |                       |
|   +--------+   |  클릭   |   +--------+          |
|   |  확인  |   | -----> |   |  확인  |          |
|   +--------+   |         |   +--------+          |
|                |         |                       |
|   (빈 영역)    |         |  a = A, b = @, c = 가 |
+----------------+         +-----------------------+
```

> **핵심:** `MessageBox`로 띄우지 않고, **`t1.Text = "..."`** 로 창 안의 TextBlock에 직접 표시합니다.
> 이것이 WPF 앱의 일반적인 동작 방식입니다.

---

## 3. 문자열형 (string) — 문자들의 묶음

`string` 형은 **문자들의 묶음**을 저장하며, **큰따옴표(`"`)** 로 감쌉니다.

```csharp
string s = "Hello";
//   ↑   ↑    ↑
//  자료형 이름  값(큰따옴표)
```

---

### 예제 2 — TextBox로 입력받아 TextBlock에 출력 (WPF의 핵심 패턴)

**목표:** 사용자가 TextBox에 이름을 입력하고 버튼을 클릭하면,
TextBlock에 인사말이 나타나도록 합니다.

#### MainWindow.xaml

```xml
<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="이름 입력"
        Height="240" Width="300">

    <StackPanel VerticalAlignment="Center" Margin="20">

        <!-- TextBlock: 안내 문구 (입력 전 미리 작성된 텍스트) -->
        <TextBlock Text="이름을 입력하세요:"
                   Margin="0,0,0,5"/>

        <!-- TextBox: 사용자 입력란 -->
        <!-- Name="tb1": C#에서 tb1.Text 로 입력값 읽음 -->
        <TextBox Name="tb1"
                 Height="28"
                 Margin="0,0,0,10"/>

        <!-- Button: 클릭 시 b1_Click 실행 -->
        <Button Name="b1"
                Content="인사하기"
                Height="35"
                Margin="0,0,0,10"
                Click="b1_Click"/>

        <!-- TextBlock: 결과 출력 영역 -->
        <TextBlock Name="t1"
                   FontSize="14"
                   HorizontalAlignment="Center"/>

    </StackPanel>
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
            // tb1.Text: TextBox에 입력된 값을 string으로 가져옴
            string name = tb1.Text;

            // t1.Text: TextBlock에 결과 문자열을 대입
            t1.Text = "안녕하세요, " + name + "님!";
        }
    }
}
```

#### 동작 흐름

```
사용자가 TextBox에 입력         버튼 클릭            결과 출력
+------------------+         +----------+       +----------------------+
| tb1.Text="홍길동"| ------> | b1_Click | ----> | t1.Text="안녕하세요, |
+------------------+         +----------+       |       홍길동님!"     |
                                                +----------------------+
       (입력)                    (이벤트)              (출력)
```

#### 실행 결과

```
   입력 후                          버튼 클릭 후
+----------------------+         +----------------------+
| 이름을 입력하세요:   |         | 이름을 입력하세요:   |
| +------------------+ |         | +------------------+ |
| | 홍길동           | |  클릭   | | 홍길동           | |
| +------------------+ | -----> | +------------------+ |
| +------------------+ |         | +------------------+ |
| |   인사하기       | |         | |   인사하기       | |
| +------------------+ |         | +------------------+ |
|                      |         |                      |
| (빈 영역)            |         | 안녕하세요, 홍길동님!|
+----------------------+         +----------------------+
```

> **이것이 WPF의 핵심 패턴입니다:**
> **TextBox(입력) -> Button(이벤트) -> TextBlock(출력)**

---

## 4. 문자열 연결 — `+` 와 `$"..."`

### 방법 1 — `+` 연산자

```csharp
t1.Text = "이름: " + name + ", 나이: " + age + "세";
```

### 방법 2 — 문자열 보간 `$"..."` (권장)

문자열 앞에 **`$`** 를 붙이면, **`{변수명}`** 형태로 변수를 바로 끼워 넣을 수 있어 깔끔합니다.

```csharp
t1.Text = $"이름: {name}, 나이: {age}세";
```

### 두 방법 비교

```
[+ 연산자 방식]                              [$ 보간 방식]
"이름: " + name + ", 나이: " + age + "세"    $"이름: {name}, 나이: {age}세"
        ↑                                           ↑
  따옴표와 + 가 많아 헷갈림                한 줄로 깔끔하게 표현
```

---

## 5. 이스케이프 시퀀스 (Escape Sequence)

문자열 안에 **줄바꿈, 탭, 따옴표** 같은 특수 문자를 넣을 때는
백슬래시(`\`) 뒤에 특정 문자를 붙여서 사용합니다.

| 표기 | 의미 | 사용 예 |
|---|---|---|
| `\n` | 줄바꿈 | `"첫줄\n둘째줄"` |
| `\t` | 탭 (간격) | `"이름\t나이"` |
| `\\` | 백슬래시 자체 | `"C:\\Users"` |
| `\"` | 큰따옴표 | `"그가 \"안녕\"이라 했다"` |

> **WPF Tip:** TextBlock에서 긴 문자열이 화면을 벗어날 때는 XAML에 **`TextWrapping="Wrap"`** 을
> 추가하면 자동으로 줄바꿈됩니다.

---

## 6. 문자열 주요 메서드

`string` 형에는 편리한 기능(메서드)이 많이 있습니다.

| 메서드 | 기능 | 예시 |
|---|---|---|
| `.Length` | 문자열 길이 | `"홍길동".Length` -> `3` |
| `.ToUpper()` | 대문자 변환 | `"hello".ToUpper()` -> `"HELLO"` |
| `.ToLower()` | 소문자 변환 | `"HELLO".ToLower()` -> `"hello"` |
| `.Contains("x")` | 포함 여부 | `"abc".Contains("b")` -> `true` |
| `.Replace("a", "b")` | 문자 치환 | `"Java".Replace("J", "L")` -> `"Lava"` |
| `.Trim()` | 앞뒤 공백 제거 | `"  Hi  ".Trim()` -> `"Hi"` |

> **참고:** `.Length` 는 괄호가 없는 **속성**이고, 나머지는 괄호가 있는 **메서드**입니다.

---

## 7. char와 string 비교

| 구분 | `char` | `string` |
|---|---|---|
| 저장 단위 | 단 하나의 문자 | 0개 이상의 문자 묶음 |
| 따옴표 | 작은따옴표 `'A'` | 큰따옴표 `"Hello"` |
| 예시 | `char c = 'A';` | `string s = "Hello";` |

---

## 8. 핵심 정리

- **WPF의 입출력 패턴**: TextBox로 입력받고, Button으로 이벤트 발생, TextBlock에 출력합니다.
- 컨트롤의 문자열 값은 **`Text` 속성**으로 읽고 씁니다 (`tb1.Text`, `t1.Text`).
- **`char`** 는 단 하나의 문자만 저장하며 **작은따옴표(`'`)** 를 사용합니다.
- **`string`** 은 문자열을 저장하며 **큰따옴표(`"`)** 를 사용합니다.
- 문자열 연결은 **`+`** 또는 **`$"..."` 보간 문법** 으로 합니다.
- **`\n`, `\t`, `\\`, `\"`** 같은 이스케이프 시퀀스로 특수 문자를 표현합니다.
- 문자열은 `.Length`, `.ToUpper()`, `.Contains()`, `.Replace()`, `.Trim()` 등 다양한 기능을 제공합니다.

---

## 예제

---

### 예제 3 — 두 문자열 합치기

성과 이름을 따로 입력받아 풀네임을 만들어 출력합니다.

#### MainWindow.xaml

```xml
<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="이름 합치기"
        Height="280" Width="300">

    <StackPanel VerticalAlignment="Center" Margin="20">

        <TextBlock Text="성:" Margin="0,0,0,3"/>
        <!-- 성을 입력받는 TextBox -->
        <TextBox Name="tb1" Height="28" Margin="0,0,0,8"/>

        <TextBlock Text="이름:" Margin="0,0,0,3"/>
        <!-- 이름을 입력받는 TextBox -->
        <TextBox Name="tb2" Height="28" Margin="0,0,0,10"/>

        <Button Content="합치기"
                Height="35"
                Margin="0,0,0,10"
                Click="b1_Click"/>

        <!-- 결과 출력 영역 -->
        <TextBlock Name="t1"
                   FontSize="14"
                   HorizontalAlignment="Center"/>

    </StackPanel>
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
            string s1 = tb1.Text;               // 성 (예: "홍")
            string s2 = tb2.Text;               // 이름 (예: "길동")
            string s3 = s1 + s2;                // 풀네임 ("홍길동")

            // $"..." 보간 문법으로 깔끔하게 출력
            t1.Text = $"풀네임: {s3}";
        }
    }
}
```

#### 실행 결과

```
   입력 후                       버튼 클릭 후
+--------------------+        +--------------------+
| 성:                |        | 성:                |
| +----------------+ |        | +----------------+ |
| | 홍             | |        | | 홍             | |
| +----------------+ |        | +----------------+ |
| 이름:              |        | 이름:              |
| +----------------+ |  클릭  | +----------------+ |
| | 길동           | | -----> | | 길동           | |
| +----------------+ |        | +----------------+ |
| +----------------+ |        | +----------------+ |
| |    합치기      | |        | |    합치기      | |
| +----------------+ |        | +----------------+ |
|                    |        |                    |
| (빈 영역)          |        |  풀네임: 홍길동    |
+--------------------+        +--------------------+
```

---

### 예제 4 — 문자열 메서드 응용 (입력 가공)

사용자가 입력한 문자열을 다양하게 가공해서 한 번에 보여줍니다.

#### MainWindow.xaml

```xml
<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="문자열 가공"
        Height="320" Width="320">

    <StackPanel VerticalAlignment="Center" Margin="20">

        <TextBlock Text="문자열을 입력하세요:" Margin="0,0,0,5"/>
        <TextBox Name="tb1" Height="28" Margin="0,0,0,10"/>

        <Button Content="가공하기"
                Height="35"
                Margin="0,0,0,10"
                Click="b1_Click"/>

        <!-- TextWrapping="Wrap": 길어지면 자동 줄바꿈 -->
        <TextBlock Name="t1"
                   FontSize="13"
                   TextWrapping="Wrap"/>

    </StackPanel>
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
            string s = tb1.Text;                // 사용자 입력 가져오기

            // \n 으로 줄바꿈해서 여러 정보를 한 번에 출력
            t1.Text = $"원본: {s}\n" +
                      $"길이: {s.Length}\n" +
                      $"대문자: {s.ToUpper()}\n" +
                      $"소문자: {s.ToLower()}\n" +
                      $"공백 제거: '{s.Trim()}'";
        }
    }
}
```

#### 실행 결과 (예: "Hello" 입력 후)

```
+--------------------------+
| 문자열을 입력하세요:     |
| +--------------------+   |
| | Hello              |   |
| +--------------------+   |
| +--------------------+   |
| |    가공하기        |   |
| +--------------------+   |
|                          |
| 원본: Hello              |
| 길이: 5                  |
| 대문자: HELLO            |
| 소문자: hello            |
| 공백 제거: 'Hello'       |
+--------------------------+
```

---

### 예제 5 — 입력값에 특정 단어가 포함되어 있는지 확인

`Contains()` 메서드로 입력 검사를 합니다.

#### MainWindow.xaml

```xml
<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="단어 검사"
        Height="240" Width="320">

    <StackPanel VerticalAlignment="Center" Margin="20">

        <TextBlock Text="문장을 입력하세요:" Margin="0,0,0,5"/>
        <TextBox Name="tb1" Height="28" Margin="0,0,0,10"/>

        <Button Content="C# 포함 여부 확인"
                Height="35"
                Margin="0,0,0,10"
                Click="b1_Click"/>

        <TextBlock Name="t1"
                   FontSize="14"
                   HorizontalAlignment="Center"/>

    </StackPanel>
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
            string s = tb1.Text;
            bool ok = s.Contains("C#");         // "C#" 이 들어있으면 true

            t1.Text = $"\"C#\" 포함 여부: {ok}";
        }
    }
}
```

#### 실행 결과

```
"나는 C#을 좋아한다" 입력 시:    "Hello World" 입력 시:
+----------------------+        +----------------------+
| "C#" 포함 여부: True |        | "C#" 포함 여부: False|
+----------------------+        +----------------------+
```

> **Tip:** C# 코드에서 따옴표 안에 따옴표를 쓸 때는 `\"` 를 사용합니다.

---

## 문제

---

### 문제 1

WPF에서 **사용자에게 입력을 받는** 컨트롤과 **결과를 출력하는** 컨트롤의 이름은 각각 무엇인가요?

<details>
<summary>정답 보기 (클릭)</summary>

- 입력: **`TextBox`**
- 출력: **`TextBlock`**

두 컨트롤 모두 **`Text`** 속성으로 문자열을 다룹니다.
- 입력값 읽기: `string s = tb1.Text;`
- 출력값 쓰기: `t1.Text = "결과";`

</details>

---

### 문제 2

다음 중 **올바른 선언**은 무엇인가요?

```
① char c = "A";
② char c = 'AB';
③ char c = 'A';
④ string s = 'Hello';
```

<details>
<summary>정답 보기 (클릭)</summary>

③ `char c = 'A';`

- ① 틀림: `char`는 큰따옴표가 아닌 **작은따옴표**
- ② 틀림: `char`는 **단 하나의 문자만** 저장 가능
- ④ 틀림: `string`은 작은따옴표가 아닌 **큰따옴표**

</details>

---

### 문제 3

빈칸을 채워, 사용자가 `tb1`에 입력한 글자를 그대로 `t1`에 표시하도록 완성하세요.

```csharp
private void b1_Click(object sender, RoutedEventArgs e)
{
    string s = ________;                    // TextBox 값 읽기
    ________ = "입력값: " + s;              // TextBlock에 출력
}
```

<details>
<summary>정답 보기 (클릭)</summary>

```csharp
private void b1_Click(object sender, RoutedEventArgs e)
{
    string s = tb1.Text;                    // TextBox 값 읽기
    t1.Text = "입력값: " + s;               // TextBlock에 출력
}
```

</details>

---

### 문제 4

다음 코드의 출력 결과는 무엇인가요?

```csharp
private void b1_Click(object sender, RoutedEventArgs e)
{
    string s = "Hello";
    t1.Text = $"길이: {s.Length}, 대문자: {s.ToUpper()}";
}
```

<details>
<summary>정답 보기 (클릭)</summary>

```
길이: 5, 대문자: HELLO
```

</details>

---

### 문제 5

빈칸을 채워, 사용자가 입력한 이름과 나이를 사용해
**`이름: 홍길동, 나이: 25세`** 형식으로 출력하도록 완성하세요. (`$"..."` 사용)

```xml
<TextBox Name="tb1"/>   <!-- 이름 입력 -->
<TextBox Name="tb2"/>   <!-- 나이 입력 -->
<TextBlock Name="t1"/>
```

```csharp
private void b1_Click(object sender, RoutedEventArgs e)
{
    string name = tb1.Text;
    string age = tb2.Text;

    t1.Text = ____________________________;
}
```

<details>
<summary>정답 보기 (클릭)</summary>

```csharp
private void b1_Click(object sender, RoutedEventArgs e)
{
    string name = tb1.Text;
    string age = tb2.Text;

    t1.Text = $"이름: {name}, 나이: {age}세";
}
```

문자열 앞에 `$` 를 붙이면, 문자열 안에 `{변수명}` 으로 값을 바로 넣을 수 있습니다.

</details>

---

### 문제 6

다음 코드에서 **잘못된 부분을 모두 찾아** 고치세요.

```csharp
private void b1_Click(object sender, RoutedEventArgs e)
{
    char grade = "A";
    string name = '홍길동';
    t1.text = "이름: " name + ", 학점: " + grade
}
```

<details>
<summary>정답 보기 (클릭)</summary>

```csharp
private void b1_Click(object sender, RoutedEventArgs e)
{
    char grade = 'A';                                   // "A" -> 'A' (char는 작은따옴표)
    string name = "홍길동";                             // '홍길동' -> "홍길동" (string은 큰따옴표)
    t1.Text = "이름: " + name + ", 학점: " + grade;     // text -> Text, + 추가, ; 추가
}
```

수정 사항:
1. `char grade = "A";` -> `char grade = 'A';` (`char`는 작은따옴표)
2. `string name = '홍길동';` -> `string name = "홍길동";` (`string`은 큰따옴표)
3. `t1.text` -> `t1.Text` (속성명은 대문자 T로 시작)
4. `"이름: " name` -> `"이름: " + name` (`+` 연산자 누락)
5. 마지막 줄 끝에 세미콜론(`;`) 누락

</details>

---

> **Tip:**
> - WPF의 핵심 패턴은 **TextBox(입력) -> Button(이벤트) -> TextBlock(출력)** 입니다.
> - 컨트롤의 값은 **`Name속성.Text`** 로 읽고 씁니다 (`tb1.Text`, `t1.Text`).
> - 문자열 연결은 **`$"..."`** 가 가장 깔끔합니다.
> - `char`는 **작은따옴표**, `string`은 **큰따옴표** — 절대 헷갈리지 마세요.
