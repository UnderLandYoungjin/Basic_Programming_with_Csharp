# C# WPF + OpenCvSharp Canny 임계값 슬라이더 최종 수정 완성 튜토리얼

> 이 문서는 최종 수정 사항까지 반영한 **WPF + OpenCvSharp + Canny Edge Detection 슬라이더 학습 도구** 완성본입니다.  
> 그대로 따라 하면 다음 기능이 동작합니다.
>
> - 사진 1장 불러오기
> - 원본 이미지 표시
> - 흑백 변환 결과 표시
> - Gaussian Blur 결과 표시
> - Canny Edge Detection 결과 표시
> - Morphology Close 결과 표시
> - 최종 윤곽선 시각화 결과 표시
> - Canny 낮은 임계값 슬라이더
> - Canny 높은 임계값 슬라이더
> - 최소 윤곽선 면적 슬라이더
> - 윤곽선 두께 슬라이더
> - 슬라이더 변경 시 자동 재처리
> - WPF 초기화 중 `NullReferenceException` 방지
> - 기존 코드와 슬라이더 버전 코드가 섞일 때 생기는 오류 방지

---

# 1. 최종 프로젝트 개요

이 프로젝트는 C# WPF에서 OpenCVSharp를 사용하여 이미지의 경계를 찾는 학습용 프로그램입니다.

핵심은 단순히 최종 결과만 보여주는 것이 아니라, 이미지가 내부적으로 어떤 순서로 변환되는지 단계별로 보여주는 것입니다.

```text
원본 이미지
→ 흑백 변환
→ 가우시안 블러
→ Canny 엣지 검출
→ Morphology Close
→ 윤곽선 검출
→ 최종 결과 시각화
```

---

# 2. 이번 최종 수정에서 해결한 문제

이전 버전에서 발생했던 문제는 크게 2가지였습니다.

## 2-1. 기존 WPF 버전과 슬라이더 버전 코드가 섞인 문제

오류 예시:

```text
EdgeDetectionService에는 DetectEdges에 대한 정의가 포함되어 있지 않습니다.
MatBitmapConverter에는 ToBitmapSource에 대한 정의가 포함되어 있지 않습니다.
```

이 오류는 아래 3개 파일의 버전이 서로 맞지 않을 때 발생합니다.

```text
MainWindow.xaml.cs
EdgeDetectionService.cs
MatBitmapConverter.cs
```

최종 버전에서는 다음 형태로 통일합니다.

```text
MainWindow.xaml.cs
→ edgeService.DetectEdges(currentFilePath, options)
→ MatBitmapConverter.ToBitmapSource(...)

EdgeDetectionService.cs
→ DetectEdges(string filePath, EdgeDetectionOptions options)

MatBitmapConverter.cs
→ ToBitmapSource(Mat mat)
```

## 2-2. WPF 초기화 중 Slider 이벤트가 먼저 실행되는 문제

오류 예시:

```text
System.NullReferenceException: Object reference not set to an instance of an object.
```

발생 위치 예시:

```csharp
TxtCannyHigh.Text = ((int)SliderCannyHigh.Value).ToString();
```

이유는 WPF가 XAML을 초기화하는 도중에 `Slider.ValueChanged` 이벤트를 먼저 실행할 수 있기 때문입니다.  
그 시점에는 `TxtCannyHigh`, `TxtMinArea`, `TxtThickness` 같은 컨트롤이 아직 생성되지 않았을 수 있습니다.

최종 버전에서는 아래 방식으로 해결합니다.

```csharp
private bool isWindowReady = false;
```

그리고 슬라이더 이벤트에서 아래처럼 방어합니다.

```csharp
if (!isWindowReady)
{
    return;
}
```

또한 모든 슬라이더와 텍스트 컨트롤이 실제로 생성되었는지 확인하는 함수도 추가합니다.

```csharp
private bool AreSliderControlsReady()
```

---

# 3. 개발 환경

## 3-1. 사용 도구

```text
Visual Studio 2022 이상 권장
.NET 8.0 WPF 앱 권장
Windows 환경
```

## 3-2. 사용하는 NuGet 패키지

```text
OpenCvSharp4
OpenCvSharp4.runtime.win
```

---

# 4. 프로젝트 생성 방법

Visual Studio에서 아래 순서로 진행합니다.

```text
1. Visual Studio 실행
2. 새 프로젝트 만들기 클릭
3. WPF 앱 선택
4. 프로젝트 이름 입력: ProductEdgeViewerWpfSlider
5. 프레임워크 선택: .NET 8.0 권장
6. 만들기 클릭
```

---

# 5. OpenCvSharp 설치 방법

Visual Studio에서 아래 메뉴로 이동합니다.

```text
도구
→ NuGet 패키지 관리자
→ 패키지 관리자 콘솔
```

패키지 관리자 콘솔에서 기본 프로젝트가 아래처럼 되어 있는지 확인합니다.

```text
기본 프로젝트: ProductEdgeViewerWpfSlider
```

그 다음 아래 명령어를 실행합니다.

```powershell
Install-Package OpenCvSharp4
Install-Package OpenCvSharp4.runtime.win
```

---

# 6. 왜 이 라이브러리를 사용하는가?

## 6-1. OpenCvSharp4

C#에서 OpenCV 기능을 사용할 수 있게 해주는 래퍼 라이브러리입니다.

이번 프로젝트에서는 아래 함수들을 사용합니다.

```text
Cv2.ImRead()
Cv2.CvtColor()
Cv2.GaussianBlur()
Cv2.Canny()
Cv2.MorphologyEx()
Cv2.FindContours()
Cv2.DrawContours()
Cv2.ImEncode()
```

## 6-2. OpenCvSharp4.runtime.win

Windows에서 실제 OpenCV 네이티브 DLL이 실행되도록 해주는 런타임 패키지입니다.

정리하면 다음과 같습니다.

```text
OpenCvSharp4 = C#에서 OpenCV 함수를 호출하기 위한 패키지
OpenCvSharp4.runtime.win = Windows에서 OpenCV가 실제 실행되기 위한 네이티브 런타임
```

둘 다 설치해야 합니다.

---

# 7. 최종 폴더 구조

아래 구조로 맞춥니다.

```text
ProductEdgeViewerWpfSlider
 ├─ App.xaml
 ├─ App.xaml.cs
 ├─ MainWindow.xaml
 ├─ MainWindow.xaml.cs
 ├─ Models
 │   └─ EdgeDetectionOptions.cs
 ├─ Services
 │   ├─ EdgeDetectionResult.cs
 │   └─ EdgeDetectionService.cs
 └─ Utils
     └─ MatBitmapConverter.cs
```

