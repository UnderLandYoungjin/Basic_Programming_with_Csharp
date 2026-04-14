# 🟣 제2강 — 레이아웃 (Grid, StackPanel, DockPanel)

## 📌 개요
WPF에서 **레이아웃(Layout)** 이란 컨트롤(버튼, 텍스트 등)을 화면에 **어떻게 배치할지** 결정하는 구조입니다.
레이아웃을 잘 이해해야 원하는 위치에 원하는 크기로 컨트롤을 배치할 수 있습니다.

이번 강의에서 배우는 3가지 레이아웃 패널:

| 패널 | 특징 | 주로 사용하는 경우 |
|---|---|---|
| `Grid` | 행/열로 나눠서 배치 | 복잡한 화면 구성, 폼 레이아웃 |
| `StackPanel` | 위→아래 또는 좌→우로 쌓아서 배치 | 버튼 목록, 메뉴 구성 |
| `DockPanel` | 상/하/좌/우 가장자리에 붙여서 배치 | 메뉴바, 상태바, 사이드바 구성 |

---

## 1️⃣ Grid — 행과 열로 나누는 레이아웃

`Grid`는 화면을 **표(행 × 열)** 처럼 나눠서 각 칸에 컨트롤을 배치합니다.
WPF에서 가장 많이 사용하는 레이아웃입니다.

### 핵심 속성

| 속성 | 설명 |
|---|---|
| `RowDefinitions` | 행(가로줄) 개수와 높이 정의 |
| `ColumnDefinitions` | 열(세로줄) 개수와 너비 정의 |
| `Grid.Row="숫자"` | 컨트롤이 위치할 행 번호 (0부터 시작) |
| `Grid.Column="숫자"` | 컨트롤이 위치할 열 번호 (0부터 시작) |
| `*` | 남은 공간을 비율로 나눔 (예: `*` `2*` → 1:2 비율) |
| `Auto` | 컨트롤 크기에 맞게 자동 조절 |

---

### 💻 예제 1 — Grid로 버튼 4개 배치

> 🎯 **목표:** 2행 × 2열 Grid를 만들고 각 칸에 버튼을 배치합니다.
> 각 버튼을 클릭하면 어느 위치의 버튼인지 메시지가 나타납니다.

#### 📄 `MainWindow.xaml`

```xml
<Window x:Class="WpfLayoutApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Grid 예제" Height="250" Width="350">

    <!-- Grid: 행과 열로 화면을 분할하는 레이아웃 패널 -->
    <Grid>

        <!-- RowDefinitions: 행(가로줄)을 정의하는 구역 -->
        <Grid.RowDefinitions>
            <!-- Height="*": 사용 가능한 공간을 동일 비율로 나눔 (1:1) -->
            <RowDefinition Height="*"/>   <!-- 0번 행: 전체의 절반 높이 -->
            <RowDefinition Height="*"/>   <!-- 1번 행: 전체의 절반 높이 -->
        </Grid.RowDefinitions>

        <!-- ColumnDefinitions: 열(세로줄)을 정의하는 구역 -->
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*"/> <!-- 0번 열: 전체의 절반 너비 -->
            <ColumnDefinition Width="*"/> <!-- 1번 열: 전체의 절반 너비 -->
        </Grid.ColumnDefinitions>

        <!-- Grid.Row="0" Grid.Column="0": 0행 0열 (왼쪽 위) 에 배치 -->
        <!-- Margin="5": 상하좌우 5px 여백 -->
        <Button Grid.Row="0" Grid.Column="0"
                Content="[0행 0열]"
                Margin="5"
                Click="btn00_Click"/>

        <!-- Grid.Row="0" Grid.Column="1": 0행 1열 (오른쪽 위) 에 배치 -->
        <Button Grid.Row="0" Grid.Column="1"
                Content="[0행 1열]"
                Margin="5"
                Click="btn01_Click"/>

        <!-- Grid.Row="1" Grid.Column="0": 1행 0열 (왼쪽 아래) 에 배치 -->
        <Button Grid.Row="1" Grid.Column="0"
                Content="[1행 0열]"
                Margin="5"
                Click="btn10_Click"/>

        <!-- Grid.Row="1" Grid.Column="1": 1행 1열 (오른쪽 아래) 에 배치 -->
        <Button Grid.Row="1" Grid.Column="1"
                Content="[1행 1열]"
                Margin="5"
                Click="btn11_Click"/>

    </Grid>

</Window>
```

