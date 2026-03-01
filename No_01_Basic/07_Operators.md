# 🟣 C# 제7강 — 연산자 (Operators)

## 📌 개요
**연산자(Operator)** 는 값을 계산하거나 비교하고, 논리적인 판단을 수행하는 **기호**입니다.  
C#에는 산술, 대입, 비교, 논리, 증감, 비트, null 병합 등 다양한 종류의 연산자가 있습니다.

---

## 1. 산술 연산자 (Arithmetic Operators)

### 📌 기본 사칙연산

숫자 간의 **덧셈, 뺄셈, 곱셈, 나눗셈, 나머지** 계산에 사용합니다.

| 연산자 | 의미 | 예시 | 결과 |
|---|---|---|---|
| `+` | 덧셈 | `5 + 3` | `8` |
| `-` | 뺄셈 | `5 - 3` | `2` |
| `*` | 곱셈 | `5 * 3` | `15` |
| `/` | 나눗셈 | `5 / 2` | `2` (정수 나눗셈) |
| `%` | 나머지 | `5 % 2` | `1` |

> ⚠️ **주의:** 정수끼리 나눗셈(`/`)을 하면 소수점이 버려집니다.  
> 소수점까지 구하려면 피연산자 중 하나를 `double`로 변환해야 합니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int a = 10;
        int b = 3;

        Console.WriteLine(a + b);  // 13
        Console.WriteLine(a - b);  // 7
        Console.WriteLine(a * b);  // 30
        Console.WriteLine(a / b);  // 3  (소수점 버림)
        Console.WriteLine(a % b);  // 1  (나머지)

        // 소수점 결과가 필요할 때
        Console.WriteLine((double)a / b);  // 3.3333...
    }
}
```

**실행 결과**
```
13
7
30
3
1
3.3333333333333335
```

---

## 2. 대입 연산자 (Assignment Operators)

### 📌 기본 대입 연산자 `=`

`=` 는 오른쪽 값을 왼쪽 변수에 **저장(대입)** 합니다.

```csharp
int x = 10;  // 10을 x에 대입
```

### 📌 복합 대입 연산자

산술 연산과 대입을 **한 번에** 처리하는 축약형 연산자입니다.

| 연산자 | 풀어쓴 표현 | 의미 |
|---|---|---|
| `x += 5` | `x = x + 5` | x에 5를 더한 값을 x에 저장 |
| `x -= 5` | `x = x - 5` | x에서 5를 뺀 값을 x에 저장 |
| `x *= 5` | `x = x * 5` | x에 5를 곱한 값을 x에 저장 |
| `x /= 5` | `x = x / 5` | x를 5로 나눈 값을 x에 저장 |
| `x %= 5` | `x = x % 5` | x를 5로 나눈 나머지를 x에 저장 |

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int x = 10;

        x += 5;  Console.WriteLine(x);  // 15
        x -= 3;  Console.WriteLine(x);  // 12
        x *= 2;  Console.WriteLine(x);  // 24
        x /= 4;  Console.WriteLine(x);  // 6
        x %= 4;  Console.WriteLine(x);  // 2
    }
}
```

**실행 결과**
```
15
12
24
6
2
```

---

## 3. 증감 연산자 (Increment / Decrement Operators)

값을 **1씩 증가시키거나 감소**시킬 때 사용합니다.

| 연산자 | 의미 | 설명 |
|---|---|---|
| `++x` | 전위 증가 | 먼저 1 증가 후 사용 |
| `x++` | 후위 증가 | 먼저 사용 후 1 증가 |
| `--x` | 전위 감소 | 먼저 1 감소 후 사용 |
| `x--` | 후위 감소 | 먼저 사용 후 1 감소 |

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int a = 5;
        int b = 5;

        Console.WriteLine(++a);  // 6  (먼저 증가 후 출력)
        Console.WriteLine(b++);  // 5  (먼저 출력 후 증가)
        Console.WriteLine(b);    // 6  (증가된 b 출력)

        int c = 10;
        Console.WriteLine(--c);  // 9  (먼저 감소 후 출력)
        Console.WriteLine(c--);  // 9  (먼저 출력 후 감소)
        Console.WriteLine(c);    // 8  (감소된 c 출력)
    }
}
```

**실행 결과**
```
6
5
6
9
9
8
```

> 💡 **Tip:** 단순히 변수를 1 증가시킬 때는 `x++` 또는 `x += 1`을 사용합니다.  
> 전위/후위의 차이는 **다른 표현과 함께 쓸 때** 나타납니다.

---

## 4. 비교 연산자 (Comparison Operators)

두 값을 **비교**하여 `true` 또는 `false`(bool 값)를 반환합니다.

| 연산자 | 의미 | 예시 | 결과 |
|---|---|---|---|
| `==` | 같다 | `5 == 5` | `true` |
| `!=` | 같지 않다 | `5 != 3` | `true` |
| `>` | 크다 | `5 > 3` | `true` |
| `<` | 작다 | `5 < 3` | `false` |
| `>=` | 크거나 같다 | `5 >= 5` | `true` |
| `<=` | 작거나 같다 | `5 <= 3` | `false` |

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int a = 10;
        int b = 5;

        Console.WriteLine(a == b);   // False
        Console.WriteLine(a != b);   // True
        Console.WriteLine(a > b);    // True
        Console.WriteLine(a < b);    // False
        Console.WriteLine(a >= 10);  // True
        Console.WriteLine(b <= 5);   // True
    }
}
```

