<div align="center">

# 🟣 C# 제12강 — 다형성과 인터페이스 (Polymorphism & Interface)

</div>

---

# 📌 강의 목표

이 강의를 마치면 다음을 이해할 수 있습니다:

- 다형성의 개념과 동작 원리
- 추상 클래스 (`abstract`)의 용도
- 인터페이스 (`interface`)의 구조와 활용
- 추상 클래스 vs 인터페이스 차이
- `is` / `as` 연산자 사용법

---

# 📚 왜 다형성과 인터페이스가 필요한가?

기존 방식:

```csharp
if (type == "Dog")  { dog.Bark(); }
if (type == "Cat")  { cat.Purr(); }
if (type == "Cow")  { cow.Moo();  }
```

문제점:

- 새로운 동물이 추가될 때마다 조건문 수정
- 코드가 길어지고 실수 가능성 증가

👉 해결: **같은 호출 코드로 객체마다 다르게 동작 = 다형성**

> 🎮 **다형성** — 리모컨의 전원 버튼은 같지만, TV에서 누르면 TV가 켜지고, 에어컨에서 누르면 에어컨이 켜집니다.  
> 🔌 **인터페이스** — "전자기기라면 반드시 전원 버튼이 있어야 한다"는 규격(계약)입니다.

---

# 🎭 다형성 (Polymorphism)

**부모 타입 변수로 자식 객체를 다루면**, 실제 객체 타입에 맞는 메서드가 호출됩니다.

---

# 🔷 예제 1 — 다형성 (Shape)

```csharp
using System;

class Shape
{
    public string color;

    public Shape(string color)
    {
        this.color = color;
    }

    public virtual void Draw()
    {
        Console.WriteLine($"{color} 도형을 그립니다.");
    }

    public virtual double Area()
    {
        return 0;
    }
}

class Circle : Shape
{
    double radius;

    public Circle(string color, double radius) : base(color)
    {
        this.radius = radius;
    }

    public override void Draw()
    {
        Console.WriteLine($"{color} 원을 그립니다. (반지름: {radius})");
    }

    public override double Area()
    {
        return 3.14 * radius * radius;
    }
}

class Rectangle : Shape
{
    double width, height;

    public Rectangle(string color, double width, double height) : base(color)
    {
        this.width  = width;
        this.height = height;
    }

    public override void Draw()
    {
        Console.WriteLine($"{color} 직사각형을 그립니다. ({width} x {height})");
    }

    public override double Area()
    {
        return width * height;
    }
}

class Triangle : Shape
{
    double base_, height;

    public Triangle(string color, double base_, double height) : base(color)
    {
        this.base_  = base_;
        this.height = height;
    }

    public override void Draw()
    {
        Console.WriteLine($"{color} 삼각형을 그립니다. (밑변: {base_}, 높이: {height})");
    }

    public override double Area()
    {
        return base_ * height / 2;
    }
}

class Program
{
    static void Main()
    {
        // 부모 타입 배열에 자식 객체들을 담음
        Shape[] shapes = {
            new Circle   ("빨간", 5),
            new Rectangle("파란", 4, 6),
            new Triangle ("초록", 8, 3)
        };

        foreach (Shape s in shapes)
        {
            s.Draw();
            Console.WriteLine($"  → 넓이: {s.Area():F2}\n");
        }
    }
}
```

**실행 결과**
```
빨간 원을 그립니다. (반지름: 5)
  → 넓이: 78.50

파란 직사각형을 그립니다. (4 x 6)
  → 넓이: 24.00

초록 삼각형을 그립니다. (밑변: 8, 높이: 3)
  → 넓이: 12.00
```

### 핵심

- 같은 `Draw()`, `Area()` 호출이지만 **객체 종류에 따라 다르게 동작**
- 새로운 도형을 추가해도 반복문 코드는 **수정 불필요**

---

# 🏗 추상 클래스 (Abstract Class)

## 왜 필요한가?

- 부모 클래스를 직접 생성하면 안 될 때
- 자식이 **반드시** 특정 메서드를 구현하도록 강제하고 싶을 때

