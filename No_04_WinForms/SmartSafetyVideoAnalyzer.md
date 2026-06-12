# C# 동영상 기반 Vision AI 안전 감지 시스템 만들기

> 프로젝트명: **Smart Safety Video Analyzer**
> 대상: C# 초급자 (WinForms 기초 경험자)
> 개발 환경: Visual Studio 2022, .NET 8, WinForms
> 입력 데이터: `sample_video.mp4` (웹캠 불필요)

---

## 1. 프로젝트 개요

### 무엇을 만드는가

공장이나 작업 현장을 촬영한 동영상 파일을 불러와서, 화면에 **위험 구역(빨간 사각형)** 을 표시하고, 영상 속에서 **움직이는 물체(초록 사각형)** 를 자동으로 찾아낸 뒤, 움직이는 물체가 위험 구역 안으로 들어오면 **"위험 감지"** 로 판정하고 그 기록을 **SQLite 데이터베이스에 저장**하는 프로그램이다.

실제 산업 현장에서 쓰이는 비전 기반 안전 모니터링 시스템의 축소판이라고 보면 된다. 로봇 작업 반경에 사람이 들어오면 경고를 띄우거나 설비를 정지시키는 시스템이 모두 이 구조에서 출발한다.

### 왜 웹캠이 아니라 동영상 파일인가

수업에서 웹캠을 쓰면 문제가 많다.

- 학생마다 카메라 유무, 드라이버 상태, 해상도가 전부 다르다.
- 카메라 앞에서 직접 움직여야 테스트가 되므로 결과 재현이 안 된다.
- 카메라가 안 잡히는 PC에서는 수업 진행 자체가 막힌다.

동영상 파일을 쓰면 **모든 학생이 같은 입력으로 같은 결과**를 얻는다. 디버깅할 때도 같은 장면을 몇 번이고 다시 돌려볼 수 있다. 그리고 코드 입장에서는 웹캠과 동영상 파일이 거의 같다. OpenCV의 `VideoCapture`는 파일 경로 대신 카메라 번호(0)를 넣으면 그대로 웹캠 모드가 된다. 즉, 지금 만든 프로그램은 나중에 한 줄만 바꾸면 실시간 카메라용이 된다.

### 이 프로젝트로 배우는 것

| 분야 | 배우는 내용 |
|---|---|
| C# 기초 | 클래스 분리, 이벤트 처리, Timer, 폼 간 데이터 전달 |
| 영상 처리 | 프레임 개념, 그레이스케일, 프레임 차이, 윤곽선 검출 |
| 컴퓨터 비전 | 움직임 감지 원리, 사각형 충돌 판정 |
| 데이터베이스 | SQLite 테이블 생성, INSERT, SELECT, Dapper 사용법 |
| 설계 | Service 클래스로 역할을 나누는 구조 설계 |

---

## 2. 최종 완성 화면

```text
┌──────────────────────────────────────┐
│ Smart Safety Video Analyzer          │
├──────────────────────────────────────┤
│                                      │
│      [동영상 재생 화면]              │
│                                      │
│      빨간 사각형: 위험 구역          │
│      초록 사각형: 감지된 움직임      │
│                                      │
│      위험 시 화면 좌상단에           │
│      "DANGER!" 빨간 글씨 표시        │
│                                      │
├──────────────────────────────────────┤
│ 상태: 정상 / 위험 감지               │
│ 현재 프레임: 152                     │
│ 감지 수: 3                           │
│ [영상 열기] [시작] [정지] [로그 보기]│
└──────────────────────────────────────┘
```

동작 흐름:

1. [영상 열기] → mp4 파일 선택
2. [시작] → 영상이 재생되면서 움직임 박스가 실시간으로 그려짐
3. 움직임 박스가 빨간 구역과 겹치는 순간 상태가 "위험 감지"로 바뀌고 DB에 저장
4. [로그 보기] → 지금까지 저장된 위험 기록을 표로 확인

---

## 3. 전체 기능 목록

| 기능 | 설명 | 난이도 |
|---|---|---|
| 동영상 열기 | OpenFileDialog로 MP4 파일을 선택해서 불러옴 | 쉬움 |
| 동영상 재생 | Timer로 프레임을 하나씩 읽어 PictureBox에 출력 | 쉬움 |
| 위험 구역 표시 | 고정 좌표에 빨간 사각형을 매 프레임 그림 | 쉬움 |
| 움직임 감지 | 이전 프레임과 현재 프레임의 차이로 움직임 영역 검출 | 보통 |
| 위험 판정 | 움직임 박스가 위험 구역과 겹치면 위험 처리 | 보통 |
| 경고 표시 | 위험 시 화면에 "DANGER!" 문구와 상태 라벨 변경 | 쉬움 |
| 로그 저장 | 위험 발생 시각, 프레임 번호, 좌표를 SQLite에 저장 | 보통 |
| 로그 조회 | DataGridView로 저장된 기록 전체 조회 | 쉬움 |

