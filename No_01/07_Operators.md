# 🟣 C# 제7강 — 연산자 (Operators)

## 📌 개요
**연산자(Operator)** 는 값을 계산하거나 비교하고, 논리적인 판단을 수행하는 **기호**입니다.  
C#에는 산술, 대입, 비교, 논리, 증감 등 다양한 종류의 연산자가 있습니다.

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

### 📌 논리 연산 진리표

| A | B | A && B | A \|\| B | !A |
|---|---|---|---|---|
| true | true | true | true | false |
| true | false | false | true | false |
| false | true | false | true | true |
| false | false | false | false | true |

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int age   = 20;
        bool hasId = true;

        // && : 나이가 18 이상이고, 신분증도 있어야 입장 가능
        Console.WriteLine(age >= 18 && hasId);   // True

        // || : 둘 중 하나만 참이어도 됨
        Console.WriteLine(age >= 18 || hasId);   // True

        // ! : 조건 반전
        Console.WriteLine(!hasId);               // False

        bool isRaining = false;
        bool hasUmbrella = true;
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
```

---

## 6. 문자열 연결 연산자 (`+`)

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

## 7. 삼항 연산자 (Ternary Operator)

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

## 8. 연산자 우선순위

여러 연산자가 한 식에 있을 때, **우선순위가 높은 연산자**부터 계산됩니다.

| 우선순위 | 연산자 | 설명 |
|---|---|---|
| 1 (높음) | `++`, `--`, `!` | 증감, 논리 NOT |
| 2 | `*`, `/`, `%` | 곱셈, 나눗셈, 나머지 |
| 3 | `+`, `-` | 덧셈, 뺄셈 |
| 4 | `>`, `<`, `>=`, `<=` | 크기 비교 |
| 5 | `==`, `!=` | 동등 비교 |
| 6 | `&&` | 논리 AND |
| 7 | `\|\|` | 논리 OR |
| 8 (낮음) | `=`, `+=`, `-=` 등 | 대입 |

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

        Console.WriteLine(result1);  // 14
        Console.WriteLine(result2);  // 20
        Console.WriteLine(check1);   // True
        Console.WriteLine(check2);   // True
    }
}
```

**실행 결과**
```
14
20
True
True
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

        bool isPassed  = score >= 60;
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

        count++;     Console.WriteLine($"count++ : {count}");  // 1
        count++;     Console.WriteLine($"count++ : {count}");  // 2
        count += 5;  Console.WriteLine($"count+=5 : {count}"); // 7
        count -= 3;  Console.WriteLine($"count-=3 : {count}"); // 4
        count *= 2;  Console.WriteLine($"count*=2 : {count}"); // 8
        count /= 4;  Console.WriteLine($"count/=4 : {count}"); // 2
    }
}
```

**실행 결과**
```
count++ : 1
count++ : 2
count+=5 : 7
count-=3 : 4
count*=2 : 8
count/=4 : 2
```

---

## 🔍 연산자 종류 요약

| 분류 | 연산자 | 설명 |
|---|---|---|
| 산술 | `+` `-` `*` `/` `%` | 사칙연산 및 나머지 |
| 대입 | `=` `+=` `-=` `*=` `/=` `%=` | 값 저장 |
| 증감 | `++` `--` | 1씩 증가/감소 |
| 비교 | `==` `!=` `>` `<` `>=` `<=` | 참/거짓 반환 |
| 논리 | `&&` `\|\|` `!` | 조건 결합 |
| 삼항 | `? :` | 조건에 따른 값 선택 |
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
```

<details>
<summary>정답 보기</summary>

```
① false  (true && false → false)
② true   (true || false → true)
③ false  (!(true) → false)
④ true   (true && true → true)
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

> 📌 **Tip:**
> - **산술 연산**에서 정수 나눗셈은 소수점이 버려집니다. 소수 결과가 필요하면 `(double)`로 형변환하세요.
> - **비교 연산자**는 항상 `bool` 값(`true` / `false`)을 반환합니다.
> - **논리 연산자** `&&`와 `||`는 `bool` 값에만 사용합니다.
> - **삼항 연산자**는 간단한 조건 분기를 한 줄로 표현할 때 유용합니다.
> - 우선순위가 헷갈릴 때는 **괄호`()`** 를 적극 활용하세요.
