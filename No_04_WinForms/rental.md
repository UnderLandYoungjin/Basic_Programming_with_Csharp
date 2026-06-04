# 도서 대여 관리 시스템 만들기 (C# WinForms + ADO.NET)

MDI 기반 도서 대여점 관리 프로그램을 처음부터 끝까지 만든다.
데이터베이스는 SQL Server(MSSQL)를 쓰고, 데이터 접근은 ORM 없이 **ADO.NET**(`SqlConnection`, `SqlCommand`, `SqlDataAdapter`)으로 직접 다룬다.

(비디오 대여 관리 시스템과 구조·계층이 동일하다. 도메인만 도서로 바뀌었다.)

---

## 1. 만들 프로그램

도서 대여점에서 쓰는 관리 프로그램이다. 메인 창(MDI) 안에 기능별 자식 창이 뜬다.

| 화면 | 폼 클래스 | 하는 일 |
|------|-----------|---------|
| 메인 | `MainForm` | MDI 컨테이너, 메뉴로 각 창 열기 |
| 도서 정보 | `BookForm` | 도서 등록/수정/삭제, 목록 |
| 회원 정보 | `MemberForm` | 회원 등록/수정/삭제, RFID 카드 발급 |
| 대여 관리 | `RentalForm` | 회원 검색(코드/이름/카드) → 도서 대여/반납 |
| 정보 조회 | `QueryForm` | 도서 대여순위 / 회원 대여순위 / 대여중인 도서 |
| 환경 설정 | `SettingForm` | 신간·구간 요금 설정, 목록/현황 엑셀(CSV) 출력 |

핵심 동작은 이렇게 흘러간다.

1. 도서와 회원을 먼저 등록한다.
2. 대여 관리에서 회원을 찾고, 도서 코드를 입력하면
   - 출판일 + 환경설정의 "전환 기간"으로 **신간/구간을 자동 판정**하고
   - 그에 맞는 **대여료·대여기간·연체단가**를 자동으로 적용한다.
3. 반납할 때 `DATEDIFF`로 **연체료를 계산**한다.

> **바코드 / RFID 안내**
> 바코드 스캐너는 키보드처럼 텍스트박스에 글자를 찍고 Enter를 누르는 장치다. 그래서 별도 코드가 필요 없다. 그냥 코드 입력칸에 스캔(ISBN/도서코드)하면 된다.
> RFID 카드는 하드웨어가 있어야 하므로, 여기서는 카드번호를 만들어 DB에 저장하고 그 번호로 조회하는 방식으로 시뮬레이션한다.

---

## 2. 개발 환경

- Visual Studio 2019 이상
- 프로젝트 형식: **Windows Forms App (.NET Framework)**
  - .NET Framework는 `System.Data.SqlClient`가 기본 내장이라 NuGet이 필요 없다.
  - .NET 6/7/8로 만들고 싶으면 NuGet에서 **`Microsoft.Data.SqlClient`** 를 받고, 코드의 `using System.Data.SqlClient;` 를 `using Microsoft.Data.SqlClient;` 로만 바꾸면 그대로 돌아간다.
- SQL Server (LocalDB / Express / 일반 인스턴스 아무거나)
- SSMS(SQL Server Management Studio) 또는 VS의 SQL Server 개체 탐색기

---

## 3. 프로젝트 생성

1. Visual Studio → **새 프로젝트 만들기**
2. **Windows Forms 앱(.NET Framework)**, C# 선택
3. 프로젝트 이름: `BookRentalSystem`
   - 아래 모든 코드의 네임스페이스가 `BookRentalSystem`이다. 이름을 다르게 했다면 각 파일 `namespace` 줄을 맞춰 줘야 한다.
4. 만들어진 기본 `Form1.cs`는 삭제한다. (메인 폼은 직접 만든다)

이 실습은 **디자이너로 컨트롤을 끌어다 놓지 않고 코드로 UI를 구성**한다. 디자이너 파일에 묶이지 않아서 한 파일만 보면 화면이 다 보이고, 복사·붙여넣기로 따라 하기 좋기 때문이다.

---

## 4. 파일 구조

프로젝트에 아래 `.cs` 파일들을 추가한다. (솔루션 탐색기 → 프로젝트 우클릭 → 추가 → 클래스)

```
BookRentalSystem/
├── Program.cs          // 진입점
├── DB.cs               // ADO.NET 헬퍼 + UI 헬퍼
├── Models.cs           // Book, Member, Setting 모델
├── DAO.cs              // DB 접근 계층 (Book/Member/Rental/Setting DAO)
├── MainForm.cs         // MDI 메인
├── BookForm.cs         // 도서 정보
├── MemberForm.cs       // 회원 정보
├── RentalForm.cs       // 대여 관리
├── QueryForm.cs        // 정보 조회
└── SettingForm.cs      // 환경 설정
```

계층은 이렇게 나뉜다.

```
[ Form들 (화면) ]
        │  메서드 호출
        ▼
[ DAO (SQL 정의) ]
        │  SQL + 파라미터
        ▼
[ DBManager (ADO.NET 실행) ]
        │
        ▼
[ SQL Server ]
```

화면은 SQL을 직접 쓰지 않고 DAO만 부른다. DAO는 SQL을 만들어 DBManager에 넘긴다. DBManager는 연결·실행만 책임진다. 이렇게 나눠 두면 DB가 바뀌어도 DBManager만 손보면 된다.

---

## 5. 데이터베이스 구축

SSMS에서 아래 스크립트를 통째로 실행한다. DB, 테이블, 기본 설정값, 샘플 데이터까지 한 번에 만든다.