**실행 결과**
```
False
True
True
False
True
True
```

> ⚠️ **주의:** 같은지 비교할 때는 `==`를 사용합니다.  
> `=`는 **대입 연산자**이므로 혼동하지 마세요!

---

## 5. 논리 연산자 (Logical Operators)

`bool` 값(참/거짓)들을 **논리적으로 결합**할 때 사용합니다.

| 연산자 | 의미 | 설명 |
|---|---|---|
| `&&` | AND (그리고) | 두 조건이 **모두** `true`일 때 `true` |
| `\|\|` | OR (또는) | 두 조건 중 **하나라도** `true`이면 `true` |
| `!` | NOT (부정) | `true`면 `false`, `false`면 `true` |
| `^` | XOR (배타적 OR) | 두 조건이 **서로 다를 때**만 `true` |

### 📌 논리 연산 진리표

| A | B | A && B | A \|\| B | A ^ B | !A |
|---|---|---|---|---|---|
| true | true | true | true | **false** | false |
| true | false | false | true | **true** | false |
| false | true | false | true | **true** | true |
| false | false | false | false | **false** | true |

> 💡 **XOR(`^`) 핵심:** 두 값이 **같으면 false**, **다르면 true**

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int age    = 20;
        bool hasId = true;

        // && : 나이가 18 이상이고, 신분증도 있어야 입장 가능
        Console.WriteLine(age >= 18 && hasId);   // True

        // || : 둘 중 하나만 참이어도 됨
        Console.WriteLine(age >= 18 || hasId);   // True

        // ! : 조건 반전
        Console.WriteLine(!hasId);               // False

        // ^ : XOR — 두 조건이 서로 다를 때만 true
        bool a = true;
        bool b = false;
        Console.WriteLine(a ^ b);   // True  (서로 다름)
        Console.WriteLine(a ^ a);   // False (서로 같음)

        // 실용 예시: 둘 중 하나만 선택해야 하는 경우
        bool loginWithEmail  = true;
        bool loginWithGoogle = false;
        Console.WriteLine(loginWithEmail ^ loginWithGoogle);  // True (하나만 선택됨 → 정상)

        bool isRaining    = false;
        bool hasUmbrella  = true;
        Console.WriteLine(!isRaining || hasUmbrella);  // True
    }
}
```

**실행 결과**
```
True
True
False
True
False
True
True
```

---

## 6. 비트 연산자 (Bitwise Operators)

정수값을 **2진수(비트) 단위**로 직접 조작할 때 사용합니다.  
주로 **플래그 처리, 성능 최적화, 저수준 데이터 처리** 등에 활용됩니다.

| 연산자 | 의미 | 설명 |
|---|---|---|
| `&` | 비트 AND | 두 비트가 **모두 1**일 때만 1 |
| `\|` | 비트 OR | 두 비트 중 **하나라도 1**이면 1 |
| `^` | 비트 XOR | 두 비트가 **서로 다를 때**만 1 |
| `~` | 비트 NOT | 모든 비트를 **반전** (0→1, 1→0) |
| `<<` | 왼쪽 시프트 | 비트를 왼쪽으로 이동 (2배 곱셈 효과) |
| `>>` | 오른쪽 시프트 | 비트를 오른쪽으로 이동 (2로 나누기 효과) |

### 📌 비트 연산 예시 (a=12, b=10)

```
a = 12  →  0000 1100
b = 10  →  0000 1010

