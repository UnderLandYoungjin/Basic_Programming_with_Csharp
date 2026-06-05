# 도서 대여 관리 시스템 — 상세 주석 전체 코드

C# WinForms + ADO.NET(MSSQL) 도서 대여점 관리 프로그램의 전체 코드에, 한 줄씩 따라갈 수 있도록 상세 주석을 달아 둔 버전이다. 파일 이름에 그대로 붙여 넣으면 동작한다. 주석은 강의 중 설명하는 톤으로 적었으니, 학생 배포용으로 쓸 때 필요 없는 줄은 지워도 된다.

파일 순서: SQL → Program.cs → DB.cs → Models.cs → DAO.cs → MainForm.cs → BookForm.cs → MemberForm.cs → RentalForm.cs → QueryForm.cs → SettingForm.cs

---

## 1. 데이터베이스 스크립트 (SSMS에서 실행)

```sql
-- ===========================================================
-- 도서 대여 DB 생성 스크립트
-- DB → 테이블 5개 → 기본 설정값 → 샘플 데이터 순으로 한 번에 만든다.
-- SSMS에서 통째로 실행한다.
-- ===========================================================

CREATE DATABASE BookRentalDB;   -- 데이터베이스를 새로 만든다
GO                              -- GO: 앞 배치를 끝내고 다음 배치를 시작하라는 SSMS 구분자
USE BookRentalDB;               -- 앞으로의 명령을 이 DB에 대해 실행
GO

-- -----------------------------------------------------------
-- 도서 테이블: 책 한 권의 정보
-- -----------------------------------------------------------
CREATE TABLE Book (
    BookCode    VARCHAR(20)   NOT NULL PRIMARY KEY,   -- 도서 코드(ISBN/바코드). 기본키라 중복 불가
    Category    NVARCHAR(30)  NULL,                   -- 분류(소설, 자기계발 등). 한글이라 NVARCHAR
    Title       NVARCHAR(100) NULL,                   -- 제목
    Author      NVARCHAR(50)  NULL,                   -- 저자
    Translator  NVARCHAR(50)  NULL,                   -- 역자(없을 수 있어 NULL 허용)
    Publisher   NVARCHAR(50)  NULL,                   -- 출판사
    PublishDate DATE          NULL                    -- 출판일. 신간/구간 판정의 기준이 된다
);

-- -----------------------------------------------------------
-- 회원 테이블: 대여 회원 정보
-- -----------------------------------------------------------
CREATE TABLE Member (
    MemberNo INT           NOT NULL PRIMARY KEY,       -- 회원번호. 기본키
    Name     NVARCHAR(30)  NULL,                       -- 이름
    Jumin    VARCHAR(15)   NULL,                       -- 주민등록번호(숫자/하이픈이라 VARCHAR)
    Grade    NVARCHAR(10)  NULL,                       -- 등급(일반/학생)
    Gender   NVARCHAR(5)   NULL,                       -- 성별(남자/여자)
    Phone    VARCHAR(20)   NULL,                       -- 전화번호
    Mobile   VARCHAR(20)   NULL,                       -- 휴대폰
    ZipCode  VARCHAR(10)   NULL,                       -- 우편번호
    Address  NVARCHAR(100) NULL,                       -- 주소(한글)
    CardId   VARCHAR(40)   NULL                        -- RFID 카드번호. 발급 전엔 NULL
);

-- -----------------------------------------------------------
-- 대여 내역 테이블: 누가 어떤 책을 언제 빌렸는지 한 건씩 쌓인다
-- -----------------------------------------------------------
CREATE TABLE Rental (
    RentalId    INT IDENTITY(1,1) PRIMARY KEY,         -- 대여 일련번호. IDENTITY로 자동 1씩 증가
    MemberNo    INT          NOT NULL,                 -- 빌린 회원
    BookCode    VARCHAR(20)  NOT NULL,                 -- 빌린 도서
    RentDate    DATE         NOT NULL,                 -- 대여일
    DueDate     DATE         NOT NULL,                 -- 반납 예정일
    ReturnDate  DATE         NULL,                     -- 실제 반납일(반납 전엔 NULL)
    RentFee     INT          NOT NULL DEFAULT 0,       -- 대여료. ★대여 시점 금액을 복사해 저장★
    OverdueRate INT          NOT NULL DEFAULT 0,       -- 1일당 연체단가. ★대여 시점에 확정★
    OverdueFee  INT          NOT NULL DEFAULT 0,       -- 실제 부과된 연체료. 반납할 때 채운다
    IsReturned  BIT          NOT NULL DEFAULT 0,       -- 반납 여부(0=대여중, 1=반납)
    FOREIGN KEY (MemberNo) REFERENCES Member(MemberNo),-- 외래키: 존재하는 회원만 대여 가능
    FOREIGN KEY (BookCode) REFERENCES Book(BookCode)   -- 외래키: 존재하는 도서만 대여 가능
);
-- 요금(RentFee, OverdueRate)을 대여 행에 복사해 두는 이유:
--   요금표(RentalSetting)는 나중에 바뀔 수 있다. 그때 적용된 금액을 박아 두지 않으면
--   요금을 수정하는 순간 과거 대여 건의 정산 금액이 흔들린다.

-- -----------------------------------------------------------
-- 요금 설정 테이블: 신간/구간 요금. 항상 Id=1 한 행만 유지한다
-- -----------------------------------------------------------
CREATE TABLE RentalSetting (
    Id            INT PRIMARY KEY,    -- 항상 1로 고정
    SwitchPeriod  INT NOT NULL,       -- 전환 기간(일). 출판 후 N일 이내면 신간
    NewRentDays   INT NOT NULL,       -- 신간 대여 기간(일)
    NewRentFee    INT NOT NULL,       -- 신간 대여료
    NewOverdueFee INT NOT NULL,       -- 신간 연체단가(/일)
    OldRentDays   INT NOT NULL,       -- 구간 대여 기간(일)
    OldRentFee    INT NOT NULL,       -- 구간 대여료
    OldOverdueFee INT NOT NULL        -- 구간 연체단가(/일)
);

-- 기본 설정값 한 줄 삽입
-- (Id=1, 전환14일, 신간:7일/700원/연체200, 구간:14일/500원/연체100)
INSERT INTO RentalSetting VALUES (1, 14, 7, 700, 200, 14, 500, 100);

-- 화면 동작 확인용 샘플 데이터
INSERT INTO Book VALUES ('B0001','소설','테스트','테스트','','테스트출판','2008-01-01');
INSERT INTO Book VALUES ('B0002','자기계발','아주 작은 습관의 힘','제임스 클리어','이한이','비즈니스북스','2019-02-26');
INSERT INTO Member VALUES (101,'홍길동','123456-1234567','일반','남자','055-123-4567','010-1234-1234','123-456','대한민국',NULL);
```

---

## 2. Program.cs — 진입점

```csharp
using System;                       // 기본 시스템 타입
using System.Windows.Forms;         // WinForms(Application, Form 등)

namespace BookRentalSystem
{
    // static class: 인스턴스를 만들 수 없는, 진입점 전용 클래스
    static class Program
    {
        // [STAThread]: 이 메인 스레드를 STA(Single-Threaded Apartment) 모델로 시작하라는 표시.
        //   WinForms가 내부에서 쓰는 윈도우 COM 기능(클립보드, 드래그앤드롭,
        //   OpenFileDialog/SaveFileDialog 같은 공통 대화상자)이 STA에서만 동작하기 때문에 필수다.
        //   이걸 빼면 SettingForm의 SaveFileDialog(CSV 저장) 등에서 예외가 난다.
        [STAThread]
        static void Main()
        {
            // 컨트롤에 윈도우 테마(비주얼 스타일)를 입힌다. 버튼 모서리 등이 OS 기본 모양이 됨
            Application.EnableVisualStyles();

            // 텍스트 렌더링 방식을 최신(GDI+ 아님)으로. 폼 생성 전에 한 번 호출하는 게 규칙
            Application.SetCompatibleTextRenderingDefault(false);

            // 메인 폼을 띄우고 메시지 루프 시작. 이 폼이 닫히면 프로그램이 종료된다
            Application.Run(new MainForm());
        }
    }
}
```

---

## 3. DB.cs — ADO.NET 헬퍼 + UI 헬퍼

