using System.Text;

namespace WinFormsApp21
{
    public partial class Form1 : Form
    {
        // ★ 수입+지출 모두 하나의 리스트로 관리
        private List<MoneyItem> items = new List<MoneyItem>();

        // 지출 카테고리 배열
        private readonly string[] expenseCategories = {
            "식비", "교통", "쇼핑", "의료", "교육", "문화", "공과금", "통신", "기타"
        };

        // 수입 카테고리 배열
        private readonly string[] incomeCategories = {
            "급여", "부업", "용돈", "이자", "환급", "기타수입"
        };

        public Form1()
        {
            InitializeComponent();
        }

        // ===================================================================
        //  폼 로드
        // ===================================================================
        private void Form1_Load(object sender, EventArgs e)
        {
            // 기본: 지출 카테고리로 시작
            UpdateCategoryList();

            // 월 선택 콤보박스
            InitializeMonthComboBox();

            // DataGridView 설정
            SetupDataGridView();

            // 금액 입력칸 숫자만
            txtAmount.KeyPress += TxtAmount_KeyPress;
        }

        /// <summary>
        /// 수입/지출 라디오버튼 변경 시 카테고리 목록 교체
        /// </summary>
        private void rdoType_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCategoryList();
        }

        /// <summary>
        /// 현재 선택된 타입에 맞는 카테고리 배열을 ComboBox에 세팅
        /// </summary>
        private void UpdateCategoryList()
        {
            cboCategory.Items.Clear();

            if (rdoIncome.Checked)
            {
                // ★ 수입 카테고리 배열을 AddRange
                cboCategory.Items.AddRange(incomeCategories);
            }
            else
            {
                // ★ 지출 카테고리 배열을 AddRange
                cboCategory.Items.AddRange(expenseCategories);
            }

            cboCategory.SelectedIndex = 0;
        }

        private void InitializeMonthComboBox()
        {
            cboMonth.Items.Clear();
            int year = DateTime.Now.Year;

            for (int month = 1; month <= 12; month++)
            {
                cboMonth.Items.Add($"{year}년 {month}월");
            }

            cboMonth.SelectedIndex = DateTime.Now.Month - 1;
        }

        private void SetupDataGridView()
        {
            dgvItems.Columns.Clear();

            dgvItems.Columns.Add("colType", "구분");
            dgvItems.Columns.Add("colDate", "날짜");
            dgvItems.Columns.Add("colCategory", "분류");
            dgvItems.Columns.Add("colAmount", "금액");
            dgvItems.Columns.Add("colMemo", "메모");

            dgvItems.Columns["colType"].Width = 55;
            dgvItems.Columns["colDate"].Width = 115;
            dgvItems.Columns["colCategory"].Width = 75;
            dgvItems.Columns["colAmount"].Width = 120;
            dgvItems.Columns["colMemo"].Width = 395;

            // 금액 오른쪽 정렬
            dgvItems.Columns["colAmount"].DefaultCellStyle.Alignment
                = DataGridViewContentAlignment.MiddleRight;

            // 줄 번갈아 색상
            dgvItems.AlternatingRowsDefaultCellStyle.BackColor
                = Color.FromArgb(248, 248, 255);

            // 선택 행 색상
            dgvItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 220, 255);
            dgvItems.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        private void TxtAmount_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        // ===================================================================
        //  ➕ 추가
        // ===================================================================
        private void btnAdd_Click(object sender, EventArgs e)
        {
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

            var newItem = new MoneyItem
            {
                IsIncome = rdoIncome.Checked,
                Date = dtpDate.Value.Date,
                Category = cboCategory.SelectedItem?.ToString() ?? "기타",
                Amount = amount,
                Memo = txtMemo.Text.Trim()
            };

            items.Add(newItem);

            RefreshDataGridView();
            UpdateStatistics();

            txtAmount.Text = "";
            txtMemo.Text = "";
            txtAmount.Focus();
        }