a & b   →  0000 1000  =  8   (둘 다 1인 비트만)
a | b   →  0000 1110  = 14   (하나라도 1인 비트)
a ^ b   →  0000 0110  =  6   (서로 다른 비트만)
~a      →  1111 0011  = -13  (모든 비트 반전)
a << 1  →  0001 1000  = 24   (왼쪽으로 1칸 이동 = ×2)
a >> 1  →  0000 0110  =  6   (오른쪽으로 1칸 이동 = ÷2)
```

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int a = 12;  // 이진수: 1100
        int b = 10;  // 이진수: 1010

        Console.WriteLine($"a & b  = {a & b}");   //  8  (1000)
        Console.WriteLine($"a | b  = {a | b}");   // 14  (1110)
        Console.WriteLine($"a ^ b  = {a ^ b}");   //  6  (0110)
        Console.WriteLine($"~a     = {~a}");       // -13
        Console.WriteLine($"a << 1 = {a << 1}");  // 24  (×2)
        Console.WriteLine($"a >> 1 = {a >> 1}");  //  6  (÷2)
    }
}
```

**실행 결과**
```
a & b  = 8
a | b  = 14
a ^ b  = 6
~a     = -13
a << 1 = 24
a >> 1 = 6
```

> 💡 **Tip:**
> - `<<` 1번 = ×2, `<<` 2번 = ×4 (2의 거듭제곱 곱셈을 빠르게 처리)
> - `>>` 1번 = ÷2, `>>` 2번 = ÷4
> - `~a`의 결과: 정수 `a`에 대해 `~a = -(a + 1)` 공식이 성립합니다.

### 📌 복합 비트 대입 연산자

| 연산자 | 풀어쓴 표현 |
|---|---|
| `x &= y` | `x = x & y` |
| `x \|= y` | `x = x \| y` |
| `x ^= y` | `x = x ^ y` |
| `x <<= n` | `x = x << n` |
| `x >>= n` | `x = x >> n` |

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int flags = 0b0000;  // 이진 리터럴

        flags |= 0b0001;  Console.WriteLine($"비트0 켜기: {Convert.ToString(flags, 2).PadLeft(4,'0')}");  // 0001
        flags |= 0b0100;  Console.WriteLine($"비트2 켜기: {Convert.ToString(flags, 2).PadLeft(4,'0')}");  // 0101
        flags &= ~0b0001; Console.WriteLine($"비트0 끄기: {Convert.ToString(flags, 2).PadLeft(4,'0')}");  // 0100
        flags ^= 0b0110;  Console.WriteLine($"비트1,2 토글: {Convert.ToString(flags, 2).PadLeft(4,'0')}");// 0010
    }
}
```

**실행 결과**
```
비트0 켜기: 0001
비트2 켜기: 0101
비트0 끄기: 0100
비트1,2 토글: 0010
```

---

## 7. 문자열 연결 연산자 (`+`)

`+` 연산자는 **문자열끼리** 또는 **문자열과 다른 자료형**을 이어 붙일 때도 사용합니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string firstName = "길동";
        string lastName  = "홍";
        int    age       = 25;

        Console.WriteLine(lastName + firstName);           // 홍길동
        Console.WriteLine("나이: " + age + "세");          // 나이: 25세
        Console.WriteLine($"이름: {lastName}{firstName}"); // 이름: 홍길동
    }
}
```

**실행 결과**
```
홍길동
나이: 25세
이름: 홍길동
```

> 💡 **Tip:** 문자열과 다른 자료형을 `+`로 연결할 때, 다른 자료형은 자동으로 문자열로 변환됩니다.  
> 가독성을 위해 **문자열 보간(`$"..."`)** 을 권장합니다.

---

## 8. 삼항 연산자 (Ternary Operator)

`if-else` 구조를 **한 줄로** 표현하는 연산자입니다.

