# 🟣 C# 제5강 — 컬렉션 (Collection)

## 📌 개요

4강에서 배운 **배열(Array)** 은 크기가 고정되어 있습니다.  
처음에 `new int[5]`로 만들면 5칸을 넘길 수 없고, 줄일 수도 없습니다.

```csharp
int[] arr = new int[5]; // 항상 5칸 — 더 추가하거나 삭제 불가
```

실제 프로그램에서는 데이터가 얼마나 늘어날지 미리 알기 어렵습니다.  
**컬렉션(Collection)** 은 크기가 유동적으로 변하고, 다양한 기능을 제공하는 데이터 저장소입니다.

> 💡 컬렉션을 사용하려면 코드 맨 위에 `using System.Collections.Generic;` 을 추가해야 합니다.

---

## 1. List\<T\> — 크기가 자동으로 늘어나는 배열

`List<T>` 는 가장 많이 사용하는 컬렉션입니다.  
배열처럼 인덱스로 접근할 수 있으면서, 요소를 **자유롭게 추가·삭제**할 수 있습니다.

> `<T>` 는 저장할 자료형을 의미합니다. `<int>`, `<string>` 처럼 씁니다.

### 📌 선언과 초기화

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        List<string> names = new List<string>(); // 빈 리스트
        List<int> scores = new List<int> { 90, 85, 78 }; // 값과 함께 초기화
    }
}
```

---

### 📌 주요 메서드

| 메서드 / 속성 | 설명 |
|---|---|
| `.Add(값)` | 맨 끝에 요소 추가 |
| `.Insert(인덱스, 값)` | 지정한 위치에 요소 삽입 |
| `.Remove(값)` | 특정 값을 찾아 첫 번째 것 삭제 |
| `.RemoveAt(인덱스)` | 인덱스 위치의 요소 삭제 |
| `.Contains(값)` | 값이 있으면 `true`, 없으면 `false` |
| `.Count` | 현재 저장된 요소 개수 |
| `.Clear()` | 모든 요소 삭제 |

---

### 📌 Add, Remove 예제

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

### 📌 Insert, RemoveAt 예제

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        List<string> fruits = new List<string> { "사과", "포도", "딸기" };

        fruits.Insert(1, "바나나"); // 인덱스 1 위치에 삽입

        foreach (string f in fruits)
        {
            Console.Write(f + " ");
        }
        Console.WriteLine();

        fruits.RemoveAt(0); // 인덱스 0 위치 삭제

        foreach (string f in fruits)
        {
            Console.Write(f + " ");
        }
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

## 2. Dictionary\<TKey, TValue\> — 키-값 쌍으로 저장

`Dictionary<TKey, TValue>` 는 **키(Key)** 와 **값(Value)** 을 한 쌍으로 저장합니다.  
사전에서 단어(키)로 뜻(값)을 찾듯이, 키를 이용해 빠르게 값을 찾을 수 있습니다.

> 💡 키는 중복될 수 없습니다. 같은 키로 두 번 추가하면 오류가 납니다.

### 📌 선언과 초기화

```csharp
Dictionary<string, int> scores = new Dictionary<string, int>();
Dictionary<string, string> capitals = new Dictionary<string, string>
{
    { "한국", "서울" },
    { "일본", "도쿄" },
    { "중국", "베이징" }
};
```

---

### 📌 주요 메서드

| 메서드 / 속성 | 설명 |
|---|---|
| `.Add(키, 값)` | 키-값 쌍 추가 |
| `[키]` | 키로 값에 접근 또는 수정 |
| `.Remove(키)` | 해당 키-값 쌍 삭제 |
| `.ContainsKey(키)` | 키가 있으면 `true` |
| `.ContainsValue(값)` | 값이 있으면 `true` |
| `.Count` | 저장된 쌍의 수 |
| `.Keys` | 모든 키 컬렉션 |
| `.Values` | 모든 값 컬렉션 |

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
        Console.WriteLine($"총 인원: {scores.Count}명");

        scores["김철수"] = 95; // 값 수정
        Console.WriteLine($"김철수 수정 점수: {scores["김철수"]}");
    }
}
```

