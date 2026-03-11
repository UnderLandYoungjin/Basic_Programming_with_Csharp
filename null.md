# 💰 C# WinForms 배열 프로젝트 - 심플 지출 관리 앱

> **대상**: C# 초급~중급 (WinForms 기초를 배운 학생)  
> **소요 시간**: 2~3시간  
> **학습 목표**: `List<T>`, 배열 정렬/검색/집계, CSV 파일 입출력  
> **수익화**: 소상공인 맞춤 납품(5~15만원), 크몽/숨고 판매, 기능 확장 후 앱 출시

---

## 📌 완성 앱 미리보기

```
┌──────────────────────────────────────────────────────────┐
│  📊 심플 지출 관리                                         │
│ ┌──────────────────────── 지출 입력 ──────────────────────┐ │
│ │ 날짜: [2025-03-11 ▼]  분류: [식비 ▼]  금액: [     ]   │ │
│ │ 메모: [                              ]  [➕추가] [🗑삭제]│ │
│ └────────────────────────────────────────────────────────┘ │
│                                                            │
│ ┌─ 날짜 ──────┬─ 분류 ─┬─── 금액 ───┬─── 메모 ─────────┐ │
│ │ 2025-03-11  │ 식비   │   12,500원 │ 점심 김치찌개     │ │
│ │ 2025-03-11  │ 교통   │    1,400원 │ 버스              │ │
│ │ 2025-03-10  │ 쇼핑   │   35,000원 │ 마트 장보기       │ │
│ │ 2025-03-10  │ 문화   │   15,000원 │ 영화              │ │
│ └─────────────┴────────┴────────────┴──────────────────┘ │
│                                                            │
│ ┌───────────────── 월별 통계 ─────────────────────────────┐ │
│ │ 총 지출: 63,900원  │ 건당 평균: 15,975원 │ 최대: 35,000원│ │
│ └────────────────────────────────────────────────────────┘ │
│                                                            │
│ [📁 CSV 내보내기] [📂 CSV 불러오기]    [2025년 3월 ▼]      │
└──────────────────────────────────────────────────────────┘
```

---

## 🛠 Step 1: 프로젝트 생성

### 1-1. Visual Studio에서 새 프로젝트

1. Visual Studio 실행
2. 시작 화면에서 **새 프로젝트 만들기** 클릭
3. 검색창에 `winforms` 입력
4. **Windows Forms 앱(.NET Framework)** 선택 → C# 확인 → **다음**
5. 프로젝트 이름: `SimpleExpenseManager`
6. 위치: 원하는 폴더 선택
7. 프레임워크: **.NET Framework 4.8**
8. **만들기** 클릭

> 💡 **.NET Framework** 버전을 선택해야 합니다! (.NET 8 아님)  
> .NET Framework가 없으면 Visual Studio Installer에서 ".NET 데스크탑 개발" 워크로드를 설치하세요.

---

## 🎨 Step 2: 폼(Form) 기본 설정

디자이너에서 `Form1`의 빈 공간을 클릭하고, 오른쪽 **속성(Properties)** 창에서 아래 값을 변경합니다.

| 속성 | 값 | 설명 |
|------|-----|------|
| `(Name)` | `Form1` | 기본값 유지 |
| `Text` | `📊 심플 지출 관리` | 타이틀바에 표시될 텍스트 |
| `Size` | `820, 640` | 폼 크기 (가로, 세로) |
| `StartPosition` | `CenterScreen` | 화면 중앙에 표시 |
| `FormBorderStyle` | `FixedSingle` | 크기 조절 불가 (깔끔한 UI) |
| `MaximizeBox` | `False` | 최대화 버튼 비활성 |
| `Font` | `맑은 고딕, 9pt` | 폼 전체 기본 폰트 |

> 🔍 **속성 창이 안 보이면**: 메뉴 → `보기(View)` → `속성 창(Properties Window)` 또는 `F4` 키

---

## 🎨 Step 3: 컨트롤 배치 (드래그 & 드롭 상세 가이드)

### 3-0. 도구 상자(Toolbox) 열기

```
메뉴 → 보기(View) → 도구 상자(Toolbox)
또는 단축키: Ctrl + Alt + X
```

왼쪽에 도구 상자 패널이 나타납니다. **[공용 컨트롤]** 과 **[컨테이너]** 를 주로 사용합니다.

---

### 3-1. 입력 영역 - GroupBox + 내부 컨트롤

#### GroupBox 배치

