# C# WPF + OpenCvSharp Canny 임계값 슬라이더 추가 버전 튜토리얼

> 목표: **사진 1장**을 불러와서 OpenCV Canny Edge Detection 과정을 단계별로 보여주고,  
> 사용자가 **Canny 낮은 임계값 / 높은 임계값 / 최소 윤곽선 면적 / 윤곽선 두께**를 슬라이더로 조절하면서  
> 결과가 어떻게 변하는지 직접 확인하는 WPF 학습용 프로그램을 완성합니다.

---

# 1. 이번 버전에서 추가되는 기능

기존 버전은 아래처럼 고정값으로 처리했습니다.

```csharp
Cv2.Canny(blurred, edges, 50, 150);
```

이번 버전은 이 값을 화면에서 직접 조절합니다.

| 조절 항목 | 기본값 | 의미 |
|---|---:|---|
| Canny 낮은 임계값 | 50 | 약한 경계를 살릴지 버릴지 결정 |
| Canny 높은 임계값 | 150 | 강한 경계로 인정할 기준 |
| 최소 윤곽선 면적 | 300 | 너무 작은 잡음 윤곽선 제거 기준 |
| 윤곽선 두께 | 3 | 최종 결과에 그릴 초록색 선 두께 |

---

# 2. 학생에게 설명할 핵심 개념

## 2-1. Canny 임계값이란?

Canny는 경계를 세 부류로 나눕니다.

```text
높은 임계값보다 강한 변화 → 확실한 경계
낮은 임계값과 높은 임계값 사이 → 애매한 경계
낮은 임계값보다 약한 변화 → 경계 아님
```

예를 들어:

```text
Low Threshold = 50
High Threshold = 150
```

이면 OpenCV는 대략 이렇게 봅니다.

| 밝기 변화 강도 | 판단 |
|---:|---|
| 180 | 확실한 경계 |
| 90 | 애매한 경계 |
| 20 | 경계 아님 |

애매한 경계는 **확실한 경계와 연결되어 있으면 살리고**, 혼자 떨어져 있으면 버립니다.

---

# 3. 이번 프로그램의 화면 구성

이번 WPF 화면은 아래처럼 구성합니다.

```text
상단:
[사진 열기] [상태 표시]

설정 영역:
Canny 낮은 임계값 슬라이더
Canny 높은 임계값 슬라이더
최소 윤곽선 면적 슬라이더
윤곽선 두께 슬라이더
[다시 처리] 버튼

이미지 영역:
[원본] [흑백] [블러]
[Canny 엣지] [Close 결과] [최종 윤곽선]
```

이번 버전에서는 **Close 결과**도 추가합니다.  
이유는 Canny에서 끊어진 선이 Morphology Close 이후 어떻게 연결되는지 보여주기 좋기 때문입니다.

---

# 4. 프로젝트 생성

Visual Studio에서 새 프로젝트를 만듭니다.

```text
Visual Studio 실행
→ 새 프로젝트 만들기
→ WPF 앱 선택
→ 프로젝트 이름: ProductEdgeViewerWpfSlider
→ 프레임워크: .NET 8.0 또는 .NET 9.0 또는 .NET 10.0
→ 만들기
```

---

# 5. NuGet 패키지 설치

Visual Studio에서 아래 메뉴로 이동합니다.

```text
도구
→ NuGet 패키지 관리자
→ 패키지 관리자 콘솔
```

패키지 관리자 콘솔에서 **기본 프로젝트**가 현재 프로젝트인지 확인합니다.

```text
기본 프로젝트: ProductEdgeViewerWpfSlider
```

그 다음 아래 명령어를 실행합니다.

```powershell
Install-Package OpenCvSharp4
Install-Package OpenCvSharp4.runtime.win
```

## 5-1. 왜 이 라이브러리를 쓰는가?

| 라이브러리 | 사용 이유 |
|---|---|
| OpenCvSharp4 | C#에서 OpenCV 함수 사용 |
| OpenCvSharp4.runtime.win | Windows에서 OpenCV 네이티브 DLL 실행 |

