
# 🟣 C# 제9강 — 클래스와 객체 (Class & Object)

>

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

> 🏠 설계도 하나로 집을 여러 채 지을 수 있듯이, 클래스 하나로 객체를 여러 개 만들 수 있습니다.

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

# 🐶 예제 1 — 기본 클래스 (Dog)

```csharp
using System;

class Dog
{
    // 필드 (속성/데이터)
    public string name;
    public int    age;
    public string breed;

    // 메서드 (기능/행동)
    public void Bark()
    {
        Console.WriteLine($"{name}가 짖습니다: 왈왈!");
    }

    public void Info()
    {
        Console.WriteLine($"이름: {name}, 나이: {age}살, 품종: {breed}");
    }
}

class Program
{
    static void Main()
    {
        Dog dog1 = new Dog();
        dog1.name  = "초코";
        dog1.age   = 3;
        dog1.breed = "푸들";

        Dog dog2 = new Dog();
        dog2.name  = "망고";
        dog2.age   = 5;
        dog2.breed = "골든리트리버";

        dog1.Info();
        dog1.Bark();

        dog2.Info();
        dog2.Bark();
    }
}
```

**실행 결과**
```
이름: 초코, 나이: 3살, 품종: 푸들
초코가 짖습니다: 왈왈!
이름: 망고, 나이: 5살, 품종: 골든리트리버
망고가 짖습니다: 왈왈!
```

### 핵심

- `new` → 객체 생성
- `.` → 필드/메서드 접근
- 객체는 서로 독립적

---

# 📦 필드 (Field)

클래스가 가지는 **데이터(속성)** 를 저장하는 변수입니다.

```csharp
using System;

class Car
{
    public string brand;   // 브랜드
    public string color;   // 색상
    public int    speed;   // 현재 속도
}

class Program
{
    static void Main()
    {
        Car car = new Car();
        car.brand = "현대";
        car.color = "흰색";
        car.speed = 0;

        Console.WriteLine($"{car.color} {car.brand} 자동차 (현재 속도: {car.speed}km/h)");
    }
}
```

**실행 결과**
```
흰색 현대 자동차 (현재 속도: 0km/h)
```

---

# ⚙️ 메서드 (Method)

클래스가 수행하는 **기능(행동)** 을 정의합니다.

```csharp
반환형 메서드이름(매개변수)
{
    // 실행 코드
    return 값;  // 반환형이 void이면 생략
}
```

```csharp
using System;

class Calculator
{
    // 반환값이 있는 메서드
    public int Add(int a, int b)
    {
        return a + b;
    }

    public int Multiply(int a, int b)
    {
        return a * b;
    }

    // 반환값이 없는 메서드 (void)
    public void PrintResult(string op, int result)
    {
        Console.WriteLine($"결과: {op} = {result}");
    }
}

class Program
{
    static void Main()
    {
        Calculator calc = new Calculator();

        int sum     = calc.Add(10, 5);
        int product = calc.Multiply(10, 5);

        calc.PrintResult("10 + 5", sum);
        calc.PrintResult("10 * 5", product);
    }
}
```

**실행 결과**
```
결과: 10 + 5 = 15
결과: 10 * 5 = 50
```

---

# 🏗 생성자 (Constructor)

## 왜 필요한가?

기존 방식:

```csharp
Dog d = new Dog();
d.name = "초코";
d.age  = 3;
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

규칙:

1. 클래스 이름과 동일
2. 반환형 없음 (void도 쓰지 않음)
3. `new` 할 때 자동 실행

---

## 🧑 예제 2 — 생성자 적용 (Person)

```csharp
using System;

class Person
{
    public string name;
    public int    age;

    // 생성자
    public Person(string n, int a)
    {
        name = n;
        age  = a;
        Console.WriteLine($"{name} 객체가 생성되었습니다.");
    }

    public void Introduce()
    {
        Console.WriteLine($"안녕하세요, 저는 {name}이고 {age}살입니다.");
    }
}

class Program
{
    static void Main()
    {
        Person p1 = new Person("홍길동", 25);
        Person p2 = new Person("김영희", 30);

        p1.Introduce();
        p2.Introduce();
    }
}
```

**실행 결과**
```
홍길동 객체가 생성되었습니다.
김영희 객체가 생성되었습니다.
안녕하세요, 저는 홍길동이고 25살입니다.
안녕하세요, 저는 김영희이고 30살입니다.
```

> 💡 생성자를 따로 정의하지 않으면 C#이 **기본 생성자(매개변수 없는 생성자)** 를 자동으로 만들어줍니다.

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

## 🎓 예제 3 — this 적용 (Student)

```csharp
using System;

