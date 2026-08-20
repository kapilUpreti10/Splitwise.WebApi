namespace Splitwise.Contracts.DTOs.Balances
{
    // One suggested payment from the debt-simplification algorithm:
    // "FromUser should pay ToUser this Amount to settle up."
    public class SettlementSuggestionDto
    {
        public string FromUserId { get; set; } = string.Empty;
        public string? FromUserName { get; set; }
        public string ToUserId { get; set; } = string.Empty;
        public string? ToUserName { get; set; }
        public decimal Amount { get; set; }
    }
}