WinForms와 WPF 중에서는 **WinForms를 선택**한다. PictureBox에 Bitmap을 꽂는 방식이 직관적이고, OpenCvSharp의 `BitmapConverter`가 변환을 한 줄로 해결해 주기 때문에 초급자가 영상 출력에서 막힐 일이 거의 없다. WPF는 이미지 바인딩과 `WriteableBitmap` 개념이 추가로 필요해서 이 단계에서는 부담이 된다.

---

## 4. 수업용 단계별 개발 순서

### 4-1. Visual Studio 프로젝트 만들기

1. Visual Studio 2022 실행 → 새 프로젝트 만들기
2. **Windows Forms 앱** (.NET 8) 선택 — ".NET Framework"가 붙은 템플릿이 아니라 그냥 "Windows Forms 앱"을 골라야 한다.
3. 프로젝트 이름: `SmartSafetyVideoAnalyzer`
4. 생성 후 솔루션 탐색기에서 `Form1.cs`를 `MainForm.cs`로 이름 변경 (이름 바꿀 때 "참조도 함께 바꾸시겠습니까?" → 예)

`.csproj` 파일을 열어 아래처럼 되어 있는지 확인한다.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <Nullable>disable</Nullable>
    <UseWindowsForms>true</UseWindowsForms>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

`<Nullable>disable</Nullable>`로 두면 초급 단계에서 null 경고에 시달리지 않는다.

### 4-2. NuGet 패키지 설치

도구 → NuGet 패키지 관리자 → 패키지 관리자 콘솔에서 아래를 순서대로 실행한다.

```powershell
Install-Package OpenCvSharp4
Install-Package OpenCvSharp4.runtime.win
Install-Package OpenCvSharp4.Extensions
Install-Package Microsoft.Data.Sqlite
Install-Package Dapper
```

설치가 끝나면 한 번 빌드(Ctrl+Shift+B)해서 오류가 없는지 확인한다. **여기서 빌드가 안 되면 다음 단계로 넘어가지 말 것.**

### 4-3. 폴더(클래스) 구성

솔루션 탐색기에서 프로젝트 우클릭 → 추가 → 새 폴더로 `Models`, `Services` 폴더를 만든다. 이후 단계에서 각 폴더에 클래스를 추가한다.

### 4-4. 모델 클래스 만들기 — `Models/DetectionLog.cs`

위험 기록 한 건을 담는 그릇이다. DB 테이블의 한 행과 1:1로 대응한다.

```csharp
namespace SmartSafetyVideoAnalyzer.Models
{
    public class DetectionLog
    {
        public int Id { get; set; }              // DB가 자동으로 붙여주는 번호
        public string DetectedAt { get; set; }   // 감지 시각 (문자열로 저장)
        public int FrameNumber { get; set; }     // 몇 번째 프레임에서 감지됐는지
        public int ObjectX { get; set; }         // 감지된 물체 박스의 좌표와 크기
        public int ObjectY { get; set; }
        public int ObjectWidth { get; set; }
        public int ObjectHeight { get; set; }
        public string Message { get; set; }      // "위험 구역 침입" 같은 설명
    }
}
```

### 4-5. 동영상 서비스 만들기 — `Services/VideoService.cs`

동영상을 열고, 프레임을 한 장씩 꺼내주는 역할만 담당한다.

```csharp
using OpenCvSharp;

namespace SmartSafetyVideoAnalyzer.Services
{
    public class VideoService : IDisposable
    {
        private VideoCapture _capture;

        public bool IsOpened => _capture != null && _capture.IsOpened();
        public int FrameNumber { get; private set; }
        public double Fps => (_capture != null && _capture.Fps > 0) ? _capture.Fps : 30.0;

        // 동영상 파일을 연다. 성공하면 true
        public bool Open(string filePath)
        {
            _capture?.Dispose();
            _capture = new VideoCapture(filePath);
            FrameNumber = 0;
            return _capture.IsOpened();
        }

        // 프레임을 한 장 읽는다. 영상이 끝나면 null 반환
        public Mat ReadFrame()
        {
            if (!IsOpened) return null;

            var frame = new Mat();
            if (!_capture.Read(frame) || frame.Empty())
            {
                frame.Dispose();
                return null;   // 영상 끝
            }

            FrameNumber++;
            return frame;
        }

        public void Dispose()
        {
            _capture?.Dispose();
            _capture = null;
        }
    }
}
```

> 나중에 웹캠으로 바꾸려면 `new VideoCapture(filePath)`를 `new VideoCapture(0)`으로 바꾸기만 하면 된다.