#### 📄 `MainWindow.xaml.cs`

```csharp
using System.Windows;
namespace WpfLayoutApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();      // XAML 컨트롤 생성
        }

        // 0행 0열 버튼 클릭 시 실행
        private void btn00_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("왼쪽 위 버튼 (0행 0열)");
        }

        // 0행 1열 버튼 클릭 시 실행
        private void btn01_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("오른쪽 위 버튼 (0행 1열)");
        }

        // 1행 0열 버튼 클릭 시 실행
        private void btn10_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("왼쪽 아래 버튼 (1행 0열)");
        }

        // 1행 1열 버튼 클릭 시 실행
        private void btn11_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("오른쪽 아래 버튼 (1행 1열)");
        }
    }
}
```

#### 🔗 연결 구조

```
📄 MainWindow.xaml                    📄 MainWindow.xaml.cs
──────────────────────────────        ────────────────────────────────────
<Button Click="btn00_Click"/> ──────► private void btn00_Click(...) { }
<Button Click="btn01_Click"/> ──────► private void btn01_Click(...) { }
<Button Click="btn10_Click"/> ──────► private void btn10_Click(...) { }
<Button Click="btn11_Click"/> ──────► private void btn11_Click(...) { }
```

#### ▶️ 실행 결과

```
┌──────────────────────────────────┐
│  Grid 예제                 - □ X │
├──────────────────────────────────┤
│  ┌──────────────┐┌─────────────┐ │
│  │  [0행 0열]   ││  [0행 1열]  │ │
│  └──────────────┘└─────────────┘ │
│  ┌──────────────┐┌─────────────┐ │
│  │  [1행 0열]   ││  [1행 1열]  │ │
│  └──────────────┘└─────────────┘ │
└──────────────────────────────────┘
```

```
[0행 0열] 클릭 시               [1행 1열] 클릭 시
┌─────────────────────┐        ┌─────────────────────┐
│  ℹ️              X  │        │  ℹ️              X  │
│                     │        │                     │
│  왼쪽 위 버튼       │        │  오른쪽 아래 버튼   │
│  (0행 0열)          │        │  (1행 1열)          │
│        [ 확인 ]     │        │        [ 확인 ]     │
└─────────────────────┘        └─────────────────────┘
```

---

### 💡 Grid 크기 지정 방법 3가지

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="100"/>  <!-- 고정: 항상 100px -->
        <RowDefinition Height="Auto"/> <!-- 자동: 내용 크기에 맞게 조절 -->
        <RowDefinition Height="*"/>    <!-- 비율: 나머지 공간 전부 차지 -->
    </Grid.RowDefinitions>
</Grid>
```

```
┌──────────────────────┐
│  항상 100px (고정)    │  ← Height="100"
├──────────────────────┤
│  내용만큼 (자동)      │  ← Height="Auto"
├──────────────────────┤
│                      │
│  나머지 전부 (비율)   │  ← Height="*"
│                      │
└──────────────────────┘
```

---

## 2️⃣ StackPanel — 순서대로 쌓는 레이아웃

`StackPanel`은 자식 컨트롤을 **위→아래(세로)** 또는 **좌→우(가로)** 방향으로 순서대로 나열합니다.

### 핵심 속성

| 속성 | 값 | 설명 |
|---|---|---|
| `Orientation` | `Vertical` (기본값) | 위에서 아래로 쌓음 |
| `Orientation` | `Horizontal` | 왼쪽에서 오른쪽으로 나열 |
| `Margin` | `좌,상,우,하` | 컨트롤 주변 여백 |

---

### 💻 예제 2 — StackPanel로 메뉴 버튼 만들기

> 🎯 **목표:** 세로 StackPanel로 메뉴 버튼 3개를 만들고,
> 각 버튼 클릭 시 해당 메뉴 이름이 메시지로 나타납니다.

#### 📄 `MainWindow.xaml`

```xml
<Window x:Class="WpfLayoutApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="StackPanel 예제" Height="250" Width="250">

    <!-- StackPanel: 자식 요소를 순서대로 쌓는 레이아웃 -->
    <!-- Orientation="Vertical": 위에서 아래로 쌓음 (기본값이므로 생략 가능) -->
    <!-- VerticalAlignment="Center": 창 세로 가운데 배치 -->
    <!-- HorizontalAlignment="Center": 창 가로 가운데 배치 -->
    <StackPanel Orientation="Vertical"
                VerticalAlignment="Center"
                HorizontalAlignment="Center">

        <!-- 각 버튼은 위에서 아래 순서로 자동 배치됨 -->
        <!-- Margin="0,0,0,8": 버튼 아래 8px 여백 (버튼 간 간격) -->
        <Button Content="📁 파일 열기"
                Width="150" Height="40"
                Margin="0,0,0,8"
                Click="btnFile_Click"/>

        <Button Content="💾 저장"
                Width="150" Height="40"
                Margin="0,0,0,8"
                Click="btnSave_Click"/>

        <Button Content="❌ 종료"
                Width="150" Height="40"
                Click="btnExit_Click"/>

    </StackPanel>

