# 🟣 C# 제6강 — 열거형 (Enum)

## 📌 개요

프로그램을 만들다 보면 **정해진 선택지 중 하나**를 값으로 사용해야 하는 경우가 많습니다.

예를 들어 신호등을 숫자로 표현하면:

```csharp
int light = 0; // 0이 빨강? 노랑? 초록? 의미를 알기 어렵다
```

이 방식의 문제점은 다음과 같습니다.

- **가독성 문제:** `0`, `1` 같은 숫자만 보고 의미를 파악하기 어렵다  
- **안전성 문제:** `5`, `100` 같은 잘못된 값도 들어갈 수 있다

이럴 때 **열거형(Enum)** 을 사용하면 “정해진 선택지”를 이름으로 표현할 수 있어 코드가 더 명확해집니다.

```csharp
TrafficLight light = TrafficLight.Red; // 의미가 명확하고, 정해진 값만 사용 가능
```

> 열거형(Enum)은 **정해진 선택지들의 집합을 이름으로 묶어 놓은 자료형** 입니다.

---

## 1. 열거형 선언과 초기값

### 📌 기본 선언 방법

```
enum 열거형이름
{
    항목1,
    항목2,
    항목3
}
```

열거형은 보통 `class` 블록 **바깥**에 선언합니다.

```csharp
using System;

enum TrafficLight
{
    Red,    // 0
    Yellow, // 1
    Green   // 2
}

class Hello
{
    public static void Main()
    {
        TrafficLight light = TrafficLight.Red;
        Console.WriteLine(light);
    }
}
```

**실행 결과**
```
Red
```

> 💡 열거형 항목에는 기본적으로 **0부터 시작하는 정수값**이 자동 할당됩니다.  
> `Red = 0`, `Yellow = 1`, `Green = 2`

---

### ⚠️ 열거형 항목은 반드시 `열거형이름.항목` 형태로 사용

```csharp
TrafficLight light = Red;            // ❌ 오류
TrafficLight light = TrafficLight.Red; // ✅ 올바른 사용
```

---

## 2. 열거형과 조건문

열거형은 조건문과 함께 자주 사용합니다.  
항목이 여러 개일 때는 `switch` 문이 특히 깔끔합니다.

### 📌 switch 문과 함께 사용 ✅ 권장

```csharp
using System;

enum TrafficLight { Red, Yellow, Green }

class Hello
{
    public static void Main()
    {
        TrafficLight light = TrafficLight.Yellow;

        switch (light)
        {
            case TrafficLight.Red:
                Console.WriteLine("정지!");
                break;

            case TrafficLight.Yellow:
                Console.WriteLine("주의!");
                break;

            case TrafficLight.Green:
                Console.WriteLine("출발!");
                break;
        }
    }
}
```

**실행 결과**
```
주의!
```

> 💡 항목이 3개 이상이면 `if-else` 보다 `switch` 가 더 읽기 쉬운 경우가 많습니다.

---

## 3. 열거형의 숫자값 (형 변환)

열거형은 내부적으로 정수값을 가지고 있습니다.  
필요하면 **형 변환**으로 숫자값을 확인하거나, 반대로 숫자를 열거형으로 바꿀 수 있습니다.

### 📌 열거형 → 숫자

```csharp
using System;

enum Day { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }

class Hello
{
    public static void Main()
    {
        int num = (int)Day.Wednesday;
        Console.WriteLine(num);
    }
}
```

**실행 결과**
```
3
```

---

### 📌 숫자 → 열거형

```csharp
using System;

enum Day { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }

class Hello
{
    public static void Main()
    {
        int num = 5;
        Day day = (Day)num;

        Console.WriteLine(day);
    }
}
```

**실행 결과**
```
Friday
```

> ⚠️ 숫자를 열거형으로 바꾸는 것은 가능하지만,  
> 열거형에 없는 숫자를 넣어도 “형 변환 자체는” 됩니다. (의미가 이상해질 수 있음)

---

## 4. 숫자값 직접 지정