---

## 추상 클래스 구조

```csharp
abstract class 클래스이름
{
    public abstract 반환형 메서드이름();  // 구현 없이 선언만
    public void 일반메서드() { ... }       // 일반 메서드는 구현 가능
}
```

규칙:

1. `abstract` 클래스는 직접 객체 생성 불가
2. `abstract` 메서드는 자식이 **무조건 오버라이딩** 해야 함
3. 일반 메서드는 공통 동작을 구현 가능

---

## 🐾 예제 2 — 추상 클래스 (Animal)

```csharp
using System;

abstract class Animal
{
    public string name;

    public Animal(string name)
    {
        this.name = name;
    }

    // 추상 메서드: 반드시 자식이 구현해야 함
    public abstract void Sound();

    // 일반 메서드: 공통 동작
    public void Sleep()
    {
        Console.WriteLine($"{name}가 잠을 잡니다.");
    }
}

class Dog : Animal
{
    public Dog(string name) : base(name) { }

    public override void Sound()
    {
        Console.WriteLine($"{name}: 왈왈!");
    }
}

class Duck : Animal
{
    public Duck(string name) : base(name) { }

    public override void Sound()
    {
        Console.WriteLine($"{name}: 꽥꽥!");
    }
}

class Program
{
    static void Main()
    {
        // Animal a = new Animal("?");  // ❌ 추상 클래스는 직접 생성 불가!

        Animal[] animals = { new Dog("초코"), new Duck("도널드") };

        foreach (Animal a in animals)
        {
            a.Sound();
            a.Sleep();
        }
    }
}
```

**실행 결과**
```
초코: 왈왈!
초코가 잠을 잡니다.
도널드: 꽥꽥!
도널드가 잠을 잡니다.
```

### 핵심

- `Sound()`를 구현하지 않은 자식 클래스는 **컴파일 에러**
- "이 클래스를 상속하는 모든 자식은 반드시 이 기능을 가져야 한다"는 **강제 규약**

---

# 🔌 인터페이스 (Interface)

## 왜 필요한가?

- **기능의 목록(계약)** 만 정의하고, 구현은 전혀 없는 순수한 명세
- 클래스는 인터페이스를 **여러 개 동시에** 구현 가능

---

## 인터페이스 구조

```csharp
interface 인터페이스이름
{
    반환형 메서드이름();  // 구현 없이 선언만
}
```

> 💡 인터페이스 이름은 관례적으로 **`I`** 로 시작합니다. (`IFlyable`, `IComparable` 등)

---

## 🦆 예제 3 — 인터페이스 (IFlyable / ISwimmable)

```csharp
using System;

interface IFlyable
{
    void Fly();
}

interface ISwimmable
{
    void Swim();
}

class Eagle : IFlyable
{
    public void Fly()
    {
        Console.WriteLine("독수리가 하늘을 날아갑니다.");
    }
}

class Fish : ISwimmable
{
    public void Swim()
    {
        Console.WriteLine("물고기가 헤엄칩니다.");
    }
}

// 오리는 날기도, 수영도 가능 — 인터페이스 여러 개 구현
class Duck : IFlyable, ISwimmable
{
    public void Fly()
    {
        Console.WriteLine("오리가 낮게 날아갑니다.");
    }

    public void Swim()
    {
        Console.WriteLine("오리가 물 위를 헤엄칩니다.");
    }
}

class Program
{
    static void Main()
    {
        Duck duck = new Duck();
        duck.Fly();
        duck.Swim();

        Console.WriteLine();

        // 인터페이스 타입으로 참조
        IFlyable   flyer   = new Eagle();
        ISwimmable swimmer = new Fish();

        flyer.Fly();
        swimmer.Swim();
    }
}
```

**실행 결과**
```
오리가 낮게 날아갑니다.
오리가 물 위를 헤엄칩니다.

독수리가 하늘을 날아갑니다.
물고기가 헤엄칩니다.
```

---

# ⚖️ 추상 클래스 vs 인터페이스

