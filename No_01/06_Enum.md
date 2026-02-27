# 🟣 C# 제6강 — 열거형 (Enum)

## 📌 개요

프로그램을 만들다 보면 **정해진 선택지 중 하나**를 값으로 사용해야 하는 경우가 많습니다.

예를 들어 신호등을 숫자로 표현하면:

```csharp
int light = 0; // 0이 빨강? 노랑? 초록? 의미를 알기 어렵다
```

이 방식의 문제점은 다음과 같습니다.

- **가독성 문제:** `0`, `1` 같은 숫자만 보고 무슨 뜻인지 알기 어렵다  
- **안전성 문제:** `5`, `100` 같은 말도 안 되는 숫자도 넣을 수 있다

이럴 때 **열거형(Enum)** 을 사용하면 "정해진 선택지"를 **이름**으로 표현할 수 있어 훨씬 알기 쉬운 코드가 됩니다.

```csharp
TrafficLight light = TrafficLight.Red; // 빨간불이라는 게 바로 보인다!
```

> 열거형(Enum)은 **골라 쓸 수 있는 선택지들을 이름으로 묶어 놓은 자료형** 입니다.
>
> 마치 메뉴판처럼 — "이 중에서만 골라주세요!" 하고 정해 두는 것입니다.

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
    Red,    // 자동으로 0이 된다
    Yellow, // 자동으로 1이 된다
    Green   // 자동으로 2가 된다
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

> 💡 항목 이름을 적을 때 숫자를 따로 쓰지 않으면 **위에서부터 0, 1, 2, 3...** 순서로 자동으로 번호가 붙습니다.  
> `Red = 0`, `Yellow = 1`, `Green = 2`

---

### ⚠️ 열거형 항목을 쓸 때는 반드시 앞에 열거형 이름을 붙여야 합니다

```csharp
TrafficLight light = Red;              // ❌ 오류 — Red가 뭔지 컴퓨터가 모른다
TrafficLight light = TrafficLight.Red; // ✅ 올바른 사용
```

---

## 2. 열거형과 조건문

열거형은 조건문과 함께 자주 쓰입니다.  
선택지가 여러 개일 때는 `switch` 문을 쓰면 코드가 훨씬 깔끔해집니다.

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

> 💡 선택지가 3개 이상이면 `if-else` 보다 `switch` 가 읽기 훨씬 쉽습니다.  
> 각 선택지마다 무슨 일이 일어나는지 한눈에 보이기 때문입니다.

---

## 3. 열거형의 숫자값 (형 변환)

열거형은 사실 내부적으로 **숫자를 이름으로 감싸 놓은 것**입니다.  
필요하면 숫자로 꺼내거나, 반대로 숫자를 열거형으로 바꿀 수 있습니다.

### 📌 열거형 → 숫자 (이름을 숫자로 꺼내기)

```csharp
using System;

enum Day { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }

class Hello
{
    public static void Main()
    {
        int num = (int)Day.Wednesday; // Wednesday는 몇 번째일까?
        Console.WriteLine(num);
    }
}
```

**실행 결과**
```
3
```

> Sunday = 0, Monday = 1, Tuesday = 2, **Wednesday = 3** 순서이므로 3이 출력됩니다.

---

### 📌 숫자 → 열거형 (숫자를 이름으로 바꾸기)

```csharp
using System;

enum Day { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }

class Hello
{
    public static void Main()
    {
        int num = 5;
        Day day = (Day)num; // 5번에 해당하는 항목은?

        Console.WriteLine(day);
    }
}
```

**실행 결과**
```
Friday
```

> ⚠️ 숫자를 열거형으로 바꾸는 건 되긴 되는데,  
> 열거형에 **없는 숫자**를 넣어도 오류가 나지 않고 이상한 값이 될 수 있으니 주의하세요.

---

## 4. 숫자값 직접 지정

자동으로 0부터 번호가 붙는 대신, 원하는 숫자를 직접 지정할 수도 있습니다.  
지정하지 않은 항목은 바로 위 항목의 숫자에서 **+1** 됩니다.

```csharp
using System;

enum MemberGrade
{
    Bronze = 1,   // 직접 1로 지정
    Silver = 2,   // 직접 2로 지정
    Gold   = 3,   // 직접 3으로 지정
    Master = 10   // 직접 10으로 지정 (중간을 건너뛸 수 있다)
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

### 📌 열거형 → 문자열 (이름을 텍스트로 꺼내기)

열거형 값을 `ToString()` 으로 그냥 텍스트 문자열로 만들 수 있습니다.

```csharp
using System;

enum Season { Spring, Summer, Fall, Winter }

