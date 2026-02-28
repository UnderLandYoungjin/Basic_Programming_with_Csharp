# 🟣 C# 제9강 — 클래스와 객체 (Class & Object)

---

## 📌 개요

프로그램은 현실의 사물이나 개념을 코드로 표현합니다.  
예를 들어 "강아지"를 표현하려면 이름, 나이라는 **데이터**와 짖는다는 **행동**이 함께 필요합니다.  
이처럼 **관련된 데이터(필드)와 기능(메서드)을 하나로 묶은 것**이 **클래스(Class)** 입니다.

클래스는 **설계도**이고, 그 설계도로 만들어진 **실체**가 **객체(Object)** 입니다.

> 🏠 **비유:**  
> 클래스 = 건축 **설계도** → 객체 = 그 설계도로 지어진 **실제 집**  
> 설계도 하나로 집을 여러 채 짓듯이, 클래스 하나로 객체를 여러 개 만들 수 있습니다.

---

## 1. 클래스 정의와 객체 생성

클래스를 선언하는 것만으로는 아무 일도 일어나지 않습니다.  
반드시 **`new` 키워드**로 객체를 생성해야 메모리에 실체가 만들어집니다.  
이후 **`.`(점 연산자)** 로 필드와 메서드에 접근합니다.

```
class 클래스이름
{
    // 필드: 데이터를 저장하는 변수
    // 메서드: 클래스가 수행하는 기능
}
```

```csharp
using System;

class Dog
{
    public string name;  // 필드: 객체마다 독립적으로 저장되는 데이터
    public int    age;

    public void Bark()   // 메서드: 클래스가 수행하는 기능
    {
        Console.WriteLine($"{name}가 짖습니다: 왈왈!");
    }
}

class Hello
{
    public static void Main()
    {
        Dog dog = new Dog();  // new: 클래스를 바탕으로 객체(실체)를 생성
        dog.name = "초코";    // . (점): 객체의 필드에 접근
        dog.age  = 3;

        dog.Bark();           // . (점): 객체의 메서드를 호출
    }
}
```

**실행 결과**
```
초코가 짖습니다: 왈왈!
```

---

## 2. 필드 (Field)

필드는 클래스 내부에 선언된 **데이터 저장 변수**입니다.  
객체마다 **독립된 공간**에 저장되므로, 같은 클래스로 만든 객체라도 서로 다른 값을 가집니다.

`public`을 붙이면 클래스 외부에서도 `.`으로 읽고 쓸 수 있습니다.  
선언만 하고 초기화하지 않으면 숫자형은 `0`, 문자열은 `null`로 자동 설정됩니다.

```csharp
using System;

class Car
{
    public string brand;  // 선언만 하면 null (문자열 기본값)
    public int    speed;  // 선언만 하면 0    (정수 기본값)
}

class Hello
{
    public static void Main()
    {
        Car car1 = new Car();  // car1과 car2는 같은 설계도로 만들었지만
        Car car2 = new Car();  // 각자 독립된 필드를 가짐

        car1.brand = "현대";
        car1.speed = 100;

        car2.brand = "기아";
        car2.speed = 80;

        // 두 객체의 데이터는 서로 영향을 주지 않음
        Console.WriteLine($"{car1.brand}: {car1.speed}km/h");
        Console.WriteLine($"{car2.brand}: {car2.speed}km/h");
    }
}
```

**실행 결과**
```
현대: 100km/h
기아: 80km/h
```

---

## 3. 메서드 (Method)

메서드는 클래스가 수행하는 **기능을 정의한 코드 블록**입니다.  
일반 함수와 같은 구조이지만, **클래스 내부에 속한다**는 점이 다릅니다.  
반환값이 없으면 `void`를, 값을 돌려줄 때는 해당 자료형을 반환형으로 씁니다.

```
반환형 메서드이름(매개변수)
{
    // 실행할 코드
    return 값;  // void이면 생략
}
```

```csharp
using System;

class Calculator
{
    // 반환형 int: 계산 결과를 호출한 곳으로 돌려줌
    public int Add(int a, int b)
    {
        return a + b;
    }

    // 반환형 void: 결과를 돌려주지 않고 출력만 수행
    public void PrintResult(int result)
    {
        Console.WriteLine($"계산 결과: {result}");
    }
}

class Hello
{
    public static void Main()
    {
        Calculator calc = new Calculator();

        int sum = calc.Add(10, 5);  // 메서드가 반환한 값을 변수에 저장
        calc.PrintResult(sum);      // 저장한 값을 출력 메서드에 전달
    }
}
```

**실행 결과**
```
계산 결과: 15
```

---

## 4. 생성자 (Constructor)

생성자는 **`new`로 객체를 만들 때 자동으로 호출**되는 특별한 메서드입니다.  
주로 필드의 **초기값을 설정**하는 용도로 사용합니다.

생성자의 규칙:
- 이름이 **클래스 이름과 동일**해야 합니다.
- **반환형을 쓰지 않습니다** (`void`도 쓰지 않음).
- 생성자를 정의하지 않으면 C#이 **기본 생성자(빈 생성자)** 를 자동으로 만들어줍니다.

생성자가 없으면 객체를 만든 뒤 필드를 일일이 지정해야 합니다.  
생성자를 쓰면 **생성과 동시에 초기화**를 할 수 있어 코드가 간결해집니다.