---

# 8. 폴더 생성 방법

Visual Studio의 솔루션 탐색기에서 다음처럼 만듭니다.

```text
프로젝트 우클릭
→ 추가
→ 새 폴더
→ Models
```

```text
프로젝트 우클릭
→ 추가
→ 새 폴더
→ Services
```

```text
프로젝트 우클릭
→ 추가
→ 새 폴더
→ Utils
```

각 폴더에 클래스를 추가합니다.

```text
Models 폴더 우클릭
→ 추가
→ 클래스
→ EdgeDetectionOptions.cs
```

```text
Services 폴더 우클릭
→ 추가
→ 클래스
→ EdgeDetectionResult.cs
```

```text
Services 폴더 우클릭
→ 추가
→ 클래스
→ EdgeDetectionService.cs
```

```text
Utils 폴더 우클릭
→ 추가
→ 클래스
→ MatBitmapConverter.cs
```

---

# 9. 전체 코드

아래 코드를 각 파일에 그대로 넣으면 됩니다.

---

# 파일 1/8 — `ProductEdgeViewerWpfSlider/App.xaml`

## 무엇을 위한 파일인가?

이 파일은 WPF 프로그램의 시작 창을 지정합니다.

```xml
<!-- ProductEdgeViewerWpfSlider/App.xaml -->
<!-- 이 파일은 WPF 애플리케이션의 시작 설정을 담당합니다. -->

<Application x:Class="ProductEdgeViewerWpfSlider.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="MainWindow.xaml">
    <!-- StartupUri는 프로그램 실행 시 처음 열릴 창을 지정합니다. -->

    <Application.Resources>
        <!-- 전역 스타일이나 리소스가 필요하면 이곳에 추가합니다. -->
    </Application.Resources>
</Application>
```

---

# 파일 2/8 — `ProductEdgeViewerWpfSlider/App.xaml.cs`

## 무엇을 위한 파일인가?

이 파일은 WPF 애플리케이션의 기본 App 클래스를 담당합니다.

```csharp
// ProductEdgeViewerWpfSlider/App.xaml.cs // 이 파일은 WPF 애플리케이션의 App 클래스를 담당합니다.

using System.Windows; // WPF Application 클래스를 사용하기 위해 필요한 네임스페이스입니다.

namespace ProductEdgeViewerWpfSlider; // 현재 프로젝트의 네임스페이스입니다.

public partial class App : Application // App.xaml과 연결되는 WPF 애플리케이션 클래스입니다.
{
    // 현재는 별도의 시작 로직이 필요하지 않으므로 비워 둡니다.
}
```

---

# 파일 3/8 — `ProductEdgeViewerWpfSlider/Models/EdgeDetectionOptions.cs`

## 무엇을 위한 파일인가?

이 파일은 슬라이더에서 조절할 Canny 임계값, 최소 윤곽선 면적, 윤곽선 두께를 담는 모델입니다.

```csharp
// ProductEdgeViewerWpfSlider/Models/EdgeDetectionOptions.cs // 이 파일은 경계 검출 옵션 값을 담는 모델 클래스입니다.

namespace ProductEdgeViewerWpfSlider.Models; // 옵션 모델이 속한 네임스페이스입니다.

public sealed class EdgeDetectionOptions // 경계 검출에 사용할 설정값을 담는 클래스입니다.
{
    public double CannyLowThreshold { get; set; } = 50; // Canny 낮은 임계값입니다.
    public double CannyHighThreshold { get; set; } = 150; // Canny 높은 임계값입니다.
    public double MinContourArea { get; set; } = 300; // 잡음 제거를 위한 최소 윤곽선 면적입니다.
    public int ContourThickness { get; set; } = 3; // 최종 결과 이미지에 그릴 윤곽선 두께입니다.
}
```

---

# 파일 4/8 — `ProductEdgeViewerWpfSlider/MainWindow.xaml`

## 무엇을 위한 파일인가?

이 파일은 프로그램의 전체 화면 UI를 담당합니다.

- 사진 열기 버튼
- 상태 표시 영역
- Canny 낮은 임계값 슬라이더
- Canny 높은 임계값 슬라이더
- 최소 윤곽선 면적 슬라이더
- 윤곽선 두께 슬라이더
- 6단계 이미지 표시 영역

## 1/4

```xml
<!-- ProductEdgeViewerWpfSlider/MainWindow.xaml -->
<!-- 이 파일은 Canny 임계값 슬라이더가 포함된 WPF 메인 화면 UI를 담당합니다. -->

<Window x:Class="ProductEdgeViewerWpfSlider.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="C# OpenCV Canny Edge Detection 슬라이더 학습 도구"
        Width="1700"
        Height="1000"
        MinWidth="1300"
        MinHeight="850"
        WindowStartupLocation="CenterScreen"
        Background="#F1F5F9"
        FontFamily="Malgun Gothic">

    <Window.Resources>
        <!-- 카드 스타일입니다. -->
        <Style x:Key="CardStyle" TargetType="Border">
            <Setter Property="Background" Value="White"/>
            <Setter Property="BorderBrush" Value="#CBD5E1"/>
            <Setter Property="BorderThickness" Value="1"/>
            <Setter Property="CornerRadius" Value="10"/>
            <Setter Property="Padding" Value="16"/>
        </Style>

        <!-- 이미지 제목 스타일입니다. -->
        <Style x:Key="ImageTitleStyle" TargetType="TextBlock">
            <Setter Property="FontSize" Value="18"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="Foreground" Value="#0F172A"/>
            <Setter Property="HorizontalAlignment" Value="Center"/>
            <Setter Property="Margin" Value="0,0,0,8"/>
        </Style>

        <!-- 이미지 설명 스타일입니다. -->
        <Style x:Key="ImageDescriptionStyle" TargetType="TextBlock">
            <Setter Property="FontSize" Value="12"/>
            <Setter Property="Foreground" Value="#64748B"/>
            <Setter Property="HorizontalAlignment" Value="Center"/>
            <Setter Property="TextAlignment" Value="Center"/>
            <Setter Property="TextWrapping" Value="Wrap"/>
            <Setter Property="Margin" Value="0,0,0,10"/>
        </Style>

        <!-- 설정 항목 제목 스타일입니다. -->
        <Style x:Key="SettingLabelStyle" TargetType="TextBlock">
            <Setter Property="FontSize" Value="13"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="Foreground" Value="#334155"/>
            <Setter Property="VerticalAlignment" Value="Center"/>
        </Style>
    </Window.Resources>
```

## 2/4

