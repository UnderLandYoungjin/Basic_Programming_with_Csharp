# 🟣 C# 제11강 — 상속 (Inheritance)

## 📌 개요
**상속(Inheritance)** 은 기존 클래스의 **필드와 메서드를 물려받아** 새로운 클래스를 만드는 기능입니다.  
코드를 반복해서 작성하지 않아도 되고, **공통 기능은 부모에게**, **고유 기능은 자식에게** 두어 구조를 깔끔하게 유지할 수 있습니다.

> 👨‍👩‍👧 **비유:** 부모님의 재산(코드)을 자녀가 물려받고,  
> 자녀는 거기에 자신만의 것을 추가합니다.  
> 부모 것을 그대로 쓰거나, 자신만의 방식으로 바꿔 쓸 수도 있습니다.

---

## 1. 기본 상속

`: 부모클래스` 형태로 상속합니다.

```
class 자식클래스 : 부모클래스
{
    // 부모의 필드·메서드를 자동으로 물려받음
    // 추가 필드·메서드 작성
}
```

```csharp
using System;

// 부모 클래스 (기반 클래스)
class Animal
{
    public string name;
    public int    age;

    public void Eat()
    {
        Console.WriteLine($"{name}가 밥을 먹습니다.");
    }

    public void Sleep()
    {
        Console.WriteLine($"{name}가 잠을 잡니다.");
    }
}

// 자식 클래스 (파생 클래스) — Animal을 상속
class Dog : Animal
{
    public string breed;

    public void Bark()
    {
        Console.WriteLine($"{name}가 짖습니다: 왈왈!");
    }
}

class Cat : Animal
{
    public void Purr()
    {
        Console.WriteLine($"{name}가 그릉그릉거립니다.");
    }
}

class Hello
{
    public static void Main()
    {
        Dog dog = new Dog();
        dog.name  = "초코";
        dog.age   = 3;
        dog.breed = "푸들";

        dog.Eat();    // 부모에서 물려받은 메서드
        dog.Sleep();  // 부모에서 물려받은 메서드
        dog.Bark();   // Dog만의 메서드

        Cat cat = new Cat();
        cat.name = "나비";
        cat.Eat();   // 부모에서 물려받은 메서드
        cat.Purr();  // Cat만의 메서드
    }
}
```

**실행 결과**
```
초코가 밥을 먹습니다.
초코가 잠을 잡니다.
초코가 짖습니다: 왈왈!
나비가 밥을 먹습니다.
나비가 그릉그릉거립니다.
```

---

## 2. base 키워드

`base`는 **부모 클래스의 생성자나 메서드**를 자식에서 호출할 때 사용합니다.

```csharp
using System;

class Animal
{
    public string name;
    public int    age;

    public Animal(string name, int age)
    {
        this.name = name;
        this.age  = age;
        Console.WriteLine($"Animal 생성자 호출: {name}");
    }
}

class Dog : Animal
{
    public string breed;

    // base(...)로 부모 생성자 호출
    public Dog(string name, int age, string breed) : base(name, age)
    {
        this.breed = breed;
        Console.WriteLine($"Dog 생성자 호출: {breed}");
    }

    public void Info()
    {
        Console.WriteLine($"이름: {name}, 나이: {age}, 품종: {breed}");
    }
}

class Hello
{
    public static void Main()
    {
        Dog dog = new Dog("망고", 2, "말티즈");
        dog.Info();
    }
}
```

**실행 결과**
```
Animal 생성자 호출: 망고
Dog 생성자 호출: 말티즈
이름: 망고, 나이: 2, 품종: 말티즈
```

> 💡 **Tip:** 자식 생성자는 항상 부모 생성자를 먼저 호출합니다.  
> `base(...)`를 명시하지 않으면 **기본 생성자(매개변수 없는)** 가 자동 호출됩니다.

---

## 3. 메서드 오버라이딩 (Method Overriding)

부모에게서 물려받은 메서드를 **자식이 자신만의 방식으로 재정의**하는 것입니다.

- 부모 메서드에 `virtual` 키워드를 붙입니다.
- 자식 메서드에 `override` 키워드를 붙입니다.

