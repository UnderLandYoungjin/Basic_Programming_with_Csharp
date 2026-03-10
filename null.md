# 🟣 C# 제2강 — 변수 (Variable)

## 📌 개요
**변수(Variable)** 란 데이터를 저장하기 위한 **메모리 공간에 붙여진 이름**입니다.
프로그램에서 숫자, 문자 등 다양한 값을 임시로 저장하고 사용할 때 변수를 활용합니다.

---

## 1. 변수의 선언과 대입

### 📌 변수 선언
변수를 사용하려면 먼저 **어떤 종류의 데이터를 저장할지(자료형)** 와 **변수의 이름**을 정해야 합니다.

```
자료형 변수이름;
```

```csharp
int age;        // 정수형 변수 age 선언
double height;  // 실수형 변수 height 선언
```

### 📌 값의 대입 (Assignment)
선언한 변수에 `=` 기호를 사용하여 값을 저장합니다.

```
변수이름 = 값;
```

```csharp
age = 20;       // age 변수에 20을 대입
height = 175.5; // height 변수에 175.5를 대입
```

### 📌 선언과 동시에 대입 (초기화)
선언과 대입을 한 줄에 작성할 수 있습니다.

```
자료형 변수이름 = 값;
```

```csharp
int age = 20;
double height = 175.5;
```

---

## 2. 변수의 표시 (출력)

변수에 저장된 값을 화면에 출력하려면 `Console.WriteLine()` 을 사용합니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int age = 20;
        double height = 175.5;

        Console.WriteLine(age);      // 변수 값 출력: 20
        Console.WriteLine(height);   // 변수 값 출력: 175.5
    }
}
```

**실행 결과**
```
20
175.5
```

### 📌 문자열과 변수를 함께 출력하기

#### 방법 1 — `+` 연산자로 연결 ✅ 사용

```csharp
Console.WriteLine("나이: " + age);
Console.WriteLine("키: " + height);
```

**실행 결과**
```
나이: 20
키: 175.5
```

---

## 3. 변수의 선언 방법 규칙

| 규칙 | 설명 | 예시 |
|---|---|---|
| ✅ 영문자, 숫자, `_` 사용 가능 | 변수 이름에 사용 가능한 문자 | `myAge`, `score_1` |
| ✅ 숫자로 시작 불가 | 첫 글자는 영문자 또는 `_` | `age1` (O), `1age` (X) |
| ✅ 대소문자 구분 | `Age` 와 `age` 는 다른 변수 | `Age ≠ age` |
| ✅ 예약어 사용 불가 | C# 키워드는 이름으로 사용 불가 | `int`, `class`, `void` 등 |
| ✅ 의미 있는 이름 권장 | 변수의 용도를 알 수 있도록 | `score` (O), `s` (비권장) |

```csharp
// 올바른 변수 이름
int score = 100;
double my_height = 175.5;
int _count = 0;

// 잘못된 변수 이름 (컴파일 오류 발생)
// int 1score = 100;   // 숫자로 시작 불가
// int int = 10;       // 예약어 사용 불가
```

---

## 4. 정수형 변수 (Integer Types)

정수(소수점 없는 숫자)를 저장하는 자료형입니다.

| 형의 이름 | 읽는 방법 | 크기 (비트) | 값의 범위 |
|---|---|---|---|
| `sbyte` | 에스바이트 | 8 bit | -128 ~ 127 |
| `byte` | 바이트 | 8 bit | 0 ~ 255 |
| `short` | 쇼트 | 16 bit | -32,768 ~ 32,767 |
| `ushort` | 유쇼트 | 16 bit | 0 ~ 65,535 |
| `int` | 인트 | 32 bit | -2,147,483,648 ~ 2,147,483,647 |
| `uint` | 유인트 | 32 bit | 0 ~ 4,294,967,295 |
| `long` | 롱 | 64 bit | -9,223,372,036,854,775,808 ~ 9,223,372,036,854,775,807 |
| `ulong` | 유롱 | 64 bit | 0 ~ 18,446,744,073,709,551,615 |

> 💡 **Tip:** 일반적으로 정수를 저장할 때는 **`int`** 를 가장 많이 사용합니다.
> 앞에 `u`가 붙으면 **부호 없는(Unsigned)** 정수형으로 음수를 저장할 수 없는 대신 더 큰 양수를 저장할 수 있습니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        sbyte  a = -100;
        byte   b = 200;
        short  c = -30000;
        ushort d = 60000;
        int    e = -2000000000;
        uint   f = 4000000000;
        long   g = -9000000000000000000;
        ulong  h = 18000000000000000000;

        Console.WriteLine("sbyte  : " + a);
        Console.WriteLine("byte   : " + b);
        Console.WriteLine("short  : " + c);
        Console.WriteLine("ushort : " + d);
        Console.WriteLine("int    : " + e);
        Console.WriteLine("uint   : " + f);
        Console.WriteLine("long   : " + g);
        Console.WriteLine("ulong  : " + h);
    }
}
```