</Window>
```

#### 📄 `MainWindow.xaml.cs`

```csharp
using System.Windows;
namespace WpfLayoutApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();      // XAML 컨트롤 생성
        }

        // "파일 열기" 버튼 클릭 시 실행
        private void btnFile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("파일 열기 메뉴를 선택했습니다.");
        }

        // "저장" 버튼 클릭 시 실행
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("저장 메뉴를 선택했습니다.");
        }

        // "종료" 버튼 클릭 시 실행
        // this.Close(): 현재 창을 닫아서 프로그램을 종료함
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();               // 현재 창 닫기 → 프로그램 종료
        }
    }
}
```

#### ▶️ 실행 결과

```
  실행 직후                  파일 열기 클릭 후
┌─────────────────────┐     ┌─────────────────────┐
│ StackPanel 예제 - □X│     │ StackPanel 예제 - □X│
├─────────────────────┤     ├─────────────────────┤
│                     │     │  ┌───────────────┐  │
│  ┌───────────────┐  │     │  │ ℹ️          X │  │
│  │  📁 파일 열기 │  │ 클릭 │  │               │  │
│  └───────────────┘  │ ──► │  │ 파일 열기 메뉴│  │
│  ┌───────────────┐  │     │  │ 를 선택했습니 │  │
│  │    💾 저장    │  │     │  │     다.       │  │
│  └───────────────┘  │     │  │   [ 확인 ]   │  │
│  ┌───────────────┐  │     │  └───────────────┘  │
│  │    ❌ 종료    │  │     └─────────────────────┘
│  └───────────────┘  │
└─────────────────────┘
         ↑
    ❌ 종료 클릭 시 → 창이 닫히며 프로그램 종료
```

---

### 💡 StackPanel 가로 방향 (Horizontal)

`Orientation="Horizontal"` 로 바꾸면 버튼이 **왼쪽에서 오른쪽**으로 나열됩니다.

```xml
<!-- Orientation="Horizontal": 왼쪽에서 오른쪽으로 나열 -->
<StackPanel Orientation="Horizontal"
            VerticalAlignment="Center"
            HorizontalAlignment="Center">

    <!-- Margin="0,0,8,0": 버튼 오른쪽 8px 여백 (버튼 간 간격) -->
    <Button Content="이전" Width="80" Height="35" Margin="0,0,8,0"/>
    <Button Content="다음" Width="80" Height="35"/>