```sql
CREATE DATABASE BookRentalDB;
GO
USE BookRentalDB;
GO

-- 도서
CREATE TABLE Book (
    BookCode    VARCHAR(20)   NOT NULL PRIMARY KEY,   -- ISBN/바코드로 입력
    Category    NVARCHAR(30)  NULL,                   -- 분류(소설, 자기계발 등)
    Title       NVARCHAR(100) NULL,
    Author      NVARCHAR(50)  NULL,                   -- 저자
    Translator  NVARCHAR(50)  NULL,                   -- 역자
    Publisher   NVARCHAR(50)  NULL,                   -- 출판사
    PublishDate DATE          NULL                    -- 출판일
);

-- 회원
CREATE TABLE Member (
    MemberNo INT           NOT NULL PRIMARY KEY,       -- 바코드로 입력
    Name     NVARCHAR(30)  NULL,
    Jumin    VARCHAR(15)   NULL,
    Grade    NVARCHAR(10)  NULL,                       -- 일반 / 학생
    Gender   NVARCHAR(5)   NULL,
    Phone    VARCHAR(20)   NULL,
    Mobile   VARCHAR(20)   NULL,
    ZipCode  VARCHAR(10)   NULL,
    Address  NVARCHAR(100) NULL,
    CardId   VARCHAR(40)   NULL                        -- RFID 카드번호
);

-- 대여 내역
CREATE TABLE Rental (
    RentalId    INT IDENTITY(1,1) PRIMARY KEY,
    MemberNo    INT          NOT NULL,
    BookCode    VARCHAR(20)  NOT NULL,
    RentDate    DATE         NOT NULL,
    DueDate     DATE         NOT NULL,
    ReturnDate  DATE         NULL,
    RentFee     INT          NOT NULL DEFAULT 0,
    OverdueRate INT          NOT NULL DEFAULT 0,       -- 1일당 연체단가(대여 시점에 확정)
    OverdueFee  INT          NOT NULL DEFAULT 0,       -- 반납 시 실제 부과액
    IsReturned  BIT          NOT NULL DEFAULT 0,
    FOREIGN KEY (MemberNo) REFERENCES Member(MemberNo),
    FOREIGN KEY (BookCode) REFERENCES Book(BookCode)
);

-- 요금 설정 (한 행만 유지)
CREATE TABLE RentalSetting (
    Id            INT PRIMARY KEY,
    SwitchPeriod  INT NOT NULL,   -- 출판 후 N일 이내면 신간
    NewRentDays   INT NOT NULL,
    NewRentFee    INT NOT NULL,
    NewOverdueFee INT NOT NULL,
    OldRentDays   INT NOT NULL,
    OldRentFee    INT NOT NULL,
    OldOverdueFee INT NOT NULL
);

-- 신간: 14일 이내 / 7일 대여 / 700원 / 연체 200원
-- 구간: 14일 대여 / 500원 / 연체 100원
INSERT INTO RentalSetting VALUES (1, 14, 7, 700, 200, 14, 500, 100);

-- 샘플 데이터
INSERT INTO Book VALUES ('B0001','소설','테스트','테스트','','테스트출판','2008-01-01');
INSERT INTO Book VALUES ('B0002','자기계발','아주 작은 습관의 힘','제임스 클리어','이한이','비즈니스북스','2019-02-26');
INSERT INTO Member VALUES (101,'홍길동','123456-1234567','일반','남자','055-123-4567','010-1234-1234','123-456','대한민국',NULL);
```

테이블 설계에서 짚어 둘 점이 두 가지 있다.

- `Rental`에 `RentFee`, `OverdueRate`를 **대여 시점에 복사해서 저장**한다. 요금표(`RentalSetting`)는 나중에 바뀔 수 있으니, 그때 적용된 금액을 내역에 박아 두는 것이다. 이렇게 해야 과거 대여 건의 정산이 흔들리지 않는다.
- 연체료(`OverdueFee`)는 빌릴 때가 아니라 **반납할 때** 날짜 차이로 계산해 채운다.

---

## 6. 코드

여기부터는 파일별 전체 코드다. 위에서 만든 파일 이름에 그대로 붙여 넣으면 된다.

### 6-1. DB.cs — ADO.NET 헬퍼 + UI 헬퍼

ADO.NET 호출이 반복되는 부분(`Query`/`Execute`/`Scalar`)과, 코드로 컨트롤을 만드는 보조 함수(`UI`)를 여기 모아 둔다.

```csharp
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace BookRentalSystem
{
    public static class DBManager
    {
        // ★ 환경에 맞게 수정하는 곳 ★
        // LocalDB:   Server=(localdb)\MSSQLLocalDB;Database=BookRentalDB;Integrated Security=True;
        // Express:   Server=.\SQLEXPRESS;Database=BookRentalDB;Integrated Security=True;
        // 계정 로그인: Server=localhost;Database=BookRentalDB;User Id=sa;Password=암호;
        public static string ConnectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=BookRentalDB;Integrated Security=True;";

        // SELECT → DataTable 반환
        public static DataTable Query(string sql, params SqlParameter[] ps)
        {
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                var dt = new DataTable();
                using (var da = new SqlDataAdapter(cmd)) da.Fill(dt);
                return dt;
            }
        }

        // INSERT / UPDATE / DELETE → 영향받은 행 수
        public static int Execute(string sql, params SqlParameter[] ps)
        {
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // COUNT 등 단일 값
        public static object Scalar(string sql, params SqlParameter[] ps)
        {
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                con.Open();
                return cmd.ExecuteScalar();
            }
        }
    }

    // 디자이너 없이 코드로 화면을 짜기 위한 보조 함수 모음
    public static class UI
    {
        public static Label Lbl(Control p, string t, int x, int y, int w = 80)
        {
            var l = new Label { Text = t, Left = x, Top = y + 4, Width = w,
                                TextAlign = ContentAlignment.MiddleRight };
            p.Controls.Add(l); return l;
        }
        public static TextBox Txt(Control p, int x, int y, int w = 150, bool ro = false)
        {
            var t = new TextBox { Left = x, Top = y, Width = w, ReadOnly = ro };
            p.Controls.Add(t); return t;
        }
        public static Button Btn(Control p, string t, int x, int y, EventHandler click, int w = 80)
        {
            var b = new Button { Text = t, Left = x, Top = y, Width = w, Height = 28 };
            b.Click += click; p.Controls.Add(b); return b;
        }
        // 간단한 입력 대화상자 (카드번호 입력 등에 사용)
        public static string Prompt(string title, string label, string def = "")
        {
            using (var f = new Form { Width = 340, Height = 150, Text = title,
                FormBorderStyle = FormBorderStyle.FixedDialog, StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false, MinimizeBox = false })
            {
                var l = new Label { Left = 12, Top = 15, Width = 300, Text = label };
                var t = new TextBox { Left = 12, Top = 40, Width = 300, Text = def };
                var ok = new Button { Text = "확인", Left = 150, Top = 75, DialogResult = DialogResult.OK };
                f.Controls.AddRange(new Control[] { l, t, ok }); f.AcceptButton = ok;
                return f.ShowDialog() == DialogResult.OK ? t.Text.Trim() : null;
            }
        }
    }
}
```

ADO.NET을 다룰 때 두 가지를 습관으로 들이는 게 좋다.

- **`using` 블록**으로 `SqlConnection`/`SqlCommand`를 감싼다. 블록을 벗어나면 연결이 자동으로 닫히고 반환된다.
- **파라미터(`@변수`)** 로 값을 넘긴다. 문자열을 `+`로 이어 붙여 SQL을 만들면 SQL 인젝션에 뚫린다.

