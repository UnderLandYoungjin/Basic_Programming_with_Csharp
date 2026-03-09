<img width="796" height="662" alt="image" src="https://github.com/user-attachments/assets/62aef7c7-3959-4bb6-a5db-10d982bd608d" />







# LINQ 실습 - 단어 통계 분석기 (WinForms)

> **학습 목표**: LINQ의 핵심 메서드를 WinForms 실전 예제로 체험한다.

---

## 1. 완성 화면

- RichTextBox에 텍스트 입력
- "분석 실행" 버튼 클릭
- ListView에 단어 빈도 순위표 출력 (1~3위 색상 강조)
- 총 단어 수 / 고유 단어 수 통계 표시

---

## 2. 필요한 컨트롤 (디자이너에서 드래그앤드롭)

| 컨트롤 | Name 속성 | 역할 |
|--------|-----------|------|
| `RichTextBox` | `rtbInput` | 텍스트 입력 영역 |
| `Button` | `btnAnalyze` | 분석 실행 |
| `Button` | `btnClear` | 초기화 |
| `ListView` | `lvResult` | 순위표 출력 |
| `Label` | `lblTotal` | 총 통계 표시 |
| `NumericUpDown` | `nudTop` | 상위 N개 선택 |

> **버튼 이벤트 연결**: 각 버튼을 디자이너에서 더블클릭하거나,  
> 속성창 → ⚡ 이벤트 탭 → Click 항목에 메서드명 직접 입력

---

## 3. 전체 코드

```csharp
using System.Text.RegularExpressions;

namespace WinFormsApp10
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // ListView 초기 설정
            lvResult.Columns.Add("순위", 50);
            lvResult.Columns.Add("단어", 270);
            lvResult.Columns.Add("출연횟수", 180);
            lvResult.View = View.Details;
            lvResult.FullRowSelect = true;

            // NumericUpDown 범위 설정
            nudTop.Minimum = 1;
            nudTop.Maximum = 500;
            nudTop.Value = 10;

            // 샘플 텍스트
            rtbInput.Text = "apple banana apple orange banana apple grape banana orange apple";
        }

        // 분석 실행 버튼
        private void btnAnalyze_Click_1(object sender, EventArgs e)
        {
            string text = rtbInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("텍스트를 입력해주세요!", "알림");
                return;
            }

            // Step 1. 단어 추출 (정규식으로 영문/한글만)
            var words = Regex.Matches(text.ToLower(), @"[a-zA-Z가-힣]+")
                             .Cast<Match>()
                             .Select(m => m.Value)
                             .ToList();

            // Step 2. LINQ 핵심 체이닝
            int topN = (int)nudTop.Value;

            var ranking = words
                .GroupBy(w => w)                                           // 단어별 그룹화
                .Select(g => new { Word = g.Key, Count = g.Count() })      // 단어 + 횟수
                .OrderByDescending(x => x.Count)                           // 많은 순 정렬
                .ThenBy(x => x.Word)                                       // 동점이면 알파벳순
                .Take(topN)                                                // 상위 N개만
                .ToList();

            // Step 3. ListView에 결과 출력
            lvResult.Items.Clear();

            for (int i = 0; i < ranking.Count; i++)
            {
                var item = new ListViewItem((i + 1).ToString());
                item.SubItems.Add(ranking[i].Word);
                item.SubItems.Add(ranking[i].Count.ToString());

                // 1~3위 색상 강조
                if      (i == 0) item.BackColor = System.Drawing.Color.Gold;
                else if (i == 1) item.BackColor = System.Drawing.Color.Silver;
                else if (i == 2) item.BackColor = System.Drawing.Color.Peru;

                lvResult.Items.Add(item);
            }

            // Step 4. 총 통계
            int totalWords  = words.Count;
            int uniqueWords = words.Distinct().Count();

            lblTotal.Text = $"총 단어: {totalWords}개  |  고유 단어: {uniqueWords}개  |  TOP {topN} 표시 중";
        }

        // 초기화 버튼
        private void btnClear_Click_1(object sender, EventArgs e)
        {
            rtbInput.Clear();
            lvResult.Items.Clear();
            lblTotal.Text = "";
        }
    }
}
```

---

## 4. LINQ 핵심 포인트 해설

```csharp
var ranking = words
    .GroupBy(w => w)                      // ① 같은 단어끼리 묶기
    .Select(g => new { ... })             // ② 단어명 + 개수로 변환
    .OrderByDescending(x => x.Count)      // ③ 많은 순으로 정렬
    .ThenBy(x => x.Word)                  // ④ 개수 같으면 알파벳순
    .Take(topN)                           // ⑤ 상위 N개만 자르기
    .ToList();                            // ⑥ 여기서 실제 실행!
```

### 메서드별 역할 요약