```csharp
using System;
using System.Data;                  // DataTable 등
using System.Data.SqlClient;        // SqlConnection, SqlCommand, SqlDataAdapter (.NET Framework 내장)
                                    //   .NET 6+ 라면 이 줄을 Microsoft.Data.SqlClient 로 바꾼다
using System.Drawing;               // ContentAlignment (라벨 정렬)
using System.Windows.Forms;         // Control, Label, TextBox, Button, Form

namespace BookRentalSystem
{
    // ===========================================================
    // DBManager: ADO.NET 호출을 세 패턴(Query/Execute/Scalar)으로 감싼 헬퍼.
    //   DAO는 SQL만 넘기고, 실제 연결·실행은 전부 여기서 한다.
    //   DB가 바뀌어도 이 클래스만 손보면 된다.
    // ===========================================================
    public static class DBManager
    {
        // ★ 환경에 맞게 반드시 수정하는 곳 ★ — 가장 자주 막히는 지점
        // LocalDB:   Server=(localdb)\MSSQLLocalDB;Database=BookRentalDB;Integrated Security=True;
        // Express:   Server=.\SQLEXPRESS;Database=BookRentalDB;Integrated Security=True;
        // 계정 로그인: Server=localhost;Database=BookRentalDB;User Id=sa;Password=암호;
        public static string ConnectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=BookRentalDB;Integrated Security=True;";

        // ---------------------------------------------------------
        // Query: SELECT 결과를 DataTable로 받아 온다
        //   sql: 실행할 SELECT 문
        //   ps : @변수 파라미터들(가변 인자). 없으면 안 넘겨도 됨
        // ---------------------------------------------------------
        public static DataTable Query(string sql, params SqlParameter[] ps)
        {
            // using: 블록을 벗어나면 연결/명령을 자동으로 닫고 반환한다(자원 누수 방지)
            using (var con = new SqlConnection(ConnectionString))   // DB 연결 객체
            using (var cmd = new SqlCommand(sql, con))              // 실행할 명령 객체
            {
                if (ps != null) cmd.Parameters.AddRange(ps);        // 파라미터 일괄 등록
                var dt = new DataTable();                           // 결과를 담을 표
                // SqlDataAdapter.Fill: 연결을 알아서 열고 닫으며 결과를 dt에 채운다
                //   그래서 여기엔 con.Open()이 따로 없다
                using (var da = new SqlDataAdapter(cmd)) da.Fill(dt);
                return dt;                                          // 채워진 표를 돌려줌
            }
        }

        // ---------------------------------------------------------
        // Execute: INSERT / UPDATE / DELETE 실행 → 영향받은 행 수 반환
        // ---------------------------------------------------------
        public static int Execute(string sql, params SqlParameter[] ps)
        {
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                con.Open();                                  // 변경 계열은 직접 연결을 연다
                return cmd.ExecuteNonQuery();                // 행을 바꾸는 명령 실행, 변경 행 수 반환
            }
        }

        // ---------------------------------------------------------
        // Scalar: COUNT(*) 처럼 값 하나만 필요할 때
        //   결과의 첫 행, 첫 열 값을 object로 돌려준다(호출 측에서 형변환)
        // ---------------------------------------------------------
        public static object Scalar(string sql, params SqlParameter[] ps)
        {
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                con.Open();
                return cmd.ExecuteScalar();                  // 단일 값 반환
            }
        }
    }

    // ===========================================================
    // UI: 디자이너 없이 코드로 화면을 짜기 위한 보조 함수 모음.
    //   라벨/텍스트박스/버튼을 한 줄로 만들어 부모(p)에 붙인다.
    // ===========================================================
    public static class UI
    {
        // 라벨 생성: p=부모, t=글자, x/y=위치, w=너비(기본 80)
        public static Label Lbl(Control p, string t, int x, int y, int w = 80)
        {
            var l = new Label
            {
                Text = t,
                Left = x,
                Top = y + 4,                              // 옆 텍스트박스와 세로 중앙을 맞추려 +4
                Width = w,
                TextAlign = ContentAlignment.MiddleRight  // 글자를 오른쪽 정렬(입력칸에 붙게)
            };
            p.Controls.Add(l);   // 부모에 부착
            return l;            // 만든 컨트롤을 돌려줌(필요하면 호출 측에서 더 손봄)
        }

        // 텍스트박스 생성: w=너비(기본 150), ro=읽기전용 여부(기본 false)
        public static TextBox Txt(Control p, int x, int y, int w = 150, bool ro = false)
        {
            var t = new TextBox { Left = x, Top = y, Width = w, ReadOnly = ro };
            p.Controls.Add(t);
            return t;
        }

        // 버튼 생성: t=글자, click=클릭 시 실행할 핸들러, w=너비(기본 80)
        public static Button Btn(Control p, string t, int x, int y, EventHandler click, int w = 80)
        {
            var b = new Button { Text = t, Left = x, Top = y, Width = w, Height = 28 };
            b.Click += click;    // 클릭 이벤트에 핸들러 연결
            p.Controls.Add(b);
            return b;
        }

        // 한 줄 입력 대화상자: 카드번호 입력 등에 사용
        //   확인을 누르면 입력값(앞뒤 공백 제거)을, 취소면 null을 반환
        public static string Prompt(string title, string label, string def = "")
        {
            using (var f = new Form
            {
                Width = 340, Height = 150, Text = title,
                FormBorderStyle = FormBorderStyle.FixedDialog,        // 크기 고정 대화상자
                StartPosition = FormStartPosition.CenterParent,       // 부모 가운데에 표시
                MaximizeBox = false, MinimizeBox = false
            })
            {
                var l = new Label { Left = 12, Top = 15, Width = 300, Text = label };
                var t = new TextBox { Left = 12, Top = 40, Width = 300, Text = def };
                var ok = new Button { Text = "확인", Left = 150, Top = 75,
                                      DialogResult = DialogResult.OK }; // 누르면 OK로 닫힘
                f.Controls.AddRange(new Control[] { l, t, ok });
                f.AcceptButton = ok;                                   // Enter = 확인 버튼
                // ShowDialog: 이 창이 닫힐 때까지 대기. OK면 입력값, 아니면 null
                return f.ShowDialog() == DialogResult.OK ? t.Text.Trim() : null;
            }
        }
    }
}
```

---

## 4. Models.cs — 데이터 모델

```csharp
using System;

namespace BookRentalSystem
{
    // DB 한 행을 담아 계층 사이로 옮기는 운반용 클래스들.
    // 로직 없이 데이터만 들고 다닌다(필드를 public으로 단순 노출).

    // Book 테이블 한 행에 대응
    public class Book
    {
        public string BookCode, Category, Title, Author, Translator, Publisher;
        public DateTime PublishDate = DateTime.Today;  // 비어 있을 때 신간/구간 판정이 깨지지 않게 기본값
    }

    // Member 테이블 한 행에 대응
    public class Member
    {
        public int MemberNo;
        public string Name, Jumin, Grade, Gender, Phone, Mobile, ZipCode, Address, CardId;
    }

    // RentalSetting 테이블 한 행(요금표)에 대응
    public class Setting
    {
        public int SwitchPeriod,    // 전환 기간(일)
                   NewRentDays, NewRentFee, NewOverdueFee,   // 신간 3종
                   OldRentDays, OldRentFee, OldOverdueFee;   // 구간 3종
    }
}
```

---

## 5. DAO.cs — DB 접근 계층

