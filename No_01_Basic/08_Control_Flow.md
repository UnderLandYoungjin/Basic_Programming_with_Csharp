# 🟣 C# 제8강 — 제어문 (Control Flow)

## 📌 개요
**제어문(Control Flow)** 은 프로그램의 **실행 흐름을 조건에 따라 분기하거나 반복**시키는 구문입니다.  
지금까지는 코드가 위에서 아래로 순서대로 실행됐지만, 제어문을 사용하면 **"이 조건이면 이쪽으로"**, **"조건이 맞는 동안 계속 반복해"** 와 같은 흐름을 만들 수 있습니다.

제어문은 크게 두 가지로 나뉩니다.
- **조건문** — 조건에 따라 실행 경로를 선택 (`if`, `switch`)
- **반복문** — 조건이 만족되는 동안 반복 실행 (`for`, `while`, `do-while`, `foreach`)

---

## 1. if 문

### 📌 기본 if 문

가장 기본적인 조건문입니다. 조건이 `true`일 때만 블록 안의 코드가 실행됩니다.

```
if (조건) {
    // 조건이 true일 때 실행
}
```

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int temperature = 35;

        if (temperature >= 30)
        {
            Console.WriteLine("날씨가 덥습니다.");
        }
    }
}
```

**실행 결과**
```
날씨가 덥습니다.
```

---

### 📌 if-else 문

조건이 `true`이면 `if` 블록, `false`이면 `else` 블록이 실행됩니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int temperature = 20;

        if (temperature >= 30)
        {
            Console.WriteLine("날씨가 덥습니다.");
        }
        else
        {
            Console.WriteLine("날씨가 적당합니다.");
        }
    }
}
```

**실행 결과**
```
날씨가 적당합니다.
```

---

### 📌 if-else if-else 문

세 가지 이상의 경우를 처리할 때는 `else if`를 사용합니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int temperature = 5;

        if (temperature >= 30)
        {
            Console.WriteLine("날씨가 덥습니다.");
        }
        else if (temperature >= 15)
        {
            Console.WriteLine("날씨가 적당합니다.");
        }
        else
        {
            Console.WriteLine("날씨가 춥습니다.");
        }
    }
}
```

**실행 결과**
```
날씨가 춥습니다.
```

> 💡 **Tip:** `else if`는 위에서부터 순서대로 조건을 확인합니다.  
> **처음으로 true가 되는 블록 하나만 실행**되고 나머지는 건너뜁니다.

---

## 2. switch 문

하나의 변수 값에 따라 여러 경우를 처리할 때, `if-else if`보다 **더 깔끔하게** 표현할 수 있습니다.

```
switch (변수) {
    case 값1:
        // 실행 코드
        break;
    case 값2:
        // 실행 코드
        break;
    default:
        // 어느 case도 해당 없을 때
        break;
}
```

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int day = 3;

        switch (day)
        {
            case 1:
                Console.WriteLine("월요일");
                break;
            case 2:
                Console.WriteLine("화요일");
                break;
            case 3:
                Console.WriteLine("수요일");
                break;
            case 4:
                Console.WriteLine("목요일");
                break;
            case 5:
                Console.WriteLine("금요일");
                break;
            default:
                Console.WriteLine("주말");
                break;
        }
    }
}
```

**실행 결과**
```
수요일
```

> ⚠️ **주의:** 각 `case` 블록 마지막에는 반드시 `break;`를 적어야 합니다.  
> `break`가 없으면 다음 `case`까지 계속 실행되는 오류가 발생합니다.

### 📌 여러 case를 묶기

