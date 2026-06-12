# C# OpenCV 제품 경계 검출 튜토리얼

## 사진 1장을 불러와서 왼쪽은 원본, 오른쪽은 경계 검출 결과로 보기

이 문서는 C# 초급자가 Visual Studio에서 직접 따라 하며, 사진 1장을 불러와 제품의 경계를 찾는 Vision AI 기초 실습을 완성하도록 만든 튜토리얼입니다.

핵심 목표는 다음과 같습니다.

1. OpenCV가 무엇을 하는 라이브러리인지 이해한다.
2. 사진 1장을 프로그램에서 불러온다.
3. 왼쪽 PictureBox에는 원본 이미지를 표시한다.
4. 오른쪽 PictureBox에는 제품 경계가 선으로 표시된 이미지를 표시한다.
5. Canny Edge Detection의 이론적 배경과 처리 순서를 이해한다.
6. 경계선만 찾는 것과 제품 외곽 윤곽선까지 찾는 것의 차이를 이해한다.

---

## 1. 이번 실습에서 만드는 프로그램

프로그램 이름 예시:

```text
ProductEdgeViewer
```

화면 구성:

```text
+----------------------------------------------------------+
| [사진 열기]  상태: 경계 검출 완료                         |
+----------------------------+-----------------------------+
|                            |                             |
|       원본 사진             |      경계 검출 결과           |
|                            |                             |
+----------------------------+-----------------------------+
```

동작 방식:

1. 사용자가 `사진 열기` 버튼을 누른다.
2. 제품 사진을 선택한다.
3. OpenCV가 이미지를 읽는다.
4. 이미지를 흑백으로 바꾼다.
5. 노이즈를 줄이기 위해 Gaussian Blur를 적용한다.
6. Canny Edge Detection으로 경계선을 찾는다.
7. 찾은 경계선을 바탕으로 윤곽선 Contour를 찾는다.
8. 원본 이미지 위에 윤곽선을 초록색 선으로 그린다.
9. 왼쪽에는 원본, 오른쪽에는 결과 이미지를 표시한다.

---

## 2. 왜 이 실습이 Vision AI 입문에 좋은가?

Vision AI를 바로 YOLO 같은 객체 탐지 모델로 시작하면 학생들이 내부 원리를 이해하기 어렵습니다.

반면 Edge Detection은 다음 질문에 직접 답할 수 있습니다.

```text
컴퓨터는 사진에서 물체의 경계를 어떻게 알아볼까?
```

사람은 제품의 모서리나 외곽을 자연스럽게 보지만, 컴퓨터는 이미지를 숫자 배열로 봅니다.

예를 들어 흑백 이미지는 다음과 같은 밝기 숫자들의 표입니다.

```text
0   = 검정
255 = 흰색
중간 값 = 회색
```

컴퓨터는 옆 픽셀과 밝기 차이가 큰 위치를 찾습니다.

```text
밝기 변화가 작다  → 같은 면일 가능성이 높다.
밝기 변화가 크다  → 경계, 모서리, 선일 가능성이 높다.
```

이것이 Edge Detection의 출발점입니다.

---

## 3. OpenCV가 하는 일

OpenCV는 Open Source Computer Vision Library의 약자입니다.

C# 자체는 이미지 처리 알고리즘을 대규모로 제공하지 않습니다. 그래서 C#에서 사진을 읽고, 흑백 변환하고, 노이즈 제거하고, 경계선을 찾는 일을 직접 구현하려면 수학과 행렬 연산 코드를 많이 작성해야 합니다.

OpenCV는 이런 기능을 이미 구현해 둔 컴퓨터 비전 라이브러리입니다.

이번 실습에서 OpenCV가 하는 일은 다음과 같습니다.

| 단계 | OpenCV 함수 | 역할 |
|---|---|---|
| 이미지 읽기 | `Cv2.ImRead()` | 파일에서 사진을 읽어 Mat 객체로 변환 |
| 색상 변환 | `Cv2.CvtColor()` | 컬러 이미지를 흑백 이미지로 변환 |
| 노이즈 제거 | `Cv2.GaussianBlur()` | 작은 점, 먼지, 압축 노이즈를 부드럽게 처리 |
| 경계 검출 | `Cv2.Canny()` | 밝기 변화가 큰 부분을 경계선으로 검출 |
| 윤곽선 찾기 | `Cv2.FindContours()` | 연결된 경계선을 하나의 외곽선 후보로 묶음 |
| 윤곽선 그리기 | `Cv2.DrawContours()` | 찾은 외곽선을 이미지 위에 시각화 |
| 이미지 인코딩 | `Cv2.ImEncode()` | OpenCV Mat 이미지를 WinForms 표시용 Bitmap으로 변환 |

---

## 4. Edge Detection의 이론적 배경

### 4.1 이미지란 무엇인가?

컴퓨터 입장에서 이미지는 픽셀의 집합입니다.

컬러 이미지는 일반적으로 BGR 또는 RGB 3개 채널을 가집니다.

```text
B = Blue, 파란색 성분
G = Green, 초록색 성분
R = Red, 빨간색 성분
```

