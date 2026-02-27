# 🟣 C# 제16강 — 예외 처리 (Exception Handling)

## 📌 개요
프로그램을 실행하다 보면 예상치 못한 상황이 발생합니다.  
파일이 없거나, 숫자 대신 문자를 입력받거나, 0으로 나누거나 — 이런 상황을 **예외(Exception)** 라고 합니다.

예외 처리를 하지 않으면 프로그램이 **갑자기 종료**됩니다.  
예외 처리를 잘 하면 문제가 생겨도 **안내 메시지를 보여주고 계속 실행**되는 안정적인 프로그램을 만들 수 있습니다.

> 🚗 **비유:** 자동차를 운전하다 타이어가 펑크 났을 때,  
> 예외 처리가 없으면 → 차가 그대로 멈추고 고장  
> 예외 처리가 있으면 → "타이어 이상 감지! 안전하게 정차합니다" 메시지 후 안전 정차

---

## 1. 예외가 발생하는 상황

먼저 예외가 어떤 상황에서 터지는지 알아봅니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        // 0으로 나누기
        int a = 10;
        int b = 0;
        Console.WriteLine(a / b);  // ❌ DivideByZeroException 발생!
    }
}
```

```
처리되지 않은 예외: System.DivideByZeroException: 0으로 나누려 했습니다.
```

이처럼 예외 처리 없이는 프로그램이 즉시 종료됩니다.

---

## 2. try-catch — 기본 예외 처리

```
try
{
    // 예외가 발생할 수 있는 코드
}
catch (예외타입 변수)
{
    // 예외가 발생했을 때 처리할 코드
}
```

```csharp
using System;
class Hello
{
    public static void Main()
    {
        try
        {
            int a = 10;
            int b = 0;
            int result = a / b;          // 예외 발생!
            Console.WriteLine(result);   // 이 줄은 실행되지 않음
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine($"오류: 0으로 나눌 수 없습니다.");
            Console.WriteLine($"상세: {ex.Message}");
        }

        Console.WriteLine("프로그램 계속 실행됩니다.");
    }
}
```

**실행 결과**
```
오류: 0으로 나눌 수 없습니다.
상세: 0으로 나누려 했습니다.
프로그램 계속 실행됩니다.
```

> 💡 **Tip:** `ex.Message`로 예외의 상세 메시지를 확인할 수 있습니다.  
> `try` 블록에서 예외가 발생하면 **남은 코드를 건너뛰고** 즉시 `catch`로 이동합니다.

---

## 3. finally — 항상 실행되는 블록

예외 발생 여부와 **관계없이 반드시 실행**되어야 하는 코드를 `finally`에 넣습니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        Console.WriteLine("=== 정상 케이스 ===");
        try
        {
            int result = 10 / 2;
            Console.WriteLine($"결과: {result}");
        }
        catch (DivideByZeroException)
        {
            Console.WriteLine("0으로 나눌 수 없습니다.");
        }
        finally
        {
            Console.WriteLine("finally: 항상 실행됩니다.\n");
        }

        Console.WriteLine("=== 예외 케이스 ===");
        try
        {
            int result = 10 / 0;   // 예외 발생
            Console.WriteLine($"결과: {result}");
        }
        catch (DivideByZeroException)
        {
            Console.WriteLine("0으로 나눌 수 없습니다.");
        }
        finally
        {
            Console.WriteLine("finally: 항상 실행됩니다.");
        }
    }
}
```

**실행 결과**
```
=== 정상 케이스 ===
결과: 5
finally: 항상 실행됩니다.

=== 예외 케이스 ===
0으로 나눌 수 없습니다.
finally: 항상 실행됩니다.
```

> 💡 **Tip:** 파일 닫기, 로그 기록, 연결 해제처럼 **마무리 작업**은 `finally`에 두는 것이 안전합니다.

---

## 4. 여러 catch — 예외 종류별 처리