**실행 결과**
```
홍길동 점수: 90
총 인원: 3명
김철수 수정 점수: 95
```

---

### 📌 전체 순회 예제

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

        foreach (KeyValuePair<string, string> pair in capitals)
        {
            Console.WriteLine($"{pair.Key}의 수도: {pair.Value}");
        }
    }
}
```

**실행 결과**
```
한국의 수도: 서울
일본의 수도: 도쿄
중국의 수도: 베이징
```

> 💡 `KeyValuePair<string, string>` 대신 `var` 를 써도 됩니다.
> ```csharp
> foreach (var pair in capitals) { ... }
> ```

---

### 📌 ContainsKey로 안전하게 접근

없는 키로 접근하면 오류가 납니다. `ContainsKey`로 먼저 확인하세요.

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
            { "김철수", 85 }
        };

        string target = "이영희";

        if (scores.ContainsKey(target))
        {
            Console.WriteLine($"{target} 점수: {scores[target]}");
        }
        else
        {
            Console.WriteLine($"{target} 는 등록되지 않았습니다.");
        }
    }
}
```

**실행 결과**
```
이영희 는 등록되지 않았습니다.
```

---

## 3. HashSet\<T\> — 중복 없는 집합

`HashSet<T>` 는 **중복을 허용하지 않는** 컬렉션입니다.  
같은 값을 여러 번 추가해도 한 번만 저장됩니다.  
순서는 보장되지 않습니다.

### 📌 선언과 초기화

```csharp
HashSet<int> set = new HashSet<int>();
HashSet<string> tags = new HashSet<string> { "C#", "Java", "Python" };
```

---

### 📌 주요 메서드

| 메서드 / 속성 | 설명 |
|---|---|
| `.Add(값)` | 요소 추가 (중복이면 무시) |
| `.Remove(값)` | 요소 삭제 |
| `.Contains(값)` | 포함 여부 확인 |
| `.Count` | 요소 개수 |
| `.UnionWith(다른집합)` | 합집합 |
| `.IntersectWith(다른집합)` | 교집합 |
| `.ExceptWith(다른집합)` | 차집합 |

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
        tags.Add("C#");   // 중복! 무시됨
        tags.Add("Python");
        tags.Add("Java"); // 중복! 무시됨

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

