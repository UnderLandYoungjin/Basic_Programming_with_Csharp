# 🟣 C# 제5강 — 컬렉션 (Collection)

## 📌 개요

4강에서 배운 배열(Array)은 같은 자료형의 값을 여러 개 묶어서 저장할 수 있지만, **크기가 고정**되어 있다는 한계가 있습니다.

예를 들어 5칸짜리 배열을 만들면:

```csharp
int[] scores = new int[5]; // 딱 5칸
```

처음에는 5명만 저장하면 되지만, 나중에 학생이 6명, 7명이 되면 **배열 크기를 늘릴 수 없어** 불편해집니다.  
또한 중간 요소를 삭제하거나, 필요한 만큼 자동으로 늘어나게 만들기도 어렵습니다.

이런 문제를 해결하기 위해 C#에서는 **컬렉션(Collection)** 을 제공합니다.  
컬렉션은 배열처럼 데이터를 담는 그릇이지만, **상황에 맞게 더 편리한 기능**을 제공합니다.

---

> 💡 컬렉션을 사용하려면 코드 맨 위에 아래 네임스페이스가 필요합니다.
> ```csharp
> using System.Collections.Generic;
> ```

---

## 1. 컬렉션 종류 한눈에 보기

| 컬렉션 | 특징 | 현실 예시 |
|---|---|---|
| `List<T>` | 순서 있음, 크기 자동 증가, 추가/삭제 쉬움 | 명단, 목록 |
| `Dictionary<TKey, TValue>` | 키로 빠르게 검색, 키는 중복 불가 | 사전(단어→뜻), 학생이름→점수 |
| `HashSet<T>` | 중복 허용 안 함, 포함 여부 확인 빠름 | 중복 제거, 태그 |
| `Queue<T>` | 선입선출(FIFO) | 은행 대기표 |
| `Stack<T>` | 후입선출(LIFO) | 뒤로가기, 실행취소(Undo) |

> 💡 처음에는 `List<T>` 와 `Dictionary<TKey, TValue>` 를 우선 익히는 것을 권장합니다.

---

## 2. List<T> — 크기가 자동으로 늘어나는 배열

`List<T>` 는 배열처럼 **순서(인덱스)** 가 있고, 필요하면 **크기가 자동으로 늘어나는** 컬렉션입니다.  
즉, 배열의 단점(크기 고정)을 해결하는 가장 대표적인 방법입니다.

`<T>` 의 T는 Type(타입)의 약자이며, 저장할 자료형을 지정합니다.

```
List<int>    → 정수를 담는 리스트
List<string> → 문자열을 담는 리스트
```

---

### 📌 주요 메서드/속성

| 메서드 / 속성 | 설명 |
|---|---|
| `.Add(값)` | 맨 끝에 추가 |
| `.Insert(인덱스, 값)` | 지정 위치에 삽입 |
| `.Remove(값)` | 값으로 삭제(첫 번째만) |
| `.RemoveAt(인덱스)` | 인덱스 위치 삭제 |
| `.Contains(값)` | 포함 여부 확인 |
| `.Count` | 현재 개수 |
| `.Clear()` | 전부 삭제 |

> ⚠️ 배열은 `.Length`, List는 `.Count` 입니다.

---

### 📌 기본 사용 예제

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        List<string> names = new List<string>();

        names.Add("홍길동");
        names.Add("김철수");
        names.Add("이영희");

        Console.WriteLine($"현재 인원: {names.Count}명");

        names.Remove("김철수"); // 값으로 삭제

        Console.WriteLine($"삭제 후 인원: {names.Count}명");

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

        fruits.Insert(1, "바나나"); // 1번 위치에 삽입 → 뒤로 밀림

        foreach (string f in fruits) { Console.Write(f + " "); }
        Console.WriteLine();

        fruits.RemoveAt(0); // 0번 인덱스 삭제(사과)

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

## 3. Dictionary<TKey, TValue> — 키-값으로 저장