```xml
    <Grid Margin="20">
        <!-- 전체 화면을 상단, 설정, 이미지 영역으로 나눕니다. -->
        <Grid.RowDefinitions>
            <RowDefinition Height="100"/>
            <RowDefinition Height="190"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!-- 상단 영역입니다. -->
        <Border Grid.Row="0" Style="{StaticResource CardStyle}">
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

                    <TextBlock Text="슬라이더를 조절하면 Canny 결과와 윤곽선 결과가 자동으로 다시 계산됩니다."
                               FontSize="13"
                               Foreground="#64748B"
                               Margin="0,6,0,0"
                               TextWrapping="Wrap"/>
                </StackPanel>
            </Grid>
        </Border>

        <!-- 설정 영역입니다. -->
        <Border Grid.Row="1" Margin="0,16,0,16" Style="{StaticResource CardStyle}">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>

                <Grid.RowDefinitions>
                    <RowDefinition Height="42"/>
                    <RowDefinition Height="42"/>
                    <RowDefinition Height="42"/>
                    <RowDefinition Height="42"/>
                </Grid.RowDefinitions>

                <!-- Canny 낮은 임계값 -->
                <TextBlock Grid.Row="0" Grid.Column="0" Text="Canny 낮은 임계값" Style="{StaticResource SettingLabelStyle}"/>
                <StackPanel Grid.Row="0" Grid.Column="1" Orientation="Horizontal">
                    <Slider x:Name="SliderCannyLow"
                            Width="420"
                            Minimum="0"
                            Maximum="300"
                            Value="50"
                            TickFrequency="10"
                            IsSnapToTickEnabled="False"
                            ValueChanged="Slider_ValueChanged"/>
                    <TextBlock x:Name="TxtCannyLow"
                               Text="50"
                               Width="70"
                               FontSize="14"
                               FontWeight="Bold"
                               VerticalAlignment="Center"
                               Margin="12,0,0,0"/>
                </StackPanel>
```

## 3/4

```xml
                <!-- Canny 높은 임계값 -->
                <TextBlock Grid.Row="1" Grid.Column="0" Text="Canny 높은 임계값" Style="{StaticResource SettingLabelStyle}"/>
                <StackPanel Grid.Row="1" Grid.Column="1" Orientation="Horizontal">
                    <Slider x:Name="SliderCannyHigh"
                            Width="420"
                            Minimum="1"
                            Maximum="400"
                            Value="150"
                            TickFrequency="10"
                            IsSnapToTickEnabled="False"
                            ValueChanged="Slider_ValueChanged"/>
                    <TextBlock x:Name="TxtCannyHigh"
                               Text="150"
                               Width="70"
                               FontSize="14"
                               FontWeight="Bold"
                               VerticalAlignment="Center"
                               Margin="12,0,0,0"/>
                </StackPanel>

                <!-- 최소 윤곽선 면적 -->
                <TextBlock Grid.Row="2" Grid.Column="0" Text="최소 윤곽선 면적" Style="{StaticResource SettingLabelStyle}"/>
                <StackPanel Grid.Row="2" Grid.Column="1" Orientation="Horizontal">
                    <Slider x:Name="SliderMinArea"
                            Width="420"
                            Minimum="0"
                            Maximum="20000"
                            Value="300"
                            TickFrequency="100"
                            IsSnapToTickEnabled="False"
                            ValueChanged="Slider_ValueChanged"/>
                    <TextBlock x:Name="TxtMinArea"
                               Text="300"
                               Width="70"
                               FontSize="14"
                               FontWeight="Bold"
                               VerticalAlignment="Center"
                               Margin="12,0,0,0"/>
                </StackPanel>

                <!-- 윤곽선 두께 -->
                <TextBlock Grid.Row="3" Grid.Column="0" Text="윤곽선 두께" Style="{StaticResource SettingLabelStyle}"/>
                <StackPanel Grid.Row="3" Grid.Column="1" Orientation="Horizontal">
                    <Slider x:Name="SliderThickness"
                            Width="420"
                            Minimum="1"
                            Maximum="10"
                            Value="3"
                            TickFrequency="1"
                            IsSnapToTickEnabled="True"
                            ValueChanged="Slider_ValueChanged"/>
                    <TextBlock x:Name="TxtThickness"
                               Text="3"
                               Width="70"
                               FontSize="14"
                               FontWeight="Bold"
                               VerticalAlignment="Center"
                               Margin="12,0,0,0"/>
                </StackPanel>
            </Grid>
        </Border>
```

## 4/4

