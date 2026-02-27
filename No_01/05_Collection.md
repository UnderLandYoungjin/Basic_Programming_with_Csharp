# 🟣 C# 제5강 — 컬렉션 (Collection)

## 📌 컬렉션이란?

**컬렉션(Collection)** 은 데이터를 담는 그릇입니다.  
4강에서 배운 배열도 데이터를 담는 그릇이지만, **크기가 고정**되어 있다는 한계가 있었습니다.

```csharp
int[] arr = new int[5]; // 딱 5칸 — 더 추가하거나 삭제 불가
```

컬렉션은 배열의 단점을 보완하고, **용도에 맞는 다양한 형태**를 제공합니다.

| 현실의 예 | 컬렉션 | 특징 |
|---|---|---|
| 일반 메모장 (목록) | `List<T>` | 순서 있고 자유롭게 추가·삭제 |
| 사전 (단어 → 뜻) | `Dictionary<K,V>` | 이름(키)으로 빠르게 검색 |
| 도장 찍기 (중복 방지) | `HashSet<T>` | 같은 값 두 번 못 넣음 |
| 은행 대기표 | `Queue<T>` | 먼저 들어온 것이 먼저 나감 |
| 뒤로 가기 버튼 | `Stack<T>` | 나중에 들어온 것이 먼저 나감 |

> 💡 컬렉션을 사용하려면 코드 맨 위에 아래 한 줄을 반드시 추가해야 합니다.
> ```csharp
> using System.Collections.Generic;
> ```

---

## 1. List\<T\> — 크기가 자동으로 늘어나는 배열

`List<T>` 는 **가장 많이 쓰는 컬렉션**입니다.  
배열처럼 인덱스(순번)로 접근할 수 있고, 요소를 자유롭게 **추가·삭제**할 수 있습니다.

`<T>` 의 T는 Type(타입)의 약자로, 꺾쇠괄호 `< >` 안에 저장할 자료형을 지정합니다.

```
List<int>    → 정수를 담는 리스트
List<string> → 문자열을 담는 리스트
```

### 📌 주요 메서드

| 메서드 / 속성 | 설명 |
|---|---|
| `.Add(값)` | 맨 끝에 요소 추가 |
| `.Insert(인덱스, 값)` | 지정한 위치에 요소 삽입 |
| `.Remove(값)` | 해당 값을 찾아 삭제 (첫 번째 것만) |
| `.RemoveAt(인덱스)` | 인덱스 위치의 요소 삭제 |
| `.Contains(값)` | 값이 있으면 `true`, 없으면 `false` |
| `.Count` | 현재 저장된 요소 개수 |
| `.Clear()` | 모든 요소 삭제 |

> ⚠️ 배열은 `.Length`, List는 `.Count` 입니다. 헷갈리지 않도록 주의하세요!

---

### 📌 기본 사용 예제

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        // List 선언 — 꺾쇠괄호 안에 저장할 자료형을 씁니다
        List<string> names = new List<string>();

        // Add() — 맨 끝에 요소를 추가합니다
        names.Add("홍길동");
        names.Add("김철수");
        names.Add("이영희");

        // Count — 현재 저장된 요소의 수
        Console.WriteLine($"현재 인원: {names.Count}명"); // 3

        // Remove() — 값을 찾아서 삭제합니다
        names.Remove("김철수");

        Console.WriteLine($"삭제 후 인원: {names.Count}명"); // 2

        // foreach로 전체 출력
        foreach (string name in names)
        {
            Console.WriteLine(name);
        }
    }
}
```

**실행 결과**
```
현재 인원: 3명
삭제 후 인원: 2명
홍길동
이영희
```

---

### 📌 Insert / RemoveAt 예제

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        List<string> fruits = new List<string> { "사과", "포도", "딸기" };
        //  인덱스:                                  [0]     [1]    [2]

        // Insert(인덱스, 값) — 인덱스 1 자리에 "바나나" 삽입
        // 기존 요소들은 한 칸씩 뒤로 밀립니다
        fruits.Insert(1, "바나나");
        // 결과: 사과 바나나 포도 딸기

        foreach (string f in fruits) { Console.Write(f + " "); }
        Console.WriteLine();

        // RemoveAt(인덱스) — 인덱스 0("사과") 삭제
        fruits.RemoveAt(0);
        // 결과: 바나나 포도 딸기

        foreach (string f in fruits) { Console.Write(f + " "); }
    }
}
```

