# C# WPF + OpenCVSharp 제품 경계 검출 학습 프로젝트 전체 튜토리얼

> 이 문서는 **Visual Studio + WPF + OpenCvSharp** 환경에서,
> **사진 한 장을 불러와 경계 검출 과정을 단계별로 시각화**하는 학습용 프로그램을
> **그대로 따라 하면 완성되도록** 정리한 `.md` 튜토리얼입니다.
>
> 특히 이번 버전은 단순히 **원본 / 최종 결과**만 보여주는 것이 아니라,
> 아래의 **중간 처리 단계까지 전부 시각화**합니다.
>
> ```text
> 원본 이미지
> → 흑백 변환
> → 가우시안 블러
> → Canny 엣지 검출
> → 윤곽선 시각화 최종 결과
> ```

---

# 1. 이 프로젝트로 배우는 것

이 프로젝트를 통해 학생들은 아래 내용을 한 번에 체험할 수 있습니다.

- WPF에서 사진 파일을 불러오는 방법
- OpenCvSharp를 C#에서 사용하는 방법
- 이미지가 내부적으로 어떤 순서로 처리되는지
- 왜 어떤 사진은 잘 검출되고, 어떤 사진은 잘 안 되는지
- **Edge(엣지)** 와 **Contour(윤곽선)** 의 차이
- OpenCV가 “물체를 이해”하는 것이 아니라 “밝기 변화”를 찾는다는 사실

---

# 2. 최종 프로그램 화면 구성

최종 프로그램은 아래 5개의 이미지를 한 화면에서 보여줍니다.

1. **원본 사진**
2. **흑백 변환 이미지**
3. **블러 처리 이미지**
4. **Canny 엣지 이미지**
5. **최종 윤곽선 시각화 결과**

즉, 학생이 “결과만 보는 것”이 아니라,
**중간 단계가 어떻게 바뀌는지**를 직접 눈으로 볼 수 있게 하는 것이 핵심입니다.

---

# 3. 왜 WPF를 사용하는가?

이번 프로젝트는 WinForms 대신 **WPF**를 사용합니다.

그 이유는 다음과 같습니다.

| 항목 | WinForms | WPF |
|---|---|---|
| DPI / 해상도 대응 | 상대적으로 불편 | 더 안정적 |
| UI 배치 | 수동 조절이 자주 필요 | Grid 기반으로 깔끔 |
| 이미지 표시 | 가능 | 더 유연함 |
| 학습용 시각화 UI | 가능하지만 다소 불편 | 훨씬 적합 |

즉, **중간 처리 결과를 여러 장 배치하는 학습용 프로그램**에는 WPF가 더 적합합니다.

---

# 4. 개발 환경

## 4-1. 사용 도구

- Visual Studio 2022 이상 권장
- .NET 8 WPF 앱 권장
- NuGet 패키지:
  - `OpenCvSharp4`
  - `OpenCvSharp4.runtime.win`

## 4-2. 프로젝트 종류

Visual Studio에서 아래 프로젝트를 선택합니다.

```text
WPF 앱
```

---

# 5. 프로젝트 생성 방법

## 5-1. Visual Studio에서 생성

아래 순서로 진행합니다.

```text
1. Visual Studio 실행
2. 새 프로젝트 만들기 클릭
3. "WPF 앱" 선택
4. 다음 클릭
5. 프로젝트 이름: ProductEdgeViewerWpf
6. 위치 선택
7. 프레임워크: .NET 8.0 선택
8. 만들기 클릭
```

---

# 6. OpenCvSharp 라이브러리 설치 방법

이 단계가 매우 중요합니다.

## 6-1. Visual Studio의 패키지 관리자 콘솔에서 설치

아래 위치에서 실행합니다.

```text
Visual Studio 상단 메뉴
→ 도구
→ NuGet 패키지 관리자
→ 패키지 관리자 콘솔
```

패키지 관리자 콘솔 창에서 **기본 프로젝트**가 아래처럼 되어 있는지 확인합니다.

```text
기본 프로젝트: ProductEdgeViewerWpf
```

그 다음 아래 명령어를 그대로 실행합니다.

```powershell
Install-Package OpenCvSharp4
Install-Package OpenCvSharp4.runtime.win
```

## 6-2. 왜 이 라이브러리가 필요한가?

### `OpenCvSharp4`

이 패키지는 C#에서 OpenCV 함수를 직접 사용할 수 있게 해 줍니다.

예를 들어 아래 함수들을 사용하게 됩니다.

- `Cv2.ImRead()`
- `Cv2.CvtColor()`
- `Cv2.GaussianBlur()`
- `Cv2.Canny()`
- `Cv2.FindContours()`
- `Cv2.DrawContours()`

### `OpenCvSharp4.runtime.win`

이 패키지는 Windows에서 OpenCV의 실제 네이티브 DLL이 돌아가도록 해 줍니다.

즉,

- `OpenCvSharp4` = C#용 래퍼
- `OpenCvSharp4.runtime.win` = 실제 Windows 실행용 런타임

둘 다 있어야 정상 동작합니다.

---

# 7. 폴더 구조

최종 프로젝트 폴더 구조는 아래처럼 맞춰 주세요.

```text
ProductEdgeViewerWpf
 ├─ App.xaml
 ├─ App.xaml.cs
 ├─ MainWindow.xaml
 ├─ MainWindow.xaml.cs
 ├─ Services
 │   ├─ EdgeDetectionResult.cs
 │   └─ EdgeDetectionService.cs
 └─ Utils
     └─ MatBitmapConverter.cs
```

## 7-1. 폴더 만드는 방법

