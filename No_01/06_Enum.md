# 🟣 C# 제6강 — 열거형 (Enum)

## 📌 개요

프로그램을 만들다 보면 정해진 선택지 중 하나를 값으로 사용해야 할 때가 많습니다.

예를 들어 요일을 숫자로 나타낸다면:

```csharp
int day = 1; // 1이 월요일? 일요일? 헷갈림!
```

이렇게 하면 코드를 읽는 사람이 1이 어떤 요일인지 알 수 없습니다.  
**열거형(Enum)** 을 사용하면 이름이 있는 상수들의 집합을 만들 수 있어 코드가 훨씬 읽기 쉬워집니다.

```csharp
DayOfWeek day = DayOfWeek.Monday; // 명확하게 월요일!
```

---

## 1. 열거형 선언

### 📌 기본 선언 방법

```
enum 열거형이름
{
    항목1,
    항목2,
    항목3,
    ...
}
```

```csharp
enum Day
{
    Sunday,    // 0
    Monday,    // 1
    Tuesday,   // 2
    Wednesday, // 3
    Thursday,  // 4
    Friday,    // 5
    Saturday   // 6
}
```

> 💡 열거형의 각 항목에는 자동으로 **0부터 시작하는 정수값**이 순서대로 할당됩니다.  
> `Sunday = 0`, `Monday = 1`, `Tuesday = 2`, ...

---

### 📌 열거형은 클래스 바깥에 선언

열거형은 `class` 블록 **바깥**에 선언하는 것이 일반적입니다.

```csharp
using System;

enum Season
{
    Spring,  // 0
    Summer,  // 1
    Fall,    // 2
    Winter   // 3
}

class Hello
{
    public static void Main()
    {
        Season now = Season.Summer;
        Console.WriteLine(now); // Summer
    }
}
```

**실행 결과**
```
Summer
```

---

## 2. 열거형 사용

### 📌 변수에 저장하기

```csharp
using System;

enum TrafficLight
{
    Red,
    Yellow,
    Green
}

class Hello
{
    public static void Main()
    {
        TrafficLight light = TrafficLight.Red;
        Console.WriteLine(light); // Red
    }
}
```

**실행 결과**
```
Red
```

---

### 📌 if 문과 함께 사용

```csharp
using System;

enum TrafficLight
{
    Red,
    Yellow,
    Green
}

class Hello
{
    public static void Main()
    {
        TrafficLight light = TrafficLight.Green;

        if (light == TrafficLight.Red)
        {
            Console.WriteLine("정지!");
        }
        else if (light == TrafficLight.Yellow)
        {
            Console.WriteLine("주의!");
        }
        else if (light == TrafficLight.Green)
        {
            Console.WriteLine("출발!");
        }
    }
}
```

**실행 결과**
```
출발!
```

---

### 📌 switch 문과 함께 사용 ✅ 권장

열거형은 `switch` 문과 함께 쓰면 더욱 깔끔합니다.

```csharp
using System;

enum TrafficLight
{
    Red,
    Yellow,
    Green
}

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

---

## 3. 열거형의 숫자값

### 📌 기본 숫자값 확인

열거형 항목을 `int`로 형 변환하면 숫자값을 얻을 수 있습니다.

```csharp
using System;

enum Day
{
    Sunday,    // 0
    Monday,    // 1
    Tuesday,   // 2
    Wednesday, // 3
    Thursday,  // 4
    Friday,    // 5
    Saturday   // 6
}

class Hello
{
    public static void Main()
    {
        int num = (int)Day.Wednesday;
        Console.WriteLine(num); // 3
    }
}
```

**실행 결과**
```
3
```

---

### 📌 숫자값 직접 지정

항목에 원하는 숫자를 직접 지정할 수 있습니다.  
숫자를 지정하지 않은 항목은 이전 값에서 **+1** 됩니다.

```csharp
using System;

enum ErrorCode
{
    None    = 0,
    Warning = 100,
    Error   = 200,
    Fatal   = 300
}

class Hello
{
    public static void Main()
    {
        Console.WriteLine((int)ErrorCode.None);    // 0
        Console.WriteLine((int)ErrorCode.Warning); // 100
        Console.WriteLine((int)ErrorCode.Error);   // 200
        Console.WriteLine((int)ErrorCode.Fatal);   // 300
    }
}
```

**실행 결과**
```
0
100
200
300
```

---

### 📌 숫자 → 열거형으로 변환

반대로 숫자를 열거형으로 변환할 수도 있습니다.

```csharp
using System;

enum Day
{
    Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday
}