```csharp
using System;
using System.Data;
using System.Data.SqlClient;

namespace BookRentalSystem
{
    // ===========================================================
    // BookDAO: 도서 관련 SQL 모음
    // ===========================================================
    public static class BookDAO
    {
        // 전체 도서 목록.
        //   AS 한글별칭 → 이 DataTable을 DataGridView에 넣으면 별칭이 컬럼 헤더가 된다.
        //   CONVERT(...,23) → 날짜를 'yyyy-MM-dd' 문자열로 변환
        public static DataTable GetAll() => DBManager.Query(
            @"SELECT BookCode AS 코드, Category AS 분류, Title AS 제목, Author AS 저자,
                     Translator AS 역자, Publisher AS 출판사,
                     CONVERT(varchar(10),PublishDate,23) AS 출판일
              FROM Book ORDER BY BookCode");

        // 전체 도서 수. Scalar 결과는 object라 (int)로 캐스팅
        public static int Count() => (int)DBManager.Scalar("SELECT COUNT(*) FROM Book");

        // 해당 코드의 도서가 존재하는가(COUNT > 0). 저장 시 INSERT/UPDATE 분기에 사용
        public static bool Exists(string code) =>
            (int)DBManager.Scalar("SELECT COUNT(*) FROM Book WHERE BookCode=@c",
                new SqlParameter("@c", code)) > 0;

        // 코드로 도서 한 건을 Book 객체로 조회. 없으면 null
        public static Book GetByCode(string code)
        {
            var dt = DBManager.Query("SELECT * FROM Book WHERE BookCode=@c",
                new SqlParameter("@c", code));
            if (dt.Rows.Count == 0) return null;     // 결과 없음
            var r = dt.Rows[0];                       // 첫 행
            return new Book
            {
                BookCode = r["BookCode"].ToString(),
                Category = r["Category"].ToString(),
                Title = r["Title"].ToString(),
                Author = r["Author"].ToString(),
                Translator = r["Translator"].ToString(),
                Publisher = r["Publisher"].ToString(),
                // DB가 NULL이면 오늘 날짜로, 아니면 날짜로 변환
                PublishDate = r["PublishDate"] == DBNull.Value
                    ? DateTime.Today : Convert.ToDateTime(r["PublishDate"])
            };
        }

        // 신규 등록
        public static void Insert(Book b) => DBManager.Execute(
            @"INSERT INTO Book(BookCode,Category,Title,Author,Translator,Publisher,PublishDate)
              VALUES(@code,@c,@t,@a,@tr,@p,@d)", P(b));

        // 수정(코드 기준)
        public static void Update(Book b) => DBManager.Execute(
            @"UPDATE Book SET Category=@c,Title=@t,Author=@a,Translator=@tr,Publisher=@p,PublishDate=@d
              WHERE BookCode=@code", P(b));

        // 삭제(대여 이력이 있으면 FK 제약에 막힌다 → 호출 측에서 try/catch)
        public static void Delete(string code) => DBManager.Execute(
            "DELETE FROM Book WHERE BookCode=@c", new SqlParameter("@c", code));

        // 분류 목록(중복 제거). 조회 화면의 콤보박스 채우기에 사용
        public static DataTable Categories() => DBManager.Query(
            "SELECT DISTINCT Category FROM Book WHERE Category IS NOT NULL ORDER BY Category");

        // INSERT/UPDATE가 공유하는 파라미터 배열.
        //   (object)값 ?? DBNull.Value : C#의 null을 DB의 NULL로 변환하는 관용 표현
        static SqlParameter[] P(Book b) => new[]
        {
            new SqlParameter("@code", b.BookCode),
            new SqlParameter("@c", (object)b.Category ?? DBNull.Value),
            new SqlParameter("@t", (object)b.Title ?? DBNull.Value),
            new SqlParameter("@a", (object)b.Author ?? DBNull.Value),
            new SqlParameter("@tr", (object)b.Translator ?? DBNull.Value),
            new SqlParameter("@p", (object)b.Publisher ?? DBNull.Value),
            new SqlParameter("@d", b.PublishDate)
        };
    }

    // ===========================================================
    // MemberDAO: 회원 관련 SQL + 카드 발급
    // ===========================================================
    public static class MemberDAO
    {
        // 전체 회원 목록
        public static DataTable GetAll() => DBManager.Query(
            @"SELECT MemberNo AS 코드, Name AS 성명, Jumin AS 주민등록번,
                     Grade AS 등급, Gender AS 성별, Phone AS 연락처, Mobile AS 휴대폰
              FROM Member ORDER BY MemberNo");

        public static int Count() => (int)DBManager.Scalar("SELECT COUNT(*) FROM Member");

        public static bool Exists(int no) =>
            (int)DBManager.Scalar("SELECT COUNT(*) FROM Member WHERE MemberNo=@n",
                new SqlParameter("@n", no)) > 0;

        // 회원번호로 단건 조회 → Member (없으면 null). Map으로 변환
        public static Member GetByNo(int no) => Map(
            DBManager.Query("SELECT * FROM Member WHERE MemberNo=@n", new SqlParameter("@n", no)));

        // 카드번호로 단건 조회 → Member (RFID 카드 읽기에 사용)
        public static Member GetByCard(string card) => Map(
            DBManager.Query("SELECT * FROM Member WHERE CardId=@c", new SqlParameter("@c", card)));

        // 이름으로 조회 → DataTable.
        //   동명이인이 있을 수 있어 단건(Member)이 아니라 표(DataTable)로 돌려준다
        public static DataTable GetByName(string name) => DBManager.Query(
            @"SELECT MemberNo AS 코드, Name AS 성명, Jumin AS 주민번호,
                     Grade AS 등급, Phone AS 연락처, Mobile AS 휴대폰
              FROM Member WHERE Name=@nm ORDER BY MemberNo",
            new SqlParameter("@nm", name));

        public static void Insert(Member m) => DBManager.Execute(
            @"INSERT INTO Member(MemberNo,Name,Jumin,Grade,Gender,Phone,Mobile,ZipCode,Address,CardId)
              VALUES(@no,@nm,@j,@g,@s,@p,@m,@z,@a,@cd)", P(m));

        public static void Update(Member m) => DBManager.Execute(
            @"UPDATE Member SET Name=@nm,Jumin=@j,Grade=@g,Gender=@s,Phone=@p,
                     Mobile=@m,ZipCode=@z,Address=@a WHERE MemberNo=@no", P(m));

        public static void Delete(int no) => DBManager.Execute(
            "DELETE FROM Member WHERE MemberNo=@n", new SqlParameter("@n", no));

        // RFID 카드 발급: 실제 하드웨어 대신 카드번호를 만들어 저장하고 그 문자열을 반환
        public static string IssueCard(int no)
        {
            // 'RFID-회원번호-네자리난수' 형태로 카드번호 생성
            string card = "RFID-" + no + "-" + new Random().Next(1000, 9999);
            DBManager.Execute("UPDATE Member SET CardId=@c WHERE MemberNo=@n",
                new SqlParameter("@c", card), new SqlParameter("@n", no));
            return card;
        }

        // 등급 목록(중복 제거). 조회 화면의 콤보박스에 사용
        public static DataTable Grades() => DBManager.Query(
            "SELECT DISTINCT Grade FROM Member WHERE Grade IS NOT NULL ORDER BY Grade");

        // DataTable 첫 행을 Member 객체로 변환(공통 헬퍼). 행이 없으면 null
        static Member Map(DataTable dt)
        {
            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return new Member
            {
                MemberNo = Convert.ToInt32(r["MemberNo"]),
                Name = r["Name"].ToString(), Jumin = r["Jumin"].ToString(),
                Grade = r["Grade"].ToString(), Gender = r["Gender"].ToString(),
                Phone = r["Phone"].ToString(), Mobile = r["Mobile"].ToString(),
                ZipCode = r["ZipCode"].ToString(), Address = r["Address"].ToString(),
                CardId = r["CardId"].ToString()
            };
        }

        // INSERT/UPDATE 공유 파라미터
        static SqlParameter[] P(Member m) => new[]
        {
            new SqlParameter("@no", m.MemberNo),
            new SqlParameter("@nm", (object)m.Name ?? DBNull.Value),
            new SqlParameter("@j", (object)m.Jumin ?? DBNull.Value),
            new SqlParameter("@g", (object)m.Grade ?? DBNull.Value),
            new SqlParameter("@s", (object)m.Gender ?? DBNull.Value),
            new SqlParameter("@p", (object)m.Phone ?? DBNull.Value),
            new SqlParameter("@m", (object)m.Mobile ?? DBNull.Value),
            new SqlParameter("@z", (object)m.ZipCode ?? DBNull.Value),
            new SqlParameter("@a", (object)m.Address ?? DBNull.Value),
            new SqlParameter("@cd", (object)m.CardId ?? DBNull.Value)
        };
    }

    // ===========================================================
    // RentalDAO: 대여/반납 흐름의 핵심 SQL
    // ===========================================================
    public static class RentalDAO
    {
        // 그 도서가 지금 누군가에게 나가 있는가(반납 안 됨) → 중복 대여 방지
        public static bool IsActive(string bookCode) =>
            (int)DBManager.Scalar(
                "SELECT COUNT(*) FROM Rental WHERE BookCode=@b AND IsReturned=0",
                new SqlParameter("@b", bookCode)) > 0;

        // 특정 회원이 현재 대여중인 목록(도서 제목과 조인). 화면 표시·합계 계산에 사용
        public static DataTable GetActiveRaw(int memberNo) => DBManager.Query(
            @"SELECT r.RentalId, r.BookCode, b.Title, r.RentDate, r.DueDate,
                     r.RentFee, r.OverdueRate
              FROM Rental r JOIN Book b ON r.BookCode=b.BookCode
              WHERE r.MemberNo=@no AND r.IsReturned=0
              ORDER BY r.RentDate",
            new SqlParameter("@no", memberNo));

        // 대여 등록. ★대여료(rentFee)·연체단가(overdueRate)를 이 시점 값으로 함께 저장★
        public static void Rent(int memberNo, string bookCode, DateTime rent,
                                DateTime due, int rentFee, int overdueRate) =>
            DBManager.Execute(
                @"INSERT INTO Rental(MemberNo,BookCode,RentDate,DueDate,RentFee,OverdueRate)
                  VALUES(@c,@b,@rd,@dd,@rf,@or)",
                new SqlParameter("@c", memberNo), new SqlParameter("@b", bookCode),
                new SqlParameter("@rd", rent), new SqlParameter("@dd", due),
                new SqlParameter("@rf", rentFee), new SqlParameter("@or", overdueRate));

        // 반납 처리: 반납 표시 + 연체료 확정
        //   DATEDIFF(day, DueDate, @d): 반납예정일~반납일의 날짜 차이(늦은 일수)
        //   양수일 때만 (일수 × 연체단가)를 OverdueFee에 채우고, 아니면 0
        public static void Return(int rentalId, DateTime returnDate) =>
            DBManager.Execute(
                @"UPDATE Rental
                  SET IsReturned=1, ReturnDate=@d,
                      OverdueFee = CASE WHEN DATEDIFF(day,DueDate,@d) > 0
                                        THEN DATEDIFF(day,DueDate,@d)*OverdueRate ELSE 0 END
                  WHERE RentalId=@id",
                new SqlParameter("@d", returnDate), new SqlParameter("@id", rentalId));
    }

    // ===========================================================
    // SettingDAO: 요금표(Id=1 한 행)를 읽고 쓴다
    // ===========================================================
    public static class SettingDAO
    {
        // 요금표 한 행을 Setting 객체로 읽어 온다
        public static Setting Get()
        {
            var r = DBManager.Query("SELECT * FROM RentalSetting WHERE Id=1").Rows[0];
            return new Setting
            {
                SwitchPeriod = (int)r["SwitchPeriod"],
                NewRentDays = (int)r["NewRentDays"], NewRentFee = (int)r["NewRentFee"],
                NewOverdueFee = (int)r["NewOverdueFee"],
                OldRentDays = (int)r["OldRentDays"], OldRentFee = (int)r["OldRentFee"],
                OldOverdueFee = (int)r["OldOverdueFee"]
            };
        }

        // 요금표 저장(항상 Id=1 한 행을 UPDATE)
        public static void Save(Setting s) => DBManager.Execute(
            @"UPDATE RentalSetting SET SwitchPeriod=@sp,NewRentDays=@nd,NewRentFee=@nf,
                     NewOverdueFee=@no,OldRentDays=@od,OldRentFee=@of,OldOverdueFee=@oo
              WHERE Id=1",
            new SqlParameter("@sp", s.SwitchPeriod),
            new SqlParameter("@nd", s.NewRentDays), new SqlParameter("@nf", s.NewRentFee),
            new SqlParameter("@no", s.NewOverdueFee),
            new SqlParameter("@od", s.OldRentDays), new SqlParameter("@of", s.OldRentFee),
            new SqlParameter("@oo", s.OldOverdueFee));
    }
}
```