Visual Studio의 **솔루션 탐색기**에서 아래 순서로 만듭니다.

```text
프로젝트 우클릭
→ 추가
→ 새 폴더
→ Services

프로젝트 우클릭
→ 추가
→ 새 폴더
→ Utils
```

그 다음 각 폴더에 클래스를 추가합니다.

```text
Services 폴더 우클릭
→ 추가
→ 클래스
→ EdgeDetectionResult.cs

Services 폴더 우클릭
→ 추가
→ 클래스
→ EdgeDetectionService.cs

Utils 폴더 우클릭
→ 추가
→ 클래스
→ MatBitmapConverter.cs
```

---

# 8. 전체 코드 제공

아래 코드를 각 파일에 그대로 넣으면 됩니다.

---

# 파일 1/7 — `ProductEdgeViewerWpf/App.xaml`

## 무엇을 위한 파일인가?

이 파일은 **WPF 애플리케이션 시작 설정**을 담당합니다.

```xml
<!-- ProductEdgeViewerWpf/App.xaml -->
<!-- 이 파일은 WPF 프로그램의 시작 설정을 담당합니다. -->

<Application x:Class="ProductEdgeViewerWpf.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="MainWindow.xaml">
    <!-- 프로그램이 시작될 때 MainWindow.xaml 창을 자동으로 엽니다. -->

    <Application.Resources>
        <!-- 전역 리소스가 필요하면 이곳에 추가할 수 있습니다. -->
    </Application.Resources>
</Application>
```

---

# 파일 2/7 — `ProductEdgeViewerWpf/App.xaml.cs`

## 무엇을 위한 파일인가?

이 파일은 **WPF 애플리케이션 클래스**를 담당합니다.

```csharp
// ProductEdgeViewerWpf/App.xaml.cs // 이 파일은 WPF 애플리케이션의 App 클래스를 담당합니다.

using System.Windows; // WPF Application 클래스를 사용하기 위해 필요한 네임스페이스입니다.

namespace ProductEdgeViewerWpf; // 현재 파일이 속한 프로젝트 네임스페이스입니다.

public partial class App : Application // App.xaml과 연결되는 WPF 애플리케이션 클래스입니다.
{
    // 현재는 별도 시작 로직이 없으므로 비워 둡니다.
}
```

---

# 파일 3/7 — `ProductEdgeViewerWpf/MainWindow.xaml`

## 무엇을 위한 파일인가?

이 파일은 **사용자가 보는 전체 화면 UI**를 담당합니다.

아래 코드는 길기 때문에 **1/2, 2/2**로 나누어 제공합니다.

## 1/2

```xml
<!-- ProductEdgeViewerWpf/MainWindow.xaml -->
<!-- 이 파일은 사용자가 보는 메인 화면 UI를 담당합니다. -->

<Window x:Class="ProductEdgeViewerWpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="C# OpenCV 단계별 경계 검출 학습 도구"
        Width="1650"
        Height="980"
        MinWidth="1280"
        MinHeight="820"
        WindowStartupLocation="CenterScreen"
        Background="#F1F5F9"
        FontFamily="Malgun Gothic">

    <Window.Resources>
        <!-- 카드 모양 Border에 공통으로 적용할 스타일입니다. -->
        <Style x:Key="CardBorderStyle" TargetType="Border">
            <Setter Property="Background" Value="White"/>
            <Setter Property="BorderBrush" Value="#CBD5E1"/>
            <Setter Property="BorderThickness" Value="1"/>
            <Setter Property="CornerRadius" Value="10"/>
            <Setter Property="Padding" Value="16"/>
        </Style>

        <!-- 이미지 제목용 TextBlock 공통 스타일입니다. -->
        <Style x:Key="ImageTitleStyle" TargetType="TextBlock">
            <Setter Property="FontSize" Value="20"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="Foreground" Value="#0F172A"/>
            <Setter Property="HorizontalAlignment" Value="Center"/>
            <Setter Property="Margin" Value="0,0,0,10"/>
        </Style>

        <!-- 이미지 설명용 TextBlock 공통 스타일입니다. -->
        <Style x:Key="ImageDescriptionStyle" TargetType="TextBlock">
            <Setter Property="FontSize" Value="13"/>
            <Setter Property="Foreground" Value="#64748B"/>
            <Setter Property="HorizontalAlignment" Value="Center"/>
            <Setter Property="TextAlignment" Value="Center"/>
            <Setter Property="Margin" Value="0,0,0,12"/>
            <Setter Property="TextWrapping" Value="Wrap"/>
        </Style>
    </Window.Resources>

    <Grid Margin="20">
        <!-- 전체 화면을 상단 정보 영역과 본문 영역으로 나눕니다. -->
        <Grid.RowDefinitions>
            <RowDefinition Height="110"/>
            <RowDefinition Height="56"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!-- 상단 버튼 및 상태 표시 영역입니다. -->
        <Border Grid.Row="0" Style="{StaticResource CardBorderStyle}">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="180"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>

                <Button x:Name="BtnOpen"
                        Grid.Column="0"
                        Content="사진 열기"
                        Width="150"
                        Height="48"
                        FontSize="18"
                        FontWeight="Bold"
                        Foreground="White"
                        Background="#2563EB"
                        BorderThickness="0"
                        Cursor="Hand"
                        HorizontalAlignment="Left"
                        VerticalAlignment="Center"
                        Click="BtnOpen_Click"/>

                <StackPanel Grid.Column="1" VerticalAlignment="Center">
                    <TextBlock x:Name="TxtStatus"
                               Text="상태: 사진을 열어 주세요."
                               FontSize="18"
                               FontWeight="Bold"
                               Foreground="#0F172A"
                               TextTrimming="CharacterEllipsis"/>

                    <TextBlock Text="이 프로그램은 원본 → 흑백 → 블러 → Canny 엣지 → 최종 윤곽선 결과를 단계별로 시각화합니다."
                               FontSize="13"
                               Foreground="#64748B"
                               Margin="0,6,0,0"
                               TextWrapping="Wrap"/>
                </StackPanel>
            </Grid>
        </Border>

        <!-- 처리 순서를 시각적으로 보여주는 간단한 안내 영역입니다. -->
        <Border Grid.Row="1" Margin="0,14,0,14" Background="#E2E8F0" CornerRadius="8" Padding="14">
            <TextBlock Text="처리 순서: 원본 이미지 → 흑백 변환 → 가우시안 블러 → Canny 엣지 검출 → Morphology Close → 윤곽선 찾기 → 최종 시각화"
                       FontSize="14"
                       FontWeight="SemiBold"
                       Foreground="#334155"
                       VerticalAlignment="Center"
                       TextWrapping="Wrap"/>
        </Border>
```