### 4-6. 움직임 감지 서비스 만들기 — `Services/MotionDetectionService.cs`

이 프로젝트의 핵심이다. 이전 프레임과 현재 프레임을 비교해서 달라진 영역을 사각형 목록으로 돌려준다.

```csharp
using OpenCvSharp;

namespace SmartSafetyVideoAnalyzer.Services
{
    public class MotionDetectionService : IDisposable
    {
        private Mat _prevGray;                 // 직전 프레임(흑백 변환본) 보관
        private const double MinArea = 500;    // 이보다 작은 변화는 노이즈로 보고 무시

        // 현재 프레임에서 움직임 영역들을 찾아 사각형 목록으로 반환
        public List<Rect> Detect(Mat frame)
        {
            var boxes = new List<Rect>();

            // 1) 컬러 → 흑백 변환 (계산을 단순하게 하기 위해)
            var gray = new Mat();
            Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);

            // 2) 블러 처리 (작은 잡음 제거)
            Cv2.GaussianBlur(gray, gray, new Size(21, 21), 0);

            // 첫 프레임이면 비교 대상이 없으므로 저장만 하고 끝
            if (_prevGray == null)
            {
                _prevGray = gray;
                return boxes;
            }

            // 3) 이전 프레임과의 차이 계산
            using var diff = new Mat();
            Cv2.Absdiff(_prevGray, gray, diff);

            // 4) 차이가 25 이상인 픽셀만 흰색(255)으로 만든다
            Cv2.Threshold(diff, diff, 25, 255, ThresholdTypes.Binary);

            // 5) 흰 영역을 부풀려서 끊어진 덩어리를 이어 붙인다
            Cv2.Dilate(diff, diff, null, iterations: 2);

            // 6) 흰 덩어리들의 윤곽선을 찾는다
            Cv2.FindContours(diff, out OpenCvSharp.Point[][] contours, out _,
                RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            // 7) 충분히 큰 덩어리만 사각형으로 변환
            foreach (var contour in contours)
            {
                if (Cv2.ContourArea(contour) < MinArea) continue;
                boxes.Add(Cv2.BoundingRect(contour));
            }

            // 8) 현재 프레임을 다음 비교를 위해 보관
            _prevGray.Dispose();
            _prevGray = gray;

            return boxes;
        }

        // 새 영상을 열 때 이전 프레임 기억을 지운다
        public void Reset()
        {
            _prevGray?.Dispose();
            _prevGray = null;
        }

        public void Dispose() => Reset();
    }
}
```

### 4-7. 로그 서비스 만들기 — `Services/LogService.cs`

SQLite 파일을 만들고, 위험 기록을 저장/조회한다.

```csharp
using Dapper;
using Microsoft.Data.Sqlite;
using SmartSafetyVideoAnalyzer.Models;

namespace SmartSafetyVideoAnalyzer.Services
{
    public class LogService
    {
        private readonly string _connectionString;

        public LogService()
        {
            // 실행 파일 옆에 Data 폴더를 만들고 그 안에 DB 파일 생성
            string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            Directory.CreateDirectory(dataDir);

            string dbPath = Path.Combine(dataDir, "safety_logs.db");
            _connectionString = $"Data Source={dbPath}";

            CreateTable();
        }

        private void CreateTable()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Execute(@"
                CREATE TABLE IF NOT EXISTS DetectionLog (
                    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
                    DetectedAt   TEXT    NOT NULL,
                    FrameNumber  INTEGER NOT NULL,
                    ObjectX      INTEGER,
                    ObjectY      INTEGER,
                    ObjectWidth  INTEGER,
                    ObjectHeight INTEGER,
                    Message      TEXT
                );");
        }

        public void Insert(DetectionLog log)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Execute(@"
                INSERT INTO DetectionLog
                    (DetectedAt, FrameNumber, ObjectX, ObjectY, ObjectWidth, ObjectHeight, Message)
                VALUES
                    (@DetectedAt, @FrameNumber, @ObjectX, @ObjectY, @ObjectWidth, @ObjectHeight, @Message);",
                log);
        }

        public List<DetectionLog> GetAll()
        {
            using var conn = new SqliteConnection(_connectionString);
            return conn.Query<DetectionLog>(
                "SELECT * FROM DetectionLog ORDER BY Id DESC;").ToList();
        }
    }
}
```

### 4-8. 메인 화면 만들기 — `MainForm.cs`

디자이너로 컨트롤을 배치해도 되지만, 수업에서는 학생마다 배치가 달라져 코드가 어긋나기 쉽다. 그래서 **컨트롤을 전부 코드로 생성**한다. 아래 코드를 통째로 붙여 넣으면 그대로 동작한다. (기존 `MainForm.Designer.cs`의 `InitializeComponent`는 그대로 두고, 그 뒤에서 우리가 만든 `BuildUi()`가 화면을 구성한다.)

