namespace WinFormsApp21
{
    /// <summary>
    /// 수입/지출 항목 하나를 나타내는 클래스
    /// </summary>
    public class MoneyItem
    {
        /// <summary>
        /// true = 수입, false = 지출
        /// </summary>
        public bool IsIncome { get; set; }

        public DateTime Date { get; set; }
        public string Category { get; set; } = "";
        public decimal Amount { get; set; }
        public string Memo { get; set; } = "";

        /// <summary>
        /// 수입/지출 구분 텍스트
        /// </summary>
        public string TypeText => IsIncome ? "수입" : "지출";

        /// <summary>
        /// CSV 한 줄로 변환
        /// </summary>
        public string ToCsvLine()
        {
            string safeMemo = Memo.Contains(',') ? $"\"{Memo}\"" : Memo;
            return $"{TypeText},{Date:yyyy-MM-dd},{Category},{Amount},{safeMemo}";
        }

        /// <summary>
        /// CSV 한 줄에서 MoneyItem 생성
        /// </summary>
        public static MoneyItem FromCsvLine(string line)
        {
            string[] parts = line.Split(',');

            if (parts.Length < 4)
                throw new FormatException("CSV 형식이 올바르지 않습니다.");

            return new MoneyItem
            {
                IsIncome = parts[0].Trim() == "수입",
                Date = DateTime.Parse(parts[1].Trim()),
                Category = parts[2].Trim(),
                Amount = decimal.Parse(parts[3].Trim()),
                Memo = parts.Length > 4 ? parts[4].Trim().Trim('"') : ""
            };
        }
    }
}
