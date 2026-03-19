##HeapSortVisualizer


https://github.com/user-attachments/assets/47bf7f6e-49ff-4a3e-bfb6-48dc163033ba


WPF 코드 MainWindow.xaml
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

MainWindow.xaml.cs코드

```csharp