같은 코드를 실행해야 하는 case는 **묶어서 처리**할 수 있습니다.

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int day = 6;

        switch (day)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
                Console.WriteLine("평일입니다.");
                break;
            case 6:
            case 7:
                Console.WriteLine("주말입니다!");
                break;
            default:
                Console.WriteLine("잘못된 입력입니다.");
                break;
        }
    }
}
```

**실행 결과**
```
주말입니다!
```

---

## 3. for 문

**횟수가 정해진 반복**에 가장 많이 사용합니다.

```
for (초기식; 조건식; 증감식) {
    // 반복 실행할 코드
}
```

| 구성 요소 | 설명 | 예시 |
|---|---|---|
| 초기식 | 반복 시작 전 한 번 실행 | `int i = 0` |
| 조건식 | `true`인 동안 반복 | `i < 5` |
| 증감식 | 매 반복 후 실행 | `i++` |

```csharp
using System;
class Hello
{
    public static void Main()
    {
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"반복 {i}번째");
        }
    }
}
```

**실행 결과**
```
반복 0번째
반복 1번째
반복 2번째
반복 3번째
반복 4번째
```

> 💡 **Tip:** `i`는 0부터 시작하는 것이 C# 관례입니다.  
> `i < 5`이면 0, 1, 2, 3, 4 — 총 **5번** 반복됩니다.

### 📌 역순 반복

```csharp
using System;
class Hello
{
    public static void Main()
    {
        for (int i = 5; i >= 1; i--)
        {
            Console.WriteLine($"카운트다운: {i}");
        }
        Console.WriteLine("발사!");
    }
}
```

**실행 결과**
```
카운트다운: 5
카운트다운: 4
카운트다운: 3
카운트다운: 2
카운트다운: 1
발사!
```

---

## 4. while 문

**조건이 true인 동안** 계속 반복합니다.  
반복 횟수를 미리 알 수 없을 때 주로 사용합니다.

```
while (조건) {
    // 반복 실행할 코드
}
```

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int count = 1;

        while (count <= 5)
        {
            Console.WriteLine($"{count}번 실행 중...");
            count++;
        }
        Console.WriteLine("완료!");
    }
}
```

**실행 결과**
```
1번 실행 중...
2번 실행 중...
3번 실행 중...
4번 실행 중...
5번 실행 중...
완료!
```

> ⚠️ **주의:** `count++`처럼 조건이 언젠가 `false`가 되도록 반드시 **탈출 조건**을 만들어야 합니다.  
> 그렇지 않으면 **무한 루프**가 발생합니다.

---

## 5. do-while 문

`while`과 비슷하지만, **조건 확인 전에 블록을 먼저 한 번 실행**합니다.  
즉, 조건이 처음부터 `false`여도 **최소 1번은 반드시 실행**됩니다.