class Hello
{
    public static void Main()
    {
        int num = 5;
        Day day = (Day)num;
        Console.WriteLine(day); // Friday
    }
}
```

**실행 결과**
```
Friday
```

---

## 4. 열거형과 문자열 변환

### 📌 열거형 → 문자열

`.ToString()` 메서드를 사용하면 열거형 항목의 이름을 문자열로 얻을 수 있습니다.

```csharp
using System;

enum Season { Spring, Summer, Fall, Winter }

class Hello
{
    public static void Main()
    {
        Season s = Season.Fall;
        string name = s.ToString();
        Console.WriteLine(name); // Fall
    }
}
```

**실행 결과**
```
Fall
```

---

### 📌 문자열 → 열거형 (`Enum.Parse`)

`Enum.Parse()`를 사용하면 문자열을 열거형으로 변환할 수 있습니다.

```csharp
using System;

enum Season { Spring, Summer, Fall, Winter }

class Hello
{
    public static void Main()
    {
        string input = "Winter";
        Season s = (Season)Enum.Parse(typeof(Season), input);
        Console.WriteLine(s);        // Winter
        Console.WriteLine((int)s);   // 3
    }
}
```

**실행 결과**
```
Winter
3
```

> 💡 `typeof(Season)` 은 `Season` 열거형의 **자료형 정보**를 전달하는 표현입니다.  
> `Enum.Parse()`는 입력한 문자열이 열거형에 없으면 오류가 발생합니다.

---

## 5. 열거형 항목 목록 출력 (`Enum.GetNames`)

`Enum.GetNames()`를 사용하면 열거형의 모든 항목 이름을 배열로 얻을 수 있습니다.

```csharp
using System;

enum Day
{
    Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday
}

class Hello
{
    public static void Main()
    {
        string[] days = Enum.GetNames(typeof(Day));

        foreach (string day in days)
        {
            Console.WriteLine(day);
        }
    }
}
```

**실행 결과**
```
Sunday
Monday
Tuesday
Wednesday
Thursday
Friday
Saturday
```

---

## 6. 열거형 vs 숫자 상수 비교

열거형을 쓰지 않고 숫자 상수를 직접 사용하면 코드가 의미를 잃어버립니다.

| 구분 | 숫자 상수 (❌ 비권장) | 열거형 (✅ 권장) |
|---|---|---|
| 선언 | `int RED = 0; int YELLOW = 1;` | `enum Light { Red, Yellow, Green }` |
| 사용 | `if (light == 0)` | `if (light == Light.Red)` |
| 가독성 | 낮음 (0이 뭔지 모름) | 높음 (Red임이 명확) |
| 안전성 | 낮음 (잘못된 숫자 입력 가능) | 높음 (정해진 항목만 사용) |

---

## 🧪 예제

### 예제 1 — 방향 나침반

```csharp
using System;

enum Direction
{
    North,
    South,
    East,
    West
}

class Hello
{
    public static void Main()
    {
        Direction dir = Direction.East;

        switch (dir)
        {
            case Direction.North: Console.WriteLine("북쪽으로 이동합니다."); break;
            case Direction.South: Console.WriteLine("남쪽으로 이동합니다."); break;
            case Direction.East:  Console.WriteLine("동쪽으로 이동합니다."); break;
            case Direction.West:  Console.WriteLine("서쪽으로 이동합니다."); break;
        }
    }
}
```

**실행 결과**
```
동쪽으로 이동합니다.
```

---

### 예제 2 — 게임 캐릭터 직업

```csharp
using System;

enum JobClass
{
    Warrior  = 1,
    Mage     = 2,
    Archer   = 3,
    Healer   = 4
}

class Hello
{
    public static void Main()
    {
        JobClass myJob = JobClass.Mage;

        Console.WriteLine($"직업: {myJob}");
        Console.WriteLine($"직업 번호: {(int)myJob}");

        switch (myJob)
        {
            case JobClass.Warrior: Console.WriteLine("특기: 근접 전투"); break;
            case JobClass.Mage:    Console.WriteLine("특기: 마법 공격"); break;
            case JobClass.Archer:  Console.WriteLine("특기: 원거리 공격"); break;
            case JobClass.Healer:  Console.WriteLine("특기: 회복 마법"); break;
        }
    }
}
```

**실행 결과**
```
직업: Mage
직업 번호: 2
특기: 마법 공격
```

---

### 예제 3 — 모든 계절 출력

```csharp
using System;

enum Season
{
    Spring = 1,
    Summer = 2,
    Fall   = 3,
    Winter = 4
}

