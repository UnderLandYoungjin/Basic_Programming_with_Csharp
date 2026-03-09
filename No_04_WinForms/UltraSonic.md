

https://github.com/user-attachments/assets/d82cc0dd-0765-4738-a078-216622a68201

# 아두이노 HC-04 초음파 센서 + 웹캠 WinForms 모니터

> **학습 목표**: 아두이노 시리얼 통신, 실시간 그래프, 웹캠 영상을 WinForms에서 통합 구현한다.

---

## 1. 전체 구성

```
HC-04 초음파 센서
       ↓
Arduino UNO (시리얼 송신)
       ↓ USB
PC WinForms 앱
  ├── 거리값 숫자 표시
  ├── 실시간 그래프 (PictureBox)
  └── 웹캠 영상 (OpenCvSharp)
```

---

## 2. HC-04 배선

| HC-04 핀 | 아두이노 UNO 핀 |
|----------|----------------|
| VCC      | 5V             |
| GND      | GND            |
| TRIG     | 9번            |
| ECHO     | 10번           |

---

## 3. 아두이노 코드

```cpp
// HC-04 초음파 센서
// TRIG : 9번 핀 / ECHO : 10번 핀

const int TRIG = 9;
const int ECHO = 10;

void setup() {
  Serial.begin(9600);
  pinMode(TRIG, OUTPUT);
  pinMode(ECHO, INPUT);
}

void loop() {
  // 초음파 발사
  digitalWrite(TRIG, LOW);
  delayMicroseconds(2);
  digitalWrite(TRIG, HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIG, LOW);

  // 거리 계산 (cm)
  long duration = pulseIn(ECHO, HIGH);
  float distance = duration * 0.034 / 2.0;

  // 유효 범위 필터 (2cm ~ 400cm)
  if (distance >= 2 && distance <= 400) {
    Serial.println(distance);
  } else {
    Serial.println(-1);  // 범위 초과 신호
  }

  delay(100);  // 100ms 간격 송신
}
```

> 💡 **업로드 순서**: 아두이노 업로드 → 시리얼 모니터에서 숫자 확인 → WinForms 연결

---

## 4. NuGet 패키지 설치

패키지 관리자 콘솔 (`도구 → NuGet 패키지 관리자 → 패키지 관리자 콘솔`) 에서 순서대로 입력:

```
Install-Package System.IO.Ports
Install-Package OpenCvSharp4.Windows
Install-Package OpenCvSharp4.Extensions
```

---

## 5. WinForms 컨트롤 배치

### 디자이너에서 드래그앤드롭

| 컨트롤 | Name 속성 | 역할 |
|--------|-----------|------|
| `ComboBox` | `cboPort` | COM 포트 선택 |
| `Button` | `btnConnect` | 연결/해제 토글 |
| `Label` | `lblDistance` | 거리값 크게 표시 |
| `Label` | `lblStatus` | 연결 상태 표시 |
| `PictureBox` | `picGraph` | 실시간 그래프 |
| `Timer` | `timer1` | UI 갱신 주기 |
| `ComboBox` | `cboCam` | 카메라 선택 |
| `PictureBox` | `picCam` | 웹캠 영상 표시 |

> ⚠️ **주의**: `SerialPort`는 .NET 6 이상에서 디자이너 지원 안됨 → 코드로 직접 선언

### 버튼 이벤트 연결
`btnConnect` 더블클릭 → `btnConnect_Click` 자동 연결

---

## 6. WinForms 전체 코드 (Form1.cs)