| 구분 | 추상 클래스 | 인터페이스 |
|------|-------------|------------|
| 키워드 | `abstract class` | `interface` |
| 메서드 구현 | 일부 구현 가능 | 구현 불가 (명세만) |
| 다중 상속 | ❌ 하나만 가능 | ✅ 여러 개 구현 가능 |
| 생성자 | 있음 | 없음 |
| 필드 | 있음 | 없음 |
| 용도 | 공통 기능 + 강제 구현 | 기능 규격(계약) 정의 |

선택 기준:

- **공통 코드**가 있고 **"is-a" 관계**이면 → 추상 클래스
- **기능 규격**만 정의하고 **여러 클래스에 공통 계약**을 부여하려면 → 인터페이스

---

# 🔍 is / as 연산자

런타임에 객체의 실제 타입을 확인하거나 변환할 때 사용합니다.

| 연산자 | 설명 |
|--------|------|
| `is` | 객체가 특정 타입인지 확인 (`bool` 반환) |
| `as` | 객체를 특정 타입으로 변환 (실패하면 `null`) |

---

## 🐕🐈 예제 4 — is / as 연산자

```csharp
using System;

class Animal { public string name = "동물"; }
class Dog : Animal { public void Bark() { Console.WriteLine("왈왈!"); } }
class Cat : Animal { public void Purr()  { Console.WriteLine("그릉!"); } }

class Program
{
    static void Main()
    {
        Animal[] animals = { new Dog(), new Cat(), new Dog() };

        foreach (Animal a in animals)
        {
            if (a is Dog dog)   // is로 타입 확인 + 동시에 변환
            {
                Console.Write("강아지 발견 → ");
                dog.Bark();
            }
            else if (a is Cat cat)
            {
                Console.Write("고양이 발견 → ");
                cat.Purr();
            }
        }
    }
}
```

**실행 결과**
```
강아지 발견 → 왈왈!
고양이 발견 → 그릉!
강아지 발견 → 왈왈!
```

---

# 💳 종합 예제 — 결제 시스템

```csharp
using System;

interface IPayable
{
    void Pay(int amount);
    string GetPaymentInfo();
}

class CreditCard : IPayable
{
    string cardNumber;

    public CreditCard(string cardNumber)
    {
        this.cardNumber = cardNumber;
    }

    public void Pay(int amount)
    {
        Console.WriteLine($"신용카드({cardNumber})로 {amount:N0}원 결제");
    }

    public string GetPaymentInfo()
    {
        return $"신용카드: {cardNumber}";
    }
}

class KakaoPay : IPayable
{
    string phoneNumber;

    public KakaoPay(string phoneNumber)
    {
        this.phoneNumber = phoneNumber;
    }

    public void Pay(int amount)
    {
        Console.WriteLine($"카카오페이({phoneNumber})로 {amount:N0}원 결제");
    }

    public string GetPaymentInfo()
    {
        return $"카카오페이: {phoneNumber}";
    }
}

class Cash : IPayable
{
    public void Pay(int amount)
    {
        Console.WriteLine($"현금으로 {amount:N0}원 결제");
    }

    public string GetPaymentInfo()
    {
        return "현금";
    }
}

class Program
{
    static void ProcessPayment(IPayable payment, int amount)
    {
        Console.WriteLine($"결제 수단: {payment.GetPaymentInfo()}");
        payment.Pay(amount);
        Console.WriteLine("결제 완료!\n");
    }

    static void Main()
    {
        IPayable[] methods = {
            new CreditCard("1234-5678"),
            new KakaoPay  ("010-1234-5678"),
            new Cash      ()
        };

        int[] amounts = { 15000, 8900, 3000 };

        for (int i = 0; i < methods.Length; i++)
        {
            ProcessPayment(methods[i], amounts[i]);
        }
    }
}
```

**실행 결과**
```
결제 수단: 신용카드: 1234-5678
신용카드(1234-5678)로 15,000원 결제
결제 완료!

결제 수단: 카카오페이: 010-1234-5678
카카오페이(010-1234-5678)로 8,900원 결제
결제 완료!

결제 수단: 현금
현금으로 3,000원 결제
결제 완료!
```