</StackPanel>
```

```
┌───────────────────────────┐
│                           │
│    ┌──────┐  ┌──────┐     │
│    │  이전 │  │  다음 │    │
│    └──────┘  └──────┘     │
│                           │
└───────────────────────────┘
```

---

## 3️⃣ DockPanel — 가장자리에 붙이는 레이아웃

`DockPanel`은 자식 컨트롤을 창의 **상/하/좌/우 가장자리**에 붙여서 배치합니다.
상단 메뉴바, 하단 상태바, 좌측 사이드바 같은 구성에 자주 사용됩니다.

### 핵심 속성

| 속성 | 값 | 설명 |
|---|---|---|
| `DockPanel.Dock` | `Top` | 위쪽에 붙임 |
| `DockPanel.Dock` | `Bottom` | 아래쪽에 붙임 |
| `DockPanel.Dock` | `Left` | 왼쪽에 붙임 |
| `DockPanel.Dock` | `Right` | 오른쪽에 붙임 |
| `LastChildFill` | `True` (기본값) | 마지막 자식이 남은 공간 전부 채움 |

---

### 💻 예제 3 — DockPanel로 앱 기본 틀 만들기

> 🎯 **목표:** 상단 메뉴바 + 하단 상태바 + 중앙 본문 버튼으로 구성된
> 앱의 기본 화면 틀을 만듭니다. 각 영역 버튼 클릭 시 위치를 알려줍니다.

#### 📄 `MainWindow.xaml`

```xml
<Window x:Class="WpfLayoutApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="DockPanel 예제" Height="300" Width="400">

    <!-- DockPanel: 가장자리에 컨트롤을 붙이는 레이아웃 -->
    <!-- LastChildFill="True": 마지막 자식이 남은 공간을 모두 채움 -->
    <DockPanel LastChildFill="True">

        <!-- DockPanel.Dock="Top": 창 상단에 붙임 → 메뉴바 역할 -->
        <Button DockPanel.Dock="Top"
                Content="📋 상단 메뉴바"
                Height="40"
                Click="btnTop_Click"/>

        <!-- DockPanel.Dock="Bottom": 창 하단에 붙임 → 상태바 역할 -->
        <Button DockPanel.Dock="Bottom"
                Content="🔵 하단 상태바"
                Height="35"
                Click="btnBottom_Click"/>

        <!-- DockPanel.Dock="Left": 창 왼쪽에 붙임 → 사이드바 역할 -->
        <Button DockPanel.Dock="Left"
                Content="◀ 왼쪽"
                Width="80"
                Click="btnLeft_Click"/>

        <!-- 마지막 자식: Dock 지정 없어도 남은 중앙 공간을 전부 채움 -->
        <!-- LastChildFill="True" 덕분에 자동으로 중앙 영역을 가득 채움 -->
        <Button Content="⬜ 중앙 본문 영역"
                Click="btnCenter_Click"/>

    </DockPanel>

</Window>
```

#### 📄 `MainWindow.xaml.cs`

```csharp
using System.Windows;
namespace WpfLayoutApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();      // XAML 컨트롤 생성
        }

        // 상단 메뉴바 버튼 클릭 시 실행
        private void btnTop_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("상단 메뉴바 영역입니다.");
        }

        // 하단 상태바 버튼 클릭 시 실행
        private void btnBottom_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("하단 상태바 영역입니다.");
        }

        // 왼쪽 사이드바 버튼 클릭 시 실행
        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("왼쪽 사이드바 영역입니다.");
        }

        // 중앙 본문 버튼 클릭 시 실행
        private void btnCenter_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("중앙 본문 영역입니다.");
        }
    }
}
```

#### ▶️ 실행 결과

```
┌──────────────────────────────────────┐
│  DockPanel 예제                - □ X │
├──────────────────────────────────────┤
│  ┌────────────────────────────────┐  │
│  │        📋 상단 메뉴바          │  │  ← DockPanel.Dock="Top"
│  └────────────────────────────────┘  │
│  ┌───────┐ ┌─────────────────────┐  │
│  │       │ │                     │  │
│  │ ◀왼쪽│ │   ⬜ 중앙 본문 영역  │  │  ← 마지막 자식 (남은 공간 전부)
│  │       │ │                     │  │
│  └───────┘ └─────────────────────┘  │  ← DockPanel.Dock="Left"
│  ┌────────────────────────────────┐  │
│  │        🔵 하단 상태바          │  │  ← DockPanel.Dock="Bottom"
│  └────────────────────────────────┘  │
└──────────────────────────────────────┘
```

---

## 🔀 레이아웃 중첩 — Grid 안에 StackPanel

실제 앱에서는 레이아웃 패널을 **중첩**해서 사용합니다.

### 💻 예제 4 — 입력 폼 만들기

> 🎯 **목표:** Grid로 전체 구조를 나누고, 하단에 StackPanel로 버튼 2개(확인/취소)를 가로로 배치합니다.
> 확인 클릭 시 "저장되었습니다!", 취소 클릭 시 창이 닫힙니다.

#### 📄 `MainWindow.xaml`

```xml
<Window x:Class="WpfLayoutApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="입력 폼" Height="220" Width="350">

    <!-- 바깥 구조: Grid 2행으로 나눔 (위: 입력 영역 / 아래: 버튼 영역) -->
    <Grid Margin="10">

        <Grid.RowDefinitions>
            <!-- 0행: 남은 공간 전부 (입력 영역) -->
            <RowDefinition Height="*"/>
            <!-- 1행: 내용 크기에 맞게 자동 (버튼 영역) -->
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- 0행: 안내 텍스트 (TextBlock: 읽기 전용 텍스트 표시 컨트롤) -->
        <!-- VerticalAlignment="Center": 셀 안에서 세로 가운데 정렬 -->
        <!-- HorizontalAlignment="Center": 셀 안에서 가로 가운데 정렬 -->
        <TextBlock Grid.Row="0"
                   Text="여기에 입력 컨트롤이 들어갑니다."
                   VerticalAlignment="Center"
                   HorizontalAlignment="Center"
                   FontSize="14"/>

        <!-- 1행: StackPanel로 버튼 2개를 가로로 배치 -->
        <!-- Orientation="Horizontal": 왼쪽에서 오른쪽으로 나열 -->
        <!-- HorizontalAlignment="Right": 버튼 묶음을 오른쪽 정렬 -->
        <StackPanel Grid.Row="1"
                    Orientation="Horizontal"
                    HorizontalAlignment="Right"
                    Margin="0,10,0,0">

            <!-- 확인 버튼: 오른쪽에 8px 여백으로 취소 버튼과 간격 확보 -->
            <Button Content="✔ 확인"
                    Width="90" Height="35"
                    Margin="0,0,8,0"
                    Click="btnOk_Click"/>

            <!-- 취소 버튼 -->
            <Button Content="✖ 취소"
                    Width="90" Height="35"
                    Click="btnCancel_Click"/>

        </StackPanel>

    </Grid>