---

# 6. 최종 폴더 구조

아래 구조로 만듭니다.

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

폴더 생성 방법:

```text
프로젝트 우클릭
→ 추가
→ 새 폴더
→ Models

프로젝트 우클릭
→ 추가
→ 새 폴더
→ Services

프로젝트 우클릭
→ 추가
→ 새 폴더
→ Utils
```

---

# 7. 전체 코드

아래 파일들을 그대로 작성합니다.

---

# 파일 1/8 — `ProductEdgeViewerWpfSlider/App.xaml`

## 무엇을 위한 파일인가?

WPF 프로그램의 시작 창을 지정합니다.

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

WPF 애플리케이션의 기본 App 클래스를 담당합니다.

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

이 파일은 Canny 임계값, 최소 면적, 선 두께 같은 **사용자 조절 옵션**을 담습니다.

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

사용자 화면 UI를 담당합니다.  
슬라이더와 단계별 이미지 표시 영역을 포함합니다.

## 1/3

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

## 2/3

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

                    <TextBlock Text="슬라이더를 조절한 뒤 다시 처리 버튼을 누르면 Canny 결과와 윤곽선 결과가 바뀝니다."
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
```

## 3/3

```xml
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

버튼 클릭, 슬라이더 값 읽기, OpenCV 처리 호출, 화면 표시를 담당합니다.

## 1/2

```csharp
// ProductEdgeViewerWpfSlider/MainWindow.xaml.cs // 이 파일은 버튼 이벤트, 슬라이더 이벤트, 화면 표시 로직을 담당합니다.

using Microsoft.Win32; // WPF 파일 선택 창을 사용하기 위해 필요합니다.
using ProductEdgeViewerWpfSlider.Models; // Canny 임계값 옵션 모델을 사용합니다.
using ProductEdgeViewerWpfSlider.Services; // OpenCV 경계 검출 서비스를 사용합니다.
using ProductEdgeViewerWpfSlider.Utils; // OpenCV Mat을 WPF 이미지로 변환하는 유틸리티를 사용합니다.
using System; // Exception 같은 기본 기능을 사용합니다.
using System.IO; // 파일명 추출을 위해 Path 기능을 사용합니다.
using System.Windows; // WPF Window와 MessageBox를 사용합니다.

namespace ProductEdgeViewerWpfSlider; // 현재 프로젝트의 네임스페이스입니다.

public partial class MainWindow : Window // MainWindow.xaml과 연결되는 WPF 메인 창 클래스입니다.
{
    private readonly EdgeDetectionService edgeService = new(); // OpenCV 경계 검출 로직을 담당하는 서비스 객체입니다.
    private string? currentFilePath; // 현재 불러온 이미지 파일 경로를 저장합니다.

    public MainWindow() // 메인 창이 생성될 때 실행되는 생성자입니다.
    {
        InitializeComponent(); // XAML에 정의된 UI 요소들을 실제 객체로 초기화합니다.
        UpdateSliderText(); // 프로그램 시작 시 슬라이더 숫자 표시를 초기화합니다.
    }

    private void BtnOpen_Click(object sender, RoutedEventArgs e) // 사진 열기 버튼을 클릭했을 때 실행되는 함수입니다.
    {
        var dialog = new OpenFileDialog(); // WPF 파일 선택 창 객체를 생성합니다.
        dialog.Title = "제품 사진 선택"; // 파일 선택 창 제목을 설정합니다.
        dialog.Filter = "이미지 파일|*.jpg;*.jpeg;*.png;*.bmp|모든 파일|*.*"; // 선택 가능한 파일 확장자를 지정합니다.

        bool? selected = dialog.ShowDialog(); // 파일 선택 창을 열고 결과를 받습니다.

        if (selected != true) // 사용자가 취소했는지 확인합니다.
        {
            return; // 취소한 경우 아무 작업도 하지 않고 종료합니다.
        }

        currentFilePath = dialog.FileName; // 선택된 파일 경로를 저장합니다.
        ProcessCurrentImage(); // 현재 파일과 현재 슬라이더 값으로 이미지를 처리합니다.
    }

    private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) // 슬라이더 값이 바뀔 때 실행되는 함수입니다.
    {
        UpdateSliderText(); // 화면에 보이는 숫자 값을 갱신합니다.

        if (IsLoaded && currentFilePath is not null) // 화면이 로드된 뒤이고 이미지가 선택되어 있는지 확인합니다.
        {
            ProcessCurrentImage(); // 슬라이더 값 변경에 맞춰 이미지를 다시 처리합니다.
        }
    }
```

