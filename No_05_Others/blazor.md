

https://github.com/user-attachments/assets/289ffae1-36b6-4c9c-a39b-8d0dfb7a5381



https://github.com/user-attachments/assets/8b5e7086-393b-463c-a872-427bdad2a3b1

# 미니 게시판 만들기 (C# Blazor Server + SQLite)

웹 브라우저에서 동작하는 게시판을 처음부터 끝까지 만든다.
서버는 **Blazor Server**, 데이터베이스는 설치가 필요 없는 **SQLite**를 쓴다.
데이터 접근은 지금까지 해 온 방식 그대로 **ADO.NET 스타일**(`SqliteConnection`, `SqliteCommand`)로 직접 다룬다.

HTML/CSS/JavaScript를 따로 배우지 않아도 된다. 화면도 로직도 전부 C#으로 작성한다.
(도서 대여 관리 시스템과 계층 구조가 같다. 화면이 WinForms에서 웹으로 바뀌었을 뿐이다.)

이 문서는 **STEP 1부터 STEP 12까지 위에서 아래로 순서대로** 따라 하면 완성되도록 짜여 있다.
각 STEP 끝의 **✅ 확인** 항목을 통과한 뒤 다음 STEP으로 넘어가자. 중간중간 직접 실행해서
화면이 늘어나는 걸 눈으로 확인하며 진행한다.

---

## 0. 만들 프로그램

웹 브라우저로 접속하는 자유 게시판이다. 페이지는 딱 3개다.

| 화면 | 파일 | 주소(URL) | 하는 일 |
|------|------|-----------|---------|
| 글 목록 | `Board.razor` | `/` | 전체 글을 표로 표시, 제목 클릭으로 이동 |
| 글 쓰기·수정 | `PostEdit.razor` | `/board/write`, `/board/edit/3` | 새 글 작성과 기존 글 수정을 한 페이지로 처리 |
| 글 보기 | `PostView.razor` | `/board/3` | 글 내용 표시, 조회수 증가, 수정/삭제 |

핵심 동작은 이렇게 흘러간다.

1. 목록에서 **글쓰기** 버튼을 누르면 입력 화면이 뜬다.
2. 저장하면 SQLite의 `Post` 테이블에 INSERT 되고 목록으로 돌아온다.
3. 제목을 클릭하면 글 보기 화면이 뜨면서 조회수가 1 올라간다.
4. 글 보기에서 수정·삭제할 수 있다. 삭제는 확인창을 한 번 거친다.

완성 후의 계층 구조는 도서 대여 시스템과 똑같이 나뉜다.

```
화면(.razor 페이지)  →  PostDao  →  DB(SqliteConnection)  →  board.db 파일
```

WinForms 때 `BookForm → BookDao → DBManager → SQL Server`였던 것과 정확히 같은 구조다.
화면 계층만 바뀌었고, DAO 아래쪽은 하던 그대로다.

> **WinForms와 무엇이 같고 무엇이 다른가**
> - 같은 것: `버튼 클릭 → C# 메서드 실행 → 화면 갱신` 이벤트 구조, DAO 계층, SQL 직접 작성
> - 다른 것: 화면을 디자이너 대신 **HTML 태그**로 그린다. 컨트롤 배치 코드가 HTML로 바뀌었다고 생각하면 된다.

만드는 순서는 **아래층부터 위층으로** 올라간다. 데이터 계층(DB → 모델 → DAO)을 먼저 완성하고,
그 위에 화면을 한 장씩 얹으면서 매번 실행해 확인한다.

### 준비물

- Visual Studio 2022 (.NET 8 SDK 포함)
- 프로젝트 형식: **Blazor Web App**
- NuGet 패키지: **Microsoft.Data.Sqlite** 1개만 추가
- SQLite는 별도 설치가 없다. 실행하면 프로젝트 폴더에 `board.db` 파일 하나가 생기고 그게 DB 전부다.

---

## STEP 1. 프로젝트 생성

### Visual Studio에서

