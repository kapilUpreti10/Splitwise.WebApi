using Splitwise.Contracts.DTOs.Balances;

namespace Splitwise.Services.Interfaces
{
    public interface IBalanceService
    {
        Task<GroupBalanceDto?> GetGroupBalanceAsync(int groupId);
    }
}
