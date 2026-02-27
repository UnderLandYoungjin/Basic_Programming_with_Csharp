# 🟣 C# 제13강 — 문자열 응용 (String 심화)

## 📌 개요
1강에서 `string`이 문자열을 담는 자료형이라는 것을 배웠습니다.  
이번 강에서는 문자열을 **자르고, 찾고, 바꾸고, 합치는** 다양한 기능을 깊이 있게 다룹니다.  
실제 프로그램에서 문자열을 다루는 일은 매우 빈번하기 때문에 꼭 익혀두어야 합니다.

> 📝 **비유:** 문자열은 **종이에 쓴 글**입니다.  
> 가위로 자르고, 형광펜으로 찾고, 화이트로 지우고 다시 쓰는 것처럼  
> C#의 문자열 메서드로 텍스트를 자유롭게 다룰 수 있습니다.

---

## 1. 문자열 기본 속성

### 📌 Length — 문자열 길이

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string name = "홍길동";
        string email = "hong@example.com";

        Console.WriteLine(name.Length);    // 3
        Console.WriteLine(email.Length);   // 16

        // 빈 문자열 확인
        string empty = "";
        Console.WriteLine(empty.Length);   // 0
        Console.WriteLine(empty == "");    // True
    }
}
```

**실행 결과**
```
3
16
0
True
```

### 📌 인덱스로 문자 접근

문자열의 각 문자는 배열처럼 **인덱스(0부터 시작)** 로 접근할 수 있습니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string word = "Hello";

        Console.WriteLine(word[0]);   // H
        Console.WriteLine(word[1]);   // e
        Console.WriteLine(word[4]);   // o

        // 전체 문자 출력
        for (int i = 0; i < word.Length; i++)
        {
            Console.Write(word[i] + " ");
        }
        Console.WriteLine();
    }
}
```

**실행 결과**
```
H
e
o
H e l l o 
```

---

## 2. 대소문자 변환

| 메서드 | 설명 |
|---|---|
| `ToUpper()` | 모두 **대문자**로 변환 |
| `ToLower()` | 모두 **소문자**로 변환 |

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string text = "Hello, World!";

        Console.WriteLine(text.ToUpper());  // HELLO, WORLD!
        Console.WriteLine(text.ToLower());  // hello, world!

        // 대소문자 구분 없이 비교할 때 유용
        string input = "YES";
        if (input.ToLower() == "yes")
        {
            Console.WriteLine("동의하셨습니다.");
        }
    }
}
```

**실행 결과**
```
HELLO, WORLD!
hello, world!
동의하셨습니다.
```

---

## 3. 공백 제거 — Trim

| 메서드 | 설명 |
|---|---|
| `Trim()` | 앞뒤 공백 제거 |
| `TrimStart()` | 앞 공백만 제거 |
| `TrimEnd()` | 뒤 공백만 제거 |

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string input = "   홍길동   ";

        Console.WriteLine($"원본  : [{input}]");
        Console.WriteLine($"Trim  : [{input.Trim()}]");
        Console.WriteLine($"Start : [{input.TrimStart()}]");
        Console.WriteLine($"End   : [{input.TrimEnd()}]");
    }
}
```

**실행 결과**
```
원본  : [   홍길동   ]
Trim  : [홍길동]
Start : [홍길동   ]
End   : [   홍길동]
```

> 💡 **Tip:** 사용자 입력값을 받을 때 `Trim()`을 습관적으로 사용하면  
> 앞뒤 공백으로 인한 오류를 예방할 수 있습니다.

---

## 4. 문자열 검색

| 메서드 | 설명 | 반환값 |
|---|---|---|
| `Contains(str)` | 포함 여부 확인 | `bool` |
| `StartsWith(str)` | 특정 문자열로 시작하는지 | `bool` |
| `EndsWith(str)` | 특정 문자열로 끝나는지 | `bool` |
| `IndexOf(str)` | 처음 등장하는 위치 | `int` (없으면 `-1`) |

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string sentence = "C#은 마이크로소프트가 만든 프로그래밍 언어입니다.";

        Console.WriteLine(sentence.Contains("마이크로소프트"));   // True
        Console.WriteLine(sentence.Contains("애플"));             // False
        Console.WriteLine(sentence.StartsWith("C#"));             // True
        Console.WriteLine(sentence.EndsWith("언어입니다."));       // True

        int pos = sentence.IndexOf("프로그래밍");
        Console.WriteLine($"'프로그래밍' 위치: {pos}번째");        // 18번째
    }
}
```

**실행 결과**
```
True
False
True
True
'프로그래밍' 위치: 18번째
```

---

## 5. 문자열 자르기 — Substring

```
Substring(시작인덱스)           // 시작 위치부터 끝까지
Substring(시작인덱스, 길이)     // 시작 위치부터 지정한 길이만큼
```

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string date = "2025-07-15";

        string year  = date.Substring(0, 4);   // 2025
        string month = date.Substring(5, 2);   // 07
        string day   = date.Substring(8, 2);   // 15

        Console.WriteLine($"연도: {year}");
        Console.WriteLine($"월: {month}");
        Console.WriteLine($"일: {day}");

        string email = "hong@example.com";
        int atPos = email.IndexOf("@");
        string userId = email.Substring(0, atPos);  // @ 이전
        Console.WriteLine($"아이디: {userId}");
    }
}
```