**실행 결과**
```
sbyte  : -100
byte   : 200
short  : -30000
ushort : 60000
int    : -2000000000
uint   : 4000000000
long   : -9000000000000000000
ulong  : 18000000000000000000
```

---

## 5. 실수형 변수 (Floating-Point Types)

소수점이 있는 숫자(실수)를 저장하는 자료형입니다.

| 형의 이름 | 읽는 방법 | 크기 (비트) | 값의 범위 | 정밀도 (유효 자릿수) |
|---|---|---|---|---|
| `float` | 플로트 | 32 bit | ±1.5 × 10⁻⁴⁵ ~ ±3.4 × 10³⁸ | 약 7자리 |
| `double` | 더블 | 64 bit | ±5.0 × 10⁻³²⁴ ~ ±1.7 × 10³⁰⁸ | 약 15~16자리 |
| `decimal` | 데시말 | 128 bit | ±1.0 × 10⁻²⁸ ~ ±7.9 × 10²⁸ | 약 28~29자리 |

> 💡 **Tip:**
> - 일반적인 실수 계산에는 **`double`** 을 가장 많이 사용합니다.
> - **`float`** 값은 끝에 `f` 또는 `F` 를 붙여야 합니다. (예: `3.14f`)
> - **`decimal`** 은 금융·회계처럼 정밀한 소수점 계산이 필요할 때 사용하며 끝에 `m` 또는 `M` 을 붙입니다. (예: `3.14m`)

```csharp
using System;
class Hello
{
    public static void Main()
    {
        float   a = 3.14f;
        double  b = 3.141592653589;
        decimal c = 3.14159265358979323846m;

        Console.WriteLine("float   : " + a);
        Console.WriteLine("double  : " + b);
        Console.WriteLine("decimal : " + c);
    }
}
```

**실행 결과**
```
float   : 3.14
double  : 3.141592653589
decimal : 3.14159265358979323846
```

---

## 🧪 예제

### 예제 1 — 변수 선언, 대입, 출력

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int    score  = 95;
        double weight = 68.5;

        Console.WriteLine("점수 : " + score + "점");
        Console.WriteLine("몸무게 : " + weight + "kg");
    }
}
```

**실행 결과**
```
점수 : 95점
몸무게 : 68.5kg
```

---

### 예제 2 — 변수 값 변경

변수는 값을 나중에 바꿀 수 있습니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int count = 0;
        Console.WriteLine("초기값 : " + count);

        count = 10;
        Console.WriteLine("변경 후 : " + count);

        count = count + 5;
        Console.WriteLine("5 더한 후 : " + count);
    }
}
```

**실행 결과**
```
초기값 : 0
변경 후 : 10
5 더한 후 : 15
```

---

### 예제 3 — 다양한 자료형 함께 사용

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string name    = "홍길동";
        int    age     = 25;
        double height  = 172.3;
        float  weight  = 68.5f;

        Console.WriteLine("=== 개인 정보 ===");
        Console.WriteLine("이름   : " + name);
        Console.WriteLine("나이   : " + age + "세");
        Console.WriteLine("키     : " + height + "cm");
        Console.WriteLine("몸무게 : " + weight + "kg");
    }
}
```

**실행 결과**
```
=== 개인 정보 ===
이름   : 홍길동
나이   : 25세
키     : 172.3cm
몸무게 : 68.5kg
```

---

## 🔍 자료형 요약 정리

| 구분 | 형의 이름 | 읽는 방법 | 크기 (비트) | 값의 범위 / 특징 |
|---|---|---|---|---|
| 정수형 | `sbyte` | 에스바이트 | 8 | -128 ~ 127 |
| 정수형 | `byte` | 바이트 | 8 | 0 ~ 255 |
| 정수형 | `short` | 쇼트 | 16 | -32,768 ~ 32,767 |
| 정수형 | `ushort` | 유쇼트 | 16 | 0 ~ 65,535 |
| 정수형 | `int` | 인트 | 32 | 약 ±21억 |
| 정수형 | `uint` | 유인트 | 32 | 0 ~ 약 42억 |
| 정수형 | `long` | 롱 | 64 | 약 ±9.2 × 10¹⁸ |
| 정수형 | `ulong` | 유롱 | 64 | 0 ~ 약 1.8 × 10¹⁹ |
| 실수형 | `float` | 플로트 | 32 | 약 7자리 정밀도, 리터럴에 `f` 필요 |
| 실수형 | `double` | 더블 | 64 | 약 15~16자리 정밀도, 기본 실수형 |
| 실수형 | `decimal` | 데시말 | 128 | 약 28~29자리 정밀도, 금융 계산용, 리터럴에 `m` 필요 |

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
        int x = 10;
        int y = 20;
        Console.WriteLine(x + y);
    }
}
```

<details>
<summary>정답 보기</summary>

```
30
```

</details>

---

### 문제 2