OpenCV는 기본적으로 컬러 이미지를 BGR 순서로 다룹니다.

C# WinForms의 Bitmap이나 일반적인 웹 이미지 설명에서는 RGB라는 말을 많이 쓰지만, OpenCV 내부에서는 BGR을 기본으로 쓰는 경우가 많습니다.

---

### 4.2 왜 흑백으로 바꾸는가?

경계 검출에서 중요한 것은 색상 자체가 아니라 밝기의 변화입니다.

예를 들어 제품과 배경 사이에 밝기 차이가 크면 경계가 잘 보입니다.

컬러 이미지를 그대로 처리하면 B, G, R 세 채널을 모두 고려해야 해서 설명이 복잡해집니다.

그래서 초급 실습에서는 먼저 흑백 이미지로 변환합니다.

```text
컬러 이미지 → 흑백 이미지 → 밝기 변화 분석
```

OpenCV 함수:

```csharp
Cv2.CvtColor(original, gray, ColorConversionCodes.BGR2GRAY);
```

의미:

```text
BGR 컬러 이미지를 밝기 정보만 가진 흑백 이미지로 바꾼다.
```

---

### 4.3 노이즈 제거가 필요한 이유

사진에는 작은 점, 먼지, 그림자, 압축 흔적이 포함될 수 있습니다.

이런 작은 변화도 컴퓨터는 경계로 오해할 수 있습니다.

그래서 경계 검출 전에 이미지를 살짝 부드럽게 만듭니다.

이때 사용하는 대표적인 방법이 Gaussian Blur입니다.

OpenCV 함수:

```csharp
Cv2.GaussianBlur(gray, blurred, new OpenCvSharp.Size(5, 5), 1.5);
```

의미:

```text
주변 픽셀을 참고하여 이미지를 부드럽게 만든다.
작은 잡음은 줄이고 큰 경계는 남긴다.
```

주의:

```text
Blur가 너무 약하면 노이즈가 많이 남는다.
Blur가 너무 강하면 실제 제품 경계도 흐려질 수 있다.
```

---

### 4.4 밝기 변화와 Gradient

Edge Detection의 핵심은 Gradient입니다.

Gradient는 쉽게 말해 밝기가 얼마나 급격하게 변하는지를 나타냅니다.

예를 들어 왼쪽은 어둡고 오른쪽은 밝다면 그 사이에는 큰 변화가 있습니다.

```text
어두움  어두움  어두움  밝음  밝음  밝음
  20      22      24     180   182   185
                 ↑
              큰 변화
```

이 큰 변화가 있는 위치가 경계 후보입니다.

제품 외곽, 모서리, 구멍, 홈, 라벨 경계 등이 여기에 해당할 수 있습니다.

---

## 5. Canny Edge Detection 원리

Canny Edge Detection은 1986년 John F. Canny가 제안한 대표적인 경계 검출 알고리즘입니다.

Canny는 단순히 밝기 차이가 큰 부분을 전부 선으로 표시하지 않습니다. 여러 단계를 거쳐 의미 있는 경계만 남기려고 합니다.

### 5.1 Canny의 처리 순서

```text
1단계: 흑백 변환
2단계: Gaussian Blur로 노이즈 제거
3단계: Gradient 계산
4단계: 얇은 선만 남기기
5단계: 이중 임계값 적용
6단계: 강한 경계와 연결된 약한 경계만 유지
```

---

### 5.2 1단계: 흑백 변환

색상 정보보다 밝기 변화가 중요하므로 컬러 이미지를 흑백으로 바꿉니다.

---

### 5.3 2단계: Gaussian Blur

노이즈를 줄입니다.

노이즈가 많으면 실제 제품 경계가 아닌 작은 점까지 경계로 검출됩니다.

---

### 5.4 3단계: Gradient 계산

각 픽셀 주변에서 밝기가 어느 방향으로, 얼마나 강하게 변하는지 계산합니다.

밝기 변화가 큰 곳은 경계 후보가 됩니다.

---

### 5.5 4단계: Non-Maximum Suppression

경계 후보가 두껍게 나오면 실제 경계 위치를 보기 어렵습니다.

Canny는 경계 방향을 기준으로 가장 강한 위치만 남겨 선을 얇게 만듭니다.

결과적으로 1픽셀 정도의 얇은 경계선에 가까운 결과를 얻습니다.

---

### 5.6 5단계: 이중 임계값

Canny에는 보통 두 개의 값이 들어갑니다.

```text
낮은 임계값 = low threshold
높은 임계값 = high threshold
```

이번 코드에서는 다음 값을 사용합니다.

```text
low threshold  = 50
high threshold = 150
```

의미:

```text
150보다 강한 경계 → 확실한 경계
50보다 약한 경계  → 버림
50~150 사이 경계 → 애매한 경계
```

---

### 5.7 6단계: Hysteresis

애매한 경계는 무조건 버리지 않습니다.

강한 경계와 연결되어 있으면 실제 경계의 일부일 가능성이 높으므로 살립니다.

강한 경계와 연결되지 않은 약한 경계는 노이즈일 가능성이 높으므로 버립니다.

---

## 6. Edge와 Contour의 차이