---

## 6. MainForm.cs — MDI 메인

```csharp
using System;
using System.Windows.Forms;

namespace BookRentalSystem
{
    // 프로그램의 바탕 창. 자기 안에 자식 창을 품고(MDI) 메뉴로 각 화면을 연다
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "도서 관리 프로그램";              // 제목 표시줄
            IsMdiContainer = true;                  // ★자식 창을 품는 컨테이너로 동작★
            WindowState = FormWindowState.Maximized; // 최대화로 시작

            var menu = new MenuStrip();             // 상단 메뉴 막대

            // 파일 메뉴
            var mFile = new ToolStripMenuItem("파일");
            //   Add(텍스트, 이미지(null), 클릭핸들러)
            mFile.DropDownItems.Add("종료", null, (s, e) => Close());

            // 도서 대여/반납 메뉴
            var mRent = new ToolStripMenuItem("도서 대여/반납");
            mRent.DropDownItems.Add("대여 관리", null, (s, e) => Open(new RentalForm()));

            // 도서 관리 메뉴
            var mBook = new ToolStripMenuItem("도서 관리");
            mBook.DropDownItems.Add("도서 정보", null, (s, e) => Open(new BookForm()));

            // 회원 관리 메뉴
            var mMember = new ToolStripMenuItem("회원 관리");
            mMember.DropDownItems.Add("회원 정보", null, (s, e) => Open(new MemberForm()));

            // 정보 조회 메뉴
            var mQuery = new ToolStripMenuItem("정보 조회");
            mQuery.DropDownItems.Add("정보 조회", null, (s, e) => Open(new QueryForm()));

            // 환경설정 메뉴
            var mEnv = new ToolStripMenuItem("환경설정");
            mEnv.DropDownItems.Add("환경 설정", null, (s, e) => Open(new SettingForm()));

            // 메뉴들을 막대에 등록
            menu.Items.AddRange(new ToolStripItem[] { mFile, mRent, mBook, mMember, mQuery, mEnv });
            MainMenuStrip = menu;   // 이 폼의 주 메뉴로 지정
            Controls.Add(menu);     // 폼에 부착
        }

        // 같은 종류 창이 이미 떠 있으면 새로 열지 않고 활성화만 한다(중복 방지)
        void Open(Form f)
        {
            foreach (var c in MdiChildren)                    // 현재 떠 있는 자식 창들을 훑어
                if (c.GetType() == f.GetType())               // 같은 타입이 있으면
                { c.Activate(); f.Dispose(); return; }        // 기존 걸 활성화하고 새 폼은 버림
            f.MdiParent = this;                               // 이 폼을 부모로 지정(안쪽에 뜨게)
            f.Show();                                          // 표시
        }
    }
}
```

---

## 7. BookForm.cs — 도서 정보

```csharp
using System;
using System.Windows.Forms;

namespace BookRentalSystem
{
    // 도서 등록·수정·삭제 + 목록 화면
    public class BookForm : Form
    {
        // 입력 컨트롤들(생성자에서 만들고 메서드에서 값 읽고 쓴다)
        TextBox tCode, tCategory, tTitle, tAuthor, tTranslator, tPublisher;
        DateTimePicker dpPublish;   // 출판일 선택기
        Label lblCount;             // 전체 권수 표시
        DataGridView grid;          // 목록

        public BookForm()
        {
            Text = "도서 정보"; Width = 720; Height = 600;

            // --- 입력 폼 배치(라벨 + 입력칸을 좌표로 직접 깔기) ---
            UI.Lbl(this, "도서 코드", 20, 20);  tCode = UI.Txt(this, 110, 20);
            UI.Lbl(this, "분류", 290, 20);      tCategory = UI.Txt(this, 360, 20);
            UI.Lbl(this, "제목", 20, 55);       tTitle = UI.Txt(this, 110, 55, 400);
            UI.Lbl(this, "저자", 20, 90);       tAuthor = UI.Txt(this, 110, 90);
            UI.Lbl(this, "역자", 290, 90);      tTranslator = UI.Txt(this, 360, 90);
            UI.Lbl(this, "출판사", 20, 125);    tPublisher = UI.Txt(this, 110, 125);
            UI.Lbl(this, "출판일", 290, 125);
            dpPublish = new DateTimePicker { Left = 360, Top = 125, Width = 150,
                                             Format = DateTimePickerFormat.Short }; // yyyy-MM-dd 형태
            Controls.Add(dpPublish);

            // --- 버튼들(클릭 시 각 메서드 호출) ---
            UI.Btn(this, "추가", 20, 165, (s, e) => NewMode());  // 입력칸 비우기
            UI.Btn(this, "저장", 110, 165, (s, e) => Save());    // 등록/수정
            UI.Btn(this, "삭제", 200, 165, (s, e) => Delete());
            UI.Btn(this, "취소", 290, 165, (s, e) => Load());    // 목록 새로고침
            UI.Btn(this, "나가기", 540, 165, (s, e) => Close());

            // --- 권수 표시 라벨 ---
            UI.Lbl(this, "전체 도서 수 :", 20, 205, 110);
            lblCount = new Label { Left = 135, Top = 209, Width = 80, Text = "0권" };
            Controls.Add(lblCount);

            // --- 목록 그리드 ---
            grid = new DataGridView
            {
                Left = 20, Top = 235, Width = 660, Height = 320,
                ReadOnly = true,                                          // 셀 직접 편집 금지
                AllowUserToAddRows = false,                               // 빈 추가행 숨김
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,// 폭 자동 채움
                SelectionMode = DataGridViewSelectionMode.FullRowSelect    // 행 단위 선택
            };
            // 셀 클릭 → 그 행을 입력칸에 채우기(RowIndex >= 0 은 헤더 클릭 제외)
            grid.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadRow(e.RowIndex); };
            Controls.Add(grid);

            Load();   // 창이 뜰 때 목록 로드
        }

        // 목록과 권수 새로고침
        void Load()
        {
            grid.DataSource = BookDAO.GetAll();      // DataTable을 그대로 그리드에
            lblCount.Text = BookDAO.Count() + "권";
        }

        // 입력칸 비우기(신규 입력 준비)
        void NewMode()
        {
            tCode.Text = tCategory.Text = tTitle.Text = tAuthor.Text =
                tTranslator.Text = tPublisher.Text = "";
            dpPublish.Value = DateTime.Today;
            tCode.Focus();   // 커서를 코드칸에
        }

        // 그리드 한 줄 클릭 → 그 도서를 다시 조회해 입력칸에 채움
        void LoadRow(int row)
        {
            string code = grid.Rows[row].Cells["코드"].Value.ToString(); // "코드"는 DAO의 AS 별칭
            var b = BookDAO.GetByCode(code);
            if (b == null) return;
            tCode.Text = b.BookCode; tCategory.Text = b.Category; tTitle.Text = b.Title;
            tAuthor.Text = b.Author; tTranslator.Text = b.Translator; tPublisher.Text = b.Publisher;
            dpPublish.Value = b.PublishDate;
        }

        // 저장: 코드가 이미 있으면 UPDATE, 없으면 INSERT(버튼 하나로 처리)
        void Save()
        {
            if (tCode.Text.Trim() == "")                       // 코드는 필수
            { MessageBox.Show("도서 코드를 입력하세요."); return; }

            // 입력값을 Book 객체로 묶기
            var b = new Book
            {
                BookCode = tCode.Text.Trim(), Category = tCategory.Text.Trim(),
                Title = tTitle.Text.Trim(), Author = tAuthor.Text.Trim(),
                Translator = tTranslator.Text.Trim(), Publisher = tPublisher.Text.Trim(),
                PublishDate = dpPublish.Value.Date
            };
            try
            {
                if (BookDAO.Exists(b.BookCode)) BookDAO.Update(b); // 있으면 수정
                else BookDAO.Insert(b);                            // 없으면 등록
                Load();
                MessageBox.Show("저장되었습니다.");
            }
            catch (Exception ex) { MessageBox.Show("오류: " + ex.Message); }
        }

        // 삭제(대여 이력이 있으면 FK 제약에 막혀 예외 → 안내)
        void Delete()
        {
            if (tCode.Text.Trim() == "") return;
            if (MessageBox.Show("삭제하시겠습니까?", "확인", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try { BookDAO.Delete(tCode.Text.Trim()); Load(); NewMode(); }
            catch (Exception ex) { MessageBox.Show("대여 이력이 있으면 삭제 불가.\n" + ex.Message); }
        }
    }
}
```

