# LS G100 인버터 WPF 제어 프로그램


<img width="893" height="461" alt="image" src="https://github.com/user-attachments/assets/7d5ddb8d-e089-4464-97d7-745c00616986" />




https://github.com/user-attachments/assets/bf9e6322-ba34-4157-a5ca-f508b24ac05a


## 프로젝트 문서 (전체 소스코드 포함)

---

## 1. 프로젝트 개요

| 항목 | 내용 |
|------|------|
| 프로젝트명 | LSG100_InverterControl |
| 대상 장비 | LS Electric LSLV-G100 시리즈 인버터 |
| 통신 방식 | Modbus RTU (RS-485) |
| 프레임워크 | .NET 8.0 WPF (Windows) |
| 아키텍처 | MVVM (Model-View-ViewModel) |
| NuGet 패키지 | `System.IO.Ports` 8.0.0 |
| UI 테마 | Catppuccin Mocha + 네온 사이버 스타일 |

---

## 2. 프로젝트 구조

```
LSG100_InverterControl/
├── LSG100_InverterControl.csproj    ← 프로젝트 파일
├── App.xaml                         ← 전역 테마/스타일 리소스
├── App.xaml.cs
├── MainWindow.xaml                  ← 메인 UI (계기판 + 제어)
├── MainWindow.xaml.cs               ← 코드비하인드 (로그 자동스크롤)
├── Models/
│   └── InverterModel.cs             ← 레지스터 맵, 명령 상수, 상태 열거형
├── Services/
│   └── ModbusRtuService.cs          ← Modbus RTU 통신 (FC03/FC06, CRC16)
├── ViewModels/
│   ├── RelayCommand.cs              ← ICommand 구현
│   └── MainViewModel.cs             ← 메인 뷰모델 (제어 로직 + 모니터링)
└── Converters/
    └── BoolToColorConverter.cs      ← 값 변환기 모음 (5개 클래스)
```

---

## 3. Modbus RTU 레지스터 맵

### 3.1 쓰기 레지스터 (Control Area)

| 주소 | 이름 | 단위 | 설명 |
|------|------|------|------|
| `0x0004` | REG_FREQ_SET | 0.01 Hz | 주파수 설정 (예: 3000 = 30.00Hz) |
| `0x0005` | REG_CMD | - | 운전 명령 |

### 3.2 운전 명령 비트값

| 값 | 상수명 | 동작 |
|----|--------|------|
| `0x0001` | CMD_STOP | 정지 (bit0) |
| `0x0002` | CMD_FWD | 정방향 운전 (bit1) |
| `0x0004` | CMD_REV | 역방향 운전 (bit2) |

### 3.3 읽기 레지스터 (Monitoring Area)

| 주소 | 이름 | 단위 | 설명 |
|------|------|------|------|
| `0x0008` | REG_STATUS | - | 인버터 상태 |
| `0x0009` | REG_FREQ_OUT | 0.01 Hz | 출력 주파수 |
| `0x000A` | REG_CURR_OUT | 0.01 A | 출력 전류 |

> **참고**: 출력 전류의 실제 단위는 인버터 용량에 따라 다를 수 있습니다.
> 현재 코드에서는 `/ 1000.0`으로 변환 중이며, 실측에 맞게 조정하세요.

---

## 4. UI 게이지 스케일 조정 가이드

### 4.1 구조

주파수와 전류 게이지는 모두 **동일한 컨버터(`ValueToStrokeDashConverter`)**를 사용합니다.
스케일 조정은 **MainWindow.xaml**의 `ConverterParameter`에서 합니다.

```
BoolToColorConverter.cs 내부 클래스:
  ValueToStrokeDashConverter
    ↑
    │  ConverterParameter="최대값" 또는 "최대값,보정값"
    │
MainWindow.xaml에서 호출
  ├── 주파수 글로우 아크:  ConverterParameter=60
  ├── 주파수 선명 아크:    ConverterParameter=60
  ├── 전류 글로우 아크:    ConverterParameter=0.23  ← 현재 설정
  └── 전류 선명 아크:      ConverterParameter=20    ← 현재 설정
```

### 4.2 ConverterParameter 형식

```
"최대값"           →  예: "60"       (보정값은 기본 35.5)
"최대값,보정값"     →  예: "60,35.5"  (직접 지정)
```

- **최대값**: 게이지 풀스케일 (이 값일 때 아크 100%)
- **보정값(totalDashUnits)**: 아크 길이 보정
  - 줄이면 → 아크가 더 많이 채워짐
  - 늘리면 → 아크가 덜 채워짐
  - 기본값: `35.5`

### 4.3 수정 위치 (MainWindow.xaml)

#### 주파수 게이지 스케일 변경

검색어: `OutputFrequency` → `ConverterParameter` 2곳 + 눈금 `Text="60"` 1곳

```xml
<!-- 글로우 아크 -->
ConverterParameter=60          ← 최대 주파수(Hz)

<!-- 선명 아크 -->
ConverterParameter=60          ← 동일하게 맞춤

<!-- 눈금 라벨 -->
Text="60"                      ← 표시용
```

#### 전류 게이지 스케일 변경

검색어: `OutputCurrent` → `ConverterParameter` 2곳 + 눈금 `Text="20"` 1곳

```xml
<!-- 글로우 아크 (현재 0.23) -->
ConverterParameter=0.23       ← 최대 전류(A) 또는 "최대값,보정값"

<!-- 선명 아크 (현재 20) -->
ConverterParameter=20         ← ⚠ 글로우와 값이 다름! 통일 필요

<!-- 눈금 라벨 -->
Text="20"                     ← 표시용
```

> **⚠ 주의**: 현재 전류 게이지의 글로우(0.23)와 선명(20)의 ConverterParameter가 다릅니다.
> 둘을 같은 값으로 통일해야 정상 동작합니다. 예: 둘 다 `5`로 변경.

### 4.4 변경 예시

0.4kW 인버터 (정격전류 약 2.5A), 최대 표시 5A로 설정:

```xml
<!-- 전류 글로우 아크 -->
ConverterParameter=5

<!-- 전류 선명 아크 -->
ConverterParameter=5

<!-- 눈금 라벨 -->
Text="5"
```

---

## 5. 전체 소스코드

### 5.1 LSG100_InverterControl.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWPF>true</UseWPF>
    <RootNamespace>LSG100_InverterControl</RootNamespace>
    <AssemblyName>LSG100_InverterControl</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="System.IO.Ports" Version="8.0.0" />
  </ItemGroup>