Edge와 Contour는 비슷해 보이지만 다릅니다.

| 구분 | 의미 | 예시 |
|---|---|---|
| Edge | 밝기 변화가 큰 픽셀들의 선 | 제품 모서리, 글자 경계, 그림자 경계 |
| Contour | 연결된 Edge들을 하나의 외곽선 묶음으로 해석한 것 | 제품 외곽선, 구멍 외곽선, 부품 테두리 |

이번 실습에서는 다음 흐름을 사용합니다.

```text
Canny로 Edge 검출 → FindContours로 연결된 외곽선 추출 → DrawContours로 원본 위에 표시
```

이렇게 하면 단순히 흰색 선만 보이는 화면보다, 원본 위에 제품 경계가 시각화되어 교육용으로 더 이해하기 쉽습니다.

---

## 7. 실습 환경

### 7.1 권장 환경

| 항목 | 권장 값 |
|---|---|
| OS | Windows 10 또는 Windows 11 |
| IDE | Visual Studio 2022 |
| 프로젝트 | Windows Forms App |
| 언어 | C# |
| 대상 프레임워크 | .NET 8 또는 .NET 9 |
| 라이브러리 | OpenCvSharp4.Windows |

---

## 8. 프로젝트 생성

Visual Studio에서 실행합니다.

```text
Visual Studio 실행 → 새 프로젝트 만들기 → Windows Forms App 선택 → 다음
```

설정값:

```text
프로젝트 이름: ProductEdgeViewer
위치: 원하는 폴더
프레임워크: .NET 8 또는 .NET 9
```

프로젝트가 생성되면 기본 `Form1.cs`가 만들어집니다.

이번 튜토리얼에서는 직접 코드를 작성하기 위해 파일명을 다음처럼 바꾸겠습니다.

```text
Form1.cs → MainForm.cs
```

또는 새 파일을 만들어도 됩니다.

---

## 9. NuGet 패키지 설치

Visual Studio에서 실행합니다.

```text
프로젝트 우클릭 → NuGet 패키지 관리 → 찾아보기
```

검색 후 설치:

```text
OpenCvSharp4.Windows
```

또는 Visual Studio의 패키지 관리자 콘솔에서 실행합니다.

```powershell
Install-Package OpenCvSharp4.Windows
```

### 왜 OpenCvSharp4.Windows를 설치하는가?

OpenCV는 원래 C++ 중심의 라이브러리입니다.

C#에서 OpenCV를 쓰려면 C# 코드와 OpenCV 네이티브 DLL 사이를 연결해 주는 래퍼가 필요합니다.

`OpenCvSharp`은 C#에서 OpenCV 기능을 사용할 수 있게 해주는 .NET 래퍼입니다.

`OpenCvSharp4.Windows`는 Windows 환경에서 필요한 OpenCvSharp 본체와 네이티브 실행 파일을 함께 제공하는 패키지입니다.

---

## 10. 프로젝트 파일 확인

Visual Studio의 솔루션 탐색기에서 `.csproj` 파일을 열어 다음과 비슷한지 확인합니다.

### 파일 목적

이 파일은 C# 프로젝트 설정 파일입니다. WinForms 사용 여부, .NET 버전, NuGet 패키지 참조를 정의합니다.

```xml
<!-- ProductEdgeViewer/ProductEdgeViewer.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWindowsForms>true</UseWindowsForms>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="OpenCvSharp4.Windows" Version="4.13.0.20260531" />
  </ItemGroup>
</Project>
```

주의:

```text
패키지 버전은 설치 시점에 따라 달라질 수 있습니다.
NuGet에서 설치한 버전과 자동으로 맞춰지는 것이 정상입니다.
```

---

## 11. Program.cs 작성

### 파일 목적

이 파일은 프로그램의 시작점입니다. WinForms 애플리케이션을 실행하고 MainForm 화면을 엽니다.

```csharp
// ProductEdgeViewer/Program.cs
using System; // 프로그램 실행에 필요한 기본 기능을 사용합니다.
using System.Windows.Forms; // WinForms 화면 실행 기능을 사용합니다.

namespace ProductEdgeViewer; // 이 프로젝트의 코드 묶음 이름을 지정합니다.

internal static class Program // 프로그램 시작 클래스를 정의합니다.
{
    [STAThread] // Windows UI 프로그램에 필요한 단일 스레드 아파트먼트 설정입니다.
    private static void Main() // 프로그램이 처음 시작되는 함수입니다.
    {
        ApplicationConfiguration.Initialize(); // WinForms 기본 설정을 초기화합니다.
        Application.Run(new MainForm()); // MainForm 화면을 실행합니다.
    }
}
```

---

## 12. MainForm.cs 작성

### 파일 목적

이 파일은 실제 화면을 만들고, 사진을 불러오고, OpenCV로 제품 경계를 검출한 뒤 좌우에 이미지를 표시하는 핵심 코드입니다.

