# 도서 대여 관리 시스템 만들기 (C# WinForms + ADO.NET)

> 이 문서는 기존 초안의 코드 원문을 유지하면서, 학생이 그대로 따라 만들 수 있도록 **파일별 역할, 코드 흐름, 핵심 문법, DB 동작 원리, 자주 나는 오류**를 보강한 상세 해설판이다.
>
> 기준 구조는 `C# WinForms + ADO.NET + SQL Server`이다. ORM(Entity Framework 등)은 쓰지 않고, `SqlConnection`, `SqlCommand`, `SqlDataAdapter`, `SqlParameter`를 직접 사용한다.

---

## 0. 전체 구조를 먼저 이해하기

이 프로그램은 단순히 화면만 만드는 예제가 아니라, 실제 업무용 프로그램에서 자주 쓰는 **3계층 구조**를 작게 만든 것이다.

```text
사용자
  ↓
WinForms 화면(Form)
  ↓
DAO(Data Access Object)
  ↓
DBManager(ADO.NET 공통 실행기)
  ↓
SQL Server Database
```

### 왜 이렇게 나누는가?

| 구분 | 파일 | 책임 |
|---|---|---|
| 화면 계층 | `BookForm.cs`, `MemberForm.cs`, `RentalForm.cs`, `QueryForm.cs`, `SettingForm.cs` | 버튼 클릭, 입력값 읽기, 그리드 표시 |
| DB 접근 계층 | `DAO.cs` | SQL문 작성, INSERT/UPDATE/DELETE/SELECT 실행 요청 |
| DB 공통 실행 계층 | `DB.cs` | SQL Server 연결, 명령 실행, 결과 반환 |
| 데이터 모델 | `Models.cs` | 도서, 회원, 설정값을 C# 객체로 표현 |
| 프로그램 시작 | `Program.cs` | WinForms 앱 시작 |
| 메인 화면 | `MainForm.cs` | MDI 메뉴 구성, 자식 창 열기 |

### 이 예제에서 반드시 익혀야 하는 핵심

1. **CRUD**
   - Create: 등록
   - Read: 조회
   - Update: 수정
   - Delete: 삭제

2. **ADO.NET**
   - `SqlConnection`: SQL Server 연결
   - `SqlCommand`: SQL문 실행 준비
   - `SqlDataAdapter`: SELECT 결과를 `DataTable`에 채움
   - `SqlParameter`: SQL 인젝션을 막기 위한 안전한 값 전달

3. **WinForms**
   - `Form`: 화면 창
   - `TextBox`: 입력칸
   - `Button`: 버튼
   - `DataGridView`: 표 형태 목록
   - `ComboBox`: 선택 목록
   - `DateTimePicker`: 날짜 선택

4. **DB 관계**
   - `Book`: 도서 기본 정보
   - `Member`: 회원 기본 정보
   - `Rental`: 대여 내역
   - `RentalSetting`: 대여료/대여기간/연체료 설정

---

## 0-1. 사용 기술을 선택한 이유

| 기술 | 사용 이유 | 이 실습에서 배우는 것 |
|---|---|---|
| C# | Windows 업무용 프로그램 제작에 많이 쓰임 | 이벤트 기반 프로그래밍, 객체 사용 |
| WinForms | 화면을 빠르게 만들 수 있고 학습 난도가 낮음 | 폼, 버튼, 그리드, 메뉴 |
| ADO.NET | DB 연결 원리를 직접 이해하기 좋음 | 연결, 명령, 파라미터, 결과표 |
| SQL Server | C#과 궁합이 좋고 교육/업무 현장에서 흔함 | 테이블, 외래키, IDENTITY, SELECT |
| MDI | 하나의 메인 창 안에서 여러 업무 화면 관리 가능 | 업무용 데스크톱 프로그램 구조 |
| CSV 출력 | 별도 라이브러리 없이 엑셀에서 열 수 있음 | 파일 저장, UTF-8 BOM, 데이터 내보내기 |

---

## 0-2. 실습 전 준비 체크리스트

| 항목 | 확인 방법 |
|---|---|
| Visual Studio 설치 | Windows 시작 메뉴에서 Visual Studio 실행 |
| Windows Forms App(.NET Framework) 선택 가능 | 새 프로젝트 만들기에서 확인 |
| SQL Server 설치 | SSMS에서 서버 접속 가능해야 함 |
| SSMS 설치 | SQL 스크립트 실행에 필요 |
| 프로젝트 이름 | `BookRentalSystem` 권장 |
| DB 이름 | `BookRentalDB`로 통일 |
| 연결 문자열 | `DB.cs`의 `ConnectionString` 수정 |

