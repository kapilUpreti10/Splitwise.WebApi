using Microsoft.EntityFrameworkCore;
using Splitwise.Contracts.DTOs.Balances;
using Splitwise.DataAccess;
using Splitwise.Services.Interfaces;

namespace Splitwise.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly ApplicationDbContext _db;

        public BalanceService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GroupBalanceDto?> GetGroupBalanceAsync(int groupId)
        {
            var groupExists = await _db.Groups.AnyAsync(g => g.Id == groupId);
            if (!groupExists) return null;

            var members = await _db.GroupMembers
                .Where(gm => gm.GroupId == groupId)
                .Include(gm => gm.User)
                .ToListAsync();

            var expenses = await _db.Expenses
                .Where(e => e.GroupId == groupId)
                .Include(e => e.ExpenseSplits)
                .ToListAsync();

            // Balance is DERIVED, never stored: net[user] = total they paid out
            // (as the fronting payer) minus total they owe (via their splits),
            // summed across every expense in the group. No dedicated "Balance"
            // table exists — the ledger (Expenses + ExpenseSplits) is the only
            // source of truth, so there's nothing that can drift out of sync.
            var net = members.ToDictionary(m => m.UserId, m => 0m);

            foreach (var expense in expenses)
            {
                if (net.ContainsKey(expense.PaidBy))
                    net[expense.PaidBy] += expense.TotalAmount;

                foreach (var split in expense.ExpenseSplits)
                {
                    if (net.ContainsKey(split.UserId))
                        net[split.UserId] -= split.IndivudialAmount;
                }
            }

            var balances = members.Select(m => new UserBalanceDto
            {
                UserId = m.UserId,
                UserName = m.User?.Name,
                NetBalance = Math.Round(net[m.UserId], 2)
            }).ToList();

            var simplifiedDebts = SimplifyDebts(balances);

            return new GroupBalanceDto
            {
                GroupId = groupId,
                Balances = balances,
                SimplifiedDebts = simplifiedDebts
            };
        }

        // Greedy debt-simplification: repeatedly match the largest creditor
        // against the largest debtor until everyone is settled. This collapses
        // a web of pairwise debts into the minimum number of payments needed —
        // e.g. instead of "A owes B $10, B owes C $10", it suggests "A pays C $10" directly.
        private static List<SettlementSuggestionDto> SimplifyDebts(List<UserBalanceDto> balances)
        {
            var result = new List<SettlementSuggestionDto>();

            var creditors = balances.Where(b => b.NetBalance > 0.01m)
                .OrderByDescending(b => b.NetBalance)
                .Select(b => new { b.UserId, b.UserName, Amount = b.NetBalance })
                .ToList();

            var debtors = balances.Where(b => b.NetBalance < -0.01m)
                .OrderByDescending(b => -b.NetBalance)
                .Select(b => new { b.UserId, b.UserName, Amount = -b.NetBalance })
                .ToList();

            var creditorAmounts = creditors.Select(c => c.Amount).ToArray();
            var debtorAmounts = debtors.Select(d => d.Amount).ToArray();

            int ci = 0, di = 0;
            while (ci < creditors.Count && di < debtors.Count)
            {
                var settled = Math.Min(creditorAmounts[ci], debtorAmounts[di]);
                if (settled > 0.01m)
                {
                    result.Add(new SettlementSuggestionDto
                    {
                        FromUserId = debtors[di].UserId,
                        FromUserName = debtors[di].UserName,
                        ToUserId = creditors[ci].UserId,
                        ToUserName = creditors[ci].UserName,
                        Amount = Math.Round(settled, 2)
                    });
                }

                creditorAmounts[ci] -= settled;
                debtorAmounts[di] -= settled;

                if (creditorAmounts[ci] <= 0.01m) ci++;
                if (debtorAmounts[di] <= 0.01m) di++;
            }

            return result;
        }
    }
}