1. 도구 상자 → **[컨테이너]** 섹션 펼치기
2. `GroupBox`를 찾아 폼 위로 **드래그 & 드롭**
3. 속성 설정:

| 속성 | 값 |
|------|-----|
| `(Name)` | `grpInput` |
| `Text` | `지출 입력` |
| `Location` | `12, 12` |
| `Size` | `780, 100` |

#### GroupBox 안에 컨트롤 넣기 (중요!)

> ⚠️ **핵심**: GroupBox를 먼저 클릭해서 **선택된 상태**로 만든 후, 도구 상자에서 컨트롤을 GroupBox **영역 안**으로 드래그해야 합니다. GroupBox 바깥에 놓으면 자식이 되지 않습니다!

**안에 넣을 컨트롤들 (순서대로 드래그):**

| # | 컨트롤 | (Name) | 주요 속성 | 위치/크기 |
|---|--------|--------|----------|----------|
| 1 | Label | `lblDate` | Text: `날짜:` | Location: `15, 30` |
| 2 | DateTimePicker | `dtpDate` | Format: `Short` | Location: `55, 27`, Width: `130` |
| 3 | Label | `lblCategory` | Text: `분류:` | Location: `200, 30` |
| 4 | ComboBox | `cboCategory` | DropDownStyle: `DropDownList` | Location: `240, 27`, Width: `90` |
| 5 | Label | `lblAmount` | Text: `금액:` | Location: `345, 30` |
| 6 | TextBox | `txtAmount` | (기본) | Location: `385, 27`, Width: `110` |
| 7 | Label | `lblMemo` | Text: `메모:` | Location: `15, 65` |
| 8 | TextBox | `txtMemo` | (기본) | Location: `55, 62`, Width: `440` |
| 9 | Button | `btnAdd` | Text: `➕ 추가`, BackColor: `PaleGreen` | Location: `560, 22`, Size: `100, 32` |
| 10 | Button | `btnDelete` | Text: `🗑 삭제`, BackColor: `MistyRose` | Location: `670, 22`, Size: `100, 32` |

**각 컨트롤 배치 상세 과정 (Label 예시):**

```
1. 도구 상자 → [공용 컨트롤] 펼치기
2. "Label" 을 찾는다
3. Label을 마우스 왼쪽 버튼으로 누른 채 → grpInput 안쪽으로 드래그
4. 원하는 위치에서 마우스 버튼을 놓는다 (드롭)
5. 방금 놓은 Label이 선택된 상태에서 → 오른쪽 속성 창 확인
6. (Name) 속성을 "lblDate" 로 변경
7. Text 속성을 "날짜:" 로 변경
8. Location 속성을 "15, 30" 으로 변경
```

**DateTimePicker 배치:**

```
1. 도구 상자 → [공용 컨트롤] → "DateTimePicker" 드래그
2. grpInput 안, lblDate 오른쪽에 드롭
3. 속성 창에서:
   - (Name): dtpDate
   - Format: Short   ← 드롭다운에서 선택 (날짜만 표시)
   - Location: 55, 27
   - Width: 130      ← Size 속성의 첫 번째 값
```

**ComboBox 배치:**

```
1. 도구 상자 → "ComboBox" 드래그 → grpInput 안에 드롭
2. 속성 창에서:
   - (Name): cboCategory
   - DropDownStyle: DropDownList  ← 직접 입력 불가, 선택만 가능
   - Location: 240, 27
   - Width: 90
   ※ Items 속성은 코드에서 추가할 예정 (여기서는 비워둡니다)
```

---

### 3-2. 데이터 표시 영역 - DataGridView

1. 도구 상자 → **[데이터]** 섹션 또는 **[공용 컨트롤]** → `DataGridView`
2. 폼의 GroupBox 아래쪽 빈 공간으로 드래그 & 드롭

> 💡 DataGridView를 놓으면 작은 화살표 메뉴가 뜰 수 있는데, **Esc** 키로 닫으세요.

| 속성 | 값 |
|------|-----|
| `(Name)` | `dgvExpenses` |
| `Location` | `12, 120` |
| `Size` | `780, 310` |
| `ReadOnly` | `True` |
| `SelectionMode` | `FullRowSelect` |
| `AllowUserToAddRows` | `False` |
| `AllowUserToDeleteRows` | `False` |
| `RowHeadersVisible` | `False` |
| `BackgroundColor` | `White` |
| `AutoSizeColumnsMode` | `Fill` |

> ※ `SelectionMode`를 `FullRowSelect`로 해야 행 전체가 선택되어 삭제 기능이 편리합니다.