```csharp
// ProductEdgeViewer/MainForm.cs

using OpenCvSharp; // OpenCV의 이미지 처리 기능을 C#에서 사용합니다.
using System; // 기본 자료형과 이벤트 기능을 사용합니다.
using System.Collections.Generic; // List 같은 컬렉션 기능을 사용합니다.
using System.Drawing; // Bitmap과 화면 표시용 이미지 기능을 사용합니다.
using System.IO; // MemoryStream을 사용하여 Mat을 Bitmap으로 변환합니다.
using System.Linq; // 윤곽선 필터링을 위해 LINQ 기능을 사용합니다.
using System.Windows.Forms; // WinForms 화면, 버튼, PictureBox 기능을 사용합니다.

namespace ProductEdgeViewer; // 이 파일이 속한 프로젝트의 네임스페이스입니다.

public partial class MainForm : Form // WinForms Designer 파일과 합쳐지도록 partial 클래스로 정의합니다.
{
    private readonly Button btnOpen = new(); // 사진 열기 버튼을 생성합니다.
    private readonly Label lblStatus = new(); // 현재 처리 상태를 보여줄 라벨을 생성합니다.
    private readonly PictureBox picOriginal = new(); // 원본 이미지를 표시할 PictureBox를 생성합니다.
    private readonly PictureBox picResult = new(); // 경계 검출 결과 이미지를 표시할 PictureBox를 생성합니다.

    public MainForm() // MainForm 화면이 생성될 때 실행되는 생성자입니다.
    {
        InitializeComponent(); // Visual Studio WinForms Designer가 만든 기본 초기화 코드를 실행합니다.

        Controls.Clear(); // Designer에서 자동 생성된 기본 컨트롤이 있으면 모두 제거하고 코드 기반 화면으로 다시 구성합니다.

        Text = "C# OpenCV 제품 경계 검출 시연"; // 창 제목을 설정합니다.
        Width = 1200; // 창 너비를 1200픽셀로 설정합니다.
        Height = 750; // 창 높이를 750픽셀로 설정합니다.
        StartPosition = FormStartPosition.CenterScreen; // 화면 가운데에서 프로그램을 시작합니다.

        var root = new TableLayoutPanel(); // 전체 화면을 위아래로 나눌 레이아웃을 생성합니다.
        root.Dock = DockStyle.Fill; // 레이아웃을 창 전체에 채웁니다.
        root.RowCount = 2; // 행을 2개로 나눕니다.
        root.ColumnCount = 1; // 열은 1개만 사용합니다.
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // 첫 번째 행은 버튼 영역으로 60픽셀을 사용합니다.
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // 두 번째 행은 이미지 영역으로 남은 공간을 사용합니다.
        Controls.Add(root); // 만든 레이아웃을 화면에 추가합니다.

        var topPanel = new FlowLayoutPanel(); // 버튼과 상태 라벨을 담을 상단 패널을 생성합니다.
        topPanel.Dock = DockStyle.Fill; // 상단 패널을 첫 번째 행 전체에 채웁니다.
        topPanel.Padding = new Padding(12); // 내부 여백을 12픽셀로 설정합니다.
        topPanel.FlowDirection = FlowDirection.LeftToRight; // 컨트롤을 왼쪽에서 오른쪽으로 배치합니다.
        root.Controls.Add(topPanel, 0, 0); // 상단 패널을 첫 번째 행에 추가합니다.

        btnOpen.Text = "사진 열기"; // 버튼에 표시될 글자를 설정합니다.
        btnOpen.Width = 120; // 버튼 너비를 120픽셀로 설정합니다.
        btnOpen.Height = 32; // 버튼 높이를 32픽셀로 설정합니다.
        btnOpen.Click += BtnOpen_Click; // 버튼 클릭 시 실행할 이벤트 함수를 연결합니다.
        topPanel.Controls.Add(btnOpen); // 버튼을 상단 패널에 추가합니다.

        lblStatus.Text = "상태: 사진을 열어 주세요."; // 초기 상태 메시지를 설정합니다.
        lblStatus.AutoSize = true; // 라벨 크기를 글자 길이에 맞게 자동 조절합니다.
        lblStatus.Padding = new Padding(12, 7, 0, 0); // 라벨 위치를 보기 좋게 조정합니다.
        topPanel.Controls.Add(lblStatus); // 상태 라벨을 상단 패널에 추가합니다.

        var imagePanel = new TableLayoutPanel(); // 원본과 결과 이미지를 좌우로 나눌 레이아웃을 생성합니다.
        imagePanel.Dock = DockStyle.Fill; // 이미지 패널을 두 번째 행 전체에 채웁니다.
        imagePanel.RowCount = 2; // 제목 행과 이미지 행으로 나눕니다.
        imagePanel.ColumnCount = 2; // 왼쪽 원본, 오른쪽 결과로 나눕니다.
        imagePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // 왼쪽 열은 전체의 50퍼센트입니다.
        imagePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // 오른쪽 열은 전체의 50퍼센트입니다.
        imagePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // 제목 행 높이를 40픽셀로 설정합니다.
        imagePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // 이미지 행은 남은 공간을 사용합니다.
        root.Controls.Add(imagePanel, 0, 1); // 이미지 패널을 두 번째 행에 추가합니다.

        imagePanel.Controls.Add(MakeTitleLabel("원본 사진"), 0, 0); // 왼쪽 제목 라벨을 추가합니다.
        imagePanel.Controls.Add(MakeTitleLabel("경계 검출 결과"), 1, 0); // 오른쪽 제목 라벨을 추가합니다.

        ConfigurePictureBox(picOriginal); // 원본 PictureBox의 공통 설정을 적용합니다.
        ConfigurePictureBox(picResult); // 결과 PictureBox의 공통 설정을 적용합니다.

        imagePanel.Controls.Add(picOriginal, 0, 1); // 원본 PictureBox를 왼쪽 이미지 영역에 추가합니다.
        imagePanel.Controls.Add(picResult, 1, 1); // 결과 PictureBox를 오른쪽 이미지 영역에 추가합니다.
    }

    private static Label MakeTitleLabel(string text) // 이미지 영역의 제목 라벨을 만드는 함수입니다.
    {
        return new Label // 라벨 객체를 생성하여 반환합니다.
        {
            Text = text, // 라벨에 표시할 제목을 설정합니다.
            Dock = DockStyle.Fill, // 라벨을 셀 전체에 채웁니다.
            TextAlign = ContentAlignment.MiddleCenter, // 글자를 가운데 정렬합니다.
            Font = new Font("맑은 고딕", 12, FontStyle.Bold) // 한글이 잘 보이는 글꼴과 굵기를 설정합니다.
        };
    }

    private static void ConfigurePictureBox(PictureBox pictureBox) // PictureBox 공통 설정을 적용하는 함수입니다.
    {
        pictureBox.Dock = DockStyle.Fill; // PictureBox를 셀 전체에 채웁니다.
        pictureBox.SizeMode = PictureBoxSizeMode.Zoom; // 이미지 비율을 유지하면서 화면에 맞춥니다.
        pictureBox.BorderStyle = BorderStyle.FixedSingle; // 이미지 영역의 테두리를 표시합니다.
        pictureBox.BackColor = Color.FromArgb(245, 245, 245); // 이미지가 없을 때 배경색을 연한 회색으로 설정합니다.
    }

    private void BtnOpen_Click(object? sender, EventArgs e) // 사진 열기 버튼을 눌렀을 때 실행되는 함수입니다.
    {
        using var dialog = new OpenFileDialog(); // 파일 선택 창을 생성하고 사용 후 자동 정리합니다.
        dialog.Title = "제품 사진 선택"; // 파일 선택 창의 제목을 설정합니다.
        dialog.Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp|모든 파일|*.*"; // 선택 가능한 이미지 확장자를 제한합니다.

        if (dialog.ShowDialog() != DialogResult.OK) // 사용자가 파일 선택을 취소했는지 확인합니다.
        {
            return; // 취소했다면 아무 작업도 하지 않고 함수를 종료합니다.
        }

        ProcessImage(dialog.FileName); // 사용자가 선택한 이미지 파일을 처리합니다.
    }

    private void ProcessImage(string filePath) // 이미지 파일을 읽고 경계 검출을 수행하는 함수입니다.
    {
        using var original = Cv2.ImRead(filePath, ImreadModes.Color); // OpenCV로 컬러 이미지를 읽습니다.

        if (original.Empty()) // 이미지 읽기에 실패했는지 확인합니다.
        {
            MessageBox.Show("이미지를 읽을 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); // 오류 메시지를 표시합니다.
            return; // 더 이상 처리하지 않고 함수를 종료합니다.
        }

        using var gray = new Mat(); // 흑백 이미지를 저장할 Mat 객체를 생성합니다.
        using var blurred = new Mat(); // 블러 처리된 이미지를 저장할 Mat 객체를 생성합니다.
        using var edges = new Mat(); // Canny 결과 경계 이미지를 저장할 Mat 객체를 생성합니다.
        using var closed = new Mat(); // 끊어진 경계선을 보정한 이미지를 저장할 Mat 객체를 생성합니다.
        using var result = original.Clone(); // 원본을 복사하여 결과 이미지를 그릴 Mat 객체를 생성합니다.

        Cv2.CvtColor(original, gray, ColorConversionCodes.BGR2GRAY); // 컬러 이미지를 흑백 이미지로 변환합니다.
        Cv2.GaussianBlur(gray, blurred, new OpenCvSharp.Size(5, 5), 1.5); // 노이즈를 줄이기 위해 가우시안 블러를 적용합니다.
        Cv2.Canny(blurred, edges, 50, 150); // Canny 알고리즘으로 경계선을 검출합니다.

        using var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3)); // 경계선 보정용 3x3 사각 커널을 생성합니다.
        Cv2.MorphologyEx(edges, closed, MorphTypes.Close, kernel); // 끊어진 작은 경계선을 닫기 연산으로 연결합니다.

        Cv2.FindContours( // 이진 이미지에서 윤곽선을 찾는 OpenCV 함수를 실행합니다.
            closed.Clone(), // FindContours가 이미지를 내부적으로 수정할 수 있으므로 복사본을 전달합니다.
            out OpenCvSharp.Point[][] contours, // 검출된 윤곽선 좌표 목록을 저장합니다.
            out HierarchyIndex[] hierarchy, // 윤곽선의 포함 관계 정보를 저장합니다.
            RetrievalModes.External, // 가장 바깥쪽 외곽선만 찾습니다.
            ContourApproximationModes.ApproxSimple // 직선 구간의 중복 점을 줄여 메모리를 절약합니다.
        );

        List<OpenCvSharp.Point[]> filteredContours = contours // 검출된 전체 윤곽선 목록을 대상으로 합니다.
            .Where(c => Cv2.ContourArea(c) > 300) // 면적이 300보다 큰 윤곽선만 남겨 작은 잡음을 제거합니다.
            .ToList(); // 필터링 결과를 List 형태로 변환합니다.

        Cv2.DrawContours(result, filteredContours, -1, Scalar.LimeGreen, 3); // 필터링된 윤곽선을 원본 복사 이미지 위에 초록색으로 그립니다.

        ReplaceImage(picOriginal, MatToBitmap(original)); // 왼쪽 PictureBox에 원본 이미지를 표시합니다.
        ReplaceImage(picResult, MatToBitmap(result)); // 오른쪽 PictureBox에 윤곽선 결과 이미지를 표시합니다.

        lblStatus.Text = $"상태: 경계 검출 완료 / 윤곽선 {filteredContours.Count}개 검출"; // 처리 결과를 상태 라벨에 표시합니다.
    }

    private static Bitmap MatToBitmap(Mat mat) // OpenCV Mat 이미지를 WinForms Bitmap 이미지로 변환하는 함수입니다.
    {
        Cv2.ImEncode(".bmp", mat, out byte[] bytes); // Mat 이미지를 BMP 형식의 바이트 배열로 인코딩합니다.
        using var stream = new MemoryStream(bytes); // 바이트 배열을 읽기 위한 메모리 스트림을 생성합니다.
        using var tempBitmap = new Bitmap(stream); // 스트림에서 임시 Bitmap 객체를 생성합니다.
        return new Bitmap(tempBitmap); // 스트림이 닫혀도 안전하게 사용할 수 있도록 새 Bitmap으로 복사하여 반환합니다.
    }

    private static void ReplaceImage(PictureBox pictureBox, Bitmap bitmap) // PictureBox의 기존 이미지를 새 이미지로 교체하는 함수입니다.
    {
        Image? oldImage = pictureBox.Image; // 기존 이미지를 임시 변수에 저장합니다.
        pictureBox.Image = bitmap; // 새 이미지를 PictureBox에 표시합니다.
        oldImage?.Dispose(); // 기존 이미지가 있으면 메모리 누수를 막기 위해 해제합니다.
    }
}
```