## 2/2

```xml
        <!-- 본문 영역은 스크롤 가능하게 하여 작은 화면에서도 잘 보이도록 합니다. -->
        <ScrollViewer Grid.Row="2" VerticalScrollBarVisibility="Auto">
            <UniformGrid Columns="3" Rows="2" Margin="0,0,0,10">
                <!-- 1. 원본 사진 카드 -->
                <Border Style="{StaticResource CardBorderStyle}" Margin="8">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="*"/>
                        </Grid.RowDefinitions>

                        <TextBlock Grid.Row="0" Text="1. 원본 사진" Style="{StaticResource ImageTitleStyle}"/>
                        <TextBlock Grid.Row="1" Text="사용자가 불러온 원본 이미지입니다." Style="{StaticResource ImageDescriptionStyle}"/>

                        <Border Grid.Row="2" Background="#F8FAFC" BorderBrush="#CBD5E1" BorderThickness="1" CornerRadius="6">
                            <Image x:Name="ImgOriginal" Stretch="Uniform" Margin="10"/>
                        </Border>
                    </Grid>
                </Border>

                <!-- 2. 흑백 변환 카드 -->
                <Border Style="{StaticResource CardBorderStyle}" Margin="8">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="*"/>
                        </Grid.RowDefinitions>

                        <TextBlock Grid.Row="0" Text="2. 흑백 변환" Style="{StaticResource ImageTitleStyle}"/>
                        <TextBlock Grid.Row="1" Text="색상 정보를 제거하고 밝기 정보 중심으로 변환한 결과입니다." Style="{StaticResource ImageDescriptionStyle}"/>

                        <Border Grid.Row="2" Background="#F8FAFC" BorderBrush="#CBD5E1" BorderThickness="1" CornerRadius="6">
                            <Image x:Name="ImgGray" Stretch="Uniform" Margin="10"/>
                        </Border>
                    </Grid>
                </Border>

                <!-- 3. 블러 처리 카드 -->
                <Border Style="{StaticResource CardBorderStyle}" Margin="8">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="*"/>
                        </Grid.RowDefinitions>

                        <TextBlock Grid.Row="0" Text="3. 가우시안 블러" Style="{StaticResource ImageTitleStyle}"/>
                        <TextBlock Grid.Row="1" Text="작은 노이즈를 줄이고 불필요한 미세 변화를 완화한 결과입니다." Style="{StaticResource ImageDescriptionStyle}"/>

                        <Border Grid.Row="2" Background="#F8FAFC" BorderBrush="#CBD5E1" BorderThickness="1" CornerRadius="6">
                            <Image x:Name="ImgBlurred" Stretch="Uniform" Margin="10"/>
                        </Border>
                    </Grid>
                </Border>

                <!-- 4. Canny 엣지 카드 -->
                <Border Style="{StaticResource CardBorderStyle}" Margin="8">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="*"/>
                        </Grid.RowDefinitions>

                        <TextBlock Grid.Row="0" Text="4. Canny 엣지 검출" Style="{StaticResource ImageTitleStyle}"/>
                        <TextBlock Grid.Row="1" Text="밝기 변화가 큰 위치만 흰색 선으로 표시한 결과입니다." Style="{StaticResource ImageDescriptionStyle}"/>

                        <Border Grid.Row="2" Background="#F8FAFC" BorderBrush="#CBD5E1" BorderThickness="1" CornerRadius="6">
                            <Image x:Name="ImgEdges" Stretch="Uniform" Margin="10"/>
                        </Border>
                    </Grid>
                </Border>

                <!-- 5. 최종 윤곽선 결과 카드 -->
                <Border Style="{StaticResource CardBorderStyle}" Margin="8">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="*"/>
                        </Grid.RowDefinitions>

                        <TextBlock Grid.Row="0" Text="5. 최종 윤곽선 시각화" Style="{StaticResource ImageTitleStyle}"/>
                        <TextBlock Grid.Row="1" Text="필터링된 윤곽선을 원본 이미지 위에 초록색으로 그린 최종 결과입니다." Style="{StaticResource ImageDescriptionStyle}"/>

                        <Border Grid.Row="2" Background="#F8FAFC" BorderBrush="#CBD5E1" BorderThickness="1" CornerRadius="6">
                            <Image x:Name="ImgResult" Stretch="Uniform" Margin="10"/>
                        </Border>
                    </Grid>
                </Border>

                <!-- 6. 학습 설명 카드 -->
                <Border Style="{StaticResource CardBorderStyle}" Margin="8">
                    <ScrollViewer VerticalScrollBarVisibility="Auto">
                        <StackPanel>
                            <TextBlock Text="학습 핵심 정리" Style="{StaticResource ImageTitleStyle}"/>

                            <TextBlock TextWrapping="Wrap" FontSize="14" Foreground="#334155" Margin="0,4,0,10">
                                이 프로그램은 AI가 물체를 이해하는 것이 아니라,
                                이미지에서 밝기 변화가 큰 위치를 찾아 경계 후보로 판단합니다.
                            </TextBlock>

                            <TextBlock Text="• 원본: 사용자가 보는 사진" FontSize="14" Foreground="#334155" Margin="0,4,0,4"/>
                            <TextBlock Text="• 흑백: 색보다 밝기 변화에 집중" FontSize="14" Foreground="#334155" Margin="0,4,0,4"/>
                            <TextBlock Text="• 블러: 노이즈 감소" FontSize="14" Foreground="#334155" Margin="0,4,0,4"/>
                            <TextBlock Text="• Canny: 밝기 변화가 큰 곳만 추출" FontSize="14" Foreground="#334155" Margin="0,4,0,4"/>
                            <TextBlock Text="• Contour: 연결된 선을 윤곽선으로 묶음" FontSize="14" Foreground="#334155" Margin="0,4,0,4"/>
                            <TextBlock Text="• Final: 윤곽선을 원본 위에 시각화" FontSize="14" Foreground="#334155" Margin="0,4,0,4"/>

                            <TextBlock TextWrapping="Wrap" FontSize="14" Foreground="#475569" Margin="0,16,0,0">
                                금속 부품처럼 경계가 선명하고 딱딱한 물체는 잘 검출되지만,
                                털이 있는 고양이처럼 부드럽고 경계가 흐린 물체는 잘 안 잡힐 수 있습니다.
                            </TextBlock>
                        </StackPanel>
                    </ScrollViewer>
                </Border>
            </UniformGrid>
        </ScrollViewer>
    </Grid>
</Window>
```

