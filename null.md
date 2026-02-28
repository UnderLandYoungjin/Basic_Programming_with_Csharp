<div align="center">

# 🟣 C# 제9강 — 클래스와 객체 (Class & Object)

초급 직업훈련 과정용 강의 자료  
C# 객체지향 기초 개념 정리

</div>

---

# 📌 강의 목표

이 강의를 마치면 다음을 이해할 수 있습니다:

- 클래스와 객체의 차이
- 객체 생성 방법 (`new`)
- 필드와 메서드의 역할
- 생성자의 필요성과 동작 원리
- `this` 키워드의 의미

---

# 📚 왜 클래스가 필요한가?

기존 방식:

```csharp
string name = "홍길동";
int age = 25;
```

문제점:

- 관련 데이터가 분리되어 있음
- 사람 수가 늘어나면 변수 증가
- 기능(행동)을 묶을 수 없음

👉 해결: **데이터 + 기능을 하나로 묶는 구조 = 클래스**

---

# 🏗 클래스와 객체 개념

| 구분 | 의미 |
|------|------|
| 클래스 | 객체를 만들기 위한 설계도 |
| 객체 | 클래스로 만든 실제 인스턴스 |

비유:

- 설계도 → 클래스
- 실제 건물 → 객체

---

# 🧱 클래스 기본 구조

```csharp
class 클래스이름
{
    // 필드 (데이터)
    // 메서드 (기능)
}
```

---

# 🐶 예제 1 — 기본 클래스

```csharp
using System;

class Dog
{
    public string name;
    public int age;

    public void Bark()
    {
        Console.WriteLine($"{name}가 짖습니다.");
    }

    public void Info()
    {
        Console.WriteLine($"이름: {name}, 나이: {age}");
    }
}

class Program
{
    static void Main()
    {
        Dog dog1 = new Dog();
        dog1.name = "초코";
        dog1.age = 3;

        dog1.Info();
        dog1.Bark();
    }
}
```

### 핵심

- `new` → 객체 생성
- `.` → 필드/메서드 접근
- 객체는 서로 독립적

---

# 🏗 생성자 (Constructor)

## 왜 필요한가?

기존 방식:

```csharp
Dog d = new Dog();
d.name = "초코";
d.age = 3;
```

문제:

- 초기화 누락 가능
- 코드가 길어짐

---

## 생성자 구조

```csharp
class 클래스명
{
    public 클래스명(매개변수)
    {
        // 초기 설정
    }
}
```

---

## 🐶 예제 2 — 생성자 적용

```csharp
using System;

class Dog
{
    public string name;
    public int age;

    public Dog(string name, int age)
    {
        this.name = name;
        this.age = age;
    }

    public void Info()
    {
        Console.WriteLine($"이름: {name}, 나이: {age}");
    }
}

class Program
{
    static void Main()
    {
        Dog d = new Dog("망고", 5);
        d.Info();
    }
}
```

### 규칙

1. 클래스 이름과 동일
2. 반환형 없음
3. new 할 때 자동 실행

---

# 🔍 this 키워드

## 문제 상황

```csharp
name = name;
```

필드에 값이 저장되지 않음.

---

## 해결

```csharp
this.name = name;
```

| 구분 | 의미 |
|------|------|
| this.name | 필드 |
| name | 매개변수 |

---

# 🏦 종합 예제 — BankAccount

```csharp
using System;

class BankAccount
{
    public string owner;
    public int balance;

    public BankAccount(string owner, int balance)
    {
        this.owner = owner;
        this.balance = balance;
    }

    public void Deposit(int amount)
    {
        balance += amount;
        Console.WriteLine($"{amount}원 입금 → 잔액: {balance}");
    }

    public void Withdraw(int amount)
    {
        if (amount > balance)
        {
            Console.WriteLine("잔액 부족");
            return;
        }

        balance -= amount;
        Console.WriteLine($"{amount}원 출금 → 잔액: {balance}");
    }
}

class Program
{
    static void Main()
    {
        BankAccount account = new BankAccount("홍길동", 10000);

        account.Deposit(5000);
        account.Withdraw(3000);
        account.Withdraw(20000);
    }
}
```

---

# 📊 핵심 정리

| 개념 | 설명 |
|------|------|
| 클래스 | 설계도 |
| 객체 | new로 생성된 실체 |
| 필드 | 객체가 가진 데이터 |
| 메서드 | 객체의 기능 |
| 생성자 | 객체 생성 시 초기 설정 |
| this | 현재 객체 자신 |

---

# 📝 학습 체크

1. 객체 생성 키워드는 무엇인가?
2. 생성자는 언제 실행되는가?
3. this는 무엇을 가리키는가?

---

# ⏭ 다음 강의 예고

- 접근 제한자 (public / private)
- 캡슐화
- 왜 필드를 public으로 두면 위험한가

---

<div align="center">

## 🚀 Practice Makes Perfect

객체지향은 암기가 아니라 **반복 실습으로 이해하는 구조입니다.**

</div>
