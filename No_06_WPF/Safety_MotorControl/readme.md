# Safety_MotorControl

## LS G100 인버터 + YOLOv8 사람 감지 안전 정지 시스템

USB 카메라로 작업 영역을 모니터링하다가 **사람이 감지되면 인버터 모터를 즉시 정지**시키는 WPF 데스크톱 애플리케이션입니다.

---

## 프로젝트 정보

| 항목 | 내용 |
|------|------|
| 네임스페이스 | `Safety_MotorControl` |
| 대상 장비 | LS Electric LSLV-G100 시리즈 인버터 |
| 통신 방식 | Modbus RTU (RS-485) |
| 프레임워크 | .NET 8.0 WPF |
| 아키텍처 | MVVM (Model-View-ViewModel) |
| 객체 감지 | YOLOv8 Nano (ONNX Runtime — GPU CUDA) |
| 카메라 | USB Camera (OpenCvSharp4) |
| UI 테마 | Catppuccin Mocha + 네온 사이버 |

---

## 핵심 동작 흐름

```
USB 카메라 프레임 캡처 (~30fps, 백그라운드 스레드)
    │
    ▼
YOLOv8n ONNX 추론 (GPU CUDA 우선, CPU 자동 폴백)
    │
    ▼
COCO class 0 "person" 감지?
    │
    ├─ YES ──▶ Modbus FC06 → REG_CMD = STOP (0x0001)
    │          ● 안전정지 상태 활성화
    │          ● FWD/REV 버튼 비활성화
    │          ● 카메라 영상에 빨간 바운딩박스 + "SAFETY STOP" 경고
    │
    └─ NO ───▶ 마지막 감지 후 2초 경과 시 안전정지 해제
               ● 수동으로 FWD/REV 재운전 가능
```

---

## 프로젝트 구조

```
Safety_MotorControl/
├── Safety_MotorControl.csproj       ← 프로젝트 파일 (GPU OnnxRuntime)
├── App.xaml                         ← 전역 테마/스타일 리소스
├── App.xaml.cs
├── MainWindow.xaml                  ← 메인 UI (좌: 인버터 제어 / 우: 카메라+YOLO)
├── MainWindow.xaml.cs               ← 코드비하인드 (로그 자동스크롤)
├── Models/
│   └── InverterModel.cs             ← 레지스터 맵, 명령 상수, 상태 열거형
├── Services/
│   ├── ModbusRtuService.cs          ← Modbus RTU 통신 (FC03/FC06, CRC16)
│   ├── CameraService.cs             ← USB 카메라 백그라운드 캡처
│   └── YoloDetectionService.cs      ← YOLOv8 ONNX 추론 (GPU/CPU)
├── ViewModels/
│   ├── RelayCommand.cs              ← ICommand 구현
│   └── MainViewModel.cs             ← 통합 제어 로직 (인버터 + 카메라 + YOLO)
├── Converters/
│   └── BoolToColorConverter.cs      ← 값 변환기 모음 (7개 클래스)
└── yolov8n.onnx                     ← YOLOv8 Nano 모델 (별도 준비)
```

---

## NuGet 패키지

| 패키지 | 버전 | 용도 |
|--------|------|------|
| `System.IO.Ports` | 8.0.0 | RS-485 시리얼 통신 |
| `OpenCvSharp4` | 4.9.0.20240103 | 카메라 캡처 + 영상 처리 |
| `OpenCvSharp4.Extensions` | 4.9.0.20240103 | Mat → BitmapSource 변환 |
| `OpenCvSharp4.runtime.win` | 4.9.0.20240103 | Windows 네이티브 바인딩 |
| `Microsoft.ML.OnnxRuntime.Gpu` | 1.17.1 | ONNX 추론 (CUDA GPU) |

---

## GPU 사용 요구사항

YOLOv8 추론을 GPU로 실행하려면 아래 환경이 필요합니다.

| 요구사항 | 버전 |
|----------|------|
| NVIDIA GPU | Compute Capability 3.5 이상 |
| CUDA Toolkit | **11.8** |
| cuDNN | **8.x** (CUDA 11.8 대응) |
| NVIDIA 드라이버 | 452.39 이상 |

### GPU 환경 확인

```bash
# CUDA 설치 확인
nvcc --version

# GPU 인식 확인
nvidia-smi
```

### GPU가 없거나 CUDA 미설치 시