</Project>
```

---

### 5.2 App.xaml

```xml
<Application x:Class="LSG100_InverterControl.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="MainWindow.xaml">
    <Application.Resources>

        <!-- ===== 색상 팔레트 (Catppuccin Mocha + 네온 악센트) ===== -->
        <Color x:Key="Crust">#0A0A12</Color>
        <Color x:Key="Mantle">#12121E</Color>
        <Color x:Key="Base">#181826</Color>
        <Color x:Key="Surface0">#252538</Color>
        <Color x:Key="Surface1">#35354D</Color>
        <Color x:Key="Surface2">#45456A</Color>
        <Color x:Key="Overlay0">#6C7086</Color>
        <Color x:Key="Text">#E0E4F7</Color>
        <Color x:Key="Subtext0">#8891B2</Color>
        <Color x:Key="Cyan">#00F0FF</Color>
        <Color x:Key="Green">#39FF85</Color>
        <Color x:Key="Red">#FF4070</Color>
        <Color x:Key="Yellow">#FFD644</Color>
        <Color x:Key="Orange">#FF8844</Color>
        <Color x:Key="Magenta">#D45BFF</Color>
        <Color x:Key="Blue">#4488FF</Color>
        <Color x:Key="DimCyan">#1A5058</Color>

        <SolidColorBrush x:Key="CrustBrush"    Color="{StaticResource Crust}" />
        <SolidColorBrush x:Key="MantleBrush"   Color="{StaticResource Mantle}" />
        <SolidColorBrush x:Key="BaseBrush"     Color="{StaticResource Base}" />
        <SolidColorBrush x:Key="Surface0Brush" Color="{StaticResource Surface0}" />
        <SolidColorBrush x:Key="Surface1Brush" Color="{StaticResource Surface1}" />
        <SolidColorBrush x:Key="Surface2Brush" Color="{StaticResource Surface2}" />
        <SolidColorBrush x:Key="Overlay0Brush" Color="{StaticResource Overlay0}" />
        <SolidColorBrush x:Key="TextBrush"     Color="{StaticResource Text}" />
        <SolidColorBrush x:Key="Subtext0Brush" Color="{StaticResource Subtext0}" />
        <SolidColorBrush x:Key="CyanBrush"     Color="{StaticResource Cyan}" />
        <SolidColorBrush x:Key="GreenBrush"    Color="{StaticResource Green}" />
        <SolidColorBrush x:Key="RedBrush"      Color="{StaticResource Red}" />
        <SolidColorBrush x:Key="YellowBrush"   Color="{StaticResource Yellow}" />
        <SolidColorBrush x:Key="OrangeBrush"   Color="{StaticResource Orange}" />
        <SolidColorBrush x:Key="MagentaBrush"  Color="{StaticResource Magenta}" />
        <SolidColorBrush x:Key="BlueBrush"     Color="{StaticResource Blue}" />
        <SolidColorBrush x:Key="DimCyanBrush"  Color="{StaticResource DimCyan}" />

        <!-- ===== 버튼 ===== -->
        <Style x:Key="HiTechButton" TargetType="Button">
            <Setter Property="FontSize" Value="13" />
            <Setter Property="FontWeight" Value="Bold" />
            <Setter Property="FontFamily" Value="Consolas" />
            <Setter Property="Foreground" Value="{StaticResource CrustBrush}" />
            <Setter Property="Height" Value="38" />
            <Setter Property="Cursor" Value="Hand" />
            <Setter Property="BorderThickness" Value="0" />
            <Setter Property="VerticalAlignment" Value="Center" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Border x:Name="border" Background="{TemplateBinding Background}"
                                CornerRadius="4" Padding="14,0"
                                BorderBrush="{TemplateBinding Background}" BorderThickness="1">
                            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center" />
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter TargetName="border" Property="Opacity" Value="0.88" />
                            </Trigger>
                            <Trigger Property="IsPressed" Value="True">
                                <Setter TargetName="border" Property="Opacity" Value="0.7" />
                            </Trigger>
                            <Trigger Property="IsEnabled" Value="False">
                                <Setter TargetName="border" Property="Opacity" Value="0.35" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <!-- ===== TextBox ===== -->
        <Style x:Key="HiTechTextBox" TargetType="TextBox">
            <Setter Property="Background" Value="{StaticResource Surface0Brush}" />
            <Setter Property="Foreground" Value="{StaticResource CyanBrush}" />
            <Setter Property="CaretBrush" Value="{StaticResource CyanBrush}" />
            <Setter Property="BorderBrush" Value="{StaticResource Surface2Brush}" />
            <Setter Property="BorderThickness" Value="1" />
            <Setter Property="FontFamily" Value="Consolas" />
            <Setter Property="FontSize" Value="14" />
            <Setter Property="FontWeight" Value="Bold" />
            <Setter Property="Height" Value="36" />
            <Setter Property="VerticalAlignment" Value="Center" />
            <Setter Property="VerticalContentAlignment" Value="Center" />
            <Setter Property="Padding" Value="8,0" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="TextBox">
                        <Border Background="{TemplateBinding Background}"
                                BorderBrush="{TemplateBinding BorderBrush}"
                                BorderThickness="{TemplateBinding BorderThickness}"
                                CornerRadius="4">
                            <ScrollViewer x:Name="PART_ContentHost"
                                          VerticalAlignment="Center"
                                          Margin="{TemplateBinding Padding}" />
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <!-- ===== ComboBox ToggleButton ===== -->
        <ControlTemplate x:Key="ComboBoxToggleButton" TargetType="ToggleButton">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition />
                    <ColumnDefinition Width="26" />
                </Grid.ColumnDefinitions>
                <Border Grid.ColumnSpan="2" Background="{StaticResource Surface0Brush}"
                        BorderBrush="{StaticResource Surface2Brush}"
                        BorderThickness="1" CornerRadius="4" />
                <Path Grid.Column="1" Fill="{StaticResource CyanBrush}"
                      HorizontalAlignment="Center" VerticalAlignment="Center"
                      Data="M 0 0 L 4 4 L 8 0 Z" />
            </Grid>
        </ControlTemplate>

        <Style x:Key="HiTechComboBox" TargetType="ComboBox">
            <Setter Property="Foreground" Value="{StaticResource CyanBrush}" />
            <Setter Property="FontFamily" Value="Consolas" />
            <Setter Property="FontSize" Value="13" />
            <Setter Property="FontWeight" Value="Bold" />
            <Setter Property="Height" Value="36" />
            <Setter Property="VerticalAlignment" Value="Center" />
            <Setter Property="VerticalContentAlignment" Value="Center" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="ComboBox">
                        <Grid>
                            <ToggleButton Template="{StaticResource ComboBoxToggleButton}"
                                          Focusable="False"
                                          IsChecked="{Binding IsDropDownOpen, Mode=TwoWay,
                                                      RelativeSource={RelativeSource TemplatedParent}}"
                                          ClickMode="Press" />
                            <ContentPresenter IsHitTestVisible="False"
                                              Content="{TemplateBinding SelectionBoxItem}"
                                              ContentTemplate="{TemplateBinding SelectionBoxItemTemplate}"
                                              Margin="10,0,26,0"
                                              VerticalAlignment="Center" HorizontalAlignment="Left" />
                            <Popup Placement="Bottom"
                                   IsOpen="{TemplateBinding IsDropDownOpen}"
                                   AllowsTransparency="True" Focusable="False" PopupAnimation="Slide">
                                <Grid SnapsToDevicePixels="True"
                                      MinWidth="{TemplateBinding ActualWidth}"
                                      MaxHeight="{TemplateBinding MaxDropDownHeight}">
                                    <Border Background="{StaticResource Surface0Brush}"
                                            BorderBrush="{StaticResource CyanBrush}"
                                            BorderThickness="1" CornerRadius="4" Margin="0,2,0,0" />
                                    <ScrollViewer Margin="4,6">
                                        <StackPanel IsItemsHost="True"
                                                    KeyboardNavigation.DirectionalNavigation="Contained" />
                                    </ScrollViewer>
                                </Grid>
                            </Popup>
                        </Grid>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style TargetType="ComboBoxItem">
            <Setter Property="Foreground" Value="{StaticResource TextBrush}" />
            <Setter Property="FontFamily" Value="Consolas" />
            <Setter Property="Background" Value="Transparent" />
            <Setter Property="Padding" Value="10,5" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="ComboBoxItem">
                        <Border x:Name="Bd" Background="{TemplateBinding Background}"
                                Padding="{TemplateBinding Padding}" CornerRadius="3">
                            <ContentPresenter VerticalAlignment="Center" />
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsHighlighted" Value="True">
                                <Setter TargetName="Bd" Property="Background"
                                        Value="{StaticResource Surface1Brush}" />
                                <Setter Property="Foreground" Value="{StaticResource CyanBrush}" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

    </Application.Resources>