```
조건 ? 참일 때 값 : 거짓일 때 값
```

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int score = 75;

        // if-else 방식
        // if (score >= 60) Console.WriteLine("합격"); else Console.WriteLine("불합격");

        // 삼항 연산자 방식
        string result = score >= 60 ? "합격" : "불합격";
        Console.WriteLine(result);  // 합격

        int a = 10, b = 20;
        int max = a > b ? a : b;
        Console.WriteLine($"최댓값: {max}");  // 최댓값: 20
    }
}
```

**실행 결과**
```
합격
최댓값: 20
```

---

## 9. null 관련 연산자 (Null Operators)

C#에서 변수가 **null(값 없음)** 일 때를 안전하게 처리하는 연산자들입니다.

### 📌 null 병합 연산자 `??`

왼쪽 값이 `null`이면 오른쪽 값을 사용하고, `null`이 아니면 왼쪽 값을 그대로 사용합니다.

```
값 ?? null일 때 대체값
```

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string name = null;

        // name이 null이면 "이름 없음" 사용
        string displayName = name ?? "이름 없음";
        Console.WriteLine(displayName);  // 이름 없음

        string name2 = "홍길동";
        string displayName2 = name2 ?? "이름 없음";
        Console.WriteLine(displayName2);  // 홍길동

        // 숫자 nullable 타입과도 사용 가능
        int? score = null;
        int result = score ?? 0;  // null이면 0으로 처리
        Console.WriteLine($"점수: {result}");  // 점수: 0
    }
}
```

**실행 결과**
```
이름 없음
홍길동
점수: 0
```

### 📌 null 병합 대입 연산자 `??=`

변수가 `null`일 때만 값을 대입합니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string name = null;
        name ??= "기본 이름";        // name이 null이므로 대입됨
        Console.WriteLine(name);    // 기본 이름

        string name2 = "홍길동";
        name2 ??= "기본 이름";       // name2가 null이 아니므로 무시됨
        Console.WriteLine(name2);   // 홍길동
    }
}
```

**실행 결과**
```
기본 이름
홍길동
```

### 📌 null 조건부 연산자 `?.`

객체가 `null`이면 전체 식을 `null`로 처리하여 **NullReferenceException을 방지**합니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string text = null;

        // null 조건부 없이 접근하면 예외 발생!
        // int len = text.Length;  // ❌ NullReferenceException

        // ?. 를 사용하면 null을 안전하게 처리
        int? len = text?.Length;
        Console.WriteLine(len);  // (출력 없음, null)

        string text2 = "Hello";
        int? len2 = text2?.Length;
        Console.WriteLine(len2);  // 5

        // ?? 와 함께 사용하면 기본값 지정 가능
        int safeLen = text?.Length ?? 0;
        Console.WriteLine($"안전한 길이: {safeLen}");  // 안전한 길이: 0
    }
}
```

**실행 결과**
```

5
안전한 길이: 0
```

> 💡 **Tip:** `?.`와 `??`를 조합하면 null 처리를 한 줄로 안전하게 할 수 있습니다.

---

## 10. 연산자 우선순위

여러 연산자가 한 식에 있을 때, **우선순위가 높은 연산자**부터 계산됩니다.

| 우선순위 | 연산자 | 설명 |
|---|---|---|
| 1 (높음) | `++`, `--`, `!`, `~` | 증감, 논리 NOT, 비트 NOT |
| 2 | `*`, `/`, `%` | 곱셈, 나눗셈, 나머지 |
| 3 | `+`, `-` | 덧셈, 뺄셈 |
| 4 | `<<`, `>>` | 비트 시프트 |
| 5 | `>`, `<`, `>=`, `<=` | 크기 비교 |
| 6 | `==`, `!=` | 동등 비교 |
| 7 | `&` | 비트 AND |
| 8 | `^` | 비트/논리 XOR |
| 9 | `\|` | 비트 OR |
| 10 | `&&` | 논리 AND |
| 11 | `\|\|` | 논리 OR |
| 12 | `??` | null 병합 |
| 13 (낮음) | `=`, `+=`, `-=`, `??=` 등 | 대입 |

> 💡 **Tip:** 우선순위가 헷갈릴 때는 **괄호`()`** 를 사용하면 명확하고 안전합니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int result1 = 2 + 3 * 4;          // 14  (곱셈 먼저)
        int result2 = (2 + 3) * 4;        // 20  (괄호 먼저)

        bool check1 = 5 > 3 && 10 > 7;   // True  (비교 후 AND)
        bool check2 = 5 > 3 || 10 < 7;   // True  (True || False)

        // XOR 우선순위 확인
        bool check3 = true || false ^ true;  // true || (false ^ true) = true || true = true
        bool check4 = (true || false) ^ true; // true ^ true = false

        Console.WriteLine(result1);  // 14
        Console.WriteLine(result2);  // 20
        Console.WriteLine(check1);   // True
        Console.WriteLine(check2);   // True
        Console.WriteLine(check3);   // True
        Console.WriteLine(check4);   // False
    }
}
```

**실행 결과**
```
14
20
True
True
True
False
```

---

## 🧪 예제

### 예제 1 — 사칙연산 계산기

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int a = 17;
        int b = 5;

        Console.WriteLine($"{a} + {b} = {a + b}");
        Console.WriteLine($"{a} - {b} = {a - b}");
        Console.WriteLine($"{a} * {b} = {a * b}");
        Console.WriteLine($"{a} / {b} = {a / b}");
        Console.WriteLine($"{a} % {b} = {a % b}");
        Console.WriteLine($"{a} / {b} (실수) = {(double)a / b:F2}");
    }
}
```