1. **새 프로젝트 만들기** → **Blazor Web App** 선택
2. 프로젝트 이름: `BoardWeb`
   - 아래 모든 코드의 네임스페이스가 `BoardWeb`이다. 다르게 지었다면 `namespace` 줄을 맞춰 준다.
3. 추가 정보 화면에서 아래처럼 선택한다. **여기가 중요하다.**
   - Framework: **.NET 8.0**
   - Interactive render mode: **Server**
   - Interactivity location: **Per page/component** (기본값)

> PC에 .NET 9 이상 SDK가 깔려 있어 8.0이 안 보이면 다른 버전으로 만들어도 진행은 된다.
> 다만 그 경우 템플릿이 만들어 주는 `Program.cs` 내용이 이 문서와 조금 다를 수 있다.
> STEP 5에서 다시 짚겠지만, **자기 템플릿 코드는 건드리지 말고 한 줄만 추가**하는 방식이라 버전이 달라도 괜찮다.

### 명령줄(CLI)로 할 경우

```bash
dotnet new blazor -n BoardWeb --interactivity Server
cd BoardWeb
```

### 템플릿 정리

템플릿에 딸려 오는 예제 페이지를 지운다. **특히 `Home.razor`는 반드시 지워야 한다.**
우리가 만들 목록 페이지가 같은 주소 `/`를 쓰기 때문에, 안 지우면 주소가 충돌해서 실행이 안 된다.

- `Components/Pages/Home.razor` → 삭제 (필수)
- `Components/Pages/Counter.razor` → 삭제 (선택)
- `Components/Pages/Weather.razor` → 삭제 (선택)

**✅ 확인**
- 솔루션 탐색기에 `BoardWeb` 프로젝트가 보이고, `Components/Pages` 안에 `Home.razor`가 없다.
- (이 시점에 실행하면 `/` 주소에 페이지가 없어 빈 화면/404가 뜨는 게 정상이다. 페이지는 STEP 7부터 만든다.)

---

## STEP 2. NuGet 패키지 설치 — Microsoft.Data.Sqlite

SQLite를 C#에서 쓰기 위한 패키지 하나만 설치한다.

- 솔루션 탐색기 → 프로젝트 우클릭 → **NuGet 패키지 관리** → 찾아보기 탭에서 `Microsoft.Data.Sqlite` 검색 → 설치

명령줄이라면:

```bash
dotnet add package Microsoft.Data.Sqlite
```

**✅ 확인**
- 프로젝트의 **종속성 → 패키지** 아래에 `Microsoft.Data.Sqlite`가 보인다.

---

## STEP 3. 데이터 계층 (1) — Data/DB.cs

이제부터 아래층(데이터 계층)을 만든다. 프로젝트에 `Data` 폴더부터 만든다.
(프로젝트 우클릭 → 추가 → 새 폴더 → 이름 `Data`)

첫 파일은 SQLite 연결을 한 곳에서 관리하는 헬퍼다.
`Data` 폴더 우클릭 → 추가 → 클래스 → `DB.cs`

```csharp
using Microsoft.Data.Sqlite;

namespace BoardWeb.Data
{
    // SQLite 연결을 한 곳에서 관리한다.
    public static class DB
    {
        // SQLite는 서버 주소가 없다. 파일 경로가 곧 연결 문자열이다.
        // 실행 폴더(프로젝트 폴더)에 board.db 파일이 만들어진다.
        public const string ConnStr = "Data Source=board.db";

        // 열린 연결을 돌려준다. 쓰는 쪽에서 using으로 감싸면 자동으로 닫힌다.
        public static SqliteConnection Open()
        {
            var conn = new SqliteConnection(ConnStr);
            conn.Open();
            return conn;
        }

        // 앱이 시작할 때 한 번 호출한다. 테이블이 없으면 만든다.
        // SSMS에서 테이블을 만들던 작업을 코드가 대신하는 셈이다.
        public static void Init()
        {
            using var conn = Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Post (
                    Id        INTEGER PRIMARY KEY AUTOINCREMENT,  -- MSSQL의 IDENTITY에 해당
                    Title     TEXT    NOT NULL,
                    Writer    TEXT    NOT NULL,
                    Content   TEXT    NOT NULL,
                    CreatedAt TEXT    NOT NULL,   -- SQLite는 날짜형이 없어 문자열로 저장한다
                    Views     INTEGER NOT NULL DEFAULT 0
                );";
            cmd.ExecuteNonQuery();
        }
    }
}
```