열거형 항목에 원하는 숫자값을 직접 지정할 수 있습니다.  
지정하지 않은 항목은 이전 값에서 **+1** 됩니다.

```csharp
using System;

enum MemberGrade
{
    Bronze = 1,
    Silver = 2,
    Gold   = 3,
    Master = 10
}

class Hello
{
    public static void Main()
    {
        Console.WriteLine((int)MemberGrade.Gold);
        Console.WriteLine((int)MemberGrade.Master);
    }
}
```

**실행 결과**
```
3
10
```

---

## 5. 열거형과 문자열 변환

### 📌 열거형 → 문자열 (`ToString()`)

```csharp
using System;

enum Season { Spring, Summer, Fall, Winter }

class Hello
{
    public static void Main()
    {
        Season s = Season.Fall;
        Console.WriteLine(s.ToString());
    }
}
```

**실행 결과**
```
Fall
```

---

### 📌 문자열 → 열거형 (`Enum.Parse`)

```csharp
using System;

enum Season { Spring, Summer, Fall, Winter }

class Hello
{
    public static void Main()
    {
        Season s = (Season)Enum.Parse(typeof(Season), "Winter");

        Console.WriteLine(s);
        Console.WriteLine((int)s);
    }
}
```

**실행 결과**
```
Winter
3
```

> ⚠️ `Enum.Parse()` 는 문자열이 열거형에 없으면 오류가 발생합니다.

---

## 6. 열거형 항목 전체 출력 (GetNames / GetValues)

열거형에 어떤 항목이 있는지 전체 목록을 출력할 수 있습니다.

```csharp
using System;

enum Season { Spring = 1, Summer = 2, Fall = 3, Winter = 4 }

class Hello
{
    public static void Main()
    {
        string[] names  = Enum.GetNames(typeof(Season));
        int[]    values = (int[])Enum.GetValues(typeof(Season));

        for (int i = 0; i < names.Length; i++)
        {
            Console.WriteLine($"{values[i]}번: {names[i]}");
        }
    }
}
```

**실행 결과**
```
1번: Spring
2번: Summer
3번: Fall
4번: Winter
```

---

## 7. 숫자 상수 vs 열거형 비교

| 구분 | 숫자 상수 (비권장) | 열거형 (권장) |
|---|---|---|
| 사용 예 | `if (light == 0)` | `if (light == TrafficLight.Red)` |
| 의미 | 불명확 | 명확 |
| 잘못된 값 | 들어갈 수 있음 | 정해진 항목만 사용 |
| 가독성 | 낮음 | 높음 |

---

## 🧪 예제 — 게임 캐릭터 직업

```csharp
using System;

enum JobClass
{
    Warrior = 1,
    Mage    = 2,
    Archer  = 3,
    Healer  = 4
}

class Hello
{
    public static void Main()
    {
        JobClass myJob = JobClass.Mage;

        Console.WriteLine($"직업: {myJob} / 번호: {(int)myJob}");

        switch (myJob)
        {
            case JobClass.Warrior:
                Console.WriteLine("특기: 근접 전투");
                break;

            case JobClass.Mage:
                Console.WriteLine("특기: 마법 공격");
                break;

            case JobClass.Archer:
                Console.WriteLine("특기: 원거리 공격");
                break;

            case JobClass.Healer:
                Console.WriteLine("특기: 회복 마법");
                break;
        }
    }
}
```

**실행 결과**
```
직업: Mage / 번호: 2
특기: 마법 공격
```

---

## 🔍 열거형 요약 정리

| 구분 | 작성 방법 |
|---|---|
| 열거형 선언 | `enum 이름 { 항목1, 항목2, ... }` |
| 변수 선언 | `열거형이름 변수 = 열거형이름.항목;` |
| 열거형 → 숫자 | `(int)열거형변수` |
| 숫자 → 열거형 | `(열거형이름)정수값` |
| 열거형 → 문자열 | `변수.ToString()` |
| 문자열 → 열거형 | `(열거형)Enum.Parse(typeof(열거형), "문자열")` |
| 모든 이름 목록 | `Enum.GetNames(typeof(열거형))` |
| 모든 값 목록 | `(int[])Enum.GetValues(typeof(열거형))` |