</Application>
```

---

### 5.3 App.xaml.cs

```csharp
using System.Windows;

namespace LSG100_InverterControl
{
    public partial class App : Application { }
}
```

---

### 5.4 MainWindow.xaml

```xml
<Window x:Class="LSG100_InverterControl.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="clr-namespace:LSG100_InverterControl.ViewModels"
        xmlns:cvt="clr-namespace:LSG100_InverterControl.Converters"
        Title="LS G100 INVERTER CONTROL"
        Width="920" Height="760"
        MinWidth="860" MinHeight="700"
        Background="{StaticResource CrustBrush}"
        WindowStartupLocation="CenterScreen"
        Closing="Window_Closing">

    <Window.DataContext>
        <vm:MainViewModel />
    </Window.DataContext>

    <Window.Resources>
        <cvt:BoolToColorConverter x:Key="BoolToColor" />
        <cvt:StateToColorConverter x:Key="StateToColor" />
        <cvt:ValueToAngleConverter x:Key="ValToAngle" />
        <cvt:ValueToStrokeDashConverter x:Key="ValToDash" />
    </Window.Resources>

    <Grid Margin="14">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>

        <!-- ROW 0 : 타이틀 바 -->
        <Border Grid.Row="0" Background="{StaticResource MantleBrush}"
                CornerRadius="6" Padding="18,10" Margin="0,0,0,8"
                BorderBrush="{StaticResource Surface1Brush}" BorderThickness="1">
            <DockPanel>
                <StackPanel DockPanel.Dock="Right" Orientation="Horizontal"
                            VerticalAlignment="Center">
                    <Ellipse Width="8" Height="8" Margin="0,0,6,0"
                             Fill="{Binding IsConnected, Converter={StaticResource BoolToColor}}" />
                    <TextBlock Text="{Binding ConnectionStatusText}"
                               Foreground="{StaticResource Subtext0Brush}"
                               FontFamily="Consolas" FontSize="12" FontWeight="Bold" />
                </StackPanel>
                <StackPanel Orientation="Horizontal" VerticalAlignment="Center">
                    <TextBlock Text="⚡" FontSize="18" Margin="0,0,8,0"
                               VerticalAlignment="Center" Foreground="{StaticResource CyanBrush}" />
                    <TextBlock Text="LS G100  INVERTER CONTROL"
                               FontFamily="Consolas" FontSize="16" FontWeight="Bold"
                               Foreground="{StaticResource CyanBrush}"
                               VerticalAlignment="Center" />
                    <TextBlock Text="  │  MODBUS RTU"
                               FontFamily="Consolas" FontSize="11"
                               Foreground="{StaticResource Subtext0Brush}"
                               VerticalAlignment="Center" />
                </StackPanel>
            </DockPanel>
        </Border>

        <!-- ROW 1 : 연결 설정 -->
        <Border Grid.Row="1" Background="{StaticResource MantleBrush}"
                CornerRadius="6" Padding="14,8" Margin="0,0,0,8"
                BorderBrush="{StaticResource Surface1Brush}" BorderThickness="1">
            <DockPanel>
                <StackPanel DockPanel.Dock="Right" Orientation="Horizontal"
                            VerticalAlignment="Center">
                    <Button Content="CONNECT" Command="{Binding ConnectCommand}"
                            Style="{StaticResource HiTechButton}" Width="90"
                            Background="{StaticResource GreenBrush}" Margin="0,0,6,0" />
                    <Button Content="DISCONNECT" Command="{Binding DisconnectCommand}"
                            Style="{StaticResource HiTechButton}" Width="110"
                            Background="{StaticResource RedBrush}" />
                </StackPanel>
                <StackPanel Orientation="Horizontal" VerticalAlignment="Center">
                    <TextBlock Text="PORT" Foreground="{StaticResource Subtext0Brush}"
                               FontFamily="Consolas" FontSize="11" FontWeight="Bold"
                               VerticalAlignment="Center" Margin="0,0,6,0" />
                    <ComboBox ItemsSource="{Binding AvailablePorts}"
                              SelectedItem="{Binding SelectedPort}"
                              Style="{StaticResource HiTechComboBox}" Width="110"
                              Margin="0,0,14,0" />
                    <TextBlock Text="BAUD" Foreground="{StaticResource Subtext0Brush}"
                               FontFamily="Consolas" FontSize="11" FontWeight="Bold"
                               VerticalAlignment="Center" Margin="0,0,6,0" />
                    <ComboBox ItemsSource="{Binding BaudRates}"
                              SelectedItem="{Binding SelectedBaudRate}"
                              Style="{StaticResource HiTechComboBox}" Width="90"
                              Margin="0,0,14,0" />
                    <TextBlock Text="ID" Foreground="{StaticResource Subtext0Brush}"
                               FontFamily="Consolas" FontSize="11" FontWeight="Bold"
                               VerticalAlignment="Center" Margin="0,0,6,0" />
                    <TextBox Text="{Binding SlaveId, UpdateSourceTrigger=PropertyChanged}"
                             Style="{StaticResource HiTechTextBox}" Width="46"
                             TextAlignment="Center" />
                </StackPanel>
            </DockPanel>
        </Border>

        <!-- ROW 2 : 모니터링 패널 (계기판 스타일) -->
        <Border Grid.Row="2" Background="{StaticResource MantleBrush}"
                CornerRadius="6" Padding="14" Margin="0,0,0,8"
                BorderBrush="{StaticResource Surface1Brush}" BorderThickness="1">
            <UniformGrid Columns="3">

                <!-- 출력 주파수 게이지 -->
                <Border Background="{StaticResource BaseBrush}" CornerRadius="6"
                        Padding="8" Margin="0,0,5,0"
                        BorderBrush="{StaticResource Surface1Brush}" BorderThickness="1">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto" />
                            <RowDefinition Height="Auto" />
                        </Grid.RowDefinitions>
                        <Viewbox Grid.Row="0" Height="140" Margin="0,4,0,0">
                            <Canvas Width="160" Height="130">
                                <!-- 배경 아크 -->
                                <Path Stroke="{StaticResource Surface1Brush}" StrokeThickness="8"
                                      StrokeStartLineCap="Round" StrokeEndLineCap="Round">
                                    <Path.Data>
                                        <PathGeometry>
                                            <PathFigure StartPoint="17,105">
                                                <ArcSegment Point="143,105" Size="70,70"
                                                            SweepDirection="Clockwise"
                                                            IsLargeArc="True" />
                                            </PathFigure>
                                        </PathGeometry>
                                    </Path.Data>
                                </Path>
                                <!-- 글로우 아크 -->
                                <Path StrokeThickness="8"
                                      StrokeStartLineCap="Round" StrokeEndLineCap="Round"
                                      StrokeDashArray="{Binding OutputFrequency,
                                          Converter={StaticResource ValToDash},
                                          ConverterParameter=60}">
                                    <Path.Stroke>
                                        <LinearGradientBrush StartPoint="0,0" EndPoint="1,0">
                                            <GradientStop Color="#00F0FF" Offset="0" />
                                            <GradientStop Color="#39FF85" Offset="1" />
                                        </LinearGradientBrush>
                                    </Path.Stroke>
                                    <Path.Data>
                                        <PathGeometry>
                                            <PathFigure StartPoint="17,105">
                                                <ArcSegment Point="143,105" Size="70,70"
                                                            SweepDirection="Clockwise"
                                                            IsLargeArc="True" />
                                            </PathFigure>
                                        </PathGeometry>
                                    </Path.Data>
                                    <Path.Effect>
                                        <BlurEffect Radius="6" />
                                    </Path.Effect>
                                </Path>
                                <!-- 선명 아크 -->
                                <Path StrokeThickness="8"
                                      StrokeStartLineCap="Round" StrokeEndLineCap="Round"
                                      StrokeDashArray="{Binding OutputFrequency,
                                          Converter={StaticResource ValToDash},
                                          ConverterParameter=60}">
                                    <Path.Stroke>
                                        <LinearGradientBrush StartPoint="0,0" EndPoint="1,0">
                                            <GradientStop Color="#00F0FF" Offset="0" />
                                            <GradientStop Color="#39FF85" Offset="1" />
                                        </LinearGradientBrush>
                                    </Path.Stroke>
                                    <Path.Data>
                                        <PathGeometry>
                                            <PathFigure StartPoint="17,105">
                                                <ArcSegment Point="143,105" Size="70,70"
                                                            SweepDirection="Clockwise"
                                                            IsLargeArc="True" />
                                            </PathFigure>
                                        </PathGeometry>
                                    </Path.Data>
                                </Path>
                                <!-- 중앙 숫자 -->
                                <TextBlock Canvas.Left="80" Canvas.Top="48"
                                           FontFamily="Consolas" FontSize="34" FontWeight="Bold"
                                           Foreground="#00F0FF" TextAlignment="Center"
                                           Text="{Binding OutputFrequency,
                                                  StringFormat={}{0:F2}}">
                                    <TextBlock.RenderTransform>
                                        <TranslateTransform X="-40" />
                                    </TextBlock.RenderTransform>
                                    <TextBlock.Effect>
                                        <DropShadowEffect Color="#00F0FF" BlurRadius="12"
                                                          ShadowDepth="0" Opacity="0.5" />
                                    </TextBlock.Effect>
                                </TextBlock>
                                <TextBlock Canvas.Left="80" Canvas.Top="84"
                                           FontFamily="Consolas" FontSize="13"
                                           Foreground="#5A8A8F" TextAlignment="Center"
                                           Text="Hz">
                                    <TextBlock.RenderTransform>
                                        <TranslateTransform X="-10" />
                                    </TextBlock.RenderTransform>
                                </TextBlock>
                                <TextBlock Canvas.Left="8" Canvas.Top="110"
                                           FontFamily="Consolas" FontSize="9"
                                           Foreground="#5A5A7A" Text="0" />
                                <TextBlock Canvas.Left="132" Canvas.Top="110"
                                           FontFamily="Consolas" FontSize="9"
                                           Foreground="#5A5A7A" Text="60" />
                            </Canvas>
                        </Viewbox>
                        <TextBlock Grid.Row="1" Text="OUTPUT FREQ"
                                   FontFamily="Consolas" FontSize="11" FontWeight="Bold"
                                   Foreground="{StaticResource Subtext0Brush}"
                                   HorizontalAlignment="Center" Margin="0,2,0,6" />
                    </Grid>
                </Border>

                <!-- 출력 전류 게이지 -->
                <Border Background="{StaticResource BaseBrush}" CornerRadius="6"
                        Padding="8" Margin="5,0,5,0"
                        BorderBrush="{StaticResource Surface1Brush}" BorderThickness="1">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto" />
                            <RowDefinition Height="Auto" />
                        </Grid.RowDefinitions>
                        <Viewbox Grid.Row="0" Height="140" Margin="0,4,0,0">
                            <Canvas Width="160" Height="130">
                                <Path Stroke="{StaticResource Surface1Brush}" StrokeThickness="8"
                                      StrokeStartLineCap="Round" StrokeEndLineCap="Round">
                                    <Path.Data>
                                        <PathGeometry>
                                            <PathFigure StartPoint="17,105">
                                                <ArcSegment Point="143,105" Size="70,70"
                                                            SweepDirection="Clockwise"
                                                            IsLargeArc="True" />
                                            </PathFigure>
                                        </PathGeometry>
                                    </Path.Data>
                                </Path>
                                <!-- ★ 전류 글로우 아크 — ConverterParameter 수정 위치 -->
                                <Path StrokeThickness="8"
                                      StrokeStartLineCap="Round" StrokeEndLineCap="Round"
                                      StrokeDashArray="{Binding OutputCurrent,
                                          Converter={StaticResource ValToDash},
                                          ConverterParameter=0.23}">
                                    <Path.Stroke>
                                        <LinearGradientBrush StartPoint="0,0" EndPoint="1,0">
                                            <GradientStop Color="#FF8844" Offset="0" />
                                            <GradientStop Color="#FFD644" Offset="1" />
                                        </LinearGradientBrush>
                                    </Path.Stroke>
                                    <Path.Data>
                                        <PathGeometry>
                                            <PathFigure StartPoint="17,105">
                                                <ArcSegment Point="143,105" Size="70,70"
                                                            SweepDirection="Clockwise"
                                                            IsLargeArc="True" />
                                            </PathFigure>
                                        </PathGeometry>
                                    </Path.Data>
                                    <Path.Effect>
                                        <BlurEffect Radius="6" />
                                    </Path.Effect>
                                </Path>
                                <!-- ★ 전류 선명 아크 — ConverterParameter 수정 위치 -->
                                <Path StrokeThickness="8"
                                      StrokeStartLineCap="Round" StrokeEndLineCap="Round"
                                      StrokeDashArray="{Binding OutputCurrent,
                                          Converter={StaticResource ValToDash},
                                          ConverterParameter=20}">
                                    <Path.Stroke>
                                        <LinearGradientBrush StartPoint="0,0" EndPoint="1,0">
                                            <GradientStop Color="#FF8844" Offset="0" />
                                            <GradientStop Color="#FFD644" Offset="1" />
                                        </LinearGradientBrush>
                                    </Path.Stroke>
                                    <Path.Data>
                                        <PathGeometry>
                                            <PathFigure StartPoint="17,105">
                                                <ArcSegment Point="143,105" Size="70,70"
                                                            SweepDirection="Clockwise"
                                                            IsLargeArc="True" />
                                            </PathFigure>
                                        </PathGeometry>
                                    </Path.Data>
                                </Path>
                                <TextBlock Canvas.Left="80" Canvas.Top="48"
                                           FontFamily="Consolas" FontSize="34" FontWeight="Bold"
                                           Foreground="#FF8844" TextAlignment="Center"
                                           Text="{Binding OutputCurrent,
                                                  StringFormat={}{0:F2}}">
                                    <TextBlock.RenderTransform>
                                        <TranslateTransform X="-40" />
                                    </TextBlock.RenderTransform>
                                    <TextBlock.Effect>
                                        <DropShadowEffect Color="#FF8844" BlurRadius="12"
                                                          ShadowDepth="0" Opacity="0.5" />
                                    </TextBlock.Effect>
                                </TextBlock>
                                <TextBlock Canvas.Left="80" Canvas.Top="84"
                                           FontFamily="Consolas" FontSize="13"
                                           Foreground="#7A6A4A" TextAlignment="Center"
                                           Text="A">
                                    <TextBlock.RenderTransform>
                                        <TranslateTransform X="-5" />
                                    </TextBlock.RenderTransform>
                                </TextBlock>
                                <TextBlock Canvas.Left="8" Canvas.Top="110"
                                           FontFamily="Consolas" FontSize="9"
                                           Foreground="#5A5A7A" Text="0" />
                                <!-- ★ 전류 눈금 라벨 — 수정 위치 -->
                                <TextBlock Canvas.Left="132" Canvas.Top="110"
                                           FontFamily="Consolas" FontSize="9"
                                           Foreground="#5A5A7A" Text="20" />
                            </Canvas>
                        </Viewbox>
                        <TextBlock Grid.Row="1" Text="OUTPUT CURRENT"
                                   FontFamily="Consolas" FontSize="11" FontWeight="Bold"
                                   Foreground="{StaticResource Subtext0Brush}"
                                   HorizontalAlignment="Center" Margin="0,2,0,6" />
                    </Grid>
                </Border>

                <!-- 운전 상태 -->
                <Border Background="{StaticResource BaseBrush}" CornerRadius="6"
                        Padding="8" Margin="5,0,0,0"
                        BorderBrush="{StaticResource Surface1Brush}" BorderThickness="1">
                    <Grid>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto" />
                            <RowDefinition Height="Auto" />
                        </Grid.RowDefinitions>
                        <Viewbox Grid.Row="0" Height="140" Margin="0,4,0,0">
                            <Canvas Width="160" Height="130">
                                <Ellipse Canvas.Left="48" Canvas.Top="16" Width="64" Height="64"
                                         Stroke="{StaticResource Surface2Brush}" StrokeThickness="3"
                                         Fill="Transparent" />
                                <Ellipse Canvas.Left="56" Canvas.Top="24" Width="48" Height="48"
                                         Fill="{Binding CurrentState,
                                                Converter={StaticResource StateToColor}}">
                                    <Ellipse.Effect>
                                        <DropShadowEffect ShadowDepth="0" BlurRadius="20"
                                                          Opacity="0.7" />
                                    </Ellipse.Effect>
                                </Ellipse>
                                <TextBlock Canvas.Left="106" Canvas.Top="101"
                                           FontFamily="Consolas" FontSize="16" FontWeight="Bold"
                                           Foreground="{StaticResource TextBrush}"
                                           TextAlignment="Center"
                                           Text="{Binding StateDisplayText}">
                                    <TextBlock.RenderTransform>
                                        <TranslateTransform X="-50" />
                                    </TextBlock.RenderTransform>
                                </TextBlock>
                            </Canvas>
                        </Viewbox>
                        <TextBlock Grid.Row="1" Text="DRIVE STATUS"
                                   FontFamily="Consolas" FontSize="11" FontWeight="Bold"
                                   Foreground="{StaticResource Subtext0Brush}"
                                   HorizontalAlignment="Center" Margin="0,2,0,6" />
                    </Grid>
                </Border>

            </UniformGrid>
        </Border>

        <!-- ROW 3 : 제어 패널 -->
        <Border Grid.Row="3" Background="{StaticResource MantleBrush}"
                CornerRadius="6" Padding="14,10" Margin="0,0,0,8"
                BorderBrush="{StaticResource Surface1Brush}" BorderThickness="1">
            <DockPanel>
                <StackPanel DockPanel.Dock="Right" Orientation="Horizontal"
                            VerticalAlignment="Center">
                    <TextBlock Text="FREQ" Foreground="{StaticResource Subtext0Brush}"
                               FontFamily="Consolas" FontSize="12" FontWeight="Bold"
                               VerticalAlignment="Center" Margin="0,0,6,0" />
                    <TextBox Text="{Binding TargetFrequency,
                                   UpdateSourceTrigger=PropertyChanged,
                                   StringFormat=F1}"
                             Style="{StaticResource HiTechTextBox}"
                             Width="100" Height="44" FontSize="18"
                             TextAlignment="Center" Padding="6,4" />
                    <TextBlock Text="Hz" Foreground="{StaticResource Subtext0Brush}"
                               FontFamily="Consolas" FontSize="12" FontWeight="Bold"
                               VerticalAlignment="Center" Margin="6,0,10,0" />
                    <Button Content="SET FREQ" Command="{Binding SetFrequencyCommand}"
                            Style="{StaticResource HiTechButton}"
                            Width="110" Height="44" FontSize="13"
                            Background="{StaticResource MagentaBrush}" />
                </StackPanel>
                <StackPanel Orientation="Horizontal" VerticalAlignment="Center">
                    <Button Content="▶ FWD RUN" Command="{Binding RunForwardCommand}"
                            Style="{StaticResource HiTechButton}" Width="110"
                            Background="{StaticResource GreenBrush}" Margin="0,0,6,0" />
                    <Button Content="◀ REV RUN" Command="{Binding RunReverseCommand}"
                            Style="{StaticResource HiTechButton}" Width="110"
                            Background="{StaticResource BlueBrush}" Margin="0,0,6,0" />
                    <Button Content="■ STOP" Command="{Binding StopCommand}"
                            Style="{StaticResource HiTechButton}" Width="90"
                            Background="{StaticResource RedBrush}" />
                </StackPanel>
            </DockPanel>
        </Border>

        <!-- ROW 4 : 통신 로그 -->
        <Border Grid.Row="4" Background="{StaticResource MantleBrush}"
                CornerRadius="6" Padding="10"
                BorderBrush="{StaticResource Surface1Brush}" BorderThickness="1">
            <DockPanel>
                <DockPanel DockPanel.Dock="Top" Margin="0,0,0,6">
                    <Button DockPanel.Dock="Right" Content="CLEAR"
                            Command="{Binding ClearLogCommand}"
                            Style="{StaticResource HiTechButton}"
                            Width="60" Height="24" FontSize="10"
                            Background="{StaticResource Surface2Brush}"
                            Foreground="{StaticResource TextBrush}" />
                    <TextBlock Text="COMM LOG"
                               Foreground="{StaticResource Subtext0Brush}"
                               FontFamily="Consolas" FontSize="11" FontWeight="Bold"
                               VerticalAlignment="Center" />
                </DockPanel>
                <Border Background="{StaticResource CrustBrush}" CornerRadius="4"
                        BorderBrush="{StaticResource Surface1Brush}" BorderThickness="1">
                    <ListBox ItemsSource="{Binding LogMessages}"
                             Background="Transparent" Foreground="#5AE0B0"
                             BorderThickness="0" FontFamily="Consolas" FontSize="11"
                             ScrollViewer.HorizontalScrollBarVisibility="Auto"
                             x:Name="LogListBox" Padding="4">
                        <ListBox.ItemContainerStyle>
                            <Style TargetType="ListBoxItem">
                                <Setter Property="Padding" Value="6,1" />
                                <Setter Property="Background" Value="Transparent" />
                                <Setter Property="BorderThickness" Value="0" />
                                <Setter Property="Foreground" Value="#5AE0B0" />
                                <Setter Property="Template">
                                    <Setter.Value>
                                        <ControlTemplate TargetType="ListBoxItem">
                                            <Border Padding="{TemplateBinding Padding}"
                                                    Background="{TemplateBinding Background}">
                                                <ContentPresenter />
                                            </Border>
                                        </ControlTemplate>
                                    </Setter.Value>
                                </Setter>
                            </Style>
                        </ListBox.ItemContainerStyle>
                    </ListBox>
                </Border>
            </DockPanel>
        </Border>

    </Grid>