---

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


### 5-0. SQL 스크립트 실행 위치

아래 SQL은 **Visual Studio가 아니라 SSMS(SQL Server Management Studio)** 에서 실행한다.

실행 순서:

1. SSMS 실행
2. SQL Server에 접속
3. 새 쿼리 창 열기
4. 아래 스크립트 전체 붙여넣기
5. `실행` 버튼 클릭 또는 `F5`

### 5-1. 테이블 관계 구조

```text
Member(회원)
   1
   │
   └── N Rental(대여내역) N ─── 1 Book(도서)

RentalSetting(환경설정)
   └── 대여료/대여기간/연체료 기준값 1행 저장
```

`Rental` 테이블은 회원과 도서를 연결하는 **중간 이력 테이블**이다.  
회원 한 명은 여러 번 대여할 수 있고, 도서 한 권도 시간이 지나면 여러 회원에게 대여될 수 있다.

### 5-2. SQL 핵심 문법 설명

| SQL 문법 | 의미 |
|---|---|
| `CREATE DATABASE` | 새 데이터베이스 생성 |
| `CREATE TABLE` | 새 테이블 생성 |
| `PRIMARY KEY` | 행을 구분하는 기본키 |
| `FOREIGN KEY` | 다른 테이블의 기본키를 참조하는 외래키 |
| `IDENTITY(1,1)` | 1부터 시작해서 1씩 자동 증가 |
| `NVARCHAR` | 한글 저장 가능한 문자열 |
| `VARCHAR` | 영문/숫자 중심 문자열 |
| `DATE` | 날짜 저장 |
| `BIT` | 참/거짓 저장. SQL Server에서는 보통 0 또는 1 |
| `DEFAULT` | 값을 안 넣었을 때 기본값 |
| `GO` | SSMS에서 SQL 실행 단위를 나누는 구분자 |

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


#### 이 파일의 목적

`DB.cs`는 두 가지 역할을 한다.

1. **DBManager**
   - SQL Server에 연결한다.
   - SELECT, INSERT, UPDATE, DELETE 실행을 공통 메서드로 처리한다.
   - 다른 파일들이 DB 연결 코드를 반복해서 쓰지 않게 한다.

2. **UI**
   - `Label`, `TextBox`, `Button` 같은 WinForms 컨트롤을 코드로 쉽게 만들게 도와준다.
   - 디자이너 없이 실습하기 위해 만든 보조 클래스다.

#### 핵심 메서드

| 메서드 | 사용 시점 | 반환값 |
|---|---|---|
| `Query()` | SELECT 결과가 여러 행/여러 열일 때 | `DataTable` |
| `Execute()` | INSERT, UPDATE, DELETE 실행할 때 | 영향받은 행 수 |
| `Scalar()` | COUNT, MAX처럼 값 하나만 필요할 때 | `object` |

#### 코드 흐름

```text
Form에서 DAO 메서드 호출
  ↓
DAO에서 SQL문과 SqlParameter 준비
  ↓
DBManager.Query / Execute / Scalar 호출
  ↓
SqlConnection으로 DB 연결
  ↓
SqlCommand로 SQL 실행
  ↓
결과 반환
```

#### 특히 중요한 부분

`ConnectionString`은 본인 PC의 SQL Server 환경에 맞게 반드시 수정해야 한다.

예시:

```csharp
// LocalDB 사용 시
Server=(localdb)\MSSQLLocalDB;Database=BookRentalDB;Integrated Security=True;

// SQL Server Express 사용 시
Server=.\SQLEXPRESS;Database=BookRentalDB;Integrated Security=True;

// SQL 계정 로그인 사용 시
Server=localhost;Database=BookRentalDB;User Id=sa;Password=비밀번호;
```

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


#### 이 파일의 목적

`Models.cs`는 DB 데이터를 C# 객체로 담기 위한 파일이다.  
DB의 한 행(row)을 C# 클래스 하나로 옮겨 담는다고 보면 된다.