class Student
{
    public string name;
    public int    grade;

    public Student(string name, int grade)
    {
        this.name  = name;   // this.name = 필드, name = 매개변수
        this.grade = grade;
    }

    public void Print()
    {
        Console.WriteLine($"학생: {this.name}, {this.grade}학년");
    }
}

class Program
{
    static void Main()
    {
        Student s = new Student("이민준", 3);
        s.Print();
    }
}
```

**실행 결과**
```
학생: 이민준, 3학년
```

---

# 🏦 종합 예제 — BankAccount

```csharp
using System;

class BankAccount
{
    public string owner;
    public int    balance;

    public BankAccount(string owner, int balance)
    {
        this.owner   = owner;
        this.balance = balance;
    }

    public void Deposit(int amount)
    {
        balance += amount;
        Console.WriteLine($"{amount}원 입금 → 잔액: {balance}원");
    }

    public void Withdraw(int amount)
    {
        if (amount > balance)
        {
            Console.WriteLine("잔액이 부족합니다.");
            return;
        }
        balance -= amount;
        Console.WriteLine($"{amount}원 출금 → 잔액: {balance}원");
    }

    public void PrintInfo()
    {
        Console.WriteLine($"[{owner}] 현재 잔액: {balance}원");
    }
}

class Program
{
    static void Main()
    {
        BankAccount account = new BankAccount("홍길동", 10000);
        account.PrintInfo();
        account.Deposit(5000);
        account.Withdraw(3000);
        account.Withdraw(20000);
        account.PrintInfo();
    }
}
```

**실행 결과**
```
[홍길동] 현재 잔액: 10000원
5000원 입금 → 잔액: 15000원
3000원 출금 → 잔액: 12000원
잔액이 부족합니다.
[홍길동] 현재 잔액: 12000원
```

---

# 📊 핵심 정리

| 개념 | 설명 | 비유 |
|------|------|------|
| 클래스 | 객체를 만들기 위한 설계도 | 건축 설계도 |
| 객체 | new로 생성된 실체 | 실제 건물 |
| 필드 | 객체가 가진 데이터 | 건물의 방 개수, 면적 |
| 메서드 | 객체의 기능 | 건물에서 할 수 있는 일 |
| 생성자 | 객체 생성 시 초기 설정 | 입주 당일 초기 세팅 |
| this | 현재 객체 자신 | "나 자신" |

---

# 📝 학습 체크

### 문제 1

다음 코드의 출력 결과는 무엇인가요?

```csharp
class Box
{
    public int width;
    public int height;

    public int Area()
    {
        return width * height;
    }
}

Box b = new Box();
b.width  = 5;
b.height = 8;
Console.WriteLine(b.Area());
```

<details>
<summary>정답 보기</summary>

```
40
```

`Area()`는 `width * height` = `5 * 8` = `40`을 반환합니다.

</details>

---

### 문제 2

생성자를 포함한 `Circle` 클래스를 작성하세요.

- 필드: `radius` (반지름)
- 메서드: `Area()` — 원의 넓이 반환 (`3.14 * radius * radius`)
- 생성자: `radius`를 매개변수로 받아 초기화

<details>
<summary>정답 보기</summary>

```csharp
class Circle
{
    public double radius;

    public Circle(double radius)
    {
        this.radius = radius;
    }

    public double Area()
    {
        return 3.14 * radius * radius;
    }
}

Circle c = new Circle(5);
Console.WriteLine($"넓이: {c.Area()}");
// 넓이: 78.5
```

</details>

---

### 문제 3

아래 설명에서 **클래스**와 **객체**에 해당하는 것을 각각 고르세요.

```
① 붕어빵 틀
② 붕어빵
③ 자동차 설계도
④ 공장에서 생산된 자동차
```

<details>
<summary>정답 보기</summary>

**클래스:** ①, ③  
**객체:** ②, ④  

설계도(틀)가 클래스, 그것으로 만들어진 실체가 객체입니다.

</details>

---

# ⏭ 다음 강의 예고

- 접근 제한자 (public / private)
- 캡슐화
- 왜 필드를 public으로 두면 위험한가

---