</Window>
```

---

### 5.5 MainWindow.xaml.cs

```csharp
using System.Collections.Specialized;
using System.Windows;
using LSG100_InverterControl.ViewModels;

namespace LSG100_InverterControl
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (DataContext is MainViewModel vm)
                vm.LogMessages.CollectionChanged += LogMessages_CollectionChanged;
        }

        private void LogMessages_CollectionChanged(object? sender,
            NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && LogListBox.Items.Count > 0)
                LogListBox.ScrollIntoView(LogListBox.Items[^1]);
        }

        private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is MainViewModel vm) vm.Dispose();
        }
    }
}
```

---

### 5.6 Models/InverterModel.cs

```csharp
namespace LSG100_InverterControl.Models
{
    public static class InverterRegisters
    {
        // ===== 쓰기 레지스터 =====
        public const ushort REG_FREQ_SET = 0x0004;   // 주파수 설정 (0.01Hz 단위)
        public const ushort REG_CMD      = 0x0005;   // 운전 명령

        // ===== 읽기 레지스터 (모니터링) =====
        public const ushort REG_STATUS   = 0x0008;   // 인버터 상태
        public const ushort REG_FREQ_OUT = 0x0009;   // 출력 주파수 (0.01Hz 단위)
        public const ushort REG_CURR_OUT = 0x000A;   // 출력 전류 (0.01A 단위)

