
<img width="2507" height="1333" alt="image" src="https://github.com/user-attachments/assets/07895348-b31c-47bb-be22-3c0f52f540e3" />

```csharp
namespace WinFormsApp8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "Hello, World!";
        }
    }
}
```

<img width="795" height="480" alt="image" src="https://github.com/user-attachments/assets/e9670587-f3b1-4ef6-a892-5fbd284c3bec" />
<img width="784" height="473" alt="image" src="https://github.com/user-attachments/assets/78c4ab5b-8730-4d92-9c0a-f9ef389b868b" />



# C# WinForms 기초 — Hello, World! 버튼 이벤트

---

## 1. 학습 목표

- WinForms 프로젝트를 생성할 수 있다.
- `Label`과 `Button` 컨트롤을 폼에 배치할 수 있다.
- 버튼 클릭 이벤트 핸들러를 작성할 수 있다.
- 이벤트를 통해 컨트롤의 속성을 변경할 수 있다.

---

## 2. 프로젝트 생성

1. Visual Studio 실행 → **새 프로젝트 만들기** 클릭  
2. **Windows Forms 앱 (.NET)** 선택 → **다음**  
3. 프로젝트 이름 입력 (예: `WinFormsApp1`) → **만들기**

> ✅ 프로젝트가 생성되면 `Form1`이 자동으로 열립니다.

---

## 3. 컨트롤 배치

### 3-1. Label 추가

1. 왼쪽 **도구 상자**에서 `Label`을 찾아 폼 위로 드래그합니다.
2. 폼 중앙 상단 위치에 놓습니다.
3. 오른쪽 **속성** 창에서 `(Name)` → `label1` 확인  
   (기본값이 `label1`이므로 그대로 사용합니다.)

### 3-2. Button 추가

1. 도구 상자에서 `Button`을 찾아 Label 아래쪽에 드래그합니다.
2. 속성 창에서 `(Name)` → `button1` 확인  
   `Text` 속성은 기본값 `button1`을 그대로 사용합니다.

> 배치 후 폼은 아래와 같은 모습입니다.
>
> ```
> ┌─────────────────────────┐
> │  label1                 │  ← 처음엔 "label1" 텍스트 표시
> │                         │
> │       [ button1 ]       │
> └─────────────────────────┘
> ```

---

## 4. 이벤트 핸들러 연결

버튼을 클릭했을 때 동작을 정의하려면 **Click 이벤트**를 등록해야 합니다.

### 방법 A — 디자이너에서 더블클릭 (가장 간단)

폼 디자이너에서 `button1`을 **더블클릭**하면 자동으로 아래 코드가 생성됩니다.

### 방법 B — 속성 창 이벤트 탭 이용

1. `button1` 선택 → 속성 창 상단의 **번개 모양(⚡) 아이콘** 클릭  
2. `Click` 항목 옆 빈 칸을 **더블클릭**  
3. 코드 에디터로 자동 이동되며 이벤트 메서드가 생성됩니다.

---

## 5. 코드 작성

`Form1.cs` 파일을 열면 아래와 같은 구조가 되어 있습니다.  
`button1_Click` 메서드 안에 한 줄만 추가합니다.

```csharp
namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        // 생성자: 폼이 처음 만들어질 때 호출됩니다.
        public Form1()
        {
            InitializeComponent(); // 디자이너에서 배치한 컨트롤들을 초기화
        }

        // button1을 클릭했을 때 실행되는 이벤트 핸들러
        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "Hello, World!"; // label1의 텍스트를 변경
        }
    }
}
```

### 코드 설명

| 코드 | 설명 |
|------|------|
| `public partial class Form1 : Form` | Form1 클래스가 Form을 상속받아 윈도우 창이 됩니다 |
| `InitializeComponent()` | 디자이너에서 배치한 컨트롤을 코드로 초기화합니다 |
| `button1_Click(object sender, EventArgs e)` | 버튼 클릭 시 자동 호출되는 이벤트 핸들러 메서드입니다 |
| `sender` | 이벤트를 발생시킨 컨트롤 객체 (여기서는 button1) |
| `EventArgs e` | 이벤트 관련 추가 정보를 담는 매개변수 |
| `label1.Text = "Hello, World!"` | label1 컨트롤의 Text 속성을 변경합니다 |

---

## 6. 실행 결과

**F5** 키 또는 상단 **▶ 실행** 버튼을 눌러 프로그램을 실행합니다.

- **실행 초기**: label1에 `label1` 텍스트가 표시됩니다.
- **button1 클릭 후**: label1 텍스트가 `Hello, World!`로 바뀝니다.

```
실행 전             실행 후 (버튼 클릭)
┌────────────┐      ┌────────────────┐
│ label1     │  →   │ Hello, World!  │
│            │      │                │
│ [button1]  │      │ [button1]      │
└────────────┘      └────────────────┘
```

---

## 7. 핵심 개념 정리

### 이벤트(Event)란?
사용자의 행동(클릭, 키 입력 등)이나 시스템 상황 변화가 발생했을 때  
프로그램에 알려주는 **신호**입니다.

### 이벤트 핸들러(Event Handler)란?
이벤트가 발생했을 때 실행되는 **메서드(함수)**입니다.  
버튼 클릭 → `button1_Click` 메서드 자동 호출 → 내부 코드 실행

```
[사용자] 버튼 클릭
    ↓
[WinForms] Click 이벤트 발생
    ↓
[코드] button1_Click() 메서드 실행
    ↓
[결과] label1.Text 변경
```

---

## 8. 과제

> 💡 **과제**: 버튼을 클릭할 때마다 label1에 표시되는 텍스트가  
> `"Hello, World!"` 와 `"안녕하세요!"` 로 **번갈아 바뀌도록** 코드를 수정하세요.

**힌트**: `if`문과 현재 `label1.Text` 값을 비교하면 됩니다.

```csharp
private void button1_Click(object sender, EventArgs e)
{
    if (label1.Text == "Hello, World!")
    {
        // 여기에 코드 작성
    }
    else
    {
        // 여기에 코드 작성
    }
}
```

<details>
<summary>정답 보기1 (스스로 풀어본 후 확인하세요!)</summary>

```csharp
private void button1_Click(object sender, EventArgs e)
{
    if (label1.Text == "Hello, World!")
    {
        label1.Text = "안녕하세요!";
    }
    else
    {
        label1.Text = "Hello, World!";
    }
}
```

</details>

<details>
<summary>정답 보기2 (스스로 풀어본 후 확인하세요!)</summary>

```
    namespace WinFormsApp8
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
            label1.Text = "Hello, World!";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (label1.Text == "Hello, World!")
            {
                label1.Text = "안녕하세요!";
            }
            else
            {
                label1.Text = "Hello, World!";
            }
        }
    }
}
```

</details>





---