---

## 8. MemberForm.cs — 회원 정보

```csharp
using System;
using System.Windows.Forms;

namespace BookRentalSystem
{
    // 회원 등록·수정·삭제 + 목록 + RFID 카드 발급
    public class MemberForm : Form
    {
        TextBox tNo, tName, tJumin, tPhone, tMobile, tZip, tAddr;
        ComboBox cbGrade, cbGender;   // 등급/성별은 정해진 값만 받게 콤보박스로
        Label lblCount;
        DataGridView grid;

        public MemberForm()
        {
            Text = "회원 정보"; Width = 720; Height = 620;

            UI.Lbl(this, "회원번호", 20, 20);   tNo = UI.Txt(this, 110, 20);
            UI.Lbl(this, "주민등록번호", 290, 20); tJumin = UI.Txt(this, 380, 20);
            UI.Lbl(this, "회원명", 20, 55);     tName = UI.Txt(this, 110, 55);

            // 회원 등급 콤보박스(DropDownList = 직접 입력 불가, 목록에서만 선택)
            UI.Lbl(this, "회원 등급", 20, 90);
            cbGrade = new ComboBox { Left = 110, Top = 90, Width = 150,
                                     DropDownStyle = ComboBoxStyle.DropDownList };
            cbGrade.Items.AddRange(new[] { "일반", "학생" });
            cbGrade.SelectedIndex = 0; Controls.Add(cbGrade);   // 첫 항목 기본 선택

            // 성별 콤보박스
            UI.Lbl(this, "성별", 290, 90);
            cbGender = new ComboBox { Left = 380, Top = 90, Width = 150,
                                      DropDownStyle = ComboBoxStyle.DropDownList };
            cbGender.Items.AddRange(new[] { "남자", "여자" });
            cbGender.SelectedIndex = 0; Controls.Add(cbGender);

            UI.Lbl(this, "전화번호", 20, 125);  tPhone = UI.Txt(this, 110, 125);
            UI.Lbl(this, "휴대폰", 290, 125);   tMobile = UI.Txt(this, 380, 125);
            UI.Lbl(this, "우편번호", 20, 160);  tZip = UI.Txt(this, 110, 160);
            UI.Lbl(this, "주소", 20, 195);      tAddr = UI.Txt(this, 110, 195, 420);

            UI.Btn(this, "추가", 20, 230, (s, e) => NewMode());
            UI.Btn(this, "저장", 110, 230, (s, e) => Save());
            UI.Btn(this, "삭제", 200, 230, (s, e) => Delete());
            UI.Btn(this, "취소", 290, 230, (s, e) => Load());
            UI.Btn(this, "카드 관리", 400, 230, (s, e) => IssueCard(), 90); // RFID 발급
            UI.Btn(this, "나가기", 540, 230, (s, e) => Close());

            UI.Lbl(this, "현재 회원 수 :", 20, 270, 100);
            lblCount = new Label { Left = 125, Top = 274, Width = 80, Text = "0명" };
            Controls.Add(lblCount);

            grid = new DataGridView
            {
                Left = 20, Top = 300, Width = 660, Height = 270,
                ReadOnly = true, AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            grid.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadRow(e.RowIndex); };
            Controls.Add(grid);

            Load();
        }

        void Load()
        {
            grid.DataSource = MemberDAO.GetAll();
            lblCount.Text = MemberDAO.Count() + "명";
        }

        void NewMode()
        {
            tNo.Text = tName.Text = tJumin.Text = tPhone.Text =
                tMobile.Text = tZip.Text = tAddr.Text = "";
            cbGrade.SelectedIndex = 0; cbGender.SelectedIndex = 0; tNo.Focus();
        }

        // 그리드 클릭 → 그 회원을 조회해 입력칸 채우기
        void LoadRow(int row)
        {
            int no = Convert.ToInt32(grid.Rows[row].Cells["코드"].Value);
            var m = MemberDAO.GetByNo(no);
            if (m == null) return;
            tNo.Text = m.MemberNo.ToString(); tName.Text = m.Name; tJumin.Text = m.Jumin;
            cbGrade.Text = m.Grade; cbGender.Text = m.Gender;
            tPhone.Text = m.Phone; tMobile.Text = m.Mobile; tZip.Text = m.ZipCode; tAddr.Text = m.Address;
        }

        // 입력값을 Member 객체로 읽어 온다(검증 겸용). 회원번호가 숫자가 아니면 null
        Member Read()
        {
            int no;
            if (!int.TryParse(tNo.Text.Trim(), out no))         // 숫자 변환 시도
            { MessageBox.Show("회원번호는 숫자입니다."); return null; }
            return new Member
            {
                MemberNo = no, Name = tName.Text.Trim(), Jumin = tJumin.Text.Trim(),
                Grade = cbGrade.Text, Gender = cbGender.Text, Phone = tPhone.Text.Trim(),
                Mobile = tMobile.Text.Trim(), ZipCode = tZip.Text.Trim(), Address = tAddr.Text.Trim()
            };
        }

        void Save()
        {
            var m = Read(); if (m == null) return;   // 검증 실패면 중단
            try
            {
                if (MemberDAO.Exists(m.MemberNo)) MemberDAO.Update(m);
                else MemberDAO.Insert(m);
                Load(); MessageBox.Show("저장되었습니다.");
            }
            catch (Exception ex) { MessageBox.Show("오류: " + ex.Message); }
        }

        void Delete()
        {
            int no;
            if (!int.TryParse(tNo.Text.Trim(), out no)) return;
            if (MessageBox.Show("삭제하시겠습니까?", "확인", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try { MemberDAO.Delete(no); Load(); NewMode(); }
            catch (Exception ex) { MessageBox.Show("대여 이력이 있으면 삭제 불가.\n" + ex.Message); }
        }

        // RFID 카드 발급: 저장된 회원인지 확인 후 카드번호 생성·저장
        void IssueCard()
        {
            int no;
            if (!int.TryParse(tNo.Text.Trim(), out no) || !MemberDAO.Exists(no))
            { MessageBox.Show("저장된 회원을 먼저 선택하세요."); return; }
            string card = MemberDAO.IssueCard(no);   // 발급된 카드번호
            MessageBox.Show("RFID 카드 발급 완료\n카드번호: " + card);
        }
    }
}
```

---

## 9. RentalForm.cs — 대여 관리