        // ===== 운전 명령 비트값 =====
        public const ushort CMD_STOP = 0x0001;
        public const ushort CMD_FWD  = 0x0002;
        public const ushort CMD_REV  = 0x0004;
    }

    public enum InverterState
    {
        Disconnected,
        Stopped,
        RunningForward,
        RunningReverse,
        Error
    }
}
```

---

### 5.7 Services/ModbusRtuService.cs

```csharp
using System;
using System.IO.Ports;
using System.Threading;

namespace LSG100_InverterControl.Services
{
    public class ModbusRtuService : IDisposable
    {
        private SerialPort? _serial;
        private readonly object _lock = new();
        private bool _disposed;

        public bool IsConnected => _serial?.IsOpen == true;
        public event Action<string>? LogReceived;

        public bool Connect(string portName, int baudRate, byte dataBits = 8,
                            Parity parity = Parity.None, StopBits stopBits = StopBits.One)
        {
            try
            {
                Disconnect();
                _serial = new SerialPort
                {
                    PortName = portName, BaudRate = baudRate, DataBits = dataBits,
                    Parity = parity, StopBits = stopBits,
                    ReadTimeout = 1000, WriteTimeout = 1000
                };
                _serial.Open();
                _serial.DiscardInBuffer();
                _serial.DiscardOutBuffer();
                Log($"[연결 성공] {portName} / {baudRate}bps");
                return true;
            }
            catch (Exception ex) { Log($"[연결 실패] {ex.Message}"); return false; }
        }

