namespace Splitwise.Contracts.DTOs.Balances
{
    public class GroupBalanceDto
    {
        public int GroupId { get; set; }
        public List<UserBalanceDto> Balances { get; set; } = new();
        public List<SettlementSuggestionDto> SimplifiedDebts { get; set; } = new();
    }
}