class Hello
{
    public static void Main()
    {
        Season s = Season.Fall;
        Console.WriteLine(s.ToString()); // "Fall" 이라는 텍스트가 된다
    }
}
```

**실행 결과**
```
Fall
```

---

### 📌 문자열 → 열거형 (텍스트를 이름으로 바꾸기)

반대로 `"Winter"` 같은 텍스트를 열거형 값으로 바꾸려면 `Enum.Parse()` 를 씁니다.

```csharp
using System;

enum Season { Spring, Summer, Fall, Winter }

class Hello
{
    public static void Main()
    {
        Season s = (Season)Enum.Parse(typeof(Season), "Winter");
        // "Winter" 라는 문자열을 Season.Winter 로 변환

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

> ⚠️ `Enum.Parse()` 에 열거형에 없는 이름을 넣으면 프로그램이 오류로 멈춥니다.  
> 예: `"Spring123"` 같은 이름은 존재하지 않으므로 오류 발생!

---

## 6. 열거형 항목 전체 출력 (GetNames / GetValues)

열거형에 어떤 항목이 있는지 **전체 목록**을 한꺼번에 가져올 수 있습니다.

```csharp
using System;

enum Season { Spring = 1, Summer = 2, Fall = 3, Winter = 4 }

class Hello
{
    public static void Main()
    {
        string[] names  = Enum.GetNames(typeof(Season));   // 이름 목록
        int[]    values = (int[])Enum.GetValues(typeof(Season)); // 숫자 목록

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

> 💡 `GetNames()` 는 이름들을 문자열 배열로,  
> `GetValues()` 는 숫자들을 값 배열로 돌려줍니다.  
> 두 배열의 순서가 일치하기 때문에 `i` 로 함께 묶어서 출력할 수 있습니다.

---

## 7. 숫자 상수 vs 열거형 비교

| 구분 | 숫자 상수 (비권장) | 열거형 (권장) |
|---|---|---|
| 사용 예 | `if (light == 0)` | `if (light == TrafficLight.Red)` |
| 의미 | 불명확 — 0이 뭔지 모른다 | 명확 — Red가 바로 보인다 |
| 잘못된 값 | 아무 숫자나 들어갈 수 있음 | 정해진 항목만 사용 가능 |
| 가독성 | 낮음 | 높음 |

---

## 🧪 예제 — 게임 캐릭터 직업

```csharp
using System;

enum JobClass
{
    Warrior = 1, // 전사
    Mage    = 2, // 마법사
    Archer  = 3, // 궁수
    Healer  = 4  // 힐러
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

항목을 따로 숫자 지정 없이 나열했으므로 `Red = 0`, `Green = 1`, `Blue = 2` 로 자동 할당됩니다.  
`Color.Green` 은 이름으로 출력하면 `Green`, 숫자로 꺼내면 `1` 입니다.

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

① 아무것도 지정하지 않으면 기본값은 **0**부터 시작합니다.  
② `class` 블록 **바깥**에 선언하는 게 일반적입니다.  
④ `int` 를 열거형에 바로 넣으려면 `(열거형이름)` 형 변환이 반드시 필요합니다.

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

`dir` 의 값에 따라 해당 `case` 로 이동합니다.  
`dir = Direction.South` 이므로 `"남"` 이 출력됩니다.

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
열거형 항목을 쓸 때는 반드시 앞에 `열거형이름.` 을 붙여야 합니다.

② `f.tostring()` → `f.ToString()`  
C#은 대소문자를 정확히 구분합니다. `ToString` 의 `T` 와 `S` 가 대문자여야 합니다.

</details>

---

### 문제 5 (심화)

아래 조건을 만족하는 코드를 작성하세요.

- `Grade` 열거형 선언: `A = 90`, `B = 80`, `C = 70`, `D = 60`, `F = 0`
- 점수를 입력받아 해당 등급을 출력 (`if-else if` 사용)
- 90점 이상이면 A, 80점 이상이면 B, 70점 이상이면 C, 60점 이상이면 D, 그 미만은 F

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

> 💡 `if-else if` 는 위에서 아래로 순서대로 검사합니다.  
> 90 이상인지 먼저 보고, 아니면 80 이상인지... 순서대로 내려오므로  
> 조건을 **큰 숫자부터** 써야 올바르게 동작합니다.

</details>

---

> 📌 **Tip**
> - 숫자를 따로 지정하지 않으면 기본값은 **0부터 시작**합니다.
> - 항목을 쓸 때는 반드시 **`열거형이름.항목`** 형태로 씁니다.
> - 선택지가 많을수록 `if` 보다 **`switch`** 가 훨씬 보기 좋습니다.
> - 숫자 ↔ 열거형 서로 변환할 때는 **형 변환** `(int)` 또는 `(열거형이름)` 이 필요합니다.
> - `Enum.GetNames()` / `Enum.GetValues()` 로 열거형의 전체 항목을 한꺼번에 확인할 수 있습니다.