### 6-2. Models.cs — 데이터 모델

DB 한 행을 담아 옮기는 단순한 클래스들이다.

```csharp
using System;

namespace BookRentalSystem
{
    public class Book
    {
        public string BookCode, Category, Title, Author, Translator, Publisher;
        public DateTime PublishDate = DateTime.Today;
    }

    public class Member
    {
        public int MemberNo;
        public string Name, Jumin, Grade, Gender, Phone, Mobile, ZipCode, Address, CardId;
    }

    public class Setting
    {
        public int SwitchPeriod, NewRentDays, NewRentFee, NewOverdueFee,
                   OldRentDays, OldRentFee, OldOverdueFee;
    }
}
```

### 6-3. DAO.cs — DB 접근 계층

SQL은 전부 여기 모은다. 화면에서는 `BookDAO.Insert(b)` 같은 메서드만 부른다.

```csharp
using System;
using System.Data;
using System.Data.SqlClient;

namespace BookRentalSystem
{
    // ===== 도서 =====
    public static class BookDAO
    {
        public static DataTable GetAll() => DBManager.Query(
            @"SELECT BookCode AS 코드, Category AS 분류, Title AS 제목, Author AS 저자,
                     Translator AS 역자, Publisher AS 출판사,
                     CONVERT(varchar(10),PublishDate,23) AS 출판일
              FROM Book ORDER BY BookCode");

        public static int Count() => (int)DBManager.Scalar("SELECT COUNT(*) FROM Book");

        public static bool Exists(string code) =>
            (int)DBManager.Scalar("SELECT COUNT(*) FROM Book WHERE BookCode=@c",
                new SqlParameter("@c", code)) > 0;

        public static Book GetByCode(string code)
        {
            var dt = DBManager.Query("SELECT * FROM Book WHERE BookCode=@c",
                new SqlParameter("@c", code));
            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return new Book
            {
                BookCode = r["BookCode"].ToString(),
                Category = r["Category"].ToString(),
                Title = r["Title"].ToString(),
                Author = r["Author"].ToString(),
                Translator = r["Translator"].ToString(),
                Publisher = r["Publisher"].ToString(),
                PublishDate = r["PublishDate"] == DBNull.Value
                    ? DateTime.Today : Convert.ToDateTime(r["PublishDate"])
            };
        }

        public static void Insert(Book b) => DBManager.Execute(
            @"INSERT INTO Book(BookCode,Category,Title,Author,Translator,Publisher,PublishDate)
              VALUES(@code,@c,@t,@a,@tr,@p,@d)", P(b));

        public static void Update(Book b) => DBManager.Execute(
            @"UPDATE Book SET Category=@c,Title=@t,Author=@a,Translator=@tr,Publisher=@p,PublishDate=@d
              WHERE BookCode=@code", P(b));

        public static void Delete(string code) => DBManager.Execute(
            "DELETE FROM Book WHERE BookCode=@c", new SqlParameter("@c", code));

        public static DataTable Categories() => DBManager.Query(
            "SELECT DISTINCT Category FROM Book WHERE Category IS NOT NULL ORDER BY Category");

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

    // ===== 회원 =====
    public static class MemberDAO
    {
        public static DataTable GetAll() => DBManager.Query(
            @"SELECT MemberNo AS 코드, Name AS 성명, Jumin AS 주민등록번,
                     Grade AS 등급, Gender AS 성별, Phone AS 연락처, Mobile AS 휴대폰
              FROM Member ORDER BY MemberNo");

        public static int Count() => (int)DBManager.Scalar("SELECT COUNT(*) FROM Member");

        public static bool Exists(int no) =>
            (int)DBManager.Scalar("SELECT COUNT(*) FROM Member WHERE MemberNo=@n",
                new SqlParameter("@n", no)) > 0;

        public static Member GetByNo(int no) => Map(
            DBManager.Query("SELECT * FROM Member WHERE MemberNo=@n", new SqlParameter("@n", no)));

        public static Member GetByCard(string card) => Map(
            DBManager.Query("SELECT * FROM Member WHERE CardId=@c", new SqlParameter("@c", card)));

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

        // RFID 카드 발급 (실하드웨어 대신 카드번호 생성·저장)
        public static string IssueCard(int no)
        {
            string card = "RFID-" + no + "-" + new Random().Next(1000, 9999);
            DBManager.Execute("UPDATE Member SET CardId=@c WHERE MemberNo=@n",
                new SqlParameter("@c", card), new SqlParameter("@n", no));
            return card;
        }

        public static DataTable Grades() => DBManager.Query(
            "SELECT DISTINCT Grade FROM Member WHERE Grade IS NOT NULL ORDER BY Grade");

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

    // ===== 대여 =====
    public static class RentalDAO
    {
        public static bool IsActive(string bookCode) =>
            (int)DBManager.Scalar(
                "SELECT COUNT(*) FROM Rental WHERE BookCode=@b AND IsReturned=0",
                new SqlParameter("@b", bookCode)) > 0;

        public static DataTable GetActiveRaw(int memberNo) => DBManager.Query(
            @"SELECT r.RentalId, r.BookCode, b.Title, r.RentDate, r.DueDate,
                     r.RentFee, r.OverdueRate
              FROM Rental r JOIN Book b ON r.BookCode=b.BookCode
              WHERE r.MemberNo=@no AND r.IsReturned=0
              ORDER BY r.RentDate",
            new SqlParameter("@no", memberNo));

        public static void Rent(int memberNo, string bookCode, DateTime rent,
                                DateTime due, int rentFee, int overdueRate) =>
            DBManager.Execute(
                @"INSERT INTO Rental(MemberNo,BookCode,RentDate,DueDate,RentFee,OverdueRate)
                  VALUES(@c,@b,@rd,@dd,@rf,@or)",
                new SqlParameter("@c", memberNo), new SqlParameter("@b", bookCode),
                new SqlParameter("@rd", rent), new SqlParameter("@dd", due),
                new SqlParameter("@rf", rentFee), new SqlParameter("@or", overdueRate));

        // 반납: 반납 처리 + 연체일수 × 연체단가로 연체료 확정
        public static void Return(int rentalId, DateTime returnDate) =>
            DBManager.Execute(
                @"UPDATE Rental
                  SET IsReturned=1, ReturnDate=@d,
                      OverdueFee = CASE WHEN DATEDIFF(day,DueDate,@d) > 0
                                        THEN DATEDIFF(day,DueDate,@d)*OverdueRate ELSE 0 END
                  WHERE RentalId=@id",
                new SqlParameter("@d", returnDate), new SqlParameter("@id", rentalId));
    }

    // ===== 설정 =====
    public static class SettingDAO
    {
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

`SELECT` 컬럼에 `AS 코드`처럼 한글 별칭을 준 이유가 있다. 이 `DataTable`을 그대로 `DataGridView.DataSource`에 넣으면 별칭이 **컬럼 헤더**가 되기 때문에, 그리드 컬럼을 따로 손볼 필요가 없다.

### 6-4. MainForm.cs — MDI 메인

```csharp
using System;
using System.Windows.Forms;