        public void Disconnect()
        {
            if (_serial?.IsOpen == true)
            { try { _serial.Close(); Log("[연결 해제]"); } catch { } }
            _serial?.Dispose(); _serial = null;
        }

        public bool WriteSingleRegister(byte slaveId, ushort register, ushort value)
        {
            lock (_lock)
            {
                if (!IsConnected) return false;
                try
                {
                    byte[] frame = new byte[6];
                    frame[0] = slaveId; frame[1] = 0x06;
                    frame[2] = (byte)(register >> 8); frame[3] = (byte)(register & 0xFF);
                    frame[4] = (byte)(value >> 8); frame[5] = (byte)(value & 0xFF);
                    byte[] crc = CalculateCRC16(frame);
                    byte[] msg = new byte[8];
                    Array.Copy(frame, msg, 6); msg[6] = crc[0]; msg[7] = crc[1];

                    _serial!.DiscardInBuffer();
                    _serial.Write(msg, 0, msg.Length);
                    Log($"  TX: {BitConverter.ToString(msg).Replace("-", " ")}");

                    Thread.Sleep(50);
                    byte[] resp = new byte[8];
                    int bytesRead = 0, timeout = 0;
                    while (bytesRead < 8 && timeout < 20)
                    {
                        if (_serial.BytesToRead > 0)
                            bytesRead += _serial.Read(resp, bytesRead, 8 - bytesRead);
                        else { Thread.Sleep(50); timeout++; }
                    }
                    if (bytesRead == 8)
                    {
                        Log($"  RX: {BitConverter.ToString(resp, 0, bytesRead).Replace("-", " ")}");
                        bool match = true;
                        for (int i = 0; i < 8; i++)
                            if (msg[i] != resp[i]) { match = false; break; }
                        Log(match ? "  ✓ 응답 일치" : "  ✗ 응답 불일치");
                        return match;
                    }
                    Log($"  응답 부족 ({bytesRead}바이트)"); return false;
                }
                catch (Exception ex) { Log($"  [쓰기 오류] {ex.Message}"); return false; }
            }
        }