```csharp
using System;
using System.Data;
using System.Windows.Forms;

namespace BookRentalSystem
{
    // 가장 큰 화면. 회원 검색(코드/이름/카드) → 도서 대여 → 반납까지 처리한다
    public class RentalForm : Form
    {
        TabControl tab;                                                    // 회원 검색 탭
        TextBox tInName, tInCode, tInPhone, tInMobile;                     // 회원 입력 탭의 검색칸
        DataGridView gridSelect;                                           // 회원 선택 탭(동명이인 목록)
        TextBox iCode, iName, iJumin, iGrade, iPhone, iMobile, iZip, iAddr;// 선택된 회원 정보(읽기전용)
        TextBox tBCode, tBTitle, tRentFee, tOverdue, tDue;                 // 도서 입력/결과 표시
        DateTimePicker dpRent;                                             // 대여일
        Label lblTotCnt, lblTotFee, lblTotOver;                           // 합계(권수/대여료/연체료)
        DataGridView grid;                                                 // 현재 회원의 대여중 목록
        int curMember = -1;   // 현재 선택된 회원번호. -1이면 아무도 안 고른 상태

        public RentalForm()
        {
            Text = "대여 관리"; Width = 980; Height = 700;

            // ===== 회원 검색 탭(왼쪽 위) =====
            tab = new TabControl { Left = 15, Top = 15, Width = 430, Height = 220 };
            var p1 = new TabPage("회원 입력");   // 검색 입력 탭
            var p2 = new TabPage("회원 선택");   // 동명이인 선택 탭
            tab.TabPages.Add(p1); tab.TabPages.Add(p2); Controls.Add(tab);

            // 회원 입력 탭: 이름/코드/전화/휴대폰으로 검색
            UI.Lbl(p1, "회원명", 10, 15);   tInName = UI.Txt(p1, 90, 15);
            UI.Lbl(p1, "회원코드", 10, 50); tInCode = UI.Txt(p1, 90, 50);
            UI.Lbl(p1, "전화번호", 10, 85); tInPhone = UI.Txt(p1, 90, 85);
            UI.Lbl(p1, "휴대폰", 10, 120);  tInMobile = UI.Txt(p1, 90, 120);
            UI.Btn(p1, "카드 읽기", 30, 155, (s, e) => ReadCard(), 90);  // RFID 카드로 찾기
            UI.Btn(p1, "찾기", 140, 155, (s, e) => Find());             // 코드/이름으로 찾기

            // 회원 선택 탭: 동명이인이 여럿일 때 표에서 더블클릭으로 선택
            gridSelect = new DataGridView
            {
                Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            gridSelect.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                int no = Convert.ToInt32(gridSelect.Rows[e.RowIndex].Cells["코드"].Value);
                ShowMember(MemberDAO.GetByNo(no));   // 더블클릭한 회원을 선택
            };
            p2.Controls.Add(gridSelect);

            // ===== 회원 정보 표시(오른쪽 위, 전부 읽기전용) =====
            int bx = 470;   // 이 블록 시작 x좌표
            UI.Lbl(this, "회원코드", bx, 20);   iCode = UI.Txt(this, bx + 80, 20, 140, true);
            UI.Lbl(this, "회원명", bx + 240, 20); iName = UI.Txt(this, bx + 320, 20, 140, true);
            UI.Lbl(this, "주민번호", bx, 55);    iJumin = UI.Txt(this, bx + 80, 55, 140, true);
            UI.Lbl(this, "회원등급", bx + 240, 55); iGrade = UI.Txt(this, bx + 320, 55, 140, true);
            UI.Lbl(this, "전화번호", bx, 90);    iPhone = UI.Txt(this, bx + 80, 90, 140, true);
            UI.Lbl(this, "휴대폰", bx + 240, 90); iMobile = UI.Txt(this, bx + 320, 90, 140, true);
            UI.Lbl(this, "우편번호", bx, 125);   iZip = UI.Txt(this, bx + 80, 125, 140, true);
            UI.Lbl(this, "주소", bx, 160);       iAddr = UI.Txt(this, bx + 80, 160, 380, true);

            // ===== 대여 도서 입력(가운데) =====
            UI.Lbl(this, "도서 코드", 15, 260, 90); tBCode = UI.Txt(this, 110, 260, 120);
            UI.Lbl(this, "도서 제목", 250, 260, 90); tBTitle = UI.Txt(this, 345, 260, 300, true);
            UI.Btn(this, "등록", 670, 258, (s, e) => RegisterRental());

            // 대여 결과 표시(읽기전용): 대여료/연체단가/대여일/반납예정일
            UI.Lbl(this, "대여료", 15, 300, 60); tRentFee = UI.Txt(this, 80, 300, 90, true);
            UI.Lbl(this, "연체료", 190, 300, 60); tOverdue = UI.Txt(this, 255, 300, 90, true);
            UI.Lbl(this, "대여일", 360, 300, 50);
            dpRent = new DateTimePicker { Left = 415, Top = 300, Width = 130,
                                          Format = DateTimePickerFormat.Short };
            Controls.Add(dpRent);
            UI.Lbl(this, "반납 예정일", 560, 300, 80); tDue = UI.Txt(this, 645, 300, 130, true);

            // ===== 합계 + 반납 버튼 =====
            UI.Lbl(this, "총 대여 권수", 15, 340, 90);
            lblTotCnt = new Label { Left = 110, Top = 344, Width = 60, Text = "0권" }; Controls.Add(lblTotCnt);
            UI.Lbl(this, "총 대여료", 200, 340, 70);
            lblTotFee = new Label { Left = 275, Top = 344, Width = 90, Text = "0원" }; Controls.Add(lblTotFee);
            UI.Lbl(this, "총 연체료", 400, 340, 70);
            lblTotOver = new Label { Left = 475, Top = 344, Width = 90, Text = "0원" }; Controls.Add(lblTotOver);
            UI.Btn(this, "도서 반납", 670, 338, (s, e) => ReturnBook(), 100);

            // ===== 대여중 목록 그리드(아래) =====
            grid = new DataGridView
            {
                Left = 15, Top = 380, Width = 940, Height = 270,
                ReadOnly = true, AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            Controls.Add(grid);

            // 바코드 스캐너는 코드를 찍은 뒤 Enter를 보낸다 → Enter면 바로 대여 등록
            tBCode.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) RegisterRental(); };
        }

        // RFID 카드 읽기 → 카드번호로 회원 조회
        void ReadCard()
        {
            string card = UI.Prompt("카드 읽기", "RFID 카드번호를 입력(스캔)하세요.");
            if (string.IsNullOrEmpty(card)) return;   // 취소/빈값이면 중단
            var m = MemberDAO.GetByCard(card);
            if (m == null) MessageBox.Show("해당 카드 회원이 없습니다.");
            else ShowMember(m);
        }

        // 회원 찾기: 코드가 있으면 코드로, 없으면 이름으로. 동명이인이면 선택 탭으로
        void Find()
        {
            // (1) 코드가 입력돼 있으면 코드 우선
            if (tInCode.Text.Trim() != "")
            {
                int no;
                if (int.TryParse(tInCode.Text.Trim(), out no))
                {
                    var m = MemberDAO.GetByNo(no);
                    if (m == null) MessageBox.Show("회원이 없습니다."); else ShowMember(m);
                }
                return;
            }
            // (2) 코드가 없으면 이름으로
            if (tInName.Text.Trim() == "") { MessageBox.Show("회원명 또는 코드를 입력하세요."); return; }

            var dt = MemberDAO.GetByName(tInName.Text.Trim());
            if (dt.Rows.Count == 0) { MessageBox.Show("회원이 없습니다."); return; }
            if (dt.Rows.Count == 1)   // 한 명이면 바로 선택
            {
                ShowMember(MemberDAO.GetByNo(Convert.ToInt32(dt.Rows[0]["코드"])));
            }
            else                       // 여러 명(동명이인)이면 선택 탭으로 안내
            {
                gridSelect.DataSource = dt;
                tab.SelectedIndex = 1;   // '회원 선택' 탭으로 전환
                MessageBox.Show("동명이인이 있습니다. '회원 선택' 탭에서 더블클릭하세요.");
            }
        }

        // 회원을 선택 상태로 만들고 정보 칸/대여 목록을 채운다
        void ShowMember(Member m)
        {
            if (m == null) return;
            curMember = m.MemberNo;   // 현재 작업 대상 회원 기억
            iCode.Text = m.MemberNo.ToString(); iName.Text = m.Name; iJumin.Text = m.Jumin;
            iGrade.Text = m.Grade; iPhone.Text = m.Phone; iMobile.Text = m.Mobile;
            iZip.Text = m.ZipCode; iAddr.Text = m.Address;
            LoadRentals();   // 이 회원의 대여중 목록 표시
        }

        // 도서 대여 등록: 신간/구간을 자동 판정해 요금을 정하고 DB에 저장
        void RegisterRental()
        {
            if (curMember < 0) { MessageBox.Show("회원을 먼저 선택하세요."); return; }
            string code = tBCode.Text.Trim();
            if (code == "") return;

            var b = BookDAO.GetByCode(code);
            if (b == null) { MessageBox.Show("도서가 없습니다."); return; }
            if (RentalDAO.IsActive(code)) { MessageBox.Show("이미 대여 중인 도서입니다."); return; }

            var s = SettingDAO.Get();   // 현재 요금표

            // ★신간/구간 판정★: (오늘 - 출판일)이 전환 기간 이하이면 신간
            bool isNew = (DateTime.Today - b.PublishDate).TotalDays <= s.SwitchPeriod;

            // 판정 결과에 따라 대여기간/대여료/연체단가 선택
            int days = isNew ? s.NewRentDays : s.OldRentDays;
            int fee  = isNew ? s.NewRentFee  : s.OldRentFee;
            int rate = isNew ? s.NewOverdueFee : s.OldOverdueFee;

            DateTime rent = dpRent.Value.Date;     // 대여일
            DateTime due = rent.AddDays(days);     // 반납예정일 = 대여일 + 대여기간

            // 대여 등록(이 시점 요금을 함께 저장)
            RentalDAO.Rent(curMember, code, rent, due, fee, rate);

            // 결과 표시 + 입력칸 정리
            tBTitle.Text = b.Title;
            tRentFee.Text = fee + "원";
            tOverdue.Text = rate + "원/일";
            tDue.Text = due.ToString("yyyy-MM-dd");
            tBCode.Text = "";
            LoadRentals();   // 목록 갱신
            tBCode.Focus();  // 다음 스캔 대기
        }

        // 현재 회원의 대여중 목록 + 합계를 계산해 그리드에 표시
        void LoadRentals()
        {
            var raw = RentalDAO.GetActiveRaw(curMember);   // DB에서 원본 행들

            // 화면 표시용 표를 새로 구성(컬럼 헤더를 한글로 직접 정의)
            var dt = new DataTable();
            dt.Columns.Add("번호"); dt.Columns.Add("도서코드"); dt.Columns.Add("제목");
            dt.Columns.Add("대여일"); dt.Columns.Add("반납예정일");
            dt.Columns.Add("대여료"); dt.Columns.Add("연체료(예상)"); dt.Columns.Add("상태");

            int totFee = 0, totOver = 0;   // 합계 누적용
            foreach (DataRow r in raw.Rows)
            {
                DateTime due = Convert.ToDateTime(r["DueDate"]);
                int rate = Convert.ToInt32(r["OverdueRate"]);
                int fee = Convert.ToInt32(r["RentFee"]);

                // ★예상 연체료(화면용)★: 오늘 기준 늦은 일수 × 연체단가.
                //   아직 반납 전이라 "지금 반납하면 이만큼"이라는 추정치다.
                //   실제 확정은 반납 시 RentalDAO.Return의 DATEDIFF로 들어간다.
                int late = Math.Max(0, (DateTime.Today - due).Days);  // 음수는 0으로
                int over = late * rate;

                dt.Rows.Add(r["RentalId"], r["BookCode"], r["Title"],
                    Convert.ToDateTime(r["RentDate"]).ToString("yyyy-MM-dd"),
                    due.ToString("yyyy-MM-dd"), fee, over,
                    late > 0 ? "연체" : "대여중");

                totFee += fee; totOver += over;   // 합계 누적
            }

            grid.DataSource = dt;
            lblTotCnt.Text = dt.Rows.Count + "권";
            lblTotFee.Text = totFee + "원";
            lblTotOver.Text = totOver + "원";
        }

        // 선택한 도서 반납: 반납일을 오늘로 처리(연체료는 DB에서 확정)
        void ReturnBook()
        {
            if (grid.CurrentRow == null) { MessageBox.Show("반납할 도서를 선택하세요."); return; }
            int id = Convert.ToInt32(grid.CurrentRow.Cells["번호"].Value);  // 대여 일련번호
            RentalDAO.Return(id, DateTime.Today);   // 반납 처리 + 연체료 확정
            LoadRentals();                           // 목록에서 사라짐(반납됨)
            MessageBox.Show("반납 처리되었습니다.");
        }
    }
}
```

