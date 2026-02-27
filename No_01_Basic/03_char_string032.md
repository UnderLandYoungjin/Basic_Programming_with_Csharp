# 🟣 C# 제3강 — 문자와 문자열 (char & string)

## 📌 개요
**문자(char)** 는 `'A'`, `'가'` 처럼 **단 하나의 문자**를 저장하는 자료형이고,  
**문자열(string)** 은 `"Hello"`, `"안녕하세요"` 처럼 **문자들의 연속(묶음)** 을 저장하는 자료형입니다.

---

## 1. 문자형 (char)

### 📌 char 선언과 초기화

`char` 형은 **단 하나의 문자**를 저장하며, 반드시 **작은따옴표(`'`)** 로 감쌉니다.

```
char 변수이름 = '문자';
```

```csharp
char grade = 'A';
char symbol = '@';
char letter = '가';
```

> ⚠️ **주의:** `char`는 단 하나의 문자만 저장합니다.  
> 두 글자 이상을 작은따옴표로 감싸면 **컴파일 오류**가 발생합니다.

```csharp
// 잘못된 예 (컴파일 오류)
// char wrong = 'AB';  // 두 글자 이상 불가
// char wrong = "A";   // 큰따옴표 사용 불가
```

### 📌 char 출력 예제

```csharp
using System;
class Hello
{
    public static void Main()
    {
        char grade  = 'A';
        char symbol = '@';
        char letter = '가';

        Console.WriteLine(grade);          // A
        Console.WriteLine(symbol);         // @
        Console.WriteLine(letter);         // 가
        Console.WriteLine($"학점: {grade}"); // 학점: A
    }
}
```

**실행 결과**
```
A
@
가
학점: A
```

---

## 2. 문자열형 (string)

### 📌 string 선언과 초기화

`string` 형은 **0개 이상의 문자로 이루어진 문자열**을 저장하며, 반드시 **큰따옴표(`"`)** 로 감쌉니다.

```
string 변수이름 = "문자열";
```

```csharp
string name    = "홍길동";
string message = "Hello, World!";
string empty   = "";           // 빈 문자열도 가능
```

### 📌 string 출력 예제

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string name    = "홍길동";
        string message = "안녕하세요!";

        Console.WriteLine(name);
        Console.WriteLine(message);
        Console.WriteLine($"이름: {name}, 인사: {message}");
    }
}
```

**실행 결과**
```
홍길동
안녕하세요!
이름: 홍길동, 인사: 안녕하세요!
```

---

## 3. 문자열 연결 (String Concatenation)

### 📌 `+` 연산자로 문자열 연결

`+` 연산자를 사용하면 두 문자열을 이어 붙일 수 있습니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string firstName = "길동";
        string lastName  = "홍";
        string fullName  = lastName + firstName;

        Console.WriteLine(fullName);              // 홍길동
        Console.WriteLine("이름: " + fullName);   // 이름: 홍길동
    }
}
```

**실행 결과**
```
홍길동
이름: 홍길동
```

### 📌 문자열 보간 (`$` 사용) ✅ 권장

