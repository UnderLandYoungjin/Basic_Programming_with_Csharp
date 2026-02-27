# 🟣 C# 제14강 — 파일과 스트림 (File & Stream)

## 📌 개요
지금까지 만든 데이터는 프로그램이 종료되면 모두 사라졌습니다.  
**파일 입출력**을 사용하면 데이터를 **하드디스크에 저장**하고, 나중에 다시 불러올 수 있습니다.

> 💾 **비유:** 지금까지는 칠판에 쓰고 지웠다면,  
> 이제는 **종이 공책에 기록**하는 것입니다.  
> 공책은 프로그램이 꺼져도 내용이 남아 있습니다.

C#에서 파일을 다루는 방법은 크게 두 가지입니다.
- **File 클래스** — 간단한 파일 읽기·쓰기를 한 줄로 처리
- **Stream 클래스** — 대용량 파일이나 세밀한 제어가 필요할 때

---

## 1. File 클래스 — 간편 파일 입출력

`System.IO` 네임스페이스에 포함된 `File` 클래스를 사용하면 파일을 아주 간단하게 다룰 수 있습니다.

### 📌 파일 쓰기 — WriteAllText / WriteAllLines

```csharp
using System;
using System.IO;   // 파일 관련 기능 사용을 위해 필요

class Hello
{
    public static void Main()
    {
        // 한 번에 전체 내용 쓰기
        string content = "안녕하세요!\nC# 파일 입출력 예제입니다.\n즐겁게 공부하세요!";
        File.WriteAllText("hello.txt", content);
        Console.WriteLine("파일 저장 완료!");

        // 여러 줄을 배열로 쓰기
        string[] lines = { "1번째 줄", "2번째 줄", "3번째 줄" };
        File.WriteAllLines("lines.txt", lines);
        Console.WriteLine("여러 줄 저장 완료!");
    }
}
```

**실행 결과**
```
파일 저장 완료!
여러 줄 저장 완료!
```

> 💡 **Tip:** 파일 경로를 `"hello.txt"`처럼 파일 이름만 쓰면  
> 프로그램 실행 폴더에 파일이 생성됩니다.

---

### 📌 파일 읽기 — ReadAllText / ReadAllLines

```csharp
using System;
using System.IO;

class Hello
{
    public static void Main()
    {
        // 전체 내용을 한 번에 읽기
        string content = File.ReadAllText("hello.txt");
        Console.WriteLine("=== 파일 전체 내용 ===");
        Console.WriteLine(content);

        Console.WriteLine();

        // 줄 단위로 배열에 읽기
        string[] lines = File.ReadAllLines("lines.txt");
        Console.WriteLine("=== 줄 단위 읽기 ===");
        for (int i = 0; i < lines.Length; i++)
        {
            Console.WriteLine($"{i + 1}번 줄: {lines[i]}");
        }
    }
}
```

**실행 결과**
```
=== 파일 전체 내용 ===
안녕하세요!
C# 파일 입출력 예제입니다.
즐겁게 공부하세요!

=== 줄 단위 읽기 ===
1번 줄: 1번째 줄
2번 줄: 2번째 줄
3번 줄: 3번째 줄
```

---

### 📌 파일 추가 — AppendAllText

기존 파일을 덮어쓰지 않고 **내용을 이어서 추가**합니다.

```csharp
using System;
using System.IO;

class Hello
{
    public static void Main()
    {
        string logFile = "log.txt";

        // 로그 기록 (이어 쓰기)
        File.AppendAllText(logFile, "2025-07-15 10:00 - 프로그램 시작\n");
        File.AppendAllText(logFile, "2025-07-15 10:05 - 데이터 처리 완료\n");
        File.AppendAllText(logFile, "2025-07-15 10:10 - 프로그램 종료\n");

        Console.WriteLine(File.ReadAllText(logFile));
    }
}
```

**실행 결과**
```
2025-07-15 10:00 - 프로그램 시작
2025-07-15 10:05 - 데이터 처리 완료
2025-07-15 10:10 - 프로그램 종료
```

---

### 📌 파일 존재 확인 / 삭제 / 복사

