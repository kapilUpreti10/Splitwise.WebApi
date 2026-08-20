namespace Splitwise.Contracts.DTOs.Balances
{
    // Positive NetBalance = this user is owed money overall in the group.
    // Negative NetBalance = this user owes money overall in the group.
    public class UserBalanceDto
    {
        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public decimal NetBalance { get; set; }
    }
}