`$` 기호를 사용하면 문자열 안에 변수를 `{변수명}` 형태로 바로 삽입할 수 있어 훨씬 편리합니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string name = "홍길동";
        int    age  = 25;

        Console.WriteLine($"이름: {name}, 나이: {age}세");
    }
}
```

**실행 결과**
```
이름: 홍길동, 나이: 25세
```

---

## 4. 이스케이프 시퀀스 (Escape Sequence)

문자열 안에 **특수 문자** (따옴표, 줄바꿈, 탭 등)를 넣을 때는 **이스케이프 시퀀스**를 사용합니다.  
이스케이프 시퀀스는 백슬래시(`\`) 뒤에 특정 문자를 조합합니다.

| 이스케이프 시퀀스 | 의미 | 설명 |
|---|---|---|
| `\n` | 줄바꿈 (New Line) | 다음 줄로 이동 |
| `\t` | 탭 (Tab) | 탭 간격만큼 이동 |
| `\\` | 백슬래시 (`\`) | 백슬래시 문자 자체 |
| `\"` | 큰따옴표 (`"`) | 문자열 안에 큰따옴표 삽입 |
| `\'` | 작은따옴표 (`'`) | 문자 안에 작은따옴표 삽입 |
| `\0` | 널 문자 (Null) | 문자열의 끝을 나타냄 |

```csharp
using System;
class Hello
{
    public static void Main()
    {
        Console.WriteLine("첫 번째 줄\n두 번째 줄");
        Console.WriteLine("이름\t나이\t점수");
        Console.WriteLine("경로: C:\\Users\\홍길동");
        Console.WriteLine("그가 \"안녕\"이라고 했다.");
    }
}
```

**실행 결과**
```
첫 번째 줄
두 번째 줄
이름	나이	점수
경로: C:\Users\홍길동
그가 "안녕"이라고 했다.
```

---

## 5. 문자열 주요 속성과 메서드

`string` 형은 다양한 기능(속성, 메서드)을 제공합니다.

### 📌 Length — 문자열 길이

`.Length` 속성으로 문자열의 문자 개수를 알 수 있습니다.

```csharp
string name = "홍길동";
Console.WriteLine(name.Length); // 3
```

### 📌 ToUpper() / ToLower() — 대소문자 변환

```csharp
string str = "Hello";
Console.WriteLine(str.ToUpper()); // HELLO
Console.WriteLine(str.ToLower()); // hello
```

### 📌 Contains() — 특정 문자열 포함 여부

```csharp
string sentence = "C#은 재미있다";
Console.WriteLine(sentence.Contains("재미")); // True
Console.WriteLine(sentence.Contains("Python")); // False
```

### 📌 Replace() — 문자열 치환

```csharp
string str = "나는 Java를 좋아한다";
string result = str.Replace("Java", "C#");
Console.WriteLine(result); // 나는 C#를 좋아한다
```

### 📌 Trim() — 앞뒤 공백 제거

```csharp
string str = "   안녕하세요   ";
Console.WriteLine(str.Trim()); // 안녕하세요
```

### 📌 Substring() — 부분 문자열 추출

```csharp
string str = "Hello, World!";
Console.WriteLine(str.Substring(7));    // World!  (7번째 인덱스부터 끝까지)
Console.WriteLine(str.Substring(7, 5)); // World   (7번째 인덱스부터 5글자)
```

> 💡 **인덱스(Index):** 문자열의 각 문자는 0부터 시작하는 번호를 가집니다.  
> `"Hello"` 에서 `H`는 0번, `e`는 1번, ... `o`는 4번입니다.

---

## 6. char와 string 비교

| 구분 | `char` | `string` |
|---|---|---|
| 저장 가능 단위 | 단 하나의 문자 | 0개 이상의 문자 |
| 리터럴 표기 | 작은따옴표 `'A'` | 큰따옴표 `"Hello"` |
| 크기 | 2 byte (고정) | 가변 (문자 수에 따라 다름) |
| 예시 | `char c = 'A';` | `string s = "Hello";` |

---

## 🧪 예제

### 예제 1 — char와 string 함께 사용

```csharp
using System;
class Hello
{
    public static void Main()
    {
        char   initial = 'H';
        string name    = "Hong GilDong";

        Console.WriteLine($"이니셜: {initial}");
        Console.WriteLine($"이름: {name}");
        Console.WriteLine($"이름 길이: {name.Length}글자");
    }
}
```

**실행 결과**
```
이니셜: H
이름: Hong GilDong
이름 길이: 12글자
```

---

### 예제 2 — 이스케이프 시퀀스 활용

```csharp
using System;
class Hello
{
    public static void Main()
    {
        Console.WriteLine("=== 성적표 ===");
        Console.WriteLine("이름\t점수\t등급");
        Console.WriteLine("홍길동\t95\tA");
        Console.WriteLine("김철수\t82\tB");
        Console.WriteLine("이영희\t76\tC");
    }
}
```

**실행 결과**
```
=== 성적표 ===
이름	점수	등급
홍길동	95	A
김철수	82	B
이영희	76	C
```

---

### 예제 3 — 문자열 메서드 종합 예제

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string message = "  Hello, C# World!  ";

        Console.WriteLine($"원본         : \"{message}\"");
        Console.WriteLine($"공백 제거    : \"{message.Trim()}\"");
        Console.WriteLine($"대문자       : {message.Trim().ToUpper()}");
        Console.WriteLine($"소문자       : {message.Trim().ToLower()}");
        Console.WriteLine($"길이         : {message.Trim().Length}글자");
        Console.WriteLine($"C# 포함?     : {message.Contains("C#")}");
        Console.WriteLine($"치환         : {message.Trim().Replace("C#", "Java")}");
    }
}
```

