using Microsoft.EntityFrameworkCore;
using Splitwise.Models;

namespace Splitwise.DataAccess.Seed
{
    public static class GroupSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>().HasData(
                new Group
                {
                    Id = 1,
                    Name = "Khimchi",
                    Description = "entrepreneurship friday class may 30",
                    CreatedBy = "user-001",
                    CreatedAt = new DateTime(2026, 1, 10)
                }
            );
        }
    }
}