```
do {
    // 최소 한 번은 반드시 실행
} while (조건);
```

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int num = 10;

        do
        {
            Console.WriteLine($"num = {num}");
            num++;
        } while (num < 5);  // 처음부터 false지만 한 번은 실행됨

        Console.WriteLine("do-while 종료");
    }
}
```

**실행 결과**
```
num = 10
do-while 종료
```

> 💡 **Tip:** "메뉴를 최소 한 번은 보여준 뒤, 다시 볼지 물어본다"는 상황처럼  
> **무조건 한 번은 실행해야 하는 경우**에 `do-while`이 적합합니다.

---

## 6. foreach 문

배열이나 컬렉션의 **모든 요소를 순서대로 꺼내서** 처리할 때 사용합니다.  
인덱스 없이 더 간결하게 작성할 수 있어 가독성이 뛰어납니다.

```
foreach (자료형 변수 in 컬렉션) {
    // 각 요소에 대해 실행
}
```

```csharp
using System;
class Hello
{
    public static void Main()
    {
        string[] fruits = { "사과", "바나나", "포도", "딸기" };

        foreach (string fruit in fruits)
        {
            Console.WriteLine($"과일: {fruit}");
        }
    }
}
```

**실행 결과**
```
과일: 사과
과일: 바나나
과일: 포도
과일: 딸기
```

> 💡 **Tip:** `for`는 인덱스가 필요할 때, `foreach`는 **요소 자체만 필요할 때** 사용합니다.

---

## 7. break / continue

반복문 안에서 흐름을 **중간에 제어**할 때 사용합니다.

| 키워드 | 설명 |
|---|---|
| `break` | 반복문을 **즉시 탈출** |
| `continue` | 이번 회차만 **건너뛰고** 다음 반복으로 |

### 📌 break 예제

```csharp
using System;
class Hello
{
    public static void Main()
    {
        for (int i = 1; i <= 10; i++)
        {
            if (i == 5)
            {
                Console.WriteLine("5에서 멈춥니다!");
                break;  // 반복문 탈출
            }
            Console.WriteLine(i);
        }
    }
}
```

**실행 결과**
```
1
2
3
4
5에서 멈춥니다!
```

### 📌 continue 예제

```csharp
using System;
class Hello
{
    public static void Main()
    {
        for (int i = 1; i <= 7; i++)
        {
            if (i % 2 == 0)
            {
                continue;  // 짝수는 건너뜀
            }
            Console.WriteLine(i);
        }
    }
}
```

**실행 결과**
```
1
3
5
7
```

---

## 🧪 예제

### 예제 1 — 구구단 출력 (for 중첩 반복문)

```csharp
using System;
class Hello
{
    public static void Main()
    {
        for (int i = 2; i <= 9; i++)
        {
            Console.WriteLine($"--- {i}단 ---");
            for (int j = 1; j <= 9; j++)
            {
                Console.WriteLine($"{i} x {j} = {i * j}");
            }
        }
    }
}
```

**실행 결과 (일부)**
```
--- 2단 ---
2 x 1 = 2
2 x 2 = 4
...
--- 9단 ---
9 x 8 = 72
9 x 9 = 81
```

> 💡 **Tip:** 반복문 안에 반복문을 넣는 것을 **중첩 반복문**이라 합니다.  
> 구구단처럼 2차원적으로 반복할 때 유용합니다.

---

### 예제 2 — 합계 계산 (while 문)

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int sum  = 0;
        int num  = 1;

        while (num <= 100)
        {
            sum += num;
            num++;
        }

        Console.WriteLine($"1부터 100까지의 합: {sum}");
    }
}
```

**실행 결과**
```
1부터 100까지의 합: 5050
```

---

### 예제 3 — 점수 등급 판정 (if-else if + switch 비교)

```csharp
using System;
class Hello
{
    public static void Main()
    {
        int score = 76;
        string grade;

        if (score >= 90)      grade = "A";
        else if (score >= 80) grade = "B";
        else if (score >= 70) grade = "C";
        else if (score >= 60) grade = "D";
        else                  grade = "F";

        Console.WriteLine($"점수: {score}점, 등급: {grade}");

        switch (grade)
        {
            case "A":
                Console.WriteLine("최우수! 장학금 대상입니다.");
                break;
            case "B":
                Console.WriteLine("우수한 성적입니다.");
                break;
            case "C":
                Console.WriteLine("양호한 성적입니다.");
                break;
            case "D":
                Console.WriteLine("조금 더 노력해 보세요.");
                break;
            default:
                Console.WriteLine("재수강을 권장합니다.");
                break;
        }
    }
}
```

**실행 결과**
```
점수: 76점, 등급: C
양호한 성적입니다.
```

---

### 예제 4 — 짝수만 출력 (for + continue)

```csharp
using System;
class Hello
{
    public static void Main()
    {
        Console.Write("1~20 중 짝수: ");

        for (int i = 1; i <= 20; i++)
        {
            if (i % 2 != 0) continue;
            Console.Write($"{i} ");
        }

        Console.WriteLine();  // 줄바꿈
    }
}
```

**실행 결과**
```
1~20 중 짝수: 2 4 6 8 10 12 14 16 18 20 
```

---

## 🔍 제어문 종류 요약