---

# 파일 4/7 — `ProductEdgeViewerWpf/MainWindow.xaml.cs`

## 무엇을 위한 파일인가?

이 파일은 **버튼 클릭, 파일 열기, 이미지 처리 호출, 화면 반영**을 담당합니다.

이 파일도 길기 때문에 **1/2, 2/2**로 나누어 제공합니다.

## 1/2

```csharp
// ProductEdgeViewerWpf/MainWindow.xaml.cs // 이 파일은 버튼 이벤트와 화면 표시 로직을 담당합니다.

using Microsoft.Win32; // WPF에서 파일 선택 창인 OpenFileDialog를 사용하기 위해 필요합니다.
using ProductEdgeViewerWpf.Services; // OpenCV 경계 검출 서비스 클래스를 사용합니다.
using ProductEdgeViewerWpf.Utils; // Mat 이미지를 WPF ImageSource로 변환하는 유틸리티를 사용합니다.
using System; // Exception 같은 기본 .NET 기능을 사용합니다.
using System.IO; // 파일명만 추출하기 위해 Path 기능을 사용합니다.
using System.Windows; // WPF Window와 MessageBox 기능을 사용합니다.

namespace ProductEdgeViewerWpf; // 현재 파일이 속한 프로젝트 네임스페이스입니다.

public partial class MainWindow : Window // MainWindow.xaml과 연결되는 WPF 메인 창 클래스입니다.
{
    private readonly EdgeDetectionService edgeService = new(); // OpenCV 경계 검출 처리를 담당하는 서비스 객체입니다.

    public MainWindow() // 메인 창이 생성될 때 실행되는 생성자입니다.
    {
        InitializeComponent(); // XAML에 정의된 UI 요소들을 실제 객체로 초기화합니다.
    }

    private void BtnOpen_Click(object sender, RoutedEventArgs e) // 사진 열기 버튼을 클릭했을 때 실행되는 함수입니다.
    {
        var dialog = new OpenFileDialog(); // 파일 선택 창 객체를 생성합니다.
        dialog.Title = "제품 사진 선택"; // 파일 선택 창 제목을 설정합니다.
        dialog.Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp|모든 파일|*.*"; // 사용자가 선택할 수 있는 파일 형식을 지정합니다.

        bool? selected = dialog.ShowDialog(); // 파일 선택 창을 열고 사용자의 선택 결과를 받아옵니다.

        if (selected != true) // 사용자가 파일 선택을 취소했는지 확인합니다.
        {
            return; // 취소한 경우 아무 작업도 하지 않고 함수를 종료합니다.
        }

        ShowEdgeDetectionResult(dialog.FileName); // 선택한 이미지 파일을 처리하고 화면에 표시합니다.
    }
```

## 2/2

