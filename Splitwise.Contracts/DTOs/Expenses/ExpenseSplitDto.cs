namespace Splitwise.Contracts.DTOs.Expenses
{
    // Output shape: the RESOLVED amount, regardless of how it was originally entered.
    public class ExpenseSplitDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public decimal Amount { get; set; }
    }
}