**실행 결과**
```
연도: 2025
월: 07
일: 15
아이디: hong
```

---

## 6. 문자열 교체 — Replace

```
Replace(기존문자열, 새문자열)
```

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string text = "나는 자바를 좋아합니다. 자바는 정말 재미있습니다.";

        string result = text.Replace("자바", "C#");
        Console.WriteLine(result);

        // 공백 제거에도 활용
        string noSpace = "Hello World".Replace(" ", "");
        Console.WriteLine(noSpace);  // HelloWorld

        // 민감 정보 마스킹
        string phone = "010-1234-5678";
        string masked = phone.Replace(phone.Substring(4, 4), "****");
        Console.WriteLine(masked);  // 010-****-5678
    }
}
```

**실행 결과**
```
나는 C#를 좋아합니다. C#는 정말 재미있습니다.
HelloWorld
010-****-5678
```

---

## 7. 문자열 분리와 결합

### 📌 Split — 분리

```csharp
using System;
class Hello
{
    public static void Main()
    {
        // 쉼표로 분리
        string fruits = "사과,바나나,포도,딸기";
        string[] list = fruits.Split(',');

        foreach (string f in list)
        {
            Console.WriteLine(f);
        }

        Console.WriteLine();

        // 공백으로 분리
        string sentence = "C# 프로그래밍 입문";
        string[] words = sentence.Split(' ');

        for (int i = 0; i < words.Length; i++)
        {
            Console.WriteLine($"단어{i + 1}: {words[i]}");
        }
    }
}
```

**실행 결과**
```
사과
바나나
포도
딸기

단어1: C#
단어2: 프로그래밍
단어3: 입문
```

### 📌 Join — 결합

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string[] names = { "홍길동", "김영희", "이민준" };

        string joined1 = string.Join(", ", names);
        string joined2 = string.Join(" / ", names);

        Console.WriteLine(joined1);  // 홍길동, 김영희, 이민준
        Console.WriteLine(joined2);  // 홍길동 / 김영희 / 이민준
    }
}
```

**실행 결과**
```
홍길동, 김영희, 이민준
홍길동 / 김영희 / 이민준
```

---

## 8. 문자열 포맷팅

### 📌 문자열 보간 (String Interpolation)

```csharp
string name = "홍길동";
int age = 25;
Console.WriteLine($"이름: {name}, 나이: {age}");
```

### 📌 서식 지정자

| 서식 | 의미 | 예시 | 결과 |
|---|---|---|---|
| `{0:N0}` | 천 단위 구분 쉼표 | `{1234567:N0}` | `1,234,567` |
| `{0:F2}` | 소수점 자릿수 | `{3.14159:F2}` | `3.14` |
| `{0:D5}` | 최소 자릿수 (0 채움) | `{42:D5}` | `00042` |
| `{0:P0}` | 퍼센트 | `{0.85:P0}` | `85%` |

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int    price    = 1234567;
        double rate     = 0.856;
        double pi       = 3.141592;
        int    id       = 7;

        Console.WriteLine($"가격:    {price:N0}원");     // 1,234,567원
        Console.WriteLine($"정확도:  {rate:P1}");        // 85.6%
        Console.WriteLine($"원주율:  {pi:F3}");          // 3.142
        Console.WriteLine($"ID:      {id:D5}");          // 00007
    }
}
```

**실행 결과**
```
가격:    1,234,567원
정확도:  85.6%
원주율:  3.142
ID:      00007
```

---

## 9. StringBuilder — 효율적인 문자열 조립

`string`은 수정할 때마다 **새 객체를 만들기 때문에** 반복적으로 이어 붙이면 성능이 나빠집니다.  
`StringBuilder`는 **같은 객체를 수정**하기 때문에 반복 작업에 훨씬 효율적입니다.

```csharp
using System;
using System.Text;   // StringBuilder 사용을 위해 필요

class Hello
{
    public static void Main()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("안녕하세요. ");
        sb.Append("저는 C#을 공부하고 있습니다. ");
        sb.AppendLine("잘 부탁드립니다.");   // + 줄바꿈
        sb.Append("감사합니다.");

        Console.WriteLine(sb.ToString());   // 최종 문자열 출력
        Console.WriteLine($"길이: {sb.Length}");

