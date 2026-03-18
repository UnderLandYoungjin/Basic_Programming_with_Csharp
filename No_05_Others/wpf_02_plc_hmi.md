# PLC HMI 모니터링 시스템 - WPF + Modbus TCP


https://github.com/user-attachments/assets/d4b4c810-4168-4170-bfde-3eca439e5293




## 📋 개요

산업용 PLC와 Modbus TCP로 통신하는 **WPF 기반 로컬 HMI** 시스템입니다.
실제 PLC 없이도 시뮬레이터로 즉시 테스트 가능하며, 실제 현장에서는 한 줄 교체로 PLC 연결이 가능합니다.

- **프레임워크**: .NET 10 / WPF
- **패턴**: MVVM (CommunityToolkit.Mvvm)
- **PLC 통신**: NModbus (Modbus TCP)
- **로깅**: Serilog (콘솔 + 파일)
- **차트**: LiveChartsCore (확장용)

---

## 📁 프로젝트 구조

```
PlcHmiWpf/
│
├── PlcHmiWpf.csproj                # 프로젝트 설정 (.NET 10, WPF, NuGet 패키지)
├── App.xaml                        # Application 리소스 (Theme.xaml 참조)
├── App.xaml.cs                     # 앱 진입점, Serilog 초기화, 전역 예외 처리
│
├── Models/
│   └── PlcModels.cs                # 데이터 모델 정의
│       ├── PlcTag                  #   - PLC 태그 (이름, 주소, 스케일, 알람 한계)
│       ├── PlcTagValue             #   - 런타임 실시간 값
│       ├── AlarmRecord             #   - 알람 이력 레코드
│       ├── TrendDataPoint          #   - 트렌드 차트용 시계열 포인트
│       ├── PlcConnectionConfig     #   - 통신 설정 (IP, Port, 폴링 주기)
│       └── Enums                   #   - PlcDeviceArea, PlcConnectionState 등
│
├── Services/
│   ├── PlcCommunicationService.cs  # 실제 PLC Modbus TCP 통신
│   │   ├── ConnectAsync()          #   - TCP 연결 + ModbusFactory 생성
│   │   ├── ReadAllTagsAsync()      #   - 전체 태그 일괄 읽기 (200ms 폴링)
│   │   ├── WriteBit()              #   - Coil 쓰기 (시작/정지 명령)
│   │   ├── WriteRegister()         #   - Register 쓰기 (설정값)
│   │   └── 알람 자동 판정           #   - 상한/하한 초과 시 AlarmTriggered 이벤트
│   │
│   └── PlcSimulatorService.cs      # PLC 시뮬레이터 (개발/테스트용)
│       ├── 온도 PID 시뮬레이션     #   - 설정값으로 서서히 수렴 + 노이즈
│       ├── 모터/압력/수위 연동      #   - 물리적 연관관계 시뮬레이션
│       └── 동일 이벤트 인터페이스   #   - PlcCommunicationService와 교체 가능
│
├── ViewModels/
│   └── MainViewModel.cs            # 메인 MVVM ViewModel
│       ├── [ObservableProperty]    #   - Temperature, Pressure, MotorSpeed 등 자동 바인딩
│       ├── [RelayCommand]          #   - StartOperation, StopOperation, EmergencyStop 등
│       ├── TrendData Collections   #   - 실시간 차트용 ObservableCollection
│       └── AlarmHistory            #   - 알람 이력 관리 + 확인/삭제
│
├── Views/
│   ├── MainWindow.xaml             # HMI 메인 화면 (XAML 레이아웃)
│   │   ├── 헤더바                  #   - 타이틀, 운전상태 LED, 연결 상태, 연결 버튼
│   │   ├── 좌측: 실시간 계측       #   - 온도/압력/속도/수위 카드 + 프로그레스바
│   │   ├── 좌측: 디지털 I/O        #   - 운전/경보/도어/예비 LED 표시
│   │   ├── 우측: 운전 제어         #   - 시작/정지/E-STOP 버튼 + 설정값 슬라이더
│   │   ├── 우측: 통신 설정         #   - IP, Port 입력
│   │   ├── 우측: 알람 이력         #   - DataGrid (시간, 태그, 내용, 값)
│   │   └── 상태바                  #   - 상태 메시지, 모드, 현재 시간
│   │
│   └── MainWindow.xaml.cs          # 코드비하인드 (Closing 시 Dispose만)
│
├── Converters/
│   └── Converters.cs               # XAML 값 변환기 7개
│       ├── ConnectionStateToColorConverter   # 연결상태 → 색상 (초록/주황/빨강/회색)
│       ├── ConnectionStateToTextConverter    # 연결상태 → 텍스트 (연결됨/연결중/오류/미연결)
│       ├── BoolToRunColorConverter           # bool → 운전색상 (초록/빨강)
│       ├── BoolToRunTextConverter            # bool → 텍스트 (● RUN / ■ STOP)
│       ├── AlarmToBrushConverter             # 알람여부 → 배경색
│       ├── BoolToVisibilityConverter         # bool → Visible/Collapsed
│       ├── AlarmSeverityToColorConverter     # 심각도 → 색상
│       └── ValueToPercentConverter           # 값 → 백분율 (IMultiValueConverter)
│
└── Assets/
    └── Theme.xaml                  # 산업용 다크 테마 리소스 딕셔너리
        ├── 색상 (BgDark, StatusRun, Accent 등)
        ├── HmiCard 스타일 (둥근 모서리 + 그림자)
        ├── HmiButton / HmiDangerButton
        ├── BigValue / UnitText / SectionHeader
        ├── HmiTextBox / HmiProgressBar
        └── StatusLed 스타일
```

