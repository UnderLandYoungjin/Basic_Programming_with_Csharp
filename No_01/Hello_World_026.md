# 🟣 C# 첫 번째 프로그램 — Hello, World!

## 📌 개요

프로그램을 설치했다면, 가장 먼저 만들어 볼 것은 **"Hello, World!"** 를 출력하는 간단한 프로그램입니다.  
이 예제를 통해 C# 프로그램의 **기본 구조**를 이해할 수 있습니다.

---

## 💻 소스 코드

```csharp
using System;   // Console 클래스를 사용하기 위해 System 네임스페이스를 가져옵니다.

class Hello     // Hello라는 이름의 클래스를 정의합니다. (프로그램의 기본 단위)
{
    // 프로그램이 시작되는 메서드(Main 메서드)
    // public  : 외부에서도 접근 가능
    // static  : 객체 생성 없이 실행 가능
    // void    : 반환값이 없음
    public static void Main()
    {
        // Console.WriteLine()
        // Console : 콘솔(출력창) 기능을 제공하는 클래스
        // WriteLine : 한 줄을 출력하고 줄바꿈을 수행하는 메서드
        Console.WriteLine("Hello, World!"); // 화면에 "Hello, World!"를 출력합니다.
    }
}
```

```csharp
using System;   // Console 기능을 사용하기 위해 System 네임스페이스를 포함합니다.

namespace ConsoleApp5   // 프로젝트 이름과 동일한 네임스페이스 정의 (코드 영역 구분용)
{
    // internal : 같은 프로젝트 내부에서만 접근 가능
    // class    : Program이라는 클래스 정의
    internal class Program
    {
        // static : 객체 생성 없이 실행 가능
        // void   : 반환값 없음
        // Main   : 프로그램 시작 지점 (Entry Point)
        // string[] args : 실행 시 전달되는 명령줄 인수를 저장하는 배열
        static void Main(string[] args)
        {
            // 콘솔 창에 문자열 출력
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
