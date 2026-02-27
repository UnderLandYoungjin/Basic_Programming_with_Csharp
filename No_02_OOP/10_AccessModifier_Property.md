# 🟣 C# 제10강 — 접근 제한자와 프로퍼티 (Access Modifier & Property)

## 📌 개요
클래스 내부의 데이터를 **외부에서 함부로 바꾸지 못하도록** 보호하는 것이 객체지향의 중요한 원칙 중 하나입니다.  
이를 **캡슐화(Encapsulation)** 라고 하며, **접근 제한자**와 **프로퍼티**가 그 핵심 도구입니다.

> 🏦 **비유:** 은행 금고는 함부로 열 수 없습니다.  
> 반드시 은행 직원(메서드)을 통해서만 돈을 넣고 뺄 수 있죠.  
> 이것이 바로 캡슐화의 개념입니다.

---

## 1. 접근 제한자 (Access Modifier)

클래스의 필드, 메서드, 클래스 자체에 **접근 가능한 범위**를 지정합니다.

| 접근 제한자 | 접근 가능 범위 |
|---|---|
| `public` | 어디서든 접근 가능 |
| `private` | 같은 클래스 내부에서만 접근 가능 |
| `protected` | 같은 클래스 + 자식 클래스에서 접근 가능 (11강에서 상세히) |

### 📌 public vs private

```csharp
using System;

class Person
{
    public  string name;    // 어디서든 접근 가능
    private int    age;     // 클래스 내부에서만 접근 가능

    public void SetAge(int a)
    {
        if (a < 0 || a > 150)
        {
            Console.WriteLine("유효하지 않은 나이입니다.");
            return;
        }
        age = a;  // 클래스 내부에서는 접근 가능
    }

    public int GetAge()
    {
        return age;
    }
}

class Hello
{
    public static void Main()
    {
        Person p = new Person();
        p.name = "홍길동";   // public → 접근 가능
        // p.age = 25;       // ❌ private → 컴파일 에러!

        p.SetAge(25);        // 메서드를 통해서만 age 설정
        Console.WriteLine($"{p.name}, {p.GetAge()}살");

        p.SetAge(-5);        // 유효하지 않은 값 → 거부됨
    }
}
```

**실행 결과**
```
홍길동, 25살
유효하지 않은 나이입니다.
```

> 💡 **Tip:** 필드는 보통 `private`으로 숨기고, 메서드를 통해서만 접근하도록 설계합니다.  
> 이렇게 하면 **잘못된 값이 들어오는 것을 방지**할 수 있습니다.

---

## 2. 프로퍼티 (Property)

`GetAge()` / `SetAge()` 처럼 getter/setter 메서드를 쌍으로 만드는 것은 번거롭습니다.  
C#은 이를 더 깔끔하게 처리하는 **프로퍼티** 문법을 제공합니다.

```
접근제한자 자료형 프로퍼티이름
{
    get { return 필드; }
    set { 필드 = value; }
}
```

```csharp
using System;

class Person
{
    private string name;
    private int    age;

    // 프로퍼티
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public int Age
    {
        get { return age; }
        set
        {
            if (value < 0 || value > 150)
            {
                Console.WriteLine("유효하지 않은 나이입니다.");
                return;
            }
            age = value;
        }
    }
}

class Hello
{
    public static void Main()
    {
        Person p = new Person();

        p.Name = "김민준";   // set 호출
        p.Age  = 28;         // set 호출

        Console.WriteLine($"{p.Name}, {p.Age}살");  // get 호출

        p.Age = -10;         // 유효성 검사 실패
        Console.WriteLine($"나이: {p.Age}");        // 그대로 28
    }
}
```

**실행 결과**
```
김민준, 28살
유효하지 않은 나이입니다.
나이: 28
```

---

## 3. 자동 프로퍼티 (Auto Property)

단순히 값을 읽고 쓰기만 하면 된다면, **한 줄로 간결하게** 작성할 수 있습니다.