---

## 13. 실행 방법

Visual Studio에서 실행합니다.

```text
상단 메뉴 → 디버그 → 디버그하지 않고 시작
```

또는 단축키:

```text
Ctrl + F5
```

실행 후:

```text
1. 사진 열기 버튼 클릭
2. 제품 사진 선택
3. 왼쪽 원본 확인
4. 오른쪽 경계 검출 결과 확인
```

---

## 14. 실습용 사진 선택 기준

처음 실습할 때는 다음 조건의 사진이 좋습니다.

| 좋은 사진 조건 | 이유 |
|---|---|
| 제품과 배경 색이 다름 | 경계가 뚜렷하게 검출됨 |
| 배경이 단순함 | 불필요한 윤곽선이 적음 |
| 제품이 화면 중앙에 있음 | 결과를 학생들이 이해하기 쉬움 |
| 조명이 너무 어둡지 않음 | 밝기 변화가 안정적으로 잡힘 |
| 그림자가 너무 강하지 않음 | 그림자를 제품 경계로 오해할 가능성이 줄어듦 |

피해야 할 사진:

```text
배경이 복잡한 사진
제품과 배경 색이 비슷한 사진
그림자가 강한 사진
반사가 심한 금속 제품 사진
너무 흐린 사진
```

---

## 15. 주요 코드 해설