```csharp
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace WinFormsApp13
{
    public partial class Form1 : Form
    {
        // ── 시리얼 필드 ────────────────────────────────────────
        private SerialPort serialPort1 = new SerialPort();

        private List<float> _distanceBuffer = new List<float>();
        private const int   BUFFER_SIZE     = 100;
        private const float MAX_DISTANCE    = 100f;

        private float _currentDistance = 0f;
        private bool  _isConnected     = false;

        // ── 웹캠 필드 ──────────────────────────────────────────
        private VideoCapture _capture;
        private System.Windows.Forms.Timer _camTimer = new System.Windows.Forms.Timer();
        private bool _isCamRunning = false;

        public Form1()
        {
            InitializeComponent();

            // COM 포트 목록 로드
            cboPort.Items.AddRange(SerialPort.GetPortNames());
            if (cboPort.Items.Count > 0) cboPort.SelectedIndex = 0;

            // SerialPort 설정
            serialPort1.BaudRate      = 9600;
            serialPort1.DataBits      = 8;
            serialPort1.Parity        = Parity.None;
            serialPort1.StopBits      = StopBits.One;
            serialPort1.DataReceived += SerialPort1_DataReceived;

            // 시리얼 타이머
            timer1.Interval  = 50;
            timer1.Tick      += Timer1_Tick;

            // 카메라 타이머 (30fps)
            _camTimer.Interval = 33;
            _camTimer.Tick    += CamTimer_Tick;

            // 초기 UI
            lblDistance.Text      = "-- cm";
            lblDistance.Font      = new Font("Arial", 36, FontStyle.Bold);
            lblDistance.ForeColor = Color.Black;
            lblStatus.Text        = "● 연결 안됨";
            lblStatus.ForeColor   = Color.Red;

            // 이벤트
            picGraph.Paint += picGraph_Paint;
            cboCam.SelectedIndexChanged += CboCam_SelectedIndexChanged;

            // Form Load
            this.Load += Form1_Load;
        }

        // ── Form Load: 카메라 탐색 ─────────────────────────────
        private void Form1_Load(object sender, EventArgs e)
        {
            cboCam.Items.Clear();
            for (int i = 0; i < 4; i++)
            {
                using (var test = new VideoCapture(i))
                {
                    if (test.IsOpened())
                        cboCam.Items.Add($"카메라 {i}번");
                }
            }

            if (cboCam.Items.Count > 0)
            {
                cboCam.SelectedIndex = 0;
                StartCamera(0);
            }
            else
            {
                MessageBox.Show("연결된 카메라가 없습니다.", "알림");
            }
        }

        // ── 카메라 선택 변경 ───────────────────────────────────
        private void CboCam_SelectedIndexChanged(object sender, EventArgs e)
        {
            StartCamera(cboCam.SelectedIndex);
        }

        // ── 카메라 시작 ────────────────────────────────────────
        private void StartCamera(int index)
        {
            _camTimer.Stop();
            _capture?.Release();
            _capture?.Dispose();

            _capture = new VideoCapture(index);

            if (_capture.IsOpened())
            {
                _isCamRunning = true;
                _camTimer.Start();
            }
            else
            {
                MessageBox.Show($"카메라 {index}번을 열 수 없습니다.", "오류");
            }
        }

        // ── 카메라 프레임 갱신 ─────────────────────────────────
        private void CamTimer_Tick(object sender, EventArgs e)
        {
            if (!_isCamRunning || _capture == null) return;

            using (var frame = new Mat())
            {
                _capture.Read(frame);
                if (frame.Empty()) return;

                Cv2.Resize(frame, frame, new OpenCvSharp.Size(picCam.Width, picCam.Height));

                var old = picCam.Image;
                picCam.Image = BitmapConverter.ToBitmap(frame);
                old?.Dispose();
            }
        }

        // ── 연결/해제 버튼 ─────────────────────────────────────
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!_isConnected)
            {
                try
                {
                    serialPort1.PortName = cboPort.SelectedItem.ToString();
                    serialPort1.Open();
                    _isConnected = true;
                    timer1.Start();

                    btnConnect.Text     = "연결 해제";
                    lblStatus.Text      = "● 연결됨";
                    lblStatus.ForeColor = Color.Green;
                    cboPort.Enabled     = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"연결 실패: {ex.Message}", "오류");
                }
            }
            else
            {
                Disconnect();
            }
        }

        // ── 시리얼 데이터 수신 (별도 스레드) ──────────────────
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string raw = serialPort1.ReadLine().Trim();
                if (float.TryParse(raw, out float dist))
                {
                    _currentDistance = dist;
                    lock (_distanceBuffer)
                    {
                        _distanceBuffer.Add(dist);
                        if (_distanceBuffer.Count > BUFFER_SIZE)
                            _distanceBuffer.RemoveAt(0);
                    }
                }
            }
            catch { }
        }

        // ── 타이머: UI 갱신 ────────────────────────────────────
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (_currentDistance < 0)
            {
                lblDistance.Text      = "범위 초과";
                lblDistance.ForeColor = Color.Gray;
            }
            else
            {
                lblDistance.Text = $"{_currentDistance:F1} cm";

                if      (_currentDistance < 20) lblDistance.ForeColor = Color.Red;
                else if (_currentDistance < 50) lblDistance.ForeColor = Color.Orange;
                else                            lblDistance.ForeColor = Color.Green;
            }

            picGraph.Invalidate();
        }

        // ── 그래프 그리기 ──────────────────────────────────────
        private void picGraph_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black);

            int w = picGraph.Width;
            int h = picGraph.Height;

            // 격자선 + Y축 라벨
            using (Pen gridPen = new Pen(Color.FromArgb(50, 50, 50)))
            {
                for (int i = 0; i <= 4; i++)
                {
                    int   y     = h * i / 4;
                    float label = MAX_DISTANCE - (MAX_DISTANCE * i / 4);
                    g.DrawLine(gridPen, 0, y, w, y);
                    g.DrawString($"{label:F0}cm", new Font("Arial", 7),
                                 Brushes.DimGray, 2, y + 2);
                }
            }

            // 데이터 라인
            List<float> snapshot;
            lock (_distanceBuffer)
            {
                snapshot = new List<float>(_distanceBuffer);
            }

            if (snapshot.Count < 2) return;

            using (Pen linePen = new Pen(Color.Cyan, 2))
            {
                for (int i = 1; i < snapshot.Count; i++)
                {
                    float Clamp(float v) => Math.Max(0, Math.Min(v, MAX_DISTANCE));

                    float x1 = (float)(i - 1) / BUFFER_SIZE * w;
                    float y1 = h - (Clamp(snapshot[i - 1]) / MAX_DISTANCE * h);
                    float x2 = (float)i       / BUFFER_SIZE * w;
                    float y2 = h - (Clamp(snapshot[i])     / MAX_DISTANCE * h);

                    g.DrawLine(linePen, x1, y1, x2, y2);
                }
            }

            // 현재값 노란 점
            if (snapshot.Count > 0)
            {
                float last = snapshot[snapshot.Count - 1];
                float cx   = (float)(snapshot.Count - 1) / BUFFER_SIZE * w;
                float cy   = h - (Math.Min(last, MAX_DISTANCE) / MAX_DISTANCE * h);
                g.FillEllipse(Brushes.Yellow, cx - 4, cy - 4, 8, 8);
            }
        }

        // ── 연결 해제 ──────────────────────────────────────────
        private void Disconnect()
        {
            timer1.Stop();
            if (serialPort1.IsOpen) serialPort1.Close();
            _isConnected = false;

            btnConnect.Text     = "연결";
            lblStatus.Text      = "● 연결 안됨";
            lblStatus.ForeColor = Color.Red;
            cboPort.Enabled     = true;
        }

        // ── 폼 닫을 때 정리 ────────────────────────────────────
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _camTimer.Stop();
            _capture?.Release();
            _capture?.Dispose();
            Disconnect();
            base.OnFormClosing(e);
        }
    }
}
```