```csharp
    private void ShowEdgeDetectionResult(string filePath) // 이미지 경계 검출 결과를 화면에 표시하는 함수입니다.
    {
        try // 이미지 처리 과정에서 오류가 발생할 수 있으므로 try 블록을 사용합니다.
        {
            using EdgeDetectionResult result = edgeService.DetectEdges(filePath); // 서비스에서 처리 결과를 받아옵니다.

            ImgOriginal.Source = MatBitmapConverter.ToBitmapSource(result.Original); // 원본 Mat 이미지를 WPF Image에 표시합니다.
            ImgGray.Source = MatBitmapConverter.ToBitmapSource(result.Gray); // 흑백 변환 Mat 이미지를 WPF Image에 표시합니다.
            ImgBlurred.Source = MatBitmapConverter.ToBitmapSource(result.Blurred); // 블러 처리 Mat 이미지를 WPF Image에 표시합니다.
            ImgEdges.Source = MatBitmapConverter.ToBitmapSource(result.Edges); // Canny 엣지 Mat 이미지를 WPF Image에 표시합니다.
            ImgResult.Source = MatBitmapConverter.ToBitmapSource(result.Visualized); // 최종 시각화 Mat 이미지를 WPF Image에 표시합니다.

            string fileName = Path.GetFileName(filePath); // 전체 경로에서 파일명만 추출합니다.
            TxtStatus.Text = $"상태: 경계 검출 완료 / 윤곽선 {result.ContourCount}개 / 파일: {fileName}"; // 상태 표시줄에 처리 결과를 출력합니다.
        }
        catch (Exception ex) // 처리 중 오류가 발생했을 때 실행됩니다.
        {
            MessageBox.Show(ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error); // 오류 메시지를 사용자에게 보여줍니다.
        }
    }
}
```

---

# 파일 5/7 — `ProductEdgeViewerWpf/Services/EdgeDetectionResult.cs`

## 무엇을 위한 파일인가?

이 파일은 **중간 처리 결과와 최종 결과를 한 번에 담는 데이터 클래스**입니다.

```csharp
// ProductEdgeViewerWpf/Services/EdgeDetectionResult.cs // 이 파일은 단계별 OpenCV 처리 결과 데이터를 담는 클래스입니다.

using OpenCvSharp; // OpenCV Mat 이미지 객체를 사용하기 위해 필요합니다.
using System; // IDisposable 인터페이스를 사용하기 위해 필요합니다.

namespace ProductEdgeViewerWpf.Services; // 서비스 관련 클래스가 속한 네임스페이스입니다.

public sealed class EdgeDetectionResult : IDisposable // 처리 결과 Mat들을 안전하게 해제할 수 있는 결과 클래스입니다.
{
    public Mat Original { get; } // 원본 이미지를 저장하는 속성입니다.
    public Mat Gray { get; } // 흑백 변환 결과 이미지를 저장하는 속성입니다.
    public Mat Blurred { get; } // 블러 처리 결과 이미지를 저장하는 속성입니다.
    public Mat Edges { get; } // Canny 엣지 결과 이미지를 저장하는 속성입니다.
    public Mat Visualized { get; } // 최종 윤곽선 시각화 결과 이미지를 저장하는 속성입니다.
    public int ContourCount { get; } // 검출된 윤곽선 개수를 저장하는 속성입니다.

    public EdgeDetectionResult(Mat original, Mat gray, Mat blurred, Mat edges, Mat visualized, int contourCount) // 결과 객체를 생성하는 생성자입니다.
    {
        Original = original; // 전달받은 원본 Mat을 저장합니다.
        Gray = gray; // 전달받은 흑백 Mat을 저장합니다.
        Blurred = blurred; // 전달받은 블러 Mat을 저장합니다.
        Edges = edges; // 전달받은 엣지 Mat을 저장합니다.
        Visualized = visualized; // 전달받은 최종 결과 Mat을 저장합니다.
        ContourCount = contourCount; // 전달받은 윤곽선 개수를 저장합니다.
    }

    public void Dispose() // 결과 객체 사용 후 내부 Mat 리소스를 해제하는 함수입니다.
    {
        Original.Dispose(); // 원본 Mat을 해제합니다.
        Gray.Dispose(); // 흑백 Mat을 해제합니다.
        Blurred.Dispose(); // 블러 Mat을 해제합니다.
        Edges.Dispose(); // 엣지 Mat을 해제합니다.
        Visualized.Dispose(); // 최종 결과 Mat을 해제합니다.
    }
}
```

---

# 파일 6/7 — `ProductEdgeViewerWpf/Services/EdgeDetectionService.cs`

## 무엇을 위한 파일인가?

이 파일은 **OpenCV 알고리즘 처리 핵심**을 담당합니다.

이 파일은 길기 때문에 **1/2, 2/2**로 나누어 제공합니다.

## 1/2