```csharp
using OpenCvSharp;
using OpenCvSharp.Extensions;
using SmartSafetyVideoAnalyzer.Models;
using SmartSafetyVideoAnalyzer.Services;

namespace SmartSafetyVideoAnalyzer
{
    public partial class MainForm : Form
    {
        // ── 화면 컨트롤 ─────────────────────────────
        private PictureBox picVideo;
        private Label lblStatus;
        private Label lblFrame;
        private Label lblCount;
        private Button btnOpen;
        private Button btnStart;
        private Button btnStop;
        private Button btnLog;
        private System.Windows.Forms.Timer timer;

        // ── 서비스 ─────────────────────────────────
        private readonly VideoService _video = new VideoService();
        private readonly MotionDetectionService _motion = new MotionDetectionService();
        private readonly LogService _log = new LogService();

        // ── 상태 값 ────────────────────────────────
        // 위험 구역: (x=200, y=150)에서 가로 200, 세로 200 (영상 크기에 맞게 조절)
        private Rect _dangerZone = new Rect(200, 150, 200, 200);
        private int _detectCount = 0;
        private int _lastLogFrame = -1000;   // 같은 위험을 매 프레임 중복 저장하지 않기 위한 기억

        public MainForm()
        {
            InitializeComponent();
            BuildUi();
        }

        // 화면을 코드로 구성한다
        private void BuildUi()
        {
            Text = "Smart Safety Video Analyzer";
            ClientSize = new System.Drawing.Size(800, 640);
            StartPosition = FormStartPosition.CenterScreen;

            picVideo = new PictureBox
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(780, 520),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Black
            };

            lblStatus = new Label { Location = new System.Drawing.Point(10, 540), AutoSize = true, Text = "상태: 대기 중", Font = new Font("맑은 고딕", 11, FontStyle.Bold) };
            lblFrame  = new Label { Location = new System.Drawing.Point(200, 542), AutoSize = true, Text = "현재 프레임: 0" };
            lblCount  = new Label { Location = new System.Drawing.Point(360, 542), AutoSize = true, Text = "감지 수: 0" };

            btnOpen  = new Button { Location = new System.Drawing.Point(10, 575),  Size = new System.Drawing.Size(110, 35), Text = "영상 열기" };
            btnStart = new Button { Location = new System.Drawing.Point(130, 575), Size = new System.Drawing.Size(110, 35), Text = "시작", Enabled = false };
            btnStop  = new Button { Location = new System.Drawing.Point(250, 575), Size = new System.Drawing.Size(110, 35), Text = "정지", Enabled = false };
            btnLog   = new Button { Location = new System.Drawing.Point(370, 575), Size = new System.Drawing.Size(110, 35), Text = "로그 보기" };

            btnOpen.Click  += BtnOpen_Click;
            btnStart.Click += BtnStart_Click;
            btnStop.Click  += BtnStop_Click;
            btnLog.Click   += BtnLog_Click;

            timer = new System.Windows.Forms.Timer();
            timer.Tick += Timer_Tick;

            Controls.AddRange(new Control[] { picVideo, lblStatus, lblFrame, lblCount, btnOpen, btnStart, btnStop, btnLog });

            FormClosing += (s, e) => { timer.Stop(); _video.Dispose(); _motion.Dispose(); };
        }

        // [영상 열기]
        private void BtnOpen_Click(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Title = "동영상 파일 선택",
                Filter = "동영상 파일|*.mp4;*.avi;*.mov;*.mkv"
            };
            if (dialog.ShowDialog() != DialogResult.OK) return;

            if (!_video.Open(dialog.FileName))
            {
                MessageBox.Show("동영상 파일을 열 수 없습니다.", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _motion.Reset();
            _detectCount = 0;
            _lastLogFrame = -1000;
            lblCount.Text = "감지 수: 0";
            lblStatus.Text = "상태: 준비 완료";
            lblStatus.ForeColor = Color.Black;

            // 영상의 FPS에 맞춰 타이머 간격 설정 (30fps → 약 33ms)
            timer.Interval = (int)(1000.0 / _video.Fps);

            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        // [시작]
        private void BtnStart_Click(object sender, EventArgs e)
        {
            timer.Start();
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            lblStatus.Text = "상태: 정상";
            lblStatus.ForeColor = Color.Black;
        }

        // [정지]
        private void BtnStop_Click(object sender, EventArgs e)
        {
            timer.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            lblStatus.Text = "상태: 일시 정지";
            lblStatus.ForeColor = Color.Black;
        }

        // [로그 보기]
        private void BtnLog_Click(object sender, EventArgs e)
        {
            var logs = _log.GetAll();
            using var form = new LogForm(logs);
            form.ShowDialog(this);
        }

        // 타이머가 깜빡일 때마다 프레임 한 장 처리 (프로그램의 심장)
        private void Timer_Tick(object sender, EventArgs e)
        {
            using Mat frame = _video.ReadFrame();

            // 영상이 끝났으면 정지
            if (frame == null)
            {
                timer.Stop();
                btnStart.Enabled = false;
                btnStop.Enabled = false;
                lblStatus.Text = "상태: 재생 종료";
                lblStatus.ForeColor = Color.Black;
                return;
            }

            // 1) 움직임 감지
            List<Rect> motionBoxes = _motion.Detect(frame);

            // 2) 위험 구역 그리기 (빨강)
            Cv2.Rectangle(frame, _dangerZone, Scalar.Red, 2);

            // 3) 움직임 박스 그리기 (초록) + 위험 판정
            bool danger = false;
            Rect dangerBox = default;

            foreach (var box in motionBoxes)
            {
                Cv2.Rectangle(frame, box, new Scalar(0, 255, 0), 2);

                if (box.IntersectsWith(_dangerZone))
                {
                    danger = true;
                    dangerBox = box;
                }
            }

            // 4) 위험 처리
            if (danger)
            {
                Cv2.PutText(frame, "DANGER!", new OpenCvSharp.Point(20, 60),
                    HersheyFonts.HersheySimplex, 1.8, Scalar.Red, 4);

                lblStatus.Text = "상태: 위험 감지";
                lblStatus.ForeColor = Color.Red;

                // 너무 자주 저장하지 않도록 약 1초(=FPS 프레임)에 한 번만 기록
                if (_video.FrameNumber - _lastLogFrame >= (int)_video.Fps)
                {
                    _detectCount++;
                    _lastLogFrame = _video.FrameNumber;

                    _log.Insert(new DetectionLog
                    {
                        DetectedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        FrameNumber = _video.FrameNumber,
                        ObjectX = dangerBox.X,
                        ObjectY = dangerBox.Y,
                        ObjectWidth = dangerBox.Width,
                        ObjectHeight = dangerBox.Height,
                        Message = "위험 구역 침입"
                    });
                }
            }
            else
            {
                lblStatus.Text = "상태: 정상";
                lblStatus.ForeColor = Color.Black;
            }

            // 5) 화면 출력 (이전 이미지는 반드시 Dispose해서 메모리 누수 방지)
            var oldImage = picVideo.Image;
            picVideo.Image = BitmapConverter.ToBitmap(frame);
            oldImage?.Dispose();

            lblFrame.Text = $"현재 프레임: {_video.FrameNumber}";
            lblCount.Text = $"감지 수: {_detectCount}";
        }
    }
}
```