---

### 3-3. 통계 영역 - GroupBox

1. 도구 상자 → **[컨테이너]** → `GroupBox` → 폼 하단에 드래그

| 속성 | 값 |
|------|-----|
| `(Name)` | `grpStats` |
| `Text` | `월별 통계` |
| `Location` | `12, 438` |
| `Size` | `780, 55` |

**GroupBox 안에 Label 3개 배치:**

| # | (Name) | Text | Font | Location (안) |
|---|--------|------|------|--------------|
| 1 | `lblTotal` | `총 지출: 0원` | `맑은 고딕, 10pt, Bold` | `20, 22` |
| 2 | `lblAvg` | `건당 평균: 0원` | `맑은 고딕, 9pt` | `280, 24` |
| 3 | `lblMax` | `최대 지출: 0원` | `맑은 고딕, 9pt` | `520, 24` |

> Font 변경 방법: 속성 창에서 Font 옆의 `...` 버튼 클릭 → 글꼴 대화상자에서 설정

---

### 3-4. 하단 버튼 영역

GroupBox 아래에 버튼과 콤보박스를 배치합니다.

| # | 컨트롤 | (Name) | 주요 속성 | 위치/크기 |
|---|--------|--------|----------|----------|
| 1 | Button | `btnExport` | Text: `📁 CSV 내보내기` | Location: `12, 505`, Size: `140, 35` |
| 2 | Button | `btnLoad` | Text: `📂 CSV 불러오기` | Location: `162, 505`, Size: `140, 35` |
| 3 | Label | `lblMonth` | Text: `조회 월:` | Location: `580, 513` |
| 4 | ComboBox | `cboMonth` | DropDownStyle: `DropDownList` | Location: `635, 510`, Width: `155` |

---

### 3-5. 최종 배치 확인

모든 컨트롤을 배치한 후 **Ctrl+S** 로 저장하고, 디자이너에서 전체 레이아웃을 확인합니다.

**컨트롤 정렬 팁:**

```
여러 컨트롤 선택: Ctrl 키를 누른 채 하나씩 클릭
정렬: 메뉴 → 서식(Format) → 맞춤(Align) → 위쪽/왼쪽 등
간격: 메뉴 → 서식(Format) → 가로 간격/세로 간격 → 같게
```

**잘못 배치했을 때:**

```
- 컨트롤이 GroupBox 밖에 있으면: 컨트롤 선택 → Ctrl+X → GroupBox 클릭 → Ctrl+V
- 위치가 안 맞으면: 속성 창에서 Location 값을 직접 숫자로 입력
- 삭제: 컨트롤 선택 → Delete 키
```

---

## 💻 Step 4: 코드 작성

### 4-1. 데이터 클래스 추가

솔루션 탐색기에서 프로젝트 오른쪽 클릭 → **추가** → **클래스** → 이름: `ExpenseItem.cs`

```csharp
using System;

namespace SimpleExpenseManager
{
    /// <summary>
    /// 지출 항목 하나를 나타내는 클래스
    /// List<ExpenseItem> 형태로 배열처럼 관리됩니다.
    /// </summary>
    public class ExpenseItem
    {
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public string Memo { get; set; }

        /// <summary>
        /// CSV 한 줄로 변환
        /// </summary>
        public string ToCsvLine()
        {
            // 메모에 쉼표가 있을 수 있으므로 따옴표로 감싸기
            string safeMemo = Memo.Contains(",") ? $"\"{Memo}\"" : Memo;
            return $"{Date:yyyy-MM-dd},{Category},{Amount},{safeMemo}";
        }

        /// <summary>
        /// CSV 한 줄에서 ExpenseItem 생성
        /// </summary>
        public static ExpenseItem FromCsvLine(string line)
        {
            // ★ Split으로 문자열을 배열로 분리
            string[] parts = line.Split(',');

            if (parts.Length < 3)
                throw new FormatException("CSV 형식이 올바르지 않습니다.");

            return new ExpenseItem
            {
                Date = DateTime.Parse(parts[0].Trim()),
                Category = parts[1].Trim(),
                Amount = decimal.Parse(parts[2].Trim()),
                Memo = parts.Length > 3 ? parts[3].Trim().Trim('"') : ""
            };
        }

        public override string ToString()
        {
            return $"[{Date:MM/dd}] {Category} {Amount:#,##0}원 - {Memo}";
        }
    }
}
```

---

### 4-2. Form1.cs 전체 코드