```csharp
// ProductEdgeViewerWpf/Services/EdgeDetectionService.cs // 이 파일은 OpenCV 기반 단계별 경계 검출 알고리즘을 담당합니다.

using OpenCvSharp; // OpenCV의 Mat, Canny, FindContours, DrawContours 같은 이미지 처리 기능을 사용합니다.
using System; // Exception을 사용하기 위해 필요합니다.
using System.Collections.Generic; // List 자료구조를 사용하기 위해 필요합니다.
using System.Linq; // LINQ 기반 필터링을 사용하기 위해 필요합니다.

namespace ProductEdgeViewerWpf.Services; // 서비스 클래스가 속한 네임스페이스입니다.

public sealed class EdgeDetectionService // 사진을 입력받아 단계별 처리 결과를 반환하는 서비스 클래스입니다.
{
    public EdgeDetectionResult DetectEdges(string filePath) // 이미지 파일 경로를 받아 단계별 처리 결과를 반환하는 함수입니다.
    {
        var original = Cv2.ImRead(filePath, ImreadModes.Color); // OpenCV로 원본 컬러 이미지를 읽어옵니다.

        if (original.Empty()) // 이미지가 정상적으로 읽히지 않았는지 확인합니다.
        {
            original.Dispose(); // 비어 있는 Mat도 안전하게 해제합니다.
            throw new Exception("이미지를 읽을 수 없습니다."); // 호출한 쪽에서 처리할 수 있도록 예외를 발생시킵니다.
        }

        var gray = new Mat(); // 흑백 변환 결과를 저장할 Mat 객체를 생성합니다.
        var blurred = new Mat(); // 블러 처리 결과를 저장할 Mat 객체를 생성합니다.
        var edges = new Mat(); // Canny 엣지 결과를 저장할 Mat 객체를 생성합니다.
        var closed = new Mat(); // Morphology Close 처리 결과를 저장할 Mat 객체를 생성합니다.
        var visualized = original.Clone(); // 최종 윤곽선을 원본 위에 그리기 위해 복사본을 만듭니다.

        try // 중간 처리 중 오류가 발생해도 자원을 정리하기 위해 try 블록을 사용합니다.
        {
            Cv2.CvtColor(original, gray, ColorConversionCodes.BGR2GRAY); // 컬러 이미지를 흑백 이미지로 변환합니다.
            Cv2.GaussianBlur(gray, blurred, new OpenCvSharp.Size(5, 5), 1.5); // 작은 노이즈를 줄이기 위해 가우시안 블러를 적용합니다.
            Cv2.Canny(blurred, edges, 50, 150); // Canny 알고리즘으로 밝기 변화가 큰 위치를 경계 후보로 추출합니다.

            using var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3)); // 끊어진 선을 조금 연결하기 위한 3x3 사각 커널을 생성합니다.
            Cv2.MorphologyEx(edges, closed, MorphTypes.Close, kernel); // 가까운 엣지 조각들을 연결하기 위해 Close 연산을 수행합니다.

            using var contourInput = closed.Clone(); // FindContours가 입력 이미지를 수정할 수 있으므로 복사본을 사용합니다.
```

## 2/2

```csharp
            Cv2.FindContours(
                contourInput, // 윤곽선을 찾을 입력 이미지입니다.
                out OpenCvSharp.Point[][] contours, // 검출된 윤곽선 좌표 목록을 저장합니다.
                out HierarchyIndex[] hierarchy, // 윤곽선 계층 정보를 저장합니다.
                RetrievalModes.External, // 가장 바깥쪽 윤곽선을 중심으로 찾습니다.
                ContourApproximationModes.ApproxSimple // 윤곽선 좌표를 단순화합니다.
            );

            List<OpenCvSharp.Point[]> filteredContours = contours // 전체 윤곽선을 대상으로 합니다.
                .Where(contour => Cv2.ContourArea(contour) > 300) // 면적이 너무 작은 윤곽선은 잡음으로 보고 제외합니다.
                .ToList(); // 필터링 결과를 List 형태로 변환합니다.

            Cv2.DrawContours(visualized, filteredContours, -1, Scalar.LimeGreen, 3); // 필터링된 윤곽선을 원본 복사 이미지 위에 초록색으로 그립니다.

            return new EdgeDetectionResult(
                original.Clone(), // 원본 이미지를 반환용으로 복사합니다.
                gray.Clone(), // 흑백 이미지를 반환용으로 복사합니다.
                blurred.Clone(), // 블러 이미지를 반환용으로 복사합니다.
                edges.Clone(), // 엣지 이미지를 반환용으로 복사합니다.
                visualized.Clone(), // 최종 결과 이미지를 반환용으로 복사합니다.
                filteredContours.Count // 검출된 윤곽선 개수를 반환합니다.
            );
        }
        catch // 처리 중 오류가 발생했을 때 실행됩니다.
        {
            throw; // 예외를 호출한 쪽으로 다시 전달합니다.
        }
        finally // 함수가 끝나면 중간 Mat 리소스를 반드시 해제합니다.
        {
            original.Dispose(); // 원본 Mat을 해제합니다.
            gray.Dispose(); // 흑백 Mat을 해제합니다.
            blurred.Dispose(); // 블러 Mat을 해제합니다.
            edges.Dispose(); // 엣지 Mat을 해제합니다.
            closed.Dispose(); // Close 연산 Mat을 해제합니다.
            visualized.Dispose(); // 최종 시각화용 Mat을 해제합니다.
        }
    }
}
```

---

# 파일 7/7 — `ProductEdgeViewerWpf/Utils/MatBitmapConverter.cs`

## 무엇을 위한 파일인가?

이 파일은 **OpenCV Mat 이미지를 WPF Image가 표시 가능한 BitmapSource로 변환**합니다.

```csharp
// ProductEdgeViewerWpf/Utils/MatBitmapConverter.cs // 이 파일은 OpenCV Mat 이미지를 WPF BitmapSource로 변환하는 기능을 담당합니다.

using OpenCvSharp; // OpenCV Mat 이미지와 이미지 인코딩 기능을 사용합니다.
using System.IO; // 바이트 배열을 메모리 스트림으로 읽기 위해 MemoryStream을 사용합니다.
using System.Windows.Media.Imaging; // WPF Image 컨트롤에 표시할 BitmapSource를 사용합니다.

namespace ProductEdgeViewerWpf.Utils; // 유틸리티 클래스가 속한 네임스페이스입니다.

public static class MatBitmapConverter // OpenCV Mat을 WPF BitmapSource로 변환하는 정적 유틸리티 클래스입니다.
{
    public static BitmapSource ToBitmapSource(Mat mat) // OpenCV Mat 객체를 WPF BitmapSource 객체로 변환하는 함수입니다.
    {
        Cv2.ImEncode(".png", mat, out byte[] bytes); // Mat 이미지를 PNG 형식의 바이트 배열로 인코딩합니다.
        using var stream = new MemoryStream(bytes); // PNG 바이트 배열을 읽기 위한 메모리 스트림을 생성합니다.

        var bitmap = new BitmapImage(); // WPF에서 표시 가능한 BitmapImage 객체를 생성합니다.
        bitmap.BeginInit(); // BitmapImage 초기화를 시작합니다.
        bitmap.CacheOption = BitmapCacheOption.OnLoad; // 스트림이 닫혀도 이미지를 유지하도록 즉시 로드 옵션을 설정합니다.
        bitmap.StreamSource = stream; // BitmapImage가 읽을 이미지 데이터를 메모리 스트림으로 지정합니다.
        bitmap.EndInit(); // BitmapImage 초기화를 완료합니다.
        bitmap.Freeze(); // UI 스레드 안정성을 위해 이미지를 불변 객체로 만듭니다.

        return bitmap; // 변환된 WPF BitmapSource 이미지를 반환합니다.
    }
}
```

