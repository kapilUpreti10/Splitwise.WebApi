using Microsoft.EntityFrameworkCore;
using Splitwise.Models;

namespace Splitwise.DataAccess.Seed
{
    public static class ExpenseSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Expense>().HasData(
                new Expense
                {
                    Id = 1,
                    GroupId = 1,
                    PaidBy = "user-001",
                    TotalAmount = 1500,
                    CreatedAt = new DateTime(2026, 2, 1)
                },

                new Expense
                {
                    Id = 2,
                    GroupId = 1,
                    PaidBy = "user-002",
                    TotalAmount = 2000,
                    CreatedAt = new DateTime(2026, 2, 5)
                }

            );
        }
    }
}