class Hello
{
    public static void Main()
    {
        string[] seasons = Enum.GetNames(typeof(Season));
        int[]    values  = (int[])Enum.GetValues(typeof(Season));

        Console.WriteLine("=== 계절 목록 ===");
        for (int i = 0; i < seasons.Length; i++)
        {
            Console.WriteLine($"{values[i]}번: {seasons[i]}");
        }
    }
}
```

**실행 결과**
```
=== 계절 목록 ===
1번: Spring
2번: Summer
3번: Fall
4번: Winter
```

---

## 🔍 열거형 요약 정리

| 구분 | 작성 방법 |
|---|---|
| 열거형 선언 | `enum 이름 { 항목1, 항목2, ... }` |
| 변수 선언 | `열거형이름 변수 = 열거형이름.항목;` |
| 숫자로 변환 | `(int)열거형변수` |
| 숫자 → 열거형 | `(열거형이름)정수값` |
| 이름 문자열로 | `변수.ToString()` |
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

`Color.Green`의 이름은 `"Green"`이고, 자동 할당된 숫자값은 `1`입니다.  
(`Red = 0`, `Green = 1`, `Blue = 2`)

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

**③ 열거형 항목에 직접 숫자를 지정할 수 있다.**

① 기본값은 **0**부터 시작합니다.  
② 열거형은 `class` **바깥**에 선언하는 것이 일반적입니다.  
④ 정수를 열거형 변수에 넣으려면 `(열거형이름)` 형 변환이 필요합니다.

</details>

---

### 문제 3

다음 빈칸을 채워서 `direction`이 `North`일 때 `"북"`, `South`일 때 `"남"`을 출력하는 코드를 완성하세요.

```csharp
enum Direction { North, South, East, West }

Direction direction = Direction.South;

________ (direction)
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
switch (direction)
```

**실행 결과**
```
남
```

열거형과 `switch` 문을 함께 사용하면 각 항목에 따른 처리를 깔끔하게 작성할 수 있습니다.

</details>

---

### 문제 4

다음 열거형과 코드를 보고 출력 결과를 쓰세요.

```csharp
using System;

enum Level
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
        Level myLevel = Level.Gold;
        Console.WriteLine(myLevel);
        Console.WriteLine((int)myLevel);

        Level topLevel = (Level)10;
        Console.WriteLine(topLevel);
    }
}
```

<details>
<summary>정답 보기</summary>

```
Gold
3
Master
```

`Level.Gold`의 이름은 `"Gold"`, 숫자값은 `3`입니다.  
숫자 `10`을 `Level`로 변환하면 `Master`가 됩니다.

</details>

---

### 문제 5

다음 코드에서 **잘못된 부분을 2곳** 찾아서 수정하세요.

```csharp
using System;

enum Fruit
{
    Apple,
    Banana,
    Grape
}

class Hello
{
    public static void Main()
    {
        Fruit f = Apple;            // ①
        Console.WriteLine((int)f);
        
        string name = f.tostring(); // ②
        Console.WriteLine(name);
    }
}
```

<details>
<summary>정답 보기</summary>

① `Fruit f = Apple;` → `Fruit f = Fruit.Apple;`  
열거형 항목을 사용할 때는 반드시 `열거형이름.항목` 형식으로 써야 합니다.

② `f.tostring()` → `f.ToString()`  
C#은 대소문자를 구분합니다. `ToString()`의 `T`, `S`는 대문자입니다.

**수정된 코드:**
```csharp
Fruit f = Fruit.Apple;
Console.WriteLine((int)f);

string name = f.ToString();
Console.WriteLine(name);
```

**실행 결과:**
```
0
Apple
```

</details>

---

### 문제 6 (심화)

아래 조건을 만족하는 코드를 작성하세요.

- `Grade` 열거형을 선언합니다: `A = 90`, `B = 80`, `C = 70`, `D = 60`, `F = 0`
- 점수를 하나 입력받아서 해당하는 등급을 출력합니다.
- `switch` 문 대신 `if-else if` 문을 사용합니다.

```
출력 예시 (점수 85 입력 시):
점수: 85
등급: B
```

<details>
<summary>정답 보기</summary>

```csharp
using System;

enum Grade
{
    A = 90,
    B = 80,
    C = 70,
    D = 60,
    F = 0
}

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

> 📌 **Tip:**
> - 열거형 항목의 기본 숫자값은 **0부터 시작**합니다.
> - 열거형 항목을 사용할 때는 반드시 **`열거형이름.항목`** 형식으로 씁니다.
> - 열거형과 `switch` 문을 함께 사용하면 코드가 더 **명확하고 안전**해집니다.
> - 숫자 ↔ 열거형 변환 시 **`(int)` 또는 `(열거형이름)` 형 변환**이 필요합니다.
> - `Enum.GetNames()`로 열거형의 **모든 항목 이름**을 배열로 가져올 수 있습니다.