```csharp
using System;
using System.IO;

class Hello
{
    public static void Main()
    {
        string path = "hello.txt";

        // 존재 확인
        if (File.Exists(path))
        {
            Console.WriteLine("파일이 존재합니다.");

            // 복사
            File.Copy(path, "hello_backup.txt", overwrite: true);
            Console.WriteLine("백업 파일 생성 완료!");

            // 삭제
            // File.Delete(path);
            // Console.WriteLine("파일 삭제 완료!");
        }
        else
        {
            Console.WriteLine("파일이 없습니다.");
        }
    }
}
```

**실행 결과**
```
파일이 존재합니다.
백업 파일 생성 완료!
```

---

## 2. StreamWriter / StreamReader

`File` 클래스보다 **세밀한 제어**가 필요할 때 사용합니다.  
특히 **대용량 파일**을 줄 단위로 처리할 때 메모리를 훨씬 효율적으로 사용할 수 있습니다.

### 📌 StreamWriter — 쓰기

```csharp
using System;
using System.IO;

class Hello
{
    public static void Main()
    {
        // using 블록: 작업 완료 후 파일을 자동으로 닫음
        using (StreamWriter writer = new StreamWriter("score.txt"))
        {
            writer.WriteLine("=== 성적 기록부 ===");
            writer.WriteLine("홍길동 : 92점");
            writer.WriteLine("김영희 : 88점");
            writer.WriteLine("이민준 : 75점");
        }
        Console.WriteLine("성적 파일 저장 완료!");
    }
}
```

**실행 결과**
```
성적 파일 저장 완료!
```

> ⚠️ **주의:** 파일 작업 후에는 반드시 **닫아야(Close)** 합니다.  
> `using` 블록을 사용하면 블록이 끝날 때 **자동으로 파일이 닫힙니다.** 가장 안전한 방법입니다.

---

### 📌 StreamReader — 읽기

```csharp
using System;
using System.IO;

class Hello
{
    public static void Main()
    {
        using (StreamReader reader = new StreamReader("score.txt"))
        {
            string line;

            // ReadLine()이 null을 반환할 때까지 반복
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }
    }
}
```

**실행 결과**
```
=== 성적 기록부 ===
홍길동 : 92점
김영희 : 88점
이민준 : 75점
```

---

### 📌 이어 쓰기 (Append 모드)

`StreamWriter`의 두 번째 인수에 `true`를 전달하면 **기존 내용 뒤에 이어 씁니다.**

```csharp
using System;
using System.IO;

class Hello
{
    public static void Main()
    {
        // append: true → 기존 파일에 이어 쓰기
        using (StreamWriter writer = new StreamWriter("score.txt", append: true))
        {
            writer.WriteLine("박지수 : 95점");
        }

        // 결과 확인
        Console.WriteLine(File.ReadAllText("score.txt"));
    }
}
```

**실행 결과**
```
=== 성적 기록부 ===
홍길동 : 92점
김영희 : 88점
이민준 : 75점
박지수 : 95점
```

---

## 3. 예외 처리와 파일 입출력

파일이 없거나, 경로가 잘못되었거나, 권한이 없을 때 **예외(Exception)** 가 발생합니다.  
`try-catch`로 예외를 처리하면 프로그램이 갑자기 종료되는 것을 막을 수 있습니다.

```csharp
using System;
using System.IO;

class Hello
{
    public static void Main()
    {
        string path = "없는파일.txt";

        try
        {
            string content = File.ReadAllText(path);
            Console.WriteLine(content);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"오류: '{path}' 파일을 찾을 수 없습니다.");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("오류: 파일에 접근할 권한이 없습니다.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"예상치 못한 오류: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("파일 처리 시도 완료.");
        }
    }
}
```

**실행 결과**
```
오류: '없는파일.txt' 파일을 찾을 수 없습니다.
파일 처리 시도 완료.
```

> 💡 **Tip:** `finally` 블록은 예외 발생 여부와 **관계없이 항상 실행**됩니다.  
> 정리 작업(로그 기록 등)을 여기에 두면 좋습니다.

---

## 4. 경로 다루기 — Path 클래스

```csharp
using System;
using System.IO;

class Hello
{
    public static void Main()
    {
        string filePath = @"C:\Users\user\Documents\data.txt";

        Console.WriteLine(Path.GetFileName(filePath));           // data.txt
        Console.WriteLine(Path.GetFileNameWithoutExtension(filePath)); // data
        Console.WriteLine(Path.GetExtension(filePath));          // .txt
        Console.WriteLine(Path.GetDirectoryName(filePath));      // C:\Users\user\Documents

        // 경로 조합 (OS에 맞는 구분자 자동 처리)
        string folder   = @"C:\Users\user\Documents";
        string fileName = "result.txt";
        string combined = Path.Combine(folder, fileName);
        Console.WriteLine(combined);  // C:\Users\user\Documents\result.txt
    }
}
```