## 2/2

```csharp
    private void UpdateSliderText() // 슬라이더 옆 숫자 표시를 갱신하는 함수입니다.
    {
        if (TxtCannyLow is null) // XAML 초기화 전 호출될 가능성을 방지합니다.
        {
            return; // 아직 컨트롤이 준비되지 않았으면 종료합니다.
        }

        TxtCannyLow.Text = ((int)SliderCannyLow.Value).ToString(); // 낮은 임계값 표시를 갱신합니다.
        TxtCannyHigh.Text = ((int)SliderCannyHigh.Value).ToString(); // 높은 임계값 표시를 갱신합니다.
        TxtMinArea.Text = ((int)SliderMinArea.Value).ToString(); // 최소 면적 표시를 갱신합니다.
        TxtThickness.Text = ((int)SliderThickness.Value).ToString(); // 윤곽선 두께 표시를 갱신합니다.
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

OpenCV의 단계별 결과 이미지를 보관합니다.

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

OpenCV 경계 검출 알고리즘을 담당합니다.  
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

OpenCV `Mat` 이미지를 WPF `Image` 컨트롤에 표시할 수 있는 `BitmapSource`로 변환합니다.

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

# 8. 실행 방법

Visual Studio에서 실행합니다.

```text
상단 메뉴 → 디버그 → 디버그 시작
```

또는:

```text
F5
```

실행 후:

```text
1. 사진 열기 클릭
2. 제품 사진 선택
3. 슬라이더 조절
4. 각 단계 이미지 변화 확인
```

---

# 9. 슬라이더 값별 해석

## 9-1. Canny 낮은 임계값을 낮추면

```text
약한 경계까지 더 많이 살아납니다.
결과적으로 엣지가 많아질 수 있습니다.
잡음도 같이 증가할 수 있습니다.
```

## 9-2. Canny 낮은 임계값을 높이면

```text
약한 경계가 많이 제거됩니다.
결과가 깔끔해질 수 있지만 필요한 경계도 사라질 수 있습니다.
```

## 9-3. Canny 높은 임계값을 낮추면

```text
강한 경계로 인정되는 기준이 낮아집니다.
더 많은 선이 검출될 수 있습니다.
```

## 9-4. Canny 높은 임계값을 높이면

```text
정말 강한 밝기 변화만 경계로 인정됩니다.
결과가 단순해지지만 일부 경계가 끊길 수 있습니다.
```

## 9-5. 최소 윤곽선 면적을 높이면

```text
작은 윤곽선이 제거됩니다.
복잡한 사진에서 잡음을 줄이는 데 도움이 됩니다.
```

## 9-6. 최소 윤곽선 면적을 낮추면

```text
작은 윤곽선도 살아납니다.
구멍이나 작은 부품 경계가 보일 수 있지만 잡음도 많아집니다.
```

---

# 10. 수업에서 설명할 문장

아래 문장을 그대로 설명에 사용해도 됩니다.

```text
이 프로그램은 AI가 사진 속 물체가 무엇인지 이해하는 것이 아닙니다.

OpenCV는 이미지를 픽셀 숫자의 배열로 보고,
밝기 변화가 급격한 위치를 경계 후보로 찾습니다.

Canny 낮은 임계값은 약한 경계를 얼마나 살릴지 결정하고,
높은 임계값은 확실한 경계로 인정할 기준을 정합니다.

그 다음 FindContours는 연결된 경계선들을 윤곽선으로 묶고,
최소 윤곽선 면적보다 작은 것은 잡음으로 보고 제거합니다.