> SQLite의 자료형은 `INTEGER`, `TEXT`, `REAL`, `BLOB` 정도로 단출하다.
> MSSQL에서 `NVARCHAR`를 쓰던 자리는 전부 `TEXT`다. SQLite의 TEXT는 기본이 유니코드라 한글 걱정이 없다.

**✅ 확인**
- 빌드(Ctrl+Shift+B)했을 때 오류가 없다. (오류가 나면 대부분 STEP 2 패키지 미설치다.)

---

## STEP 4. 데이터 계층 (2) — Data/Post.cs

테이블 한 행을 담을 모델 클래스다. `Data` 폴더에 `Post.cs`를 추가한다.

```csharp
namespace BoardWeb.Data
{
    // Post 테이블 한 행을 담는 클래스. 도서 시스템의 Book과 같은 역할이다.
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Writer { get; set; } = "";
        public string Content { get; set; } = "";
        public string CreatedAt { get; set; } = "";
        public int Views { get; set; }
    }
}
```

**✅ 확인**
- STEP 3의 `CREATE TABLE` 컬럼과 이 클래스의 프로퍼티가 1:1로 대응하는지 눈으로 맞춰 본다.
  (Id, Title, Writer, Content, CreatedAt, Views — 여섯 개)

---

## STEP 5. 데이터 계층 (3) — Data/PostDao.cs

데이터 계층의 마지막 파일이다. 게시글 CRUD를 전담하며, **SQL은 전부 이 파일에만 있다.**
`Data` 폴더에 `PostDao.cs`를 추가한다.

```csharp
using Microsoft.Data.Sqlite;

namespace BoardWeb.Data
{
    // 게시글 CRUD를 전담한다. SQL은 전부 이 파일에만 있다.
    public static class PostDao
    {
        // 전체 목록 (최신 글이 위로 오도록 Id 내림차순)
        public static List<Post> GetAll()
        {
            var list = new List<Post>();
            using var conn = DB.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT Id, Title, Writer, CreatedAt, Views FROM Post ORDER BY Id DESC";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Post
                {
                    Id        = r.GetInt32(0),
                    Title     = r.GetString(1),
                    Writer    = r.GetString(2),
                    CreatedAt = r.GetString(3),
                    Views     = r.GetInt32(4)
                });
            }
            return list;
        }

        // 글 1건 조회 (없으면 null)
        public static Post? Get(int id)
        {
            using var conn = DB.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT Id, Title, Writer, Content, CreatedAt, Views FROM Post WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new Post
            {
                Id        = r.GetInt32(0),
                Title     = r.GetString(1),
                Writer    = r.GetString(2),
                Content   = r.GetString(3),
                CreatedAt = r.GetString(4),
                Views     = r.GetInt32(5)
            };
        }

        // 조회수 +1
        public static void IncreaseViews(int id)
        {
            using var conn = DB.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Post SET Views = Views + 1 WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        // 새 글 등록
        public static void Insert(Post p)
        {
            using var conn = DB.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Post (Title, Writer, Content, CreatedAt, Views)
                VALUES (@t, @w, @c, @d, 0)";
            cmd.Parameters.AddWithValue("@t", p.Title);
            cmd.Parameters.AddWithValue("@w", p.Writer);
            cmd.Parameters.AddWithValue("@c", p.Content);
            cmd.Parameters.AddWithValue("@d", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            cmd.ExecuteNonQuery();
        }

        // 글 수정
        public static void Update(Post p)
        {
            using var conn = DB.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Post SET Title = @t, Writer = @w, Content = @c WHERE Id = @id";
            cmd.Parameters.AddWithValue("@t", p.Title);
            cmd.Parameters.AddWithValue("@w", p.Writer);
            cmd.Parameters.AddWithValue("@c", p.Content);
            cmd.Parameters.AddWithValue("@id", p.Id);
            cmd.ExecuteNonQuery();
        }

        // 글 삭제
        public static void Delete(int id)
        {
            using var conn = DB.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Post WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
```