프로그램이 자동으로 **CPU 모드로 폴백**합니다. 로그에 아래와 같이 표시됩니다.

```
[YOLO] GPU 로드 실패 → CPU 폴백: ...
[YOLO] 모델 로드 성공 (CPU): yolov8n.onnx
```

UI 우측 상단에 현재 실행 모드가 `GPU (CUDA)` 또는 `CPU`로 표시됩니다.

---

## YOLOv8 ONNX 모델 준비

```bash
# ultralytics 설치
pip install ultralytics

# yolov8n (Nano) 모델 ONNX 내보내기
yolo export model=yolov8n.pt format=onnx imgsz=640

# 생성된 yolov8n.onnx를 프로젝트 실행 폴더(bin/Debug)에 복사
```

또는 [Ultralytics GitHub Releases](https://github.com/ultralytics/assets/releases)에서 직접 다운로드 가능합니다.

**모델 사양:**
- 입력: `1 × 3 × 640 × 640` (RGB, float32, 0~1 정규화)
- 출력: `1 × 84 × 8400` (cx, cy, w, h + 80 class confidences)
- COCO 클래스 0 = "person"

---

## 빌드 및 실행

### 1단계: 프로젝트 생성

Visual Studio 2022에서 **WPF 앱(.NET 8.0)** 프로젝트를 새로 만듭니다.
- 프로젝트 이름: `Safety_MotorControl`

### 2단계: NuGet 패키지 설치

패키지 관리자 콘솔에서:

```
Install-Package System.IO.Ports -Version 8.0.0
Install-Package OpenCvSharp4 -Version 4.9.0.20240103
Install-Package OpenCvSharp4.Extensions -Version 4.9.0.20240103
Install-Package OpenCvSharp4.runtime.win -Version 4.9.0.20240103
Install-Package Microsoft.ML.OnnxRuntime.Gpu -Version 1.17.1
```

### 3단계: 소스코드 배치

다운로드한 파일들을 아래 경로에 배치합니다.

```
Safety_MotorControl/
├── Safety_MotorControl.csproj       ← 프로젝트 파일 교체
├── App.xaml
├── App.xaml.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── Models/InverterModel.cs
├── Services/ModbusRtuService.cs
├── Services/CameraService.cs
├── Services/YoloDetectionService.cs
├── ViewModels/RelayCommand.cs
├── ViewModels/MainViewModel.cs
└── Converters/BoolToColorConverter.cs
```

### 4단계: 모델 파일 배치

`yolov8n.onnx`를 출력 디렉토리(`bin/Debug/net8.0-windows/`)에 복사합니다.

### 5단계: 빌드 및 실행

```
Ctrl+B (빌드) → F5 (실행)
```

---

## 사용 방법

### 기본 순서

1. **LOAD MODEL** → `yolov8n.onnx` 파일 선택 (GPU/CPU 모드 자동 표시)
2. **카메라 START** → USB 카메라 영상 시작 (CAM 인덱스 0이 기본)
3. **CONNECT** → COM 포트 선택 후 인버터 연결
4. **FWD / REV** → 모터 정방향/역방향 운전
5. 카메라에 사람 감지 시 → **자동 STOP + 빨간 경고**
6. 사람이 2초 이상 사라지면 → 안전정지 해제 → 다시 운전 가능

### UI 레이아웃

```
┌─────────────────────────────┬──────────────────┐
│  ⚡ SAFETY MOTOR CONTROL    │ 🎥 YOLO CAMERA   │
│  │ MODBUS RTU + YOLO        │  CAM [0] ▶ ■     │
├─────────────────────────────│  LOAD MODEL      │
│  PORT [COM3] BAUD [9600]    │  GPU (CUDA)      │
│  CONNECT / DISCONNECT       ├──────────────────┤
├─────────────────────────────│  ☑ 감지  CONF 0.45│
│  ┌────────┬────────┬──────┐ │  안전 감지 대기중  │
│  │ FREQ   │ CURRENT│STATUS│ ├──────────────────┤
│  │ 30.00  │  0.12  │ 정방향│ │                  │
│  │  Hz    │   A    │      │ │  카메라 영상      │
│  └────────┴────────┴──────┘ │  + 바운딩박스     │
├─────────────────────────────│                  │
│  ▶FWD  ◀REV  ■STOP         │                  │
│  FREQ [30.0] Hz  SET FREQ   ├──────────────────┤
├─────────────────────────────│  DETECTED: 0 명   │
│  COMM LOG                   │                  │
│  [12:34:56.789] TX: ...     │                  │
│  [12:34:56.840] RX: ...     │                  │
└─────────────────────────────┴──────────────────┘
```

### 설정 조정

| 항목 | 위치 | 설명 |
|------|------|------|
| 카메라 인덱스 | CAM 입력란 | USB 카메라가 여러 대일 때 0, 1, 2... |
| 신뢰도 임계값 | CONF 입력란 | 기본 0.45 (높이면 정확도↑ 감도↓) |
| 감지 ON/OFF | 감지 체크박스 | 체크 해제 시 YOLO 감지 비활성화 |
| 안전 해제 대기 | 코드 내 상수 | `SafetyClearSeconds = 2.0` (초) |

---

## Modbus RTU 레지스터 맵

### 쓰기 레지스터

| 주소 | 이름 | 단위 | 설명 |
|------|------|------|------|
| `0x0004` | REG_FREQ_SET | 0.01 Hz | 주파수 설정 (3000 = 30.00Hz) |
| `0x0005` | REG_CMD | - | 운전 명령 |

### 운전 명령

| 값 | 동작 |
|----|------|
| `0x0001` | 정지 (STOP) |
| `0x0002` | 정방향 운전 (FWD) |
| `0x0004` | 역방향 운전 (REV) |

### 읽기 레지스터

| 주소 | 이름 | 단위 | 설명 |
|------|------|------|------|
| `0x0008` | REG_STATUS | - | 인버터 상태 |
| `0x0009` | REG_FREQ_OUT | 0.01 Hz | 출력 주파수 |
| `0x000A` | REG_CURR_OUT | 0.01 A | 출력 전류 |

---

## 안전 정지 로직 상세

```
[카메라 프레임 수신] (백그라운드 스레드, ~30fps)
    │
    ▼
[YOLO 추론] → person 클래스 감지 목록
    │
    ├─ 감지됨 (count > 0)
    │   ├─ _lastDetectionTime = 현재시각
    │   ├─ SafetyStopActive = true
    │   └─ 인버터 운전 중 && 아직 STOP 미전송?
    │       └─ YES → Modbus STOP 전송 + 상태=Stopped
    │                _safetyStopSent = true (중복 방지)
    │
    └─ 미감지 (count == 0)
        └─ (현재시각 - _lastDetectionTime) > 2초?
            ├─ YES → SafetyStopActive = false
            │        _safetyStopSent = false (재감지 대비)
            └─ NO  → SafetyStopActive 유지 (대기 중)
```

**안전 정지 중 제한 사항:**
- FWD / REV 버튼의 `CanExecute`가 `false` → 클릭 불가
- 사람이 사라진 후 2초가 지나야 해제
- 해제 후 수동으로 FWD/REV를 눌러야 재운전 (자동 재시작 없음)

---

## 게이지 스케일 조정

### 주파수 게이지 (기본 60Hz)

`MainWindow.xaml`에서 `OutputFrequency` 바인딩의 `ConverterParameter` 수정:

```xml
ConverterParameter=60    ← 글로우 아크 + 선명 아크 동일
Text="60"                ← 눈금 라벨
```

### 전류 게이지 (기본 5A)

```xml
ConverterParameter=5     ← 글로우 아크 + 선명 아크 통일됨
Text="5"                 ← 눈금 라벨
```

---

## 인버터 사전 설정

인버터 파라미터를 아래와 같이 설정해야 Modbus 통신이 가능합니다.

| 파라미터 | 설명 | 설정값 |
|----------|------|--------|
| `dr.91` | 통신 프로토콜 | Modbus RTU |
| `dr.92` | Slave ID | 1 (프로그램 기본값) |
| `dr.93` | 통신 속도 | 9600 (프로그램 기본값) |

---

## 주의사항

- **전류 단위**: 현재 `/ 1000.0` 변환 중. 실측 후 `/ 100.0` 또는 `/ 10.0`으로 조정 필요
- **CUDA 버전**: OnnxRuntime.Gpu 1.17.1은 CUDA 11.8 기준. CUDA 12.x 사용 시 버전 호환 확인 필요
- **안전 경고**: 이 프로그램은 교육/데모 목적입니다. 실제 산업 현장의 안전 시스템을 대체하지 않습니다
- **카메라 해상도**: 기본 640×480. 고해상도 시 YOLO 추론 속도에 영향 없음 (내부 640×640 리사이즈)