</Window>
```

#### 📄 `MainWindow.xaml.cs`

```csharp
using System.Windows;
namespace WpfLayoutApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();      // XAML 컨트롤 생성
        }

        // 확인 버튼 클릭 시 실행
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("저장되었습니다!");  // 저장 완료 메시지 출력
        }

        // 취소 버튼 클릭 시 실행
        // this.Close(): 현재 창을 닫아서 프로그램을 종료함
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();               // 창 닫기
        }
    }
}
```

#### ▶️ 실행 결과

```
  실행 직후                         확인 클릭 후
┌───────────────────────────┐      ┌───────────────────────────┐
│  입력 폼             - □ X│      │  입력 폼             - □ X│
├───────────────────────────┤      ├───────────────────────────┤
│                           │      │  ┌─────────────────────┐  │
│  여기에 입력 컨트롤이      │      │  │  ℹ️              X  │  │
│  들어갑니다.              │ 클릭 │  │                     │  │
│                           │ ───► │  │   저장되었습니다!   │  │
│           ┌────┐  ┌────┐  │      │  │                     │  │
│           │ 확인│  │ 취소│ │      │  │       [ 확인 ]      │  │
│           └────┘  └────┘  │      │  └─────────────────────┘  │
└───────────────────────────┘      └───────────────────────────┘

                                    취소 클릭 후 → 창이 닫히며 종료
```

---

## 📝 핵심 포인트

- `Grid`는 행/열 표 형태로 복잡한 레이아웃 구성에 사용합니다.
- `StackPanel`은 컨트롤을 순서대로 세로/가로로 쌓을 때 사용합니다.
- `DockPanel`은 상단 메뉴바, 하단 상태바 같은 가장자리 배치에 사용합니다.
- 레이아웃 패널은 **중첩해서** 사용할 수 있습니다 (Grid 안에 StackPanel 등).
- `Margin="좌,상,우,하"` 로 컨트롤 간 여백을 조절합니다.
- `Grid.Row`, `Grid.Column` 은 **0부터** 시작합니다.

---

## 📝 문제

---

### 문제 1

Grid에서 행과 열 번호는 몇 번부터 시작하나요?

**정답:**
<details>
<summary>정답 보기 (클릭)</summary>

**0번** 부터 시작합니다.
첫 번째 행은 `Grid.Row="0"`, 두 번째 행은 `Grid.Row="1"` 입니다.

</details>

---

### 문제 2

다음 중 `StackPanel`에서 컨트롤을 **왼쪽에서 오른쪽**으로 나열하려면 어떤 속성을 설정해야 하나요?

```xml
<StackPanel Orientation="________">
```

**정답:**
<details>
<summary>정답 보기 (클릭)</summary>

```xml
<StackPanel Orientation="Horizontal">
```

`Vertical`(기본값)은 위→아래, `Horizontal`은 좌→우 방향입니다.

</details>

---

### 문제 3

`DockPanel`에서 버튼을 창 하단에 붙이려면 어떤 속성을 설정해야 하나요?

```xml
<Button DockPanel.Dock="________" Content="하단 버튼"/>
```

**정답:**
<details>
<summary>정답 보기 (클릭)</summary>

```xml
<Button DockPanel.Dock="Bottom" Content="하단 버튼"/>
```

</details>

---

### 문제 4

다음 코드는 Grid 3행을 정의합니다. 각 행의 높이가 어떻게 되는지 설명하세요.

```xml
<Grid.RowDefinitions>
    <RowDefinition Height="60"/>
    <RowDefinition Height="Auto"/>
    <RowDefinition Height="*"/>