```csharp
using System;

class Person
{
    public string name;
    public int    age;

    // 생성자: new Person(...) 호출 시 자동 실행
    public Person(string name, int age)
    {
        this.name = name;  // this: 매개변수와 필드 이름이 같을 때 필드를 가리킴
        this.age  = age;
    }

    public void Introduce()
    {
        Console.WriteLine($"저는 {name}, {age}살입니다.");
    }
}

class Hello
{
    public static void Main()
    {
        // 객체 생성과 동시에 초기화 (생성자 호출)
        Person p = new Person("홍길동", 25);
        p.Introduce();
    }
}
```

**실행 결과**
```
저는 홍길동, 25살입니다.
```

---

## 5. this 키워드

`this`는 **현재 객체 자신**을 가리키는 키워드입니다.  
생성자나 메서드에서 **매개변수 이름과 필드 이름이 같을 때** 두 가지를 구분하기 위해 사용합니다.

| 표현 | 의미 |
|---|---|
| `this.name` | 현재 객체의 **필드** `name` |
| `name` | 생성자의 **매개변수** `name` |

```csharp
using System;

class Book
{
    public string title;
    public int    page;

    public Book(string title, int page)
    {
        this.title = title;  // this.title → 필드, title → 매개변수
        this.page  = page;
    }

    public void Info()
    {
        Console.WriteLine($"제목: {title}, 페이지: {page}쪽");
    }
}

class Hello
{
    public static void Main()
    {
        Book b = new Book("C# 입문", 300);
        b.Info();
    }
}
```

**실행 결과**
```
제목: C# 입문, 페이지: 300쪽
```

---

## 🧪 종합 예제 — 은행 계좌

지금까지 배운 필드, 생성자, 메서드, `this`를 모두 적용한 예제입니다.

```csharp
using System;

class BankAccount
{
    public string owner;
    public int    balance;

    // 생성자: 계좌 개설 시 소유자와 초기 잔액을 설정
    public BankAccount(string owner, int balance)
    {
        this.owner   = owner;
        this.balance = balance;
    }

    public void Deposit(int amount)
    {
        balance += amount;  // 잔액 증가
        Console.WriteLine($"{amount}원 입금 → 잔액: {balance}원");
    }

    public void Withdraw(int amount)
    {
        if (amount > balance)   // 출금액이 잔액보다 크면 거부
        {
            Console.WriteLine("잔액 부족");
            return;
        }
        balance -= amount;      // 잔액 감소
        Console.WriteLine($"{amount}원 출금 → 잔액: {balance}원");
    }
}

class Hello
{
    public static void Main()
    {
        BankAccount acc = new BankAccount("홍길동", 10000);
        acc.Deposit(5000);
        acc.Withdraw(3000);
        acc.Withdraw(20000);  // 잔액 부족 케이스
    }
}
```

**실행 결과**
```
5000원 입금 → 잔액: 15000원
3000원 출금 → 잔액: 12000원
잔액 부족
```

---

## 🔍 핵심 개념 요약

| 개념 | 설명 | 핵심 키워드 |
|---|---|---|
| 클래스 | 데이터와 기능을 묶은 설계도 | `class` |
| 객체 | 클래스로 만든 실체 | `new` |
| 필드 | 객체가 저장하는 데이터 | `public 자료형 변수명` |
| 메서드 | 객체가 수행하는 기능 | `반환형 메서드명()` |
| 생성자 | 객체 생성 시 자동 호출, 초기화 담당 | 클래스명과 동일, 반환형 없음 |
| this | 현재 객체 자신을 가리킴 | `this.필드명` |

---

## 📝 문제

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

`Area()`는 `width × height = 5 × 8 = 40`을 반환합니다.

</details>

---

### 문제 2

아래 조건에 맞는 `Circle` 클래스를 작성하세요.

- 필드: `radius` (반지름, double)
- 생성자: `radius`를 매개변수로 받아 초기화 (`this` 사용)
- 메서드: `Area()` — 원의 넓이 반환 (`3.14 * radius * radius`)

<details>
<summary>정답 보기</summary>

```csharp
class Circle
{
    public double radius;

    public Circle(double radius)
    {
        this.radius = radius;  // 매개변수와 필드 구분
    }

    public double Area()
    {
        return 3.14 * radius * radius;
    }
}

Circle c = new Circle(5);
Console.WriteLine($"넓이: {c.Area()}");  // 넓이: 78.5
```

</details>

---

### 문제 3

다음 중 **클래스**에 해당하는 것과 **객체**에 해당하는 것을 구분하세요.

```
① 붕어빵 틀       ② 붕어빵
③ 자동차 설계도   ④ 완성된 자동차
```

<details>
<summary>정답 보기</summary>

- **클래스(설계도):** ①, ③
- **객체(실체):** ②, ④

</details>

---

> 📌 **마무리 요약**
> - 클래스는 **설계도**, 객체는 **`new`로 만든 실체**
> - 필드는 **객체의 데이터**, 메서드는 **객체의 기능**
> - 생성자는 **객체 생성 시 자동 호출** — 초기화에 사용
> - `this`는 **현재 객체의 필드**를 명시적으로 가리킴