```csharp
using System;

class Product
{
    public string Name  { get; set; }
    public int    Price { get; set; }
    public int    Stock { get; set; }

    public Product(string name, int price, int stock)
    {
        Name  = name;
        Price = price;
        Stock = stock;
    }

    public void PrintInfo()
    {
        Console.WriteLine($"[{Name}] 가격: {Price}원, 재고: {Stock}개");
    }
}

class Hello
{
    public static void Main()
    {
        Product p1 = new Product("노트북", 1200000, 5);
        Product p2 = new Product("마우스",   35000, 30);

        p1.PrintInfo();
        p2.PrintInfo();

        p1.Stock -= 2;  // 재고 감소
        Console.WriteLine($"노트북 남은 재고: {p1.Stock}개");
    }
}
```

**실행 결과**
```
[노트북] 가격: 1200000원, 재고: 5개
[마우스] 가격: 35000원, 재고: 30개
노트북 남은 재고: 3개
```

---

## 4. 읽기 전용 프로퍼티

`set`을 제거하거나 `private set`으로 설정하면 **외부에서 값을 변경하지 못하도록** 막을 수 있습니다.

```csharp
using System;

class Employee
{
    public string Name   { get; set; }
    public int    Salary { get; private set; }  // 외부에서 직접 변경 불가

    public Employee(string name, int salary)
    {
        Name   = name;
        Salary = salary;
    }

    // 연봉 인상은 반드시 이 메서드를 통해서만
    public void RaiseSalary(int amount)
    {
        if (amount > 0)
        {
            Salary += amount;
            Console.WriteLine($"{Name}의 연봉이 {amount}원 인상되었습니다.");
        }
    }
}

class Hello
{
    public static void Main()
    {
        Employee emp = new Employee("이수진", 3000000);
        Console.WriteLine($"{emp.Name}: {emp.Salary}원");

        // emp.Salary = 5000000;  // ❌ private set → 외부 변경 불가!
        emp.RaiseSalary(500000);   // 메서드를 통해서만 변경 가능

        Console.WriteLine($"{emp.Name}: {emp.Salary}원");
    }
}
```

**실행 결과**
```
이수진: 3000000원
이수진의 연봉이 500000원 인상되었습니다.
이수진: 3500000원
```

---

## 5. 정적 멤버 (static)

`static`이 붙은 필드나 메서드는 **객체를 만들지 않고도** 클래스 이름으로 직접 사용할 수 있습니다.  
모든 객체가 **공유하는 데이터나 기능**에 사용합니다.

```csharp
using System;

class Counter
{
    public string name;
    public static int totalCount = 0;  // 모든 객체가 공유

    public Counter(string name)
    {
        this.name = name;
        totalCount++;
        Console.WriteLine($"{name} 생성 (총 {totalCount}개)");
    }

    public static void PrintTotal()
    {
        Console.WriteLine($"전체 생성된 객체 수: {totalCount}");
    }
}

class Hello
{
    public static void Main()
    {
        Counter c1 = new Counter("A");
        Counter c2 = new Counter("B");
        Counter c3 = new Counter("C");

        Counter.PrintTotal();  // 클래스 이름으로 직접 호출
    }
}
```

**실행 결과**
```
A 생성 (총 1개)
B 생성 (총 2개)
C 생성 (총 3개)
전체 생성된 객체 수: 3
```

> 💡 **Tip:** `Console.WriteLine()`, `Math.Abs()` 등 우리가 자주 쓰는 것들이  
> 바로 `static` 메서드입니다. 객체 없이 클래스 이름으로 바로 사용하죠.

---

## 🧪 예제 — 학생 관리 클래스