---

## 📝 문제

---

### 문제 1

다음 코드의 출력 결과는 무엇인가요?

```csharp
using System;

enum Color { Red, Green, Blue }

class Hello
{
    public static void Main()
    {
        Color c = Color.Green;
        Console.WriteLine(c);
        Console.WriteLine((int)c);
    }
}
```

<details>
<summary>정답 보기</summary>

```
Green
1
```

`Red = 0`, `Green = 1`, `Blue = 2` 로 자동 할당됩니다.

</details>

---

### 문제 2

다음 중 열거형에 대한 설명으로 **올바른 것**을 고르세요.

```
① 열거형 항목의 기본 숫자값은 1부터 시작한다.
② 열거형은 반드시 class 안에 선언해야 한다.
③ 열거형 항목에 직접 숫자를 지정할 수 있다.
④ 열거형 변수에는 정수 값을 형 변환 없이 바로 넣을 수 있다.
```

<details>
<summary>정답 보기</summary>

**③**

① 기본값은 **0**부터 시작합니다.  
② `class` 바깥에 선언하는 것이 일반적입니다.  
④ 정수를 넣으려면 `(열거형이름)` 형 변환이 필요합니다.

</details>

---

### 문제 3

다음 빈칸을 채워서 코드를 완성하세요.

```csharp
enum Direction { North, South, East, West }

Direction dir = Direction.South;

________ (dir)
{
    case Direction.North: Console.WriteLine("북"); break;
    case Direction.South: Console.WriteLine("남"); break;
    case Direction.East:  Console.WriteLine("동"); break;
    case Direction.West:  Console.WriteLine("서"); break;
}
```

<details>
<summary>정답 보기</summary>

```csharp
switch (dir)
```

**실행 결과**
```
남
```

</details>

---

### 문제 4

다음 코드에서 **잘못된 부분을 2곳** 찾아서 수정하세요.

```csharp
using System;

enum Fruit { Apple, Banana, Grape }

class Hello
{
    public static void Main()
    {
        Fruit f = Apple;             // ①
        string name = f.tostring();  // ②
        Console.WriteLine(name);
    }
}
```

<details>
<summary>정답 보기</summary>

① `Fruit f = Apple;` → `Fruit f = Fruit.Apple;`  
열거형 항목은 반드시 `열거형이름.항목` 형식으로 써야 합니다.

② `f.tostring()` → `f.ToString()`  
C#은 대소문자를 구분합니다.

</details>

---

### 문제 5 (심화)

아래 조건을 만족하는 코드를 작성하세요.

- `Grade` 열거형 선언: `A = 90`, `B = 80`, `C = 70`, `D = 60`, `F = 0`
- 점수를 입력받아 해당 등급을 출력 (`if-else if` 사용)

```
출력 예시 (점수 85 입력 시):
점수: 85
등급: B
```

<details>
<summary>정답 보기</summary>

```csharp
using System;

enum Grade { A = 90, B = 80, C = 70, D = 60, F = 0 }

class Hello
{
    public static void Main()
    {
        Console.Write("점수를 입력하세요: ");
        int score = int.Parse(Console.ReadLine());

        Grade grade;

        if (score >= 90)      grade = Grade.A;
        else if (score >= 80) grade = Grade.B;
        else if (score >= 70) grade = Grade.C;
        else if (score >= 60) grade = Grade.D;
        else                  grade = Grade.F;

        Console.WriteLine($"점수: {score}");
        Console.WriteLine($"등급: {grade}");
    }
}
```

</details>

---

> 📌 **Tip**
> - 기본 숫자값은 **0부터 시작**합니다.
> - 항목을 사용할 때는 반드시 **`열거형이름.항목`** 형태로 씁니다.
> - 항목이 많을수록 `if` 보다 **`switch`** 가 간결합니다.
> - 숫자 ↔ 열거형 변환에는 **형 변환**이 필요합니다.
> - `Enum.GetNames()` / `Enum.GetValues()` 로 전체 항목을 확인할 수 있습니다.