`Dictionary<TKey, TValue>` 는 **키(Key)** 와 **값(Value)** 을 한 쌍으로 저장합니다.  
키를 알면 값을 빠르게 찾을 수 있어, “이름으로 검색”하는 상황에 유용합니다.

예)
- `"홍길동"` → `90`
- `"한국"` → `"서울"`

> ⚠️ **키는 중복될 수 없습니다.** 같은 키로 `Add()`를 하면 오류가 납니다.

---

### 📌 주요 메서드/속성

| 메서드 / 속성 | 설명 |
|---|---|
| `.Add(키, 값)` | 추가 |
| `[키]` | 키로 값 읽기/수정 |
| `.Remove(키)` | 키-값 삭제 |
| `.ContainsKey(키)` | 키 존재 여부 |
| `.Count` | 저장된 개수 |

---

### 📌 기본 사용 예제

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        Dictionary<string, int> scores = new Dictionary<string, int>();

        scores.Add("홍길동", 90);
        scores.Add("김철수", 85);
        scores.Add("이영희", 92);

        Console.WriteLine($"홍길동 점수: {scores["홍길동"]}");

        scores["김철수"] = 95; // 수정
        Console.WriteLine($"김철수 수정 점수: {scores["김철수"]}");

        Console.WriteLine($"총 인원: {scores.Count}명");
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

### 📌 ContainsKey 예제 (없는 키 접근 방지)

`Dictionary`에서 존재하지 않는 키로 접근하면 오류가 납니다.  
따라서 `.ContainsKey()`로 먼저 확인하는 습관이 필요합니다.

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
미국은 등록되지 않았습니다.
```

---

## 4. HashSet<T> — 중복을 허용하지 않는 집합

`HashSet<T>` 는 **중복을 허용하지 않는** 컬렉션입니다.  
같은 값을 여러 번 추가해도 한 번만 저장됩니다.

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
        tags.Add("C#");    // 중복 → 저장되지 않음
        tags.Add("Python");
        tags.Add("Java");  // 중복 → 저장되지 않음

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

> 💡 HashSet은 **순서가 보장되지 않습니다.** 출력 순서는 달라질 수 있습니다.

---

## 5. Queue<T> — 선입선출 (FIFO)

`Queue<T>` 는 **먼저 들어온 것이 먼저 나가는** 구조입니다.  
은행 대기표처럼 “순서대로 처리”가 필요한 상황에 사용합니다.

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

        waiting.Enqueue("1번 손님");
        waiting.Enqueue("2번 손님");
        waiting.Enqueue("3번 손님");

        Console.WriteLine($"대기 인원: {waiting.Count}명");
        Console.WriteLine($"다음 손님: {waiting.Peek()}");

        while (waiting.Count > 0)
        {
            Console.WriteLine($"{waiting.Dequeue()} 처리 완료");
        }
    }
}
```

**실행 결과**
```
대기 인원: 3명
다음 손님: 1번 손님
1번 손님 처리 완료
2번 손님 처리 완료
3번 손님 처리 완료
```

---

## 6. Stack<T> — 후입선출 (LIFO)

