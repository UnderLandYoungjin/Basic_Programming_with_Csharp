# WPF 실습 가이드 -- 미니 계산기 만들기

> **과정명:** C# WPF 프로그래밍 입문  
> **실습 목표:** WPF 프로젝트 생성 -> 파일 분할 설계 -> 빌드 -> 인스톨러 배포까지 전체 흐름 경험  
> **소요 시간:** 약 60~90분  
> **개발 환경:** Visual Studio 2022/2026, .NET 8 이상, Windows 10/11

# WPF 프로그램 패키징 및 배포

## 참고 자료

[WPF로 프로그램 개발후 팩키징. setup파일로 배포하기 위한 툴 : 네이버 카페](https://cafe.naver.com/f-e/cafes/30977017/articles/819?boardtype=L&menuid=55&referrerAllArticles=false)

---

## 목차

1. [WPF란 무엇인가?](#1-wpf란-무엇인가)
2. [WinForms vs WPF 비교](#2-winforms-vs-wpf-비교)
3. [프로젝트 생성](#3-프로젝트-생성)
4. [프로젝트 구조 이해](#4-프로젝트-구조-이해)
5. [코드 작성 -- 파일별 역할과 전체 소스](#5-코드-작성----파일별-역할과-전체-소스)
   - 5.1 [App.xaml -- 앱 진입점](#51-appxaml----앱-진입점)
   - 5.2 [App.xaml.cs -- 앱 코드비하인드](#52-appxamlcs----앱-코드비하인드)
   - 5.3 [CalcEngine.cs -- 계산 로직 (비즈니스 로직 분리)](#53-calcenginecs----계산-로직-비즈니스-로직-분리)
   - 5.4 [MainWindow.xaml -- UI 화면 구성 (XAML)](#54-mainwindowxaml----ui-화면-구성-xaml)
   - 5.5 [MainWindow.xaml.cs -- 이벤트 처리 (코드비하인드)](#55-mainwindowxamlcs----이벤트-처리-코드비하인드)
6. [XAML 핵심 개념 정리](#6-xaml-핵심-개념-정리)
7. [빌드 및 실행](#7-빌드-및-실행)
8. [배포 -- 단일 EXE 만들기](#8-배포----단일-exe-만들기)
9. [배포 -- Inno Setup으로 인스톨러 만들기](#9-배포----inno-setup으로-인스톨러-만들기)
10. [정리 및 과제](#10-정리-및-과제)

---

## 1. WPF란 무엇인가?

**WPF (Windows Presentation Foundation)** 는 마이크로소프트에서 만든 데스크톱 UI 프레임워크입니다.

- **XAML**이라는 마크업 언어로 UI를 선언적으로 구성합니다.
- HTML처럼 태그로 화면을 만들고, C# 코드로 동작을 처리합니다.
- DirectX 기반 렌더링으로 WinForms보다 훨씬 세련된 UI를 만들 수 있습니다.

### 핵심 키워드

| 용어 | 설명 |
|------|------|
| **XAML** | eXtensible Application Markup Language. UI를 XML 형태로 정의 |
| **코드비하인드** | `.xaml` 파일과 쌍을 이루는 `.xaml.cs` 파일. 이벤트 처리 담당 |
| **partial class** | XAML과 C# 코드가 컴파일 시 하나의 클래스로 합쳐지는 구조 |
| **데이터 바인딩** | UI 요소와 데이터를 자동으로 연결하는 WPF의 강력한 기능 |

---

## 2. WinForms vs WPF 비교

여러분이 이전에 배운 WinForms와 비교하면 이해가 쉽습니다.

| 항목 | WinForms | WPF |
|------|----------|-----|
| UI 정의 방식 | C# 코드 또는 디자이너 드래그앤드롭 | **XAML** (선언적 마크업) |
| 레이아웃 | 좌표 기반 (`Location`, `Size`) | **패널 기반** (`Grid`, `StackPanel` 등) |
| 이벤트 처리 | 동일 (이벤트 핸들러) | 동일 (이벤트 핸들러) |
| 그래픽 엔진 | GDI+ | **DirectX** |
| 데이터 바인딩 | 제한적 | **강력한 바인딩 시스템** |
| 스타일/테마 | 제한적 | **Style, Template으로 자유롭게 커스텀** |
| 확장성 | 단순한 앱에 적합 | 복잡하고 세련된 앱에 적합 |

**공통점:** 둘 다 C#으로 동작하고, 이벤트 핸들러 방식은 동일합니다.  
**차이점:** WPF는 UI를 XAML로 분리하고, 레이아웃을 패널 기반으로 구성합니다.

---

## 3. 프로젝트 생성

### 단계별 진행

**1) Visual Studio 실행 -> "새 프로젝트 만들기"**

**2) 템플릿 검색창에 "WPF" 입력**

아래 목록에서 **"WPF 애플리케이션"** 을 선택합니다.

> 주의: "WPF 애플리케이션(.NET Framework)"이 아닌,
> **".NET WPF 애플리케이션 만들기 프로젝트"** 를 선택하세요.

**3) 프로젝트 설정**

| 항목 | 입력값 |
|------|--------|
| 프로젝트 이름 | `WpfApp1` (또는 원하는 이름) |
| 위치 | `C:\cs\` (또는 원하는 경로) |
| 프레임워크 | .NET 8.0 이상 |

**4) "만들기" 클릭**

프로젝트가 생성되면 `MainWindow.xaml`이 자동으로 열립니다.

---

## 4. 프로젝트 구조 이해

Visual Studio 솔루션 탐색기에서 아래와 같은 구조를 확인할 수 있습니다.

```
WpfApp1/                     <-- 솔루션 폴더
+-- WpfApp1/                  <-- 프로젝트 폴더
    +-- App.xaml              <-- 앱 진입점 (Application 정의)
    +-- App.xaml.cs           <-- 앱 코드비하인드
    +-- AssemblyInfo.cs       <-- 어셈블리 정보 (자동 생성, 수정 불필요)
    +-- MainWindow.xaml       <-- 메인 화면 UI (XAML로 구성)
    +-- MainWindow.xaml.cs    <-- 메인 화면 이벤트 처리
    +-- CalcEngine.cs         <-- (*) 우리가 추가할 파일 (계산 로직)
```

### 파일별 역할 요약

| 파일 | 역할 | 비유 |
|------|------|------|
| `App.xaml` | 앱 전체 설정 (시작 창, 공용 스타일) | 건물의 설계도 |
| `MainWindow.xaml` | 화면 레이아웃 (버튼, 텍스트 배치) | 건물의 인테리어 |
| `MainWindow.xaml.cs` | 버튼 클릭 등 이벤트 처리 | 전기 배선 (동작 연결) |
| `CalcEngine.cs` | 순수 계산 로직 (UI 코드 없음) | 건물의 엔진룸 |

> **왜 파일을 분리하는가?**  
> UI 코드와 비즈니스 로직을 분리하면 유지보수가 쉬워집니다.  
> `CalcEngine.cs`는 WPF를 몰라도 이해할 수 있는 순수 C# 클래스입니다.  
> 이런 설계 방식이 나중에 **MVVM 패턴**의 기초가 됩니다.

---

## 5. 코드 작성 -- 파일별 역할과 전체 소스

### 5.1 App.xaml -- 앱 진입점

`App.xaml`은 WPF 앱의 시작점입니다. `StartupUri`로 처음 열릴 창을 지정합니다.

자동 생성된 `App.xaml`의 내용을 아래로 **전체 교체**합니다.

```xml
<Application x:Class="WpfApp1.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
         
    </Application.Resources>
</Application>
```

| 속성 | 설명 |
|------|------|
| `x:Class` | 이 XAML과 연결되는 C# 클래스 (네임스페이스.클래스명) |
| `StartupUri` | 앱 시작 시 열리는 첫 번째 창 |
| `Application.Resources` | 앱 전체에서 공유하는 스타일/리소스 정의 영역 |

---

### 5.2 App.xaml.cs -- 앱 코드비하인드

보통 수정하지 않습니다. 앱 시작/종료 이벤트가 필요할 때 여기에 작성합니다.

```csharp
using System.Windows;

namespace WpfApp1
{
    public partial class App : Application
    {
    }
}
```

> **partial class란?**  
> `App.xaml`(XAML)과 `App.xaml.cs`(C#)가 컴파일 시 하나의 `App` 클래스로 합쳐집니다.  
> WPF의 모든 `.xaml` + `.xaml.cs` 쌍이 이 구조입니다.

---

### 5.3 CalcEngine.cs -- 계산 로직 (비즈니스 로직 분리)

**새 파일 추가 방법:**  
솔루션 탐색기 -> WpfApp1 프로젝트 **우클릭** -> **추가** -> **클래스** -> 이름: `CalcEngine.cs` -> 추가

자동 생성된 내용을 **전부 삭제**하고 아래 코드를 붙여넣습니다.

```csharp
namespace WpfApp1
{
    /// <summary>
    /// 계산 로직 담당 클래스
    /// - UI(화면)와 완전히 분리된 순수 C# 클래스
    /// - 이 클래스는 WPF를 몰라도 이해할 수 있습니다
    /// </summary>
    public class CalcEngine
    {
        // ============================================================
        //  [1] 상태를 저장하는 변수들 (필드)
        // ============================================================

        private double num1;          // 첫 번째 숫자  (예: 3 + 5 에서 3)
        private double num2;          // 두 번째 숫자  (예: 3 + 5 에서 5)
        private string op = "";       // 연산자 저장    ("+", "-", "x", "/" 중 하나)
        private bool wait = true;     // true면 "새 숫자 입력 대기중" 이라는 뜻

        // ============================================================
        //  [2] 화면에 보여줄 값 (속성)
        //      - get : 외부에서 읽기 가능
        //      - private set : 이 클래스 안에서만 쓰기 가능
        // ============================================================

        /// <summary>화면 메인 숫자 (큰 글씨)</summary>
        public string Display { get; private set; } = "0";

        /// <summary>화면 위쪽 수식 (작은 글씨, 예: "3 + 5 =")</summary>
        public string Formula { get; private set; } = "";

        /// <summary>새 숫자 입력 대기중인지 여부</summary>
        public bool IsWait { get { return wait; } }


        // ============================================================
        //  [3] 숫자 입력  (0~9, 소수점)
        // ============================================================
        /// <summary>
        /// 사용자가 숫자 버튼(0~9) 또는 소수점(.)을 눌렀을 때 호출
        /// </summary>
        /// <param name="d">눌린 버튼의 텍스트 ("0"~"9" 또는 ".")</param>
        public void AddDigit(string d)
        {
            // --- 새 숫자를 시작하는 경우 ---
            if (wait)
            {
                // 소수점부터 시작하면 "0."으로 표시
                if (d == ".")
                    Display = "0.";
                else
                    Display = d;   // 눌린 숫자를 화면에 바로 표시

                wait = false;      // 이제 입력중 상태로 전환
                return;            // 여기서 끝
            }

            // --- 이어서 숫자를 붙이는 경우 ---

            // 소수점이 이미 있으면 또 찍지 않음
            if (d == "." && Display.Contains("."))
                return;

            // 기존 숫자 뒤에 이어붙이기  (예: "12" + "3" = "123")
            Display = Display + d;
        }


        // ============================================================
        //  [4] 연산자 입력  (+, -, x, /)
        // ============================================================
        /// <summary>
        /// 사용자가 연산자 버튼(+, -, x, /)을 눌렀을 때 호출
        /// </summary>
        /// <param name="o">눌린 연산자 문자열</param>
        public void SetOp(string o)
        {
            // 이미 이전 연산이 있고, 숫자도 새로 입력한 상태라면
            // -> 먼저 이전 계산을 수행 (예: 3 + 5 + ... 에서 3+5를 먼저 계산)
            if (op != "" && wait == false)
            {
                num2 = double.Parse(Display);  // 화면 숫자를 두번째 숫자로
                Calc();                         // 계산 실행
                num1 = double.Parse(Display);  // 결과를 첫번째 숫자로 저장
            }
            else
            {
                // 처음 연산자를 누르는 경우
                num1 = double.Parse(Display);  // 화면 숫자를 첫번째 숫자로 저장
            }

            op = o;                                // 연산자 저장
            Formula = num1 + " " + op;             // 수식 표시 (예: "3 +")
            wait = true;                           // 다음 숫자 입력 대기
        }


        // ============================================================
        //  [5] = (등호) 실행
        // ============================================================
        /// <summary>
        /// 사용자가 = 버튼을 눌렀을 때 호출
        /// </summary>
        public void Equal()
        {
            // 연산자가 없으면 아무것도 안 함
            if (op == "")
                return;

            num2 = double.Parse(Display);                  // 화면 숫자 -> 두번째 숫자
            Formula = num1 + " " + op + " " + num2 + " ="; // 수식 완성 (예: "3 + 5 =")
            Calc();                                         // 계산 실행
            op = "";                                        // 연산자 초기화
            wait = true;                                    // 다음 입력 대기
        }


        // ============================================================
        //  [6] C (전체 초기화)
        // ============================================================
        /// <summary>
        /// 모든 상태를 처음으로 되돌림
        /// </summary>
        public void Clear()
        {
            num1 = 0;
            num2 = 0;
            op = "";
            Display = "0";
            Formula = "";
            wait = true;
        }


        // ============================================================
        //  [7] +/- (부호 반전)
        // ============================================================
        /// <summary>
        /// 화면 숫자의 부호를 뒤집음 (양수 <-> 음수)
        /// </summary>
        public void FlipSign()
        {
            // 0이면 부호 반전할 필요 없음
            if (Display == "0")
                return;

            double val = double.Parse(Display);  // 문자열 -> 숫자
            val = val * -1;                       // 부호 반전
            Display = val.ToString();             // 숫자 -> 문자열로 다시 저장
        }


        // ============================================================
        //  [8] 백스페이스 (한 글자 지우기)
        // ============================================================
        /// <summary>
        /// 화면 숫자의 맨 끝 한 글자를 지움
        /// </summary>
        public void Back()
        {
            // 새 입력 대기중이면 지울 게 없음
            if (wait)
                return;

            // 글자가 2개 이상이면 맨 뒤 한 글자 제거
            if (Display.Length > 1)
            {
                // Substring(시작, 길이) : 0번째부터 (전체길이-1)개 만큼 잘라냄
                // 예: "123" -> Substring(0, 2) -> "12"
                Display = Display.Substring(0, Display.Length - 1);
            }
            else
            {
                // 한 글자만 남았으면 "0"으로 초기화
                Display = "0";
                wait = true;
            }
        }


        // ============================================================
        //  [9] 실제 계산 (내부에서만 사용하는 private 메서드)
        // ============================================================
        /// <summary>
        /// num1과 num2를 op에 따라 계산하고 결과를 Display에 저장
        /// </summary>
        private void Calc()
        {
            double result = 0;

            // 연산자에 따라 분기
            if (op == "+")
                result = num1 + num2;
            else if (op == "-")
                result = num1 - num2;
            else if (op == "\u00D7")      // "x" (곱하기 기호, 유니코드)
                result = num1 * num2;
            else if (op == "\u00F7")      // "/" (나누기 기호, 유니코드)
            {
                // 0으로 나누기 방지
                if (num2 == 0)
                {
                    Display = "오류";
                    wait = true;
                    return;               // 여기서 바로 종료
                }
                result = num1 / num2;
            }

            // 결과를 화면 표시용 문자열로 변환
            // "G10" : 유효숫자 10자리까지 표시 (너무 긴 소수점 방지)
            Display = result.ToString("G10");
        }
    }
}
```

### CalcEngine.cs 핵심 포인트

| 문법 | 설명 | 예시 |
|------|------|------|
| `{ get; private set; }` | 외부에서 읽기만 가능, 쓰기는 클래스 내부만 | `Display` |
| `double.Parse(문자열)` | 문자열을 숫자로 변환 | `double.Parse("3.14")` -> 3.14 |
| `ToString("G10")` | 숫자를 문자열로 변환 (유효숫자 10자리) | `3.14159.ToString("G10")` |
| `Substring(시작, 길이)` | 문자열의 일부분을 잘라냄 | `"123".Substring(0, 2)` -> `"12"` |
| `Contains(".")` | 문자열에 특정 문자가 있는지 확인 | `"3.14".Contains(".")` -> true |
| `private` 메서드 | 클래스 안에서만 호출 가능 | `Calc()`는 외부에서 호출 불가 |

---

### 5.4 MainWindow.xaml -- UI 화면 구성 (XAML)

자동 생성된 `MainWindow.xaml`을 **전체 교체**합니다.

```xml
<Window x:Class="WpfApp1.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="미니 계산기" 
        Height="500" Width="320"
        MinHeight="400" MinWidth="280"
        WindowStartupLocation="CenterScreen"
        Background="#FF1E1E1E">

    <!-- ==========================================
         Window.Resources: 이 창에서 사용할 스타일 정의
         - x:Key로 이름을 붙이고
         - Style="{StaticResource 이름}" 으로 적용
         ========================================== -->
    <Window.Resources>
        <!-- 숫자 버튼 스타일 (어두운 회색) -->
        <Style x:Key="NumBtn" TargetType="Button">
            <Setter Property="FontSize" Value="20"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="Margin" Value="3"/>
            <Setter Property="Background" Value="#FF3C3C3C"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Cursor" Value="Hand"/>
        </Style>

        <!-- 연산자 버튼 스타일 (파란색) -->
        <Style x:Key="OpBtn" TargetType="Button">
            <Setter Property="FontSize" Value="20"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="Margin" Value="3"/>
            <Setter Property="Background" Value="#FF0078D4"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Cursor" Value="Hand"/>
        </Style>

        <!-- 기능 버튼 스타일 (C, +/-, 백스페이스) -->
        <Style x:Key="FuncBtn" TargetType="Button">
            <Setter Property="FontSize" Value="20"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="Margin" Value="3"/>
            <Setter Property="Background" Value="#FF505050"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Cursor" Value="Hand"/>
        </Style>

        <!-- = 버튼 스타일 (초록색) -->
        <Style x:Key="EqBtn" TargetType="Button">
            <Setter Property="FontSize" Value="20"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="Margin" Value="3"/>
            <Setter Property="Background" Value="#FF107C10"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Cursor" Value="Hand"/>
        </Style>
    </Window.Resources>

    <!-- ==========================================
         Grid: WPF의 핵심 레이아웃 패널
         - RowDefinitions: 행 정의
         - Auto: 내용 크기에 맞춤
         - *: 남은 공간을 비율로 분배
         ========================================== -->
    <Grid Margin="8">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>   <!-- Row 0: 수식 표시 -->
            <RowDefinition Height="Auto"/>   <!-- Row 1: 메인 디스플레이 -->
            <RowDefinition Height="*"/>      <!-- Row 2: 버튼 영역 (나머지 전부) -->
        </Grid.RowDefinitions>

        <!-- ===== 수식 표시 (Row 0) ===== -->
        <TextBlock x:Name="txtFormula"
                   Grid.Row="0"
                   Text=""
                   FontSize="14"
                   Foreground="Gray"
                   HorizontalAlignment="Right"
                   Margin="5,5,10,0"/>

        <!-- ===== 메인 디스플레이 (Row 1) ===== -->
        <!--
            x:Name="txtDisplay" -> C# 코드에서 txtDisplay로 접근 가능
            HorizontalAlignment="Right" -> 오른쪽 정렬 (계산기답게)
            TextTrimming -> 글자가 넘치면 ... 으로 표시
        -->
        <TextBlock x:Name="txtDisplay"
                   Grid.Row="1"
                   Text="0"
                   FontSize="42"
                   FontWeight="Light"
                   Foreground="White"
                   HorizontalAlignment="Right"
                   Margin="5,5,10,10"
                   TextTrimming="CharacterEllipsis"/>

        <!-- ===== 버튼 영역 (Row 2) ===== -->
        <!--
            UniformGrid: 모든 셀이 동일한 크기
            - Rows="5" Columns="4" -> 5행 4열
            - 자식 컨트롤이 왼->오, 위->아래 순서로 배치
        -->
        <UniformGrid Grid.Row="2" Rows="5" Columns="4">

            <!-- 1행: C, +/-, 백스페이스, / -->
            <Button Content="C"  Click="BtnClear_Click"      Style="{StaticResource FuncBtn}"/>
            <Button Content="&#xB1;"  Click="BtnSign_Click"   Style="{StaticResource FuncBtn}"/>
            <Button Content="&#x232B;" Click="BtnBack_Click"  Style="{StaticResource FuncBtn}"/>
            <Button Content="&#xF7;"  Click="BtnOp_Click"     Style="{StaticResource OpBtn}"/>

            <!-- 2행: 7, 8, 9, x -->
            <Button Content="7"  Click="BtnNum_Click"        Style="{StaticResource NumBtn}"/>
            <Button Content="8"  Click="BtnNum_Click"        Style="{StaticResource NumBtn}"/>
            <Button Content="9"  Click="BtnNum_Click"        Style="{StaticResource NumBtn}"/>
            <Button Content="&#xD7;"  Click="BtnOp_Click"    Style="{StaticResource OpBtn}"/>

            <!-- 3행: 4, 5, 6, - -->
            <Button Content="4"  Click="BtnNum_Click"        Style="{StaticResource NumBtn}"/>
            <Button Content="5"  Click="BtnNum_Click"        Style="{StaticResource NumBtn}"/>
            <Button Content="6"  Click="BtnNum_Click"        Style="{StaticResource NumBtn}"/>
            <Button Content="-"  Click="BtnOp_Click"         Style="{StaticResource OpBtn}"/>

            <!-- 4행: 1, 2, 3, + -->
            <Button Content="1"  Click="BtnNum_Click"        Style="{StaticResource NumBtn}"/>
            <Button Content="2"  Click="BtnNum_Click"        Style="{StaticResource NumBtn}"/>
            <Button Content="3"  Click="BtnNum_Click"        Style="{StaticResource NumBtn}"/>
            <Button Content="+"  Click="BtnOp_Click"         Style="{StaticResource OpBtn}"/>

            <!-- 5행: 00, 0, ., = -->
            <Button Content="00" Click="BtnNum_Click"        Style="{StaticResource NumBtn}"/>
            <Button Content="0"  Click="BtnNum_Click"        Style="{StaticResource NumBtn}"/>
            <Button Content="."  Click="BtnNum_Click"        Style="{StaticResource NumBtn}"/>
            <Button Content="="  Click="BtnEqual_Click"      Style="{StaticResource EqBtn}"/>

        </UniformGrid>
    </Grid>
</Window>
```

### XAML 핵심 포인트

| XAML 요소 | 설명 |
|-----------|------|
| `x:Class="WpfApp1.MainWindow"` | 이 XAML이 연결되는 C# 클래스 |
| `x:Name="txtDisplay"` | C# 코드에서 이 컨트롤에 접근할 이름 |
| `Grid.Row="1"` | Grid 패널에서 배치될 행 번호 (0부터 시작) |
| `Style="{StaticResource NumBtn}"` | Window.Resources에 정의한 스타일 적용 |
| `Click="BtnNum_Click"` | 클릭 시 호출할 이벤트 핸들러 메서드 이름 |
| `UniformGrid` | 모든 칸이 동일 크기인 격자 레이아웃 |

---

### 5.5 MainWindow.xaml.cs -- 이벤트 처리 (코드비하인드)

자동 생성된 `MainWindow.xaml.cs`를 **전체 교체**합니다.

```csharp
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml의 코드비하인드
    /// 
    /// [역할]
    /// - XAML 화면에서 버튼을 클릭하면 여기 있는 메서드가 호출됩니다.
    /// - 실제 계산은 CalcEngine에게 맡기고,
    ///   이 파일은 "버튼 클릭 -> 엔진 호출 -> 화면 갱신" 만 담당합니다.
    /// </summary>
    public partial class MainWindow : Window
    {
        // ============================================================
        //  계산 엔진 객체 생성
        //  - 이 객체 하나가 계산기의 "두뇌" 역할을 합니다
        //  - readonly : 한번 만들면 교체 불가 (안전장치)
        // ============================================================
        private readonly CalcEngine eng = new CalcEngine();

        /// <summary>
        /// 생성자 - 창이 처음 만들어질 때 한 번 실행됩니다.
        /// </summary>
        public MainWindow()
        {
            // XAML에 정의된 버튼, 텍스트 등의 컨트롤을 초기화
            // 이 줄이 없으면 txtDisplay, txtFormula 등을 사용할 수 없음!
            InitializeComponent();
        }


        // ============================================================
        //  숫자 버튼 클릭  (0~9, 00, .)
        //  - 여러 버튼이 이 하나의 메서드를 같이 사용합니다.
        //  - sender : "어떤 버튼이 눌렸는지" 알려주는 매개변수
        // ============================================================
        private void BtnNum_Click(object sender, RoutedEventArgs e)
        {
            // sender를 Button 타입으로 변환해서 버튼 텍스트를 가져옴
            Button btn = (Button)sender;
            string txt = btn.Content.ToString()!;   // 예: "7", "00", "."

            // "00" 버튼 처리
            if (txt == "00")
            {
                // 새 입력 대기중이면 00은 무시 (0000... 방지)
                if (eng.IsWait)
                    return;

                // "0"을 두 번 입력하는 것과 같음
                eng.AddDigit("0");
                eng.AddDigit("0");
            }
            else
            {
                // 일반 숫자 또는 소수점
                eng.AddDigit(txt);
            }

            // 화면 갱신
            Refresh();
        }


        // ============================================================
        //  연산자 버튼 클릭  (+, -, x, /)
        // ============================================================
        private void BtnOp_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string txt = btn.Content.ToString()!;   // 예: "+", "-"

            eng.SetOp(txt);    // 엔진에 연산자 전달
            Refresh();         // 화면 갱신
        }


        // ============================================================
        //  = 버튼 클릭
        // ============================================================
        private void BtnEqual_Click(object sender, RoutedEventArgs e)
        {
            eng.Equal();       // 계산 실행
            Refresh();         // 화면 갱신
        }


        // ============================================================
        //  C 버튼 클릭 (전체 초기화)
        // ============================================================
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            eng.Clear();       // 모든 상태 초기화
            Refresh();         // 화면 갱신
        }


        // ============================================================
        //  +/- 버튼 클릭 (부호 반전)
        // ============================================================
        private void BtnSign_Click(object sender, RoutedEventArgs e)
        {
            eng.FlipSign();    // 양수 <-> 음수
            Refresh();         // 화면 갱신
        }


        // ============================================================
        //  백스페이스 버튼 클릭 (한 글자 지우기)
        // ============================================================
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            eng.Back();        // 맨 뒤 한 글자 제거
            Refresh();         // 화면 갱신
        }


        // ============================================================
        //  화면 갱신 메서드
        //  - CalcEngine의 상태를 XAML 컨트롤에 반영합니다
        //  - x:Name으로 지정한 이름을 그대로 사용합니다
        // ============================================================
        private void Refresh()
        {
            txtDisplay.Text = eng.Display;    // 메인 숫자 표시
            txtFormula.Text = eng.Formula;    // 수식 표시
        }
    }
}
```

### 코드비하인드 핵심 포인트

| 코드 | 설명 |
|------|------|
| `partial class MainWindow : Window` | MainWindow.xaml과 합쳐지는 부분 클래스 |
| `InitializeComponent()` | XAML의 컨트롤들을 초기화 (필수!) |
| `(Button)sender` | 이벤트를 발생시킨 컨트롤을 Button 타입으로 변환 |
| `btn.Content.ToString()!` | 버튼에 표시된 텍스트를 문자열로 가져옴 |
| `txtDisplay.Text = ...` | x:Name으로 지정한 XAML 컨트롤에 직접 접근 |

---

## 6. XAML 핵심 개념 정리

### 레이아웃 패널 종류

| 패널 | 특징 | 사용 시기 |
|------|------|----------|
| `Grid` | 행/열 기반 배치 | 가장 범용적, 복잡한 레이아웃 |
| `StackPanel` | 수직/수평으로 쌓기 | 단순한 순차 배치 |
| `UniformGrid` | 모든 칸 동일 크기 | 계산기 버튼처럼 균등 배치 |
| `WrapPanel` | 자동 줄바꿈 | 태그, 뱃지 나열 |
| `DockPanel` | 상하좌우 도킹 | 메뉴바, 상태바 레이아웃 |
| `Canvas` | 좌표 기반 (WinForms와 유사) | 드로잉, 자유 배치 |

### Grid 크기 지정 방식

| 값 | 의미 | 예시 |
|----|------|------|
| `Auto` | 내용 크기에 맞춤 | 텍스트 높이만큼 |
| `*` | 남은 공간 전부 | 버튼 영역 |
| `2*` | 남은 공간의 2배 비율 | `*`과 `2*`면 1:2 비율 |
| `100` | 고정 크기 (픽셀) | 정확히 100px |

### Style과 StaticResource

```xml
<!-- 1. 스타일 정의 (Window.Resources 안에) -->
<Style x:Key="NumBtn" TargetType="Button">
    <Setter Property="FontSize" Value="20"/>
</Style>

<!-- 2. 스타일 적용 (컨트롤에서) -->
<Button Style="{StaticResource NumBtn}" Content="7"/>
```

CSS 클래스와 비슷한 개념입니다. 한 번 정의하면 여러 컨트롤에 재사용할 수 있습니다.

---

## 7. 빌드 및 실행

### 솔루션 탐색기 확인

빌드 전에 아래 파일이 모두 있는지 확인합니다.

```
WpfApp1
  +-- 종속성
  +-- App.xaml
  |    +-- App.xaml.cs
  +-- AssemblyInfo.cs
  +-- CalcEngine.cs          <-- 이 파일이 있어야 함!
  +-- MainWindow.xaml
       +-- MainWindow.xaml.cs
```

### 빌드

| 단축키 | 동작 |
|--------|------|
| `Ctrl + Shift + B` | 빌드 (컴파일) |
| `F5` | 디버그 모드 실행 |
| `Ctrl + F5` | 디버그 없이 실행 (더 빠름) |

### 오류 발생 시 체크리스트

| 에러 메시지 | 원인 | 해결 |
|------------|------|------|
| `'CalcEngine' 형식을 찾을 수 없습니다` | CalcEngine.cs가 프로젝트에 없음 | 프로젝트 우클릭 -> 추가 -> 클래스 |
| `XDG0000` (XAML 파싱 오류) | XAML 문법 오류 | XAML 코드를 전체 교체 |
| `x:Class 불일치` | 네임스페이스가 다름 | `x:Class="WpfApp1.MainWindow"` 확인 |

---

## 8. 배포 -- 단일 EXE 만들기

다른 컴퓨터에서 실행하려면 **publish**가 필요합니다.

### 방법: 명령줄에서 publish

**1) 터미널 열기**

Visual Studio에서 프로젝트 **우클릭** -> **터미널에서 열기**  
또는 CMD에서 프로젝트 폴더로 이동합니다.

```
cd C:\cs\WpfApp1
```

**2) publish 명령 실행**

```
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish
```

| 옵션 | 의미 |
|------|------|
| `-c Release` | Release 모드 (최적화된 빌드) |
| `-r win-x64` | Windows 64비트 대상 |
| `--self-contained true` | .NET 런타임 포함 (상대방 PC에 .NET 없어도 실행 가능) |
| `-o ./publish` | 출력 폴더 지정 |

**3) 결과 확인**

`publish` 폴더에 `WpfApp1.exe`를 포함한 파일들이 생성됩니다.

### 배포 방식 비교

| 방식 | 명령어 | 파일 크기 | .NET 설치 필요 |
|------|--------|----------|---------------|
| 자체 포함 | `--self-contained true` | ~150MB | 불필요 |
| 프레임워크 종속 | `--self-contained false` | ~1MB | 필요 |

---

## 9. 배포 -- Inno Setup으로 인스톨러 만들기

전문적인 설치 프로그램(setup.exe)을 만듭니다.

### 9.1 Inno Setup 설치

**1) 다운로드**

공식 사이트: https://jrsoftware.org/isinfo.php -> **Downloads** 클릭

**2) 설치**

다운로드한 `.exe` 실행 -> 기본값으로 Next -> Next -> Install

### 9.2 인스톨러 스크립트 작성

**1) Inno Setup Compiler 실행** (시작 메뉴에서 검색)

**2) Welcome 창에서 "Create a new empty script file" 선택 -> OK**

**3) 기존 내용을 전부 삭제하고 아래 스크립트를 붙여넣기**

```iss
; ===== WpfApp1 미니 계산기 인스톨러 =====

[Setup]
AppName=미니 계산기
AppVersion=1.0
AppPublisher=허영진
DefaultDirName={autopf}\MiniCalculator
DefaultGroupName=미니 계산기
OutputDir=C:\cs\WpfApp1\SetupOutput
OutputBaseFilename=MiniCalculator_Setup
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "korean"; MessagesFile: "compiler:Languages\Korean.isl"

[Tasks]
Name: "desktopicon"; Description: "바탕화면에 바로가기 만들기"; GroupDescription: "추가 아이콘:"

[Files]
Source: "C:\cs\WpfApp1\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\미니 계산기"; Filename: "{app}\WpfApp1.exe"
Name: "{autodesktop}\미니 계산기"; Filename: "{app}\WpfApp1.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\WpfApp1.exe"; Description: "미니 계산기 실행"; Flags: nowait postinstall skipifsilent
```

### 스크립트 섹션 설명

| 섹션 | 역할 |
|------|------|
| `[Setup]` | 앱 이름, 버전, 설치 경로, 출력 파일명 등 기본 설정 |
| `[Languages]` | 설치 마법사 언어 (Korean.isl = 한국어) |
| `[Tasks]` | 사용자 선택 옵션 (바탕화면 아이콘 생성 여부) |
| `[Files]` | 설치할 파일 목록 (publish 폴더 전체) |
| `[Icons]` | 시작 메뉴, 바탕화면 단축키 생성 |
| `[Run]` | 설치 완료 후 실행할 프로그램 |

### 주요 경로 매크로

| 매크로 | 실제 경로 예시 |
|--------|---------------|
| `{autopf}` | `C:\Program Files` |
| `{app}` | 설치 폴더 (`C:\Program Files\MiniCalculator`) |
| `{group}` | 시작 메뉴 폴더 |
| `{autodesktop}` | 바탕화면 |

### 9.3 컴파일

**4) Ctrl+F9** (또는 메뉴 Build -> Compile)

**5) 완료!**

`C:\cs\WpfApp1\SetupOutput\` 폴더에 **MiniCalculator_Setup.exe** 생성!

### 9.4 설치 테스트

생성된 `MiniCalculator_Setup.exe`를 실행하면:

1. 한국어 설치 마법사가 표시됩니다.
2. 설치 경로를 선택합니다.
3. "바탕화면에 바로가기 만들기" 옵션이 표시됩니다.
4. 설치 완료 후 "미니 계산기 실행" 체크박스가 표시됩니다.
5. 바탕화면 아이콘, 시작 메뉴가 자동 등록됩니다.
6. **제어판 -> 프로그램 추가/제거**에서 언인스톨 가능합니다.

---

## 10. 정리 및 과제

### 오늘 배운 것

| 번호 | 내용 |
|------|------|
| 1 | WPF 프로젝트 생성 및 구조 이해 |
| 2 | XAML로 UI 구성 (Grid, UniformGrid, Style) |
| 3 | 코드비하인드에서 이벤트 처리 |
| 4 | UI와 비즈니스 로직 분리 (CalcEngine) |
| 5 | `dotnet publish`로 배포용 빌드 |
| 6 | Inno Setup으로 전문적인 인스톨러 제작 |

### 전체 흐름 요약

```
코드 작성 -> 빌드(F5) -> 테스트 -> dotnet publish -> Inno Setup -> Setup.exe 배포!
```

### 과제 (선택)

1. **% 버튼 추가:** 현재 숫자를 100으로 나누는 기능
2. **키보드 입력 지원:** 키보드의 숫자/Enter 키로도 조작 가능하게
3. **계산 기록:** ListBox를 추가하여 이전 계산 기록을 표시

---

> **작성:** 허영진  
> **최종 수정:** 2026년 5월  
> **개발 환경:** Visual Studio 2022/2026, .NET 8 이상, Inno Setup 6