디자이너에서 폼의 빈 공간을 더블클릭하면 `Form1_Load` 이벤트가 생성됩니다.
아래 전체 코드를 `Form1.cs`에 입력합니다.

> ⚠️ **이벤트 연결 주의**: 코드를 먼저 붙여넣으면 이벤트가 자동 연결되지 않습니다.  
> **Step 5**에서 이벤트 연결 방법을 반드시 따라하세요!

```csharp
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleExpenseManager
{
    public partial class Form1 : Form
    {
        // ============================================================
        // ★★★ 핵심 배열 개념 ★★★
        // List<T>는 C#의 "동적 배열"입니다.
        // 일반 배열(int[])은 크기가 고정이지만,
        // List<T>는 Add/Remove로 크기가 자동 조절됩니다.
        // ============================================================
        private List<ExpenseItem> expenses = new List<ExpenseItem>();

        // 카테고리 목록 - 고정 배열 (readonly)
        private readonly string[] categories = {
            "식비", "교통", "쇼핑", "의료", "교육", "문화", "공과금", "통신", "기타"
        };

        public Form1()
        {
            InitializeComponent();
        }

        // ===================================================================
        //  폼 로드 - 초기 설정
        // ===================================================================
        private void Form1_Load(object sender, EventArgs e)
        {
            // ★ 배열을 ComboBox에 한번에 추가 (AddRange)
            cboCategory.Items.AddRange(categories);
            cboCategory.SelectedIndex = 0;

            // 월 선택 콤보박스 초기화
            InitializeMonthComboBox();

            // DataGridView 컬럼 설정
            SetupDataGridView();

            // 금액 입력칸에 숫자만 입력되도록
            txtAmount.KeyPress += TxtAmount_KeyPress;
        }

        /// <summary>
        /// 월 선택 콤보박스를 현재 연도 기준 12개월로 초기화
        /// </summary>
        private void InitializeMonthComboBox()
        {
            cboMonth.Items.Clear();
            int year = DateTime.Now.Year;

            for (int month = 1; month <= 12; month++)
            {
                cboMonth.Items.Add($"{year}년 {month}월");
            }

            // 현재 월 선택
            cboMonth.SelectedIndex = DateTime.Now.Month - 1;
        }

        /// <summary>
        /// DataGridView 컬럼 구성
        /// </summary>
        private void SetupDataGridView()
        {
            dgvExpenses.Columns.Clear();

            // 컬럼 추가
            dgvExpenses.Columns.Add("colDate", "날짜");
            dgvExpenses.Columns.Add("colCategory", "분류");
            dgvExpenses.Columns.Add("colAmount", "금액");
            dgvExpenses.Columns.Add("colMemo", "메모");

            // 컬럼 너비 설정
            dgvExpenses.Columns["colDate"].Width = 120;
            dgvExpenses.Columns["colCategory"].Width = 80;
            dgvExpenses.Columns["colAmount"].Width = 130;
            dgvExpenses.Columns["colMemo"].Width = 430;

            // 금액 컬럼 오른쪽 정렬
            dgvExpenses.Columns["colAmount"].DefaultCellStyle.Alignment
                = DataGridViewContentAlignment.MiddleRight;

            // 금액 컬럼 폰트 색상
            dgvExpenses.Columns["colAmount"].DefaultCellStyle.ForeColor
                = Color.DarkBlue;

            // 줄 번갈아 색상 (가독성 향상)
            dgvExpenses.AlternatingRowsDefaultCellStyle.BackColor
                = Color.FromArgb(248, 248, 255);

            // 선택 행 색상
            dgvExpenses.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 220, 255);
            dgvExpenses.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        /// <summary>
        /// 금액 입력칸: 숫자와 백스페이스만 허용
        /// </summary>
        private void TxtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true; // 입력 차단
            }
        }

        // ===================================================================
        //  ➕ 추가 버튼 클릭
        // ===================================================================
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // --- 유효성 검사 ---
            if (string.IsNullOrWhiteSpace(txtAmount.Text))
            {
                MessageBox.Show("금액을 입력해주세요!", "입력 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return;
            }

            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("올바른 금액을 입력해주세요!\n(0보다 큰 숫자)", "입력 오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.SelectAll();
                txtAmount.Focus();
                return;
            }

            // --- 새 항목 생성 ---
            var newItem = new ExpenseItem
            {
                Date = dtpDate.Value.Date,
                Category = cboCategory.SelectedItem.ToString(),
                Amount = amount,
                Memo = txtMemo.Text.Trim()
            };

            // ★ 리스트(동적 배열)에 추가
            expenses.Add(newItem);

            // --- 화면 갱신 ---
            RefreshDataGridView();
            UpdateStatistics();

            // --- 입력 필드 초기화 ---
            txtAmount.Text = "";
            txtMemo.Text = "";
            txtAmount.Focus();
        }

        // ===================================================================
        //  🗑 삭제 버튼 클릭
        // ===================================================================
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvExpenses.SelectedRows.Count == 0)
            {
                MessageBox.Show("삭제할 항목을 선택해주세요.\n(행을 클릭하면 선택됩니다)",
                    "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                "선택한 항목을 삭제할까요?",
                "삭제 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // ★ 현재 표시된 필터링된 리스트에서 인덱스 가져오기
                int selectedIndex = dgvExpenses.SelectedRows[0].Index;
                var filteredList = GetFilteredExpenses();

                if (selectedIndex >= 0 && selectedIndex < filteredList.Count)
                {
                    // ★ 원본 리스트에서 해당 항목 제거
                    ExpenseItem itemToRemove = filteredList[selectedIndex];
                    expenses.Remove(itemToRemove);

                    RefreshDataGridView();
                    UpdateStatistics();
                }
            }
        }

        // ===================================================================
        //  ★★★ DataGridView 갱신 - 배열 순회의 핵심 ★★★
        // ===================================================================
        private void RefreshDataGridView()
        {
            dgvExpenses.Rows.Clear();

            // ★ 필터링된 리스트를 가져와서 순회 (foreach = 배열 탐색)
            List<ExpenseItem> filtered = GetFilteredExpenses();

            foreach (ExpenseItem item in filtered)
            {
                dgvExpenses.Rows.Add(
                    item.Date.ToString("yyyy-MM-dd (ddd)"),
                    item.Category,
                    item.Amount.ToString("#,##0") + "원",
                    item.Memo
                );
            }
        }

        // ===================================================================
        //  ★★★ 필터링 - LINQ를 이용한 배열 검색/정렬 ★★★
        // ===================================================================
        private List<ExpenseItem> GetFilteredExpenses()
        {
            int selectedMonth = cboMonth.SelectedIndex + 1;
            int year = DateTime.Now.Year;

            // ★ LINQ: Where (조건 검색) + OrderByDescending (내림차순 정렬)
            // 이것은 배열의 for문 검색 + Array.Sort를 한 줄로 표현한 것!
            return expenses
                .Where(item => item.Date.Year == year && item.Date.Month == selectedMonth)
                .OrderByDescending(item => item.Date)
                .ThenByDescending(item => item.Amount)
                .ToList();

            // 위 LINQ는 아래 for문과 동일한 동작:
            // List<ExpenseItem> result = new List<ExpenseItem>();
            // for (int i = 0; i < expenses.Count; i++)
            // {
            //     if (expenses[i].Date.Year == year && expenses[i].Date.Month == selectedMonth)
            //         result.Add(expenses[i]);
            // }
            // result.Sort((a, b) => b.Date.CompareTo(a.Date)); // 내림차순
            // return result;
        }

        // ===================================================================
        //  ★★★ 통계 계산 - 배열 집계 함수 ★★★
        // ===================================================================
        private void UpdateStatistics()
        {
            List<ExpenseItem> filtered = GetFilteredExpenses();

            if (filtered.Count == 0)
            {
                lblTotal.Text = "총 지출: 0원";
                lblAvg.Text = "건당 평균: 0원";
                lblMax.Text = "최대 지출: 0원";
                return;
            }

            // ★★ 배열/리스트 집계 함수들 ★★
            decimal total = filtered.Sum(x => x.Amount);       // 합계
            decimal average = filtered.Average(x => x.Amount); // 평균
            decimal max = filtered.Max(x => x.Amount);         // 최대값

            // 위 LINQ는 아래 for문과 동일:
            // decimal total = 0;
            // decimal max = 0;
            // for (int i = 0; i < filtered.Count; i++)
            // {
            //     total += filtered[i].Amount;
            //     if (filtered[i].Amount > max) max = filtered[i].Amount;
            // }
            // decimal average = total / filtered.Count;

            lblTotal.Text = $"총 지출: {total:#,##0}원 ({filtered.Count}건)";
            lblAvg.Text = $"건당 평균: {average:#,##0}원";
            lblMax.Text = $"최대 지출: {max:#,##0}원";
        }

        // ===================================================================
        //  월 변경 시 데이터 갱신
        // ===================================================================
        private void cboMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshDataGridView();
            UpdateStatistics();
        }

        // ===================================================================
        //  📁 CSV 내보내기
        // ===================================================================
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (expenses.Count == 0)
            {
                MessageBox.Show("내보낼 데이터가 없습니다.\n먼저 지출을 입력해주세요.",
                    "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "CSV 파일로 내보내기",
                Filter = "CSV 파일 (*.csv)|*.csv",
                FileName = $"지출내역_{DateTime.Now:yyyyMMdd}.csv",
                DefaultExt = "csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // ★ 리스트를 문자열 배열로 변환하여 파일 저장
                    var lines = new List<string>();

                    // 헤더 행
                    lines.Add("날짜,분류,금액,메모");

                    // ★ 리스트 순회 → 각 항목을 CSV 문자열로 변환
                    foreach (ExpenseItem item in expenses)
                    {
                        lines.Add(item.ToCsvLine());
                    }

                    // ★ List<string> → string[] 배열로 변환 후 파일 쓰기
                    File.WriteAllLines(sfd.FileName, lines.ToArray(), Encoding.UTF8);

                    MessageBox.Show(
                        $"CSV 내보내기 완료!\n\n" +
                        $"총 {expenses.Count}건 저장됨\n" +
                        $"파일: {sfd.FileName}",
                        "내보내기 성공",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"파일 저장 중 오류 발생:\n{ex.Message}",
                        "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ===================================================================
        //  📂 CSV 불러오기
        // ===================================================================
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "CSV 파일 불러오기",
                Filter = "CSV 파일 (*.csv)|*.csv|모든 파일 (*.*)|*.*"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // ★★★ 파일을 문자열 배열로 읽기 ★★★
                    string[] lines = File.ReadAllLines(ofd.FileName, Encoding.UTF8);

                    if (lines.Length <= 1)
                    {
                        MessageBox.Show("파일에 데이터가 없습니다.", "알림",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    int successCount = 0;
                    int errorCount = 0;
                    var errors = new List<string>();

                    // ★ 배열 순회: 인덱스 1부터 (0번은 헤더)
                    for (int i = 1; i < lines.Length; i++)
                    {
                        // 빈 줄 건너뛰기
                        if (string.IsNullOrWhiteSpace(lines[i]))
                            continue;

                        try
                        {
                            // ★ 문자열 → Split → 객체 변환
                            ExpenseItem item = ExpenseItem.FromCsvLine(lines[i]);
                            expenses.Add(item);
                            successCount++;
                        }
                        catch
                        {
                            errorCount++;
                            if (errors.Count < 5) // 처음 5개 에러만 기록
                                errors.Add($"  {i + 1}행: {lines[i]}");
                        }
                    }

                    // 화면 갱신
                    RefreshDataGridView();
                    UpdateStatistics();

                    // 결과 메시지
                    string message = $"불러오기 완료!\n\n✅ 성공: {successCount}건";
                    if (errorCount > 0)
                    {
                        message += $"\n❌ 실패: {errorCount}건";
                        message += $"\n\n오류 행:\n{string.Join("\n", errors)}";
                    }

                    MessageBox.Show(message,
                        errorCount > 0 ? "일부 오류 발생" : "불러오기 성공",
                        MessageBoxButtons.OK,
                        errorCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"파일 읽기 오류:\n{ex.Message}",
                        "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
```