| 클래스 | 대응 테이블 | 의미 |
|---|---|---|
| `Book` | `Book` | 도서 한 권의 정보 |
| `Member` | `Member` | 회원 한 명의 정보 |
| `Setting` | `RentalSetting` | 대여료/연체료 설정값 |

#### 왜 모델 클래스를 쓰는가?

폼에서 `TextBox` 값을 바로 SQL로 보내면 코드가 지저분해진다.  
그래서 먼저 `Book`, `Member`, `Setting` 객체에 값을 담고, DAO에 넘긴다.

```text
TextBox 입력값
  ↓
Book 객체 생성
  ↓
BookDAO.Insert(book)
  ↓
DB 저장
```

#### 이 예제의 단순화 지점

실무에서는 보통 아래처럼 속성(property)을 사용한다.

```csharp
public string BookCode { get; set; }
```

이 예제에서는 학생들이 구조를 쉽게 보기 위해 필드(field)를 단순하게 사용했다.

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


#### 이 파일의 목적

`DAO.cs`는 SQL문을 모아 두는 파일이다.  
폼 코드 안에 SQL을 직접 쓰지 않고, DAO가 DB 작업을 전담한다.

DAO는 **Data Access Object**의 약자다.  
한국어로는 보통 **데이터 접근 객체**라고 부른다.

#### DAO를 쓰는 이유

| DAO를 쓰지 않는 경우 | DAO를 쓰는 경우 |
|---|---|
| 폼마다 SQL이 흩어짐 | SQL이 한 파일에 모임 |
| 수정할 곳을 찾기 어려움 | DB 로직 수정이 쉬움 |
| 화면 코드가 복잡해짐 | 화면 코드는 버튼/입력 처리에 집중 |
| 중복 코드 증가 | 재사용 가능 |

#### 이 파일의 클래스 구성

| 클래스 | 담당 업무 |
|---|---|
| `BookDAO` | 도서 등록, 수정, 삭제, 조회 |
| `MemberDAO` | 회원 등록, 수정, 삭제, 조회, 카드 발급 |
| `RentalDAO` | 대여 등록, 반납 처리, 대여중 목록 |
| `SettingDAO` | 환경설정 조회/저장 |

#### 핵심 패턴

```text
폼에서 객체 생성
  ↓
DAO 메서드 호출
  ↓
DAO가 SQL과 파라미터 구성
  ↓
DBManager가 실제 실행
```

#### `SqlParameter`를 쓰는 이유

아래처럼 문자열을 붙여 SQL을 만들면 위험하다.

```csharp
"SELECT * FROM Member WHERE Name='" + name + "'"
```

사용자가 이상한 SQL 조각을 입력하면 DB가 공격당할 수 있다.  
그래서 이 예제는 아래처럼 파라미터를 사용한다.

```csharp
"SELECT * FROM Member WHERE Name=@nm"
new SqlParameter("@nm", name)
```

이 방식이 ADO.NET에서 기본적으로 권장되는 안전한 방식이다.

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


#### 이 파일의 목적

`MainForm.cs`는 프로그램의 메인 창이다.  
메뉴를 만들고, 사용자가 메뉴를 클릭하면 각 기능별 자식 창을 연다.

#### MDI란?

MDI는 **Multiple Document Interface**의 약자다.  
하나의 부모 창 안에 여러 자식 창을 띄우는 방식이다.

```text
MainForm
 ├─ BookForm
 ├─ MemberForm
 ├─ RentalForm
 ├─ QueryForm
 └─ SettingForm
```

#### 이 예제에서 MDI를 쓰는 이유

도서 관리, 회원 관리, 대여 관리 화면을 각각 독립된 창으로 만들되, 전체 프로그램은 하나의 메인 창에서 관리하기 위해서다.

#### 핵심 코드

| 코드 | 의미 |
|---|---|
| `IsMdiContainer = true` | 이 폼을 MDI 부모 창으로 설정 |
| `MenuStrip` | 상단 메뉴바 생성 |
| `ToolStripMenuItem` | 메뉴 항목 생성 |
| `f.MdiParent = this` | 새 폼을 메인 폼의 자식 창으로 설정 |
| `MdiChildren` | 현재 열려 있는 자식 창 목록 |

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


#### 이 파일의 목적

`BookForm.cs`는 도서를 등록, 수정, 삭제, 조회하는 화면이다.

#### 화면 기능