### 4-9. 로그 조회 화면 만들기 — `LogForm.cs`

프로젝트에 클래스를 추가하고(폼 아님, 일반 클래스) 아래 코드를 넣는다.

```csharp
using SmartSafetyVideoAnalyzer.Models;

namespace SmartSafetyVideoAnalyzer
{
    public class LogForm : Form
    {
        public LogForm(List<DetectionLog> logs)
        {
            Text = $"감지 로그 (총 {logs.Count}건)";
            ClientSize = new System.Drawing.Size(760, 420);
            StartPosition = FormStartPosition.CenterParent;

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = logs
            };

            Controls.Add(grid);
        }
    }
}
```

### 4-10. Program.cs 확인

기본 생성된 `Program.cs`에서 마지막 줄이 `MainForm`을 실행하는지만 확인한다.

```csharp
namespace SmartSafetyVideoAnalyzer
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
```

### 4-11. 실행 및 테스트

1. F5로 실행
2. [영상 열기] → 사람이나 물체가 움직이는 mp4 선택 (없으면 스마트폰으로 30초 정도 직접 촬영해서 PC로 옮기면 된다)
3. [시작] → 초록 박스가 움직임을 따라다니는지 확인
4. 움직임이 빨간 구역에 들어가는 순간 "DANGER!" 표시 확인
5. [로그 보기] → 기록이 쌓였는지 확인
6. 프로그램을 껐다 켜고 [로그 보기] → **기록이 그대로 남아 있는지** 확인 (이것이 DB를 쓰는 이유다)

> 위험 구역 위치(`_dangerZone`)는 영상 해상도에 따라 조절해야 한다. 1920×1080 영상이라면 `new Rect(600, 400, 400, 400)` 정도로 키워서 테스트하자.

### 4-12. ONNX AI 모델 연결 방향 (확장 과제 안내)

