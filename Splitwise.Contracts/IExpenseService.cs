using Splitwise.Contracts.DTOs.Expenses;

namespace Splitwise.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<List<ExpenseDto>> GetExpensesForGroupAsync(int groupId);
        Task<ExpenseDto?> GetExpenseByIdAsync(int id);
        Task<(bool Succeeded, ExpenseDto? Expense, string? Error)> CreateExpenseAsync(CreateExpenseDto dto);
        Task<(bool Succeeded, string? Error)> UpdateExpenseAsync(int id, UpdateExpenseDto dto);
        Task<bool> DeleteExpenseAsync(int id);
    }
}