### 15.1 이미지 읽기

```csharp
using var original = Cv2.ImRead(filePath, ImreadModes.Color);
```

OpenCV가 이미지 파일을 읽어 `Mat` 객체로 만듭니다.

`Mat`은 OpenCV에서 이미지를 담는 핵심 자료구조입니다.

쉽게 말하면 이미지 픽셀들이 들어 있는 행렬입니다.

---

### 15.2 흑백 변환

```csharp
Cv2.CvtColor(original, gray, ColorConversionCodes.BGR2GRAY);
```

컬러 이미지를 흑백 이미지로 바꿉니다.

경계 검출에서는 색상보다 밝기 변화가 중요하기 때문입니다.

---

### 15.3 Gaussian Blur

```csharp
Cv2.GaussianBlur(gray, blurred, new OpenCvSharp.Size(5, 5), 1.5);
```

이미지를 약간 부드럽게 만들어 작은 노이즈를 줄입니다.

`new OpenCvSharp.Size(5, 5)`는 주변 5x5 영역을 참고한다는 뜻입니다.

`1.5`는 흐림 정도를 나타내는 sigma 값입니다.

---

### 15.4 Canny Edge Detection

```csharp
Cv2.Canny(blurred, edges, 50, 150);
```

밝기 변화가 큰 부분을 경계선으로 찾습니다.