> 파라미터(`@t`, `@id`)를 쓰는 이유는 SQL Server 때와 같다.
> 문자열을 직접 이어붙이면 작은따옴표 하나에 깨지고, SQL 인젝션 공격에도 뚫린다.
> 클래스 이름이 `SqlCommand`에서 `SqliteCommand`로 바뀌었을 뿐, 쓰는 법은 완전히 같다는 점을 눈여겨봐 두자.

**✅ 확인**
- 빌드 오류가 없다. 여기까지로 **데이터 계층(아래 두 층)이 완성**됐다. 이제 화면만 얹으면 된다.

---

## STEP 6. Program.cs에 DB.Init() 추가 + 공용 using 등록

화면을 만들기 전에 손볼 곳이 두 군데 있다. 둘 다 한 줄짜리 수정이다.

### 6-1. Program.cs — `DB.Init();` 한 줄 추가

앱이 시작할 때 테이블을 만들어 두도록, 템플릿이 만들어 준 `Program.cs`에서
`var app = builder.Build();` **바로 다음 줄에 `DB.Init();`을 추가**한다.
위쪽 using에 `using BoardWeb.Data;`도 한 줄 넣는다.

.NET 8 템플릿 기준 전체 모습은 아래와 같다. **나머지는 템플릿 그대로 두면 된다.**
(.NET 9 이상으로 만들었다면 템플릿 내용이 조금 다를 수 있는데, 그래도 똑같이
`builder.Build()` 다음에 `DB.Init();` 한 줄만 추가하면 된다.)

```csharp
using BoardWeb.Components;
using BoardWeb.Data;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server 구성 (템플릿 기본)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

DB.Init();   // ★ 추가: 앱 시작 시 테이블이 없으면 만든다

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

### 6-2. Components/_Imports.razor — 공용 using 한 줄 추가

페이지마다 `@using BoardWeb.Data`를 쓰지 않도록 공용 파일에 등록한다.
`Components/_Imports.razor` 파일을 열고 **맨 아래에 한 줄**을 덧붙인다.

```razor
@using BoardWeb.Data
```

**✅ 확인**
- 빌드 오류가 없다.
- 한 번 실행(F5)해 보자. 아직 페이지가 없어 화면은 비어 있지만, **프로젝트 폴더에 `board.db` 파일이 생겼는지** 확인한다. 이게 `DB.Init()`이 동작한 증거다. 확인했으면 실행을 멈춘다.

> `board.db`는 "실행할 때의 작업 폴더"에 생긴다. VS에서 F5로 실행하면 프로젝트 폴더지만,
> `bin\Debug` 안의 exe를 직접 더블클릭해 실행하면 exe 옆에 생긴다. 파일이 안 보이면 거기를 찾아 보자.

---

## STEP 7. 첫 화면 — Components/Pages/Board.razor (글 목록)

이제 화면을 한 장씩 얹는다. `.razor` 파일은 위쪽이 HTML(화면), 아래 `@code` 블록이 C#(로직)이다.
WinForms로 치면 디자이너 화면과 코드 비하인드가 한 파일에 같이 있는 셈이다.

페이지 추가 방법: `Components/Pages` 우클릭 → 추가 → **Razor 구성 요소** → `Board.razor`

```razor
@page "/"
@page "/board"

<PageTitle>미니 게시판</PageTitle>

<div class="d-flex justify-content-between align-items-center mb-3">
    <h3 class="mb-0">자유 게시판</h3>
    <a class="btn btn-primary" href="/board/write">글쓰기</a>