```csharp
using System;

class Student
{
    public  string Name   { get; set; }
    public  int    Score  { get; private set; }
    private string grade;

    public Student(string name, int score)
    {
        Name  = name;
        SetScore(score);
    }

    public void SetScore(int score)
    {
        if (score < 0 || score > 100)
        {
            Console.WriteLine("점수는 0~100 사이여야 합니다.");
            return;
        }
        Score = score;
        UpdateGrade();
    }

    private void UpdateGrade()
    {
        if      (Score >= 90) grade = "A";
        else if (Score >= 80) grade = "B";
        else if (Score >= 70) grade = "C";
        else if (Score >= 60) grade = "D";
        else                  grade = "F";
    }

    public void PrintInfo()
    {
        Console.WriteLine($"[{Name}] 점수: {Score}점, 등급: {grade}");
    }
}

class Hello
{
    public static void Main()
    {
        Student s1 = new Student("홍길동", 92);
        Student s2 = new Student("김영희", 73);
        Student s3 = new Student("이민준", 150);  // 잘못된 점수

        s1.PrintInfo();
        s2.PrintInfo();

        s2.SetScore(85);   // 점수 수정
        s2.PrintInfo();
    }
}
```

**실행 결과**
```
점수는 0~100 사이여야 합니다.
[홍길동] 점수: 92점, 등급: A
[김영희] 점수: 73점, 등급: C
[김영희] 점수: 85점, 등급: B
```

---

## 🔍 핵심 개념 요약

| 개념 | 키워드 | 설명 |
|---|---|---|
| 공개 접근 | `public` | 어디서든 접근 가능 |
| 비공개 접근 | `private` | 클래스 내부에서만 접근 |
| 프로퍼티 | `get` / `set` | 필드를 안전하게 읽고 쓰는 방법 |
| 자동 프로퍼티 | `{ get; set; }` | 간결한 프로퍼티 선언 |
| 읽기 전용 | `private set` | 외부에서 값 변경 방지 |
| 정적 멤버 | `static` | 객체 없이 클래스로 직접 접근 |

---

## 📝 문제

---

### 문제 1

다음 코드에서 오류가 발생하는 줄을 찾고, 이유를 설명하세요.

```csharp
class Book
{
    public  string Title  { get; set; }
    private int    pages;
}

Book b = new Book();
b.Title = "C# 입문";   // ①
b.pages = 300;          // ②
```

<details>
<summary>정답 보기</summary>

**②번**에서 오류 발생.  
`pages`는 `private`으로 선언되어 있어 클래스 외부에서 접근할 수 없습니다.

</details>

---

### 문제 2

아래 조건에 맞는 `Rectangle` 클래스를 작성하세요.
- `Width`, `Height` : 자동 프로퍼티 (`private set`)
- 생성자에서 초기화
- `Area()` 메서드: 넓이 반환
- `Perimeter()` 메서드: 둘레 반환

<details>
<summary>정답 보기</summary>

```csharp
class Rectangle
{
    public int Width  { get; private set; }
    public int Height { get; private set; }

    public Rectangle(int width, int height)
    {
        Width  = width;
        Height = height;
    }

    public int Area()      => Width * Height;
    public int Perimeter() => (Width + Height) * 2;
}

Rectangle r = new Rectangle(4, 6);
Console.WriteLine($"넓이: {r.Area()}");       // 24
Console.WriteLine($"둘레: {r.Perimeter()}");  // 20
```

</details>

---

### 문제 3

`static` 필드와 일반 필드의 차이를 설명하고, `static`을 사용하기 적합한 상황의 예를 하나 드세요.

<details>
<summary>정답 보기</summary>

**일반 필드:** 각 객체마다 **별도로** 존재하는 데이터  
**static 필드:** 모든 객체가 **공유**하는 하나의 데이터

**예시:** 생성된 객체의 총 개수 카운터, 전체 사용자 수, 고정된 설정값(환율, 세율 등)

</details>

---

> 📌 **Tip:**
> - 필드는 `private`, 외부 접근은 **프로퍼티**를 통해 하도록 설계하는 것이 좋습니다.
> - **자동 프로퍼티** `{ get; set; }` 를 활용하면 코드가 훨씬 간결해집니다.
> - 모든 객체가 공유하는 데이터에는 **`static`** 을 활용하세요.
> - 외부에서 값을 변경하면 안 되는 필드는 **`private set`** 으로 보호하세요.