### 핵심

- 새로운 결제 수단이 생겨도 `IPayable`만 구현하면 **기존 코드를 수정하지 않아도** 됨
- 이것이 인터페이스의 가장 큰 장점, **확장성**

---

# 📊 OOP 4대 원칙 총정리

9강부터 12강까지 배운 내용을 객체지향의 4대 원칙으로 정리합니다.

| 원칙 | 개념 | 배운 강의 |
|------|------|-----------|
| **캡슐화** | 데이터를 숨기고 메서드로만 접근 | 10강 (접근 제한자, 프로퍼티) |
| **상속** | 부모의 기능을 물려받아 재사용 | 11강 (Inheritance) |
| **다형성** | 같은 코드가 객체에 따라 다르게 동작 | 12강 (virtual / override) |
| **추상화** | 공통 특징을 뽑아 설계도 만들기 | 9강 (Class), 12강 (abstract, interface) |

---

# 📝 학습 체크

### 문제 1

다음 코드의 출력 결과는 무엇인가요?

```csharp
abstract class Vehicle
{
    public abstract void Move();
    public void Stop() { Console.WriteLine("멈춥니다."); }
}

class Bicycle : Vehicle
{
    public override void Move() { Console.WriteLine("페달을 밟습니다."); }
}

Vehicle v = new Bicycle();
v.Move();
v.Stop();
```

<details>
<summary>정답 보기</summary>

```
페달을 밟습니다.
멈춥니다.
```

추상 클래스를 부모 타입으로 참조해도, `override`된 메서드는 자식 메서드가 호출됩니다.

</details>

---

### 문제 2

다음 중 **인터페이스**를 사용하기에 더 적합한 상황을 고르고, 이유를 설명하세요.

```
① 동물(Animal) 클래스를 만들고, 개·고양이가 공통으로 먹고 자는 기능을 공유
② 프린터·스캐너·팩스가 각각 다른 방식으로 문서를 처리하지만,
   모두 "문서처리" 기능을 반드시 가져야 함
```

<details>
<summary>정답 보기</summary>

**②번**이 인터페이스에 적합합니다.

①은 공통 구현(`Eat()`, `Sleep()`)을 공유하므로 **추상 클래스**가 적합합니다.  
②는 구현 방식이 모두 다르고, 단순히 "문서처리 기능이 있어야 한다"는 **규격(계약)** 만 필요하므로 인터페이스가 적합합니다. 또한 프린터·스캐너·팩스는 서로 "is-a" 관계가 아닙니다.

</details>

---

### 문제 3

`IResizable` 인터페이스를 정의하고, `Circle`과 `Rectangle` 클래스가 이를 구현하도록 작성하세요.

- `IResizable`: `void Resize(double factor)` 메서드 포함
- `Resize()`: 크기 관련 필드에 `factor`를 곱하여 확대/축소

<details>
<summary>정답 보기</summary>

```csharp
interface IResizable
{
    void Resize(double factor);
}

class Circle : IResizable
{
    public double radius;
    public Circle(double r) { radius = r; }

    public void Resize(double factor)
    {
        radius *= factor;
        Console.WriteLine($"원 반지름 변경 → {radius}");
    }
}

class Rectangle : IResizable
{
    public double width, height;
    public Rectangle(double w, double h) { width = w; height = h; }

    public void Resize(double factor)
    {
        width  *= factor;
        height *= factor;
        Console.WriteLine($"사각형 크기 변경 → {width} x {height}");
    }
}

// 사용
Circle    c = new Circle(5);
Rectangle r = new Rectangle(4, 6);

c.Resize(2.0);  // 원 반지름 변경 → 10
r.Resize(0.5);  // 사각형 크기 변경 → 2 x 3
```

</details>

---

# ⏭ 다음 강의 예고

- 컬렉션 (List, Dictionary)
- 제네릭 (`<T>`)
- LINQ 기초

---

<div align="center">

## 🚀 Practice Makes Perfect

객체지향은 암기가 아니라 **반복 실습으로 이해하는 구조입니다.**

</div>