**실행 결과**
```
17 + 5 = 22
17 - 5 = 12
17 * 5 = 85
17 / 5 = 3
17 % 5 = 2
17 / 5 (실수) = 3.40
```

---

### 예제 2 — 성적 판정 (비교 + 논리 + 삼항 연산자)

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int score = 88;

        bool isPassed    = score >= 60;
        bool isExcellent = score >= 90;

        Console.WriteLine($"점수: {score}점");
        Console.WriteLine($"합격 여부: {(isPassed ? "합격" : "불합격")}");
        Console.WriteLine($"우수 여부: {(isExcellent ? "우수" : "보통")}");
        Console.WriteLine($"60점 이상이고 90점 미만: {isPassed && !isExcellent}");
    }
}
```

**실행 결과**
```
점수: 88점
합격 여부: 합격
우수 여부: 보통
60점 이상이고 90점 미만: True
```

---

### 예제 3 — 복합 대입 및 증감 연산자

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int count = 0;

        count++;     Console.WriteLine($"count++  : {count}");  // 1
        count++;     Console.WriteLine($"count++  : {count}");  // 2
        count += 5;  Console.WriteLine($"count+=5 : {count}"); // 7
        count -= 3;  Console.WriteLine($"count-=3 : {count}"); // 4
        count *= 2;  Console.WriteLine($"count*=2 : {count}"); // 8
        count /= 4;  Console.WriteLine($"count/=4 : {count}"); // 2
    }
}
```

**실행 결과**
```
count++  : 1
count++  : 2
count+=5 : 7
count-=3 : 4
count*=2 : 8
count/=4 : 2
```

---

### 예제 4 — XOR 활용 (값 교환)

XOR의 재미있는 특성: **임시 변수 없이 두 변수 값 교환**이 가능합니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int x = 10;
        int y = 20;

        Console.WriteLine($"교환 전: x={x}, y={y}");

        // XOR을 이용한 값 교환 (임시 변수 불필요)
        x ^= y;  // x = x ^ y
        y ^= x;  // y = y ^ x  (원래 x값이 됨)
        x ^= y;  // x = x ^ y  (원래 y값이 됨)

        Console.WriteLine($"교환 후: x={x}, y={y}");
    }
}
```

**실행 결과**
```
교환 전: x=10, y=20
교환 후: x=20, y=10
```

---

### 예제 5 — null 연산자 종합

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string input = null;

        // ?? : null이면 기본값 사용
        string name = input ?? "익명";
        Console.WriteLine($"이름: {name}");  // 이름: 익명

        // ??= : null일 때만 대입
        input ??= "기본값";
        Console.WriteLine($"input: {input}");  // input: 기본값

        // ?. : null이면 예외 없이 null 반환
        string text = null;
        Console.WriteLine(text?.ToUpper() ?? "텍스트 없음");  // 텍스트 없음

        string text2 = "hello";
        Console.WriteLine(text2?.ToUpper() ?? "텍스트 없음");  // HELLO
    }
}
```

**실행 결과**
```
이름: 익명
input: 기본값
텍스트 없음
HELLO
```

---

## 🔍 연산자 종류 요약

| 분류 | 연산자 | 설명 |
|---|---|---|
| 산술 | `+` `-` `*` `/` `%` | 사칙연산 및 나머지 |
| 대입 | `=` `+=` `-=` `*=` `/=` `%=` | 값 저장 |
| 증감 | `++` `--` | 1씩 증가/감소 |
| 비교 | `==` `!=` `>` `<` `>=` `<=` | 참/거짓 반환 |
| 논리 | `&&` `\|\|` `!` `^` | 조건 결합, XOR |
| 비트 | `&` `\|` `^` `~` `<<` `>>` | 비트 단위 연산 |
| 삼항 | `? :` | 조건에 따른 값 선택 |
| null | `??` `??=` `?.` | null 안전 처리 |
| 문자열 연결 | `+` | 문자열 이어 붙이기 |

---

## 📝 문제