예외 종류에 따라 **다른 방식으로 처리**할 수 있습니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string[] inputs = { "10", "0", "abc", null };

        foreach (string input in inputs)
        {
            Console.Write($"입력 [{input ?? "null"}] → ");
            try
            {
                int number = int.Parse(input);   // 숫자가 아니면 예외
                int result = 100 / number;        // 0이면 예외
                Console.WriteLine($"100 / {number} = {result}");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("오류: 0으로 나눌 수 없습니다.");
            }
            catch (FormatException)
            {
                Console.WriteLine("오류: 숫자 형식이 아닙니다.");
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("오류: 값이 null입니다.");
            }
        }
    }
}
```

**실행 결과**
```
입력 [10] → 100 / 10 = 10
입력 [0] → 오류: 0으로 나눌 수 없습니다.
입력 [abc] → 오류: 숫자 형식이 아닙니다.
입력 [null] → 오류: 값이 null입니다.
```

> ⚠️ **주의:** `catch` 블록은 **위에서 아래 순서**로 매칭됩니다.  
> 더 구체적인 예외 타입을 위에, 더 일반적인 타입을 아래에 배치하세요.

---

## 5. Exception — 모든 예외의 부모

`Exception`은 모든 예외 클래스의 최상위 부모입니다.  
어떤 예외인지 모를 때 마지막 `catch`에 두어 **예상치 못한 예외까지 처리**합니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        try
        {
            int[] arr = { 1, 2, 3 };
            Console.WriteLine(arr[10]);  // IndexOutOfRangeException 발생
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine("오류: 배열 범위를 벗어났습니다.");
        }
        catch (Exception ex)
        {
            // 위에서 처리하지 못한 모든 예외
            Console.WriteLine($"예상치 못한 오류: {ex.Message}");
        }
    }
}
```

**실행 결과**
```
오류: 배열 범위를 벗어났습니다.
```

---

## 6. 자주 등장하는 예외 타입

| 예외 타입 | 발생 상황 |
|---|---|
| `DivideByZeroException` | 0으로 나누기 |
| `FormatException` | `int.Parse("abc")` 같은 형식 오류 |
| `NullReferenceException` | `null` 객체의 멤버 접근 |
| `IndexOutOfRangeException` | 배열·리스트 범위 초과 접근 |
| `ArgumentNullException` | 매개변수로 `null`이 전달됨 |
| `FileNotFoundException` | 존재하지 않는 파일 접근 |
| `OverflowException` | 자료형의 최대값 초과 |

---

## 7. throw — 예외 직접 던지기

`throw`를 사용하면 코드에서 **의도적으로 예외를 발생**시킬 수 있습니다.  
잘못된 값이 들어왔을 때 호출한 쪽에 알려주는 데 사용합니다.

```csharp
using System;
class Hello
{
    static void SetAge(int age)
    {
        if (age < 0 || age > 150)
        {
            throw new ArgumentException($"나이는 0~150 사이여야 합니다. 입력값: {age}");
        }
        Console.WriteLine($"나이 설정 완료: {age}");
    }

    public static void Main()
    {
        try
        {
            SetAge(25);
            SetAge(-5);   // 예외 발생
            SetAge(30);   // 실행되지 않음
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"입력 오류: {ex.Message}");
        }
    }
}
```

**실행 결과**
```
나이 설정 완료: 25
입력 오류: 나이는 0~150 사이여야 합니다. 입력값: -5
```

---

## 8. 커스텀 예외 — 나만의 예외 만들기

`Exception`을 상속받아 **상황에 맞는 예외 클래스**를 직접 만들 수 있습니다.