---

## 10. QueryForm.cs — 정보 조회

```csharp
using System.Collections.Generic;   // List<SqlParameter>
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BookRentalSystem
{
    // 라디오 버튼으로 조회 종류를 고르고, 분류/등급으로 걸러 검색
    public class QueryForm : Form
    {
        RadioButton rbBookRank, rbActive, rbMemberRank;  // 조회 종류 3가지
        ComboBox cbCategory, cbGrade;                    // 필터(분류/등급)
        DataGridView grid;

        public QueryForm()
        {
            Text = "도서 조회 관리"; Width = 900; Height = 640;

            // 조회 종류 라디오(같은 부모 안에서는 하나만 선택됨). 첫 번째를 기본 체크
            rbBookRank   = new RadioButton { Text = "도서 대여 순위", Left = 20, Top = 20, Width = 140, Checked = true };
            rbActive     = new RadioButton { Text = "대여중인 도서", Left = 180, Top = 20, Width = 140 };
            rbMemberRank = new RadioButton { Text = "회원 대여 순위", Left = 20, Top = 50, Width = 140 };
            Controls.AddRange(new Control[] { rbBookRank, rbActive, rbMemberRank });

            // 분류 필터: '전체' + DB에서 읽은 실제 분류들
            UI.Lbl(this, "분류", 360, 20, 50);
            cbCategory = new ComboBox { Left = 415, Top = 20, Width = 150,
                                        DropDownStyle = ComboBoxStyle.DropDownList };
            cbCategory.Items.Add("전체");
            foreach (System.Data.DataRow r in BookDAO.Categories().Rows)
                cbCategory.Items.Add(r[0].ToString());
            cbCategory.SelectedIndex = 0; Controls.Add(cbCategory);

            // 등급 필터: '전체' + DB에서 읽은 실제 등급들
            UI.Lbl(this, "회원 등급", 360, 55, 60);
            cbGrade = new ComboBox { Left = 415, Top = 55, Width = 150,
                                     DropDownStyle = ComboBoxStyle.DropDownList };
            cbGrade.Items.Add("전체");
            foreach (System.Data.DataRow r in MemberDAO.Grades().Rows)
                cbGrade.Items.Add(r[0].ToString());
            cbGrade.SelectedIndex = 0; Controls.Add(cbGrade);

            UI.Btn(this, "검색", 600, 30, (s, e) => Search());
            UI.Btn(this, "나가기", 700, 30, (s, e) => Close());

            grid = new DataGridView
            {
                Left = 20, Top = 100, Width = 850, Height = 480,
                ReadOnly = true, AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            Controls.Add(grid);
        }

        // 선택된 라디오에 따라 SQL을 동적으로 만들어 실행
        //   값은 항상 파라미터로 넘긴다(문자열 결합 금지 → 인젝션 방지)
        void Search()
        {
            var ps = new List<SqlParameter>();   // 이번 조회의 파라미터 모음
            string sql;

            if (rbBookRank.Checked)   // (1) 도서 대여 순위: 도서별 대여 횟수 COUNT
            {
                sql = @"SELECT COUNT(*) AS 대여횟수, b.BookCode AS 코드, b.Title AS 제목,
                               b.Category AS 분류, b.Author AS 저자
                        FROM Rental r JOIN Book b ON r.BookCode=b.BookCode";
                if (cbCategory.Text != "전체")   // 분류 필터가 있으면 WHERE 추가
                { sql += " WHERE b.Category=@c"; ps.Add(new SqlParameter("@c", cbCategory.Text)); }
                sql += " GROUP BY b.BookCode,b.Title,b.Category,b.Author ORDER BY 대여횟수 DESC";
            }
            else if (rbMemberRank.Checked)  // (2) 회원 대여 순위: 회원별 대여 횟수 COUNT
            {
                sql = @"SELECT COUNT(*) AS 대여횟수, m.Name AS [회원 이름], m.Grade AS 등급,
                               m.Gender AS 성별, m.Phone AS 연락처, m.Mobile AS 휴대폰, m.Address AS 주소
                        FROM Rental r JOIN Member m ON r.MemberNo=m.MemberNo";
                if (cbGrade.Text != "전체")   // 등급 필터
                { sql += " WHERE m.Grade=@gr"; ps.Add(new SqlParameter("@gr", cbGrade.Text)); }
                sql += " GROUP BY m.Name,m.Grade,m.Gender,m.Phone,m.Mobile,m.Address ORDER BY 대여횟수 DESC";
            }
            else  // (3) 대여중인 도서: 아직 반납 안 된 건만(IsReturned=0)
            {
                sql = @"SELECT b.BookCode AS 코드, b.Title AS 제목, b.Category AS 분류,
                               m.Name AS 회원명, m.Grade AS 등급,
                               CONVERT(varchar(10),r.RentDate,23) AS 대여일,
                               CONVERT(varchar(10),r.DueDate,23) AS 반납예정일
                        FROM Rental r JOIN Book b ON r.BookCode=b.BookCode
                                      JOIN Member m ON r.MemberNo=m.MemberNo
                        WHERE r.IsReturned=0";
                if (cbCategory.Text != "전체")   // 두 필터를 AND로 누적
                { sql += " AND b.Category=@c"; ps.Add(new SqlParameter("@c", cbCategory.Text)); }
                if (cbGrade.Text != "전체")
                { sql += " AND m.Grade=@gr"; ps.Add(new SqlParameter("@gr", cbGrade.Text)); }
                sql += " ORDER BY r.DueDate";
            }

            grid.DataSource = DBManager.Query(sql, ps.ToArray());   // 실행 후 표시
        }
    }
}
```