```xml
        <!-- 이미지 표시 영역입니다. -->
        <ScrollViewer Grid.Row="2" VerticalScrollBarVisibility="Auto">
            <UniformGrid Columns="3" Rows="2">
                <Border Style="{StaticResource CardStyle}" Margin="8">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="*"/>
                        </Grid.RowDefinitions>
                        <TextBlock Grid.Row="0" Text="1. 원본 사진" Style="{StaticResource ImageTitleStyle}"/>
                        <TextBlock Grid.Row="1" Text="사용자가 선택한 원본 이미지입니다." Style="{StaticResource ImageDescriptionStyle}"/>
                        <Border Grid.Row="2" Background="#F8FAFC" BorderBrush="#CBD5E1" BorderThickness="1" CornerRadius="6">
                            <Image x:Name="ImgOriginal" Stretch="Uniform" Margin="10"/>
                        </Border>
                    </Grid>
                </Border>

                <Border Style="{StaticResource CardStyle}" Margin="8">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="*"/>
                        </Grid.RowDefinitions>
                        <TextBlock Grid.Row="0" Text="2. 흑백 변환" Style="{StaticResource ImageTitleStyle}"/>
                        <TextBlock Grid.Row="1" Text="색상보다 밝기 정보에 집중합니다." Style="{StaticResource ImageDescriptionStyle}"/>
                        <Border Grid.Row="2" Background="#F8FAFC" BorderBrush="#CBD5E1" BorderThickness="1" CornerRadius="6">
                            <Image x:Name="ImgGray" Stretch="Uniform" Margin="10"/>
                        </Border>
                    </Grid>
                </Border>

                <Border Style="{StaticResource CardStyle}" Margin="8">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="*"/>
                        </Grid.RowDefinitions>
                        <TextBlock Grid.Row="0" Text="3. 가우시안 블러" Style="{StaticResource ImageTitleStyle}"/>
                        <TextBlock Grid.Row="1" Text="작은 노이즈와 미세한 변화를 줄입니다." Style="{StaticResource ImageDescriptionStyle}"/>
                        <Border Grid.Row="2" Background="#F8FAFC" BorderBrush="#CBD5E1" BorderThickness="1" CornerRadius="6">
                            <Image x:Name="ImgBlurred" Stretch="Uniform" Margin="10"/>
                        </Border>
                    </Grid>
                </Border>

                <Border Style="{StaticResource CardStyle}" Margin="8">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="*"/>
                        </Grid.RowDefinitions>
                        <TextBlock Grid.Row="0" Text="4. Canny 엣지" Style="{StaticResource ImageTitleStyle}"/>
                        <TextBlock Grid.Row="1" Text="밝기 변화가 큰 부분을 흰 선으로 표시합니다." Style="{StaticResource ImageDescriptionStyle}"/>
                        <Border Grid.Row="2" Background="#F8FAFC" BorderBrush="#CBD5E1" BorderThickness="1" CornerRadius="6">
                            <Image x:Name="ImgEdges" Stretch="Uniform" Margin="10"/>
                        </Border>
                    </Grid>
                </Border>

                <Border Style="{StaticResource CardStyle}" Margin="8">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="*"/>
                        </Grid.RowDefinitions>
                        <TextBlock Grid.Row="0" Text="5. Close 결과" Style="{StaticResource ImageTitleStyle}"/>
                        <TextBlock Grid.Row="1" Text="끊어진 선 조각을 조금 연결한 결과입니다." Style="{StaticResource ImageDescriptionStyle}"/>
                        <Border Grid.Row="2" Background="#F8FAFC" BorderBrush="#CBD5E1" BorderThickness="1" CornerRadius="6">
                            <Image x:Name="ImgClosed" Stretch="Uniform" Margin="10"/>
                        </Border>
                    </Grid>
                </Border>

                <Border Style="{StaticResource CardStyle}" Margin="8">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="*"/>
                        </Grid.RowDefinitions>
                        <TextBlock Grid.Row="0" Text="6. 최종 윤곽선" Style="{StaticResource ImageTitleStyle}"/>
                        <TextBlock Grid.Row="1" Text="필터링된 윤곽선을 원본 위에 초록색으로 표시합니다." Style="{StaticResource ImageDescriptionStyle}"/>
                        <Border Grid.Row="2" Background="#F8FAFC" BorderBrush="#CBD5E1" BorderThickness="1" CornerRadius="6">
                            <Image x:Name="ImgResult" Stretch="Uniform" Margin="10"/>
                        </Border>
                    </Grid>
                </Border>
            </UniformGrid>
        </ScrollViewer>
    </Grid>
</Window>
```

---

# 파일 5/8 — `ProductEdgeViewerWpfSlider/MainWindow.xaml.cs`

## 무엇을 위한 파일인가?

이 파일은 다음 일을 담당합니다.

- 사진 열기 버튼 클릭 처리
- 슬라이더 값 변경 처리
- WPF 초기화 중 NullReferenceException 방지
- 현재 슬라이더 값으로 `EdgeDetectionOptions` 생성
- OpenCV 서비스 호출
- 결과 이미지를 화면에 표시

## 1/3

```csharp
// ProductEdgeViewerWpfSlider/MainWindow.xaml.cs // 이 파일은 버튼 이벤트, 슬라이더 이벤트, 화면 표시 로직을 담당합니다.

using Microsoft.Win32; // WPF 파일 선택 창인 OpenFileDialog를 사용하기 위해 필요합니다.
using ProductEdgeViewerWpfSlider.Models; // Canny 임계값 옵션 모델을 사용합니다.
using ProductEdgeViewerWpfSlider.Services; // OpenCV 경계 검출 서비스를 사용합니다.
using ProductEdgeViewerWpfSlider.Utils; // OpenCV Mat을 WPF 이미지로 변환하는 유틸리티를 사용합니다.
using System; // Exception 같은 기본 기능을 사용합니다.
using System.IO; // 파일명 추출을 위해 Path 기능을 사용합니다.
using System.Windows; // WPF Window와 MessageBox를 사용합니다;

namespace ProductEdgeViewerWpfSlider; // 현재 프로젝트의 네임스페이스입니다.

public partial class MainWindow : Window // MainWindow.xaml과 연결되는 WPF 메인 창 클래스입니다.
{
    private readonly EdgeDetectionService edgeService = new(); // OpenCV 경계 검출 로직을 담당하는 서비스 객체입니다.
    private string? currentFilePath; // 현재 불러온 이미지 파일 경로를 저장합니다.
    private bool isWindowReady = false; // WPF 컨트롤 초기화가 끝났는지 확인하는 플래그입니다.

    public MainWindow() // 메인 창이 생성될 때 실행되는 생성자입니다.
    {
        InitializeComponent(); // XAML에 정의된 UI 요소들을 실제 객체로 초기화합니다.
        isWindowReady = true; // 모든 XAML 컨트롤 초기화가 끝났음을 표시합니다.
        UpdateSliderText(); // 프로그램 시작 시 슬라이더 숫자 표시를 초기화합니다.
    }

    private void BtnOpen_Click(object sender, RoutedEventArgs e) // 사진 열기 버튼을 클릭했을 때 실행되는 함수입니다.
    {
        var dialog = new OpenFileDialog(); // WPF 파일 선택 창 객체를 생성합니다.
        dialog.Title = "제품 사진 선택"; // 파일 선택 창 제목을 설정합니다.
        dialog.Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp|모든 파일|*.*"; // 선택 가능한 이미지 확장자를 지정합니다.

        bool? selected = dialog.ShowDialog(); // 파일 선택 창을 열고 사용자의 선택 결과를 받습니다.

        if (selected != true) // 사용자가 파일 선택을 취소했는지 확인합니다.
        {
            return; // 파일 선택이 없으면 아무 작업도 하지 않고 종료합니다.
        }

        currentFilePath = dialog.FileName; // 선택된 파일 경로를 저장합니다.
        ProcessCurrentImage(); // 현재 파일과 현재 슬라이더 값으로 이미지를 처리합니다.
    }

    private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) // 슬라이더 값이 바뀔 때 실행되는 함수입니다.
    {
        if (!isWindowReady) // XAML 초기화가 끝나기 전에 이벤트가 발생했는지 확인합니다.
        {
            return; // 아직 화면 컨트롤이 모두 준비되지 않았으면 아무 작업도 하지 않습니다.
        }

        UpdateSliderText(); // 슬라이더 옆 숫자 표시를 갱신합니다.

        if (currentFilePath is not null) // 이미 선택된 이미지가 있는지 확인합니다.
        {
            ProcessCurrentImage(); // 슬라이더 값 변경에 맞춰 이미지를 다시 처리합니다.
        }
    }
```

