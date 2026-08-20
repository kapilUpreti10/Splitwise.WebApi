namespace Splitwise.Contracts.DTOs.Expenses
{
    public class ExpenseDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string PaidBy { get; set; } = string.Empty;
        public string? PaidByName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ExpenseSplitDto> Splits { get; set; } = new();
    }
}