namespace BookRentalSystem
{
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "도서 관리 프로그램";
            IsMdiContainer = true;                 // 자식 창을 품는 컨테이너
            WindowState = FormWindowState.Maximized;

            var menu = new MenuStrip();

            var mFile = new ToolStripMenuItem("파일");
            mFile.DropDownItems.Add("종료", null, (s, e) => Close());

            var mRent = new ToolStripMenuItem("도서 대여/반납");
            mRent.DropDownItems.Add("대여 관리", null, (s, e) => Open(new RentalForm()));

            var mBook = new ToolStripMenuItem("도서 관리");
            mBook.DropDownItems.Add("도서 정보", null, (s, e) => Open(new BookForm()));

            var mMember = new ToolStripMenuItem("회원 관리");
            mMember.DropDownItems.Add("회원 정보", null, (s, e) => Open(new MemberForm()));

            var mQuery = new ToolStripMenuItem("정보 조회");
            mQuery.DropDownItems.Add("정보 조회", null, (s, e) => Open(new QueryForm()));

            var mEnv = new ToolStripMenuItem("환경설정");
            mEnv.DropDownItems.Add("환경 설정", null, (s, e) => Open(new SettingForm()));

            menu.Items.AddRange(new ToolStripItem[] { mFile, mRent, mBook, mMember, mQuery, mEnv });
            MainMenuStrip = menu;
            Controls.Add(menu);
        }

        // 같은 종류 창이 이미 떠 있으면 새로 열지 않고 활성화만
        void Open(Form f)
        {
            foreach (var c in MdiChildren)
                if (c.GetType() == f.GetType()) { c.Activate(); f.Dispose(); return; }
            f.MdiParent = this;
            f.Show();
        }
    }
}
```

`IsMdiContainer = true`로 두고, 자식 폼의 `MdiParent`를 이 폼으로 지정하면 메인 창 안쪽에 자식 창이 뜬다. `Open` 메서드는 같은 창을 두 번 열지 않게 막아 준다.

### 6-5. BookForm.cs — 도서 정보

```csharp
using System;
using System.Windows.Forms;

namespace BookRentalSystem
{
    public class BookForm : Form
    {
        TextBox tCode, tCategory, tTitle, tAuthor, tTranslator, tPublisher;
        DateTimePicker dpPublish;
        Label lblCount;
        DataGridView grid;

        public BookForm()
        {
            Text = "도서 정보"; Width = 720; Height = 600;

            UI.Lbl(this, "도서 코드", 20, 20);  tCode = UI.Txt(this, 110, 20);
            UI.Lbl(this, "분류", 290, 20);      tCategory = UI.Txt(this, 360, 20);
            UI.Lbl(this, "제목", 20, 55);       tTitle = UI.Txt(this, 110, 55, 400);
            UI.Lbl(this, "저자", 20, 90);       tAuthor = UI.Txt(this, 110, 90);
            UI.Lbl(this, "역자", 290, 90);      tTranslator = UI.Txt(this, 360, 90);
            UI.Lbl(this, "출판사", 20, 125);    tPublisher = UI.Txt(this, 110, 125);
            UI.Lbl(this, "출판일", 290, 125);
            dpPublish = new DateTimePicker { Left = 360, Top = 125, Width = 150, Format = DateTimePickerFormat.Short };
            Controls.Add(dpPublish);

            UI.Btn(this, "추가", 20, 165, (s, e) => NewMode());
            UI.Btn(this, "저장", 110, 165, (s, e) => Save());
            UI.Btn(this, "삭제", 200, 165, (s, e) => Delete());
            UI.Btn(this, "취소", 290, 165, (s, e) => Load());
            UI.Btn(this, "나가기", 540, 165, (s, e) => Close());

            UI.Lbl(this, "전체 도서 수 :", 20, 205, 110);
            lblCount = new Label { Left = 135, Top = 209, Width = 80, Text = "0권" };
            Controls.Add(lblCount);

            grid = new DataGridView { Left = 20, Top = 235, Width = 660, Height = 320,
                ReadOnly = true, AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            grid.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadRow(e.RowIndex); };
            Controls.Add(grid);

            Load();
        }

        void Load()
        {
            grid.DataSource = BookDAO.GetAll();
            lblCount.Text = BookDAO.Count() + "권";
        }

        void NewMode()
        {
            tCode.Text = tCategory.Text = tTitle.Text = tAuthor.Text =
                tTranslator.Text = tPublisher.Text = "";
            dpPublish.Value = DateTime.Today;
            tCode.Focus();
        }

        // 그리드 한 줄 클릭 → 입력칸에 채우기
        void LoadRow(int row)
        {
            string code = grid.Rows[row].Cells["코드"].Value.ToString();
            var b = BookDAO.GetByCode(code);
            if (b == null) return;
            tCode.Text = b.BookCode; tCategory.Text = b.Category; tTitle.Text = b.Title;
            tAuthor.Text = b.Author; tTranslator.Text = b.Translator; tPublisher.Text = b.Publisher;
            dpPublish.Value = b.PublishDate;
        }