## 2/3

```csharp
    private void UpdateSliderText() // 슬라이더 옆 숫자 표시를 갱신하는 함수입니다.
    {
        if (!AreSliderControlsReady()) // 슬라이더와 텍스트 컨트롤이 모두 준비되었는지 확인합니다.
        {
            return; // 하나라도 아직 생성되지 않았으면 종료합니다.
        }

        TxtCannyLow.Text = ((int)SliderCannyLow.Value).ToString(); // 낮은 임계값 표시를 갱신합니다.
        TxtCannyHigh.Text = ((int)SliderCannyHigh.Value).ToString(); // 높은 임계값 표시를 갱신합니다.
        TxtMinArea.Text = ((int)SliderMinArea.Value).ToString(); // 최소 면적 표시를 갱신합니다.
        TxtThickness.Text = ((int)SliderThickness.Value).ToString(); // 윤곽선 두께 표시를 갱신합니다.
    }

    private bool AreSliderControlsReady() // 슬라이더와 텍스트 컨트롤이 모두 생성되었는지 확인하는 함수입니다.
    {
        return SliderCannyLow is not null // 낮은 임계값 슬라이더가 생성되었는지 확인합니다.
            && SliderCannyHigh is not null // 높은 임계값 슬라이더가 생성되었는지 확인합니다.
            && SliderMinArea is not null // 최소 면적 슬라이더가 생성되었는지 확인합니다.
            && SliderThickness is not null // 윤곽선 두께 슬라이더가 생성되었는지 확인합니다.
            && TxtCannyLow is not null // 낮은 임계값 표시 TextBlock이 생성되었는지 확인합니다.
            && TxtCannyHigh is not null // 높은 임계값 표시 TextBlock이 생성되었는지 확인합니다.
            && TxtMinArea is not null // 최소 면적 표시 TextBlock이 생성되었는지 확인합니다.
            && TxtThickness is not null; // 윤곽선 두께 표시 TextBlock이 생성되었는지 확인합니다.
    }

    private EdgeDetectionOptions GetCurrentOptions() // 현재 슬라이더 값으로 경계 검출 옵션 객체를 만드는 함수입니다.
    {
        double low = SliderCannyLow.Value; // 낮은 임계값을 슬라이더에서 읽습니다.
        double high = SliderCannyHigh.Value; // 높은 임계값을 슬라이더에서 읽습니다.

        if (high <= low) // 높은 임계값이 낮은 임계값보다 작거나 같으면 Canny 조건이 이상해집니다.
        {
            high = low + 1; // 높은 임계값을 낮은 임계값보다 최소 1 크게 보정합니다.
            SliderCannyHigh.Value = high; // 보정된 값을 슬라이더에도 반영합니다.
        }

        return new EdgeDetectionOptions // 현재 화면 설정값을 옵션 객체로 만들어 반환합니다.
        {
            CannyLowThreshold = low, // 낮은 임계값을 저장합니다.
            CannyHighThreshold = high, // 높은 임계값을 저장합니다.
            MinContourArea = SliderMinArea.Value, // 최소 윤곽선 면적을 저장합니다.
            ContourThickness = (int)SliderThickness.Value // 윤곽선 두께를 저장합니다.
        };
    }
```

## 3/3

```csharp
    private void ProcessCurrentImage() // 현재 이미지와 현재 옵션으로 OpenCV 처리를 실행하는 함수입니다.
    {
        if (currentFilePath is null) // 아직 이미지가 선택되지 않았는지 확인합니다.
        {
            return; // 이미지가 없으면 처리하지 않습니다.
        }

        try // 이미지 처리 중 오류가 발생할 수 있으므로 예외 처리를 시작합니다.
        {
            EdgeDetectionOptions options = GetCurrentOptions(); // 현재 슬라이더 값으로 옵션을 생성합니다.
            using EdgeDetectionResult result = edgeService.DetectEdges(currentFilePath, options); // OpenCV 처리 결과를 가져옵니다.

            ImgOriginal.Source = MatBitmapConverter.ToBitmapSource(result.Original); // 원본 이미지를 화면에 표시합니다.
            ImgGray.Source = MatBitmapConverter.ToBitmapSource(result.Gray); // 흑백 이미지를 화면에 표시합니다.
            ImgBlurred.Source = MatBitmapConverter.ToBitmapSource(result.Blurred); // 블러 이미지를 화면에 표시합니다.
            ImgEdges.Source = MatBitmapConverter.ToBitmapSource(result.Edges); // Canny 엣지 이미지를 화면에 표시합니다.
            ImgClosed.Source = MatBitmapConverter.ToBitmapSource(result.Closed); // Close 결과 이미지를 화면에 표시합니다.
            ImgResult.Source = MatBitmapConverter.ToBitmapSource(result.Visualized); // 최종 윤곽선 이미지를 화면에 표시합니다.

            string fileName = Path.GetFileName(currentFilePath); // 파일 경로에서 파일명만 추출합니다.
            TxtStatus.Text = $"상태: 윤곽선 {result.ContourCount}개 / Low {options.CannyLowThreshold:0} / High {options.CannyHighThreshold:0} / Area {options.MinContourArea:0} / 파일: {fileName}"; // 상태 문구를 갱신합니다.
        }
        catch (Exception ex) // 오류가 발생했을 때 실행됩니다.
        {
            MessageBox.Show(ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error); // 오류 메시지를 표시합니다.
        }
    }
}
```

---

# 파일 6/8 — `ProductEdgeViewerWpfSlider/Services/EdgeDetectionResult.cs`

## 무엇을 위한 파일인가?

이 파일은 OpenCV의 단계별 처리 결과를 담습니다.