그래서 슬라이더를 움직이면
어떤 선이 살아남고 어떤 선이 사라지는지 직접 확인할 수 있습니다.
```

---

# 11. 추천 실습 순서

학생과 같이 진행할 때는 아래 순서를 추천합니다.

```text
1. 기본값 Low 50 / High 150 / Area 300으로 제품 사진 실행
2. Low 값을 낮춰 엣지가 늘어나는지 관찰
3. High 값을 높여 강한 경계만 남는지 관찰
4. Min Area 값을 높여 작은 윤곽선이 사라지는지 관찰
5. 고양이 사진을 넣어 왜 잘 안 잡히는지 비교
6. 금속 링 사진을 넣어 왜 잘 잡히는지 비교
```

---

# 12. 이번 버전에서 추가하면 좋은 확장 기능

현재 버전에 추가하면 좋은 기능은 아래와 같습니다.

## 12-1. 기본값 복원 버튼

슬라이더 값을 다시 아래로 되돌립니다.

```text
Low = 50
High = 150
Min Area = 300
Thickness = 3
```

## 12-2. 결과 이미지 저장 버튼

최종 윤곽선 이미지를 PNG 파일로 저장합니다.

## 12-3. 가장 큰 윤곽선만 표시 옵션

제품 외곽선 하나만 보고 싶을 때 유용합니다.

## 12-4. 내부 구멍까지 찾기 옵션

현재는 `RetrievalModes.External`을 사용합니다.  
내부 구멍까지 찾고 싶으면 `RetrievalModes.Tree`를 사용할 수 있습니다.

---

# 13. 핵심 정리

```text
Canny Edge Detection은 밝기 변화가 큰 위치를 찾는다.
낮은 임계값과 높은 임계값은 경계를 얼마나 엄격하게 볼지 결정한다.
FindContours는 엣지 선들을 윤곽선으로 묶는다.
최소 윤곽선 면적은 잡음을 제거하는 기준이다.
슬라이더를 조절하면 이 원리를 눈으로 확인할 수 있다.
```

---

# 14. 자주 발생하는 오류

## 14-1. `Cv2`를 찾을 수 없습니다

원인:

```text
OpenCvSharp4 패키지가 설치되지 않았거나,
다른 프로젝트에 설치되었을 가능성이 큽니다.
```

해결:

```powershell
Install-Package OpenCvSharp4
Install-Package OpenCvSharp4.runtime.win
```

## 14-2. `ProductEdgeViewerWpfSlider.Models`를 찾을 수 없습니다

원인:

```text
Models 폴더 또는 EdgeDetectionOptions.cs 파일이 없거나,
namespace가 다릅니다.
```

해결:

```csharp
namespace ProductEdgeViewerWpfSlider.Models;
```

를 확인합니다.

## 14-3. XAML에서 이름을 찾을 수 없습니다

원인:

```text
x:Name 이름과 MainWindow.xaml.cs에서 사용하는 이름이 다릅니다.
```

예:

```xml
<Image x:Name="ImgClosed"/>
```

와

```csharp
ImgClosed.Source = ...
```

가 일치해야 합니다.

---

# 15. 최종 체크리스트

```text
[ ] WPF 프로젝트를 생성했다.
[ ] OpenCvSharp4를 설치했다.
[ ] OpenCvSharp4.runtime.win을 설치했다.
[ ] Models 폴더를 만들었다.
[ ] Services 폴더를 만들었다.
[ ] Utils 폴더를 만들었다.
[ ] EdgeDetectionOptions.cs를 만들었다.
[ ] EdgeDetectionResult.cs를 만들었다.
[ ] EdgeDetectionService.cs를 만들었다.
[ ] MatBitmapConverter.cs를 만들었다.
[ ] MainWindow.xaml을 교체했다.
[ ] MainWindow.xaml.cs를 교체했다.
[ ] F5로 실행했다.
[ ] 사진을 불러왔다.
[ ] 슬라이더 조절 결과가 화면에 반영된다.
```