**실행 결과**
```
사과 바나나 포도 딸기 
바나나 포도 딸기 
```

---

### 📌 Contains 예제

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        List<string> fruits = new List<string> { "사과", "바나나", "포도" };

        // Contains() — 해당 값이 리스트 안에 있는지 확인
        // 있으면 true, 없으면 false 반환
        if (fruits.Contains("바나나"))
        {
            Console.WriteLine("바나나가 있습니다.");
        }
        else
        {
            Console.WriteLine("바나나가 없습니다.");
        }
    }
}
```

**실행 결과**
```
바나나가 있습니다.
```

---

## 2. Dictionary\<TKey, TValue\> — 키-값 쌍으로 저장

`Dictionary` 는 **키(Key)** 와 **값(Value)** 을 한 쌍으로 저장합니다.  
실제 사전처럼, 단어(키)를 알면 뜻(값)을 바로 찾을 수 있습니다.

```
Dictionary<string, int>
              ↑       ↑
            키 타입  값 타입
```

> ⚠️ **키는 중복될 수 없습니다.** 같은 키로 두 번 `Add`하면 오류가 납니다.  
> 값은 중복되어도 괜찮습니다.

### 📌 주요 메서드

| 메서드 / 속성 | 설명 |
|---|---|
| `.Add(키, 값)` | 키-값 쌍 추가 |
| `[키]` | 키로 값 접근 또는 수정 |
| `.Remove(키)` | 해당 키-값 쌍 삭제 |
| `.ContainsKey(키)` | 키가 있으면 `true` |
| `.Count` | 저장된 쌍의 수 |

---

### 📌 기본 사용 예제

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        // Dictionary 선언 — <키 타입, 값 타입>
        Dictionary<string, int> scores = new Dictionary<string, int>();

        // Add(키, 값) — 키-값 쌍 추가
        scores.Add("홍길동", 90);
        scores.Add("김철수", 85);
        scores.Add("이영희", 92);

        // [키] — 키를 대괄호에 넣어 값을 읽습니다
        Console.WriteLine($"홍길동 점수: {scores["홍길동"]}"); // 90

        // [키] = 새값 — 기존 값을 수정합니다
        scores["김철수"] = 95;
        Console.WriteLine($"김철수 수정 점수: {scores["김철수"]}"); // 95

        Console.WriteLine($"총 인원: {scores.Count}명"); // 3
    }
}
```

**실행 결과**
```
홍길동 점수: 90
김철수 수정 점수: 95
총 인원: 3명
```

---

### 📌 전체 순회 + ContainsKey 예제

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        Dictionary<string, string> capitals = new Dictionary<string, string>
        {
            { "한국", "서울" },
            { "일본", "도쿄" },
            { "중국", "베이징" }
        };

        // foreach로 순회 — pair.Key 와 pair.Value 로 접근
        // var 를 쓰면 KeyValuePair<string,string> 을 직접 쓰지 않아도 됩니다
        foreach (var pair in capitals)
        {
            Console.WriteLine($"{pair.Key}의 수도: {pair.Value}");
        }

        Console.WriteLine();

        // ContainsKey() — 없는 키로 접근하면 오류가 나므로 먼저 확인!
        string target = "미국";
        if (capitals.ContainsKey(target))
        {
            Console.WriteLine($"{target}의 수도: {capitals[target]}");
        }
        else
        {
            Console.WriteLine($"{target}은 등록되지 않았습니다.");
        }
    }
}
```

**실행 결과**
```
한국의 수도: 서울
일본의 수도: 도쿄
중국의 수도: 베이징