```csharp
// ProductEdgeViewerWpfSlider/Services/EdgeDetectionResult.cs // 이 파일은 OpenCV 단계별 처리 결과를 담는 클래스입니다.

using OpenCvSharp; // OpenCV Mat 이미지 객체를 사용합니다.
using System; // IDisposable 인터페이스를 사용합니다.

namespace ProductEdgeViewerWpfSlider.Services; // 서비스 관련 클래스가 속한 네임스페이스입니다.

public sealed class EdgeDetectionResult : IDisposable // 처리 결과 Mat들을 안전하게 해제할 수 있는 결과 클래스입니다.
{
    public Mat Original { get; } // 원본 이미지입니다.
    public Mat Gray { get; } // 흑백 변환 이미지입니다.
    public Mat Blurred { get; } // 블러 처리 이미지입니다.
    public Mat Edges { get; } // Canny 엣지 이미지입니다.
    public Mat Closed { get; } // Morphology Close 결과 이미지입니다.
    public Mat Visualized { get; } // 최종 윤곽선 시각화 이미지입니다.
    public int ContourCount { get; } // 검출된 윤곽선 개수입니다.

    public EdgeDetectionResult(Mat original, Mat gray, Mat blurred, Mat edges, Mat closed, Mat visualized, int contourCount) // 결과 객체 생성자입니다.
    {
        Original = original; // 원본 이미지를 저장합니다.
        Gray = gray; // 흑백 이미지를 저장합니다.
        Blurred = blurred; // 블러 이미지를 저장합니다.
        Edges = edges; // 엣지 이미지를 저장합니다.
        Closed = closed; // Close 이미지를 저장합니다.
        Visualized = visualized; // 최종 결과 이미지를 저장합니다.
        ContourCount = contourCount; // 윤곽선 개수를 저장합니다.
    }

    public void Dispose() // Mat 리소스를 해제하는 함수입니다.
    {
        Original.Dispose(); // 원본 Mat을 해제합니다.
        Gray.Dispose(); // 흑백 Mat을 해제합니다.
        Blurred.Dispose(); // 블러 Mat을 해제합니다.
        Edges.Dispose(); // 엣지 Mat을 해제합니다.
        Closed.Dispose(); // Close Mat을 해제합니다.
        Visualized.Dispose(); // 최종 결과 Mat을 해제합니다.
    }
}
```

---

# 파일 7/8 — `ProductEdgeViewerWpfSlider/Services/EdgeDetectionService.cs`

## 무엇을 위한 파일인가?

이 파일은 OpenCV 경계 검출 알고리즘을 담당합니다.  
슬라이더에서 받은 옵션을 사용합니다.

## 1/2

```csharp
// ProductEdgeViewerWpfSlider/Services/EdgeDetectionService.cs // 이 파일은 OpenCV 경계 검출 알고리즘을 담당합니다.

using OpenCvSharp; // OpenCV의 이미지 처리 기능을 사용합니다.
using ProductEdgeViewerWpfSlider.Models; // 슬라이더 옵션 모델을 사용합니다.
using System; // Exception을 사용합니다.
using System.Collections.Generic; // List 자료구조를 사용합니다.
using System.Linq; // LINQ 필터링을 사용합니다.

namespace ProductEdgeViewerWpfSlider.Services; // 서비스 클래스가 속한 네임스페이스입니다.

public sealed class EdgeDetectionService // 이미지 경계 검출 처리를 담당하는 서비스 클래스입니다.
{
    public EdgeDetectionResult DetectEdges(string filePath, EdgeDetectionOptions options) // 이미지 파일과 옵션을 받아 처리 결과를 반환합니다.
    {
        var original = Cv2.ImRead(filePath, ImreadModes.Color); // 이미지 파일을 컬러 Mat으로 읽습니다.

        if (original.Empty()) // 이미지 읽기에 실패했는지 확인합니다.
        {
            original.Dispose(); // 비어 있는 Mat을 해제합니다.
            throw new Exception("이미지를 읽을 수 없습니다."); // 오류를 호출한 쪽으로 전달합니다.
        }

        var gray = new Mat(); // 흑백 변환 결과를 저장할 Mat입니다.
        var blurred = new Mat(); // 블러 처리 결과를 저장할 Mat입니다.
        var edges = new Mat(); // Canny 엣지 결과를 저장할 Mat입니다.
        var closed = new Mat(); // Morphology Close 결과를 저장할 Mat입니다.
        var visualized = original.Clone(); // 최종 윤곽선을 그릴 원본 복사본입니다.

        try // 중간 처리 중 오류가 발생해도 자원을 정리하기 위해 try를 사용합니다.
        {
            Cv2.CvtColor(original, gray, ColorConversionCodes.BGR2GRAY); // 컬러 이미지를 흑백 이미지로 변환합니다.

            Cv2.GaussianBlur( // 흑백 이미지에 가우시안 블러를 적용합니다.
                gray, // 입력 이미지입니다.
                blurred, // 출력 이미지입니다.
                new OpenCvSharp.Size(5, 5), // 5x5 커널을 사용합니다.
                1.5 // 블러 강도입니다.
            );

            Cv2.Canny( // Canny Edge Detection을 실행합니다.
                blurred, // 블러 처리된 이미지를 입력으로 사용합니다.
                edges, // 엣지 검출 결과를 저장합니다.
                options.CannyLowThreshold, // 슬라이더에서 받은 낮은 임계값입니다.
                options.CannyHighThreshold // 슬라이더에서 받은 높은 임계값입니다.
            );
```

## 2/2

```csharp
            using var kernel = Cv2.GetStructuringElement( // Morphology Close에 사용할 커널을 생성합니다.
                MorphShapes.Rect, // 사각형 커널을 사용합니다.
                new OpenCvSharp.Size(3, 3) // 3x3 크기의 커널입니다.
            );

            Cv2.MorphologyEx( // Canny 엣지 결과에 Close 연산을 적용합니다.
                edges, // 입력 엣지 이미지입니다.
                closed, // Close 결과 이미지입니다.
                MorphTypes.Close, // 닫기 연산입니다.
                kernel // 위에서 만든 커널입니다.
            );

            using var contourInput = closed.Clone(); // FindContours가 입력 이미지를 수정할 수 있으므로 복사본을 사용합니다.

            Cv2.FindContours( // 연결된 윤곽선을 찾습니다.
                contourInput, // 윤곽선을 찾을 입력 이미지입니다.
                out OpenCvSharp.Point[][] contours, // 검출된 윤곽선 좌표 배열입니다.
                out HierarchyIndex[] hierarchy, // 윤곽선 계층 정보입니다.
                RetrievalModes.External, // 가장 바깥쪽 윤곽선만 찾습니다.
                ContourApproximationModes.ApproxSimple // 윤곽선 좌표를 단순화합니다.
            );

            List<OpenCvSharp.Point[]> filteredContours = contours // 전체 윤곽선 목록을 대상으로 합니다.
                .Where(contour => Cv2.ContourArea(contour) > options.MinContourArea) // 슬라이더에서 받은 최소 면적보다 큰 윤곽선만 남깁니다.
                .ToList(); // 결과를 List로 변환합니다.

            Cv2.DrawContours( // 최종 결과 이미지에 윤곽선을 그립니다.
                visualized, // 윤곽선을 그릴 이미지입니다.
                filteredContours, // 필터링된 윤곽선 목록입니다.
                -1, // 모든 윤곽선을 그립니다.
                Scalar.LimeGreen, // 윤곽선 색상입니다.
                options.ContourThickness // 슬라이더에서 받은 윤곽선 두께입니다.
            );

            return new EdgeDetectionResult( // 화면 표시용 결과 객체를 생성합니다.
                original.Clone(), // 원본 이미지 복사본입니다.
                gray.Clone(), // 흑백 이미지 복사본입니다.
                blurred.Clone(), // 블러 이미지 복사본입니다.
                edges.Clone(), // 엣지 이미지 복사본입니다.
                closed.Clone(), // Close 이미지 복사본입니다.
                visualized.Clone(), // 최종 결과 이미지 복사본입니다.
                filteredContours.Count // 검출된 윤곽선 개수입니다.
            );
        }
        finally // 함수가 끝나면 중간 Mat 객체들을 해제합니다.
        {
            original.Dispose(); // 원본 Mat을 해제합니다.
            gray.Dispose(); // 흑백 Mat을 해제합니다.
            blurred.Dispose(); // 블러 Mat을 해제합니다.
            edges.Dispose(); // 엣지 Mat을 해제합니다.
            closed.Dispose(); // Close Mat을 해제합니다.
            visualized.Dispose(); // 최종 결과 Mat을 해제합니다.
        }
    }
}
```

