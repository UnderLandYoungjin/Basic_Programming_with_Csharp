# 🟣 C# 제3강 실습 — 글자 수 분석기 (WinForms)
<img width="785" height="476" alt="image" src="https://github.com/user-attachments/assets/dde3e27e-9701-486a-8ae1-2bf4b95edd51" />

## 📌 개요

TextBox에 글자를 입력하고 버튼을 클릭하면  
**전체 글자 수**, **공백 수**, **특수문자 수**를 분석해서 Label에 출력하는 프로그램입니다.

---

## 🎯 완성 화면 미리보기

```
┌────────────────────────────────────────┐
│  입력:  [ 안녕하세요! 반갑습니다.    ] │
│                                        │
│         [ 글자 수 분석 ]               │
│                                        │
│  전체 글자 수  :  13                   │
│  공백 수       :  1                    │
│  특수문자 수   :  2                    │
└────────────────────────────────────────┘
```

---

## 2. 폼 디자인

| 컨트롤 | Name | Text |
|---|---|---|
| TextBox | `txtInput` | (비워두기) |
| Button | `btnAnalyze` | 글자 수 분석 |
| Label | `lblResult` | (비워두기) |

---

## 3. 전체 코드 (Form1.cs)

```csharp
using System;
using System.Windows.Forms;

namespace CharCounter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            string input = txtInput.Text;

            int total   = input.Length;
            int spaces  = 0;
            int special = 0;

            foreach (char c in input)
            {
                if (c == ' ')
                    spaces++;
                else if (!char.IsLetterOrDigit(c))
                    special++;
            }

            lblResult.Text =
                $"전체 글자 수 : {total}\r\n" +
                $"공백 수      : {spaces}\r\n" +
                $"특수문자 수  : {special}";
        }
    }
}
```

---

## 4. 코드 핵심 설명

### 📌 string.Length — 전체 글자 수

```csharp
int total = input.Length;
```

- 공백, 특수문자 포함 **모든 글자**를 셉니다.
- `"안녕하세요! 반갑습니다.".Length` → `13`

---

### 📌 foreach로 char 하나씩 꺼내기

```csharp
foreach (char c in input)
```

- `string`은 **char의 연속**이기 때문에 foreach로 한 글자씩 순회할 수 있습니다.
- 변수 `c`에 글자가 하나씩 들어옵니다.

---

### 📌 char 판별 메서드

```csharp
c == ' '                   // 공백 여부
char.IsLetterOrDigit(c)    // 글자 또는 숫자 여부
```

| 메서드 | 의미 |
|---|---|
| `char.IsLetter(c)` | 글자(한글, 영문)인지 |
| `char.IsDigit(c)` | 숫자인지 |
| `char.IsLetterOrDigit(c)` | 글자 또는 숫자인지 |
| `char.IsWhiteSpace(c)` | 공백인지 |

> 💡 공백도 아니고 글자/숫자도 아니면 → **특수문자**로 판별합니다.

---

### 📌 \r\n — 라벨 줄바꿈

```csharp
lblResult.Text =
    $"전체 글자 수 : {total}\r\n" +
    $"공백 수      : {spaces}\r\n" +
    $"특수문자 수  : {special}";
```

- WinForms Label에서 줄바꿈은 `\r\n`을 사용합니다.
- Label의 `AutoSize` 속성을 **False**로, `Size`를 충분히 크게 설정해야 여러 줄이 표시됩니다.

---

## 5. 실행 예시

| 입력 | 전체 | 공백 | 특수문자 |
|---|---|---|---|
| `안녕하세요` | 5 | 0 | 0 |
| `안녕하세요!` | 6 | 0 | 1 |
| `안녕하세요! 반갑습니다.` | 13 | 1 | 2 |
| `Hello, World!` | 13 | 1 | 2 |

---

## 🔍 핵심 정리

| 코드 | 의미 |
|---|---|
| `input.Length` | 전체 글자 수 |
| `foreach (char c in input)` | 문자열을 char 단위로 순회 |
| `c == ' '` | 공백 판별 |
| `!char.IsLetterOrDigit(c)` | 특수문자 판별 |
| `\r\n` | WinForms Label 줄바꿈 |

---

## 📝 도전 문제

### 문제 1

`"C# 프로그래밍! 재미있다."` 를 입력하면 각각 몇 개일까요?

<details>
<summary>정답 보기</summary>

```
전체 글자 수 : 17
공백 수      : 2
특수문자 수  : 2  (!  .)
```

</details>

---

### 문제 2

숫자만 세는 카운터를 추가하려면 어떻게 하면 될까요?

<details>
<summary>정답 보기</summary>

```csharp
int digits = 0;

foreach (char c in input)
{
    if (char.IsDigit(c))
        digits++;
}
```

</details>

---

> 📌 **Tip:**
> - `string`은 `char`들의 연속입니다. `foreach`로 한 글자씩 꺼낼 수 있습니다.
> - `char.IsLetterOrDigit()` 같은 char 전용 메서드를 활용하면 판별이 간단합니다.
> - Label 여러 줄 출력 시 `AutoSize = false` 설정을 잊지 마세요!