</div>

<table class="table table-hover align-middle">
    <thead class="table-light">
        <tr>
            <th style="width:70px">번호</th>
            <th>제목</th>
            <th style="width:120px">작성자</th>
            <th style="width:160px">작성일</th>
            <th style="width:80px">조회</th>
        </tr>
    </thead>
    <tbody>
        @if (posts.Count == 0)
        {
            <tr>
                <td colspan="5" class="text-center text-muted py-5">
                    아직 글이 없습니다. 첫 글을 작성해 보세요.
                </td>
            </tr>
        }
        else
        {
            @foreach (var p in posts)
            {
                <tr>
                    <td class="text-muted">@p.Id</td>
                    <td>
                        <a class="text-decoration-none fw-semibold" href="/board/@p.Id">
                            @p.Title
                        </a>
                    </td>
                    <td>@p.Writer</td>
                    <td class="text-muted">@p.CreatedAt</td>
                    <td><span class="badge bg-secondary">@p.Views</span></td>
                </tr>
            }
        }
    </tbody>
</table>

@code {
    // 화면에 뿌릴 글 목록
    private List<Post> posts = new();

    // 페이지가 열릴 때 한 번 실행된다. WinForms의 Form_Load와 같은 자리다.
    protected override void OnInitialized()
    {
        posts = PostDao.GetAll();
    }
}
```

> 이 페이지에는 버튼 클릭 같은 상호작용이 없다(글쓰기·제목은 전부 링크다).
> 그래서 `@rendermode` 지정 없이도 동작한다. 상호작용이 필요한 다음 두 페이지와 비교해 보자.

**✅ 확인**
- F5 실행 → 브라우저에 "자유 게시판" 제목과 빈 표("아직 글이 없습니다")가 보인다.
- 이 시점에 **글쓰기** 버튼을 누르면 페이지가 없어 빈 화면이 뜨는 게 정상이다. 다음 STEP에서 만든다.

---

## STEP 8. 두 번째 화면 — Components/Pages/PostEdit.razor (글 쓰기·수정)

새 글 작성과 기존 글 수정을 한 페이지로 처리한다. 주소에 Id가 있으면 수정, 없으면 새 글이다.
`Components/Pages`에 `PostEdit.razor`를 추가한다.

```razor
@page "/board/write"
@page "/board/edit/{Id:int}"
@rendermode @(new InteractiveServerRenderMode(prerender: false))
@inject NavigationManager Nav

<PageTitle>@(IsEdit ? "글 수정" : "글쓰기")</PageTitle>

<div class="card shadow-sm">
    <div class="card-header bg-white">
        <h4 class="mb-0">@(IsEdit ? "글 수정" : "글쓰기")</h4>
    </div>
    <div class="card-body">

        @if (msg != "")
        {
            <div class="alert alert-warning py-2">@msg</div>
        }

        <div class="mb-3">
            <label class="form-label">제목</label>
            <input class="form-control" @bind="post.Title" placeholder="제목을 입력하세요" />
        </div>

        <div class="mb-3">
            <label class="form-label">작성자</label>
            <input class="form-control" style="max-width: 240px"
                   @bind="post.Writer" placeholder="이름" />
        </div>

        <div class="mb-3">
            <label class="form-label">내용</label>
            <textarea class="form-control" rows="10" @bind="post.Content"
                      placeholder="내용을 입력하세요"></textarea>
        </div>
    </div>
    <div class="card-footer bg-white d-flex gap-2">
        <button class="btn btn-primary" @onclick="Save">저장</button>
        <a class="btn btn-outline-secondary" href="/board">취소</a>
    </div>
</div>

