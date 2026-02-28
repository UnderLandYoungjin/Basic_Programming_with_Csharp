# 🟣 C# 첫 번째 프로그램 — Hello, World!

## 📌 개요
프로그램을 설치했다면, 가장 먼저 만들어 볼 것은 **"Hello, World!"** 를 출력하는 간단한 프로그램입니다.
이 예제를 통해 C# 프로그램의 **기본 구조**를 이해할 수 있습니다.

---

## 💻 소스 코드
```csharp
using System;   
class Hello     
{
    public static void Main()
    {
        Console.WriteLine("Hello, World!"); // 화면에 "Hello, World!"를 출력합니다.
    }
}
```
```csharp
using System;   
namespace ConsoleApp5   
{
    // internal : 같은 프로젝트 내부에서만 접근 가능
    // class    : Program이라는 클래스 정의
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
```

---

## 🔍 코드 구조 설명

| 구성 요소 | 설명 |
|---|---|
| `using System;` | 시스템 기능(입출력 등)을 사용하기 위한 네임스페이스 선언 |
| `class Hello` | 프로그램의 기본 단위인 **클래스** 정의. 클래스명은 자유롭게 지정 가능 |
| `public static void Main()` | 프로그램이 시작되는 **진입점(Entry Point)** 메서드 |
| `Console.Write(...)` | 괄호 안의 **문자열을 화면에 출력** |
| `Console.WriteLine(...)` | 괄호 안의 **문자열을 화면에 출력**하고 줄바꿈 |

---

## ▶️ 실행 결과
```
Hello, World!
```

---

## 📝 핵심 포인트

- C# 프로그램은 반드시 **`class`** 안에 작성합니다.
- 프로그램은 항상 **`Main()` 메서드**에서 시작됩니다.
- **`Console.WriteLine()`** 은 텍스트를 출력할 때 가장 자주 사용하는 메서드입니다.
- 문장 끝에는 반드시 **세미콜론 `;`** 을 붙입니다.

---

## 🧪 예제

### 예제 1 — 이름을 바꿔서 출력해보기

`"Hello, World!"` 대신 원하는 문자열로 바꿔서 출력해봅니다.
```csharp
using System;
class Hello
{
    public static void Main()
    {
        Console.WriteLine("안녕하세요, C#!");   // 한글도 출력 가능합니다.
        Console.WriteLine("My name is C#.");
    }
}
```

**실행 결과**
```
안녕하세요, C#!
My name is C#.
```

---

### 예제 2 — Write 와 WriteLine 차이

`Console.Write()` 는 줄바꿈 없이 이어서 출력하고,
`Console.WriteLine()` 은 출력 후 줄바꿈을 합니다.
```csharp
using System;
class Hello
{
    public static void Main()
    {
        Console.Write("Hello, ");       // 줄바꿈 없이 이어서 출력
        Console.Write("World!");        // 같은 줄에 이어서 출력
        Console.WriteLine();            // 줄바꿈만 수행
        Console.WriteLine("다음 줄입니다.");
    }
}
```

**실행 결과**
```
Hello, World!
다음 줄입니다.
```

---

### 예제 3 — 여러 줄 출력하기

`Console.WriteLine()` 을 여러 번 사용하면 여러 줄을 출력할 수 있습니다.
```csharp
using System;
class Hello
{
    public static void Main()
    {
        Console.WriteLine("===================");
        Console.WriteLine("  C# 프로그래밍 시작  ");
        Console.WriteLine("===================");
    }
}
```

**실행 결과**
```
===================
  C# 프로그래밍 시작  
===================
```

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
        Console.WriteLine("Hello");
        Console.WriteLine("World");
    }
}
```

**정답:**

<details>
<summary>정답 보기 (클릭)</summary>
<pre>
Hello
World
</pre>

</details>

---

### 문제 2

다음 코드의 실행 결과는 무엇인가요?
```csharp
using System;
class Hello
{
    public static void Main()
    {
        Console.Write("C#");
        Console.Write(" is");
        Console.WriteLine(" fun!");
        Console.WriteLine("Done.");
    }
}
```

**정답:**

<details>
<summary>정답 보기 (클릭)</summary>
```
C# is fun!
Done.
```

</details>

---

### 문제 3

`Console.Write()` 와 `Console.WriteLine()` 의 차이를 설명하세요.

**정답:**

<details>
<summary>정답 보기 (클릭)</summary>

`Console.Write()`는 출력 후 줄바꿈을 하지 않아 다음 출력이 같은 줄에 이어집니다.  
`Console.WriteLine()`은 출력 후 자동으로 줄바꿈하여 다음 출력이 새 줄에서 시작됩니다.

</details>

---

### 문제 4

아래 빈칸을 채워 `Hello, C#!` 이 출력되도록 코드를 완성하세요.
```csharp
using System;
class Hello
{
    public static void Main()
    {
        Console.________("Hello, C#!");
    }
}
```

**정답:**

<details>
<summary>정답 보기 (클릭)</summary>
```
WriteLine
```

</details>

---

### 문제 5

다음 코드에서 **잘못된 부분**을 모두 찾아 수정하세요.
```csharp
using System
class Hello
{
    public static void Main()
    {
        console.writeline("Hello, World!")
    }
}
```

**정답:**

<details>
<summary>정답 보기 (클릭)</summary>

① `using System` 뒤에 세미콜론(`;`) 누락 → `using System;`  
② `console` → `Console` (C#은 대소문자 구분)  
③ `writeline` → `WriteLine` (대소문자 구분)  
④ `Console.WriteLine("Hello, World!")` 뒤에 세미콜론(`;`) 누락

</details>

---

> 📌 **Tip:** C#은 **대소문자를 구분**합니다. `console` 과 `Console` 은 전혀 다른 것으로 인식됩니다!