**실행 결과**
```
원본         : "  Hello, C# World!  "
공백 제거    : "Hello, C# World!"
대문자       : HELLO, C# WORLD!
소문자       : hello, c# world!
길이         : 16글자
C# 포함?     : True
치환         : Hello, Java World!
```

---

## 🔍 char / string 요약 정리

| 구분 | 형의 이름 | 읽는 방법 | 리터럴 표기 | 특징 |
|---|---|---|---|---|
| 문자형 | `char` | 차 | 작은따옴표 `'A'` | 단 하나의 문자, 2 byte |
| 문자열형 | `string` | 스트링 | 큰따옴표 `"Hello"` | 문자들의 묶음, 가변 크기 |

---

## 📝 문제

---

### 문제 1

다음 코드에서 화면에 출력되는 결과는 무엇인가요?

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string a = "Hello";
        string b = "World";
        Console.WriteLine(a + ", " + b + "!");
    }
}
```

<details>
<summary>정답 보기</summary>

```
Hello, World!
```

</details>

---

### 문제 2

다음 중 올바른 선언은 무엇인가요?

```
① char c = "A";
② char c = 'AB';
③ char c = 'A';
④ string s = 'Hello';
```

<details>
<summary>정답 보기</summary>

③ `char c = 'A';` — `char`는 작은따옴표를 사용하며 단 하나의 문자만 저장 가능합니다.

</details>

---

### 문제 3

다음 문자열에서 탭(`\t`)과 줄바꿈(`\n`)을 사용해 아래처럼 출력하세요.

```
출력 결과:
이름	나이
홍길동	25
```

빈칸을 채우세요.

```csharp
Console.WriteLine(________);
Console.WriteLine(________);
```

<details>
<summary>정답 보기</summary>

```csharp
Console.WriteLine("이름\t나이");
Console.WriteLine("홍길동\t25");
```

</details>

---

### 문제 4

다음 코드의 출력 결과는 무엇인가요?

```csharp
string str = "Hello, World!";
Console.WriteLine(str.Length);
Console.WriteLine(str.ToUpper());
Console.WriteLine(str.Contains("World"));
```

<details>
<summary>정답 보기</summary>

```
13
HELLO, WORLD!
True
```

</details>

---

### 문제 5

다음 코드에서 **잘못된 부분**을 모두 찾아 수정하세요.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        char grade = "A";
        string name = '홍길동';
        Console.WriteLine("이름: " name + ", 학점: " + grade)
    }
}
```

<details>
<summary>정답 보기</summary>

① `char grade = "A";` → `char grade = 'A';` (`char`는 작은따옴표 사용)  
② `string name = '홍길동';` → `string name = "홍길동";` (`string`은 큰따옴표 사용)  
③ `"이름: " name` → `"이름: " + name` (문자열 연결 연산자 `+` 누락)  
④ 마지막 줄 끝에 세미콜론(`;`) 누락

</details>

---

> 📌 **Tip:**
> - **단 하나의 문자**를 저장할 때는 `char`와 **작은따옴표(`'`)** 를 사용하세요.
> - **여러 문자로 이루어진 문자열**을 저장할 때는 `string`과 **큰따옴표(`"`)** 를 사용하세요.
> - 문자열 안에 특수 문자를 넣을 때는 **이스케이프 시퀀스(`\n`, `\t`, `\\`, `\"`)** 를 사용하세요.
> - 문자열과 변수를 함께 출력할 때는 **문자열 보간(`$"..."`)** 을 사용하면 편리합니다.