**실행 결과**
```
data.txt
data
.txt
C:\Users\user\Documents
C:\Users\user\Documents\result.txt
```

---

## 5. 디렉토리 다루기 — Directory 클래스

```csharp
using System;
using System.IO;

class Hello
{
    public static void Main()
    {
        string dirPath = "MyFolder";

        // 폴더 생성
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
            Console.WriteLine($"'{dirPath}' 폴더 생성 완료!");
        }

        // 폴더 안에 파일 쓰기
        string filePath = Path.Combine(dirPath, "note.txt");
        File.WriteAllText(filePath, "폴더 안에 저장된 파일입니다.");
        Console.WriteLine("파일 저장 완료!");

        // 현재 디렉토리의 파일 목록
        string[] files = Directory.GetFiles(dirPath);
        Console.WriteLine("\n파일 목록:");
        foreach (string f in files)
        {
            Console.WriteLine($"  - {f}");
        }
    }
}
```

**실행 결과**
```
'MyFolder' 폴더 생성 완료!
파일 저장 완료!

파일 목록:
  - MyFolder\note.txt
```

---

## 🧪 예제 — 간단한 메모장 프로그램

```csharp
using System;
using System.IO;

class Notepad
{
    const string FILE_PATH = "memo.txt";

    static void SaveMemo(string text)
    {
        File.AppendAllText(FILE_PATH, text + "\n");
        Console.WriteLine("메모가 저장되었습니다.");
    }

    static void ShowAllMemos()
    {
        if (!File.Exists(FILE_PATH))
        {
            Console.WriteLine("저장된 메모가 없습니다.");
            return;
        }

        string[] memos = File.ReadAllLines(FILE_PATH);
        Console.WriteLine($"=== 전체 메모 ({memos.Length}개) ===");
        for (int i = 0; i < memos.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {memos[i]}");
        }
    }

    static void ClearMemos()
    {
        File.WriteAllText(FILE_PATH, "");
        Console.WriteLine("모든 메모가 삭제되었습니다.");
    }

    public static void Main()
    {
        SaveMemo("오늘 할 일: C# 14강 공부하기");
        SaveMemo("장보기: 우유, 계란, 빵");
        SaveMemo("친구에게 연락하기");

        ShowAllMemos();

        Console.WriteLine();
        ClearMemos();
        ShowAllMemos();
    }
}
```

**실행 결과**
```
메모가 저장되었습니다.
메모가 저장되었습니다.
메모가 저장되었습니다.
=== 전체 메모 (3개) ===
1. 오늘 할 일: C# 14강 공부하기
2. 장보기: 우유, 계란, 빵
3. 친구에게 연락하기

모든 메모가 삭제되었습니다.
저장된 메모가 없습니다.
```

---

## 🔍 핵심 개념 요약

| 클래스 / 메서드 | 설명 |
|---|---|
| `File.WriteAllText(path, text)` | 파일에 텍스트 전체 쓰기 (덮어씀) |
| `File.WriteAllLines(path, arr)` | 파일에 배열 줄 단위로 쓰기 |
| `File.ReadAllText(path)` | 파일 전체를 문자열로 읽기 |
| `File.ReadAllLines(path)` | 파일을 줄 단위 배열로 읽기 |
| `File.AppendAllText(path, text)` | 파일 끝에 내용 이어 쓰기 |
| `File.Exists(path)` | 파일 존재 여부 확인 |
| `File.Copy(src, dst)` | 파일 복사 |
| `File.Delete(path)` | 파일 삭제 |
| `StreamWriter` | 줄 단위 파일 쓰기 (세밀한 제어) |
| `StreamReader` | 줄 단위 파일 읽기 (세밀한 제어) |
| `Path.Combine(a, b)` | 경로 조합 |
| `Directory.Exists(path)` | 폴더 존재 여부 확인 |
| `Directory.CreateDirectory(path)` | 폴더 생성 |

---

## 📝 문제

---

### 문제 1

다음 코드는 어떤 파일을 생성하고, 파일 내용은 무엇인가요?