        public ushort[]? ReadHoldingRegisters(byte slaveId, ushort startRegister, ushort quantity)
        {
            lock (_lock)
            {
                if (!IsConnected) return null;
                try
                {
                    byte[] frame = new byte[6];
                    frame[0] = slaveId; frame[1] = 0x03;
                    frame[2] = (byte)(startRegister >> 8);
                    frame[3] = (byte)(startRegister & 0xFF);
                    frame[4] = (byte)(quantity >> 8);
                    frame[5] = (byte)(quantity & 0xFF);
                    byte[] crc = CalculateCRC16(frame);
                    byte[] msg = new byte[8];
                    Array.Copy(frame, msg, 6); msg[6] = crc[0]; msg[7] = crc[1];

                    _serial!.DiscardInBuffer();
                    _serial.Write(msg, 0, msg.Length);

                    Thread.Sleep(50);
                    int expectedBytes = 3 + (quantity * 2) + 2;
                    byte[] resp = new byte[expectedBytes];
                    int bytesRead = 0, timeout = 0;
                    while (bytesRead < expectedBytes && timeout < 20)
                    {
                        if (_serial.BytesToRead > 0)
                            bytesRead += _serial.Read(resp, bytesRead, expectedBytes - bytesRead);
                        else { Thread.Sleep(50); timeout++; }
                    }
                    if (bytesRead < expectedBytes) return null;

                    byte[] respData = new byte[bytesRead - 2];
                    Array.Copy(resp, respData, respData.Length);
                    byte[] respCrc = CalculateCRC16(respData);
                    if (resp[bytesRead - 2] != respCrc[0] || resp[bytesRead - 1] != respCrc[1])
                    { Log("  [읽기] CRC 오류"); return null; }

                    ushort[] values = new ushort[quantity];
                    for (int i = 0; i < quantity; i++)
                        values[i] = (ushort)((resp[3 + i * 2] << 8) | resp[4 + i * 2]);
                    return values;
                }
                catch (Exception ex) { Log($"  [읽기 오류] {ex.Message}"); return null; }
            }
        }

        private static byte[] CalculateCRC16(byte[] data)
        {
            ushort crc = 0xFFFF;
            foreach (byte b in data)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                    crc = (crc & 0x0001) != 0
                        ? (ushort)((crc >> 1) ^ 0xA001)
                        : (ushort)(crc >> 1);
            }
            return new byte[] { (byte)(crc & 0xFF), (byte)(crc >> 8) };
        }

        public static string[] GetAvailablePorts() => SerialPort.GetPortNames();

        private void Log(string message) =>
            LogReceived?.Invoke($"[{DateTime.Now:HH:mm:ss.fff}] {message}");

        public void Dispose()
        {
            if (!_disposed) { Disconnect(); _disposed = true; }
            GC.SuppressFinalize(this);
        }
    }
}
```

---

### 5.8 ViewModels/RelayCommand.cs

```csharp
using System;
using System.Windows.Input;

namespace LSG100_InverterControl.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
            : this(_ => execute(), canExecute != null ? _ => canExecute() : null) { }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => _execute(parameter);
        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}
```

---

### 5.9 ViewModels/MainViewModel.cs

```csharp
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using LSG100_InverterControl.Models;
using LSG100_InverterControl.Services;

namespace LSG100_InverterControl.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly ModbusRtuService _modbus;
        private readonly DispatcherTimer _monitorTimer;
        private bool _disposed;

        public MainViewModel()
        {
            _modbus = new ModbusRtuService();
            _modbus.LogReceived += OnLogReceived;

            _monitorTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _monitorTimer.Tick += MonitorTimer_Tick;

            ConnectCommand = new RelayCommand(ExecuteConnect, () => !IsConnected);
            DisconnectCommand = new RelayCommand(ExecuteDisconnect, () => IsConnected);
            RunForwardCommand = new RelayCommand(ExecuteRunForward, () => IsConnected);
            RunReverseCommand = new RelayCommand(ExecuteRunReverse, () => IsConnected);
            StopCommand = new RelayCommand(ExecuteStop, () => IsConnected);
            SetFrequencyCommand = new RelayCommand(ExecuteSetFrequency, () => IsConnected);
            ClearLogCommand = new RelayCommand(() => LogMessages.Clear());

            RefreshPorts();
            SelectedBaudRate = 9600;
            SlaveId = 1;
            TargetFrequency = 30.0;
        }

        #region 연결 설정 프로퍼티

        private string[] _availablePorts = Array.Empty<string>();
        public string[] AvailablePorts
        {
            get => _availablePorts;
            set => SetField(ref _availablePorts, value);
        }

        private string? _selectedPort;
        public string? SelectedPort
        {
            get => _selectedPort;
            set => SetField(ref _selectedPort, value);
        }

        private int _selectedBaudRate;
        public int SelectedBaudRate
        {
            get => _selectedBaudRate;
            set => SetField(ref _selectedBaudRate, value);
        }

        public int[] BaudRates { get; } = { 4800, 9600, 19200, 38400, 115200 };

        private byte _slaveId;
        public byte SlaveId
        {
            get => _slaveId;
            set => SetField(ref _slaveId, value);
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (SetField(ref _isConnected, value))
                    OnPropertyChanged(nameof(ConnectionStatusText));
            }
        }

        public string ConnectionStatusText => IsConnected ? "연결됨" : "미연결";

        #endregion

        #region 모니터링 프로퍼티

        private double _outputFrequency;
        public double OutputFrequency
        {
            get => _outputFrequency;
            set => SetField(ref _outputFrequency, value);
        }

        private double _outputCurrent;
        public double OutputCurrent
        {
            get => _outputCurrent;
            set => SetField(ref _outputCurrent, value);
        }

        private double _targetFrequency;
        public double TargetFrequency
        {
            get => _targetFrequency;
            set => SetField(ref _targetFrequency, Math.Clamp(value, 0.0, 60.0));
        }

        private InverterState _currentState = InverterState.Disconnected;
        public InverterState CurrentState
        {
            get => _currentState;
            set
            {
                if (SetField(ref _currentState, value))
                {
                    OnPropertyChanged(nameof(StateDisplayText));
                    OnPropertyChanged(nameof(IsRunning));
                }
            }
        }

        public string StateDisplayText => CurrentState switch
        {
            InverterState.Disconnected => "미연결",
            InverterState.Stopped      => "정 지",
            InverterState.RunningForward => "정방향",
            InverterState.RunningReverse => "역방향",
            InverterState.Error        => "이 상",
            _ => "알 수 없음"
        };

        public bool IsRunning =>
            CurrentState is InverterState.RunningForward or InverterState.RunningReverse;

        #endregion

        public ObservableCollection<string> LogMessages { get; } = new();

        public RelayCommand ConnectCommand { get; }
        public RelayCommand DisconnectCommand { get; }
        public RelayCommand RunForwardCommand { get; }
        public RelayCommand RunReverseCommand { get; }
        public RelayCommand StopCommand { get; }
        public RelayCommand SetFrequencyCommand { get; }
        public RelayCommand ClearLogCommand { get; }

        private void ExecuteConnect()
        {
            if (string.IsNullOrEmpty(SelectedPort))
            { AddLog("COM 포트를 선택하세요."); return; }

            bool ok = _modbus.Connect(SelectedPort, SelectedBaudRate);
            IsConnected = ok;
            if (ok)
            {
                CurrentState = InverterState.Stopped;
                _monitorTimer.Start();
                AddLog($"인버터 연결 성공 - {SelectedPort} / {SelectedBaudRate}bps / ID={SlaveId}");
            }
            else CurrentState = InverterState.Disconnected;
        }

        private void ExecuteDisconnect()
        {
            _monitorTimer.Stop();
            _modbus.WriteSingleRegister(SlaveId, InverterRegisters.REG_CMD,
                                        InverterRegisters.CMD_STOP);
            _modbus.Disconnect();
            IsConnected = false;
            CurrentState = InverterState.Disconnected;
            OutputFrequency = 0;
            OutputCurrent = 0;
            AddLog("인버터 연결 해제 (STOP 전송 후 종료)");
        }

        private void ExecuteRunForward()
        {
            AddLog("[정방향 RUN]");
            if (_modbus.WriteSingleRegister(SlaveId, InverterRegisters.REG_CMD,
                                            InverterRegisters.CMD_FWD))
                CurrentState = InverterState.RunningForward;
        }

        private void ExecuteRunReverse()
        {
            AddLog("[역방향 RUN]");
            if (_modbus.WriteSingleRegister(SlaveId, InverterRegisters.REG_CMD,
                                            InverterRegisters.CMD_REV))
                CurrentState = InverterState.RunningReverse;
        }

        private void ExecuteStop()
        {
            AddLog("[STOP]");
            if (_modbus.WriteSingleRegister(SlaveId, InverterRegisters.REG_CMD,
                                            InverterRegisters.CMD_STOP))
                CurrentState = InverterState.Stopped;
        }

        private void ExecuteSetFrequency()
        {
            ushort rawValue = (ushort)(TargetFrequency * 100);
            AddLog($"[주파수 설정] {TargetFrequency:F2} Hz (raw={rawValue})");
            _modbus.WriteSingleRegister(SlaveId, InverterRegisters.REG_FREQ_SET, rawValue);
        }

        private void MonitorTimer_Tick(object? sender, EventArgs e)
        {
            if (!IsConnected) return;
            try
            {
                // 출력 주파수 (0x0009, 0.01Hz 단위)
                var freqData = _modbus.ReadHoldingRegisters(
                    SlaveId, InverterRegisters.REG_FREQ_OUT, 1);
                if (freqData is { Length: > 0 })
                    OutputFrequency = freqData[0] / 100.0;

                // 출력 전류 (0x000A, 0.01A 단위)
                // ※ 실제 단위는 인버터에 따라 다를 수 있음 → 실측 후 조정
                var currData = _modbus.ReadHoldingRegisters(
                    SlaveId, InverterRegisters.REG_CURR_OUT, 1);
                if (currData is { Length: > 0 })
                    OutputCurrent = currData[0] / 1000.0;
            }
            catch (Exception ex) { AddLog($"[모니터링 오류] {ex.Message}"); }
        }

        public void RefreshPorts()
        {
            AvailablePorts = ModbusRtuService.GetAvailablePorts();
            if (AvailablePorts.Length > 0 && SelectedPort == null)
                SelectedPort = AvailablePorts[0];
        }

        private void OnLogReceived(string message)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                LogMessages.Add(message);
                while (LogMessages.Count > 500)
                    LogMessages.RemoveAt(0);
            });
        }

        private void AddLog(string msg) =>
            OnLogReceived($"[{DateTime.Now:HH:mm:ss.fff}] {msg}");

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? n = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? n = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(n);
            return true;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _monitorTimer.Stop();
                _modbus.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
