## C#의 정렬 알고리즘은 기본적으로 Intro Sort(Quick Sort와 Heap Sort를 적절히 혼용)를 사용함

---
어떤 기술에 대한 자세한 내용은 그 기술을 만든 회사의 공식 문서에서 확인하는 것이 가장 좋음
---

https://learn.microsoft.com/ko-kr/dotnet/api/system.array.sort?view=net-8.0

---
## QuickSortVisualizer


https://github.com/user-attachments/assets/474ab039-bf91-4388-ae8e-f0b633bff754


### WPF 코드 MainWindow.xaml

```xaml
<Window x:Class="QuickSortVisualizer.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="퀵 정렬 시각화 (Quick Sort Visualizer)" 
        Height="650" Width="920"
        MinHeight="500" MinWidth="700"
        WindowStartupLocation="CenterScreen"
        Background="#1E1E2E">

    <Window.Resources>
        <Style x:Key="BtnStyle" TargetType="Button">
            <Setter Property="FontSize" Value="14"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="Padding" Value="20,10"/>
            <Setter Property="Cursor" Value="Hand"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Border x:Name="border" Background="{TemplateBinding Background}" 
                                CornerRadius="8" Padding="{TemplateBinding Padding}">
                            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter TargetName="border" Property="Opacity" Value="0.85"/>
                            </Trigger>
                            <Trigger Property="IsEnabled" Value="False">
                                <Setter TargetName="border" Property="Opacity" Value="0.4"/>
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
    </Window.Resources>

    <Grid Margin="20">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- 제목 -->
        <TextBlock Grid.Row="0" Text="퀵 정렬 시각화 (Quick Sort)" 
                   FontSize="28" FontWeight="Bold" Foreground="#CDD6F4"
                   HorizontalAlignment="Center" Margin="0,0,0,5"/>

        <!-- 상태 메시지 -->
        <TextBlock Grid.Row="1" x:Name="txtStatus" 
                   Text="▶ 시작 버튼을 눌러주세요" 
                   FontSize="16" Foreground="#A6ADC8"
                   HorizontalAlignment="Center" Margin="0,0,0,5"/>

        <!-- 비교/스왑 카운터 -->
        <StackPanel Grid.Row="2" Orientation="Horizontal" HorizontalAlignment="Center" Margin="0,0,0,10">
            <TextBlock x:Name="txtCompareCount" Text="비교: 0회" 
                       Foreground="#89B4FA" FontSize="14" FontWeight="Bold" Margin="15,0"/>
            <TextBlock x:Name="txtSwapCount" Text="스왑: 0회" 
                       Foreground="#FAB387" FontSize="14" FontWeight="Bold" Margin="15,0"/>
        </StackPanel>

        <!-- 막대 그래프 영역 -->
        <Border Grid.Row="3" Background="#181825" CornerRadius="12" Padding="20">
            <Canvas x:Name="canvas" SizeChanged="Canvas_SizeChanged"/>
        </Border>

        <!-- 범례 -->
        <StackPanel Grid.Row="4" Orientation="Horizontal" HorizontalAlignment="Center" Margin="0,12,0,8">
            <StackPanel Orientation="Horizontal" Margin="10,0">
                <Border Width="16" Height="16" CornerRadius="3" Background="#89B4FA" Margin="0,0,6,0"/>
                <TextBlock Text="기본" Foreground="#A6ADC8" FontSize="13"/>
            </StackPanel>
            <StackPanel Orientation="Horizontal" Margin="10,0">
                <Border Width="16" Height="16" CornerRadius="3" Background="#F38BA8" Margin="0,0,6,0"/>
                <TextBlock Text="피벗" Foreground="#A6ADC8" FontSize="13"/>
            </StackPanel>
            <StackPanel Orientation="Horizontal" Margin="10,0">
                <Border Width="16" Height="16" CornerRadius="3" Background="#FAB387" Margin="0,0,6,0"/>
                <TextBlock Text="비교 중" Foreground="#A6ADC8" FontSize="13"/>
            </StackPanel>
            <StackPanel Orientation="Horizontal" Margin="10,0">
                <Border Width="16" Height="16" CornerRadius="3" Background="#A6E3A1" Margin="0,0,6,0"/>
                <TextBlock Text="정렬 완료" Foreground="#A6ADC8" FontSize="13"/>
            </StackPanel>
        </StackPanel>

        <!-- 버튼 영역 -->
        <StackPanel Grid.Row="5" Orientation="Horizontal" HorizontalAlignment="Center" Margin="0,0,0,5">
            <Button x:Name="btnStart" Content="▶ 정렬 시작" 
                    Background="#89B4FA" Style="{StaticResource BtnStyle}"
                    Click="BtnStart_Click" Margin="5,0"/>
            <Button x:Name="btnPause" Content="⏸ 일시정지" 
                    Background="#FAB387" Style="{StaticResource BtnStyle}"
                    Click="BtnPause_Click" Margin="5,0" IsEnabled="False"/>
            <Button x:Name="btnReset" Content="↻ 초기화" 
                    Background="#F38BA8" Style="{StaticResource BtnStyle}"
                    Click="BtnReset_Click" Margin="5,0"/>

            <TextBlock Text="  속도:" Foreground="#A6ADC8" VerticalAlignment="Center" 
                       FontSize="14" Margin="15,0,5,0"/>
            <Slider x:Name="sliderSpeed" Width="120" Minimum="100" Maximum="2000" Value="700"
                    VerticalAlignment="Center" IsDirectionReversed="True"/>
            <TextBlock x:Name="txtSpeed" Text="700ms" Foreground="#A6ADC8" 
                       VerticalAlignment="Center" FontSize="13" Margin="5,0,0,0"/>
        </StackPanel>
    </Grid>
</Window>
```