---

# 파일 8/8 — `ProductEdgeViewerWpfSlider/Utils/MatBitmapConverter.cs`

## 무엇을 위한 파일인가?

이 파일은 OpenCV `Mat` 이미지를 WPF `Image` 컨트롤에 표시할 수 있는 `BitmapSource`로 변환합니다.

```csharp
// ProductEdgeViewerWpfSlider/Utils/MatBitmapConverter.cs // 이 파일은 OpenCV Mat 이미지를 WPF BitmapSource로 변환합니다.

using OpenCvSharp; // OpenCV Mat 이미지와 이미지 인코딩 기능을 사용합니다.
using System.IO; // MemoryStream을 사용하기 위해 필요합니다.
using System.Windows.Media.Imaging; // WPF BitmapSource와 BitmapImage를 사용하기 위해 필요합니다.

namespace ProductEdgeViewerWpfSlider.Utils; // 유틸리티 클래스가 속한 네임스페이스입니다.

public static class MatBitmapConverter // Mat을 BitmapSource로 변환하는 정적 유틸리티 클래스입니다.
{
    public static BitmapSource ToBitmapSource(Mat mat) // OpenCV Mat 이미지를 WPF BitmapSource로 변환합니다.
    {
        Cv2.ImEncode(".png", mat, out byte[] bytes); // Mat 이미지를 PNG 바이트 배열로 인코딩합니다.
        using var stream = new MemoryStream(bytes); // 바이트 배열을 읽기 위한 메모리 스트림을 생성합니다.

        var bitmap = new BitmapImage(); // WPF 이미지 객체를 생성합니다.
        bitmap.BeginInit(); // BitmapImage 초기화를 시작합니다.
        bitmap.CacheOption = BitmapCacheOption.OnLoad; // 스트림이 닫혀도 이미지를 유지하도록 설정합니다.
        bitmap.StreamSource = stream; // 이미지 데이터를 읽을 스트림을 지정합니다.
        bitmap.EndInit(); // BitmapImage 초기화를 완료합니다.
        bitmap.Freeze(); // UI 스레드 안정성을 위해 이미지를 불변 상태로 만듭니다.

        return bitmap; // 변환된 이미지를 반환합니다.
    }
}
```

---

# 10. 실행 방법

Visual Studio에서 아래 순서로 실행합니다.

```text
상단 메뉴
→ 빌드
→ 솔루션 정리
```

그 다음:

```text
상단 메뉴
→ 빌드
→ 솔루션 다시 빌드
```

그 다음 실행합니다.

```text
F5
```

---

# 11. 사용 방법

```text
1. 프로그램 실행
2. 사진 열기 클릭
3. 제품 사진 선택
4. 원본 / 흑백 / 블러 / Canny / Close / 최종 윤곽선 결과 확인
5. Canny 낮은 임계값 조절
6. Canny 높은 임계값 조절
7. 최소 윤곽선 면적 조절
8. 윤곽선 두께 조절
9. 결과가 어떻게 바뀌는지 관찰
```

---

# 12. Canny 임계값 설명

## 12-1. 낮은 임계값

낮은 임계값은 약한 경계를 얼마나 살릴지 결정합니다.

```text
낮은 임계값을 낮추면 → 약한 경계도 많이 살아남음
낮은 임계값을 높이면 → 약한 경계가 제거됨
```

## 12-2. 높은 임계값

높은 임계값은 확실한 경계로 인정할 기준입니다.

```text
높은 임계값을 낮추면 → 더 많은 경계가 강한 경계로 인정됨
높은 임계값을 높이면 → 정말 강한 경계만 남음
```

---

# 13. 최소 윤곽선 면적 설명

최소 윤곽선 면적은 작은 잡음을 제거하는 기준입니다.

```text
Min Area가 작으면 → 작은 윤곽선도 많이 표시됨
Min Area가 크면 → 작은 잡음이 제거되고 큰 윤곽선만 남음
```

예를 들어 금속 제품처럼 내부 선이 많은 사진에서는 Min Area를 높이면 작은 잡음이 줄어듭니다.

---

# 14. 윤곽선 두께 설명

윤곽선 두께는 최종 결과 이미지 위에 그려지는 초록색 선의 두께입니다.

```text
1 → 얇은 선
3 → 기본 선
10 → 매우 두꺼운 선
```

수업용 화면에서는 `3` 정도가 가장 무난합니다.

---

# 15. OpenCV 내부 처리 과정 설명

## 15-1. 원본 이미지

사용자가 선택한 사진입니다.  
컴퓨터는 이것을 사람이 보는 이미지가 아니라 픽셀 숫자의 배열로 봅니다.

## 15-2. 흑백 변환

컬러 정보를 제거하고 밝기 정보만 남깁니다.

```text
색상보다 밝기 변화가 경계 검출에 더 중요하기 때문입니다.
```

## 15-3. 가우시안 블러

작은 노이즈와 미세한 밝기 변화를 줄입니다.

```text
너무 작은 변화까지 경계로 잡히면 결과가 지저분해지기 때문입니다.
```

## 15-4. Canny 엣지 검출

밝기 변화가 큰 위치를 경계 후보로 찾습니다.

```text
밝기 변화가 큼 → 경계 후보
밝기 변화가 작음 → 경계 아님
```

## 15-5. Morphology Close

끊어진 선 조각을 조금 연결합니다.