```csharp
using System;

// 커스텀 예외 클래스
class InsufficientBalanceException : Exception
{
    public int Balance    { get; }
    public int Requested  { get; }

    public InsufficientBalanceException(int balance, int requested)
        : base($"잔액 부족! 현재 잔액: {balance}원, 요청 금액: {requested}원")
    {
        Balance   = balance;
        Requested = requested;
    }
}

class BankAccount
{
    public string Name    { get; }
    public int    Balance { get; private set; }

    public BankAccount(string name, int balance)
    {
        Name    = name;
        Balance = balance;
    }

    public void Withdraw(int amount)
    {
        if (amount > Balance)
        {
            throw new InsufficientBalanceException(Balance, amount);
        }
        Balance -= amount;
        Console.WriteLine($"{amount:N0}원 출금 완료. 잔액: {Balance:N0}원");
    }
}

class Hello
{
    public static void Main()
    {
        BankAccount account = new BankAccount("홍길동", 50000);

        try
        {
            account.Withdraw(30000);   // 정상
            account.Withdraw(40000);   // 잔액 부족 예외 발생
        }
        catch (InsufficientBalanceException ex)
        {
            Console.WriteLine($"출금 실패: {ex.Message}");
            Console.WriteLine($"부족한 금액: {ex.Requested - ex.Balance:N0}원");
        }
    }
}
```

**실행 결과**
```
30,000원 출금 완료. 잔액: 20,000원
출금 실패: 잔액 부족! 현재 잔액: 20000원, 요청 금액: 40000원
부족한 금액: 20,000원
```

> 💡 **Tip:** 커스텀 예외를 만들면 예외 이름 자체가 **상황을 설명**해 주어  
> 코드를 읽는 사람이 문제 원인을 훨씬 빠르게 파악할 수 있습니다.

---

## 🧪 예제 — 안전한 계산기

```csharp
using System;
class SafeCalculator
{
    public static double Divide(string aStr, string bStr)
    {
        int a, b;

        if (!int.TryParse(aStr, out a))
            throw new FormatException($"'{aStr}'은(는) 올바른 숫자가 아닙니다.");

        if (!int.TryParse(bStr, out b))
            throw new FormatException($"'{bStr}'은(는) 올바른 숫자가 아닙니다.");

        if (b == 0)
            throw new DivideByZeroException("나누는 수는 0이 될 수 없습니다.");

        return (double)a / b;
    }

    public static void Main()
    {
        string[][] testCases = {
            new[] { "10",  "3"   },
            new[] { "10",  "0"   },
            new[] { "abc", "3"   },
            new[] { "10",  "xyz" }
        };

        foreach (string[] tc in testCases)
        {
            Console.Write($"{tc[0]} / {tc[1]} = ");
            try
            {
                double result = Divide(tc[0], tc[1]);
                Console.WriteLine($"{result:F4}");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"형식 오류: {ex.Message}");
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine($"계산 오류: {ex.Message}");
            }
        }
    }
}
```

**실행 결과**
```
10 / 3 = 3.3333
10 / 0 = 계산 오류: 나누는 수는 0이 될 수 없습니다.
abc / 3 = 형식 오류: 'abc'은(는) 올바른 숫자가 아닙니다.
10 / xyz = 형식 오류: 'xyz'은(는) 올바른 숫자가 아닙니다.
```

---

## 🔍 핵심 개념 요약

| 키워드 | 역할 |
|---|---|
| `try` | 예외가 발생할 수 있는 코드 블록 |
| `catch` | 예외가 발생했을 때 처리 |
| `finally` | 예외 여부 관계없이 항상 실행 |
| `throw` | 의도적으로 예외 발생 |
| `Exception` | 모든 예외의 최상위 부모 클래스 |
| 커스텀 예외 | `Exception`을 상속해 직접 만든 예외 |

---

## 📝 문제

---

### 문제 1

다음 코드의 출력 결과는 무엇인가요?

```csharp
try
{
    Console.WriteLine("A");
    int x = int.Parse("hello");
    Console.WriteLine("B");
}
catch (FormatException)
{
    Console.WriteLine("C");
}
finally
{
    Console.WriteLine("D");
}
Console.WriteLine("E");
```

<details>
<summary>정답 보기</summary>

```
A
C
D
E
```

`int.Parse("hello")`에서 `FormatException`이 발생하여 `B`는 건너뛰고 `catch`로 이동합니다.  
`finally`는 항상 실행되고, 이후 코드도 정상 실행됩니다.

</details>

---

### 문제 2

`catch` 블록에서 예외 타입을 여러 개 쓸 때 순서가 중요한 이유를 설명하고,  
아래 코드에서 잘못된 점을 찾아 수정하세요.