---

# 9. 실행 방법

## 9-1. Visual Studio에서 실행

아래 위치에서 실행합니다.

```text
Visual Studio 상단 메뉴
→ 디버그
→ 디버그 시작
```

또는 단축키:

```text
F5
```

## 9-2. 실행 후 테스트 순서

```text
1. 프로그램 실행
2. "사진 열기" 버튼 클릭
3. 제품 사진 선택
4. 원본 / 흑백 / 블러 / 엣지 / 최종 결과 확인
```

---

# 10. 내부적으로 어떤 식으로 변환되어 최종 결과까지 가는가?

이 부분이 수업에서 가장 중요합니다.

아래처럼 설명하면 좋습니다.

---

## 10-1. 원본 이미지

사용자가 보는 일반적인 사진입니다.

예:

- 검은 브라켓 사진
- 원형 금속 부품 사진
- 고양이 사진

하지만 컴퓨터는 이 사진을 “고양이”, “제품”으로 이해하지 않습니다.

컴퓨터는 이것을 **픽셀 숫자의 배열**로 봅니다.

---

## 10-2. 흑백 변환

OpenCV는 먼저 컬러 이미지를 흑백 이미지로 바꿉니다.

왜냐하면 경계 검출에서 가장 중요한 것은 보통 **색상 자체**보다 **밝기 변화**이기 때문입니다.

즉,

```text
컬러 이미지 → 밝기 중심 이미지
```

예를 들어:

- 흰색 = 밝기값 큼
- 검정 = 밝기값 작음

제품과 배경이 만나는 부분에서 밝기 차이가 크면 경계 후보가 됩니다.

---

## 10-3. 가우시안 블러

흑백으로 바꾼 뒤에는 작은 노이즈를 줄이기 위해 이미지를 살짝 흐리게 만듭니다.

왜 필요한가?

작은 점, 미세한 결, 센서 잡음까지 전부 경계로 잡히면 결과가 너무 지저분해집니다.

그래서

```text
미세한 잡음 감소
불필요한 세부 변화 완화
더 중요한 큰 경계 강조
```

를 위해 블러를 적용합니다.

---

## 10-4. Canny 엣지 검출

이 단계가 핵심입니다.

Canny는 이미지 안에서 **밝기 변화가 큰 위치**를 찾습니다.

쉽게 말하면,

```text
왼쪽 픽셀과 오른쪽 픽셀의 밝기 차이가 큰가?
위쪽 픽셀과 아래쪽 픽셀의 밝기 차이가 큰가?
```

를 계산해서,
그 차이가 큰 곳을 **엣지(edge)** 로 간주합니다.

즉,

```text
엣지 = 밝기 변화가 급격한 위치
```

이 단계 결과는 보통:

- 검은 배경
- 흰색 선

형태로 보입니다.

---

## 10-5. Morphology Close

Canny 결과는 선이 끊어져 있는 경우가 많습니다.

그래서 `Close` 연산을 사용해,
가까운 선 조각끼리 조금 더 잘 이어지게 합니다.

간단히 말해:

```text
작은 틈을 메우고
끊어진 경계선을 조금 연결한다.
```

---

## 10-6. FindContours

이제 연결된 선들을 따라가며 윤곽선을 찾습니다.

여기서 중요한 개념은:

- **Edge** = 경계 후보 선
- **Contour** = 연결된 경계선을 따라 만든 윤곽선

즉,

```text
Canny는 선을 찾고,
FindContours는 그 선을 따라 도형처럼 묶는다.
```

라고 이해하면 됩니다.

---

## 10-7. 작은 윤곽선 제거

실제로는 아주 작은 잡음 윤곽선도 많이 검출됩니다.

그래서 아래 조건으로 작은 윤곽선을 제거합니다.

```csharp
.Where(contour => Cv2.ContourArea(contour) > 300)
```

이 뜻은:

```text
면적이 300보다 작은 윤곽선은 잡음일 가능성이 높으니 버린다.
```

입니다.

---

## 10-8. 최종 시각화

마지막으로 남은 윤곽선을 원본 이미지 위에 초록색으로 그립니다.

즉,

```text
원본 이미지 + 검출된 윤곽선 = 최종 결과
```

입니다.

---

# 11. 왜 어떤 사진은 잘 되고, 어떤 사진은 안 되는가?

## 잘 되는 사진 예

- 흰 배경 + 검은 제품
- 금속 링처럼 경계가 뚜렷한 부품
- 그림자와 배경이 단순한 이미지

## 잘 안 되는 사진 예

- 털이 많은 동물 사진
- 배경이 복잡한 이미지
- 반사가 심한 금속 이미지
- 경계가 흐린 물체

예를 들어 고양이 사진은:

- 털이 부드럽고 경계가 흐림
- 밝은 털과 밝은 영역이 섞임
- 닫힌 큰 윤곽선으로 연결되기 어려움