        // 코드가 있으면 UPDATE, 없으면 INSERT (저장 버튼 하나로 처리)
        void Save()
        {
            if (tCode.Text.Trim() == "")
            { MessageBox.Show("도서 코드를 입력하세요."); return; }

            var b = new Book
            {
                BookCode = tCode.Text.Trim(), Category = tCategory.Text.Trim(),
                Title = tTitle.Text.Trim(), Author = tAuthor.Text.Trim(),
                Translator = tTranslator.Text.Trim(), Publisher = tPublisher.Text.Trim(),
                PublishDate = dpPublish.Value.Date
            };
            try
            {
                if (BookDAO.Exists(b.BookCode)) BookDAO.Update(b);
                else BookDAO.Insert(b);
                Load();
                MessageBox.Show("저장되었습니다.");
            }
            catch (Exception ex) { MessageBox.Show("오류: " + ex.Message); }
        }

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

### 6-6. MemberForm.cs — 회원 정보

```csharp
using System;
using System.Windows.Forms;

namespace BookRentalSystem
{
    public class MemberForm : Form
    {
        TextBox tNo, tName, tJumin, tPhone, tMobile, tZip, tAddr;
        ComboBox cbGrade, cbGender;
        Label lblCount;
        DataGridView grid;

        public MemberForm()
        {
            Text = "회원 정보"; Width = 720; Height = 620;

            UI.Lbl(this, "회원번호", 20, 20);   tNo = UI.Txt(this, 110, 20);
            UI.Lbl(this, "주민등록번호", 290, 20); tJumin = UI.Txt(this, 380, 20);
            UI.Lbl(this, "회원명", 20, 55);     tName = UI.Txt(this, 110, 55);
            UI.Lbl(this, "회원 등급", 20, 90);
            cbGrade = new ComboBox { Left = 110, Top = 90, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cbGrade.Items.AddRange(new[] { "일반", "학생" }); cbGrade.SelectedIndex = 0; Controls.Add(cbGrade);
            UI.Lbl(this, "성별", 290, 90);
            cbGender = new ComboBox { Left = 380, Top = 90, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cbGender.Items.AddRange(new[] { "남자", "여자" }); cbGender.SelectedIndex = 0; Controls.Add(cbGender);
            UI.Lbl(this, "전화번호", 20, 125);  tPhone = UI.Txt(this, 110, 125);
            UI.Lbl(this, "휴대폰", 290, 125);   tMobile = UI.Txt(this, 380, 125);
            UI.Lbl(this, "우편번호", 20, 160);  tZip = UI.Txt(this, 110, 160);
            UI.Lbl(this, "주소", 20, 195);      tAddr = UI.Txt(this, 110, 195, 420);

            UI.Btn(this, "추가", 20, 230, (s, e) => NewMode());
            UI.Btn(this, "저장", 110, 230, (s, e) => Save());
            UI.Btn(this, "삭제", 200, 230, (s, e) => Delete());
            UI.Btn(this, "취소", 290, 230, (s, e) => Load());
            UI.Btn(this, "카드 관리", 400, 230, (s, e) => IssueCard(), 90);
            UI.Btn(this, "나가기", 540, 230, (s, e) => Close());

            UI.Lbl(this, "현재 회원 수 :", 20, 270, 100);
            lblCount = new Label { Left = 125, Top = 274, Width = 80, Text = "0명" };
            Controls.Add(lblCount);

            grid = new DataGridView { Left = 20, Top = 300, Width = 660, Height = 270,
                ReadOnly = true, AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect };
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

        void LoadRow(int row)
        {
            int no = Convert.ToInt32(grid.Rows[row].Cells["코드"].Value);
            var m = MemberDAO.GetByNo(no);
            if (m == null) return;
            tNo.Text = m.MemberNo.ToString(); tName.Text = m.Name; tJumin.Text = m.Jumin;
            cbGrade.Text = m.Grade; cbGender.Text = m.Gender;
            tPhone.Text = m.Phone; tMobile.Text = m.Mobile; tZip.Text = m.ZipCode; tAddr.Text = m.Address;
        }

        Member Read()
        {
            int no;
            if (!int.TryParse(tNo.Text.Trim(), out no))
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
            var m = Read(); if (m == null) return;
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

        // RFID 카드 발급
        void IssueCard()
        {
            int no;
            if (!int.TryParse(tNo.Text.Trim(), out no) || !MemberDAO.Exists(no))
            { MessageBox.Show("저장된 회원을 먼저 선택하세요."); return; }
            string card = MemberDAO.IssueCard(no);
            MessageBox.Show("RFID 카드 발급 완료\n카드번호: " + card);
        }
    }
}
```

### 6-7. RentalForm.cs — 대여 관리

이 폼이 제일 크다. 왼쪽 탭에서 회원을 찾고(코드/이름/카드), 오른쪽에 회원 정보를 보여 준 뒤, 아래에서 도서를 대여하고 반납한다. 동명이인이 나오면 "회원 선택" 탭에서 고른다.

```csharp
using System;
using System.Data;
using System.Windows.Forms;

namespace BookRentalSystem
{
    public class RentalForm : Form
    {
        TabControl tab;
        TextBox tInName, tInCode, tInPhone, tInMobile;                      // 회원 입력 탭
        DataGridView gridSelect;                                            // 회원 선택 탭(동명이인)
        TextBox iCode, iName, iJumin, iGrade, iPhone, iMobile, iZip, iAddr; // 회원 정보 표시
        TextBox tBCode, tBTitle, tRentFee, tOverdue, tDue;
        DateTimePicker dpRent;
        Label lblTotCnt, lblTotFee, lblTotOver;
        DataGridView grid;
        int curMember = -1;   // 현재 선택된 회원번호

        public RentalForm()
        {
            Text = "대여 관리"; Width = 980; Height = 700;

            // --- 회원 검색 탭 ---
            tab = new TabControl { Left = 15, Top = 15, Width = 430, Height = 220 };
            var p1 = new TabPage("회원 입력");
            var p2 = new TabPage("회원 선택");
            tab.TabPages.Add(p1); tab.TabPages.Add(p2); Controls.Add(tab);

            UI.Lbl(p1, "회원명", 10, 15);   tInName = UI.Txt(p1, 90, 15);
            UI.Lbl(p1, "회원코드", 10, 50); tInCode = UI.Txt(p1, 90, 50);
            UI.Lbl(p1, "전화번호", 10, 85); tInPhone = UI.Txt(p1, 90, 85);
            UI.Lbl(p1, "휴대폰", 10, 120);  tInMobile = UI.Txt(p1, 90, 120);
            UI.Btn(p1, "카드 읽기", 30, 155, (s, e) => ReadCard(), 90);
            UI.Btn(p1, "찾기", 140, 155, (s, e) => Find());

            gridSelect = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true,
                AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            gridSelect.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                int no = Convert.ToInt32(gridSelect.Rows[e.RowIndex].Cells["코드"].Value);
                ShowMember(MemberDAO.GetByNo(no));
            };
            p2.Controls.Add(gridSelect);

            // --- 회원 정보 표시 ---
            int bx = 470;
            UI.Lbl(this, "회원코드", bx, 20);   iCode = UI.Txt(this, bx + 80, 20, 140, true);
            UI.Lbl(this, "회원명", bx + 240, 20); iName = UI.Txt(this, bx + 320, 20, 140, true);
            UI.Lbl(this, "주민번호", bx, 55);    iJumin = UI.Txt(this, bx + 80, 55, 140, true);
            UI.Lbl(this, "회원등급", bx + 240, 55); iGrade = UI.Txt(this, bx + 320, 55, 140, true);
            UI.Lbl(this, "전화번호", bx, 90);    iPhone = UI.Txt(this, bx + 80, 90, 140, true);
            UI.Lbl(this, "휴대폰", bx + 240, 90); iMobile = UI.Txt(this, bx + 320, 90, 140, true);
            UI.Lbl(this, "우편번호", bx, 125);   iZip = UI.Txt(this, bx + 80, 125, 140, true);
            UI.Lbl(this, "주소", bx, 160);       iAddr = UI.Txt(this, bx + 80, 160, 380, true);

            // --- 대여 도서 입력 ---
            UI.Lbl(this, "도서 코드", 15, 260, 90); tBCode = UI.Txt(this, 110, 260, 120);
            UI.Lbl(this, "도서 제목", 250, 260, 90); tBTitle = UI.Txt(this, 345, 260, 300, true);
            UI.Btn(this, "등록", 670, 258, (s, e) => RegisterRental());

            UI.Lbl(this, "대여료", 15, 300, 60); tRentFee = UI.Txt(this, 80, 300, 90, true);
            UI.Lbl(this, "연체료", 190, 300, 60); tOverdue = UI.Txt(this, 255, 300, 90, true);
            UI.Lbl(this, "대여일", 360, 300, 50);
            dpRent = new DateTimePicker { Left = 415, Top = 300, Width = 130, Format = DateTimePickerFormat.Short };
            Controls.Add(dpRent);
            UI.Lbl(this, "반납 예정일", 560, 300, 80); tDue = UI.Txt(this, 645, 300, 130, true);

            UI.Lbl(this, "총 대여 권수", 15, 340, 90);
            lblTotCnt = new Label { Left = 110, Top = 344, Width = 60, Text = "0권" }; Controls.Add(lblTotCnt);
            UI.Lbl(this, "총 대여료", 200, 340, 70);
            lblTotFee = new Label { Left = 275, Top = 344, Width = 90, Text = "0원" }; Controls.Add(lblTotFee);
            UI.Lbl(this, "총 연체료", 400, 340, 70);
            lblTotOver = new Label { Left = 475, Top = 344, Width = 90, Text = "0원" }; Controls.Add(lblTotOver);
            UI.Btn(this, "도서 반납", 670, 338, (s, e) => ReturnBook(), 100);

            grid = new DataGridView { Left = 15, Top = 380, Width = 940, Height = 270,
                ReadOnly = true, AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            Controls.Add(grid);

            // 바코드 스캐너 입력 후 Enter → 바로 등록
            tBCode.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) RegisterRental(); };
        }

        // 카드(RFID) 읽기 → 카드번호로 회원 조회
        void ReadCard()
        {
            string card = UI.Prompt("카드 읽기", "RFID 카드번호를 입력(스캔)하세요.");
            if (string.IsNullOrEmpty(card)) return;
            var m = MemberDAO.GetByCard(card);
            if (m == null) MessageBox.Show("해당 카드 회원이 없습니다.");
            else ShowMember(m);
        }

        // 코드 우선, 없으면 이름으로 검색. 동명이인이면 선택 탭으로
        void Find()
        {
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
            if (tInName.Text.Trim() == "") { MessageBox.Show("회원명 또는 코드를 입력하세요."); return; }

            var dt = MemberDAO.GetByName(tInName.Text.Trim());
            if (dt.Rows.Count == 0) { MessageBox.Show("회원이 없습니다."); return; }
            if (dt.Rows.Count == 1)
            {
                ShowMember(MemberDAO.GetByNo(Convert.ToInt32(dt.Rows[0]["코드"])));
            }
            else
            {
                gridSelect.DataSource = dt;
                tab.SelectedIndex = 1;
                MessageBox.Show("동명이인이 있습니다. '회원 선택' 탭에서 더블클릭하세요.");
            }
        }

        void ShowMember(Member m)
        {
            if (m == null) return;
            curMember = m.MemberNo;
            iCode.Text = m.MemberNo.ToString(); iName.Text = m.Name; iJumin.Text = m.Jumin;
            iGrade.Text = m.Grade; iPhone.Text = m.Phone; iMobile.Text = m.Mobile;
            iZip.Text = m.ZipCode; iAddr.Text = m.Address;
            LoadRentals();
        }

        // 도서 대여 등록 (신간/구간 자동 판정 + 요금 자동 적용)
        void RegisterRental()
        {
            if (curMember < 0) { MessageBox.Show("회원을 먼저 선택하세요."); return; }
            string code = tBCode.Text.Trim();
            if (code == "") return;

            var b = BookDAO.GetByCode(code);
            if (b == null) { MessageBox.Show("도서가 없습니다."); return; }
            if (RentalDAO.IsActive(code)) { MessageBox.Show("이미 대여 중인 도서입니다."); return; }

            var s = SettingDAO.Get();
            bool isNew = (DateTime.Today - b.PublishDate).TotalDays <= s.SwitchPeriod;
            int days = isNew ? s.NewRentDays : s.OldRentDays;
            int fee  = isNew ? s.NewRentFee  : s.OldRentFee;
            int rate = isNew ? s.NewOverdueFee : s.OldOverdueFee;
            DateTime rent = dpRent.Value.Date;
            DateTime due = rent.AddDays(days);

            RentalDAO.Rent(curMember, code, rent, due, fee, rate);

            tBTitle.Text = b.Title;
            tRentFee.Text = fee + "원";
            tOverdue.Text = rate + "원/일";
            tDue.Text = due.ToString("yyyy-MM-dd");
            tBCode.Text = "";
            LoadRentals();
            tBCode.Focus();
        }

        // 현재 회원이 대여중인 목록 + 합계
        void LoadRentals()
        {
            var raw = RentalDAO.GetActiveRaw(curMember);
            var dt = new DataTable();
            dt.Columns.Add("번호"); dt.Columns.Add("도서코드"); dt.Columns.Add("제목");
            dt.Columns.Add("대여일"); dt.Columns.Add("반납예정일");
            dt.Columns.Add("대여료"); dt.Columns.Add("연체료(예상)"); dt.Columns.Add("상태");

            int totFee = 0, totOver = 0;
            foreach (DataRow r in raw.Rows)
            {
                DateTime due = Convert.ToDateTime(r["DueDate"]);
                int rate = Convert.ToInt32(r["OverdueRate"]);
                int fee = Convert.ToInt32(r["RentFee"]);
                int late = Math.Max(0, (DateTime.Today - due).Days);
                int over = late * rate;
                dt.Rows.Add(r["RentalId"], r["BookCode"], r["Title"],
                    Convert.ToDateTime(r["RentDate"]).ToString("yyyy-MM-dd"),
                    due.ToString("yyyy-MM-dd"), fee, over, late > 0 ? "연체" : "대여중");
                totFee += fee; totOver += over;
            }
            grid.DataSource = dt;
            lblTotCnt.Text = dt.Rows.Count + "권";
            lblTotFee.Text = totFee + "원";
            lblTotOver.Text = totOver + "원";
        }

        // 선택한 도서 반납
        void ReturnBook()
        {
            if (grid.CurrentRow == null) { MessageBox.Show("반납할 도서를 선택하세요."); return; }
            int id = Convert.ToInt32(grid.CurrentRow.Cells["번호"].Value);
            RentalDAO.Return(id, DateTime.Today);
            LoadRentals();
            MessageBox.Show("반납 처리되었습니다.");
        }
    }
}
```

신간/구간 판정은 `(오늘 − 출판일) ≤ 전환 기간`이면 신간으로 본다. 환경설정의 전환 기간(기본 14일)을 바꾸면 판정 기준이 바뀐다.

### 6-8. QueryForm.cs — 정보 조회

라디오 버튼으로 "도서 대여 순위 / 대여중인 도서 / 회원 대여 순위" 중 하나를 골라 검색한다. 순위는 `GROUP BY ... COUNT(*)`로 뽑는다.

```csharp
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BookRentalSystem
{
    public class QueryForm : Form
    {
        RadioButton rbBookRank, rbActive, rbMemberRank;
        ComboBox cbCategory, cbGrade;
        DataGridView grid;

        public QueryForm()
        {
            Text = "도서 조회 관리"; Width = 900; Height = 640;

            rbBookRank   = new RadioButton { Text = "도서 대여 순위", Left = 20, Top = 20, Width = 140, Checked = true };
            rbActive     = new RadioButton { Text = "대여중인 도서", Left = 180, Top = 20, Width = 140 };
            rbMemberRank = new RadioButton { Text = "회원 대여 순위", Left = 20, Top = 50, Width = 140 };
            Controls.AddRange(new Control[] { rbBookRank, rbActive, rbMemberRank });

            UI.Lbl(this, "분류", 360, 20, 50);
            cbCategory = new ComboBox { Left = 415, Top = 20, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cbCategory.Items.Add("전체");
            foreach (System.Data.DataRow r in BookDAO.Categories().Rows) cbCategory.Items.Add(r[0].ToString());
            cbCategory.SelectedIndex = 0; Controls.Add(cbCategory);

            UI.Lbl(this, "회원 등급", 360, 55, 60);
            cbGrade = new ComboBox { Left = 415, Top = 55, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cbGrade.Items.Add("전체");
            foreach (System.Data.DataRow r in MemberDAO.Grades().Rows) cbGrade.Items.Add(r[0].ToString());
            cbGrade.SelectedIndex = 0; Controls.Add(cbGrade);

            UI.Btn(this, "검색", 600, 30, (s, e) => Search());
            UI.Btn(this, "나가기", 700, 30, (s, e) => Close());

            grid = new DataGridView { Left = 20, Top = 100, Width = 850, Height = 480,
                ReadOnly = true, AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            Controls.Add(grid);
        }

        void Search()
        {
            var ps = new List<SqlParameter>();
            string sql;

            if (rbBookRank.Checked)   // 도서 대여 순위
            {
                sql = @"SELECT COUNT(*) AS 대여횟수, b.BookCode AS 코드, b.Title AS 제목,
                               b.Category AS 분류, b.Author AS 저자
                        FROM Rental r JOIN Book b ON r.BookCode=b.BookCode";
                if (cbCategory.Text != "전체") { sql += " WHERE b.Category=@c"; ps.Add(new SqlParameter("@c", cbCategory.Text)); }
                sql += " GROUP BY b.BookCode,b.Title,b.Category,b.Author ORDER BY 대여횟수 DESC";
            }
            else if (rbMemberRank.Checked)  // 회원 대여 순위
            {
                sql = @"SELECT COUNT(*) AS 대여횟수, m.Name AS [회원 이름], m.Grade AS 등급,
                               m.Gender AS 성별, m.Phone AS 연락처, m.Mobile AS 휴대폰, m.Address AS 주소
                        FROM Rental r JOIN Member m ON r.MemberNo=m.MemberNo";
                if (cbGrade.Text != "전체") { sql += " WHERE m.Grade=@gr"; ps.Add(new SqlParameter("@gr", cbGrade.Text)); }
                sql += " GROUP BY m.Name,m.Grade,m.Gender,m.Phone,m.Mobile,m.Address ORDER BY 대여횟수 DESC";
            }
            else  // 대여중인 도서
            {
                sql = @"SELECT b.BookCode AS 코드, b.Title AS 제목, b.Category AS 분류,
                               m.Name AS 회원명, m.Grade AS 등급,
                               CONVERT(varchar(10),r.RentDate,23) AS 대여일,
                               CONVERT(varchar(10),r.DueDate,23) AS 반납예정일
                        FROM Rental r JOIN Book b ON r.BookCode=b.BookCode
                                      JOIN Member m ON r.MemberNo=m.MemberNo
                        WHERE r.IsReturned=0";
                if (cbCategory.Text != "전체") { sql += " AND b.Category=@c"; ps.Add(new SqlParameter("@c", cbCategory.Text)); }
                if (cbGrade.Text != "전체") { sql += " AND m.Grade=@gr"; ps.Add(new SqlParameter("@gr", cbGrade.Text)); }
                sql += " ORDER BY r.DueDate";
            }

            grid.DataSource = DBManager.Query(sql, ps.ToArray());
        }
    }
}
```

### 6-9. SettingForm.cs — 환경 설정

신간·구간 요금을 수정·저장하고, 도서 목록과 대여 현황을 엑셀에서 열리는 CSV로 내보낸다.

```csharp
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace BookRentalSystem
{
    public class SettingForm : Form
    {
        TextBox tSwitch, tNewDays, tNewFee, tNewOver, tOldDays, tOldFee, tOldOver;

        public SettingForm()
        {
            Text = "환경 설정 / 도서 대여 설정"; Width = 620; Height = 480;

            var gNew = new GroupBox { Text = "신간", Left = 20, Top = 20, Width = 560, Height = 110 };
            Controls.Add(gNew);
            UI.Lbl(gNew, "전환 기간(일)", 15, 25, 90); tSwitch = UI.Txt(gNew, 110, 25, 60);
            UI.Lbl(gNew, "대여료", 300, 25, 50);       tNewFee = UI.Txt(gNew, 355, 25, 80);
            UI.Lbl(gNew, "대여 기간(일)", 15, 60, 90); tNewDays = UI.Txt(gNew, 110, 60, 60);
            UI.Lbl(gNew, "연체료(/일)", 300, 60, 70);  tNewOver = UI.Txt(gNew, 375, 60, 80);

            var gOld = new GroupBox { Text = "구간", Left = 20, Top = 140, Width = 560, Height = 110 };
            Controls.Add(gOld);
            UI.Lbl(gOld, "대여 기간(일)", 15, 25, 90); tOldDays = UI.Txt(gOld, 110, 25, 60);
            UI.Lbl(gOld, "대여료", 300, 25, 50);       tOldFee = UI.Txt(gOld, 355, 25, 80);
            UI.Lbl(gOld, "연체료(/일)", 300, 60, 70);  tOldOver = UI.Txt(gOld, 375, 60, 80);

            UI.Btn(this, "수정 저장", 20, 265, (s, e) => SaveSetting(), 90);
            UI.Btn(this, "나가기", 490, 265, (s, e) => Close());

            var gExcel = new GroupBox { Text = "엑셀로 출력", Left = 20, Top = 310, Width = 560, Height = 100 };
            Controls.Add(gExcel);
            UI.Btn(gExcel, "도서 목록", 30, 35, (s, e) => ExportBookList(), 120);
            UI.Btn(gExcel, "대여 현황", 180, 35, (s, e) => ExportRentalStatus(), 120);

            LoadSetting();
        }

        void LoadSetting()
        {
            var s = SettingDAO.Get();
            tSwitch.Text = s.SwitchPeriod.ToString();
            tNewDays.Text = s.NewRentDays.ToString(); tNewFee.Text = s.NewRentFee.ToString();
            tNewOver.Text = s.NewOverdueFee.ToString();
            tOldDays.Text = s.OldRentDays.ToString(); tOldFee.Text = s.OldRentFee.ToString();
            tOldOver.Text = s.OldOverdueFee.ToString();
        }

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

        void ExportBookList() => ToCsv(BookDAO.GetAll(), "도서목록");

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

        // 외부 라이브러리 없이 CSV(UTF-8 BOM)로 저장 → 엑셀에서 바로 열림.
        // 진짜 .xlsx가 필요하면 ClosedXML NuGet으로 교체.
        void ToCsv(DataTable dt, string name)
        {
            using (var sfd = new SaveFileDialog { Filter = "CSV 파일|*.csv", FileName = name + ".csv" })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;
                var sb = new StringBuilder();
                for (int i = 0; i < dt.Columns.Count; i++)
                    sb.Append(dt.Columns[i].ColumnName + (i < dt.Columns.Count - 1 ? "," : "\r\n"));
                foreach (DataRow r in dt.Rows)
                    for (int i = 0; i < dt.Columns.Count; i++)
                        sb.Append("\"" + r[i].ToString().Replace("\"", "\"\"") + "\""
                                  + (i < dt.Columns.Count - 1 ? "," : "\r\n"));
                File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(true));
                MessageBox.Show("저장 완료: " + sfd.FileName);
            }
        }
    }
}
```

CSV는 외부 라이브러리 없이 바로 엑셀에서 열린다. `UTF8Encoding(true)`로 BOM을 넣어야 한글이 안 깨진다. 진짜 `.xlsx` 형식이 필요하면 NuGet에서 **ClosedXML**을 받아 `ToCsv` 부분만 교체하면 된다.

### 6-10. Program.cs — 진입점

```csharp
using System;
using System.Windows.Forms;

namespace BookRentalSystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
```

---

## 7. 실행 순서

1. SSMS에서 **5장 스크립트 실행** (DB·테이블·샘플 생성).
2. `DB.cs`의 **`ConnectionString`을 본인 환경에 맞게 수정**.
3. F5로 실행 → 메인 창이 뜬다.
4. **도서 관리 → 도서 정보**에서 도서 몇 권 등록.
5. **회원 관리 → 회원 정보**에서 회원 등록, 필요하면 카드 발급.
6. **도서 대여/반납 → 대여 관리**에서 회원을 찾고(코드/이름/카드) 도서 코드 입력 → 등록.
7. **정보 조회**에서 순위·대여중 목록 확인.
8. **환경설정**에서 요금을 바꿔 보고, 목록/현황을 CSV로 출력.

데이터가 흐르는 한 사이클을 직접 돌려 보면 계층 구조가 한눈에 들어온다.

---

## 8. 자주 막히는 곳 (트러블슈팅)

| 증상 | 원인 / 해결 |
|------|-------------|
| 연결 오류 (`A network-related...`) | `ConnectionString`의 `Server=` 가 실제 인스턴스와 다름. LocalDB면 `(localdb)\MSSQLLocalDB`, Express면 `.\SQLEXPRESS`. |
| `Login failed for user` | 윈도우 인증이면 `Integrated Security=True`, SQL 계정이면 `User Id=...;Password=...`. |
| 한글이 네모/물음표 | 폼 `Font`를 맑은 고딕으로. DB 컬럼은 `NVARCHAR`인지 확인(이미 그렇게 설계함). |
| 도서/회원 삭제 시 오류 | `Rental`이 외래키로 참조 중. 대여 이력이 있으면 삭제가 막힌다(정상 동작). |
| `Microsoft.Data.SqlClient` 없음 | .NET 6+로 만든 경우. NuGet 설치 후 `using` 변경. .NET Framework면 불필요. |
| 그리드 컬럼명 못 찾음 | DAO의 `AS 별칭`과 폼의 `Cells["코드"]` 같은 이름이 일치해야 한다. |

---

## 9. 더 해 볼 것 (확장 과제)

- 신간/구간 판정 기준을 **출판일이 아니라 입고일**로 바꾸기 → `Book`에 `StockDate` 컬럼 추가 후 그걸로 판정.
- 회원 등급별로 대여 가능 권수에 제한 두기 (예: 일반 3권, 학생 5권).
- CSV 대신 **ClosedXML로 실제 .xlsx 출력**.
- 대여/반납을 **트랜잭션**으로 묶어 중간 실패 시 롤백 처리.
- 연체료 합계를 회원별로 집계하는 조회 추가.

비디오 버전과 비교해 보면, 바뀐 건 테이블·컬럼·라벨 같은 도메인 어휘뿐이고 `Query`/`Execute`/`Scalar` 세 패턴과 계층 구조는 그대로다. 도메인이 달라져도 같은 골격으로 옮겨 간다는 걸 학생들이 두 예제로 직접 확인하게 만드는 구성이다.