</Grid.RowDefinitions>
```

**정답:**
<details>
<summary>정답 보기 (클릭)</summary>

```
0행: Height="60"   → 항상 60px 고정
1행: Height="Auto" → 그 행 안에 있는 컨트롤의 크기에 맞게 자동 조절
2행: Height="*"    → 0행과 1행을 제외한 나머지 공간을 전부 차지
```

```
┌─────────────────┐
│   60px 고정     │  ← 0행
├─────────────────┤
│  내용 크기만큼  │  ← 1행 (Auto)
├─────────────────┤
│                 │
│  나머지 전부    │  ← 2행 (*)
│                 │
└─────────────────┘
```

</details>

---

### 문제 5

아래는 3×1 Grid를 만드는 코드입니다.
빈칸을 채워 각 행에 버튼을 배치하고, 클릭 시 행 번호 메시지가 나오도록 완성하세요.

#### 📄 `MainWindow.xaml`

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="*"/>
        <RowDefinition Height="*"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>

    <Button Grid.Row="__" Content="0번 행 버튼" Margin="10" Click="btn0_Click"/>
    <Button Grid.Row="__" Content="1번 행 버튼" Margin="10" Click="btn1_Click"/>
    <Button Grid.Row="__" Content="2번 행 버튼" Margin="10" Click="btn2_Click"/>
</Grid>
```

#### 📄 `MainWindow.xaml.cs`

```csharp
private void btn0_Click(object sender, RoutedEventArgs e)
{
    MessageBox.Show("________");
}
private void btn1_Click(object sender, RoutedEventArgs e)
{
    MessageBox.Show("________");
}
private void btn2_Click(object sender, RoutedEventArgs e)
{
    MessageBox.Show("________");
}
```

**정답:**
<details>
<summary>정답 보기 (클릭)</summary>

#### 📄 `MainWindow.xaml`

```xml
<Button Grid.Row="0" Content="0번 행 버튼" Margin="10" Click="btn0_Click"/>
<Button Grid.Row="1" Content="1번 행 버튼" Margin="10" Click="btn1_Click"/>
<Button Grid.Row="2" Content="2번 행 버튼" Margin="10" Click="btn2_Click"/>
```

#### 📄 `MainWindow.xaml.cs`

```csharp
private void btn0_Click(object sender, RoutedEventArgs e)
{
    MessageBox.Show("0번 행 버튼을 클릭했습니다.");
}
private void btn1_Click(object sender, RoutedEventArgs e)
{
    MessageBox.Show("1번 행 버튼을 클릭했습니다.");
}
private void btn2_Click(object sender, RoutedEventArgs e)
{
    MessageBox.Show("2번 행 버튼을 클릭했습니다.");
}
```

#### ▶️ 실행 결과

```
  실행 직후                  1번 행 클릭 후
┌──────────────────┐        ┌──────────────────┐
│  앱         - □X │        │  앱         - □X │
├──────────────────┤        ├──────────────────┤
│ ┌──────────────┐ │        │ ┌──────────────┐ │
│ │ 0번 행 버튼  │ │        │ │ 0번 행 버튼  │ │
│ └──────────────┘ │        │ └──────────────┘ │
│ ┌──────────────┐ │  클릭  │ ┌─────────────┐  │
│ │ 1번 행 버튼  │ │ ─────► │ │ ℹ️        X │  │
│ └──────────────┘ │        │ │1번 행 버튼을│  │
│ ┌──────────────┐ │        │ │클릭했습니다 │  │
│ │ 2번 행 버튼  │ │        │ │  [ 확인 ]  │  │
│ └──────────────┘ │        │ └─────────────┘  │
└──────────────────┘        └──────────────────┘
```

</details>

---

> 📌 **Tip:** 실제 WPF 앱은 대부분 `Grid`를 바깥 틀로 쓰고, 그 안에 `StackPanel`이나 또 다른 `Grid`를 중첩해서 화면을 구성합니다.
> 레이아웃 설계가 잘 되어야 나중에 컨트롤을 추가하거나 수정할 때 편합니다!