@code {
    // 주소에 Id가 있으면 들어오고, 없으면 null이다.
    [Parameter] public int? Id { get; set; }

    private Post post = new();
    private string msg = "";

    // Id 유무로 새 글 / 수정을 구분한다.
    private bool IsEdit => Id.HasValue;

    protected override void OnParametersSet()
    {
        if (IsEdit)
        {
            // 수정 모드: 기존 글을 읽어 입력칸에 채운다.
            post = PostDao.Get(Id!.Value) ?? new Post();
        }
    }

    private void Save()
    {
        // 간단한 입력 검사
        if (string.IsNullOrWhiteSpace(post.Title) ||
            string.IsNullOrWhiteSpace(post.Writer))
        {
            msg = "제목과 작성자는 반드시 입력해야 합니다.";
            return;
        }

        if (IsEdit)
        {
            PostDao.Update(post);
            Nav.NavigateTo($"/board/{Id}");   // 수정 후엔 글 보기로
        }
        else
        {
            PostDao.Insert(post);
            Nav.NavigateTo("/board");          // 등록 후엔 목록으로
        }
    }
}
```

> **버튼이 동작하려면 `@rendermode` 지시문이 필수다.**
> 목록 페이지와 달리 이 페이지는 입력칸(`@bind`)과 버튼(`@onclick`)이 있다.
> 맨 위 `@rendermode` 지시문이 빠지면 화면은 멀쩡히 떠도 버튼이 아무 반응을 안 한다. 가장 흔한 실수다.
>
> `prerender: false`는 사전 렌더링(서버에서 미리 한 번 그리는 동작)을 끈다.
> 이 페이지에서는 초기화가 한 번만 돌게 해서, 페이지가 뜨자마자 입력한 내용이 초기화되는 일을 막는다.
> 다음 STEP의 글 보기 페이지에서는 이 지시문이 더 중요한 역할을 하니 기억해 두자.

> `@bind="post.Title"`은 입력칸과 C# 변수를 묶어 준다.
> WinForms에서 `textBox1.Text`를 읽고 쓰던 일을 선언 한 번으로 끝내는 것이다.
> 버튼의 `@onclick="Save"`는 `button1.Click += ...` 이벤트 연결과 같은 역할이다.

**✅ 확인**
- F5 실행 → **글쓰기** 버튼 → 제목/작성자/내용 입력 → **저장** → 목록에 글이 한 줄 보인다.
- 제목을 비우고 저장하면 노란 경고 문구가 뜨는 것도 확인한다.
- 글을 두세 개 더 써 보자. 최신 글이 맨 위로 오는지(Id 내림차순) 본다.
- 이 시점에 제목을 클릭하면 빈 화면이 뜨는 게 정상이다. 글 보기는 다음 STEP이다.

---

## STEP 9. 세 번째 화면 — Components/Pages/PostView.razor (글 보기)

마지막 페이지다. 글 내용을 보여 주고, 조회수를 올리고, 수정/삭제 버튼을 단다.
`Components/Pages`에 `PostView.razor`를 추가한다.

```razor
@page "/board/{Id:int}"
@rendermode @(new InteractiveServerRenderMode(prerender: false))
@inject NavigationManager Nav
@inject IJSRuntime JS

<PageTitle>글 보기</PageTitle>

@if (post == null)
{
    <div class="alert alert-warning">존재하지 않는 글입니다.</div>
    <a class="btn btn-outline-secondary" href="/board">목록으로</a>
}
else
{
    <div class="card shadow-sm">
        <div class="card-header bg-white">
            <h4 class="mb-1">@post.Title</h4>
            <div class="text-muted small">
                @post.Writer · @post.CreatedAt ·
                조회 <span class="badge bg-secondary">@post.Views</span>
            </div>
        </div>
        <div class="card-body" style="white-space: pre-wrap; min-height: 200px;">
            @post.Content
        </div>
        <div class="card-footer bg-white d-flex gap-2">
            <a class="btn btn-outline-secondary" href="/board">목록</a>
            <a class="btn btn-outline-primary" href="/board/edit/@post.Id">수정</a>
            <button class="btn btn-outline-danger ms-auto" @onclick="Delete">삭제</button>
        </div>
    </div>
}