---

## 🔗 Step 5: 이벤트 연결 (매우 중요!)

코드를 작성했지만, 버튼 클릭 등의 이벤트가 코드의 메서드와 연결되어야 합니다.

### 방법 A: 디자이너에서 연결 (추천)

각 컨트롤을 **더블클릭**하면 기본 이벤트가 자동 생성됩니다. 하지만 코드를 먼저 작성한 경우에는 **속성 창의 이벤트 탭**을 사용합니다.

```
1. 디자이너에서 컨트롤 선택 (예: btnAdd 클릭)
2. 속성 창 상단의 ⚡ (번개 아이콘) 클릭 → 이벤트 목록 표시
3. 해당 이벤트 찾기 (예: Click)
4. 오른쪽 드롭다운에서 이미 작성한 메서드 선택
```

**연결해야 할 이벤트 목록:**

| 컨트롤 | ⚡ 이벤트 | 연결할 메서드 |
|--------|----------|-------------|
| `Form1` (폼 자체) | `Load` | `Form1_Load` |
| `btnAdd` | `Click` | `btnAdd_Click` |
| `btnDelete` | `Click` | `btnDelete_Click` |
| `btnExport` | `Click` | `btnExport_Click` |
| `btnLoad` | `Click` | `btnLoad_Click` |
| `cboMonth` | `SelectedIndexChanged` | `cboMonth_SelectedIndexChanged` |

