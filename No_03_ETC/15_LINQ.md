# 🟣 C# 제15강 — LINQ (Language Integrated Query)

## 📌 개요
**LINQ(링크)** 는 배열이나 List 같은 데이터 컬렉션에서 **원하는 데이터를 조회·정렬·필터링**하는 기능입니다.  
데이터베이스의 SQL 쿼리처럼 "이런 조건의 데이터만 뽑아줘"라는 작업을 C# 코드 안에서 바로 할 수 있습니다.

> 🔍 **비유:** 학생 명단이 적힌 종이 묶음에서  
> "90점 이상인 학생만 골라서 점수 높은 순으로 뽑아줘"라고 하는 것입니다.  
> LINQ 이전에는 반복문과 조건문으로 직접 구현해야 했지만, LINQ를 사용하면 **한 줄**로 해결됩니다.

LINQ를 사용하려면 `using System.Linq;`를 선언합니다.

---

## 1. LINQ 없이 vs LINQ 사용

같은 작업을 LINQ 전후로 비교해 봅니다.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

class Hello
{
    public static void Main()
    {
        List<int> numbers = new List<int> { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // ❌ LINQ 없이 — 반복문 직접 작성
        List<int> result1 = new List<int>();
        foreach (int n in numbers)
        {
            if (n >= 5) result1.Add(n);
        }
        result1.Sort();
        Console.WriteLine("LINQ 없이: " + string.Join(", ", result1));

        // ✅ LINQ 사용 — 한 줄로 해결
        var result2 = numbers.Where(n => n >= 5).OrderBy(n => n);
        Console.WriteLine("LINQ 사용: " + string.Join(", ", result2));
    }
}
```

**실행 결과**
```
LINQ 없이: 5, 6, 7, 8, 9
LINQ 사용: 5, 6, 7, 8, 9
```

---

## 2. 람다식 (Lambda Expression)

LINQ를 사용하려면 **람다식**을 알아야 합니다.  
람다식은 **이름 없는 간단한 함수**를 짧게 표현하는 방법입니다.

```
매개변수 => 실행 내용
```

```csharp
// 일반 메서드
bool IsOver5(int n) { return n >= 5; }

// 람다식으로 표현
n => n >= 5
```

> 💡 **Tip:** `n => n >= 5`는 "n을 받아서 n >= 5인지 판단한다"고 읽으면 됩니다.  
> 왼쪽은 **입력**, 오른쪽은 **결과**입니다.

---

## 3. Where — 조건 필터링

조건에 맞는 요소만 걸러냅니다.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

class Hello
{
    public static void Main()
    {
        List<int> scores = new List<int> { 92, 45, 88, 63, 75, 30, 95, 52 };

        // 60점 이상만
        var passed = scores.Where(s => s >= 60);
        Console.WriteLine("합격: " + string.Join(", ", passed));

        // 60점 미만만
        var failed = scores.Where(s => s < 60);
        Console.WriteLine("불합격: " + string.Join(", ", failed));

        // 짝수만
        var evens = scores.Where(s => s % 2 == 0);
        Console.WriteLine("짝수 점수: " + string.Join(", ", evens));
    }
}
```

**실행 결과**
```
합격: 92, 88, 63, 75, 95, 52
불합격: 45, 30, 52
짝수 점수: 92, 88, 30, 52
```

---

## 4. OrderBy / OrderByDescending — 정렬

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

class Hello
{
    public static void Main()
    {
        List<string> names = new List<string> { "Charlie", "Alice", "Eve", "Bob", "David" };

        // 오름차순 정렬
        var ascending = names.OrderBy(n => n);
        Console.WriteLine("오름차순: " + string.Join(", ", ascending));

        // 내림차순 정렬
        var descending = names.OrderByDescending(n => n);
        Console.WriteLine("내림차순: " + string.Join(", ", descending));

        // 문자열 길이 순 정렬
        var byLength = names.OrderBy(n => n.Length);
        Console.WriteLine("길이순:   " + string.Join(", ", byLength));
    }
}
```

**실행 결과**
```
오름차순: Alice, Bob, Charlie, David, Eve
내림차순: Eve, David, Charlie, Bob, Alice
길이순:   Eve, Bob, Alice, David, Charlie
```

---

## 5. Select — 변환 (투영)

각 요소를 **다른 형태로 변환**합니다.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

class Hello
{
    public static void Main()
    {
        List<string> names = new List<string> { "홍길동", "김영희", "이민준" };

        // 모두 대문자로
        var upper = names.Select(n => n.ToUpper());
        Console.WriteLine(string.Join(", ", upper));

        // 길이만 추출
        var lengths = names.Select(n => n.Length);
        Console.WriteLine("이름 길이: " + string.Join(", ", lengths));

        List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };

        // 각 숫자를 제곱으로 변환
        var squared = numbers.Select(n => n * n);
        Console.WriteLine("제곱: " + string.Join(", ", squared));
    }
}
```

**실행 결과**
```
홍길동, 김영희, 이민준
이름 길이: 3, 3, 3
제곱: 1, 4, 9, 16, 25
```

---

## 6. 집계 메서드 — Count / Sum / Max / Min / Average

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

class Hello
{
    public static void Main()
    {
        List<int> scores = new List<int> { 92, 45, 88, 63, 75, 30, 95, 52 };

        Console.WriteLine($"총 인원:  {scores.Count()}명");
        Console.WriteLine($"합계:     {scores.Sum()}점");
        Console.WriteLine($"최고점:   {scores.Max()}점");
        Console.WriteLine($"최저점:   {scores.Min()}점");
        Console.WriteLine($"평균:     {scores.Average():F1}점");

        // 조건 포함한 집계
        Console.WriteLine($"60점 이상 인원: {scores.Count(s => s >= 60)}명");
        Console.WriteLine($"60점 이상 합계: {scores.Where(s => s >= 60).Sum()}점");
    }
}
```

**실행 결과**
```
총 인원:  8명
합계:     540점
최고점:   95점
최저점:   30점
평균:     67.5점
60점 이상 인원: 5명
60점 이상 합계: 413점
```

---

## 7. First / Last / Single

| 메서드 | 설명 |
|---|---|
| `First()` | 첫 번째 요소 반환 (없으면 예외) |
| `FirstOrDefault()` | 첫 번째 요소 반환 (없으면 기본값) |
| `Last()` | 마지막 요소 반환 |
| `Single()` | 조건에 맞는 요소가 정확히 1개일 때 |

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

class Hello
{
    public static void Main()
    {
        List<int> numbers = new List<int> { 3, 7, 2, 8, 1, 9, 4 };

        Console.WriteLine($"첫 번째:       {numbers.First()}");
        Console.WriteLine($"마지막:        {numbers.Last()}");
        Console.WriteLine($"5 초과 첫 번째: {numbers.First(n => n > 5)}");

        // 없을 때 기본값 반환 (int 기본값 = 0)
        int notFound = numbers.FirstOrDefault(n => n > 100);
        Console.WriteLine($"100 초과 첫 번째: {notFound}");  // 0
    }
}
```

**실행 결과**
```
첫 번째:       3
마지막:        4
5 초과 첫 번째: 7
100 초과 첫 번째: 0
```

---

## 8. 메서드 체이닝 — 연결해서 사용

LINQ 메서드는 **여러 개를 이어서 사용**할 수 있습니다.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

class Hello
{
    public static void Main()
    {
        List<int> numbers = new List<int> { 5, 3, 8, 1, 9, 2, 7, 4, 6, 10 };

        // 짝수만 → 내림차순 정렬 → 상위 3개
        var result = numbers
            .Where(n => n % 2 == 0)
            .OrderByDescending(n => n)
            .Take(3);

        Console.WriteLine("짝수 중 상위 3개: " + string.Join(", ", result));

        // 5 이상인 것들의 제곱 합
        int sumOfSquares = numbers
            .Where(n => n >= 5)
            .Select(n => n * n)
            .Sum();

        Console.WriteLine($"5 이상 숫자들의 제곱 합: {sumOfSquares}");
    }
}
```

**실행 결과**
```
짝수 중 상위 3개: 10, 8, 6
5 이상 숫자들의 제곱 합: 355
```

---

## 9. 클래스와 함께 사용하기

실전에서는 **객체 리스트**에 LINQ를 사용하는 경우가 가장 많습니다.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

class Student
{
    public string Name  { get; set; }
    public int    Score { get; set; }
    public string Class { get; set; }

    public Student(string name, int score, string cls)
    {
        Name  = name;
        Score = score;
        Class = cls;
    }
}

class Hello
{
    public static void Main()
    {
        List<Student> students = new List<Student>
        {
            new Student("홍길동", 92, "A반"),
            new Student("김영희", 88, "B반"),
            new Student("이민준", 75, "A반"),
            new Student("박지수", 95, "B반"),
            new Student("최현우", 60, "A반"),
            new Student("정다은", 45, "B반")
        };

        // 90점 이상 학생 이름만
        var topStudents = students
            .Where(s => s.Score >= 90)
            .Select(s => s.Name);
        Console.WriteLine("90점 이상: " + string.Join(", ", topStudents));

        // A반 학생 점수 평균
        double avgA = students
            .Where(s => s.Class == "A반")
            .Average(s => s.Score);
        Console.WriteLine($"A반 평균: {avgA:F1}점");

        // 점수 내림차순 전체 출력
        var ranked = students.OrderByDescending(s => s.Score);
        Console.WriteLine("\n=== 석차 ===");
        int rank = 1;
        foreach (Student s in ranked)
        {
            Console.WriteLine($"{rank++}위 {s.Name} ({s.Class}): {s.Score}점");
        }
    }
}
```

**실행 결과**
```
90점 이상: 홍길동, 박지수
A반 평균: 75.7점

=== 석차 ===
1위 박지수 (B반): 95점
2위 홍길동 (A반): 92점
3위 김영희 (B반): 88점
4위 이민준 (A반): 75점
5위 최현우 (A반): 60점
6위 정다은 (B반): 45점
```

---

## 🔍 핵심 메서드 요약

| 메서드 | 설명 | 예시 |
|---|---|---|
| `Where(조건)` | 조건 필터링 | `Where(n => n > 5)` |
| `OrderBy(기준)` | 오름차순 정렬 | `OrderBy(n => n)` |
| `OrderByDescending(기준)` | 내림차순 정렬 | `OrderByDescending(n => n)` |
| `Select(변환)` | 형태 변환 | `Select(n => n * n)` |
| `Count()` | 개수 | `Count(n => n > 5)` |
| `Sum()` | 합계 | `Sum()` |
| `Max()` | 최댓값 | `Max()` |
| `Min()` | 최솟값 | `Min()` |
| `Average()` | 평균 | `Average()` |
| `First()` | 첫 번째 요소 | `First(n => n > 5)` |
| `FirstOrDefault()` | 첫 번째 (없으면 기본값) | `FirstOrDefault()` |
| `Take(n)` | 앞에서 n개만 | `Take(3)` |
| `Skip(n)` | 앞에서 n개 건너뜀 | `Skip(2)` |

---

## 📝 문제

---

### 문제 1

다음 코드의 출력 결과는 무엇인가요?

```csharp
List<int> nums = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
var result = nums.Where(n => n % 2 == 0).Sum();
Console.WriteLine(result);
```

<details>
<summary>정답 보기</summary>

```
30
```

짝수(2, 4, 6, 8, 10)의 합 = 30

</details>

---

### 문제 2

다음 학생 목록에서 **"B반"이면서 70점 이상**인 학생의 이름을 점수 내림차순으로 출력하는 LINQ를 작성하세요.

```csharp
List<Student> students = new List<Student>
{
    new Student("홍길동", 92, "A반"),
    new Student("김영희", 88, "B반"),
    new Student("이민준", 65, "B반"),
    new Student("박지수", 55, "B반"),
};
```

<details>
<summary>정답 보기</summary>

```csharp
var result = students
    .Where(s => s.Class == "B반" && s.Score >= 70)
    .OrderByDescending(s => s.Score)
    .Select(s => s.Name);

Console.WriteLine(string.Join(", ", result));
// 김영희, 이민준
```

</details>

---

### 문제 3

1부터 100까지 숫자 중 **3의 배수이면서 5의 배수**인 숫자들의 합을 LINQ로 구하세요.

<details>
<summary>정답 보기</summary>

```csharp
var numbers = Enumerable.Range(1, 100);

int result = numbers
    .Where(n => n % 3 == 0 && n % 5 == 0)
    .Sum();

Console.WriteLine(result);  // 315
// (15, 30, 45, 60, 75, 90 의 합)
```

</details>

---

### 문제 4

`Take`와 `Skip`을 조합하여 숫자 리스트 `{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }`에서  
**4번째부터 6번째** 요소(4, 5, 6)만 출력하는 코드를 작성하세요.

<details>
<summary>정답 보기</summary>

```csharp
List<int> nums = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

var result = nums.Skip(3).Take(3);
Console.WriteLine(string.Join(", ", result));  // 4, 5, 6
```

`Skip(3)`으로 앞 3개를 건너뛰고, `Take(3)`으로 그 다음 3개를 가져옵니다.

</details>

---

> 📌 **Tip:**
> - LINQ를 사용하려면 `using System.Linq;`를 반드시 선언하세요.
> - `Where` → `OrderBy` → `Select` 순서로 체이닝하는 것이 가장 일반적인 패턴입니다.
> - `FirstOrDefault()`는 결과가 없을 때 예외 대신 기본값을 반환하므로 안전합니다.
> - 복잡한 반복문 로직이 보이면 "LINQ로 줄일 수 없을까?" 먼저 생각해 보세요.
