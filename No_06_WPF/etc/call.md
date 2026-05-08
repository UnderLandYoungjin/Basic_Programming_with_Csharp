# WPF 실습 가이드 — 미니 계산기 만들기

<img width="301" height="489" alt="image" src="https://github.com/user-attachments/assets/e7636fa5-e8fc-4f4c-8514-2eb9346c43af" />

<img width="1446" height="763" alt="image" src="https://github.com/user-attachments/assets/37d8b170-70c5-4530-aae7-0ec3e7615da6" />
<img width="1428" height="751" alt="image" src="https://github.com/user-attachments/assets/bce57981-5d9d-432b-88b5-15f46b080018" />

> **과정명:** C# WPF 프로그래밍 입문  
> **실습 목표:** WPF 프로젝트 생성 → 파일 분할 설계 → 빌드 → 인스톨러 배포까지 전체 흐름 경험  
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
5. [코드 작성 — 파일별 역할과 전체 소스](#5-코드-작성--파일별-역할과-전체-소스)
   - 5.1 [App.xaml — 앱 진입점](#51-appxaml--앱-진입점)
   - 5.2 [App.xaml.cs — 앱 코드비하인드](#52-appxamlcs--앱-코드비하인드)
   - 5.3 [CalcEngine.cs — 계산 로직 (비즈니스 로직 분리)](#53-calcenginecs--계산-로직-비즈니스-로직-분리)
   - 5.4 [MainWindow.xaml — UI 화면 구성 (XAML)](#54-mainwindowxaml--ui-화면-구성-xaml)
   - 5.5 [MainWindow.xaml.cs — 이벤트 처리 (코드비하인드)](#55-mainwindowxamlcs--이벤트-처리-코드비하인드)
6. [XAML 핵심 개념 정리](#6-xaml-핵심-개념-정리)
7. [빌드 및 실행](#7-빌드-및-실행)
8. [배포 — 단일 EXE 만들기](#8-배포--단일-exe-만들기)
9. [배포 — Inno Setup으로 인스톨러 만들기](#9-배포--inno-setup으로-인스톨러-만들기)
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

**① Visual Studio 실행 → "새 프로젝트 만들기"**

**② 템플릿 검색창에 "WPF" 입력**

아래 목록에서 **"WPF 애플리케이션"** 을 선택합니다.

> ⚠️ 주의: "WPF 애플리케이션(.NET Framework)"이 아닌,  
> **".NET WPF 애플리케이션 만들기 프로젝트"** 를 선택하세요.

**③ 프로젝트 설정**

| 항목 | 입력값 |
|------|--------|
| 프로젝트 이름 | `WpfApp1` (또는 원하는 이름) |
| 위치 | `C:\cs\` (또는 원하는 경로) |
| 프레임워크 | .NET 8.0 이상 |

**④ "만들기" 클릭**

프로젝트가 생성되면 `MainWindow.xaml`이 자동으로 열립니다.

---

## 4. 프로젝트 구조 이해

Visual Studio 솔루션 탐색기에서 아래와 같은 구조를 확인할 수 있습니다.

```
WpfApp1/                     ← 솔루션 폴더
└── WpfApp1/                  ← 프로젝트 폴더
    ├── App.xaml              ← 앱 진입점 (Application 정의)
    ├── App.xaml.cs           ← 앱 코드비하인드
    ├── AssemblyInfo.cs       ← 어셈블리 정보 (자동 생성, 수정 불필요)
    ├── MainWindow.xaml       ← 메인 화면 UI (XAML로 구성)
    ├── MainWindow.xaml.cs    ← 메인 화면 이벤트 처리
    └── CalcEngine.cs         ← ⭐ 우리가 추가할 파일 (계산 로직)
```

### 파일별 역할 요약

| 파일 | 역할 | 비유 |
|------|------|------|
| `App.xaml` | 앱 전체 설정 (시작 창, 공용 스타일) | 건물의 설계도 |
| `MainWindow.xaml` | 화면 레이아웃 (버튼, 텍스트 배치) | 건물의 인테리어 |
| `MainWindow.xaml.cs` | 버튼 클릭 등 이벤트 처리 | 전기 배선 (동작 연결) |
| `CalcEngine.cs` | 순수 계산 로직 (UI 코드 없음) | 건물의 엔진룸 |

> 💡 **왜 파일을 분리하는가?**  
> UI 코드와 비즈니스 로직을 분리하면 유지보수가 쉬워집니다.  
> `CalcEngine.cs`는 WPF를 몰라도 이해할 수 있는 순수 C# 클래스입니다.  
> 이런 설계 방식이 나중에 **MVVM 패턴**의 기초가 됩니다.

---

## 5. 코드 작성 — 파일별 역할과 전체 소스

### 5.1 App.xaml — 앱 진입점

`App.xaml`은 WPF 앱의 시작점입니다. `StartupUri`로 처음 열릴 창을 지정합니다.

자동 생성된 `App.xaml`의 내용을 아래로 **전체 교체**합니다.

```xml
<!-- 
    Application: WPF 앱 전체를 나타내는 최상위 객체
    이 파일이 앱 시작 시 가장 먼저 읽힙니다.
-->
<Application x:Class="WpfApp1.App"
             
             <!-- xmlns: 기본 네임스페이스. 모든 WPF 컨트롤(Button, TextBlock 등)을 사용할 수 있게 해줌 -->
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             
             <!-- xmlns:x: XAML 자체 문법(x:Class, x:Name, x:Key 등)을 사용하기 위한 네임스페이스 -->
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             
             <!-- StartupUri: 앱 시작 시 처음 열릴 창을 지정 (여기서는 MainWindow.xaml) -->
             StartupUri="MainWindow.xaml">
    
    <!-- 
        Application.Resources: 앱 전체에서 공유하는 리소스(스타일, 색상, 템플릿 등)를 정의하는 영역
        지금은 비어 있지만, 나중에 모든 창에 공통으로 적용할 스타일을 여기에 둘 수 있음
    -->
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

### 5.2 App.xaml.cs — 앱 코드비하인드

보통 수정하지 않습니다. 앱 시작/종료 이벤트가 필요할 때 여기에 작성합니다.

```csharp
// System.Windows: WPF의 핵심 클래스(Window, Application 등)가 들어 있는 네임스페이스
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// App 클래스 — 앱 전체의 시작/종료/예외 처리를 담당
    /// partial 키워드: App.xaml과 App.xaml.cs가 컴파일 시 하나의 App 클래스로 합쳐짐
    /// Application 상속: WPF 앱의 기본 동작(메시지 루프, 리소스 관리 등)을 자동으로 제공받음
    /// </summary>
    public partial class App : Application
    {
        // 지금은 비어 있음
        // 필요 시 여기에 OnStartup(앱 시작), OnExit(앱 종료) 등을 오버라이드해서 작성
        // 예: 앱 시작 시 로그 파일 만들기, DB 연결 초기화 등
    }
}
```

> 💡 **partial class란?**  
> `App.xaml`(XAML)과 `App.xaml.cs`(C#)가 컴파일 시 하나의 `App` 클래스로 합쳐집니다.  
> WPF의 모든 `.xaml` + `.xaml.cs` 쌍이 이 구조입니다.

---

### 5.3 CalcEngine.cs — 계산 로직 (비즈니스 로직 분리)

**새 파일 추가 방법:**  
솔루션 탐색기 → WpfApp1 프로젝트 **우클릭** → **추가** → **클래스** → 이름: `CalcEngine.cs` → 추가

자동 생성된 내용을 **전부 삭제**하고 아래 코드를 붙여넣습니다.

```csharp
namespace WpfApp1
{
    /// <summary>
    /// 계산 엔진 클래스
    /// - UI(WPF) 코드를 전혀 사용하지 않는 순수 C# 클래스
    /// - 같은 로직을 WinForms, 콘솔, 웹앱에도 그대로 재사용 가능
    /// - 이렇게 UI와 로직을 분리하는 것이 MVVM 패턴의 첫걸음
    /// </summary>
    public class CalcEngine
    {
        // ============================================================
        // 상태 필드 (private: 클래스 내부에서만 사용)
        // 언더스코어(_)로 시작하는 것은 C# 관례 — "이건 private 필드입니다" 표시
        // ============================================================
        
        private double _firstNumber;      // 첫 번째 피연산자 (예: 5 + 3 에서 '5')
        private double _secondNumber;     // 두 번째 피연산자 (예: 5 + 3 에서 '3')
        private string _operation = "";   // 현재 선택된 연산자 (+, -, ×, ÷)
        private bool _isNewInput = true;  // 새 숫자 입력 시작 여부
                                          // true이면 다음 숫자 입력 시 디스플레이를 새로 시작
                                          // false이면 기존 숫자 뒤에 이어서 입력

        // ============================================================
        // 공개 속성 (Properties) — 외부에서 읽을 수 있는 값들
        // { get; private set; } : 외부에서는 읽기만 가능, 쓰기는 클래스 내부에서만
        // ============================================================
        
        public string DisplayText { get; private set; } = "0";  // 화면 큰 글씨로 표시될 값
        public string FormulaText { get; private set; } = "";   // 위쪽에 작게 표시되는 수식

        // 람다식 속성 (=>) : 한 줄짜리 읽기 전용 속성을 간결하게 표현
        // _isNewInput 필드 값을 그대로 외부에 공개
        public bool IsNewInput => _isNewInput;

        /// <summary>
        /// 숫자 또는 소수점 입력 처리 (0~9, .)
        /// </summary>
        /// <param name="digit">입력된 문자 ("0"~"9" 또는 ".")</param>
        public void InputDigit(string digit)
        {
            // _isNewInput이 true일 때 = 이제 막 새 숫자를 입력하려는 상태
            if (_isNewInput)
            {
                // 삼항 연산자: 조건이 true면 앞 값, false면 뒤 값
                // "."로 시작하면 "0."으로, 그 외엔 입력값 그대로 표시
                DisplayText = (digit == ".") ? "0." : digit;
                
                // 입력이 시작됐으니 더 이상 "새 입력 상태"가 아님
                _isNewInput = false;
            }
            else
            {
                // 소수점이 이미 있는데 또 "."를 누르면 무시 (3.14.5 같은 잘못된 값 방지)
                if (digit == "." && DisplayText.Contains('.'))
                    return;

                // 기존 텍스트 뒤에 입력된 숫자를 이어붙임 (예: "12" + "3" → "123")
                DisplayText += digit;
            }
        }

        /// <summary>
        /// 연산자 입력 처리 (+, -, ×, ÷)
        /// </summary>
        public void InputOperator(string op)
        {
            // 이미 숫자가 입력되어 있고, 이전 연산자도 있는 상태 → 연속 계산
            // 예: 5 + 3 + 를 누르면 5+3을 먼저 계산해서 8을 만들고, 다음 연산을 준비
            if (!_isNewInput && _operation != "")
            {
                _secondNumber = double.Parse(DisplayText);  // 현재 화면 숫자를 두 번째 피연산자로
                Calculate();                                 // 이전 연산 실행
                _firstNumber = double.Parse(DisplayText);    // 결과를 새 첫 번째 피연산자로
            }
            else
            {
                // 첫 연산자 입력이거나 새 입력 상태일 때 → 그냥 첫 번째 피연산자 저장
                _firstNumber = double.Parse(DisplayText);
            }

            _operation = op;                                  // 연산자 기억
            FormulaText = $"{_firstNumber} {_operation}";     // 위쪽 수식에 "5 +" 같은 형태로 표시
            _isNewInput = true;                               // 다음 숫자는 새 입력으로 처리
        }

        /// <summary>
        /// = 버튼 처리 — 현재까지의 연산 실행
        /// </summary>
        public void ExecuteEquals()
        {
            // 연산자가 없으면 계산할 게 없음 → 그대로 종료
            if (_operation == "") return;

            _secondNumber = double.Parse(DisplayText);                          // 두 번째 피연산자 확정
            FormulaText = $"{_firstNumber} {_operation} {_secondNumber} =";     // "5 + 3 =" 형태로 표시
            Calculate();                                                         // 실제 계산 수행
            _operation = "";                                                     // 연산자 초기화 (= 후엔 새 연산 시작)
            _isNewInput = true;                                                  // 새 입력 대기
        }

        /// <summary>
        /// C 버튼 — 모든 상태를 초기 상태로
        /// </summary>
        public void Clear()
        {
            _firstNumber = 0;
            _secondNumber = 0;
            _operation = "";
            DisplayText = "0";
            FormulaText = "";
            _isNewInput = true;
        }

        /// <summary>
        /// ± 버튼 — 현재 화면 숫자의 부호를 반전
        /// </summary>
        public void ToggleSign()
        {
            // 0은 부호를 바꿀 필요가 없으니 그대로 종료
            if (DisplayText == "0") return;
            
            double value = double.Parse(DisplayText);   // 문자열 → 숫자
            value = -value;                              // 부호 반전 (5 → -5, -3 → 3)
            DisplayText = value.ToString();              // 다시 문자열로 변환해서 표시
        }

        /// <summary>
        /// ⌫ 버튼 — 마지막 한 글자 삭제
        /// </summary>
        public void Backspace()
        {
            // 새 입력 상태(아직 아무것도 안 친 상태)면 무시
            if (_isNewInput) return;

            // 두 글자 이상이면 마지막 한 글자만 잘라냄
            if (DisplayText.Length > 1)
            {
                // [..^1] : C# 8.0의 범위 연산자
                // ^1 = "끝에서 1번째" 의미 → 마지막 글자 직전까지 가져옴
                // 예: "123"[..^1] → "12"
                DisplayText = DisplayText[..^1];
            }
            else
            {
                // 한 글자만 남았다면 0으로 리셋
                DisplayText = "0";
                _isNewInput = true;
            }
        }

        // ============================================================
        // 내부 계산 로직 (private: 외부에서 호출 불가, 내부에서만 사용)
        // ============================================================
        
        private void Calculate()
        {
            // switch 식 (C# 8.0+) — switch문보다 간결한 표현
            // _operation의 값에 따라 다른 계산 결과를 반환
            double result = _operation switch
            {
                "+" => _firstNumber + _secondNumber,
                "-" => _firstNumber - _secondNumber,
                "×" => _firstNumber * _secondNumber,
                
                // 0으로 나누기 방지: 분모가 0이면 NaN(Not a Number) 반환
                "÷" => _secondNumber != 0
                        ? _firstNumber / _secondNumber
                        : double.NaN,
                
                // _ 는 "그 외 모든 경우"를 의미 (default와 같음)
                _ => _secondNumber
            };

            // 결과가 NaN이거나 무한대면 "오류" 표시
            // 예: 0으로 나누기, 너무 큰 수의 곱셈 등
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                DisplayText = "오류";
                _isNewInput = true;
                return;
            }

            // "G10" 형식: 유효숫자 10자리까지 표시
            // 예: 0.1 + 0.2 = 0.3 (소수 부동소수점 오차 방지)
            DisplayText = result.ToString("G10");
        }
    }
}
```

### CalcEngine.cs 핵심 포인트

| 문법 | 설명 | 예시 |
|------|------|------|
| `{ get; private set; }` | 외부에서 읽기만 가능, 쓰기는 클래스 내부만 | `DisplayText` |
| `=>` (람다식 속성) | 간결한 읽기 전용 속성 | `IsNewInput => _isNewInput` |
| `switch` 식 | C# 8.0+ 패턴 매칭 | `_operation switch { "+" => ... }` |
| `[..^1]` | 범위 연산자 (마지막 한 글자 제외) | `"123"[..^1]` → `"12"` |
| `$"..."` | 문자열 보간 | `$"{_firstNumber} {_operation}"` |

---

### 5.4 MainWindow.xaml — UI 화면 구성 (XAML)

자동 생성된 `MainWindow.xaml`을 **전체 교체**합니다.

```xml
<!-- 
    Window: WPF의 최상위 창 컨테이너
    이 안에 Grid, Button, TextBlock 같은 컨트롤들을 배치
-->
<Window x:Class="WpfApp1.MainWindow"
        
        <!-- 기본 네임스페이스: WPF 컨트롤들 사용 -->
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        
        <!-- x: 네임스페이스: x:Class, x:Name 같은 XAML 문법 사용 -->
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        
        Title="미니 계산기"                          <!-- 창 제목바에 표시될 텍스트 -->
        Height="500" Width="320"                     <!-- 창 초기 크기 (픽셀) -->
        MinHeight="400" MinWidth="280"               <!-- 창 최소 크기 (이보다 작게 못 줄임) -->
        WindowStartupLocation="CenterScreen"         <!-- 창이 화면 중앙에 뜨도록 -->
        Background="#FF1E1E1E">                      <!-- 창 배경색 (#FFRRGGBB: 알파+RGB) -->

    <!-- ==========================================
         Window.Resources: 이 창 안에서만 쓰는 스타일/리소스
         x:Key로 이름을 붙여두면 컨트롤에서 StaticResource로 호출 가능
         (CSS의 클래스와 비슷한 개념)
         ========================================== -->
    <Window.Resources>
        
        <!-- 숫자 버튼용 스타일 (어두운 회색) -->
        <Style x:Key="NumBtn" TargetType="Button">
            <!-- Setter: Property=속성명, Value=값 -->
            <Setter Property="FontSize" Value="20"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="Margin" Value="3"/>             <!-- 버튼 외부 여백 -->
            <Setter Property="Background" Value="#FF3C3C3C"/> <!-- 배경색 -->
            <Setter Property="Foreground" Value="White"/>      <!-- 글자색 -->
            <Setter Property="BorderThickness" Value="0"/>     <!-- 테두리 두께 (0=없음) -->
            <Setter Property="Cursor" Value="Hand"/>           <!-- 마우스 올리면 손모양 커서 -->
        </Style>

        <!-- 연산자 버튼용 스타일 (파란색) -->
        <Style x:Key="OpBtn" TargetType="Button">
            <Setter Property="FontSize" Value="20"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="Margin" Value="3"/>
            <Setter Property="Background" Value="#FF0078D4"/>  <!-- Microsoft 블루 -->
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Cursor" Value="Hand"/>
        </Style>

        <!-- 기능 버튼용 스타일 (C, ±, ⌫) — 중간 회색 -->
        <Style x:Key="FuncBtn" TargetType="Button">
            <Setter Property="FontSize" Value="20"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="Margin" Value="3"/>
            <Setter Property="Background" Value="#FF505050"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Cursor" Value="Hand"/>
        </Style>

        <!-- = 버튼용 스타일 (초록색) — 가장 중요한 버튼이므로 강조 -->
        <Style x:Key="EqBtn" TargetType="Button">
            <Setter Property="FontSize" Value="20"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="Margin" Value="3"/>
            <Setter Property="Background" Value="#FF107C10"/>  <!-- 초록 -->
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Cursor" Value="Hand"/>
        </Style>
    </Window.Resources>

    <!-- ==========================================
         Grid: WPF의 가장 기본적인 레이아웃 패널
         - 행(Row)과 열(Column)로 영역을 나눔
         - 표(table)와 비슷한 개념
         Margin="8" : Grid 바깥쪽 8픽셀 여백
         ========================================== -->
    <Grid Margin="8">
        
        <!-- 행 정의: 3개의 행 만들기 -->
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>   <!-- Row 0: 내용 크기에 맞춤 (수식 텍스트) -->
            <RowDefinition Height="Auto"/>   <!-- Row 1: 내용 크기에 맞춤 (메인 디스플레이) -->
            <RowDefinition Height="*"/>      <!-- Row 2: 남은 공간 전부 (버튼 영역) -->
        </Grid.RowDefinitions>

        <!-- ===== 수식 표시 영역 (Row 0) ===== -->
        <!-- 
            x:Name : C# 코드에서 이 컨트롤을 참조할 때 쓰는 이름
            Grid.Row : 이 컨트롤이 들어갈 행 번호 (0부터 시작)
        -->
        <TextBlock x:Name="txtFormula"
                   Grid.Row="0"
                   Text=""                              
                   FontSize="14"
                   Foreground="Gray"                    
                   HorizontalAlignment="Right"          <!-- 오른쪽 정렬 -->
                   Margin="5,5,10,0"/>                  <!-- 좌,상,우,하 여백 -->

        <!-- ===== 메인 디스플레이 (Row 1) ===== -->
        <TextBlock x:Name="txtDisplay"
                   Grid.Row="1"
                   Text="0"                             <!-- 초기값 -->
                   FontSize="42"                        <!-- 크게 -->
                   FontWeight="Light"                   <!-- 가벼운 두께 (계산기 느낌) -->
                   Foreground="White"
                   HorizontalAlignment="Right"
                   Margin="5,5,10,10"
                   TextTrimming="CharacterEllipsis"/>   <!-- 텍스트가 넘치면 ...으로 표시 -->

        <!-- ===== 버튼 영역 (Row 2) =====
             UniformGrid: 모든 셀이 동일한 크기로 배치되는 패널
             - Rows="5" Columns="4" : 5행 4열 = 총 20칸
             - 자식 컨트롤이 자동으로 왼쪽→오른쪽, 위→아래 순서로 채워짐 -->
        <UniformGrid Grid.Row="2" Rows="5" Columns="4">

            <!-- 1행: C, ±, ⌫, ÷ -->
            <!-- 
                Click="..." : 버튼 클릭 시 실행할 메서드 이름
                Style="{StaticResource ...}" : 위 Window.Resources에 정의한 스타일 적용
            -->
            <Button Content="C"  Click="BtnClear_Click"      Style="{StaticResource FuncBtn}"/>
            <Button Content="±"  Click="BtnToggleSign_Click" Style="{StaticResource FuncBtn}"/>
            <Button Content="⌫" Click="BtnBackspace_Click"  Style="{StaticResource FuncBtn}"/>
            <Button Content="÷"  Click="BtnOperator_Click"   Style="{StaticResource OpBtn}"/>

            <!-- 2행: 7, 8, 9, × -->
            <!-- 숫자 버튼들은 모두 BtnDigit_Click 하나의 핸들러를 공유 -->
            <Button Content="7"  Click="BtnDigit_Click"      Style="{StaticResource NumBtn}"/>
            <Button Content="8"  Click="BtnDigit_Click"      Style="{StaticResource NumBtn}"/>
            <Button Content="9"  Click="BtnDigit_Click"      Style="{StaticResource NumBtn}"/>
            <Button Content="×"  Click="BtnOperator_Click"   Style="{StaticResource OpBtn}"/>

            <!-- 3행: 4, 5, 6, - -->
            <Button Content="4"  Click="BtnDigit_Click"      Style="{StaticResource NumBtn}"/>
            <Button Content="5"  Click="BtnDigit_Click"      Style="{StaticResource NumBtn}"/>
            <Button Content="6"  Click="BtnDigit_Click"      Style="{StaticResource NumBtn}"/>
            <Button Content="-"  Click="BtnOperator_Click"   Style="{StaticResource OpBtn}"/>

            <!-- 4행: 1, 2, 3, + -->
            <Button Content="1"  Click="BtnDigit_Click"      Style="{StaticResource NumBtn}"/>
            <Button Content="2"  Click="BtnDigit_Click"      Style="{StaticResource NumBtn}"/>
            <Button Content="3"  Click="BtnDigit_Click"      Style="{StaticResource NumBtn}"/>
            <Button Content="+"  Click="BtnOperator_Click"   Style="{StaticResource OpBtn}"/>

            <!-- 5행: 00, 0, ., = -->
            <Button Content="00" Click="BtnDigit_Click"      Style="{StaticResource NumBtn}"/>
            <Button Content="0"  Click="BtnDigit_Click"      Style="{StaticResource NumBtn}"/>
            <Button Content="."  Click="BtnDigit_Click"      Style="{StaticResource NumBtn}"/>
            <Button Content="="  Click="BtnEquals_Click"     Style="{StaticResource EqBtn}"/>

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
| `Click="BtnDigit_Click"` | 클릭 시 호출할 이벤트 핸들러 메서드 이름 |
| `UniformGrid` | 모든 칸이 동일 크기인 격자 레이아웃 |

---

### 5.5 MainWindow.xaml.cs — 이벤트 처리 (코드비하인드)

자동 생성된 `MainWindow.xaml.cs`를 **전체 교체**합니다.

```csharp
using System.Windows;            // Window, RoutedEventArgs 등 WPF 핵심 클래스
using System.Windows.Controls;   // Button, TextBlock 등 UI 컨트롤 클래스

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml의 코드비하인드 (Code-behind)
    /// - XAML에서 정의한 UI의 동작(이벤트)을 처리
    /// - 계산 로직 자체는 직접 처리하지 않고 CalcEngine에 위임 → 관심사 분리
    /// - partial: MainWindow.xaml과 합쳐져서 하나의 클래스로 컴파일됨
    /// - Window 상속: 창의 기본 동작(닫기, 최소화, 이동 등)을 자동으로 가짐
    /// </summary>
    public partial class MainWindow : Window
    {
        // ============================================================
        // 계산 엔진 인스턴스
        // - readonly: 한 번 할당되면 다시 바꿀 수 없음 (안정성 ↑)
        // - new() : C# 9.0+ 타겟 타입 새 식 (타입을 우변에서 생략 가능)
        //   기존 표기: new CalcEngine()
        // ============================================================
        private readonly CalcEngine _engine = new();

        /// <summary>
        /// 생성자 — 창이 만들어질 때 가장 먼저 호출됨
        /// </summary>
        public MainWindow()
        {
            // InitializeComponent() : XAML에 정의된 모든 컨트롤(Button, TextBlock 등)을
            // 메모리에 만들고, x:Name으로 지정한 컨트롤을 C# 변수로 연결해 줌
            // 이 호출 이후부터 txtDisplay, txtFormula 같은 이름을 사용할 수 있음
            // (반드시 호출해야 함! 빠뜨리면 모든 컨트롤이 null이 되어 오류)
            InitializeComponent();
        }

        // ============================================================
        // 이벤트 핸들러들
        // - 모든 이벤트 핸들러의 시그니처: (object sender, RoutedEventArgs e)
        //   sender: 이벤트를 발생시킨 컨트롤(누가 클릭됐는지)
        //   e     : 이벤트의 추가 정보
        // ============================================================

        /// <summary>
        /// 숫자 버튼 클릭 (0~9, 00, .) — XAML에서 14개 버튼이 이 메서드 하나를 공유
        /// </summary>
        private void BtnDigit_Click(object sender, RoutedEventArgs e)
        {
            // sender(클릭된 컨트롤)를 Button 타입으로 캐스팅
            // → btn.Content (버튼에 표시된 텍스트)에 접근하기 위해
            Button btn = (Button)sender;
            
            // Content는 object 타입이므로 ToString()으로 문자열 변환
            // ! (null 허용 연산자) : "이 값은 절대 null이 아니다"를 컴파일러에 알림
            string digit = btn.Content.ToString()!;

            // "00" 버튼: 0을 두 번 입력하는 효과
            if (digit == "00")
            {
                // 새 입력 상태에서 "00"을 누르면 그냥 0이 되므로 무시
                if (_engine.IsNewInput) return;
                _engine.InputDigit("0");
                _engine.InputDigit("0");
            }
            else
            {
                // 일반 숫자나 소수점은 그대로 전달
                _engine.InputDigit(digit);
            }

            // 엔진 상태가 바뀌었으니 화면을 다시 그림
            UpdateDisplay();
        }

        /// <summary>
        /// 연산자 버튼 클릭 (+, -, ×, ÷)
        /// </summary>
        private void BtnOperator_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string op = btn.Content.ToString()!;
            _engine.InputOperator(op);   // 엔진에 연산자 전달
            UpdateDisplay();
        }

        /// <summary>
        /// = 버튼 클릭
        /// </summary>
        private void BtnEquals_Click(object sender, RoutedEventArgs e)
        {
            _engine.ExecuteEquals();
            UpdateDisplay();
        }

        /// <summary>
        /// C 버튼 클릭 — 전체 초기화
        /// </summary>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            _engine.Clear();
            UpdateDisplay();
        }

        /// <summary>
        /// ± 버튼 클릭 — 부호 반전
        /// </summary>
        private void BtnToggleSign_Click(object sender, RoutedEventArgs e)
        {
            _engine.ToggleSign();
            UpdateDisplay();
        }

        /// <summary>
        /// ⌫ 버튼 클릭 — 마지막 한 자리 삭제
        /// </summary>
        private void BtnBackspace_Click(object sender, RoutedEventArgs e)
        {
            _engine.Backspace();
            UpdateDisplay();
        }

        // ============================================================
        // 화면 갱신 메서드 (private — 클래스 내부에서만 사용)
        // CalcEngine이 들고 있는 텍스트를 실제 UI에 반영
        // x:Name으로 지정한 컨트롤(txtDisplay, txtFormula)에 직접 접근 가능
        // ============================================================
        private void UpdateDisplay()
        {
            txtDisplay.Text = _engine.DisplayText;   // 메인 큰 글씨
            txtFormula.Text = _engine.FormulaText;   // 위쪽 작은 수식
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
  ├── 종속성
  ├── App.xaml
  │    └── App.xaml.cs
  ├── AssemblyInfo.cs
  ├── CalcEngine.cs          ← 이 파일이 있어야 함!
  └── MainWindow.xaml
       └── MainWindow.xaml.cs
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
| `'CalcEngine' 형식을 찾을 수 없습니다` | CalcEngine.cs가 프로젝트에 없음 | 프로젝트 우클릭 → 추가 → 클래스 |
| `XDG0000` (XAML 파싱 오류) | XAML 문법 오류 | XAML 코드를 전체 교체 |
| `x:Class 불일치` | 네임스페이스가 다름 | `x:Class="WpfApp1.MainWindow"` 확인 |

---

## 8. 배포 — 단일 EXE 만들기

다른 컴퓨터에서 실행하려면 **publish**가 필요합니다.

### 방법: 명령줄에서 publish

**① 터미널 열기**

Visual Studio에서 프로젝트 **우클릭** → **터미널에서 열기**  
또는 CMD에서 프로젝트 폴더로 이동합니다.

```
cd C:\cs\WpfApp1
```

**② publish 명령 실행**

```
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish
```

| 옵션 | 의미 |
|------|------|
| `-c Release` | Release 모드 (최적화된 빌드) |
| `-r win-x64` | Windows 64비트 대상 |
| `--self-contained true` | .NET 런타임 포함 (상대방 PC에 .NET 없어도 실행 가능) |
| `-o ./publish` | 출력 폴더 지정 |

**③ 결과 확인**

`publish` 폴더에 `WpfApp1.exe`를 포함한 파일들이 생성됩니다.

### 배포 방식 비교

| 방식 | 명령어 | 파일 크기 | .NET 설치 필요 |
|------|--------|----------|---------------|
| 자체 포함 | `--self-contained true` | ~150MB | 불필요 |
| 프레임워크 종속 | `--self-contained false` | ~1MB | 필요 |

---

## 9. 배포 — Inno Setup으로 인스톨러 만들기

전문적인 설치 프로그램(setup.exe)을 만듭니다.

### 9.1 Inno Setup 설치

**① 다운로드**

공식 사이트: https://jrsoftware.org/isinfo.php → **Downloads** 클릭

**② 설치**

다운로드한 `.exe` 실행 → 기본값으로 Next → Next → Install

### 9.2 인스톨러 스크립트 작성

**① Inno Setup Compiler 실행** (시작 메뉴에서 검색)

**② Welcome 창에서 "Create a new empty script file" 선택 → OK**

**③ 기존 내용을 전부 삭제하고 아래 스크립트를 붙여넣기**

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

**④ Ctrl+F9** (또는 메뉴 Build → Compile)

**⑤ 완료!**

`C:\cs\WpfApp1\SetupOutput\` 폴더에 **MiniCalculator_Setup.exe** 생성!

### 9.4 설치 테스트

생성된 `MiniCalculator_Setup.exe`를 실행하면:

1. 한국어 설치 마법사가 표시됩니다.
2. 설치 경로를 선택합니다.
3. "바탕화면에 바로가기 만들기" 옵션이 표시됩니다.
4. 설치 완료 후 "미니 계산기 실행" 체크박스가 표시됩니다.
5. 바탕화면 아이콘, 시작 메뉴가 자동 등록됩니다.
6. **제어판 → 프로그램 추가/제거**에서 언인스톨 가능합니다.

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
코드 작성 → 빌드(F5) → 테스트 → dotnet publish → Inno Setup → Setup.exe 배포!
```

### 과제 (선택)

1. **% 버튼 추가:** 현재 숫자를 100으로 나누는 기능
2. **키보드 입력 지원:** 키보드의 숫자/Enter 키로도 조작 가능하게
3. **계산 기록:** ListBox를 추가하여 이전 계산 기록을 표시

---

> **작성:** 허영진  
> **최종 수정:** 2026년 3월  
> **개발 환경:** Visual Studio 2026, .NET 10, Inno Setup 6.7.1