### 방법 B: Designer.cs에서 직접 연결

`Form1.Designer.cs` 파일을 열고, `InitializeComponent()` 메서드 안에 다음 코드가 있는지 확인합니다.  
없으면 직접 추가합니다.

```csharp
// Form1.Designer.cs의 InitializeComponent() 안에 아래 줄들이 있어야 합니다:

this.Load += new System.EventHandler(this.Form1_Load);
this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
this.cboMonth.SelectedIndexChanged += new System.EventHandler(this.cboMonth_SelectedIndexChanged);
```

> ⚠️ Designer.cs를 직접 수정할 때는 **매우 주의**하세요! 잘못 수정하면 디자이너가 깨질 수 있습니다.

---

## ▶️ Step 6: 실행 및 테스트

### 빌드 & 실행

```
1. Ctrl + Shift + B → 빌드 (오류 확인)
2. F5 또는 ▶ 버튼 → 디버그 실행
```

### 테스트 시나리오

```
[테스트 1] 데이터 추가
  1. 날짜: 오늘 날짜 (기본)
  2. 분류: "식비" 선택
  3. 금액: 12500 입력
  4. 메모: "점심 김치찌개" 입력
  5. [➕ 추가] 클릭
  → DataGridView에 한 줄 추가되는지 확인
  → 하단 통계가 업데이트되는지 확인

[테스트 2] 여러 건 추가
  - 교통 1400원, 쇼핑 35000원, 문화 15000원 등 추가
  → 통계의 합계, 평균, 최대가 맞는지 계산기로 검증

[테스트 3] 삭제
  1. DataGridView에서 행 하나 클릭 (파란색 선택)
  2. [🗑 삭제] 클릭
  3. "예" 선택
  → 해당 행이 사라지고 통계가 재계산되는지 확인

[테스트 4] CSV 내보내기 & 불러오기
  1. 데이터 3~5건 입력
  2. [📁 CSV 내보내기] 클릭 → 바탕화면에 저장
  3. 저장된 CSV 파일을 메모장으로 열어 내용 확인
  4. 프로그램 종료 후 다시 실행
  5. [📂 CSV 불러오기] 클릭 → 방금 저장한 파일 선택
  → 이전 데이터가 복원되는지 확인

[테스트 5] 월 변경
  1. 다른 월 날짜로 데이터 입력 (예: DateTimePicker 에서 2월 선택)
  2. 하단 콤보박스에서 해당 월로 변경
  → 해당 월 데이터만 표시되는지 확인
```