| 기능 | 설명 |
|---|---|
| 추가 | 입력칸을 비우고 새 도서 입력 준비 |
| 저장 | 도서코드가 기존에 있으면 수정, 없으면 신규 등록 |
| 삭제 | 선택한 도서를 삭제 |
| 목록 조회 | 전체 도서를 `DataGridView`에 표시 |
| 행 클릭 | 그리드에서 선택한 도서 정보를 입력칸에 표시 |

#### 데이터 흐름

```text
사용자가 도서 정보 입력
  ↓
Save() 실행
  ↓
Book 객체 생성
  ↓
BookDAO.Exists()로 기존 도서인지 확인
  ↓
기존이면 Update()
없으면 Insert()
  ↓
Load()로 목록 새로고침
```

#### 이 폼에서 중요한 개념

| 코드/개념 | 설명 |
|---|---|
| `DataGridView` | DB 조회 결과를 표로 보여 주는 컨트롤 |
| `DateTimePicker` | 출판일을 날짜 형식으로 입력받는 컨트롤 |
| `BookDAO.Exists()` | 저장 시 INSERT/UPDATE 판단에 사용 |
| `LoadRow()` | 그리드에서 선택한 행을 입력칸에 다시 채움 |
| `try-catch` | DB 오류 발생 시 프로그램이 바로 종료되지 않게 처리 |

#### 삭제가 실패할 수 있는 이유

도서가 `Rental` 테이블에서 이미 대여 이력으로 참조되고 있으면 삭제가 막힌다.  
이것은 오류가 아니라 **외래키 제약조건이 정상적으로 작동하는 것**이다.

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


#### 이 파일의 목적

`MemberForm.cs`는 회원을 등록, 수정, 삭제하고 RFID 카드번호를 발급하는 화면이다.

#### 화면 기능

| 기능 | 설명 |
|---|---|
| 추가 | 입력칸 초기화 |
| 저장 | 회원번호 기준으로 신규 등록 또는 수정 |
| 삭제 | 회원 삭제 |
| 카드 관리 | RFID 카드번호 생성 후 회원에 저장 |
| 목록 | 전체 회원 목록 표시 |

#### 데이터 흐름

```text
사용자 입력
  ↓
Read()에서 Member 객체 생성
  ↓
MemberDAO.Exists()로 기존 회원 여부 확인
  ↓
Insert 또는 Update
  ↓
목록 새로고침
```

#### `Read()` 메서드의 역할

`TextBox`, `ComboBox`에 흩어져 있는 입력값을 하나의 `Member` 객체로 모은다.  
회원번호는 반드시 숫자여야 하므로 `int.TryParse()`로 검사한다.

#### RFID 카드 발급 방식

실제 RFID 장치를 연결한 것은 아니다.  
이 예제에서는 아래 형식의 문자열을 만들어 DB에 저장한다.

```text
RFID-회원번호-랜덤4자리
```

예시:

```text
RFID-101-4821
```

이후 대여 화면에서 이 카드번호를 입력하면 회원을 찾을 수 있다.

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


#### 이 파일의 목적

`RentalForm.cs`는 이 프로그램의 핵심 화면이다.  
회원 검색, 도서 대여, 반납, 연체료 계산이 이 파일에서 처리된다.

#### 주요 기능

| 기능 | 설명 |
|---|---|
| 회원 검색 | 회원번호 또는 이름으로 회원 조회 |
| RFID 조회 | 카드번호로 회원 조회 |
| 동명이인 처리 | 같은 이름이 여러 명이면 선택 탭에서 더블클릭 |
| 도서 대여 | 도서코드 입력 후 대여 등록 |
| 신간/구간 판정 | 출판일과 설정값을 기준으로 자동 판정 |
| 반납 처리 | 선택한 대여 건을 반납 처리 |
| 연체료 계산 | 반납 예정일을 넘긴 날짜 × 1일 연체료 |

#### 전체 업무 흐름

```text
회원 선택
  ↓
도서 코드 입력
  ↓
도서 존재 여부 확인
  ↓
이미 대여중인지 확인
  ↓
환경설정 조회
  ↓
신간/구간 자동 판정
  ↓
대여료/대여기간/연체단가 결정
  ↓
Rental 테이블에 대여 내역 저장
  ↓
현재 대여 목록 새로고침
```

#### 신간/구간 판정 로직