@code {
    // 주소의 숫자가 자동으로 들어온다. /board/3 이면 Id = 3
    [Parameter] public int Id { get; set; }

    private Post? post;

    protected override void OnInitialized()
    {
        PostDao.IncreaseViews(Id);  // 조회수 +1
        post = PostDao.Get(Id);     // 글 내용 읽기
    }

    private async Task Delete()
    {
        // 브라우저의 confirm 창을 띄운다. 취소를 누르면 false가 돌아온다.
        bool ok = await JS.InvokeAsync<bool>("confirm", "정말 삭제할까요?");
        if (!ok) return;

        PostDao.Delete(Id);
        Nav.NavigateTo("/board");   // 목록으로 이동
    }
}
```

> **이 페이지에서 `prerender: false`가 꼭 필요한 이유**
> Blazor Server는 기본적으로 페이지를 두 번 그린다(서버에서 미리 한 번 + 연결 후 한 번).
> 그대로 두면 `OnInitialized`가 두 번 돌아서 조회수가 **2씩** 올라간다.
> `prerender: false`는 미리 그리기를 꺼서 한 번만 실행하게 한다.

> 조회수를 올린(`IncreaseViews`) **다음에** 글을 읽기(`Get`) 때문에, 화면의 조회수에는
> "지금 이 방문"이 포함된다. 막 작성한 글을 처음 열었을 때 조회수가 0이 아니라 1인 이유다.

**✅ 확인**
- F5 실행 → 목록에서 제목 클릭 → 글 내용이 보이고 조회수가 표시된다.
- **F5(새로고침)를 누를 때마다 조회수가 정확히 1씩** 올라간다. 2씩 올라가면 맨 위 `@rendermode` 줄을 다시 본다.
- **수정** → 내용을 바꾸고 저장 → 글 보기로 돌아오며 바뀐 내용이 보인다.
- **삭제** → 확인창에서 취소하면 그대로, 확인하면 목록으로 돌아오고 글이 사라진다.

여기까지로 **게시판의 모든 기능이 완성**됐다. 남은 건 왼쪽 메뉴 정리뿐이다.

---

## STEP 10. 왼쪽 메뉴 정리 — Components/Layout/NavMenu.razor

템플릿의 메뉴에는 지워 버린 Counter/Weather 링크가 남아 있다.
`Components/Layout/NavMenu.razor` 내용을 전부 지우고 아래로 교체한다.

```razor
<div class="top-row ps-3 navbar navbar-dark">
    <div class="container-fluid">
        <a class="navbar-brand" href="">미니 게시판</a>
    </div>
</div>

<input type="checkbox" title="Navigation menu" class="navbar-toggler" />

<div class="nav-scrollable" onclick="document.querySelector('.navbar-toggler').click()">
    <nav class="nav flex-column">
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="" Match="NavLinkMatch.All">
                <span class="bi bi-house-door-fill-nav-menu" aria-hidden="true"></span> 글 목록
            </NavLink>
        </div>
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="board/write">
                <span class="bi bi-plus-square-fill-nav-menu" aria-hidden="true"></span> 글쓰기
            </NavLink>
        </div>
    </nav>