---

## 🏗 아키텍처

```
┌─────────────────────────────────────────────────────┐
│                  MainWindow.xaml                      │
│              (WPF View - XAML UI)                     │
│  ┌───────────┐  ┌───────────┐  ┌─────────────────┐  │
│  │ 계측 카드  │  │ 제어 패널  │  │  알람 테이블    │  │
│  │ 온도/압력  │  │ 시작/정지  │  │  DataGrid      │  │
│  │ 속도/수위  │  │ 설정값입력 │  │  이력 관리     │  │
│  └───────────┘  └───────────┘  └─────────────────┘  │
│          ↕ Data Binding (MVVM)                       │
├─────────────────────────────────────────────────────┤
│               MainViewModel.cs                       │
│  ┌───────────────────────────────────────────────┐  │
│  │  [ObservableProperty] Temperature, Pressure   │  │
│  │  [RelayCommand] StartOperation, StopOperation │  │
│  │  ObservableCollection<TrendDataPoint>         │  │
│  │  ObservableCollection<AlarmRecord>            │  │
│  └───────────────────────────────────────────────┘  │
│          ↕ Events + Method Calls                     │
├─────────────────────────────────────────────────────┤
│   PlcSimulatorService  ←→  PlcCommunicationService   │
│   (테스트용 가상 데이터)     (실제 Modbus TCP 통신)    │
│                              ↕ TCP/IP                │
├─────────────────────────────────────────────────────┤
│                    PLC Hardware                       │
│   LS XBC + XGL-EFMT / Mitsubishi Q + QJ71E71        │
│   Siemens S7-1200 / 기타 Modbus TCP 지원 PLC         │
└─────────────────────────────────────────────────────┘
```

### MVVM 데이터 흐름

```
[PLC/시뮬레이터] → DataReceived 이벤트 → [ViewModel] ObservableProperty 갱신
                                                ↕ Data Binding
                                          [View] XAML UI 자동 갱신

[View] 버튼 클릭 → [RelayCommand] → [ViewModel] → WriteBit/WriteRegister → [PLC]
```

---

## ⚙ 환경 설정 및 실행

### 필수 환경

| 항목 | 요구 사항 |
|------|----------|
| OS | Windows 10/11 |
| .NET SDK | 10.0 이상 (`dotnet --version`으로 확인) |
| IDE | Visual Studio 2022+ 또는 VS Code |

### 설치 및 실행

```powershell
# 1. 프로젝트 폴더로 이동
cd C:\cs\PlcHmiWpf

# 2. NuGet 패키지 복원
dotnet restore

# 3. 빌드
dotnet build

# 4. 실행 (시뮬레이터 모드)
dotnet run
```

### 실행 확인

1. 프로그램 실행 후 우상단 **"연결됨"** 버튼 클릭
2. 시뮬레이터 연결 → 실시간 데이터 표시 시작
3. **▶ 시작** 버튼 → 운전 시작 (온도 상승, 모터 가속)
4. 온도 SP / 속도 SP 슬라이더 조절 → **적용** 클릭
5. 값이 설정값으로 서서히 수렴하는 것 확인
6. 상한/하한 초과 시 알람 이력에 자동 기록

---

## 🔧 빌드 트러블슈팅 (실제 해결 이력)

### 오류 1: `project.assets.json` 파일을 찾을 수 없습니다

**원인**: NuGet 복원이 안 된 상태에서 빌드 시도