지금 만든 구조에서 바뀌는 곳은 단 한 군데, `MotionDetectionService`다. "프레임 차이로 움직임을 찾는 클래스"를 "AI 모델로 사람을 찾는 클래스"로 교체하면 된다.

1. NuGet에서 `Microsoft.ML.OnnxRuntime` 설치
2. YOLOv8 같은 사전 학습 모델을 `.onnx` 파일로 준비
3. `PersonDetectionService` 클래스를 새로 만들어 프레임을 모델 입력 크기로 변환 → 추론 → 사람 박스 목록 반환
4. `MainForm`에서 `_motion.Detect(frame)` 호출부를 새 서비스로 교체

`Detect(Mat frame)`이 `List<Rect>`를 반환한다는 약속만 지키면 나머지 코드(위험 판정, 경고, DB 저장)는 한 글자도 바꿀 필요가 없다. 처음부터 서비스를 분리해 둔 이유가 바로 이것이다.

---

## 5. 폴더 구조

```text
SmartSafetyVideoAnalyzer
│
├─ Models
│  └─ DetectionLog.cs          ← 위험 기록 1건을 담는 클래스
│
├─ Services
│  ├─ VideoService.cs          ← 동영상 열기 / 프레임 읽기
│  ├─ MotionDetectionService.cs ← 움직임 감지 (나중에 AI로 교체되는 부분)
│  └─ LogService.cs            ← SQLite 저장 / 조회
│
├─ Data
│  └─ safety_logs.db           ← 실행하면 자동 생성됨
│
├─ Videos
│  └─ sample_video.mp4         ← 테스트용 동영상
│
├─ Captures
│  └─ (확장 과제) 위험 상황 캡처 이미지 저장
│
├─ MainForm.cs                 ← 메인 화면
├─ LogForm.cs                  ← 로그 조회 화면
└─ Program.cs
```

역할별로 파일을 나누는 이유: 한 파일에 전부 넣으면 처음엔 편하지만, 기능을 바꿀 때 어디를 고쳐야 할지 찾기 어려워진다. "영상 문제면 VideoService, 감지 문제면 MotionDetectionService"처럼 문제와 파일이 1:1로 대응되면 디버깅이 훨씬 빨라진다.

---

## 6. 필요한 NuGet 패키지

| 패키지 | 사용 이유 |
|---|---|
| OpenCvSharp4 | OpenCV의 영상 처리 기능(프레임 읽기, 차이 계산, 윤곽선 검출, 도형 그리기)을 C#에서 그대로 사용하기 위해 |
| OpenCvSharp4.runtime.win | OpenCV의 실제 엔진은 C++로 만들어져 있다. Windows에서 그 엔진(DLL)을 실행하기 위한 런타임. **이걸 빼먹으면 빌드는 되는데 실행하면 죽는다.** |
| OpenCvSharp4.Extensions | OpenCV의 이미지(`Mat`)를 WinForms가 표시할 수 있는 `Bitmap`으로 변환하는 `BitmapConverter` 제공 |
| Microsoft.Data.Sqlite | 별도 DB 서버 설치 없이 파일 하나로 동작하는 SQLite에 연결하기 위해 |
| Dapper | SQL 실행 결과를 C# 객체(`DetectionLog`)로 자동 변환해 줘서 DB 코드가 짧고 읽기 쉬워진다 |

---

## 7. 핵심 개념 설명

### 프레임(Frame)이란

동영상의 한 장면, 즉 **사진 한 장**이다. 동영상은 사실 사진을 빠르게 연속으로 넘기는 것에 불과하다. 1초에 30장을 넘기면 30fps(frames per second)라고 부른다. 우리 눈은 1초에 사진이 24장 이상 지나가면 자연스러운 움직임으로 느낀다. 그래서 우리 프로그램도 "동영상을 처리한다"가 아니라 **"사진을 1초에 30번 처리한다"** 고 생각하면 된다.

### 움직임 감지의 원리 — 프레임 차이 방식

연속된 두 장의 사진을 겹쳐 놓고 비교한다고 상상해 보자. 배경(벽, 바닥, 기계)은 두 사진에서 똑같으니 차이가 0이다. 하지만 사람이 걸어가고 있다면 사람이 있던 자리와 새로 간 자리의 픽셀 값이 달라진다. 즉,

```text
|현재 프레임 - 이전 프레임| = 움직인 부분만 밝게 남는 이미지
```

여기에 "차이가 25 이상인 픽셀만 남기기(Threshold)"를 적용하면 조명 흔들림 같은 미세한 잡음이 제거되고, 진짜 움직임만 흰 덩어리로 남는다. 그 덩어리의 외곽선(Contour)을 찾아 사각형으로 감싸면 초록 박스가 된다.

### 위험 구역 좌표