---

## 11. SettingForm.cs — 환경 설정

```csharp
using System.Data;
using System.IO;          // File, SaveFileDialog 관련
using System.Text;        // StringBuilder, UTF8Encoding
using System.Windows.Forms;

namespace BookRentalSystem
{
    // 신간/구간 요금 수정·저장 + 도서목록/대여현황 CSV 내보내기
    public class SettingForm : Form
    {
        TextBox tSwitch, tNewDays, tNewFee, tNewOver, tOldDays, tOldFee, tOldOver;

        public SettingForm()
        {
            Text = "환경 설정 / 도서 대여 설정"; Width = 620; Height = 480;

            // --- 신간 설정 그룹 ---
            var gNew = new GroupBox { Text = "신간", Left = 20, Top = 20, Width = 560, Height = 110 };
            Controls.Add(gNew);
            UI.Lbl(gNew, "전환 기간(일)", 15, 25, 90); tSwitch = UI.Txt(gNew, 110, 25, 60);  // 신간/구간 경계
            UI.Lbl(gNew, "대여료", 300, 25, 50);       tNewFee = UI.Txt(gNew, 355, 25, 80);
            UI.Lbl(gNew, "대여 기간(일)", 15, 60, 90); tNewDays = UI.Txt(gNew, 110, 60, 60);
            UI.Lbl(gNew, "연체료(/일)", 300, 60, 70);  tNewOver = UI.Txt(gNew, 375, 60, 80);

            // --- 구간 설정 그룹 ---
            var gOld = new GroupBox { Text = "구간", Left = 20, Top = 140, Width = 560, Height = 110 };
            Controls.Add(gOld);
            UI.Lbl(gOld, "대여 기간(일)", 15, 25, 90); tOldDays = UI.Txt(gOld, 110, 25, 60);
            UI.Lbl(gOld, "대여료", 300, 25, 50);       tOldFee = UI.Txt(gOld, 355, 25, 80);
            UI.Lbl(gOld, "연체료(/일)", 300, 60, 70);  tOldOver = UI.Txt(gOld, 375, 60, 80);

            UI.Btn(this, "수정 저장", 20, 265, (s, e) => SaveSetting(), 90);
            UI.Btn(this, "나가기", 490, 265, (s, e) => Close());

            // --- CSV 내보내기 그룹 ---
            var gExcel = new GroupBox { Text = "엑셀로 출력", Left = 20, Top = 310, Width = 560, Height = 100 };
            Controls.Add(gExcel);
            UI.Btn(gExcel, "도서 목록", 30, 35, (s, e) => ExportBookList(), 120);
            UI.Btn(gExcel, "대여 현황", 180, 35, (s, e) => ExportRentalStatus(), 120);

            LoadSetting();   // 현재 요금표를 입력칸에 채움
        }

        // 현재 요금표를 읽어 입력칸에 표시
        void LoadSetting()
        {
            var s = SettingDAO.Get();
            tSwitch.Text = s.SwitchPeriod.ToString();
            tNewDays.Text = s.NewRentDays.ToString(); tNewFee.Text = s.NewRentFee.ToString();
            tNewOver.Text = s.NewOverdueFee.ToString();
            tOldDays.Text = s.OldRentDays.ToString(); tOldFee.Text = s.OldRentFee.ToString();
            tOldOver.Text = s.OldOverdueFee.ToString();
        }

        // 입력값을 Setting으로 묶어 저장. 숫자가 아니면 int.Parse 예외를 잡아 안내
        void SaveSetting()
        {
            try
            {
                var s = new Setting
                {
                    SwitchPeriod = int.Parse(tSwitch.Text), NewRentDays = int.Parse(tNewDays.Text),
                    NewRentFee = int.Parse(tNewFee.Text), NewOverdueFee = int.Parse(tNewOver.Text),
                    OldRentDays = int.Parse(tOldDays.Text), OldRentFee = int.Parse(tOldFee.Text),
                    OldOverdueFee = int.Parse(tOldOver.Text)
                };
                SettingDAO.Save(s);
                MessageBox.Show("설정이 저장되었습니다.");
            }
            catch { MessageBox.Show("숫자만 입력하세요."); }
        }

        // 도서 목록 CSV: BookDAO.GetAll() 결과를 그대로 내보냄
        void ExportBookList() => ToCsv(BookDAO.GetAll(), "도서목록");

        // 대여 현황 CSV: 대여 내역을 회원/도서와 조인해 내보냄
        void ExportRentalStatus()
        {
            var dt = DBManager.Query(
                @"SELECT r.BookCode AS 코드, b.Title AS 제목, m.Name AS 회원명,
                         CONVERT(varchar(10),r.RentDate,23) AS 대여일,
                         CONVERT(varchar(10),r.DueDate,23) AS 반납예정일,
                         CASE WHEN r.IsReturned=1 THEN '반납' ELSE '대여중' END AS 상태,
                         r.RentFee AS 대여료, r.OverdueFee AS 연체료
                  FROM Rental r JOIN Book b ON r.BookCode=b.BookCode
                                JOIN Member m ON r.MemberNo=m.MemberNo
                  ORDER BY r.RentDate DESC");
            ToCsv(dt, "대여현황");
        }

        // DataTable을 CSV로 저장(외부 라이브러리 없이). 엑셀에서 바로 열린다.
        void ToCsv(DataTable dt, string name)
        {
            using (var sfd = new SaveFileDialog { Filter = "CSV 파일|*.csv", FileName = name + ".csv" })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;   // 취소면 중단

                var sb = new StringBuilder();

                // 1) 헤더 줄: 컬럼명을 콤마로 잇고, 마지막은 줄바꿈(\r\n)
                for (int i = 0; i < dt.Columns.Count; i++)
                    sb.Append(dt.Columns[i].ColumnName + (i < dt.Columns.Count - 1 ? "," : "\r\n"));

                // 2) 데이터 줄: 각 값을 큰따옴표로 감싸고, 안의 "는 ""로 이스케이프
                //    (값에 콤마/따옴표가 있어도 깨지지 않게)
                foreach (DataRow r in dt.Rows)
                    for (int i = 0; i < dt.Columns.Count; i++)
                        sb.Append("\"" + r[i].ToString().Replace("\"", "\"\"") + "\""
                                  + (i < dt.Columns.Count - 1 ? "," : "\r\n"));

                // 3) ★UTF-8 BOM(true)으로 저장★ → 엑셀에서 한글이 안 깨진다
                File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(true));
                MessageBox.Show("저장 완료: " + sfd.FileName);
            }
        }
        // 진짜 .xlsx가 필요하면 NuGet의 ClosedXML로 이 ToCsv 부분만 교체하면 된다.
    }
}
```

---

## 12. 실행 순서 요약

1. SSMS에서 **1번 SQL 스크립트** 실행(DB·테이블·샘플 생성).
2. `DB.cs`의 **`ConnectionString`** 을 본인 환경에 맞게 수정.
3. F5 실행 → 메인 창.
4. 도서 정보에서 도서 등록 → 회원 정보에서 회원 등록(필요 시 카드 발급).
5. 대여 관리에서 회원 찾고(코드/이름/카드) 도서 코드 입력 → 등록 → 반납.
6. 정보 조회로 순위·대여중 확인, 환경설정에서 요금 변경 및 CSV 출력.

주석을 따라가며 한 사이클을 직접 돌려 보면, 화면 → DAO → DBManager → DB로 값이 흐르는 경로가 코드 위에서 그대로 보인다.