        // ===================================================================
        //  🗑 삭제
        // ===================================================================
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count == 0)
            {
                MessageBox.Show("삭제할 항목을 선택해주세요.\n(행을 클릭하면 선택됩니다)",
                    "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("선택한 항목을 삭제할까요?", "삭제 확인",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int selectedIndex = dgvItems.SelectedRows[0].Index;
                List<MoneyItem> filteredList = GetFilteredItems();

                if (selectedIndex >= 0 && selectedIndex < filteredList.Count)
                {
                    MoneyItem itemToRemove = filteredList[selectedIndex];
                    items.Remove(itemToRemove);

                    RefreshDataGridView();
                    UpdateStatistics();
                }
            }
        }

        // ===================================================================
        //  ★★★ DataGridView 갱신 - 수입은 파란색, 지출은 빨간색 ★★★
        // ===================================================================
        private void RefreshDataGridView()
        {
            dgvItems.Rows.Clear();

            List<MoneyItem> filtered = GetFilteredItems();

            foreach (MoneyItem item in filtered)
            {
                int rowIndex = dgvItems.Rows.Add(
                    item.TypeText,
                    item.Date.ToString("yyyy-MM-dd (ddd)"),
                    item.Category,
                    item.Amount.ToString("#,##0") + "원",
                    item.Memo
                );

                // ★ 수입/지출에 따라 행 색상 다르게
                DataGridViewRow row = dgvItems.Rows[rowIndex];
                if (item.IsIncome)
                {
                    row.Cells["colType"].Style.ForeColor = Color.RoyalBlue;
                    row.Cells["colAmount"].Style.ForeColor = Color.RoyalBlue;
                }
                else
                {
                    row.Cells["colType"].Style.ForeColor = Color.Crimson;
                    row.Cells["colAmount"].Style.ForeColor = Color.Crimson;
                }
            }
        }

        // ===================================================================
        //  ★★★ 필터링 - LINQ 검색/정렬 ★★★
        // ===================================================================
        private List<MoneyItem> GetFilteredItems()
        {
            int selectedMonth = cboMonth.SelectedIndex + 1;
            int year = DateTime.Now.Year;

            return items
                .Where(item => item.Date.Year == year && item.Date.Month == selectedMonth)
                .OrderByDescending(item => item.Date)
                .ThenBy(item => item.IsIncome) // 같은 날짜면 지출 먼저, 수입 나중
                .ThenByDescending(item => item.Amount)
                .ToList();
        }

        // ===================================================================
        //  ★★★ 통계 - 수입 합계, 지출 합계, 잔액 ★★★
        // ===================================================================
        private void UpdateStatistics()
        {
            List<MoneyItem> filtered = GetFilteredItems();

            // ★ Where로 수입/지출 각각 필터링 후 Sum 집계
            decimal totalIncome = filtered
                .Where(x => x.IsIncome)
                .Sum(x => x.Amount);

            decimal totalExpense = filtered
                .Where(x => !x.IsIncome)
                .Sum(x => x.Amount);

            decimal balance = totalIncome - totalExpense;

            lblIncome.Text = $"수입: {totalIncome:#,##0}원";
            lblExpense.Text = $"지출: {totalExpense:#,##0}원";

            // 잔액 색상: 양수=파랑, 음수=빨강
            lblBalance.Text = $"잔액: {balance:#,##0}원";
            if (balance >= 0)
                lblBalance.ForeColor = Color.RoyalBlue;
            else
                lblBalance.ForeColor = Color.Crimson;
        }

        // ===================================================================
        //  월 변경
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
            if (items.Count == 0)
            {
                MessageBox.Show("내보낼 데이터가 없습니다.\n먼저 수입/지출을 입력해주세요.",
                    "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "CSV 파일로 내보내기",
                Filter = "CSV 파일 (*.csv)|*.csv",
                FileName = $"가계부_{DateTime.Now:yyyyMMdd}.csv",
                DefaultExt = "csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var lines = new List<string>();
                    lines.Add("구분,날짜,분류,금액,메모"); // 헤더

                    foreach (MoneyItem item in items)
                    {
                        lines.Add(item.ToCsvLine());
                    }

                    File.WriteAllLines(sfd.FileName, lines.ToArray(), Encoding.UTF8);

                    MessageBox.Show(
                        $"CSV 내보내기 완료!\n\n총 {items.Count}건 저장됨\n파일: {sfd.FileName}",
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

                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(lines[i]))
                            continue;

                        try
                        {
                            MoneyItem item = MoneyItem.FromCsvLine(lines[i]);
                            items.Add(item);
                            successCount++;
                        }
                        catch
                        {
                            errorCount++;
                            if (errors.Count < 5)
                                errors.Add($"  {i + 1}행: {lines[i]}");
                        }
                    }

                    RefreshDataGridView();
                    UpdateStatistics();

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