```csharp
bool isNew = (DateTime.Today - b.PublishDate).TotalDays <= s.SwitchPeriod;
```

의미:

```text
오늘 날짜 - 출판일 <= 전환 기간
```

기본 설정이 14일이면, 출판 후 14일 이내 도서는 신간이다.

#### 연체료 계산 로직

현재 대여 목록에서는 예상 연체료를 계산한다.

```text
오늘 날짜 - 반납 예정일 = 연체일수
연체일수 × 연체단가 = 예상 연체료
```

반납 버튼을 누르면 `RentalDAO.Return()`에서 실제 연체료가 DB에 확정 저장된다.

#### 바코드 스캐너와 연결되는 부분

바코드 스캐너는 대부분 키보드처럼 동작한다.  
즉, 스캔하면 `TextBox`에 코드가 입력되고 마지막에 Enter가 들어온다.

이 코드가 그 역할을 한다.

```csharp
tBCode.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) RegisterRental(); };
```

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


#### 이 파일의 목적

`QueryForm.cs`는 저장된 대여 데이터를 조회하고 분석하는 화면이다.

#### 조회 종류

| 조회 | SQL 핵심 |
|---|---|
| 도서 대여 순위 | `GROUP BY BookCode`, `COUNT(*)` |
| 회원 대여 순위 | `GROUP BY Member`, `COUNT(*)` |
| 대여중인 도서 | `WHERE IsReturned=0` |

#### 왜 `GROUP BY`를 쓰는가?

대여 순위는 여러 대여 이력을 도서별 또는 회원별로 묶어서 세어야 한다.  
이때 사용하는 SQL 문법이 `GROUP BY`다.

예:

```sql
SELECT BookCode, COUNT(*)
FROM Rental
GROUP BY BookCode
```

의미:

```text
Rental 테이블을 BookCode별로 묶고,
각 도서가 몇 번 대여되었는지 센다.
```

#### 필터링 구조

분류 또는 회원등급이 `전체`가 아니면 SQL 뒤에 조건을 붙인다.

```text
분류 = 전체 → 조건 없음
분류 = 소설 → WHERE b.Category=@c
```

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


#### 이 파일의 목적

`SettingForm.cs`는 신간/구간 기준과 대여료, 연체료를 설정하는 화면이다.  
또한 도서 목록과 대여 현황을 CSV 파일로 내보낸다.

#### 설정값 의미

| 항목 | 의미 |
|---|---|
| 전환 기간 | 출판 후 며칠까지 신간으로 볼지 |
| 신간 대여 기간 | 신간 도서를 며칠 빌릴 수 있는지 |
| 신간 대여료 | 신간 대여 시 부과되는 금액 |
| 신간 연체료 | 신간 연체 1일당 금액 |
| 구간 대여 기간 | 구간 도서를 며칠 빌릴 수 있는지 |
| 구간 대여료 | 구간 대여 시 부과되는 금액 |
| 구간 연체료 | 구간 연체 1일당 금액 |

#### CSV를 쓰는 이유

이 예제는 외부 라이브러리를 쓰지 않는다.  
그래서 `.xlsx` 대신 Excel에서 바로 열 수 있는 `.csv`를 만든다.

#### 한글 CSV가 깨지지 않게 하는 핵심

```csharp
new UTF8Encoding(true)
```

`true`는 UTF-8 BOM을 붙인다는 뜻이다.  
Excel은 BOM이 있는 UTF-8 CSV를 한글로 비교적 안정적으로 인식한다.

#### 실무에서 `.xlsx`가 꼭 필요하면?

NuGet에서 `ClosedXML`을 설치해서 `.xlsx` 파일로 저장하는 방식으로 바꿀 수 있다.  
다만 이 실습에서는 ADO.NET과 WinForms 흐름이 핵심이므로 CSV로 단순화했다.

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


#### 이 파일의 목적

`Program.cs`는 프로그램이 처음 시작되는 지점이다.  
C# WinForms 프로젝트에서 가장 먼저 실행되는 `Main()` 메서드가 들어 있다.

#### 실행 흐름

```text
Main()
  ↓
Application.EnableVisualStyles()
  ↓
Application.SetCompatibleTextRenderingDefault(false)
  ↓
Application.Run(new MainForm())
  ↓
MainForm 화면 표시
```

#### 핵심 코드 설명