```

---

### 5.10 Converters/BoolToColorConverter.cs

```csharp
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LSG100_InverterControl.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c) =>
            value is bool b && b
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A6E3A1"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F38BA8"));
        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotSupportedException();
    }

    public class StateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c)
        {
            if (value is Models.InverterState s)
                return s switch
                {
                    Models.InverterState.RunningForward =>
                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A6E3A1")),
                    Models.InverterState.RunningReverse =>
                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#89B4FA")),
                    Models.InverterState.Stopped =>
                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F9E2AF")),
                    Models.InverterState.Error =>
                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F38BA8")),
                    _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6C7086"))
                };
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6C7086"));
        }
        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotSupportedException();
    }

    public class FrequencyToWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type t, object p, CultureInfo c)
        {
            if (values.Length == 2 && values[0] is double freq
                                   && values[1] is double w && w > 0)
                return Math.Clamp(freq / 60.0, 0, 1) * w;
            return 0.0;
        }
        public object[] ConvertBack(object v, Type[] t, object p, CultureInfo c) =>
            throw new NotSupportedException();
    }

    /// <summary>
    /// 값(0~Max) → 아크 각도(도) 변환.
    /// ConverterParameter="최대값" (기본 60). 270도 아크 기준.
    /// </summary>
    public class ValueToAngleConverter : IValueConverter
    {
        public object Convert(object value, Type t, object parameter, CultureInfo c)
        {
            double val = value is double d ? d : 0;
            double max = 60;
            if (parameter is string s && double.TryParse(s, out double m)) max = m;
            double ratio = Math.Clamp(val / max, 0, 1);
            return ratio * 270.0;
        }
        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotSupportedException();
    }

    /// <summary>
    /// 값(0~Max) → StrokeDashArray 오프셋 변환. 아크 게이지용.
    ///
    /// ConverterParameter 형식:
    ///   "최대값"           → 예: "60"       (보정값은 기본 35.5)
    ///   "최대값,보정값"     → 예: "60,35.5"  (직접 지정)
    ///
    /// 보정값(totalDashUnits)을 조절하면 아크 채움 비율이 변합니다.
    ///   - 값을 줄이면 → 아크가 더 많이 채워짐
    ///   - 값을 늘리면 → 아크가 덜 채워짐
    /// </summary>
    public class ValueToStrokeDashConverter : IValueConverter
    {
        public object Convert(object value, Type t, object parameter, CultureInfo c)
        {
            double val = value is double d ? d : 0;
            double max = 60;
            double totalDashUnits = 35.5; // 기본 보정값

            // ConverterParameter 파싱: "최대값" 또는 "최대값,보정값"
            if (parameter is string s)
            {
                string[] parts = s.Split(',');
                if (parts.Length >= 1 && double.TryParse(parts[0].Trim(), out double m))
                    max = m;
                if (parts.Length >= 2 && double.TryParse(parts[1].Trim(), out double dash))
                    totalDashUnits = dash;
            }

            double ratio = Math.Clamp(val / max, 0, 1);
            double filled = ratio * totalDashUnits;
            double gap = totalDashUnits - filled + 5;

            return new DoubleCollection { filled, gap };
        }
        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotSupportedException();
    }
}
```

---

## 6. 빌드 및 실행 방법

1. Visual Studio 2022에서 **새 WPF 프로젝트** 생성 (.NET 8.0)
2. 프로젝트명: `LSG100_InverterControl`
3. NuGet 패키지 관리자에서 `System.IO.Ports` 설치
4. 위 소스코드를 각 파일 경로에 배치
5. **빌드(Ctrl+B)** → **실행(F5)**
6. COM 포트 선택 → CONNECT → 인버터 제어

---

## 7. 주의사항

- 전류 단위: 현재 `/ 1000.0`으로 변환 중. 실측 후 `/ 100.0` 또는 `/ 10.0`으로 조정 필요
- 전류 게이지의 글로우/선명 아크 `ConverterParameter`가 현재 불일치 상태 (`0.23` vs `20`). 같은 값으로 통일 필요
- 레지스터 주소는 LS G100 매뉴얼 7.5~7.6절에서 최종 확인 권장
- 인버터 파라미터 `dr.91`(통신 프로토콜), `dr.92`(Slave ID), `dr.93`(통신 속도) 사전 설정 필요
