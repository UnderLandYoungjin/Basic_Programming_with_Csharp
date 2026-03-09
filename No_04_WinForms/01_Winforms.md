<img width="1376" height="747" alt="image" src="https://github.com/user-attachments/assets/cb892349-074f-43e1-99ac-cd1ed8a2784f" />
<img width="1896" height="1008" alt="image" src="https://github.com/user-attachments/assets/6e8d9d71-d823-4255-ad1c-8cf13d26c414" />

<img width="2536" height="1387" alt="image" src="https://github.com/user-attachments/assets/033ed6f0-317b-453f-a15d-b33c4a0b400d" />


<img width="2507" height="1330" alt="image" src="https://github.com/user-attachments/assets/59e27f79-edfb-4ff4-9dc5-4e5dca5a4d9b" />


# Visual Studio WinForms 화면 구성

## 전체 구조 한눈에 보기

```
┌─────────────┬──────────────────┬──────────────────┐
│  도구 상자   │    UI 화면        │  솔루션 탐색기    │
│  (Toolbox)  │  (Form Designer) │  + 속성 창        │
└─────────────┴──────────────────┴──────────────────┘
```

---

## 각 영역 설명

### 🧰 도구 상자 (왼쪽)
- Button, TextBox, Label 등 **UI 컨트롤 목록**
- 원하는 컨트롤을 **드래그 → UI 화면에 배치**

### 🖥️ UI 화면 (가운데)
- 우리가 만드는 프로그램의 **실제 화면 미리보기**
- 여기서 컨트롤을 배치하면 `Form1.Designer.cs`가 자동 수정됨

### 📁 솔루션 탐색기 (오른쪽 상단)
| 파일 | 역할 |
|------|------|
| `Form1.cs` | 개발자가 직접 작성하는 **사용자 코드** |
| `Form1.Designer.cs` | 디자이너가 자동 생성하는 **UI 코드** (사용자가 조작하지 않는것이 원칙!)|
| `Form1.resx` | 이미지 등 리소스 파일 |
| `Program.cs` | 프로그램 시작점 (`Main`) |

### ⚙️ 속성 창 (오른쪽 하단)
- UI 화면에서 **컨트롤 클릭 시** 해당 객체의 속성 표시
- Text(텍스트), Size(크기), Location(위치) 등을 **GUI로 편집 가능**
- 값 변경 시 `Designer.cs` 코드에 자동 반영

---

## 핵심 흐름

```
도구 상자에서 드래그
        ↓
UI 화면에 컨트롤 배치
        ↓
속성 창에서 속성 수정
        ↓
Form1.Designer.cs 자동 업데이트
        ↓
Form1.cs에서 이벤트 로직 작성
```