`50`은 낮은 임계값입니다.

`150`은 높은 임계값입니다.

값을 낮추면 더 많은 경계가 검출됩니다.

값을 높이면 강한 경계만 남습니다.

---

### 15.5 Morphology Close

```csharp
Cv2.MorphologyEx(edges, closed, MorphTypes.Close, kernel);
```

Canny 결과는 선이 끊어져 나오는 경우가 있습니다.

Close 연산은 작은 틈을 메우는 데 도움이 됩니다.

제품 외곽선을 하나의 윤곽선으로 잡는 데 유리합니다.

---

### 15.6 FindContours

```csharp
Cv2.FindContours(closed.Clone(), out OpenCvSharp.Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
```

경계선 이미지에서 연결된 외곽선을 찾습니다.

`RetrievalModes.External`은 가장 바깥쪽 윤곽선을 중심으로 찾겠다는 뜻입니다.

제품 외곽선을 찾는 시연에서는 내부의 작은 글자나 무늬보다 바깥 경계가 중요하므로 이 설정이 적합합니다.

---

### 15.7 작은 윤곽선 제거

```csharp
List<OpenCvSharp.Point[]> filteredContours = contours.Where(c => Cv2.ContourArea(c) > 300).ToList();
```

너무 작은 윤곽선은 먼지, 노이즈, 작은 무늬일 가능성이 높습니다.

면적이 300보다 큰 윤곽선만 남깁니다.

사진 크기와 제품 크기에 따라 이 값은 조정해야 합니다.

---

### 15.8 윤곽선 그리기

```csharp
Cv2.DrawContours(result, filteredContours, -1, Scalar.LimeGreen, 3);
```

검출된 윤곽선을 원본 이미지 위에 초록색 선으로 그립니다.

`-1`은 모든 윤곽선을 그리겠다는 뜻입니다.

`3`은 선 두께입니다.

---

## 16. 학생 실습 과제

### 과제 1: Canny 임계값 바꿔보기

다음 코드를 찾습니다.

```csharp
Cv2.Canny(blurred, edges, 50, 150);
```

아래처럼 바꿔 봅니다.

```csharp
Cv2.Canny(blurred, edges, 30, 100);
```

또는:

```csharp
Cv2.Canny(blurred, edges, 100, 200);
```

관찰할 점:

```text
임계값이 낮으면 더 많은 선이 나온다.
임계값이 높으면 강한 선만 남는다.
```

---

### 과제 2: 작은 윤곽선 제거 기준 바꿔보기

다음 코드를 찾습니다.

```csharp
Cv2.ContourArea(c) > 300
```

아래처럼 바꿔 봅니다.

```csharp
Cv2.ContourArea(c) > 1000
```

관찰할 점:

```text
기준값이 작으면 작은 잡음 윤곽선도 표시된다.
기준값이 크면 큰 제품 외곽선만 남는다.
```

---

### 과제 3: 선 색과 두께 바꿔보기

다음 코드를 찾습니다.

```csharp
Cv2.DrawContours(result, filteredContours, -1, Scalar.LimeGreen, 3);
```

아래처럼 바꿔 봅니다.

```csharp
Cv2.DrawContours(result, filteredContours, -1, Scalar.Red, 5);
```

관찰할 점:

```text
선 색과 두께가 바뀐다.
교육용 시연에서는 굵은 선이 더 잘 보인다.
```

---

## 17. 수업에서 설명하면 좋은 추가 개념

### 17.1 OpenCV는 AI인가?

OpenCV 자체는 AI 모델만을 의미하지 않습니다.

OpenCV는 이미지 처리와 컴퓨터 비전 기능을 제공하는 라이브러리입니다.

이번 실습은 딥러닝 AI가 아니라 전통적인 컴퓨터 비전 알고리즘입니다.

정리하면 다음과 같습니다.

| 구분 | 설명 |
|---|---|
| OpenCV | 이미지 처리와 컴퓨터 비전 라이브러리 |
| Edge Detection | 밝기 변화 기반의 전통적 비전 알고리즘 |
| YOLO | 딥러닝 기반 객체 탐지 모델 |
| Vision AI | 이미지 처리, 객체 탐지, 분류, 추적 등을 포함하는 넓은 분야 |

---

### 17.2 이 방식의 장점

```text
AI 학습 데이터가 필요 없다.
설치와 실행이 비교적 간단하다.
처리 과정이 눈에 보인다.
초급자가 원리를 이해하기 좋다.
제품 외곽, 모서리, 단순 결함 시연에 적합하다.
```

---

### 17.3 이 방식의 한계

```text
조명 변화에 약하다.
배경이 복잡하면 잘못된 경계가 많이 나온다.
제품과 배경 색이 비슷하면 경계가 약해진다.
그림자도 경계로 오해할 수 있다.
제품 종류가 다양해지면 규칙만으로 대응하기 어렵다.
```

그래서 실제 산업 현장에서는 다음 기술들을 함께 사용합니다.