---

## 📝 배열 학습 포인트 총정리

이 프로젝트에서 배운 배열/리스트 개념을 정리합니다.

### 1. 고정 배열 vs 동적 리스트

```csharp
// 고정 배열: 크기가 정해져 있음
string[] categories = { "식비", "교통", "쇼핑" };  // 크기 3 고정

// 동적 리스트: 크기가 자유롭게 변함
List<ExpenseItem> expenses = new List<ExpenseItem>();
expenses.Add(item);      // 추가 → 크기 +1
expenses.Remove(item);   // 제거 → 크기 -1
expenses.Count;           // 현재 개수
```

### 2. 배열 순회 (탐색)

```csharp
// for문으로 순회 (인덱스 접근)
for (int i = 0; i < expenses.Count; i++)
{
    ExpenseItem item = expenses[i];  // 인덱스로 접근
}

// foreach로 순회 (간편)
foreach (ExpenseItem item in expenses)
{
    // item 사용
}
```

### 3. 배열 검색

```csharp
// 조건에 맞는 요소 찾기 (LINQ)
var result = expenses.Where(x => x.Category == "식비").ToList();

// 포함 여부 확인
bool exists = categories.Contains("식비");  // true
```

### 4. 배열 정렬

```csharp
// LINQ 정렬
var sorted = expenses.OrderByDescending(x => x.Date).ToList();

// Array.Sort (기본 배열)
int[] numbers = { 5, 2, 8, 1 };
Array.Sort(numbers);  // { 1, 2, 5, 8 }
```

### 5. 배열 집계