| 코드 | 의미 |
|---|---|
| `[STAThread]` | WinForms 같은 Windows UI 프로그램에 필요한 스레드 설정 |
| `EnableVisualStyles()` | Windows 기본 테마 스타일 적용 |
| `SetCompatibleTextRenderingDefault(false)` | 기본 텍스트 렌더링 방식 설정 |
| `Application.Run(new MainForm())` | 메인 폼을 띄우고 메시지 루프 시작 |

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


---

## 10. 파일별 코드 실행 관계 상세 정리

### 10-1. 도서 저장 버튼을 눌렀을 때

```text
BookForm.Save()
  ↓
Book 객체 생성
  ↓
BookDAO.Exists(bookCode)
  ↓
있으면 BookDAO.Update(book)
없으면 BookDAO.Insert(book)
  ↓
DBManager.Execute(sql, parameters)
  ↓
SQL Server Book 테이블 반영
  ↓
BookForm.Load()
  ↓
DataGridView 새로고침
```

### 10-2. 회원 저장 버튼을 눌렀을 때

```text
MemberForm.Save()
  ↓
MemberForm.Read()
  ↓
Member 객체 생성
  ↓
MemberDAO.Exists(memberNo)
  ↓
Insert 또는 Update
  ↓
DBManager.Execute()
  ↓
Member 테이블 반영
```

### 10-3. 도서 대여 등록 버튼을 눌렀을 때

```text
RentalForm.RegisterRental()
  ↓
현재 선택 회원 확인
  ↓
BookDAO.GetByCode(bookCode)
  ↓
RentalDAO.IsActive(bookCode)
  ↓
SettingDAO.Get()
  ↓
신간/구간 판정
  ↓
대여료, 대여기간, 연체단가 결정
  ↓
RentalDAO.Rent()
  ↓
Rental 테이블 INSERT
```

### 10-4. 도서 반납 버튼을 눌렀을 때

```text
RentalForm.ReturnBook()
  ↓
선택한 RentalId 확인
  ↓
RentalDAO.Return(rentalId, today)
  ↓
Rental 테이블 UPDATE
  ↓
IsReturned = 1
ReturnDate = 오늘
OverdueFee = 연체일수 × 연체단가
```

---

## 11. 초보자가 자주 헷갈리는 용어 정리

| 용어 | 쉬운 설명 |
|---|---|
| 클래스 | 설계도. 예: `Book`이라는 도서 설계도 |
| 객체 | 설계도로 만든 실제 데이터. 예: `new Book()` |
| 메서드 | 클래스 안에 있는 기능 |
| 필드 | 객체가 가지고 있는 값 |
| 이벤트 | 버튼 클릭, 키 입력처럼 사용자가 일으키는 동작 |
| 핸들러 | 이벤트가 발생했을 때 실행되는 코드 |
| DAO | DB 작업을 담당하는 클래스 |
| ADO.NET | C#에서 DB에 직접 접속하는 기본 기술 |
| DataTable | SELECT 결과를 표 형태로 담는 객체 |
| DataGridView | DataTable을 화면에 표로 보여주는 컨트롤 |
| ConnectionString | DB 접속 주소와 인증 정보 |
| SQL Parameter | SQL에 값을 안전하게 넣는 방식 |
| Primary Key | 한 행을 구분하는 대표값 |
| Foreign Key | 다른 테이블과 연결하는 값 |

---

## 12. 실제 제출/수업용 설명 문장 예시

### 프로젝트 설명

이 프로젝트는 C# WinForms와 ADO.NET을 사용하여 만든 도서 대여 관리 시스템이다.  
SQL Server에 도서, 회원, 대여 내역, 대여 설정 정보를 저장하고, WinForms 화면에서 CRUD 및 대여/반납 처리를 수행한다.  
데이터 접근은 DAO 계층으로 분리했으며, 공통 DB 실행은 `DBManager`에서 처리한다.

### 사용 기술 설명

- C# WinForms: Windows 데스크톱 화면 구현
- ADO.NET: SQL Server와 직접 연결하여 데이터 처리
- SQL Server: 도서, 회원, 대여 데이터 저장
- DataGridView: DB 조회 결과 표시
- MDI: 메인 창 안에 여러 업무 창 표시
- CSV: 엑셀에서 열 수 있는 목록 파일 출력

### 핵심 구현 설명