| 분류 | 구문 | 설명 |
|---|---|---|
| 조건문 | `if` / `if-else` / `if-else if-else` | 조건에 따라 분기 |
| 조건문 | `switch` | 변수 값에 따라 다중 분기 |
| 반복문 | `for` | 횟수가 정해진 반복 |
| 반복문 | `while` | 조건이 true인 동안 반복 |
| 반복문 | `do-while` | 최소 1회 실행 후 조건 확인 |
| 반복문 | `foreach` | 컬렉션 요소를 순서대로 처리 |
| 흐름 제어 | `break` | 반복문 즉시 탈출 |
| 흐름 제어 | `continue` | 현재 회차 건너뛰기 |

---

## 📝 문제

---

### 문제 1

다음 코드의 출력 결과는 무엇인가요?

```csharp
for (int i = 1; i <= 5; i++)
{
    if (i == 3) continue;
    Console.Write(i + " ");
}
```

<details>
<summary>정답 보기</summary>

```
1 2 4 5 
```

`i == 3`일 때 `continue`로 건너뛰기 때문에 3은 출력되지 않습니다.

</details>

---

### 문제 2

다음 코드의 출력 결과는 무엇인가요?

```csharp
int x = 0;

do
{
    Console.WriteLine(x);
    x++;
} while (x < 3);
```

<details>
<summary>정답 보기</summary>

```
0
1
2
```

`do-while`은 조건 확인 전에 먼저 실행하므로 0, 1, 2가 출력되고 x가 3이 되면 종료됩니다.

</details>

---

### 문제 3

다음 중 **무한 루프**가 발생하는 코드를 고르고, 그 이유를 설명하세요.

```csharp
// ① 
int i = 0;
while (i < 5) {
    Console.WriteLine(i);
    i++;
}

// ②
int j = 0;
while (j < 5) {
    Console.WriteLine(j);
}
```

<details>
<summary>정답 보기</summary>

**②번**이 무한 루프입니다.  
`j++`가 없어서 `j`가 항상 0이고, `0 < 5`는 항상 `true`이기 때문에 루프가 끝나지 않습니다.

</details>

---

### 문제 4

`switch` 문을 사용하여 아래 조건에 맞는 코드를 작성하세요.

- 변수 `season`이 `"봄"`이면 `"꽃이 핍니다."` 출력
- `"여름"`이면 `"바다로 가요."` 출력
- `"가을"`이면 `"단풍이 듭니다."` 출력
- `"겨울"`이면 `"눈이 옵니다."` 출력
- 그 외에는 `"알 수 없는 계절입니다."` 출력

```csharp
string season = "가을";
// 여기에 switch 문 작성
```

<details>
<summary>정답 보기</summary>

```csharp
string season = "가을";

switch (season)
{
    case "봄":
        Console.WriteLine("꽃이 핍니다.");
        break;
    case "여름":
        Console.WriteLine("바다로 가요.");
        break;
    case "가을":
        Console.WriteLine("단풍이 듭니다.");
        break;
    case "겨울":
        Console.WriteLine("눈이 옵니다.");
        break;
    default:
        Console.WriteLine("알 수 없는 계절입니다.");
        break;
}
```

</details>

---

### 문제 5

`for` 문을 사용하여 **1부터 50까지의 숫자 중 3의 배수만** 출력하는 코드를 작성하세요.

<details>
<summary>정답 보기</summary>

```csharp
for (int i = 1; i <= 50; i++)
{
    if (i % 3 == 0)
    {
        Console.Write(i + " ");
    }
}
```

**실행 결과**
```
3 6 9 12 15 18 21 24 27 30 33 36 39 42 45 48 
```

</details>

---

> 📌 **Tip:**
> - **`if`** 는 조건이 복잡하거나 범위 비교가 필요할 때, **`switch`** 는 하나의 변수를 여러 값과 비교할 때 사용합니다.
> - **`for`** 는 반복 횟수가 명확할 때, **`while`** 은 조건 기반 반복에 적합합니다.
> - **`break`** 와 **`continue`** 로 반복문 흐름을 세밀하게 제어할 수 있습니다.
> - **중첩 반복문** 사용 시 안쪽 `break`는 안쪽 반복문만 탈출한다는 점에 주의하세요.
