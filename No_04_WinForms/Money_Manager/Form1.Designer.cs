namespace WinFormsApp21
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            grpInput = new GroupBox();
            rdoExpense = new RadioButton();
            rdoIncome = new RadioButton();
            lblDate = new Label();
            dtpDate = new DateTimePicker();
            lblCategory = new Label();
            cboCategory = new ComboBox();
            lblAmount = new Label();
            txtAmount = new TextBox();
            lblMemo = new Label();
            txtMemo = new TextBox();
            btnAdd = new Button();
            btnDelete = new Button();
            dgvItems = new DataGridView();
            grpStats = new GroupBox();
            lblIncome = new Label();
            lblExpense = new Label();
            lblBalance = new Label();
            btnExport = new Button();
            btnLoad = new Button();
            lblMonth = new Label();
            cboMonth = new ComboBox();
            grpInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvItems).BeginInit();
            grpStats.SuspendLayout();
            SuspendLayout();
            // 
            // grpInput
            // 
            grpInput.Controls.Add(rdoExpense);
            grpInput.Controls.Add(rdoIncome);
            grpInput.Controls.Add(lblDate);
            grpInput.Controls.Add(dtpDate);
            grpInput.Controls.Add(lblCategory);
            grpInput.Controls.Add(cboCategory);
            grpInput.Controls.Add(lblAmount);
            grpInput.Controls.Add(txtAmount);
            grpInput.Controls.Add(lblMemo);
            grpInput.Controls.Add(txtMemo);
            grpInput.Controls.Add(btnAdd);
            grpInput.Controls.Add(btnDelete);
            grpInput.Location = new Point(12, 12);
            grpInput.Name = "grpInput";
            grpInput.Size = new Size(780, 100);
            grpInput.TabIndex = 0;
            grpInput.TabStop = false;
            grpInput.Text = "수입 / 지출 입력";
            // 
            // rdoExpense
            // 
            rdoExpense.AutoSize = true;
            rdoExpense.Checked = true;
            rdoExpense.ForeColor = Color.Crimson;
            rdoExpense.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
            rdoExpense.Location = new Point(15, 28);
            rdoExpense.Name = "rdoExpense";
            rdoExpense.Size = new Size(50, 19);
            rdoExpense.TabIndex = 0;
            rdoExpense.TabStop = true;
            rdoExpense.Text = "지출";
            rdoExpense.UseVisualStyleBackColor = true;
            rdoExpense.CheckedChanged += rdoType_CheckedChanged;
            // 
            // rdoIncome
            // 
            rdoIncome.AutoSize = true;
            rdoIncome.ForeColor = Color.RoyalBlue;
            rdoIncome.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
            rdoIncome.Location = new Point(75, 28);
            rdoIncome.Name = "rdoIncome";
            rdoIncome.Size = new Size(50, 19);
            rdoIncome.TabIndex = 1;
            rdoIncome.Text = "수입";
            rdoIncome.UseVisualStyleBackColor = true;
            // 
            // lblDate
            // 
            lblDate.AutoSize = true;
            lblDate.Location = new Point(140, 30);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(37, 15);
            lblDate.TabIndex = 2;
            lblDate.Text = "날짜:";
            // 
            // dtpDate
            // 
            dtpDate.Format = DateTimePickerFormat.Short;
            dtpDate.Location = new Point(180, 27);
            dtpDate.Name = "dtpDate";
            dtpDate.Size = new Size(120, 23);
            dtpDate.TabIndex = 3;
            // 
            // lblCategory
            // 
            lblCategory.AutoSize = true;
            lblCategory.Location = new Point(315, 30);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new Size(37, 15);
            lblCategory.TabIndex = 4;
            lblCategory.Text = "분류:";
            // 
            // cboCategory
            // 
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategory.FormattingEnabled = true;
            cboCategory.Location = new Point(355, 27);
            cboCategory.Name = "cboCategory";
            cboCategory.Size = new Size(90, 23);
            cboCategory.TabIndex = 5;
            // 
            // lblAmount
            // 
            lblAmount.AutoSize = true;
            lblAmount.Location = new Point(460, 30);
            lblAmount.Name = "lblAmount";
            lblAmount.Size = new Size(37, 15);
            lblAmount.TabIndex = 6;
            lblAmount.Text = "금액:";
            // 
            // txtAmount
            // 
            txtAmount.Location = new Point(500, 27);
            txtAmount.Name = "txtAmount";
            txtAmount.Size = new Size(110, 23);
            txtAmount.TabIndex = 7;
            // 
            // lblMemo
            // 
            lblMemo.AutoSize = true;
            lblMemo.Location = new Point(15, 65);
            lblMemo.Name = "lblMemo";
            lblMemo.Size = new Size(37, 15);
            lblMemo.TabIndex = 8;
            lblMemo.Text = "메모:";
            // 
            // txtMemo
            // 
            txtMemo.Location = new Point(55, 62);
            txtMemo.Name = "txtMemo";
            txtMemo.Size = new Size(555, 23);
            txtMemo.TabIndex = 9;
            // 
            // btnAdd
            // 
            btnAdd.BackColor = Color.PaleGreen;
            btnAdd.Location = new Point(625, 22);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(70, 32);
            btnAdd.TabIndex = 10;
            btnAdd.Text = "➕ 추가";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnDelete
            // 
            btnDelete.BackColor = Color.MistyRose;
            btnDelete.Location = new Point(700, 22);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(70, 32);
            btnDelete.TabIndex = 11;
            btnDelete.Text = "🗑 삭제";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += btnDelete_Click;
            // 
            // dgvItems
            // 
            dgvItems.AllowUserToAddRows = false;
            dgvItems.AllowUserToDeleteRows = false;
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.BackgroundColor = Color.White;
            dgvItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvItems.Location = new Point(12, 120);
            dgvItems.Name = "dgvItems";
            dgvItems.ReadOnly = true;
            dgvItems.RowHeadersVisible = false;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.Size = new Size(780, 310);
            dgvItems.TabIndex = 1;
            // 
            // grpStats
            // 
            grpStats.Controls.Add(lblIncome);
            grpStats.Controls.Add(lblExpense);
            grpStats.Controls.Add(lblBalance);
            grpStats.Location = new Point(12, 438);
            grpStats.Name = "grpStats";
            grpStats.Size = new Size(780, 55);
            grpStats.TabIndex = 2;
            grpStats.TabStop = false;
            grpStats.Text = "월별 통계";
            // 
            // lblIncome
            // 
            lblIncome.AutoSize = true;
            lblIncome.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);
            lblIncome.ForeColor = Color.RoyalBlue;
            lblIncome.Location = new Point(20, 22);
            lblIncome.Name = "lblIncome";
            lblIncome.Size = new Size(100, 19);
            lblIncome.TabIndex = 0;
            lblIncome.Text = "수입: 0원";
            // 
            // lblExpense
            // 
            lblExpense.AutoSize = true;
            lblExpense.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);
            lblExpense.ForeColor = Color.Crimson;
            lblExpense.Location = new Point(280, 22);
            lblExpense.Name = "lblExpense";
            lblExpense.Size = new Size(100, 19);
            lblExpense.TabIndex = 1;
            lblExpense.Text = "지출: 0원";
            // 
            // lblBalance
            // 
            lblBalance.AutoSize = true;
            lblBalance.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
            lblBalance.Location = new Point(520, 21);
            lblBalance.Name = "lblBalance";
            lblBalance.Size = new Size(100, 20);
            lblBalance.TabIndex = 2;
            lblBalance.Text = "잔액: 0원";
            // 
            // btnExport
            // 
            btnExport.Location = new Point(12, 505);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(140, 35);
            btnExport.TabIndex = 3;
            btnExport.Text = "📁 CSV 내보내기";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(162, 505);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(140, 35);
            btnLoad.TabIndex = 4;
            btnLoad.Text = "📂 CSV 불러오기";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // lblMonth
            // 
            lblMonth.AutoSize = true;
            lblMonth.Location = new Point(580, 513);
            lblMonth.Name = "lblMonth";
            lblMonth.Size = new Size(50, 15);
            lblMonth.TabIndex = 5;
            lblMonth.Text = "조회 월:";
            // 
            // cboMonth
            // 
            cboMonth.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMonth.FormattingEnabled = true;
            cboMonth.Location = new Point(635, 510);
            cboMonth.Name = "cboMonth";
            cboMonth.Size = new Size(155, 23);
            cboMonth.TabIndex = 6;
            cboMonth.SelectedIndexChanged += cboMonth_SelectedIndexChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(804, 556);
            Controls.Add(cboMonth);
            Controls.Add(lblMonth);
            Controls.Add(btnLoad);
            Controls.Add(btnExport);
            Controls.Add(grpStats);
            Controls.Add(dgvItems);
            Controls.Add(grpInput);
            Font = new Font("맑은 고딕", 9F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "💰 심플 가계부 - 수입/지출 관리";
            Load += Form1_Load;
            grpInput.ResumeLayout(false);
            grpInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvItems).EndInit();
            grpStats.ResumeLayout(false);
            grpStats.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
            ShowIcon = false;
        }

        #endregion

        private GroupBox grpInput;
        private RadioButton rdoExpense;
        private RadioButton rdoIncome;
        private Label lblDate;
        private DateTimePicker dtpDate;
        private Label lblCategory;
        private ComboBox cboCategory;
        private Label lblAmount;
        private TextBox txtAmount;
        private Label lblMemo;
        private TextBox txtMemo;
        private Button btnAdd;
        private Button btnDelete;
        private DataGridView dgvItems;
        private GroupBox grpStats;
        private Label lblIncome;
        private Label lblExpense;
        private Label lblBalance;
        private Button btnExport;
        private Button btnLoad;
        private Label lblMonth;
        private ComboBox cboMonth;
    }
}
