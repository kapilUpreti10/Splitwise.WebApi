using Microsoft.EntityFrameworkCore;
using Splitwise.Contracts.DTOs.Expenses;
using Splitwise.DataAccess;
using Splitwise.Models;
using Splitwise.Services.Interfaces;
using Splitwise.Utils.Enums;

namespace Splitwise.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ApplicationDbContext _db;

        public ExpenseService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<ExpenseDto>> GetExpensesForGroupAsync(int groupId)
        {
            // Materialize first, THEN map — MapToDto isn't SQL-translatable,
            // so calling it inside a LINQ-to-Entities .Select() would throw at runtime.
            var expenses = await _db.Expenses
                .Where(e => e.GroupId == groupId)
                .Include(e => e.PaidByUser)
                .Include(e => e.ExpenseSplits)
                    .ThenInclude(s => s.User)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return expenses.Select(MapToDto).ToList();
        }

        public async Task<ExpenseDto?> GetExpenseByIdAsync(int id)
        {
            var expense = await _db.Expenses
                .Include(e => e.PaidByUser)
                .Include(e => e.ExpenseSplits)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(e => e.Id == id);

            return expense == null ? null : MapToDto(expense);
        }

        public async Task<(bool Succeeded, ExpenseDto? Expense, string? Error)> CreateExpenseAsync(CreateExpenseDto dto)
        {
            var groupExists = await _db.Groups.AnyAsync(g => g.Id == dto.GroupId);
            if (!groupExists) return (false, null, "Group not found.");

            var splitResult = ResolveSplits(dto.TotalAmount, dto.SplitType, dto.Splits);
            if (!splitResult.Success) return (false, null, splitResult.Error);

            var expense = new Expense
            {
                GroupId = dto.GroupId,
                PaidBy = dto.PaidBy,
                TotalAmount = dto.TotalAmount,
                CreatedAt = DateTime.UtcNow
            };

            _db.Expenses.Add(expense);
            await _db.SaveChangesAsync();

            foreach (var (userId, amount) in splitResult.Resolved!)
            {
                _db.ExpenseSplits.Add(new ExpenseSplit
                {
                    Id = Guid.NewGuid(),
                    ExpenseId = expense.Id,
                    UserId = userId,
                    IndivudialAmount = amount
                });
            }
            await _db.SaveChangesAsync();

            return (true, await GetExpenseByIdAsync(expense.Id), null);
        }

        public async Task<(bool Succeeded, string? Error)> UpdateExpenseAsync(int id, UpdateExpenseDto dto)
        {
            var expense = await _db.Expenses
                .Include(e => e.ExpenseSplits)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (expense == null) return (false, "Expense not found.");

            var splitResult = ResolveSplits(dto.TotalAmount, dto.SplitType, dto.Splits);
            if (!splitResult.Success) return (false, splitResult.Error);

            expense.PaidBy = dto.PaidBy;
            expense.TotalAmount = dto.TotalAmount;

            // Simplest correct approach: wipe old splits, write the newly resolved ones.
            _db.ExpenseSplits.RemoveRange(expense.ExpenseSplits);

            foreach (var (userId, amount) in splitResult.Resolved!)
            {
                _db.ExpenseSplits.Add(new ExpenseSplit
                {
                    Id = Guid.NewGuid(),
                    ExpenseId = expense.Id,
                    UserId = userId,
                    IndivudialAmount = amount
                });
            }

            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<bool> DeleteExpenseAsync(int id)
        {
            var expense = await _db.Expenses.FindAsync(id);
            if (expense == null) return false;

            _db.Expenses.Remove(expense);
            await _db.SaveChangesAsync();
            return true;
        }

        // --- split resolution: the one place split math happens ---
        //
        // Equal:      total / participantCount, first participant absorbs any rounding remainder.
        // Exact:      caller sends amounts, must sum to TotalAmount (+/- 1 cent tolerance).
        // Percentage: caller sends percentages, must sum to 100 (+/- 0.01 tolerance);
        //             last participant absorbs the rounding remainder so the split still
        //             sums exactly to TotalAmount.
        private static (bool Success, string? Error, List<(string UserId, decimal Amount)>? Resolved) ResolveSplits(
            decimal totalAmount, SplitType splitType, List<ExpenseSplitInputDto> inputs)
        {
            if (inputs == null || inputs.Count == 0)
                return (false, "At least one split participant is required.", null);

            var userIds = inputs.Select(i => i.UserId).ToList();
            if (userIds.Distinct().Count() != userIds.Count)
                return (false, "Duplicate users in split list.", null);

            var resolved = new List<(string UserId, decimal Amount)>();

            switch (splitType)
            {
                case SplitType.Equal:
                {
                    int count = inputs.Count;
                    decimal baseShare = Math.Floor((totalAmount / count) * 100) / 100;
                    decimal distributed = baseShare * count;
                    decimal remainder = totalAmount - distributed;

                    for (int i = 0; i < inputs.Count; i++)
                    {
                        var amount = baseShare;
                        if (i == 0) amount += remainder; // deterministic: first participant eats the odd cents
                        resolved.Add((inputs[i].UserId, amount));
                    }
                    break;
                }

                case SplitType.Exact:
                {
                    if (inputs.Any(i => i.Amount == null))
                        return (false, "Amount is required for every split when using Exact split type.", null);

                    decimal sum = inputs.Sum(i => i.Amount!.Value);
                    if (Math.Abs(sum - totalAmount) > 0.01m)
                        return (false, $"Split amounts ({sum}) must add up to the total amount ({totalAmount}).", null);

                    resolved.AddRange(inputs.Select(i => (i.UserId, i.Amount!.Value)));
                    break;
                }

                case SplitType.Percentage:
                {
                    if (inputs.Any(i => i.Percentage == null))
                        return (false, "Percentage is required for every split when using Percentage split type.", null);

                    decimal sumPct = inputs.Sum(i => i.Percentage!.Value);
                    if (Math.Abs(sumPct - 100m) > 0.01m)
                        return (false, $"Percentages must add up to 100 (currently {sumPct}).", null);

                    decimal runningTotal = 0;
                    for (int i = 0; i < inputs.Count; i++)
                    {
                        decimal amount;
                        if (i == inputs.Count - 1)
                        {
                            amount = totalAmount - runningTotal; // last one absorbs rounding remainder
                        }
                        else
                        {
                            amount = Math.Round(totalAmount * (inputs[i].Percentage!.Value / 100m), 2);
                            runningTotal += amount;
                        }
                        resolved.Add((inputs[i].UserId, amount));
                    }
                    break;
                }

                default:
                    return (false, "Unsupported split type.", null);
            }

            return (true, null, resolved);
        }

        private static ExpenseDto MapToDto(Expense e)
        {
            return new ExpenseDto
            {
                Id = e.Id,
                GroupId = e.GroupId,
                PaidBy = e.PaidBy,
                PaidByName = e.PaidByUser?.Name,
                TotalAmount = e.TotalAmount,
                CreatedAt = e.CreatedAt,
                Splits = e.ExpenseSplits.Select(s => new ExpenseSplitDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    UserName = s.User?.Name,
                    Amount = s.IndivudialAmount
                }).ToList()
            };
        }
    }
}