### 📌 집합 연산 예제

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        HashSet<int> setA = new HashSet<int> { 1, 2, 3, 4, 5 };
        HashSet<int> setB = new HashSet<int> { 3, 4, 5, 6, 7 };

        // 교집합 (공통 요소)
        HashSet<int> inter = new HashSet<int>(setA);
        inter.IntersectWith(setB);
        Console.Write("교집합: ");
        foreach (int n in inter) { Console.Write(n + " "); }
        Console.WriteLine();

        // 합집합
        HashSet<int> union = new HashSet<int>(setA);
        union.UnionWith(setB);
        Console.Write("합집합: ");
        foreach (int n in union) { Console.Write(n + " "); }
        Console.WriteLine();

        // 차집합 (A에서 B에 있는 것 제거)
        HashSet<int> diff = new HashSet<int>(setA);
        diff.ExceptWith(setB);
        Console.Write("차집합(A-B): ");
        foreach (int n in diff) { Console.Write(n + " "); }
    }
}
```

**실행 결과**
```
교집합: 3 4 5 
합집합: 1 2 3 4 5 6 7 
차집합(A-B): 1 2 
```

---

## 4. Queue\<T\> — 줄 서기 (선입선출)

`Queue<T>` 는 **먼저 들어온 것이 먼저 나가는** 구조입니다.  
은행 대기줄, 프린터 출력 순서처럼 **순서를 지켜야 할 때** 사용합니다.

> FIFO (First In, First Out) 구조라고도 합니다.

### 📌 주요 메서드

| 메서드 / 속성 | 설명 |
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

        waiting.Enqueue("1번 손님");
        waiting.Enqueue("2번 손님");
        waiting.Enqueue("3번 손님");

        Console.WriteLine($"대기 인원: {waiting.Count}명");
        Console.WriteLine($"다음 손님: {waiting.Peek()}"); // 확인만

        Console.WriteLine("\n=== 순서대로 처리 ===");
        while (waiting.Count > 0)
        {
            string customer = waiting.Dequeue(); // 꺼내서 처리
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

## 5. Stack\<T\> — 쌓기 (후입선출)

`Stack<T>` 는 **마지막에 들어온 것이 먼저 나가는** 구조입니다.  
접시를 쌓았다가 위에서부터 꺼내는 것과 같습니다.  
브라우저 **뒤로 가기**, 편집기 **실행 취소(Undo)** 에 사용됩니다.

> LIFO (Last In, First Out) 구조라고도 합니다.

### 📌 주요 메서드

| 메서드 / 속성 | 설명 |
|---|---|
| `.Push(값)` | 맨 위에 추가 |
| `.Pop()` | 맨 위 요소를 꺼내고 삭제 |
| `.Peek()` | 맨 위 요소를 확인만 (삭제 안 함) |
| `.Count` | 현재 요소 개수 |

---

### 📌 Stack 예제 — 방문 기록 뒤로 가기

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
        Console.WriteLine("\n=== 뒤로 가기 ===");

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

## 6. 컬렉션 비교 정리

| 컬렉션 | 특징 | 주요 사용처 |
|---|---|---|
| `List<T>` | 순서 있음, 중복 허용, 크기 유동적 | 일반적인 목록 |
| `Dictionary<K,V>` | 키-값 쌍, 키 중복 불가, 빠른 검색 | 이름-점수, 단어-뜻 |
| `HashSet<T>` | 중복 불가, 순서 없음, 집합 연산 | 중복 제거, 태그 |
| `Queue<T>` | 선입선출 (FIFO) | 대기열, 처리 순서 |
| `Stack<T>` | 후입선출 (LIFO) | 뒤로 가기, Undo |

---

## 7. 배열 vs List\<T\> 비교

| 구분 | 배열 `int[]` | 리스트 `List<int>` |
|---|---|---|
| 크기 | 고정 | 유동적 |
| 선언 | `int[] arr = new int[5];` | `List<int> list = new List<int>();` |
| 추가 | 불가 | `.Add(값)` |
| 삭제 | 불가 | `.Remove(값)`, `.RemoveAt(인덱스)` |
| 개수 확인 | `.Length` | `.Count` |
| 인덱스 접근 | `arr[0]` | `list[0]` |

> 💡 **언제 무엇을 쓸까?**
> - 크기가 변하지 않는 데이터 → **배열**
> - 크기가 변하거나 추가·삭제가 필요한 데이터 → **List\<T\>**

---

## 🧪 예제

### 예제 1 — 출석부 관리 (List)

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        List<string> attendance = new List<string> { "홍길동", "김철수", "이영희" };

        // 추가
        attendance.Add("박민준");
        Console.WriteLine($"=== 출석부 ({attendance.Count}명) ===");
        foreach (string name in attendance)
        {
            Console.WriteLine($"  - {name}");
        }

        // 삭제
        attendance.Remove("김철수");
        Console.WriteLine($"\n김철수 퇴학 후 ({attendance.Count}명)");
        foreach (string name in attendance)
        {
            Console.WriteLine($"  - {name}");
        }
    }
}
```

**실행 결과**
```
=== 출석부 (4명) ===
  - 홍길동
  - 김철수
  - 이영희
  - 박민준

김철수 퇴학 후 (3명)
  - 홍길동
  - 이영희
  - 박민준
```

---

