# C# 기반 기말고사 (정보처리기사 실기 대비) 06월17일 수요일 기본프로그래밍 시간 2시간

---

## 문제 1. 반복문과 조건문

다음 C# 코드의 출력 결과를 쓰시오.

```csharp
using System;

class Program
{
    static void Main()
    {
        int sum = 0;

        for (int i = 1; i <= 5; i++)
        {
            if (i % 2 == 0)
            {
                sum += i * 2;
            }
            else
            {
                sum += i;
            }
        }

        Console.WriteLine(sum);
    }
}
```

<details>
<summary>정답 보기</summary>

```text
21
```

### 해설

| i | 조건 | 더해지는 값 | sum |
|---:|---|---:|---:|
| 1 | 홀수 | 1 | 1 |
| 2 | 짝수 | 2 × 2 = 4 | 5 |
| 3 | 홀수 | 3 | 8 |
| 4 | 짝수 | 4 × 2 = 8 | 16 |
| 5 | 홀수 | 5 | 21 |

따라서 출력 결과는 `21`입니다.

### 정보처리기사 연결 포인트

- 반복문
- 조건문
- 나머지 연산자 `%`
- 누적 합계 계산

</details>

---

## 문제 2. 배열과 인덱스

다음 C# 코드의 출력 결과를 쓰시오.

```csharp
using System;

class Program
{
    static void Main()
    {
        int[] arr = { 3, 5, 7, 9 };
        int result = 0;

        for (int i = 0; i < arr.Length; i++)
        {
            result += arr[i] - i;
        }

        Console.WriteLine(result);
    }
}
```

<details>
<summary>정답 보기</summary>

```text
18
```

### 해설

배열의 인덱스는 `0`부터 시작합니다.

| i | arr[i] | arr[i] - i | result |
|---:|---:|---:|---:|
| 0 | 3 | 3 - 0 = 3 | 3 |
| 1 | 5 | 5 - 1 = 4 | 7 |
| 2 | 7 | 7 - 2 = 5 | 12 |
| 3 | 9 | 9 - 3 = 6 | 18 |

따라서 출력 결과는 `18`입니다.

### 정보처리기사 연결 포인트

- 배열
- 인덱스
- `Length`
- 반복문을 이용한 배열 순회

</details>

---

## 문제 3. 문자열 처리

다음 C# 코드의 출력 결과를 쓰시오.

```csharp
using System;

class Program
{
    static void Main()
    {
        string text = "Engineer";
        string result = "";

        for (int i = text.Length - 1; i >= 0; i--)
        {
            result += text[i];
        }

        Console.WriteLine(result);
    }
}
```

<details>
<summary>정답 보기</summary>

```text
reenignE
```

### 해설

문자열 `"Engineer"`의 각 문자는 다음과 같습니다.

| 인덱스 | 문자 |
|---:|---|
| 0 | E |
| 1 | n |
| 2 | g |
| 3 | i |
| 4 | n |
| 5 | e |
| 6 | e |
| 7 | r |

코드는 문자열의 마지막 문자부터 처음 문자까지 거꾸로 읽어 `result`에 붙입니다.

따라서 `"Engineer"`가 뒤집혀서 출력됩니다.

```text
reenignE
```

### 정보처리기사 연결 포인트

- 문자열 길이
- 문자열 인덱싱
- 역순 출력
- 반복문 감소 조건

</details>

---

## 문제 4. 클래스와 생성자

다음 C# 코드의 출력 결과를 쓰시오.

```csharp
using System;

class Machine
{
    public int Count;

    public Machine(int count)
    {
        Count = count;
    }

    public void Add(int value)
    {
        Count += value;
    }
}

class Program
{
    static void Main()
    {
        Machine m = new Machine(10);

        m.Add(5);
        m.Add(3);

        Console.WriteLine(m.Count);
    }
}
```

<details>
<summary>정답 보기</summary>

```text
18
```

### 해설

`new Machine(10)`을 실행하면 생성자가 호출됩니다.

```csharp
public Machine(int count)
{
    Count = count;
}
```

따라서 처음 `Count` 값은 `10`입니다.

이후 실행 흐름은 다음과 같습니다.

| 실행 코드 | Count 값 |
|---|---:|
| `new Machine(10)` | 10 |
| `m.Add(5)` | 15 |
| `m.Add(3)` | 18 |

따라서 출력 결과는 `18`입니다.

### 정보처리기사 연결 포인트

- 클래스
- 객체 생성
- 생성자
- 메서드 호출
- 멤버 변수

Java 문제로 바뀌어도 구조는 거의 같습니다.

</details>

---

## 문제 5. 예외 처리

다음 C# 코드의 출력 결과를 쓰시오.

```csharp
using System;

class Program
{
    static void Main()
    {
        try
        {
            int a = 10;
            int b = 0;
            int c = a / b;

            Console.WriteLine(c);
        }
        catch
        {
            Console.WriteLine("Error");
        }
        finally
        {
            Console.WriteLine("End");
        }
    }
}
```

<details>
<summary>정답 보기</summary>

```text
Error
End
```

### 해설

다음 코드에서 문제가 발생합니다.

```csharp
int c = a / b;
```

`b`의 값이 `0`이므로 `10 / 0`은 계산할 수 없습니다.  
따라서 예외가 발생하고 `catch` 블록이 실행됩니다.

```csharp
catch
{
    Console.WriteLine("Error");
}
```

그 후 `finally` 블록은 예외 발생 여부와 관계없이 항상 실행됩니다.

```csharp
finally
{
    Console.WriteLine("End");
}
```

따라서 출력 결과는 다음과 같습니다.

```text
Error
End
```

### 정보처리기사 연결 포인트

- 예외 처리
- `try`
- `catch`
- `finally`
- 0으로 나누기 오류

</details>

---

## 시험 대비 우선순위 요약

| 문제 | 핵심 개념 | 정보처리기사 연결성 |
|---:|---|---|
| 1 | 반복문, 조건문 | 매우 높음 |
| 2 | 배열, 인덱스 | 매우 높음 |
| 3 | 문자열 처리 | 높음 |
| 4 | 클래스, 생성자 | 높음 |
| 5 | 예외 처리 | 중간~높음 |

---

## 학습 방향

C#으로 연습하더라도 문제의 본질은 C, Java, Python 문제와 같습니다.

특히 정보처리기사 실기에서는 다음 능력이 중요합니다.

1. 코드를 한 줄씩 추적하는 능력
2. 변수 값이 어떻게 바뀌는지 표로 정리하는 능력
3. 반복문 조건을 정확히 읽는 능력
4. 배열 인덱스를 `0`부터 세는 습관
5. 객체 생성 후 멤버 변수가 어떻게 변하는지 파악하는 능력
6. 예외 발생 시 실행 흐름이 어디로 이동하는지 이해하는 능력

따라서 C# 문제를 풀 때도 단순히 C# 문법만 보지 말고,  
**C / Java / Python으로 바뀌어도 같은 논리로 풀 수 있는지**를 확인하는 것이 좋습니다.