| 메서드 | 역할 | SQL 비유 |
|--------|------|----------|
| `GroupBy` | 같은 값끼리 그룹화 | `GROUP BY` |
| `Select` | 원하는 형태로 변환 | `SELECT` |
| `OrderByDescending` | 내림차순 정렬 | `ORDER BY DESC` |
| `ThenBy` | 2차 정렬 기준 추가 | `, 컬럼명 ASC` |
| `Take(n)` | 앞에서 n개만 추출 | `LIMIT n` |
| `Distinct` | 중복 제거 | `DISTINCT` |
| `ToList()` | 실제 실행 (지연 실행 종료) | — |

---

## 5. 지연 실행(Lazy Evaluation) 개념

```csharp
// 이 시점에서는 아직 실행되지 않음 — 쿼리 정의만 함
var ranking = words.GroupBy(...).Select(...).OrderByDescending(...);

// ToList() 호출 시 비로소 실행됨
var result = ranking.ToList();  // ← 실제 실행 시점
```

> 💡 **왜 중요한가?**  
> 데이터베이스(EF Core) 연동 시 `ToList()` 전까지 SQL이 실행되지 않는다.  
> 조건을 추가하면 최적화된 쿼리 한 번으로 처리된다.

---

## 6. 실습 과제

1. `Where`를 추가해서 **2글자 이상 단어만** 통계에 포함시켜보기
2. `Any()`를 사용해서 특정 단어가 텍스트에 있는지 확인하는 버튼 추가하기
3. 결과를 **알파벳순 정렬**로 바꿔보기 (`OrderBy` 사용)
4. 통계에 **최빈 단어 1개**만 MessageBox로 띄워보기 (`First()` 사용)

---

*Korea Polytechnic University | AI Software Convergence | C# 강의자료*




https://biz.heraldcorp.com/article/10689870?site=mapping_hyperlink

```
9일 국제유가가 결국 배럴당 100달러를 넘겼다. 미국-이란 전쟁으로 인한 중동발 ‘오일 쇼크’가 우려대로 현실화했다. 호르무즈 해협 통행이 막히고, 이를 이용하는 주요 산유국들이 원유를 감산하면서 유가 급등세가 이어졌다. 우리 산업계로선 원유를 비롯한 에너지·원자재 수입 뿐 아니라 중동 수출길까지 막히게 돼 치명적인 상황이다. 유가 급등이 고물가·고환율로 이어지면서 회복세에 들어섰던 우리 경제와 증시도 큰 타격이 예상된다. 정부와 기업, 국민이 손잡고 총력대응하지 않으면 극복하기 어려운 위기다.

당장 우리 산업계의 전방위적인 피해가 우려된다. 일단 정유·석유화학·해운업계가 비상이다. 한국은 원유의 70.7%, 액화천연가스(LNG)의 20.4%를 중동으로부터 호르무즈 해협을 통해 들여온다. 원유 뿐 아니라 석유화학 주 제품인 나프타도 중동 수입선에 기대고 있다. 수출 주력 업종이자 우리 경제를 떠받치는 반도체와 자동차도 연쇄적인 피해 영향권에 있다. 반도체 제조 필수 소재인 헬륨 가스와 브롬 가스는 각각 전체 수입량의 65%와 98%를 카타르와 이스라엘에 의존하고 있다. 중동의 데이터센터 건설 지연으로 반도체 수요도 줄어들 수 있다. 중동 자동차 시장 점유율이 10%정도인 현대차그룹의 수출 물량도 감소할 수 있고, 유가 상승은 글로벌 시장의 완성차 수요도 낮출 수 있다.

미국-이란 전쟁이 언제까지 어떤 양상으로 계속될지가 우리 경제엔 관건인데, 현재로선 조기 종전이 불투명한 것은 물론 확전과 장기화 우려까지 크다. 이날 이란은 최고지도자 자리를 미군 공습으로 사망한 아야톨라 세예드 하메네이에 이어 차남인 모즈타바 하메네이로 부자 승계했다. 도널드 트럼프 미 대통령이 “용납할 수 없다”고 직접 겨냥했던 인물로 반미 강경파다. 트럼프 대통령이 이란의 고농축 우라늄 확보를 위한 지상군 투입 등 전략적 선택지에 제한을 두지 않고, 시간에도 구애받지 않겠다는 뜻을 공언해온 상황에서 이란은 미국에 굴복하지 않겠다는 의지를 보인 것이다.

정부는 이재명 대통령 주재로 관련 부처 장관이 참석한 가운데 이날 오전 청와대에서 중동 상황 관련 비상경제점검회의를 여는 등 범부처 총력대응에 들어갔다. 석유·가스 수급 및 가격 안정화, 유가 상승으로 인한 관련 산업 피해지원, 물가·환율 변동에 대응한 민생 안정책 등이 핵심이다. 특히 에너지 대체 수입선과 해외 생산 물량 확보, 단계별 비축유 방출 계획이 중요하다. 정부 뿐 아니라 업계는 유가 급등에 편승한 각종 위법·불법·편법적 이익 추구를 삼가고, 국민들은 가계 차원에서 가능한 수준의 고유가·고물가·고환율 대비를 해야 할 것이다.
```