**해결**:
```powershell
# PowerShell에서 실행
Remove-Item -Recurse -Force obj, bin -ErrorAction SilentlyContinue
dotnet restore
dotnet build
```

> ⚠ PowerShell에서는 `rmdir /s /q`가 안 됩니다.
> `Remove-Item -Recurse -Force` 를 사용하세요.

---

### 오류 2: `TextTransform` 속성을 찾을 수 없습니다

```
error MC4005: 'TextBlock' 형식에서 Style Property 'TextTransform'을(를) 찾을 수 없습니다.
```

**원인**: WPF의 `TextBlock`에는 `TextTransform` 속성이 없음 (CSS 개념)

**해결**: `Assets/Theme.xaml`에서 해당 줄 삭제
```xml
<!-- 삭제할 줄 -->
<Setter Property="TextTransform" Value="Uppercase" />
```

---

### 오류 3: NModbus4 호환성 경고

```
warning NU1701: 패키지 'NModbus4 2.1.0'을(를) 복원했습니다. 
이 패키지는 프로젝트와 완벽하게 호환되지 않을 수 있습니다.
```

**원인**: `NModbus4`는 .NET Framework 전용 패키지

**해결**: `.csproj`에서 패키지 교체
```xml
<!-- 변경 전 -->
<PackageReference Include="NModbus4" Version="2.1.0" />

<!-- 변경 후 (.NET 8/10 호환) -->
<PackageReference Include="NModbus" Version="3.0.81" />
```

**코드 변경** (`PlcCommunicationService.cs`):
```csharp
// 변경 전 (NModbus4)
using Modbus.Device;
_master = ModbusIpMaster.CreateIp(_tcpClient);

// 변경 후 (NModbus)
using NModbus;
var factory = new ModbusFactory();
_master = factory.CreateMaster(_tcpClient);
```

**필드 타입 변경**:
```csharp
// 변경 전
private ModbusIpMaster? _master;

// 변경 후
private IModbusMaster? _master;
```

---

### 오류 4: 88개 using 관련 오류 (IDisposable, Task, DateTime 등)

```
error CS0246: 'IDisposable' 형식 또는 네임스페이스 이름을 찾을 수 없습니다.
error CS0246: 'Task' 형식 또는 네임스페이스 이름을 찾을 수 없습니다.
error CS0246: 'DateTime' 형식 또는 네임스페이스 이름을 찾을 수 없습니다.
(... 88개)
```

**원인**: `ImplicitUsings`가 비활성화 상태 → `using System;` 등 기본 네임스페이스가 자동 포함 안 됨

**해결**: `.csproj`의 `<PropertyGroup>`에 한 줄 추가
```xml
<ImplicitUsings>enable</ImplicitUsings>
```

> `ImplicitUsings`를 켜면 `System`, `System.Collections.Generic`, `System.Threading.Tasks`,
> `System.Linq` 등이 전역 자동 포함됩니다.

---

### 오류 5: `OnConnectionStateChanged` 메서드 충돌

```
error CS0111: 'MainViewModel' 형식은 동일한 매개 변수 형식을 가진 
'OnConnectionStateChanged' 멤버를 미리 정의합니다.
```

**원인**: `[ObservableProperty] _connectionState`가 CommunityToolkit에 의해
자동으로 `OnConnectionStateChanged()` 메서드를 생성하는데,
PLC 이벤트 핸들러도 같은 이름으로 정의되어 충돌

**해결**: `MainViewModel.cs`에서 이름 변경 (Ctrl+H 모두 바꾸기)
```
찾기:  OnConnectionStateChanged
바꾸기: OnPlcConnectionStateChanged
```
→ 메서드 정의 1곳 + 이벤트 구독 1곳, 총 2곳 변경

---

### 오류 6: 람다 식을 double 형식으로 변환할 수 없습니다

```
error CS1660: 람다 식은(는) 대리자 형식이 아니므로 'double' 형식으로 변환할 수 없습니다.
```

**원인**: C#에서 `double noise = () => ...` 문법은 변수에 람다를 넣는 것으로, 타입 불일치

**해결**: `PlcSimulatorService.cs`에서 로컬 함수로 변경
```csharp
// 변경 전 (오류)
double noise = () => (_rng.NextDouble() - 0.5) * 2.0;

// 변경 후 (로컬 함수)
double Noise() => (_rng.NextDouble() - 0.5) * 2.0;
```
그리고 같은 메서드 안에서 `noise()` → `Noise()` 로 모두 변경 (대소문자 주의)

---

### 오류 7: .NET Runtime 설치 요구 팝업

```
You must install or update .NET to run this application.
```