미국은 등록되지 않았습니다.
```

---

## 3. HashSet\<T\> — 중복 없는 집합

`HashSet<T>` 는 **중복을 허용하지 않는** 컬렉션입니다.  
같은 값을 여러 번 추가해도 **딱 한 번만** 저장됩니다.  
순서는 보장되지 않으며, 수학의 집합 연산(합집합·교집합·차집합)을 지원합니다.

### 📌 주요 메서드

| 메서드 / 속성 | 설명 |
|---|---|
| `.Add(값)` | 추가 (중복이면 무시, `false` 반환) |
| `.Remove(값)` | 삭제 |
| `.Contains(값)` | 포함 여부 확인 |
| `.UnionWith(집합)` | 합집합 (두 집합을 합침) |
| `.IntersectWith(집합)` | 교집합 (공통 요소만 남김) |
| `.ExceptWith(집합)` | 차집합 (상대 집합에 있는 것 제거) |

---

### 📌 중복 제거 예제

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        HashSet<string> tags = new HashSet<string>();

        tags.Add("C#");
        tags.Add("Java");
        tags.Add("C#");    // 중복! → 무시되고 추가되지 않음
        tags.Add("Python");
        tags.Add("Java");  // 중복! → 무시됨

        // 3개만 저장됩니다 (C#, Java, Python)
        Console.WriteLine($"태그 수: {tags.Count}");

        foreach (string tag in tags)
        {
            Console.WriteLine(tag);
        }
    }
}
```

**실행 결과**
```
태그 수: 3
C#
Java
Python
```

---

### 📌 Add() 반환값 활용 예제

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        HashSet<string> visitors = new HashSet<string>();
        string[] log = { "홍길동", "김철수", "홍길동", "이영희", "김철수" };

        foreach (string name in log)
        {
            // Add()는 추가 성공 시 true, 중복이면 false 반환
            // 이를 활용해 신규/재방문을 구분할 수 있습니다
            if (visitors.Add(name))
            {
                Console.WriteLine($"{name} — 신규 방문");
            }
            else
            {
                Console.WriteLine($"{name} — 이미 방문함");
            }
        }

        Console.WriteLine($"\n총 고유 방문자 수: {visitors.Count}명");
    }
}
```

**실행 결과**
```
홍길동 — 신규 방문
김철수 — 신규 방문
홍길동 — 이미 방문함
이영희 — 신규 방문
김철수 — 이미 방문함

총 고유 방문자 수: 3명
```

---

## 4. Queue\<T\> — 줄 서기 (선입선출 / FIFO)

`Queue<T>` 는 **먼저 들어온 것이 먼저 나가는** 구조입니다.  
FIFO — **F**irst **I**n, **F**irst **O**ut 의 약자입니다.

```
들어오는 방향 →  [ 3번 | 2번 | 1번 ] → 나가는 방향
                  Enqueue              Dequeue
```

은행 대기표, 프린터 출력 순서처럼 **순서를 지켜야 할 때** 사용합니다.

### 📌 주요 메서드

| 메서드 | 설명 |
|---|---|
| `.Enqueue(값)` | 맨 뒤에 추가 |
| `.Dequeue()` | 맨 앞 요소를 꺼내고 삭제 |
| `.Peek()` | 맨 앞 요소를 확인만 (삭제 안 함) |
| `.Count` | 현재 요소 개수 |

---

### 📌 Queue 예제

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        Queue<string> waiting = new Queue<string>();

        // Enqueue() — 줄의 맨 뒤에 추가
        waiting.Enqueue("1번 손님");
        waiting.Enqueue("2번 손님");
        waiting.Enqueue("3번 손님");

        Console.WriteLine($"대기 인원: {waiting.Count}명");

        // Peek() — 맨 앞을 확인만 하고 삭제하지 않음
        Console.WriteLine($"다음 손님: {waiting.Peek()}");

        Console.WriteLine("\n=== 순서대로 처리 ===");

        // Dequeue() — 맨 앞 요소를 꺼내고 삭제
        // Count가 0이 될 때까지 반복
        while (waiting.Count > 0)
        {
            string customer = waiting.Dequeue();
            Console.WriteLine($"{customer} 처리 완료");
        }
    }
}
```

**실행 결과**
```
대기 인원: 3명
다음 손님: 1번 손님

=== 순서대로 처리 ===
1번 손님 처리 완료
2번 손님 처리 완료
3번 손님 처리 완료
```

---

## 5. Stack\<T\> — 쌓기 (후입선출 / LIFO)

`Stack<T>` 는 **나중에 들어온 것이 먼저 나가는** 구조입니다.  
LIFO — **L**ast **I**n, **F**irst **O**ut 의 약자입니다.