        // 특정 내용 교체
        sb.Replace("C#", "CSharp");
        Console.WriteLine(sb.ToString());
    }
}
```

**실행 결과**
```
안녕하세요. 저는 C#을 공부하고 있습니다. 잘 부탁드립니다.
감사합니다.
길이: 44
안녕하세요. 저는 CSharp을 공부하고 있습니다. 잘 부탁드립니다.
감사합니다.
```

> 💡 **Tip:** 문자열을 수십~수백 번 이어 붙여야 할 때는 반드시 `StringBuilder`를 사용하세요.

---

## 🧪 예제 — 간단한 CSV 파서

```csharp
using System;
class Hello
{
    public static void Main()
    {
        // CSV 형식의 데이터 (이름, 나이, 점수)
        string[] csvData = {
            "홍길동,25,92",
            "김영희,22,88",
            "이민준,28,75",
            "박지수,24,95"
        };

        Console.WriteLine("이름\t나이\t점수\t등급");
        Console.WriteLine("-----------------------------");

        foreach (string row in csvData)
        {
            string[] cols = row.Split(',');

            string name  = cols[0];
            int    age   = int.Parse(cols[1]);
            int    score = int.Parse(cols[2]);
            string grade = score >= 90 ? "A"
                         : score >= 80 ? "B"
                         : score >= 70 ? "C" : "D";

            Console.WriteLine($"{name}\t{age}세\t{score}점\t{grade}");
        }
    }
}
```

**실행 결과**
```
이름    나이    점수    등급
-----------------------------
홍길동  25세    92점    A
김영희  22세    88점    B
이민준  28세    75점    C
박지수  24세    95점    A
```

---

## 🔍 핵심 메서드 요약

| 메서드 | 설명 | 예시 결과 |
|---|---|---|
| `.Length` | 문자열 길이 | `"Hello".Length` → `5` |
| `.ToUpper()` | 대문자 변환 | `"hello".ToUpper()` → `"HELLO"` |
| `.ToLower()` | 소문자 변환 | `"HELLO".ToLower()` → `"hello"` |
| `.Trim()` | 앞뒤 공백 제거 | `"  hi  ".Trim()` → `"hi"` |
| `.Contains(s)` | 포함 여부 | `"hello".Contains("ell")` → `True` |
| `.StartsWith(s)` | 시작 문자열 확인 | `"hello".StartsWith("he")` → `True` |
| `.IndexOf(s)` | 위치 검색 | `"hello".IndexOf("ll")` → `2` |
| `.Substring(i, n)` | 자르기 | `"hello".Substring(1, 3)` → `"ell"` |
| `.Replace(a, b)` | 교체 | `"hello".Replace("l","r")` → `"herro"` |
| `.Split(c)` | 분리 | `"a,b".Split(',')` → `["a","b"]` |
| `string.Join(s, arr)` | 결합 | `Join(",", ["a","b"])` → `"a,b"` |

---

## 📝 문제

---

### 문제 1

다음 코드의 출력 결과는 무엇인가요?

```csharp
string s = "Hello, C#!";
Console.WriteLine(s.Length);
Console.WriteLine(s.ToUpper());
Console.WriteLine(s.Contains("C#"));
Console.WriteLine(s.Replace("Hello", "Hi"));
```

<details>
<summary>정답 보기</summary>

```
10
HELLO, C#!
True
Hi, C#!
```

</details>

---

### 문제 2

다음 이메일 주소에서 `@` 기준으로 아이디와 도메인을 분리하여 출력하세요.

```csharp
string email = "gildong@naver.com";
// 아이디: gildong
// 도메인: naver.com
```

<details>
<summary>정답 보기</summary>

```csharp
string email = "gildong@naver.com";
string[] parts  = email.Split('@');
string   userId = parts[0];
string   domain = parts[1];

Console.WriteLine($"아이디: {userId}");
Console.WriteLine($"도메인: {domain}");
```

</details>

---

### 문제 3

`StringBuilder`를 사용해 1~5까지의 숫자를 `"1 + 2 + 3 + 4 + 5"` 형식으로 만들어 출력하세요.

<details>
<summary>정답 보기</summary>

```csharp
using System.Text;

StringBuilder sb = new StringBuilder();

for (int i = 1; i <= 5; i++)
{
    sb.Append(i);
    if (i < 5) sb.Append(" + ");
}

Console.WriteLine(sb.ToString());
// 1 + 2 + 3 + 4 + 5
```

</details>

---

### 문제 4

다음 문자열에서 잘못된 부분을 찾아 올바른 전화번호 형식으로 출력하세요.

```csharp
string phone = "010 1234 5678";
// 목표 출력: 010-1234-5678
```

<details>
<summary>정답 보기</summary>

```csharp
string phone  = "010 1234 5678";
string result = phone.Replace(" ", "-");
Console.WriteLine(result);  // 010-1234-5678
```

</details>

---

> 📌 **Tip:**
> - 문자열 비교 시 대소문자가 다를 수 있다면 `ToLower()` 또는 `ToUpper()`로 통일 후 비교하세요.
> - 사용자 입력값은 항상 `Trim()`으로 앞뒤 공백을 제거하는 습관을 들이세요.
> - 반복적인 문자열 조립에는 `string +` 대신 **`StringBuilder`** 를 사용하세요.
> - `Split()`과 `Join()`은 CSV, 로그 파싱 등 실전에서 매우 자주 사용됩니다.