---

## 7. 핵심 개념 해설

### 시리얼 통신 흐름

```
아두이노 Serial.println(distance)
         ↓ USB
SerialPort.DataReceived 이벤트 발생 (별도 스레드)
         ↓
_distanceBuffer 에 저장 (lock으로 스레드 충돌 방지)
         ↓
timer1 (50ms) → UI 갱신 + picGraph.Invalidate()
```

### 주요 개념 요약

| 개념 | 설명 |
|------|------|
| `SerialPort.DataReceived` | 데이터 수신 시 자동 호출 (별도 스레드) |
| `lock` | 멀티스레드 충돌 방지 |
| `Timer + Invalidate()` | 주기적 UI 갱신 패턴 |
| `picGraph_Paint` | PictureBox에 직접 그래프 그리기 |
| `VideoCapture` | OpenCV 카메라 캡처 객체 |
| `BitmapConverter` | OpenCV Mat → WinForms Bitmap 변환 |
| `Form1_Load` | 폼 시작 시 카메라 자동 탐색 |

### 거리별 색상 로직

```csharp
if      (distance < 20cm) → 빨강  // 위험 근접
else if (distance < 50cm) → 주황  // 주의
else                      → 초록  // 안전
```

---

## 8. 자주 발생하는 오류

| 오류 | 원인 | 해결 |
|------|------|------|
| `SerialPort` 없음 | NuGet 미설치 | `Install-Package System.IO.Ports` |
| `BitmapConverter` 없음 | Extensions 미설치 | `Install-Package OpenCvSharp4.Extensions` |
| 카메라 목록 비어있음 | 초기화 타이밍 문제 | `Form1_Load` 에서 탐색 |
| `lblStatus_Click` 오류 | 실수로 라벨 더블클릭 | Designer.cs에서 해당 줄 삭제 |
| COM 포트 연결 실패 | 다른 프로그램 점유 | 시리얼 모니터 닫고 재시도 |

---

## 9. 실습 과제

1. 거리 20cm 이하일 때 **경고음** 추가하기 (`System.Media.SystemSounds`)
2. 그래프 최대 거리를 `nudMaxDistance`로 **동적 조절** 가능하게 하기
3. 측정 데이터를 **CSV 파일로 저장**하는 버튼 추가하기
4. 웹캠 화면에 현재 거리값을 **오버레이 텍스트**로 표시하기 (`Cv2.PutText`)

---