</div>
```

**✅ 확인**
- 왼쪽 메뉴에 **글 목록 / 글쓰기** 두 항목만 보이고, 클릭하면 각 페이지로 이동한다.

---

## STEP 11. 최종 점검 — 전체 시나리오 실행

F5로 실행하고 처음부터 끝까지 한 바퀴 돌려 본다.

1. 브라우저가 열리면 게시판 목록이 보인다. 프로젝트 폴더에 `board.db`가 있는 것도 확인해 보자.
2. **글쓰기** → 제목/작성자/내용 입력 → 저장 → 목록에 글이 보인다.
3. 제목 클릭 → 글 내용이 보이고 조회수가 1 올라간다. 새로고침할 때마다 1씩 늘어난다.
4. **수정**으로 내용을 바꾸고, **삭제**를 누르면 확인창이 뜬 뒤 지워진다.
5. 같은 PC의 다른 브라우저(엣지/크롬)에서 같은 주소로 접속해 보자. **같은 데이터가 보인다.**
   DB가 서버 쪽에 있으니 어느 브라우저에서 봐도 동일하다. 이것이 WinForms와 웹의 가장 큰 차이다.

---

## STEP 12. 자주 막히는 곳

| 증상 | 원인 | 해결 |
|------|------|------|
| 실행하자마자 라우트 오류 (`/`가 중복) | `Home.razor`를 안 지움 | `Components/Pages/Home.razor` 삭제 (STEP 1) |
| 버튼을 눌러도 아무 반응이 없음 | 페이지에 `@rendermode` 지시문이 빠짐 | PostEdit·PostView 맨 위 지시문 확인 (STEP 8·9) |
| `Microsoft.Data.Sqlite`를 찾을 수 없음 | NuGet 패키지 미설치 | NuGet에서 설치 (STEP 2) |
| `PostDao`를 찾을 수 없음 | `_Imports.razor`에 using 누락 | `@using BoardWeb.Data` 추가 (STEP 6-2) |
| 조회수가 2씩 올라감 | 사전 렌더링으로 초기화가 두 번 실행 | PostView의 `prerender: false` 지시문 확인 (STEP 9) |
| `board.db` 파일이 안 보임 | 실행 방식에 따라 생성 위치가 다름 | F5 실행이면 프로젝트 폴더, exe 직접 실행이면 `bin\Debug` 쪽 확인 (STEP 6) |
| 데이터를 초기화하고 싶음 | — | 앱 종료 후 `board.db` 삭제 (다음 실행 때 새로 생성됨) |

---

## 확장 과제

1. **검색**: 목록 위에 검색창을 달고 `WHERE Title LIKE @kw` 로 제목 검색을 만들어 보자.
   (`"%" + 검색어 + "%"` 를 파라미터로 넘긴다)
2. **페이징**: 글이 많아지면 `LIMIT 10 OFFSET @n` 으로 10건씩 끊어 보여 주자.
3. **비밀번호 삭제**: 글 작성 시 비밀번호를 받아 두고, 일치할 때만 삭제되게 바꿔 보자.
4. **댓글**: `Comment` 테이블(`PostId` 외래키)을 추가하고 글 보기 아래에 댓글 목록·입력을 붙여 보자.
5. **도서 대여 시스템 이식**: 이 게시판과 똑같은 요령으로, 수업 때 만든 `BookDao`·`MemberDao`를
   가져와 도서 목록/회원 목록 페이지를 만들어 보자. DAO 코드는 거의 손대지 않아도 된다.

---

## 코드 읽는 순서 (복습용)

만든 순서 그대로, 아래층부터 다시 읽으면 된다.

1. `DB.cs` — 연결 문자열과 테이블 생성. 가장 아래층. (STEP 3)
2. `Post.cs` → `PostDao.cs` — 모델과 CRUD. SQL Server 때와 비교하며 읽기. (STEP 4·5)
3. `Board.razor` — 목록 출력. `OnInitialized`가 Form_Load 자리라는 것. (STEP 7)
4. `PostEdit.razor` — `@bind`(입력칸 연결)와 `@onclick`(버튼 이벤트). (STEP 8)
5. `PostView.razor` — 주소 파라미터(`{Id:int}`)와 페이지 이동(`NavigateTo`). (STEP 9)

---

## 13. 코드 읽는 순서 (복습용)

1. `DB.cs` — 연결 문자열과 테이블 생성. 가장 아래층.
2. `Post.cs` → `PostDao.cs` — 모델과 CRUD. SQL Server 때와 비교하며 읽기.
3. `Board.razor` — 목록 출력. `OnInitialized`가 Form_Load 자리라는 것.
4. `PostEdit.razor` — `@bind`(입력칸 연결)와 `@onclick`(버튼 이벤트).
5. `PostView.razor` — 주소 파라미터(`{Id:int}`)와 페이지 이동(`NavigateTo`).