```
        ↑ Pop (꺼내기)
  [ 유튜브 ]  ← Push로 마지막에 추가
  [ 네이버 ]
  [ 구  글 ]  ← Push로 처음 추가
        ↓
```

접시를 쌓았다가 위에서부터 꺼내는 것과 같습니다.  
브라우저 **뒤로 가기**, 편집기 **Ctrl+Z(실행취소)** 가 대표적인 예입니다.

### 📌 주요 메서드

| 메서드 | 설명 |
|---|---|
| `.Push(값)` | 맨 위에 추가 |
| `.Pop()` | 맨 위 요소를 꺼내고 삭제 |
| `.Peek()` | 맨 위 요소를 확인만 (삭제 안 함) |
| `.Count` | 현재 요소 개수 |

---

### 📌 Stack 예제

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        Stack<string> history = new Stack<string>();

        // Push() — 맨 위에 쌓습니다
        history.Push("구글");    // 가장 아래
        history.Push("네이버");
        history.Push("유튜브");  // 가장 위

        // Peek() — 맨 위(가장 최근)를 확인만
        Console.WriteLine($"현재 페이지: {history.Peek()}"); // 유튜브

        Console.WriteLine("\n=== 뒤로 가기 ===");

        // Pop() — 맨 위부터 꺼냅니다 (나중에 들어온 것이 먼저 나옴)
        while (history.Count > 0)
        {
            Console.WriteLine($"이전 페이지: {history.Pop()}");
        }
    }
}
```

**실행 결과**
```
현재 페이지: 유튜브

=== 뒤로 가기 ===
이전 페이지: 유튜브
이전 페이지: 네이버
이전 페이지: 구글
```

---

## 6. 컬렉션 한눈에 비교

| 컬렉션 | 순서 | 중복 | 접근 방법 | 주요 사용처 |
|---|---|---|---|---|
| `List<T>` | ✅ 있음 | ✅ 허용 | 인덱스 `[0]` | 일반 목록 |
| `Dictionary<K,V>` | ✅ 있음 | 키 ❌ / 값 ✅ | 키 `["이름"]` | 검색이 필요한 데이터 |
| `HashSet<T>` | ❌ 없음 | ❌ 불가 | 직접 접근 불가 | 중복 제거, 집합 연산 |
| `Queue<T>` | ✅ FIFO | ✅ 허용 | Enqueue/Dequeue | 대기열 |
| `Stack<T>` | ✅ LIFO | ✅ 허용 | Push/Pop | 뒤로 가기, Undo |

> 💡 **처음에는 `List` 하나만 확실히 익혀두세요.**  
> 나머지는 필요한 상황이 오면 그때 찾아 쓰면 됩니다.

---

## 📝 문제

---

### 문제 1

다음 코드의 출력 결과는 무엇인가요?

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        List<int> numbers = new List<int> { 10, 20, 30, 40, 50 };
        numbers.Add(60);
        numbers.RemoveAt(0);
        Console.WriteLine(numbers.Count);
        Console.WriteLine(numbers[0]);
    }
}
```

<details>
<summary>정답 보기</summary>

```
5
20
```

`Add(60)` → 6개, `RemoveAt(0)` → 맨 앞 `10` 삭제 → 5개 남음.  
첫 번째 요소는 원래 두 번째였던 `20`이 됩니다.

</details>

---

### 문제 2

다음 중 Dictionary에 대한 설명으로 **틀린 것**을 고르세요.

```
① 키(Key)는 중복될 수 없다.
② 값(Value)은 중복될 수 있다.
③ 키로 값을 빠르게 검색할 수 있다.
④ 인덱스(0, 1, 2...)로 요소에 접근한다.
```

<details>
<summary>정답 보기</summary>

**④ 인덱스(0, 1, 2...)로 요소에 접근한다.**

Dictionary는 인덱스가 아닌 **키(Key)** 로 접근합니다.  
`scores[0]` ❌ → `scores["홍길동"]` ✅

</details>

---

### 문제 3

다음 빈칸을 채워서 Queue가 올바르게 동작하도록 완성하세요.