---

### 문제 1

다음 코드의 출력 결과는 무엇인가요?

```csharp
int a = 10;
int b = 3;
Console.WriteLine(a / b);
Console.WriteLine(a % b);
```

<details>
<summary>정답 보기</summary>

```
3
1
```

정수끼리 나눗셈은 소수점을 버린 몫만 반환하고, `%`는 나머지를 반환합니다.

</details>

---

### 문제 2

다음 코드의 출력 결과는 무엇인가요?

```csharp
int x = 5;
Console.WriteLine(x++);
Console.WriteLine(x);
Console.WriteLine(++x);
```

<details>
<summary>정답 보기</summary>

```
5
6
7
```

`x++`는 출력 후 증가, `++x`는 증가 후 출력입니다.

</details>

---

### 문제 3

다음 조건식의 결과값(`true` / `false`)을 쓰세요.

```
① 10 > 5 && 3 < 2
② 10 > 5 || 3 < 2
③ !(10 == 10)
④ 7 >= 7 && 5 != 3
⑤ true ^ true
⑥ true ^ false
```

<details>
<summary>정답 보기</summary>

```
① false  (true && false → false)
② true   (true || false → true)
③ false  (!(true) → false)
④ true   (true && true → true)
⑤ false  (같은 값 XOR → false)
⑥ true   (다른 값 XOR → true)
```

</details>

---

### 문제 4

삼항 연산자를 사용하여 `num`이 짝수이면 `"짝수"`, 홀수이면 `"홀수"`를 출력하는 코드를 완성하세요.

```csharp
int num = 7;
string result = ________;
Console.WriteLine(result);
```

<details>
<summary>정답 보기</summary>

```csharp
string result = num % 2 == 0 ? "짝수" : "홀수";
```

`num % 2 == 0`이면 짝수, 그렇지 않으면 홀수입니다.

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
        int a = 10;
        int b = 3;

        if (a = b)           // 조건 비교
            Console.WriteLine("같다")
        Console.WriteLine(a && b);  // 두 수 모두 양수인지 확인
    }
}
```

<details>
<summary>정답 보기</summary>

① `a = b` → `a == b` (`=`은 대입, `==`이 비교 연산자)  
② `Console.WriteLine("같다")` → 세미콜론(`;`) 누락  
③ `a && b` → `a > 0 && b > 0` (`&&`는 bool 값끼리만 사용 가능하며, int에 직접 사용 불가)

</details>

---

### 문제 6 (신규)

다음 비트 연산의 결과를 쓰세요. (a = 6, b = 3 일 때)

```
① a & b
② a | b
③ a ^ b
④ a << 1
⑤ a >> 1
```

<details>
<summary>정답 보기</summary>

```
a = 6  →  0110
b = 3  →  0011

① a & b  = 0010 = 2  (둘 다 1인 비트만)
② a | b  = 0111 = 7  (하나라도 1인 비트)
③ a ^ b  = 0101 = 5  (서로 다른 비트만)
④ a << 1 = 1100 = 12 (×2)
⑤ a >> 1 = 0011 = 3  (÷2)
```

</details>

---

### 문제 7 (신규)

다음 코드의 출력 결과를 쓰세요.

```csharp
string a = null;
string b = "안녕";

Console.WriteLine(a ?? "기본값");
Console.WriteLine(b ?? "기본값");
a ??= "새 값";
Console.WriteLine(a);
```

<details>
<summary>정답 보기</summary>

```
기본값
안녕
새 값
```

`??`는 null일 때 오른쪽 값을 사용하고, `??=`는 null일 때만 대입합니다.

</details>

---

> 📌 **핵심 정리:**
> - **산술 연산**에서 정수 나눗셈은 소수점이 버려집니다. 소수 결과가 필요하면 `(double)`로 형변환하세요.
> - **비교 연산자**는 항상 `bool` 값(`true` / `false`)을 반환합니다.
> - **논리 XOR(`^`)** 은 두 값이 **서로 다를 때만** `true`입니다.
> - **비트 연산자**는 2진수 단위로 조작하며, `<<`/`>>`는 2의 거듭제곱 배수 연산에 유용합니다.
> - **null 연산자** `??`, `??=`, `?.`는 null 처리를 안전하고 간결하게 해줍니다.
> - **삼항 연산자**는 간단한 조건 분기를 한 줄로 표현할 때 유용합니다.
> - 우선순위가 헷갈릴 때는 **괄호`()`** 를 적극 활용하세요.