이미지의 좌표계는 수학 시간과 다르다. **왼쪽 위가 (0, 0)** 이고, 오른쪽으로 갈수록 x가, **아래로 갈수록 y가 커진다.** `new Rect(200, 150, 200, 200)`은 "왼쪽에서 200픽셀, 위에서 150픽셀 떨어진 지점부터 가로 200, 세로 200짜리 사각형"이라는 뜻이다.

### 사각형 충돌 판정

두 사각형이 겹치는지는 `IntersectsWith` 한 줄로 판정된다. 내부적으로는 "A의 오른쪽 끝이 B의 왼쪽 끝보다 왼쪽에 있으면 절대 안 겹친다" 같은 비교 4개로 이루어진 단순한 논리다. 게임에서 총알이 캐릭터에 맞았는지 판정할 때 쓰는 것과 똑같은 기법이다.

### 로그를 저장하는 의미

산업 안전 시스템에서 "그 순간 화면에 경고를 띄우는 것"만큼 중요한 것이 **"언제, 어디서, 몇 번 위험했는지 기록을 남기는 것"** 이다. 사고가 났을 때 원인을 추적하고, 위험이 자주 발생하는 구역을 찾아 작업 환경을 바꾸는 근거가 된다. 프로그램을 꺼도 기록이 남아야 하므로 변수(메모리)가 아니라 파일(DB)에 저장한다.

### 왜 SQLite인가

MySQL이나 SQL Server는 별도의 DB 서버 프로그램을 설치하고 항상 켜 둬야 한다. SQLite는 **그냥 파일 하나**(`safety_logs.db`)다. 설치할 것도, 켜 둘 것도 없고, 프로그램과 함께 복사해서 다른 PC로 옮길 수도 있다. 실제로 스마트폰 앱 대부분이 내부 저장용으로 SQLite를 쓴다.

---

## 8. 전체 흐름 의사코드

```text
동영상 파일을 연다
타이머를 켠다 (1초에 약 30번 깜빡임)

타이머가 깜빡일 때마다:
    프레임을 한 장 읽는다
    프레임이 없으면 → 영상 끝, 타이머 정지

    현재 프레임을 흑백으로 바꾼다
    이전 프레임과의 차이를 계산한다
    차이가 큰 영역을 찾아 사각형 목록을 만든다 (움직임 박스)

    화면에 위험 구역을 빨간색으로 그린다
    움직임 박스들을 초록색으로 그린다

    만약 움직임 박스가 위험 구역과 겹치면:
        화면에 "DANGER!" 경고를 그린다
        상태 라벨을 "위험 감지"로 바꾼다
        (최근 1초 내 저장 기록이 없다면) SQLite에 로그를 저장한다
    아니면:
        상태 라벨을 "정상"으로 바꾼다

    완성된 프레임을 PictureBox에 출력한다
```

---

## 9. 초급자용 구현 전략

한 번에 전부 만들려고 하면 어디서 틀렸는지 찾을 수 없게 된다. 아래 순서대로 **각 단계가 동작하는 것을 눈으로 확인한 뒤에** 다음으로 넘어간다.

| 단계 | 목표 | 성공 기준 |
|---|---|---|
| 1 | 동영상 재생만 성공시키기 | PictureBox에서 영상이 부드럽게 재생됨 |
| 2 | 위험 구역 빨간 사각형 그리기 | 영상 위에 빨간 사각형이 고정으로 보임 |
| 3 | 움직임 감지 붙이기 | 움직이는 물체에 초록 박스가 따라다님 |
| 4 | 위험 판정 + 경고 표시 | 박스가 겹칠 때 DANGER 문구가 뜸 |
| 5 | DB 저장 + 로그 조회 | 껐다 켜도 기록이 남아 있음 |
| 확장 | ONNX AI로 사람만 감지 | 움직임이 아니라 "사람"을 구분함 |

특히 1단계에서 막히는 학생이 가장 많은데(런타임 DLL 문제, 코덱 문제), 1단계만 통과하면 나머지는 대부분 순조롭게 진행된다. AI 모델을 처음부터 붙이지 않는 이유도 같다. 영상 파이프라인이 확실히 동작하는 상태에서 감지 방식만 교체해야, 문제가 생겼을 때 원인이 AI 쪽인지 영상 쪽인지 바로 구분할 수 있다.

---

## 10. 확장 아이디어