```csharp
using System;

class Animal
{
    public string name;

    public Animal(string name)
    {
        this.name = name;
    }

    // virtual: 자식이 재정의할 수 있음을 표시
    public virtual void Sound()
    {
        Console.WriteLine($"{name}가 소리를 냅니다.");
    }
}

class Dog : Animal
{
    public Dog(string name) : base(name) { }

    // override: 부모 메서드를 재정의
    public override void Sound()
    {
        Console.WriteLine($"{name}: 왈왈!");
    }
}

class Cat : Animal
{
    public Cat(string name) : base(name) { }

    public override void Sound()
    {
        Console.WriteLine($"{name}: 야옹~");
    }
}

class Cow : Animal
{
    public Cow(string name) : base(name) { }

    public override void Sound()
    {
        Console.WriteLine($"{name}: 음메~");
    }
}

class Hello
{
    public static void Main()
    {
        Animal[] animals = {
            new Dog("초코"),
            new Cat("나비"),
            new Cow("누렁이")
        };

        foreach (Animal a in animals)
        {
            a.Sound();  // 각 객체의 오버라이드된 메서드 호출
        }
    }
}
```

**실행 결과**
```
초코: 왈왈!
나비: 야옹~
누렁이: 음메~
```

> 💡 **Tip:** 같은 `Sound()` 메서드를 호출해도 **객체 종류에 따라 다르게 동작**합니다.  
> 이것이 바로 다음 강에서 배울 **다형성(Polymorphism)** 의 핵심입니다!

---

## 4. base로 부모 메서드 호출

오버라이딩하면서도 부모의 원래 동작을 함께 사용하고 싶을 때 `base.메서드명()`을 사용합니다.

```csharp
using System;

class Vehicle
{
    public virtual void Start()
    {
        Console.WriteLine("시동을 겁니다.");
    }
}

class ElectricCar : Vehicle
{
    public override void Start()
    {
        base.Start();  // 부모 메서드 먼저 실행
        Console.WriteLine("전기모터가 작동합니다. (무소음)");
    }
}

class Hello
{
    public static void Main()
    {
        ElectricCar ec = new ElectricCar();
        ec.Start();
    }
}
```

**실행 결과**
```
시동을 겁니다.
전기모터가 작동합니다. (무소음)
```

---

## 5. sealed — 상속 금지

`sealed` 키워드를 붙이면 **더 이상 상속할 수 없습니다.**

```csharp
sealed class FinalClass
{
    // 이 클래스는 상속 불가
}

// class ChildClass : FinalClass { }  // ❌ 컴파일 에러!
```

> 💡 **Tip:** 중요한 보안 클래스나 변경되어선 안 되는 핵심 클래스에 `sealed`를 사용합니다.

---

## 6. 상속 관계와 형 변환

자식 클래스의 객체는 **부모 타입 변수에 담을 수 있습니다.**

```csharp
using System;

class Shape
{
    public virtual double Area()
    {
        return 0;
    }
}

class Circle : Shape
{
    double radius;
    public Circle(double r) { radius = r; }

    public override double Area()
    {
        return 3.14 * radius * radius;
    }
}

class Square : Shape
{
    double side;
    public Square(double s) { side = s; }

    public override double Area()
    {
        return side * side;
    }
}

class Hello
{
    public static void Main()
    {
        Shape s1 = new Circle(5);   // 부모 타입에 자식 객체 담기
        Shape s2 = new Square(4);

        Console.WriteLine($"원의 넓이:    {s1.Area()}");
        Console.WriteLine($"사각형의 넓이: {s2.Area()}");
    }
}
```

**실행 결과**
```
원의 넓이:    78.5
사각형의 넓이: 16
```

---

## 🧪 예제 — 직원 급여 시스템