```csharp
Queue<string> q = new Queue<string>();

q.______("첫 번째");
q.______("두 번째");
q.______("세 번째");

Console.WriteLine(q.______());  // "첫 번째" 출력 후 삭제
Console.WriteLine(q.______());  // "두 번째" 확인만 (삭제 안 함)
Console.WriteLine(q.Count);     // 2
```

<details>
<summary>정답 보기</summary>

```csharp
q.Enqueue("첫 번째");
q.Enqueue("두 번째");
q.Enqueue("세 번째");

Console.WriteLine(q.Dequeue()); // 첫 번째
Console.WriteLine(q.Peek());    // 두 번째
Console.WriteLine(q.Count);     // 2
```

</details>

---

### 문제 4

다음 빈칸을 채워서 Dictionary에 학생 이름과 점수를 저장하고 출력하세요.

```csharp
Dictionary<________, ________> scores = new Dictionary<________, ________>();

scores.Add("홍길동", 90);
scores.Add("김철수", 85);
scores.Add("이영희", 92);

foreach (var ________ in scores)
{
    Console.WriteLine($"{________.Key}: {________.Value}점");
}
```

<details>
<summary>정답 보기</summary>

```csharp
Dictionary<string, int> scores = new Dictionary<string, int>();

scores.Add("홍길동", 90);
scores.Add("김철수", 85);
scores.Add("이영희", 92);

foreach (var pair in scores)
{
    Console.WriteLine($"{pair.Key}: {pair.Value}점");
}
```

**실행 결과**
```
홍길동: 90점
김철수: 85점
이영희: 92점
```

</details>

---

### 문제 5

다음 코드에서 **잘못된 부분을 3곳** 찾아서 수정하세요.

```csharp
List<int> list = new List<int>();
list.Add(10);
list.Add(20);
list.Add(30);

Console.WriteLine(list.Length);  // ①
list.Add("안녕");                // ②
list.RemoveAt(10);               // ③
```

<details>
<summary>정답 보기</summary>

① `list.Length` → `list.Count` — List는 `.Length` 대신 `.Count`를 사용합니다.  
② `list.Add("안녕")` → `list.Add(40)` — `List<int>` 에는 문자열을 넣을 수 없습니다.  
③ `list.RemoveAt(10)` → `list.RemoveAt(2)` — 3개짜리 리스트의 마지막 인덱스는 2입니다.

**수정된 코드**
```csharp
List<int> list = new List<int>();
list.Add(10);
list.Add(20);
list.Add(30);

Console.WriteLine(list.Count);  // 3
list.Add(40);
list.RemoveAt(2);               // 30 삭제
```

</details>

---

### 문제 6 (심화)

학생 5명의 이름과 점수를 `Dictionary<string, int>`에 저장하고,  
점수가 **90점 이상인 학생의 이름과 점수**만 출력하는 코드를 작성하세요.

```
=== 우수 학생 ===
홍길동: 95점
이영희: 92점
```

데이터: `홍길동 95`, `김철수 83`, `이영희 92`, `박민준 76`, `최수연 88`

<details>
<summary>정답 보기</summary>

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        Dictionary<string, int> scores = new Dictionary<string, int>
        {
            { "홍길동", 95 },
            { "김철수", 83 },
            { "이영희", 92 },
            { "박민준", 76 },
            { "최수연", 88 }
        };

        Console.WriteLine("=== 우수 학생 ===");

        // foreach로 전체 순회하면서 점수가 90 이상인 경우만 출력
        foreach (var pair in scores)
        {
            if (pair.Value >= 90)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}점");
            }
        }
    }
}
```

</details>

---

> 📌 **핵심 정리**
> - `List<T>` — 배열처럼 쓰되, 크기가 자유롭게 늘어납니다. 개수는 `.Count`
> - `Dictionary<K,V>` — 키로 값을 찾습니다. 없는 키 접근 전 `.ContainsKey()` 확인 필수
> - `HashSet<T>` — 중복이 자동으로 제거됩니다
> - `Queue<T>` — 먼저 넣은 것이 먼저 나옵니다 (FIFO)
> - `Stack<T>` — 나중에 넣은 것이 먼저 나옵니다 (LIFO)
> - 모든 컬렉션 사용 전 `using System.Collections.Generic;` 필수!