### 예제 2 — 단어 사전 (Dictionary)

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>
        {
            { "apple", "사과" },
            { "banana", "바나나" },
            { "grape", "포도" }
        };

        Console.Write("찾을 단어를 입력하세요: ");
        string word = Console.ReadLine();

        if (dict.ContainsKey(word))
        {
            Console.WriteLine($"{word} = {dict[word]}");
        }
        else
        {
            Console.WriteLine("등록되지 않은 단어입니다.");
        }
    }
}
```

**실행 결과** (입력: `apple`)
```
찾을 단어를 입력하세요: apple
apple = 사과
```

---

### 예제 3 — 방문자 중복 체크 (HashSet)

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
            if (visitors.Add(name)) // 추가 성공이면 true (신규 방문)
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

`Add(60)` 으로 6개, `RemoveAt(0)` 으로 인덱스 0(10)을 삭제하면 5개가 남습니다.  
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
예: `dict["홍길동"]` — 인덱스 번호가 아닌 키 문자열을 사용합니다.

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

        Console.WriteLine(q.______());  // 첫 번째 출력 후 삭제
        Console.WriteLine(q.______());  // 두 번째 확인만 (삭제 안 함)
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

Console.WriteLine(q.Dequeue()); // 첫 번째
Console.WriteLine(q.Peek());    // 두 번째
Console.WriteLine(q.Count);     // 2
```

`Enqueue`로 추가, `Dequeue`로 꺼내고 삭제, `Peek`으로 삭제 없이 확인합니다.

</details>

---

### 문제 4

다음 빈칸을 채워서 학생 이름과 점수를 Dictionary에 저장하고 출력하는 코드를 완성하세요.

```csharp
using System;
using System.Collections.Generic;

class Hello
{
    public static void Main()
    {
        Dictionary<________, ________> scores = new Dictionary<________, ________>();

        scores.Add("홍길동", 90);
        scores.Add("김철수", 85);
        scores.Add("이영희", 92);

        foreach (var ________ in scores)
        {
            Console.WriteLine($"{________.Key}: {________.Value}점");
        }
    }
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

        Console.WriteLine(list.Length);   // ①
        list.Add("안녕");                 // ②
        list.RemoveAt(10);                // ③
    }
}
```

<details>
<summary>정답 보기</summary>

① `list.Length` → `list.Count` — List는 `.Length`가 아닌 `.Count`를 사용합니다.  
② `list.Add("안녕")` → 제거 또는 `list.Add(40)` — `List<int>`에는 문자열을 추가할 수 없습니다.  
③ `list.RemoveAt(10)` → `list.RemoveAt(2)` — 3개짜리 리스트의 인덱스는 0, 1, 2까지입니다.

**수정된 코드:**
```csharp
List<int> list = new List<int>();
list.Add(10);
list.Add(20);
list.Add(30);

Console.WriteLine(list.Count);   // 3
list.Add(40);
list.RemoveAt(2);                 // 30 삭제
```

</details>

---

### 문제 6 (심화)

학생 5명의 이름과 점수를 `Dictionary<string, int>`에 저장하고,  
점수가 **90점 이상인 학생의 이름과 점수**만 출력하는 코드를 작성하세요.

```
출력 결과 예시:
=== 우수 학생 ===
홍길동: 95점
이영희: 92점
```

사용할 데이터: `{ "홍길동", 95 }, { "김철수", 83 }, { "이영희", 92 }, { "박민준", 76 }, { "최수연", 88 }`

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

> 📌 **Tip:**
> - `List<T>` 는 배열과 비슷하지만 크기가 자동으로 늘어납니다. 개수는 `.Count`로 확인합니다.
> - `Dictionary<K,V>` 는 키로 값을 빠르게 찾을 때 사용합니다. 키는 중복될 수 없습니다.
> - `HashSet<T>` 는 중복을 자동으로 제거하고 싶을 때 유용합니다.
> - `Queue<T>` 는 먼저 들어온 것이 먼저 나오는 **FIFO** 구조입니다.
> - `Stack<T>` 는 나중에 들어온 것이 먼저 나오는 **LIFO** 구조입니다.
> - 컬렉션을 사용하려면 코드 상단에 **`using System.Collections.Generic;`** 을 반드시 추가하세요.