`Stack<T>` 는 **나중에 들어온 것이 먼저 나가는** 구조입니다.  
브라우저 뒤로 가기, 실행 취소(Undo) 같은 동작에 사용됩니다.

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

        history.Push("구글");
        history.Push("네이버");
        history.Push("유튜브");

        Console.WriteLine($"현재 페이지: {history.Peek()}");

        while (history.Count > 0)
        {
            Console.WriteLine($"뒤로가기: {history.Pop()}");
        }
    }
}
```

**실행 결과**
```
현재 페이지: 유튜브
뒤로가기: 유튜브
뒤로가기: 네이버
뒤로가기: 구글
```

---

## 7. 컬렉션 요약 정리

| 컬렉션 | 작성 방법 | 핵심 특징 |
|---|---|---|
| List | `List<int> list = new List<int>();` | 크기 자동 증가, 인덱스로 접근 |
| Dictionary | `Dictionary<string,int> d = new Dictionary<string,int>();` | 키로 검색, 키 중복 불가 |
| HashSet | `HashSet<int> s = new HashSet<int>();` | 중복 허용 안 함 |
| Queue | `Queue<int> q = new Queue<int>();` | FIFO |
| Stack | `Stack<int> st = new Stack<int>();` | LIFO |

---

## 🧪 예제

### 예제 1 — List로 점수 합계/평균 구하기

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        List<int> scores = new List<int> { 90, 85, 78, 92, 88 };

        int sum = 0;
        foreach (int s in scores)
        {
            sum += s;
        }

        double avg = (double)sum / scores.Count;

        Console.WriteLine($"합계: {sum}");
        Console.WriteLine($"평균: {avg:F1}");
    }
}
```

**실행 결과**
```
합계: 433
평균: 86.6
```

---

### 예제 2 — Dictionary로 학생 점수 출력

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        Dictionary<string, int> scores = new Dictionary<string, int>
        {
            { "홍길동", 90 },
            { "김철수", 85 },
            { "이영희", 92 }
        };

        foreach (var pair in scores)
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}점");
        }
    }
}
```

**실행 결과**
```
홍길동: 90점
김철수: 85점
이영희: 92점
```

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

`Add(60)`으로 6개가 되었다가, `RemoveAt(0)`으로 맨 앞의 `10`이 삭제되어 5개가 됩니다.  
첫 번째 값은 원래 두 번째였던 `20`입니다.

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

**④**

Dictionary는 인덱스가 아니라 **키(Key)** 로 접근합니다.  
`scores[0]` ❌ → `scores["홍길동"]` ✅

</details>

---

### 문제 3

다음 빈칸을 채워서 Queue가 올바르게 동작하도록 완성하세요.

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        Queue<string> q = new Queue<string>();

        q.______("첫 번째");
        q.______("두 번째");
        q.______("세 번째");

        Console.WriteLine(q.______());  // "첫 번째" 출력 후 삭제
        Console.WriteLine(q.______());  // "두 번째" 확인만 (삭제 안 함)
        Console.WriteLine(q.Count);     // 2
    }
}
```

<details>
<summary>정답 보기</summary>

```csharp
q.Enqueue("첫 번째");
q.Enqueue("두 번째");
q.Enqueue("세 번째");

Console.WriteLine(q.Dequeue());
Console.WriteLine(q.Peek());
Console.WriteLine(q.Count);
```

</details>

---

### 문제 4

다음 코드에서 **잘못된 부분을 3곳** 찾아서 수정하세요.

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        List<int> list = new List<int>();
        list.Add(10);
        list.Add(20);
        list.Add(30);

        Console.WriteLine(list.Length);  // ①
        list.Add("안녕");                // ②
        list.RemoveAt(10);               // ③
    }
}
```

<details>
<summary>정답 보기</summary>

① `list.Length` → `list.Count` (List는 Count)  
② `List<int>`에는 문자열을 넣을 수 없음 → `list.Add(40)` 같은 int로 추가  
③ 현재 요소는 3개이므로 인덱스는 0~2 → `list.RemoveAt(2)` 가 맞음

</details>

---

> 📌 **Tip**
> - `List<T>`는 배열과 비슷하지만 **크기가 자동으로 늘어납니다.**
> - `Dictionary<TKey, TValue>`는 **키로 값을 찾는 구조**이며, 없는 키 접근은 오류가 될 수 있어 `.ContainsKey()`로 확인하면 안전합니다.
> - `HashSet<T>`는 **중복 제거**에 강합니다.
> - `Queue<T>`는 **먼저 들어온 것이 먼저 나가고(FIFO)**,
> - `Stack<T>`는 **나중에 들어온 것이 먼저 나갑니다(LIFO)**.