그래서 최종적으로 윤곽선 0개가 나올 수도 있습니다.

이것은 오류가 아니라,
**현재 알고리즘이 밝기 변화 기반 전통적인 경계 검출이기 때문**입니다.

---

# 12. 수업에서 쉽게 설명하는 문장 예시

아래 문장은 실제 수업에서 그대로 설명하기 좋습니다.

```text
이 프로그램은 AI가 제품이나 고양이를 이해하는 프로그램이 아닙니다.

OpenCV는 이미지를 픽셀 숫자의 배열로 보고,
밝기값이 급격히 변하는 위치를 경계 후보로 찾습니다.

먼저 컬러 이미지를 흑백으로 바꾸고,
노이즈를 줄이기 위해 블러를 적용한 뒤,
Canny Edge Detection으로 밝기 변화가 큰 선을 추출합니다.

그 다음 Close 연산으로 끊어진 선을 조금 연결하고,
FindContours로 연결된 선을 윤곽선으로 묶습니다.

마지막으로 남은 윤곽선을 원본 이미지 위에 초록색으로 그려서
최종 결과를 시각화합니다.
```

---

# 13. 나중에 추가하면 좋은 기능

현재 프로젝트가 안정적으로 실행되면,
아래 기능을 추가해도 학습 효과가 좋습니다.

## 13-1. Canny 임계값 슬라이더

예:

- 낮은 임계값
- 높은 임계값

사용자가 조절하면,
엣지가 많이 잡히는지 / 적게 잡히는지 바로 볼 수 있습니다.

## 13-2. 윤곽선 면적 필터 슬라이더

예:

- 최소 면적 300
- 최소 면적 1000
- 최소 면적 5000

작은 잡음 윤곽선이 얼마나 제거되는지 바로 이해할 수 있습니다.

## 13-3. Bounding Box 추가

윤곽선뿐 아니라,
검출된 영역을 사각형으로 감싸서 보여줄 수 있습니다.

## 13-4. 제품용 / 동물용 비교 실습

- 제품 사진 → 잘 검출
- 고양이 사진 → 잘 안 검출

이 비교 자체가 전통적 비전과 AI 비전의 차이를 설명하는 좋은 예제가 됩니다.

---

# 14. 자주 발생하는 문제

## 14-1. `Cv2` 를 못 찾는 오류

원인:

- OpenCvSharp4 패키지가 설치되지 않음
- OpenCvSharp4.runtime.win 패키지가 설치되지 않음
- 기본 프로젝트가 다른 프로젝트로 선택되어 설치됨

해결:

```powershell
Install-Package OpenCvSharp4
Install-Package OpenCvSharp4.runtime.win
```

## 14-2. 이미지가 안 뜨는 경우

원인:

- `MatBitmapConverter.cs` 누락
- `ImgOriginal`, `ImgGray`, `ImgBlurred`, `ImgEdges`, `ImgResult` 이름 오타
- 파일 선택은 했지만 이미지 로딩 실패

## 14-3. 윤곽선이 너무 많거나 적은 경우

해결 대상:

```csharp
Cv2.Canny(blurred, edges, 50, 150);
```

또는

```csharp
.Where(contour => Cv2.ContourArea(contour) > 300)
```

이 값을 조정해 봅니다.

---

# 15. 마지막 정리

이 튜토리얼의 핵심은 아래 3가지입니다.

## 첫째

이 프로그램은 **제품을 이해하는 AI가 아니라, 밝기 변화 기반 OpenCV 경계 검출 학습 도구**입니다.

## 둘째

최종 결과만 보는 것이 아니라,
아래 **중간 단계 전체를 시각화**해서 학생이 내부 과정을 이해할 수 있도록 만드는 것이 중요합니다.

```text
원본
→ 흑백 변환
→ 블러
→ Canny 엣지
→ 최종 윤곽선 결과
```

## 셋째

WPF를 사용하면 이러한 학습용 UI를 WinForms보다 훨씬 안정적으로 구성할 수 있습니다.

---

# 16. 실행 체크리스트

실행 전에 아래를 체크하세요.

```text
[ ] WPF 앱 프로젝트를 만들었다.
[ ] 프로젝트 이름은 ProductEdgeViewerWpf 이다.
[ ] OpenCvSharp4 패키지를 설치했다.
[ ] OpenCvSharp4.runtime.win 패키지를 설치했다.
[ ] Services 폴더를 만들었다.
[ ] Utils 폴더를 만들었다.
[ ] 각 파일에 코드를 정확히 넣었다.
[ ] F5로 실행했다.
[ ] 사진 열기를 눌러 테스트했다.
```

---

# 17. 추천 실습 순서

학생과 함께 진행한다면 아래 순서를 추천합니다.

```text
1. 원본 사진만 띄우기
2. 흑백 변환 결과 띄우기
3. 블러 결과 띄우기
4. Canny 엣지 결과 띄우기
5. 최종 윤곽선 결과 띄우기
6. 왜 고양이는 잘 안 잡히는지 비교 설명하기
7. 금속 제품은 왜 잘 잡히는지 비교 설명하기
```

이 순서로 가면 학생들이 “OpenCV가 무슨 일을 하는지”를 훨씬 쉽게 이해합니다.

---

필요하다면 다음 단계로 이어서 아래도 확장할 수 있습니다.

- **슬라이더 추가 버전**
- **Bounding Box 추가 버전**
- **가장 큰 윤곽선 하나만 강조하는 버전**
- **불량 검사용 형태로 확장한 버전**
- **`.sln` 기준 전체 파일 묶음 설명 버전**