```csharp
decimal total = expenses.Sum(x => x.Amount);       // 합계
decimal avg = expenses.Average(x => x.Amount);     // 평균
decimal max = expenses.Max(x => x.Amount);         // 최대
decimal min = expenses.Min(x => x.Amount);         // 최소
int count = expenses.Count;                         // 개수
```

### 6. 배열 ↔ 문자열 변환

```csharp
// 문자열 → 배열 (Split)
string line = "2025-03-11,식비,12500,점심";
string[] parts = line.Split(',');  // {"2025-03-11", "식비", "12500", "점심"}

// 배열 → 문자열 (Join)
string[] items = { "사과", "바나나", "딸기" };
string result = string.Join(", ", items);  // "사과, 바나나, 딸기"
```

### 7. 파일 ↔ 배열

```csharp
// 파일 → 문자열 배열 (한 줄씩)
string[] lines = File.ReadAllLines("data.csv");

// 문자열 리스트 → 배열 → 파일
List<string> lines = new List<string>();
File.WriteAllLines("data.csv", lines.ToArray());
```

---

## 🚀 확장 과제 (수익화를 위한 기능 추가 아이디어)

### 난이도 ⭐ (쉬움)

- [ ] 카테고리별 소계 표시 (GroupBy 활용)
- [ ] 최근 입력 내역 자동 저장 (앱 종료 시 CSV 자동 저장)
- [ ] 입력 후 Enter 키로 추가 (KeyDown 이벤트)

### 난이도 ⭐⭐ (보통)

- [ ] 카테고리별 파이 차트 (Chart 컨트롤)
- [ ] 월별 추이 그래프 (꺾은선 그래프)
- [ ] 수입/지출 구분 기능
- [ ] 검색 기능 (메모 키워드 검색)

### 난이도 ⭐⭐⭐ (도전)

- [ ] SQLite DB 연동 (CSV 대신 DB 저장)
- [ ] 엑셀(xlsx) 내보내기
- [ ] 반복 지출 자동 등록 (월세, 통신비 등)
- [ ] 예산 설정 및 초과 알림

> 💡 **수익화 팁**: 기본 기능 무료 배포 → 차트/엑셀/DB 기능을 "프리미엄 버전"으로 판매하는 전략도 가능합니다!

---

## ❓ 자주 발생하는 오류 & 해결

### Q1. `'Form1'에 'btnAdd_Click'에 대한 정의가 없습니다`

```
원인: 이벤트가 연결되지 않았음
해결: Step 5의 이벤트 연결 과정을 다시 확인하세요.
     특히 Designer.cs에서 += new EventHandler(...) 부분 확인
```

### Q2. `'ExpenseItem' 형식 또는 네임스페이스를 찾을 수 없습니다`

```
원인: ExpenseItem.cs 파일이 없거나 namespace가 다름
해결: 
  1. ExpenseItem.cs 파일이 프로젝트에 있는지 확인
  2. 파일 상단의 namespace가 "SimpleExpenseManager"인지 확인
  3. Form1.cs 상단의 namespace도 동일한지 확인
```

### Q3. `System.Drawing을 찾을 수 없습니다`

```
원인: 참조 누락
해결: 솔루션 탐색기 → 참조 → 오른쪽 클릭 → 참조 추가 → System.Drawing 체크
```

### Q4. DataGridView에 빈 행이 하나 표시됨

```
원인: AllowUserToAddRows가 True (기본값)
해결: 속성 창에서 AllowUserToAddRows = False 설정
```

### Q5. CSV 파일 한글이 깨짐

```
원인: 인코딩 문제
해결: File.WriteAllLines/ReadAllLines에서 Encoding.UTF8 사용 (코드에 이미 포함됨)
     메모장에서 열 때: "다른 이름으로 저장" → 인코딩을 "UTF-8"로 변경
```

---

## 📎 프로젝트 파일 구조

```
SimpleExpenseManager/
├── SimpleExpenseManager.sln          ← 솔루션 파일
└── SimpleExpenseManager/
    ├── SimpleExpenseManager.csproj    ← 프로젝트 파일
    ├── Form1.cs                       ← 메인 폼 코드 (이벤트 핸들러)
    ├── Form1.Designer.cs              ← 디자이너 자동 생성 코드
    ├── Form1.resx                     ← 폼 리소스
    ├── ExpenseItem.cs                 ← 지출 항목 데이터 클래스
    ├── Program.cs                     ← 진입점 (자동 생성)
    └── Properties/
        └── AssemblyInfo.cs
```

---

*작성: 한국폴리텍대학 AI소프트웨어융합 과정 | C# WinForms 배열 실습*
