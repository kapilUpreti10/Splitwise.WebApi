using Microsoft.EntityFrameworkCore;
using Splitwise.Models;

namespace Splitwise.DataAccess.Seed
{
    public static class ExpenseSplitSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExpenseSplit>().HasData(

                // Expense 1 = 1500
                new ExpenseSplit
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    ExpenseId = 1,
                    UserId = "user-005",
                    IndivudialAmount = 500
                },

                new ExpenseSplit
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    ExpenseId = 1,
                    UserId = "user-002",
                    IndivudialAmount = 200
                },

                new ExpenseSplit
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    ExpenseId = 1,
                    UserId = "user-003",
                    IndivudialAmount = 500
                },

                new ExpenseSplit
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                    ExpenseId = 1,
                    UserId = "user-004",
                    IndivudialAmount = 300
                },

                // Expense 2 = 2000
                new ExpenseSplit
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                    ExpenseId = 2,
                    UserId = "user-001",
                    IndivudialAmount = 400
                },

                new ExpenseSplit
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                    ExpenseId = 2,
                    UserId = "user-002",
                    IndivudialAmount = 400
                },

              

                new ExpenseSplit
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000004"),
                    ExpenseId = 2,
                    UserId = "user-004",
                    IndivudialAmount = 800
                },
                new ExpenseSplit
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000005"),
                    ExpenseId = 2,
                    UserId = "user-005",
                    IndivudialAmount = 400
                }
               
            );
        }
    }
}