**원인**: SDK 10.0이 설치되어 있지만 프로젝트 타겟이 `net8.0-windows`로 설정되어
exe 직접 실행 시 런타임 매칭 오류

**해결**: `.csproj`에서 타겟 프레임워크 변경
```xml
<!-- 변경 전 -->
<TargetFramework>net8.0-windows</TargetFramework>

<!-- 변경 후 (설치된 SDK에 맞춤) -->
<TargetFramework>net10.0-windows</TargetFramework>
```

---

## 📡 Modbus 레지스터 매핑

### 아날로그 입력 (읽기 전용 - Holding Register)

| 태그명 | 설명 | Modbus 주소 | 스케일 | 단위 | 알람 상한 | 알람 하한 |
|--------|------|-------------|--------|------|----------|----------|
| TEMP_OVEN | 오븐 온도 | D100 | ×0.1 | °C | 250.0 | 10.0 |
| PRESSURE_MAIN | 메인 압력 | D102 | ×0.01 | MPa | 8.0 | 0.5 |
| SPEED_MOTOR1 | 모터1 속도 | D104 | ×1.0 | RPM | 3000.0 | - |
| LEVEL_TANK | 탱크 수위 | D106 | ×0.1 | % | 95.0 | 5.0 |
| FLOW_RATE | 유량 | D108 | ×0.1 | L/min | - | - |

### 디지털 입력 (읽기 전용)

| 태그명 | 설명 | Modbus 영역 | 주소 |
|--------|------|-------------|------|
| RUN_STATUS | 운전 상태 | Coil | M0 |
| ALARM_STATUS | 경보 상태 | Coil | M1 |
| DOOR_SENSOR | 도어 센서 | Discrete Input | X0 |

### 제어 출력 (쓰기 가능)

| 태그명 | 설명 | Modbus 영역 | 주소 | 단위 |
|--------|------|-------------|------|------|
| CMD_START | 운전 시작 명령 | Coil | M100 | Bit |
| CMD_STOP | 운전 정지 명령 | Coil | M101 | Bit |
| CMD_SPEED_SP | 모터 속도 설정 | Holding Register | D200 | RPM |
| CMD_TEMP_SP | 온도 설정 | Holding Register | D202 | °C |

---

## 🔌 실제 PLC 연결 방법

### Step 1: ViewModel 서비스 교체

`ViewModels/MainViewModel.cs`에서 두 줄만 변경:

```csharp
// ── 변경 전 (시뮬레이터) ──
private readonly PlcSimulatorService _plc;

// 생성자:
_plc = new PlcSimulatorService();


// ── 변경 후 (실제 PLC) ──
private readonly PlcCommunicationService _plc;

// 생성자:
_plc = new PlcCommunicationService(new PlcConnectionConfig
{
    IpAddress = "192.168.1.10",   // PLC IP 주소
    Port = 502,                    // Modbus TCP 포트
    SlaveId = 1,                   // Modbus 슬레이브 ID
    PollingIntervalMs = 200,       // 폴링 주기 (ms)
    TimeoutMs = 3000,              // 통신 타임아웃
    RetryCount = 3                 // 재시도 횟수
});
```

### Step 2: PLC 측 Modbus 설정

**LS XBC (XG5000)**:
1. XGL-EFMT 이더넷 모듈 장착
2. XG5000 → 파라미터 → 이더넷 → Modbus TCP Slave 활성화
3. IP 주소 설정 (예: 192.168.1.10)

**Mitsubishi Q (GX Works2/3)**:
1. QJ71E71-100 이더넷 모듈 장착
2. 네트워크 파라미터 → Modbus TCP 설정
3. 또는 MC Protocol 사용 시 별도 라이브러리 필요

**Siemens S7-1200/1500**:
1. 내장 이더넷 사용
2. S7.Net 라이브러리 권장 (Modbus보다 효율적)
3. TIA Portal → DB 접근 허용 설정

### Step 3: 태그 주소 매핑 조정

`PlcCommunicationService.cs`의 `InitializeDefaultTags()`에서
실제 PLC 프로그램의 메모리 주소에 맞게 태그 주소를 수정합니다.

---

## 📦 NuGet 패키지 목록

| 패키지 | 버전 | 용도 |
|--------|------|------|
| CommunityToolkit.Mvvm | 8.2.2 | MVVM 패턴 ([ObservableProperty], [RelayCommand]) |
| NModbus | 3.0.81 | Modbus TCP/RTU 통신 (.NET 8/10 호환) |
| LiveChartsCore.SkiaSharpView.WPF | 2.0.0-rc3.3 | 실시간 차트 (트렌드 뷰 확장용) |
| Serilog | 3.1.1 | 구조화 로깅 프레임워크 |
| Serilog.Sinks.File | 5.0.0 | 일별 로그 파일 기록 (logs/ 폴더) |
| Serilog.Sinks.Console | 5.0.0 | 콘솔 로그 출력 (디버깅용) |