다음 중 올바른 변수 선언은 무엇인가요?

```
① int 1score = 100;
② int score_1 = 100;
③ int int = 100;
④ int score 1 = 100;
```

<details>
<summary>정답 보기</summary>

② `int score_1 = 100;` — 영문자, 숫자, 언더바(`_`)만 사용 가능하고 숫자로 시작하지 않으며 예약어가 아닌 것

</details>

---

### 문제 3

`float` 형 변수를 선언하고 `3.14` 를 저장하려고 합니다. 빈칸을 채우세요.

```csharp
float pi = ________;
```

<details>
<summary>정답 보기</summary>

```csharp
float pi = 3.14f;
```

</details>

---

### 문제 4

`int` 형으로 저장할 수 없는 값은 무엇인가요?

```
① 0
② -2147483648
③ 2147483647
④ 3000000000
```

<details>
<summary>정답 보기</summary>

④ `3000000000` — `int`의 최댓값은 약 21억(2,147,483,647)이므로 30억은 범위를 초과합니다. `long`을 사용해야 합니다.

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
        Int age = 20
        float height = 175.5;
        Console.WriteLine("나이: " age);
    }
}
```

<details>
<summary>정답 보기</summary>

① `Int` → `int` (C#의 기본 자료형은 소문자)  
② `age = 20` 뒤에 세미콜론(`;`) 누락  
③ `float height = 175.5;` → `float height = 175.5f;` (float 리터럴에는 `f`를 붙여야 함)  
④ `Console.WriteLine("나이: " age);` → `Console.WriteLine("나이: " + age);` (문자열 연결 연산자 `+` 누락)

</details>

---

> 📌 **Tip:**
> - 일반적인 **정수**를 저장할 때는 **`int`** 를 사용하세요.
> - 일반적인 **실수**를 저장할 때는 **`double`** 을 사용하세요.
> - **`float`** 은 반드시 숫자 뒤에 **`f`** 를 붙이세요. (예: `3.14f`)
> - **`decimal`** 은 반드시 숫자 뒤에 **`m`** 을 붙이세요. (예: `3.14m`)

---

## 📎 보충 — 변수의 유효 범위 (Scope)

변수는 **어디에서 선언하느냐**에 따라 사용할 수 있는 범위가 달라집니다.
이것을 **유효 범위(Scope)** 라고 합니다.

---

### 📌 지역 변수 (Local Variable)

**메서드(함수) 안에서 선언한 변수**를 지역 변수라고 합니다.
지역 변수는 선언된 `{ }` 블록 안에서만 사용할 수 있고, 블록이 끝나면 사라집니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int score = 100;  // 지역 변수: Main() 안에서만 사용 가능
        Console.WriteLine("점수 : " + score);
    }
    // 여기서는 score를 사용할 수 없음
}
```

> 지금까지 우리가 `Main()` 안에서 선언한 변수는 **모두 지역 변수**입니다.

---

### 📌 클래스 필드 (Class Field) — "전역 변수"처럼 사용

C#에는 다른 언어(C, Python 등)처럼 어디서든 접근 가능한 "전역 변수"가 없습니다.
대신, **클래스 안에서 메서드 바깥에 선언한 변수**를 **필드(Field)** 라고 하며,
`static` 키워드를 붙이면 같은 클래스의 모든 메서드에서 공유할 수 있습니다.

```csharp
using System;
class Hello
{
    static int totalScore = 0;  // 클래스 필드: 모든 메서드에서 사용 가능

    public static void Main()
    {
        totalScore = 80;
        AddBonus();
        Console.WriteLine("최종 점수 : " + totalScore);
    }

    public static void AddBonus()
    {
        totalScore = totalScore + 20;  // Main()에서 선언하지 않았지만 사용 가능
    }
}
```

**실행 결과**
```
최종 점수 : 100
```

> 💡 `totalScore`는 `Main()` 안이 아니라 **클래스 바로 아래**에 선언되어 있으므로,
> `Main()`과 `AddBonus()` **두 메서드 모두**에서 사용할 수 있습니다.

---

### 📌 지역 변수 vs 클래스 필드 비교

| 구분 | 지역 변수 | 클래스 필드 (static) |
|---|---|---|
| 선언 위치 | 메서드 `{ }` 안 | 클래스 `{ }` 안, 메서드 바깥 |
| 사용 범위 | 선언된 블록 안에서만 | 같은 클래스의 모든 static 메서드 |
| 생존 기간 | 블록 실행 중에만 존재 | 프로그램 실행 동안 유지 |
| 초기화 | 반드시 직접 초기화해야 함 | 자동 초기화 (int → 0, string → null 등) |

> 📌 **Tip:** 지금 단계에서는 **지역 변수 사용이 기본**입니다.
> 클래스 필드는 여러 메서드에서 값을 공유해야 할 때만 사용하세요.
> 클래스와 `static`에 대한 자세한 내용은 이후 **OOP(객체지향)** 강의에서 다룹니다.