```csharp
try { ... }
catch (Exception ex)       { Console.WriteLine("모든 예외"); }
catch (FormatException ex) { Console.WriteLine("형식 오류"); }
```

<details>
<summary>정답 보기</summary>

`Exception`이 모든 예외의 부모이기 때문에 `FormatException`이 발생해도 첫 번째 `catch`에서 먼저 잡힙니다.  
두 번째 `catch`는 **절대 실행되지 않습니다.**

**수정:**
```csharp
try { ... }
catch (FormatException ex) { Console.WriteLine("형식 오류"); }  // 구체적인 것 먼저
catch (Exception ex)       { Console.WriteLine("모든 예외"); }  // 일반적인 것 나중에
```

</details>

---

### 문제 3

`int.Parse()` 대신 `int.TryParse()`를 사용하는 장점은 무엇인가요?

<details>
<summary>정답 보기</summary>

`int.Parse()`는 변환에 실패하면 **예외를 던집니다.**  
`int.TryParse()`는 실패해도 예외 없이 **`false`를 반환**하고, 성공하면 `out` 매개변수로 값을 전달합니다.

```csharp
// Parse — 예외 처리 필요
try { int n = int.Parse("abc"); }
catch (FormatException) { ... }

// TryParse — 예외 없이 안전하게
if (int.TryParse("abc", out int n))
    Console.WriteLine(n);
else
    Console.WriteLine("변환 실패");
```

단순한 변환 실패 처리에는 `TryParse`가 더 간결하고 안전합니다.

</details>

---

### 문제 4

`ScoreException`이라는 커스텀 예외 클래스를 만들고,  
점수 입력 시 0~100 범위를 벗어나면 이 예외를 던지는 `SetScore()` 메서드를 작성하세요.

<details>
<summary>정답 보기</summary>

```csharp
class ScoreException : Exception
{
    public ScoreException(int score)
        : base($"유효하지 않은 점수입니다: {score} (0~100만 허용)")
    { }
}

void SetScore(int score)
{
    if (score < 0 || score > 100)
        throw new ScoreException(score);
    Console.WriteLine($"점수 설정: {score}");
}

try
{
    SetScore(85);
    SetScore(150);
}
catch (ScoreException ex)
{
    Console.WriteLine($"오류: {ex.Message}");
}
// 점수 설정: 85
// 오류: 유효하지 않은 점수입니다: 150 (0~100만 허용)
```

</details>

---

## 🗺️ 전체 커리큘럼 마무리

지금까지 16강에 걸쳐 C#의 핵심 개념을 모두 배웠습니다!

| 강의 | 주제 |
|---|---|
| 1~8강 | C# 기초 문법 (변수, 자료형, 연산자, 제어문) |
| 9~12강 | 객체지향 프로그래밍 (클래스, 캡슐화, 상속, 다형성) |
| 13~14강 | 입출력과 데이터 처리 (문자열 심화, 파일과 스트림) |
| 15~16강 | 실전 활용 (LINQ, 예외 처리) |

> 🎉 **수고하셨습니다!**  
> 이제 콘솔 프로그램의 기본 구조를 스스로 설계하고 만들 수 있는 수준이 되었습니다.  
> 다음 단계로는 **WinForms / WPF (데스크톱 UI)**, **ASP.NET (웹 개발)**,  
> 또는 **Unity (게임 개발)** 로 확장해 보세요. C#이 활약하는 분야입니다!

---

> 📌 **Tip:**
> - 예외 처리는 **"발생할 수 있는 모든 상황"** 을 고려하는 습관에서 시작합니다.
> - `catch` 블록은 **구체적인 예외 타입을 위에**, `Exception`을 **아래에** 배치하세요.
> - `finally`는 파일·연결처럼 **반드시 마무리해야 하는 자원 정리**에 사용합니다.
> - 커스텀 예외를 만들면 코드의 **의도와 오류 원인이 명확**해집니다.
> - `int.TryParse()`처럼 예외 없이 처리할 수 있는 메서드를 적극 활용하세요.