```csharp
using System;

class Employee
{
    public string Name { get; set; }
    protected int baseSalary;

    public Employee(string name, int baseSalary)
    {
        Name            = name;
        this.baseSalary = baseSalary;
    }

    public virtual int GetSalary()
    {
        return baseSalary;
    }

    public void PrintSalary()
    {
        Console.WriteLine($"{Name}: {GetSalary():N0}원");
    }
}

class Manager : Employee
{
    private int bonus;

    public Manager(string name, int baseSalary, int bonus)
        : base(name, baseSalary)
    {
        this.bonus = bonus;
    }

    public override int GetSalary()
    {
        return baseSalary + bonus;  // 기본급 + 보너스
    }
}

class PartTimer : Employee
{
    private int hoursWorked;
    private int hourlyRate;

    public PartTimer(string name, int hoursWorked, int hourlyRate)
        : base(name, 0)
    {
        this.hoursWorked = hoursWorked;
        this.hourlyRate  = hourlyRate;
    }

    public override int GetSalary()
    {
        return hoursWorked * hourlyRate;  // 시간 * 시급
    }
}

class Hello
{
    public static void Main()
    {
        Employee[]  staff = {
            new Employee  ("김철수",   3000000),
            new Manager   ("박팀장",   4000000, 1000000),
            new PartTimer ("이알바",   120, 9860)
        };

        Console.WriteLine("=== 이번 달 급여 명세 ===");
        foreach (Employee e in staff)
        {
            e.PrintSalary();
        }
    }
}
```

**실행 결과**
```
=== 이번 달 급여 명세 ===
김철수: 3,000,000원
박팀장: 5,000,000원
이알바: 1,183,200원
```

---

## 🔍 핵심 개념 요약

| 개념 | 키워드 | 설명 |
|---|---|---|
| 상속 | `: 부모클래스` | 부모의 필드·메서드를 물려받음 |
| 부모 호출 | `base` | 부모 생성자·메서드 호출 |
| 재정의 허용 | `virtual` | 자식이 오버라이딩 가능하게 표시 |
| 재정의 | `override` | 부모 메서드를 자식이 재정의 |
| 상속 금지 | `sealed` | 더 이상 상속 불가 |
| 부모 타입 참조 | `부모타입 변수 = new 자식()` | 자식 객체를 부모 타입으로 참조 |

---

## 📝 문제

---

### 문제 1

다음 코드의 출력 결과는 무엇인가요?

```csharp
class A
{
    public virtual void Hello()
    {
        Console.WriteLine("A의 Hello");
    }
}

class B : A
{
    public override void Hello()
    {
        base.Hello();
        Console.WriteLine("B의 Hello");
    }
}

B obj = new B();
obj.Hello();
```

<details>
<summary>정답 보기</summary>

```
A의 Hello
B의 Hello
```

`base.Hello()`로 부모 메서드를 먼저 실행한 후, 자식 메서드가 이어서 실행됩니다.

</details>

---

### 문제 2

`Animal` 클래스를 상속받는 `Bird` 클래스를 작성하세요.  
- 추가 필드: `canFly` (bool, 날 수 있는지)  
- 생성자: `name`, `age`, `canFly` 초기화  
- `Sound()` 오버라이딩: `"{name}: 짹짹!"` 출력  
- `Fly()` 메서드: 날 수 있으면 `"날아갑니다!"`, 없으면 `"날지 못합니다."` 출력

<details>
<summary>정답 보기</summary>

```csharp
class Bird : Animal
{
    public bool canFly;

    public Bird(string name, int age, bool canFly) : base(name, age)
    {
        this.canFly = canFly;
    }

    public override void Sound()
    {
        Console.WriteLine($"{name}: 짹짹!");
    }

    public void Fly()
    {
        Console.WriteLine(canFly ? "날아갑니다!" : "날지 못합니다.");
    }
}
```

</details>

---

### 문제 3

상속에서 `virtual`과 `override`를 반드시 함께 써야 하는 이유를 설명하세요.

<details>
<summary>정답 보기</summary>

`virtual`은 부모 메서드에 **"자식이 재정의할 수 있다"** 고 표시하는 것이고,  
`override`는 자식 메서드에 **"실제로 부모 메서드를 재정의한다"** 고 명시하는 것입니다.  

이 두 키워드를 함께 사용해야 C#이 런타임 시 올바른 메서드를 호출할 수 있습니다.  
`virtual` 없이 `override`를 쓰면 컴파일 에러가 발생합니다.

</details>

---

> 📌 **Tip:**
> - 상속은 **"is-a" 관계**일 때 사용합니다. (개 **is a** 동물 ✅ / 개 **is a** 자동차 ❌)
> - 공통 기능은 **부모 클래스에**, 고유 기능은 **자식 클래스에** 작성합니다.
> - `virtual` / `override`는 항상 쌍으로 사용합니다.
> - `base`로 부모의 생성자와 메서드를 자식에서 호출할 수 있습니다.