---

## 🎨 UI 테마 시스템

### 색상 체계 (Assets/Theme.xaml)

| 용도 | 키 | 색상코드 | 설명 |
|------|-----|---------|------|
| 배경 (최하단) | BgDark | #1A1D23 | 메인 배경 |
| 배경 (중간) | BgMedium | #242830 | 헤더/상태바 |
| 배경 (밝은) | BgLight | #2E333D | 입력필드 배경 |
| 카드 배경 | BgCard | #303540 | 계측 카드 |
| 텍스트 (주) | TextPrimary | #E8ECF1 | 큰 값 표시 |
| 텍스트 (부) | TextSecondary | #8B95A5 | 라벨, 헤더 |
| 운전 (RUN) | StatusRun | #00C853 | 초록 LED |
| 정지 (STOP) | StatusStop | #FF5252 | 빨강 LED |
| 경고 | StatusWarning | #FFAB40 | 주황 |
| 액센트 | Accent | #448AFF | 버튼, 강조 |

### 주요 스타일

| 스타일 키 | 대상 | 설명 |
|-----------|------|------|
| HmiCard | Border | 둥근 모서리(8px) + 드롭섀도우 + 패딩 |
| HmiButton | Button | 파란 액센트 + 호버/프레스 효과 |
| HmiDangerButton | Button | 빨간 배경 (비상 정지용) |
| BigValue | TextBlock | 36pt Bold (큰 계측값) |
| UnitText | TextBlock | 14pt 회색 (단위 표시) |
| SectionHeader | TextBlock | 13pt SemiBold 회색 (섹션 제목) |
| HmiTextBox | TextBox | 다크 배경 입력필드 |
| HmiProgressBar | ProgressBar | 8px 높이 바 차트 |

---

## 🚀 확장 포인트

### 1. 실시간 트렌드 차트 추가

ViewModel에 이미 `TempTrend`, `PressureTrend` 등 ObservableCollection이 준비되어 있습니다.

```xml
<!-- MainWindow.xaml에 추가 -->
<lvc:CartesianChart
    Series="{Binding TempSeries}"
    XAxes="{Binding XAxes}"
    YAxes="{Binding YAxes}"
    Height="200"/>
```

```csharp
// MainViewModel.cs에 추가
public ISeries[] TempSeries => new ISeries[]
{
    new LineSeries<TrendDataPoint>
    {
        Values = TempTrend,
        Mapping = (point, _) => new(point.Timestamp.Ticks, point.Value)
    }
};
```

### 2. 데이터 로깅 (SQLite)

```csharp
// NuGet: Microsoft.EntityFrameworkCore.Sqlite
public class HmiDbContext : DbContext
{
    public DbSet<AlarmRecord> Alarms { get; set; }
    public DbSet<TrendDataPoint> TrendData { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=hmi_data.db");
}
```

### 3. 레시피 관리

```csharp
public class Recipe
{
    public string Name { get; set; } = "";
    public double TempSetPoint { get; set; }
    public double SpeedSetPoint { get; set; }
    public int DurationSeconds { get; set; }
}

// JSON 저장/로드
var json = JsonSerializer.Serialize(recipe);
File.WriteAllText("recipes/default.json", json);
```

### 4. OPC UA 통신으로 전환

```csharp
// NuGet: OPCFoundation.NetStandard.Opc.Ua
// PlcCommunicationService의 읽기/쓰기 메서드만 교체
```

### 5. 원격 모니터링 확장

```
WPF HMI (로컬)
    ↕
ASP.NET Core 백엔드 (SignalR)
    ↕
Blazor 웹 대시보드 (원격)
```

---

## 📝 최종 .csproj 설정

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UseWPF>true</UseWPF>
    <AssemblyName>PlcHmiWpf</AssemblyName>
    <RootNamespace>PlcHmiWpf</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
    <PackageReference Include="LiveChartsCore.SkiaSharpView.WPF" Version="2.0.0-rc3.3" />
    <PackageReference Include="NModbus" Version="3.0.81" />
    <PackageReference Include="Serilog" Version="3.1.1" />
    <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="5.0.0" />
  </ItemGroup>
</Project>
```

---

## 📄 라이선스

교육/실습 목적 자유 사용