| 확장 | 내용 | 연관 기술 |
|---|---|---|
| ONNX 사람 감지 | 움직임이 아니라 "사람"만 골라서 감지 | Microsoft.ML.OnnxRuntime, YOLOv8 |
| 위험 상황 캡처 | 위험 순간의 프레임을 `Captures` 폴더에 이미지로 저장 (`frame.SaveImage(경로)` 한 줄) | OpenCvSharp |
| 감지 이력 검색 | 날짜 범위, 프레임 범위로 로그 검색 | SQL WHERE 절 |
| 관리자 화면 | 위험 구역 좌표를 마우스로 드래그해서 설정, 설정값 DB 저장 | 마우스 이벤트 |
| Blazor 웹 대시보드 | 감지 로그를 웹 브라우저에서 조회, 통계 차트 표시 | Blazor Server |
| PLC 연동 | 위험 감지 시 PLC에 정지 신호 전송 → 실제 설비 안전 정지 | Modbus RTU/TCP |
| 실시간 카메라 | `VideoCapture(0)`으로 웹캠 연결 | OpenCvSharp |
| IP 카메라 | 스마트폰을 IP 카메라 앱으로 만들어 RTSP 주소로 연결 (`VideoCapture("rtsp://...")`) | RTSP 스트리밍 |

---

## 11. 최종 산출물

수업이 끝나면 학생들은 다음을 갖게 된다.

- **실행 가능한 C# WinForms 프로그램** — 동영상을 열고 분석하는 완성품
- **동영상 분석 기능** — 프레임 단위 처리 + 움직임 감지
- **위험 구역 감지 기능** — 충돌 판정과 실시간 경고
- **SQLite 로그 DB** — `safety_logs.db` 파일과 테이블 설계 경험
- **감지 이력 조회 화면** — DataGridView 기반 로그 뷰어
- **프로젝트 설명서** — 본 문서를 기반으로 각자 작성한 README

포트폴리오 관점에서도 "영상 처리 + DB + 산업 안전"이라는 세 가지 키워드가 한 프로젝트에 들어가므로, 자동화/스마트팩토리 분야 지원 시 활용도가 높다.

---

## 12. 주의사항 (수업 중 자주 발생하는 문제)

### 동영상 파일 경로 문제

한글 폴더명이나 OneDrive 동기화 경로에서 파일이 안 열리는 경우가 있다. 테스트 영상은 `C:\Test\sample_video.mp4`처럼 **영문 경로**에 두는 것을 권장한다.

### NuGet 패키지 설치 문제

학교/회사 네트워크에서 NuGet 서버 접근이 막혀 있으면 설치가 실패한다. 사전에 패키지를 받아 둔 오프라인 폴더를 준비해 두거나, 수업 전에 설치까지 마친 프로젝트 템플릿을 배포하는 방법이 안전하다.

### OpenCvSharp 런타임 오류

실행하자마자 `DllNotFoundException: OpenCvSharpExtern`이 뜨면 100% `OpenCvSharp4.runtime.win` 미설치다. 패키지를 설치하고 **솔루션 정리 후 다시 빌드**한다. 그래도 안 되면 `bin\Debug\net8.0-windows\runtimes` 폴더에 DLL이 복사됐는지 확인한다.

### SQLite DB 파일 생성 위치 문제

DB 파일은 프로젝트 폴더가 아니라 `bin\Debug\net8.0-windows\Data\` 아래에 생긴다. "DB 파일이 안 보여요"라는 질문이 반드시 나오므로 미리 알려 줄 것. 상대 경로 대신 `AppDomain.CurrentDomain.BaseDirectory`를 쓰는 이유도 이것이다 — 실행 위치가 어디든 항상 같은 곳에 DB가 만들어진다.

### 프레임 처리 속도 문제

고해상도(4K) 영상은 프레임 차이 계산이 느려서 화면이 버벅인다. 수업용 영상은 **1280×720 이하**로 준비한다. 그래도 느리면 `MotionDetectionService.Detect` 첫 줄에서 `Cv2.Resize`로 프레임을 절반 크기로 줄여 처리하는 방법을 알려 준다 (단, 이 경우 위험 구역 좌표도 같은 비율로 줄여야 한다).

### 학생 PC 사양 차이 문제

저사양 PC에서는 타이머 간격(33ms) 안에 처리가 끝나지 않아 영상이 느리게 재생될 수 있다. 동작 자체에는 문제가 없으니 "느린 것은 정상이고, 빠르게 하려면 영상 해상도를 줄이면 된다"고 안내한다. 처리 시간 측정(`Stopwatch`)을 추가 과제로 주면 성능 개념까지 자연스럽게 연결된다.

### Point / Size 이름 충돌

`System.Drawing.Point`와 `OpenCvSharp.Point`가 이름이 같아서 `using` 두 개가 공존하면 컴파일 오류가 난다. 본 문서의 코드는 충돌 지점에서 `new OpenCvSharp.Point(...)`, `new System.Drawing.Point(...)`처럼 전체 이름을 써서 해결했다. 학생이 직접 코드를 변형하다 `'Point'은(는) 모호한 참조입니다` 오류를 만나면 이 부분을 떠올리게 한다.