```text
조명 장치
고정 카메라
단색 배경
Thresholding
Contour filtering
Template matching
딥러닝 객체 탐지
검사 기준 수치화
```

---

## 18. 다음 단계 확장 아이디어

이번 실습을 마친 뒤 다음 순서로 확장하면 좋습니다.

| 단계 | 주제 | 설명 |
|---|---|---|
| 1 | 사진 1장 경계 검출 | 이번 튜토리얼 |
| 2 | 이진화 Threshold | 제품과 배경을 흑백으로 분리 |
| 3 | 가장 큰 Contour 찾기 | 제품 하나만 자동 선택 |
| 4 | Bounding Rectangle | 제품을 사각형 박스로 표시 |
| 5 | 면적 측정 | 제품 크기나 결함 면적 추정 |
| 6 | 원형/사각형 판별 | 형상 검사 기초 |
| 7 | 동영상 처리 | 프레임마다 같은 처리 반복 |
| 8 | YOLO/ONNX | 사람, 제품, 불량 유형 탐지로 확장 |

---

## 19. 자주 발생하는 문제

### 문제 1: OpenCvSharp 네임스페이스를 찾을 수 없음

원인:

```text
NuGet 패키지가 설치되지 않았거나 프로젝트가 복원되지 않았을 가능성이 큽니다.
```

해결:

```text
프로젝트 우클릭 → NuGet 패키지 관리 → OpenCvSharp4.Windows 설치 확인
빌드 → 솔루션 정리
빌드 → 솔루션 다시 빌드
```

---

### 문제 2: 이미지를 열었는데 아무것도 안 나옴

원인:

```text
이미지 경로 문제 또는 지원되지 않는 파일 형식일 수 있습니다.
```

해결:

```text
jpg, png, bmp 파일로 테스트합니다.
한글이나 특수문자가 너무 많은 경로를 피해서 바탕화면 또는 C:\Temp 같은 경로에서 테스트합니다.
```

---

### 문제 3: 경계가 너무 많이 나옴

원인:

```text
배경이 복잡하거나 Canny 임계값이 너무 낮을 수 있습니다.
```

해결:

```csharp
Cv2.Canny(blurred, edges, 100, 200);
```

또는 제품을 단색 배경 위에 놓고 촬영합니다.

---

### 문제 4: 제품 경계가 잘 안 나옴

원인:

```text
제품과 배경의 밝기 차이가 작거나 사진이 흐릴 수 있습니다.
```

해결:

```text
배경을 바꿉니다.
조명을 밝게 합니다.
제품과 배경의 색 차이를 크게 합니다.
Canny 임계값을 낮춥니다.
```

예시:

```csharp
Cv2.Canny(blurred, edges, 30, 100);
```

---

## 20. 수업용 설명 문장 예시

학생들에게 다음처럼 설명하면 이해가 쉽습니다.

```text
컴퓨터는 사진을 사람처럼 보는 것이 아니라 숫자 표로 봅니다.
제품의 경계는 보통 밝기 값이 갑자기 바뀌는 지점입니다.
OpenCV의 Canny 알고리즘은 이런 밝기 변화가 큰 지점을 찾아 선으로 표시합니다.
그다음 FindContours는 연결된 선들을 하나의 외곽선 후보로 묶습니다.
마지막으로 DrawContours를 사용해 원본 이미지 위에 초록색 선으로 표시합니다.
그래서 왼쪽은 사람이 보는 원본 사진이고, 오른쪽은 컴퓨터가 경계라고 판단한 부분을 시각화한 결과입니다.
```

---

## 21. 최종 정리

이번 실습에서 배운 핵심은 다음과 같습니다.

```text
이미지는 픽셀 숫자 배열이다.
경계는 밝기 변화가 큰 위치다.
Canny는 노이즈 제거, 밝기 변화 계산, 얇은 선 추출, 임계값 처리를 거쳐 경계를 찾는다.
Contour는 연결된 경계선을 제품 외곽선처럼 묶은 것이다.
OpenCV는 이런 복잡한 이미지 처리 알고리즘을 C#에서 쉽게 사용할 수 있게 해준다.
```

이 실습은 딥러닝 AI 이전에 반드시 해볼 만한 Vision 기초 실습입니다.

학생들이 이 원리를 이해하면 이후 YOLO, ONNX Runtime, 산업용 비전 검사, 안전 감지 시스템으로 확장할 때 훨씬 빠르게 이해할 수 있습니다.

---

## 22. 참고 자료

- OpenCV GitHub Repository: https://github.com/opencv/opencv
- OpenCvSharp GitHub Repository: https://github.com/shimat/opencvsharp
- OpenCvSharp4.Windows NuGet: https://www.nuget.org/packages/OpenCvSharp4.Windows
- OpenCV Canny Edge Detection Tutorial: https://docs.opencv.org/4.x/da/d22/tutorial_py_canny.html
- OpenCV Smoothing Images Tutorial: https://docs.opencv.org/4.x/d4/d13/tutorial_py_filtering.html
- OpenCV Thresholding Tutorial: https://docs.opencv.org/4.x/d7/d4d/tutorial_py_thresholding.html