### MainWindow.xaml.cs코드

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace QuickSortVisualizer
{
    public partial class MainWindow : Window
    {
        // ── 데이터 ──
        private int[] arr;
        private Rectangle[] bars;
        private TextBlock[] labels;

        // ── 상태 플래그 ──
        private bool isSorting = false;
        private bool isPaused = false;

        // ── 카운터 ──
        private int compareCount = 0;
        private int swapCount = 0;

        // ── 레이아웃 계산용 ──
        private double barGap = 8;
        private double barWidth;
        private double maxBarHeight;
        private double canvasW;
        private double canvasH;

        // ── 색상 정의 (Catppuccin Mocha) ──
        private static readonly SolidColorBrush ColorDefault = NewBrush("#89B4FA");
        private static readonly SolidColorBrush ColorPivot = NewBrush("#F38BA8");
        private static readonly SolidColorBrush ColorCompare = NewBrush("#FAB387");
        private static readonly SolidColorBrush ColorDone = NewBrush("#A6E3A1");
        private static readonly SolidColorBrush ColorSwapA = NewBrush("#CBA6F7"); // 스왑 대상 A (보라)
        private static readonly SolidColorBrush ColorText = NewBrush("#CDD6F4");

        private static SolidColorBrush NewBrush(string hex)
        {
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
            brush.Freeze();
            return brush;
        }

        // =====================================================================
        // 생성자
        // =====================================================================
        public MainWindow()
        {
            InitializeComponent();
            sliderSpeed.ValueChanged += (s, e) => txtSpeed.Text = $"{(int)sliderSpeed.Value}ms";
            Loaded += (s, e) => ResetArray();
        }

        // =====================================================================
        // 배열 초기화 (Fisher-Yates 셔플)
        // =====================================================================
        private void ResetArray()
        {
            arr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            Random rng = new Random();
            for (int i = arr.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }

            compareCount = 0;
            swapCount = 0;
            UpdateCounters();
            DrawBars();
            txtStatus.Text = "▶ 시작 버튼을 눌러주세요";
        }

        // =====================================================================
        // 캔버스 크기 계산
        // =====================================================================
        private void CalcLayout()
        {
            canvasW = canvas.ActualWidth > 0 ? canvas.ActualWidth : 820;
            canvasH = canvas.ActualHeight > 0 ? canvas.ActualHeight : 380;
            barWidth = (canvasW - barGap * (arr.Length + 1)) / arr.Length;
            maxBarHeight = canvasH - 45; // 숫자 라벨 공간
        }

        // =====================================================================
        // 막대 그래프 그리기
        // =====================================================================
        private void DrawBars()
        {
            canvas.Children.Clear();
            bars = new Rectangle[arr.Length];
            labels = new TextBlock[arr.Length];

            CalcLayout();

            for (int i = 0; i < arr.Length; i++)
            {
                double barH = GetBarHeight(arr[i]);
                double leftX = GetLeftX(i);

                // 막대
                var rect = new Rectangle
                {
                    Width = barWidth,
                    Height = barH,
                    Fill = ColorDefault,
                    RadiusX = 6,
                    RadiusY = 6
                };
                Canvas.SetLeft(rect, leftX);
                Canvas.SetTop(rect, canvasH - 40 - barH);
                canvas.Children.Add(rect);
                bars[i] = rect;

                // 숫자 라벨
                var tb = new TextBlock
                {
                    Text = arr[i].ToString(),
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Foreground = ColorText,
                    TextAlignment = TextAlignment.Center,
                    Width = barWidth
                };
                Canvas.SetLeft(tb, leftX);
                Canvas.SetTop(tb, canvasH - 32);
                canvas.Children.Add(tb);
                labels[i] = tb;
            }
        }

        private double GetBarHeight(int value)
        {
            // 0~9 → 최소 20px, 최대 maxBarHeight
            return Math.Max(20, (value + 1) * (maxBarHeight / 10.5));
        }

        private double GetLeftX(int index)
        {
            return barGap + index * (barWidth + barGap);
        }

        // =====================================================================
        // 캔버스 리사이즈 대응
        // =====================================================================
        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (arr != null && bars != null && !isSorting)
                DrawBars();
        }

        // =====================================================================
        // 색상 헬퍼
        // =====================================================================
        private void SetBarColor(int index, SolidColorBrush color)
        {
            if (index >= 0 && index < bars.Length)
                bars[index].Fill = color;
        }

        private void ResetRangeColor(int left, int right, int pivotIdx = -1)
        {
            for (int k = left; k <= right; k++)
            {
                if (k == pivotIdx)
                    bars[k].Fill = ColorPivot;
                else
                    bars[k].Fill = ColorDefault;
            }
        }

        // =====================================================================
        // 카운터 업데이트
        // =====================================================================
        private void UpdateCounters()
        {
            txtCompareCount.Text = $"비교: {compareCount}회";
            txtSwapCount.Text = $"스왑: {swapCount}회";
        }

        // =====================================================================
        // 막대 스왑 (애니메이션 포함)
        // =====================================================================
        private void SwapBars(int idxA, int idxB)
        {
            if (idxA == idxB) return;

            swapCount++;
            UpdateCounters();

            // 1) 데이터 교환
            (arr[idxA], arr[idxB]) = (arr[idxB], arr[idxA]);

            // 2) 목표 위치 계산 (인덱스 기반 - 항상 정확)
            double targetLeftA = GetLeftX(idxA);
            double targetLeftB = GetLeftX(idxB);

            // 3) 현재 실제 위치 가져오기 (애니메이션 클리어 후)
            double curLeftA = Canvas.GetLeft(bars[idxA]);
            double curLeftB = Canvas.GetLeft(bars[idxB]);

            // 4) 참조 교환 (bars, labels 배열)
            (bars[idxA], bars[idxB]) = (bars[idxB], bars[idxA]);
            (labels[idxA], labels[idxB]) = (labels[idxB], labels[idxA]);

            // 5) 교환 후: bars[idxA]는 원래 idxB에 있던 막대 → targetLeftA로 이동
            //             bars[idxB]는 원래 idxA에 있던 막대 → targetLeftB로 이동
            AnimateMove(bars[idxA], curLeftB, targetLeftA);
            AnimateMove(bars[idxB], curLeftA, targetLeftB);

            // 6) 라벨도 이동
            AnimateMove(labels[idxA], curLeftB, targetLeftA);
            AnimateMove(labels[idxB], curLeftA, targetLeftB);

            // 7) 라벨 텍스트 갱신
            labels[idxA].Text = arr[idxA].ToString();
            labels[idxB].Text = arr[idxB].ToString();

            // 8) 막대 높이 및 Y위치 재계산
            UpdateBarHeight(idxA);
            UpdateBarHeight(idxB);
        }

        private void UpdateBarHeight(int idx)
        {
            double h = GetBarHeight(arr[idx]);
            bars[idx].Height = h;
            Canvas.SetTop(bars[idx], canvasH - 40 - h);
        }

        private void AnimateMove(FrameworkElement elem, double from, double to)
        {
            int animMs = Math.Min(250, DelayMs / 2);
            var anim = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(animMs),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
                FillBehavior = FillBehavior.Stop  // 애니메이션 끝나면 해제
            };

            // 애니메이션 종료 후 확정값 설정 (위치 밀림 방지)
            anim.Completed += (s, e) => Canvas.SetLeft(elem, to);
            elem.BeginAnimation(Canvas.LeftProperty, anim);
        }

        // =====================================================================
        // 딜레이 (일시정지 지원)
        // =====================================================================
        private int DelayMs => (int)sliderSpeed.Value;

        private async Task DelayWithPause(int ms)
        {
            // 일시정지 상태이면 풀릴 때까지 대기
            while (isPaused)
                await Task.Delay(50);

            await Task.Delay(ms);
        }

        // =====================================================================
        // 퀵 정렬 (비동기)
        // =====================================================================
        private async Task QuickSort(int left, int right)
        {
            if (left >= right) return;

            int pivotIndex = await Partition(left, right);

            await QuickSort(left, pivotIndex - 1);
            await QuickSort(pivotIndex + 1, right);
        }

        // =====================================================================
        // 파티션 (시각화 포함)
        // =====================================================================
        private async Task<int> Partition(int left, int right)
        {
            int pivot = arr[right];
            int i = left - 1;

            // 피벗 표시
            SetBarColor(right, ColorPivot);
            txtStatus.Text = $"피벗(pivot) = {pivot}  |  범위: [{left}] ~ [{right}]";
            await DelayWithPause(DelayMs);

            for (int j = left; j < right; j++)
            {
                compareCount++;
                UpdateCounters();

                // 비교 대상 표시
                SetBarColor(j, ColorCompare);

                bool willSwap = arr[j] <= pivot;
                txtStatus.Text = $"비교: arr[{j}]={arr[j]}  vs  피벗={pivot}   →  {(willSwap ? "스왑!" : "유지")}";
                await DelayWithPause(DelayMs);

                if (willSwap)
                {
                    i++;
                    if (i != j)
                    {
                        // 스왑 대상 표시
                        SetBarColor(i, ColorSwapA);
                        SetBarColor(j, ColorSwapA);
                        await DelayWithPause(DelayMs / 3);

                        SwapBars(i, j);
                        await DelayWithPause(DelayMs / 2);
                    }
                }

                // 비교 후 색상 복원 (피벗 유지)
                ResetRangeColor(left, right - 1);
                SetBarColor(right, ColorPivot);
            }

            // 피벗을 올바른 위치로 이동
            if (i + 1 != right)
            {
                SetBarColor(i + 1, ColorSwapA);
                SetBarColor(right, ColorSwapA);
                await DelayWithPause(DelayMs / 3);

                SwapBars(i + 1, right);
            }

            txtStatus.Text = $"피벗 {pivot} → 위치 [{i + 1}]에 배치 완료";

            // 색상 복원
            ResetRangeColor(left, right);

            // 피벗 자리 정렬 완료 표시
            SetBarColor(i + 1, ColorDone);
            await DelayWithPause(DelayMs);

            return i + 1;
        }

        // =====================================================================
        // 버튼 이벤트
        // =====================================================================
        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (isSorting) return;
            isSorting = true;
            isPaused = false;
            btnStart.IsEnabled = false;
            btnPause.IsEnabled = true;
            btnReset.IsEnabled = false;

            CalcLayout(); // 현재 캔버스 크기 확정

            await QuickSort(0, arr.Length - 1);

            // 정렬 완료 - 순차적으로 초록색 변경
            for (int i = 0; i < bars.Length; i++)
            {
                SetBarColor(i, ColorDone);
                await Task.Delay(80);
            }

            txtStatus.Text = $"✔ 정렬 완료!  (비교 {compareCount}회, 스왑 {swapCount}회)";
            isSorting = false;
            isPaused = false;
            btnPause.IsEnabled = false;
            btnPause.Content = "⏸ 일시정지";
            btnReset.IsEnabled = true;
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            if (!isSorting) return;

            isPaused = !isPaused;
            btnPause.Content = isPaused ? "▶ 재개" : "⏸ 일시정지";

            if (isPaused)
                txtStatus.Text = "⏸ 일시정지 중...  (재개 버튼을 누르세요)";
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (isSorting) return;
            btnStart.IsEnabled = true;
            btnPause.IsEnabled = false;
            btnPause.Content = "⏸ 일시정지";
            ResetArray();
        }
    }
}
```




## HeapSortVisualizer


https://github.com/user-attachments/assets/47bf7f6e-49ff-4a3e-bfb6-48dc163033ba


### WPF 코드 MainWindow.xaml
```xaml
<Window x:Class="HeapSortVisualizer.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="힙 정렬 시각화 (Heap Sort Visualizer)" 
        Height="820" Width="1000"
        MinHeight="700" MinWidth="850"
        WindowStartupLocation="CenterScreen"
        Background="#1E1E2E">

    <Window.Resources>
        <Style x:Key="BtnStyle" TargetType="Button">
            <Setter Property="FontSize" Value="14"/>
            <Setter Property="FontWeight" Value="Bold"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="Padding" Value="20,10"/>
            <Setter Property="Cursor" Value="Hand"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Border x:Name="border" Background="{TemplateBinding Background}" 
                                CornerRadius="8" Padding="{TemplateBinding Padding}">
                            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter TargetName="border" Property="Opacity" Value="0.85"/>
                            </Trigger>
                            <Trigger Property="IsEnabled" Value="False">
                                <Setter TargetName="border" Property="Opacity" Value="0.4"/>
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
    </Window.Resources>

    <Grid Margin="15">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="3*"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="2*"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- 제목 -->
        <TextBlock Grid.Row="0" Text="힙 정렬 시각화 (Heap Sort)" 
                   FontSize="26" FontWeight="Bold" Foreground="#CDD6F4"
                   HorizontalAlignment="Center" Margin="0,0,0,3"/>

        <!-- 상태 메시지 -->
        <TextBlock Grid.Row="1" x:Name="txtStatus" 
                   Text="▶ 시작 버튼을 눌러주세요" 
                   FontSize="15" Foreground="#A6ADC8"
                   HorizontalAlignment="Center" Margin="0,0,0,3"/>

        <!-- 카운터 -->
        <StackPanel Grid.Row="2" Orientation="Horizontal" HorizontalAlignment="Center" Margin="0,0,0,8">
            <TextBlock x:Name="txtPhase" Text="단계: 대기" 
                       Foreground="#F5C2E7" FontSize="13" FontWeight="Bold" Margin="12,0"/>
            <TextBlock x:Name="txtCompareCount" Text="비교: 0회" 
                       Foreground="#89B4FA" FontSize="13" FontWeight="Bold" Margin="12,0"/>
            <TextBlock x:Name="txtSwapCount" Text="스왑: 0회" 
                       Foreground="#FAB387" FontSize="13" FontWeight="Bold" Margin="12,0"/>
        </StackPanel>

        <!-- 트리 영역 -->
        <Border Grid.Row="3" Background="#181825" CornerRadius="12" Padding="10" Margin="0,0,0,5">
            <Grid>
                <TextBlock Text="이진 트리 (Heap)" Foreground="#585B70" FontSize="12" 
                           HorizontalAlignment="Left" VerticalAlignment="Top" Margin="5,2,0,0"/>
                <Canvas x:Name="canvasTree"/>
            </Grid>
        </Border>

        <!-- 구분선 -->
        <TextBlock Grid.Row="4" Text="▼ 배열 (Array) ▼" Foreground="#585B70" FontSize="12"
                   HorizontalAlignment="Center" Margin="0,2"/>

        <!-- 막대 그래프 영역 -->
        <Border Grid.Row="5" Background="#181825" CornerRadius="12" Padding="10" Margin="0,0,0,5">
            <Canvas x:Name="canvasBar"/>
        </Border>

        <!-- 범례 -->
        <StackPanel Grid.Row="6" Orientation="Horizontal" HorizontalAlignment="Center" Margin="0,6,0,6">
            <StackPanel Orientation="Horizontal" Margin="8,0">
                <Border Width="14" Height="14" CornerRadius="3" Background="#89B4FA" Margin="0,0,5,0"/>
                <TextBlock Text="기본" Foreground="#A6ADC8" FontSize="12"/>
            </StackPanel>
            <StackPanel Orientation="Horizontal" Margin="8,0">
                <Border Width="14" Height="14" CornerRadius="3" Background="#F38BA8" Margin="0,0,5,0"/>
                <TextBlock Text="부모" Foreground="#A6ADC8" FontSize="12"/>
            </StackPanel>
            <StackPanel Orientation="Horizontal" Margin="8,0">
                <Border Width="14" Height="14" CornerRadius="3" Background="#FAB387" Margin="0,0,5,0"/>
                <TextBlock Text="자식(비교)" Foreground="#A6ADC8" FontSize="12"/>
            </StackPanel>
            <StackPanel Orientation="Horizontal" Margin="8,0">
                <Border Width="14" Height="14" CornerRadius="3" Background="#CBA6F7" Margin="0,0,5,0"/>
                <TextBlock Text="스왑" Foreground="#A6ADC8" FontSize="12"/>
            </StackPanel>
            <StackPanel Orientation="Horizontal" Margin="8,0">
                <Border Width="14" Height="14" CornerRadius="3" Background="#A6E3A1" Margin="0,0,5,0"/>
                <TextBlock Text="정렬완료" Foreground="#A6ADC8" FontSize="12"/>
            </StackPanel>
            <StackPanel Orientation="Horizontal" Margin="8,0">
                <Border Width="14" Height="14" CornerRadius="3" Background="#45475A" Margin="0,0,5,0"/>
                <TextBlock Text="힙에서 제외" Foreground="#A6ADC8" FontSize="12"/>
            </StackPanel>
        </StackPanel>

        <!-- 버튼 -->
        <StackPanel Grid.Row="7" Orientation="Horizontal" HorizontalAlignment="Center" Margin="0,0,0,3">
            <Button x:Name="btnStart" Content="▶ 정렬 시작" 
                    Background="#89B4FA" Style="{StaticResource BtnStyle}"
                    Click="BtnStart_Click" Margin="4,0"/>
            <Button x:Name="btnPause" Content="⏸ 일시정지" 
                    Background="#FAB387" Style="{StaticResource BtnStyle}"
                    Click="BtnPause_Click" Margin="4,0" IsEnabled="False"/>
            <Button x:Name="btnReset" Content="↻ 초기화" 
                    Background="#F38BA8" Style="{StaticResource BtnStyle}"
                    Click="BtnReset_Click" Margin="4,0"/>

            <TextBlock Text="  속도:" Foreground="#A6ADC8" VerticalAlignment="Center" 
                       FontSize="13" Margin="12,0,4,0"/>
            <Slider x:Name="sliderSpeed" Width="110" Minimum="100" Maximum="2000" Value="700"
                    VerticalAlignment="Center" IsDirectionReversed="True"/>
            <TextBlock x:Name="txtSpeed" Text="700ms" Foreground="#A6ADC8" 
                       VerticalAlignment="Center" FontSize="12" Margin="4,0,0,0"/>
        </StackPanel>
    </Grid>