도서와 회원은 각각 `BookDAO`, `MemberDAO`를 통해 등록, 수정, 삭제, 조회한다.  
대여 처리는 `RentalForm`에서 회원과 도서를 확인한 뒤, `RentalSetting`의 기준값을 이용해 신간/구간을 판정하고 대여료와 연체료 기준을 결정한다.  
반납 시에는 SQL Server의 `DATEDIFF`를 사용하여 연체일수를 계산하고, 연체료를 확정 저장한다.

---

## 13. 확인 문제

1. `DBManager.Query()`와 `DBManager.Execute()`의 차이는 무엇인가?
2. `SqlParameter`를 쓰는 이유는 무엇인가?
3. `BookDAO.Exists()`는 어떤 상황에서 필요한가?
4. `Rental` 테이블에 `RentFee`, `OverdueRate`를 저장하는 이유는 무엇인가?
5. `IDENTITY(1,1)`은 어떤 역할을 하는가?
6. `FOREIGN KEY` 때문에 삭제가 막히는 것은 오류인가, 정상인가?
7. 신간/구간 판정 기준은 어느 파일의 어느 메서드에서 적용되는가?
8. CSV 저장 시 `new UTF8Encoding(true)`를 쓰는 이유는 무엇인가?

---

## 14. 제출 전 최종 점검표

| 점검 항목 | 완료 |
|---|---|
| 프로젝트명이 `BookRentalSystem`인지 확인 | □ |
| 기본 `Form1.cs` 삭제 또는 미사용 처리 | □ |
| `Program.cs`에서 `MainForm` 실행 확인 | □ |
| `DB.cs`의 연결 문자열 수정 | □ |
| SSMS에서 DB 생성 스크립트 실행 | □ |
| 도서 등록 테스트 | □ |
| 회원 등록 테스트 | □ |
| 카드 발급 테스트 | □ |
| 대여 등록 테스트 | □ |
| 반납 처리 테스트 | □ |
| 정보 조회 테스트 | □ |
| CSV 출력 테스트 | □ |

---

## 15. 오류가 날 때 보는 순서

1. **DB 연결 오류인지 확인**
   - `ConnectionString` 확인
   - SQL Server 인스턴스 이름 확인
   - `BookRentalDB` 생성 여부 확인

2. **테이블 오류인지 확인**
   - SSMS에서 `SELECT * FROM Book` 실행
   - 테이블명이 정확한지 확인

3. **컬럼명 오류인지 확인**
   - SQL의 `AS 코드`와 C#의 `Cells["코드"]`가 일치하는지 확인

4. **외래키 오류인지 확인**
   - 대여 이력이 있는 도서/회원 삭제 시도 여부 확인

5. **입력값 오류인지 확인**
   - 회원번호는 숫자인지 확인
   - 도서코드는 비어 있지 않은지 확인
   - 날짜값이 정상인지 확인

---

## 16. 수업 진행 순서 추천

| 단계 | 수업 내용 |
|---|---|
| 1단계 | DB 테이블 생성과 관계 설명 |
| 2단계 | `DB.cs`로 ADO.NET 공통 실행 구조 설명 |
| 3단계 | `Models.cs`로 객체와 DB 행의 관계 설명 |
| 4단계 | `DAO.cs`로 SQL 분리 구조 설명 |
| 5단계 | `BookForm.cs`로 CRUD 흐름 실습 |
| 6단계 | `MemberForm.cs`로 CRUD + 카드번호 실습 |
| 7단계 | `RentalForm.cs`로 업무 로직 실습 |
| 8단계 | `QueryForm.cs`로 집계 SQL 실습 |
| 9단계 | `SettingForm.cs`로 설정값과 파일 출력 실습 |
| 10단계 | 전체 실행 후 오류 해결 실습 |

---

## 17. 핵심 결론

이 예제에서 가장 중요한 것은 화면 디자인 자체가 아니다.  
핵심은 다음 구조를 이해하는 것이다.

```text
화면(Form)은 입력과 표시를 담당한다.
DAO는 SQL을 담당한다.
DBManager는 DB 연결과 실행을 담당한다.
SQL Server는 데이터를 저장한다.
```

이 구조를 이해하면 도서 대여 프로그램뿐 아니라 재고관리, 회원관리, 설비관리, 생산관리 같은 다른 업무 프로그램도 같은 방식으로 만들 수 있다.