```text
Canny 결과에서 끊어진 선이 있으면 Close 연산으로 일부 연결할 수 있습니다.
```

## 15-6. FindContours

연결된 선들을 따라가며 윤곽선을 찾습니다.

```text
Edge = 경계 후보 선
Contour = 연결된 경계선을 따라 만든 윤곽선
```

## 15-7. DrawContours

찾은 윤곽선을 원본 이미지 위에 초록색으로 그립니다.

---

# 16. 수업에서 설명할 문장

아래 문장을 그대로 사용해도 됩니다.

```text
이 프로그램은 AI가 사진 속 물체를 이해하는 것이 아닙니다.

OpenCV는 이미지를 픽셀 숫자의 배열로 보고,
밝기 변화가 급격한 위치를 경계 후보로 찾습니다.

Canny 낮은 임계값은 약한 경계를 얼마나 살릴지 결정하고,
높은 임계값은 확실한 경계로 인정할 기준을 정합니다.

그 다음 Morphology Close로 끊어진 선을 조금 연결하고,
FindContours로 연결된 선들을 윤곽선으로 묶습니다.

마지막으로 최소 윤곽선 면적보다 작은 잡음은 제거하고,
남은 윤곽선을 원본 이미지 위에 초록색으로 표시합니다.
```

---

# 17. 왜 고양이는 잘 안 잡히는가?

고양이 사진은 제품 사진과 다릅니다.

| 항목 | 제품 사진 | 고양이 사진 |
|---|---|---|
| 경계 | 딱딱하고 선명함 | 털 때문에 부드러움 |
| 외곽선 | 닫힌 도형에 가까움 | 털 때문에 끊어짐 |
| 배경 대비 | 높은 경우가 많음 | 털과 배경이 섞임 |
| 검출 결과 | 윤곽선으로 묶기 쉬움 | 잡음처럼 흩어질 수 있음 |

즉, 이 프로그램은 고양이를 찾는 AI가 아니라 밝기 변화 기반 경계 검출 프로그램이기 때문에 고양이 전체를 정확히 잡지 못할 수 있습니다.

---

# 18. 잘 되는 사진 조건

```text
흰 배경
단색 제품
경계가 선명한 제품
금속 링
검은 브라켓
그림자가 적은 사진
배경이 복잡하지 않은 사진
```

---

# 19. 잘 안 되는 사진 조건

```text
털이 많은 동물
배경이 복잡한 사진
반사가 심한 금속
흐릿한 사진
제품과 배경 색이 비슷한 사진
그림자가 강한 사진
```

---

# 20. 자주 발생하는 오류와 해결

## 20-1. `Cv2`를 찾을 수 없습니다

원인:

```text
OpenCvSharp4 패키지가 설치되지 않았습니다.
```

해결:

```powershell
Install-Package OpenCvSharp4
Install-Package OpenCvSharp4.runtime.win
```

## 20-2. `DetectEdges`를 찾을 수 없습니다

원인:

```text
MainWindow.xaml.cs는 슬라이더 버전인데,
EdgeDetectionService.cs는 이전 버전일 가능성이 큽니다.
```

해결:

```text
EdgeDetectionService.cs를 이 문서의 최종 코드로 교체합니다.
```

## 20-3. `ToBitmapSource`를 찾을 수 없습니다

원인:

```text
MatBitmapConverter.cs가 이전 버전이거나 메서드 이름이 다릅니다.
```

해결:

```text
MatBitmapConverter.cs를 이 문서의 최종 코드로 교체합니다.
```

## 20-4. `NullReferenceException`이 발생합니다

원인:

```text
WPF 초기화 중 Slider.ValueChanged 이벤트가 먼저 실행되었습니다.
```

해결:

```text
MainWindow.xaml.cs에서 isWindowReady와 AreSliderControlsReady()가 들어간 최종 코드를 사용합니다.
```

---

# 21. 최종 체크리스트

```text
[ ] 프로젝트 이름이 ProductEdgeViewerWpfSlider이다.
[ ] OpenCvSharp4를 설치했다.
[ ] OpenCvSharp4.runtime.win을 설치했다.
[ ] Models 폴더가 있다.
[ ] Services 폴더가 있다.
[ ] Utils 폴더가 있다.
[ ] EdgeDetectionOptions.cs가 있다.
[ ] EdgeDetectionResult.cs가 있다.
[ ] EdgeDetectionService.cs가 최종 코드다.
[ ] MatBitmapConverter.cs가 최종 코드다.
[ ] MainWindow.xaml이 최종 코드다.
[ ] MainWindow.xaml.cs가 최종 코드다.
[ ] 솔루션 정리를 했다.
[ ] 솔루션 다시 빌드를 했다.
[ ] F5로 실행했다.
[ ] 사진 열기로 이미지를 불러왔다.
[ ] 슬라이더 변경 시 결과가 자동 갱신된다.
```

---

# 22. 다음 확장 아이디어

이번 버전 다음에는 아래 기능을 추가하면 좋습니다.

## 22-1. 기본값 복원 버튼

```text
Low = 50
High = 150
Min Area = 300
Thickness = 3
```

으로 되돌리는 버튼입니다.

## 22-2. 최종 결과 저장 버튼

OpenCV 결과 이미지를 PNG로 저장하는 기능입니다.

## 22-3. 가장 큰 윤곽선만 표시

제품 외곽선 하나만 보고 싶을 때 유용합니다.

## 22-4. 내부 구멍까지 찾기 옵션

현재는 가장 바깥쪽 윤곽선 중심입니다.

```csharp
RetrievalModes.External
```

내부 구멍까지 보고 싶으면 아래 옵션을 실험할 수 있습니다.

```csharp
RetrievalModes.Tree
```

## 22-5. Bounding Box 표시

윤곽선을 감싸는 사각형을 표시하면 제품 위치 검출 설명에 좋습니다.

```csharp
Cv2.BoundingRect(contour)
```

---

# 23. 최종 정리

이 최종 버전의 핵심은 다음입니다.

```text
1. WPF로 UI를 안정적으로 구성한다.
2. OpenCvSharp로 이미지 처리를 수행한다.
3. 원본부터 최종 결과까지 모든 중간 단계를 시각화한다.
4. Canny 임계값을 슬라이더로 조절한다.
5. 슬라이더 변경에 따라 결과가 자동으로 재계산된다.
6. WPF 초기화 중 NullReferenceException을 방지한다.
```

이 프로그램은 Vision AI로 들어가기 전,  
전통적인 컴퓨터 비전이 **이미지를 어떤 방식으로 숫자 처리하는지** 이해시키기에 적합한 실습 예제입니다.