```csharp
File.WriteAllLines("fruit.txt", new string[] { "사과", "바나나", "딸기" });
```

<details>
<summary>정답 보기</summary>

`fruit.txt` 파일이 생성되며, 내용은 다음과 같습니다.
```
사과
바나나
딸기
```
`WriteAllLines`는 배열의 각 요소를 줄바꿈으로 구분하여 저장합니다.

</details>

---

### 문제 2

`WriteAllText`와 `AppendAllText`의 차이를 설명하세요.

<details>
<summary>정답 보기</summary>

`WriteAllText` — 파일이 이미 존재하면 **기존 내용을 지우고** 새로 씁니다.  
`AppendAllText` — 파일이 이미 존재하면 **기존 내용 뒤에 이어서** 씁니다. 파일이 없으면 새로 만듭니다.

</details>

---

### 문제 3

파일 작업 시 `using` 블록을 사용하는 이유를 설명하세요.

<details>
<summary>정답 보기</summary>

`using` 블록은 블록이 끝날 때 `Dispose()`를 자동 호출하여 **파일을 자동으로 닫아줍니다.**  
파일을 닫지 않으면 다른 프로그램이 해당 파일에 접근할 수 없거나, 메모리 누수가 발생할 수 있습니다.  
`try-finally`로 직접 닫는 것보다 코드가 간결하고 안전합니다.

</details>

---

### 문제 4

이름과 점수를 입력받아 `result.txt` 파일에 `"이름: 점수점"` 형식으로 3명의 데이터를 저장하는 코드를 작성하세요.  
(값은 직접 변수에 지정해도 됩니다.)

<details>
<summary>정답 보기</summary>

```csharp
using System.IO;

string[] names  = { "홍길동", "김영희", "이민준" };
int[]    scores = { 92, 88, 75 };

using (StreamWriter writer = new StreamWriter("result.txt"))
{
    for (int i = 0; i < names.Length; i++)
    {
        writer.WriteLine($"{names[i]}: {scores[i]}점");
    }
}

// 확인
Console.WriteLine(File.ReadAllText("result.txt"));
```

**result.txt 내용**
```
홍길동: 92점
김영희: 88점
이민준: 75점
```

</details>

---

### 문제 5

다음 코드에서 `finally` 블록이 실행되는 경우를 모두 고르세요.

```csharp
try
{
    string content = File.ReadAllText("data.txt");
    Console.WriteLine(content);
}
catch (FileNotFoundException)
{
    Console.WriteLine("파일 없음");
}
finally
{
    Console.WriteLine("작업 완료");
}
```

```
① data.txt가 존재하는 경우
② data.txt가 존재하지 않는 경우
③ 두 경우 모두
```

<details>
<summary>정답 보기</summary>

**③ 두 경우 모두**  
`finally`는 예외 발생 여부와 관계없이 **항상 실행**됩니다.

</details>

---

## 🗺️ 전체 커리큘럼 마무리

지금까지 14강에 걸쳐 C#의 핵심 개념을 모두 배웠습니다!

| 강의 | 주제 |
|---|---|
| 1~8강 | C# 기초 문법 (변수, 자료형, 연산자, 제어문) |
| 9~12강 | 객체지향 프로그래밍 (클래스, 캡슐화, 상속, 다형성) |
| 13~14강 | 입출력과 데이터 처리 (문자열 심화, 파일과 스트림) |

> 🎉 **수고하셨습니다!**  
> 이제 콘솔 프로그램의 기본 구조를 스스로 설계하고 만들 수 있는 수준이 되었습니다.  
> 다음 단계로는 **컬렉션(List, Dictionary)**, **LINQ**, **비동기 프로그래밍**을 공부하면 실전 프로그램 개발에 한 발 더 가까워집니다.

---

> 📌 **Tip:**
> - 파일 작업에는 반드시 `using System.IO;`를 선언하세요.
> - `File` 클래스는 간단한 작업에, `StreamWriter/Reader`는 세밀한 제어가 필요할 때 사용합니다.
> - 파일 작업은 항상 **`try-catch`** 로 예외 처리를 해두는 것이 좋습니다.
> - `using` 블록으로 `StreamWriter/Reader`를 사용하면 파일이 자동으로 닫힙니다.
> - `Path.Combine()`을 사용하면 Windows/Mac/Linux 어디서든 올바른 경로를 만들 수 있습니다.