</Window>
```

### MainWindow.xaml.cs코드

```csharp
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HeapSortVisualizer
{
    public partial class MainWindow : Window
    {
        // ── 데이터 ──
        private int[] arr;
        private int heapSize; // 현재 힙에 포함된 원소 수

        // ── 트리 시각 요소 ──
        private Ellipse[] treeNodes;
        private TextBlock[] treeLabels;
        private TextBlock[] treeIndexLabels; // 인덱스 표시
        private Line[] treeLines;            // 부모-자식 연결선
        private double[] nodeX, nodeY;       // 노드 중심 좌표

        // ── 막대 시각 요소 ──
        private Rectangle[] bars;
        private TextBlock[] barLabels;

        // ── 상태 ──
        private bool isSorting = false;
        private bool isPaused = false;
        private int compareCount = 0;
        private int swapCount = 0;

        // ── 레이아웃 ──
        private double barGap = 6;
        private double barWidth, maxBarHeight, barCanvasW, barCanvasH;
        private double treeCanvasW, treeCanvasH;
        private const double NodeRadius = 22;

        // ── 색상 (Catppuccin Mocha) ──
        private static readonly SolidColorBrush CDefault = Br("#89B4FA");
        private static readonly SolidColorBrush CParent = Br("#F38BA8");
        private static readonly SolidColorBrush CChild = Br("#FAB387");
        private static readonly SolidColorBrush CSwap = Br("#CBA6F7");
        private static readonly SolidColorBrush CDone = Br("#A6E3A1");
        private static readonly SolidColorBrush CExcluded = Br("#45475A");
        private static readonly SolidColorBrush CText = Br("#CDD6F4");
        private static readonly SolidColorBrush CTextDark = Br("#1E1E2E");
        private static readonly SolidColorBrush CLine = Br("#585B70");
        private static readonly SolidColorBrush CLineDim = Br("#313244");

        private static SolidColorBrush Br(string hex)
        {
            var b = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
            b.Freeze();
            return b;
        }

        // =====================================================================
        public MainWindow()
        {
            InitializeComponent();
            sliderSpeed.ValueChanged += (s, e) => txtSpeed.Text = $"{(int)sliderSpeed.Value}ms";
            Loaded += (s, e) => ResetArray();
        }

        // =====================================================================
        // 초기화
        // =====================================================================
        private void ResetArray()
        {
            arr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Random rng = new Random();
            for (int i = arr.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }

            heapSize = arr.Length;
            compareCount = 0;
            swapCount = 0;
            UpdateCounters();
            txtPhase.Text = "단계: 대기";
            txtStatus.Text = "▶ 시작 버튼을 눌러주세요";
            DrawAll();
        }

        // =====================================================================
        // 전체 다시 그리기
        // =====================================================================
        private void DrawAll()
        {
            DrawTree();
            DrawBars();
        }

        // =====================================================================
        // 트리 그리기
        // =====================================================================
        private void DrawTree()
        {
            canvasTree.Children.Clear();
            int n = arr.Length;

            treeCanvasW = canvasTree.ActualWidth > 0 ? canvasTree.ActualWidth : 940;
            treeCanvasH = canvasTree.ActualHeight > 0 ? canvasTree.ActualHeight : 320;

            treeNodes = new Ellipse[n];
            treeLabels = new TextBlock[n];
            treeIndexLabels = new TextBlock[n];
            treeLines = new Line[n]; // index 0은 루트(부모 없음), 1~n-1은 부모로의 선
            nodeX = new double[n];
            nodeY = new double[n];

            // 레벨별 노드 위치 계산
            // 레벨 0: 1개, 레벨 1: 2개, 레벨 2: 4개, 레벨 3: 3개(0~9 = 10개)
            int totalLevels = (int)Math.Floor(Math.Log(n, 2)) + 1;
            double levelHeight = (treeCanvasH - 30) / totalLevels;
            double topMargin = 25;

            for (int i = 0; i < n; i++)
            {
                int level = (int)Math.Floor(Math.Log(i + 1, 2));
                int posInLevel = i - ((1 << level) - 1);     // 해당 레벨에서 몇 번째
                int nodesInLevel = Math.Min(1 << level, n - ((1 << level) - 1));

                // X: 레벨의 전체 너비를 균등 분할
                int maxInLevel = 1 << level;
                double slotWidth = treeCanvasW / (maxInLevel + 1);
                double cx = slotWidth * (posInLevel + 1);

                // Y: 레벨별
                double cy = topMargin + level * levelHeight;

                nodeX[i] = cx;
                nodeY[i] = cy;
            }

            // 연결선 먼저 (노드 뒤에 깔리도록)
            for (int i = 1; i < n; i++)
            {
                int parent = (i - 1) / 2;
                var line = new Line
                {
                    X1 = nodeX[parent],
                    Y1 = nodeY[parent] + NodeRadius,
                    X2 = nodeX[i],
                    Y2 = nodeY[i] - NodeRadius,
                    Stroke = i < heapSize ? CLine : CLineDim,
                    StrokeThickness = 2
                };
                canvasTree.Children.Add(line);
                treeLines[i] = line;
            }

            // 노드 원 + 라벨
            for (int i = 0; i < n; i++)
            {
                bool inHeap = i < heapSize;

                var ellipse = new Ellipse
                {
                    Width = NodeRadius * 2,
                    Height = NodeRadius * 2,
                    Fill = inHeap ? CDefault : CExcluded,
                    Stroke = Br("#313244"),
                    StrokeThickness = 2
                };
                Canvas.SetLeft(ellipse, nodeX[i] - NodeRadius);
                Canvas.SetTop(ellipse, nodeY[i] - NodeRadius);
                canvasTree.Children.Add(ellipse);
                treeNodes[i] = ellipse;

                // 값 라벨
                var tb = new TextBlock
                {
                    Text = arr[i].ToString(),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = CTextDark,
                    TextAlignment = TextAlignment.Center,
                    Width = NodeRadius * 2
                };
                Canvas.SetLeft(tb, nodeX[i] - NodeRadius);
                Canvas.SetTop(tb, nodeY[i] - 10);
                canvasTree.Children.Add(tb);
                treeLabels[i] = tb;

                // 인덱스 라벨 (노드 아래)
                var idx = new TextBlock
                {
                    Text = $"[{i}]",
                    FontSize = 10,
                    Foreground = Br("#6C7086"),
                    TextAlignment = TextAlignment.Center,
                    Width = NodeRadius * 2
                };
                Canvas.SetLeft(idx, nodeX[i] - NodeRadius);
                Canvas.SetTop(idx, nodeY[i] + NodeRadius + 1);
                canvasTree.Children.Add(idx);
                treeIndexLabels[i] = idx;
            }
        }

        // =====================================================================
        // 막대 그리기
        // =====================================================================
        private void DrawBars()
        {
            canvasBar.Children.Clear();
            int n = arr.Length;

            barCanvasW = canvasBar.ActualWidth > 0 ? canvasBar.ActualWidth : 940;
            barCanvasH = canvasBar.ActualHeight > 0 ? canvasBar.ActualHeight : 170;
            barWidth = (barCanvasW - barGap * (n + 1)) / n;
            maxBarHeight = barCanvasH - 35;

            bars = new Rectangle[n];
            barLabels = new TextBlock[n];

            for (int i = 0; i < n; i++)
            {
                bool inHeap = i < heapSize;
                double barH = GetBarHeight(arr[i]);
                double leftX = barGap + i * (barWidth + barGap);

                var rect = new Rectangle
                {
                    Width = barWidth,
                    Height = barH,
                    Fill = inHeap ? CDefault : (i >= heapSize ? CDone : CExcluded),
                    RadiusX = 5,
                    RadiusY = 5
                };
                Canvas.SetLeft(rect, leftX);
                Canvas.SetTop(rect, barCanvasH - 30 - barH);
                canvasBar.Children.Add(rect);
                bars[i] = rect;

                var tb = new TextBlock
                {
                    Text = arr[i].ToString(),
                    FontSize = 15,
                    FontWeight = FontWeights.Bold,
                    Foreground = CText,
                    TextAlignment = TextAlignment.Center,
                    Width = barWidth
                };
                Canvas.SetLeft(tb, leftX);
                Canvas.SetTop(tb, barCanvasH - 24);
                canvasBar.Children.Add(tb);
                barLabels[i] = tb;
            }
        }

        private double GetBarHeight(int value)
        {
            return Math.Max(12, (value + 1) * (maxBarHeight / 10.5));
        }

        // =====================================================================
        // 색상 설정 (트리 + 막대 동시)
        // =====================================================================
        private void SetColor(int index, SolidColorBrush color)
        {
            if (index < 0 || index >= arr.Length) return;
            treeNodes[index].Fill = color;
            bars[index].Fill = color;
        }

        private void RefreshAllColors()
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (i >= heapSize)
                {
                    treeNodes[i].Fill = CExcluded;
                    bars[i].Fill = CDone;
                    if (treeLines[i] != null) treeLines[i].Stroke = CLineDim;
                }
                else
                {
                    treeNodes[i].Fill = CDefault;
                    bars[i].Fill = CDefault;
                    if (i > 0 && treeLines[i] != null) treeLines[i].Stroke = CLine;
                }
            }
        }

        // 트리 값 & 막대 값 동기화 (스왑 후)
        private void RefreshVisualValues()
        {
            for (int i = 0; i < arr.Length; i++)
            {
                // 트리 라벨
                treeLabels[i].Text = arr[i].ToString();

                // 막대 라벨 + 높이
                barLabels[i].Text = arr[i].ToString();
                double h = GetBarHeight(arr[i]);
                bars[i].Height = h;
                Canvas.SetTop(bars[i], barCanvasH - 30 - h);
            }
        }

        private void HighlightLine(int childIdx, SolidColorBrush color)
        {
            if (childIdx > 0 && childIdx < arr.Length && treeLines[childIdx] != null)
            {
                treeLines[childIdx].Stroke = color;
                treeLines[childIdx].StrokeThickness = 3;
            }
        }

        private void ResetLine(int childIdx)
        {
            if (childIdx > 0 && childIdx < arr.Length && treeLines[childIdx] != null)
            {
                treeLines[childIdx].Stroke = childIdx < heapSize ? CLine : CLineDim;
                treeLines[childIdx].StrokeThickness = 2;
            }
        }

        private void UpdateCounters()
        {
            txtCompareCount.Text = $"비교: {compareCount}회";
            txtSwapCount.Text = $"스왑: {swapCount}회";
        }

        // =====================================================================
        // 딜레이 (일시정지)
        // =====================================================================
        private int DelayMs => (int)sliderSpeed.Value;

        private async Task Pause(int ms)
        {
            while (isPaused) await Task.Delay(50);
            await Task.Delay(ms);
        }

        // =====================================================================
        // 스왑 (데이터 + 시각)
        // =====================================================================
        private void DoSwap(int a, int b)
        {
            if (a == b) return;
            swapCount++;
            UpdateCounters();

            (arr[a], arr[b]) = (arr[b], arr[a]);
            RefreshVisualValues();
        }

        // =====================================================================
        // 힙 정렬
        // =====================================================================
        private async Task HeapSort()
        {
            int n = arr.Length;
            heapSize = n;

            // ── 1단계: Max Heap 구축 ──
            txtPhase.Text = "1단계: Max Heap 구축";
            for (int i = n / 2 - 1; i >= 0; i--)
            {
                await Heapify(heapSize, i);
            }

            txtStatus.Text = "Max Heap 구축 완료!";
            await Pause(DelayMs);

            // ── 2단계: 추출 정렬 ──
            txtPhase.Text = "2단계: 루트 추출 → 정렬";
            for (int i = n - 1; i > 0; i--)
            {
                // 루트(최대)와 마지막 힙 원소 스왑
                txtStatus.Text = $"루트(최대) arr[0]={arr[0]} ↔ arr[{i}]={arr[i]}  →  위치 [{i}] 확정";
                SetColor(0, CSwap);
                SetColor(i, CSwap);
                await Pause(DelayMs);

                DoSwap(0, i);
                await Pause(DelayMs / 2);

                // 힙 크기 줄이기
                heapSize = i;
                RefreshAllColors();
                await Pause(DelayMs / 3);

                // 루트에서 Heapify
                await Heapify(heapSize, 0);
            }

            heapSize = 0;
            RefreshAllColors();
            SetColor(0, CDone);
        }

        // =====================================================================
        // Heapify (시각화)
        // =====================================================================
        private async Task Heapify(int size, int rootIdx)
        {
            int largest = rootIdx;
            int left = 2 * rootIdx + 1;
            int right = 2 * rootIdx + 2;

            // 부모 하이라이트
            SetColor(rootIdx, CParent);
            txtStatus.Text = $"Heapify: 부모 [{rootIdx}]={arr[rootIdx]}  (힙 크기={size})";
            await Pause(DelayMs);

            // 왼쪽 자식 비교
            if (left < size)
            {
                compareCount++;
                UpdateCounters();
                SetColor(left, CChild);
                HighlightLine(left, CChild);
                txtStatus.Text = $"비교: 부모 [{rootIdx}]={arr[rootIdx]}  vs  왼쪽자식 [{left}]={arr[left]}";
                await Pause(DelayMs);

                if (arr[left] > arr[largest])
                    largest = left;

                ResetLine(left);
            }

            // 오른쪽 자식 비교
            if (right < size)
            {
                compareCount++;
                UpdateCounters();
                SetColor(right, CChild);
                HighlightLine(right, CChild);
                txtStatus.Text = $"비교: 현재최대 [{largest}]={arr[largest]}  vs  오른쪽자식 [{right}]={arr[right]}";
                await Pause(DelayMs);

                if (arr[right] > arr[largest])
                    largest = right;

                ResetLine(right);
            }

            RefreshAllColors();

            // 스왑 필요
            if (largest != rootIdx)
            {
                txtStatus.Text = $"스왑: [{rootIdx}]={arr[rootIdx]} ↔ [{largest}]={arr[largest]}";
                SetColor(rootIdx, CSwap);
                SetColor(largest, CSwap);
                HighlightLine(largest, CSwap);
                await Pause(DelayMs);

                DoSwap(rootIdx, largest);
                await Pause(DelayMs / 2);

                ResetLine(largest);
                RefreshAllColors();

                // 재귀
                await Heapify(size, largest);
            }
        }

        // =====================================================================
        // 버튼
        // =====================================================================
        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (isSorting) return;
            isSorting = true;
            isPaused = false;
            btnStart.IsEnabled = false;
            btnPause.IsEnabled = true;
            btnReset.IsEnabled = false;

            // 현재 캔버스 크기로 레이아웃 확정
            DrawAll();

            await HeapSort();

            // 완료 연출
            for (int i = 0; i < arr.Length; i++)
            {
                SetColor(i, CDone);
                bars[i].Fill = CDone;
                await Task.Delay(80);
            }

            txtPhase.Text = "완료!";
            txtStatus.Text = $"✔ 정렬 완료!  (비교 {compareCount}회, 스왑 {swapCount}회)";
            isSorting = false;
            isPaused = false;
            btnPause.IsEnabled = false;
            btnPause.Content = "⏸ 일시정지";
            btnReset.IsEnabled = true;
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            if (!isSorting) return;
            isPaused = !isPaused;
            btnPause.Content = isPaused ? "▶ 재개" : "⏸ 일시정지";
            if (isPaused)
                txtStatus.Text = "⏸ 일시정지 중...  (재개 버튼을 누르세요)";
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (isSorting) return;
            btnStart.IsEnabled = true;
            btnPause.IsEnabled = false;
            btnPause.Content = "⏸ 일시정지";
            ResetArray();
        }
    }
}
```


## Bubble Sort ex(C# Console)

```csharp
namespace ConsoleApp17
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 정렬할 배열 (0~9까지 숫자를 랜덤 순서로 배치)
            int[] arr = { 7, 3, 9, 0, 5, 1, 8, 4, 2, 6 };
            // 현재 배열 상태 출력 (정렬 전 확인)
            Console.WriteLine("정렬 전: " + string.Join(", ", arr));
            // 바깥 반복문: 전체 반복 횟수 (한 바퀴 돌 때마다 가장 큰 값이 뒤로 이동)
            for (int i = 0; i < arr.Length - 1; i++)
            {
                // 안쪽 반복문: 앞에서부터 인접한 값들을 비교
                // (뒤쪽은 이미 정렬되었기 때문에 -i 만큼 범위를 줄임)
                for (int j = 0; j < arr.Length - 1 - i; j++)
                {
                    // 현재 값이 다음 값보다 크면 (순서가 잘못된 경우)
                    if (arr[j] > arr[j + 1])
                    {
                        // 두 값을 서로 교환 (swap)
                        // 1. 임시 변수(temp)에 현재 값을 잠깐 저장
                        int temp = arr[j];
                        // 2. 오른쪽 값을 왼쪽으로 이동
                        arr[j] = arr[j + 1];
                        // 3. 임시 변수에 저장해둔 값을 오른쪽으로 이동
                        arr[j + 1] = temp;
                    }
                }
            }
            // 정렬 완료 후 배열 상태 출력
            Console.WriteLine("정렬 후: " + string.Join(", ", arr));
        }
    }
}
```
