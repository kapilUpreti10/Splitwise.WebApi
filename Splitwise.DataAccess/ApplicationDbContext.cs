using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Splitwise.DataAccess.Seed;
using Splitwise.DataAccess.SeedData;
using Splitwise.Models;


namespace Splitwise.DataAccess
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public DbSet<Group> Groups { get; set; }

        public DbSet<GroupMember> GroupMembers { get; set; }

        public DbSet<Expense>Expenses { get; set; }

        public DbSet<ExpenseSplit> ExpenseSplits { get; set; }


        //adding basic seed data to the database 

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            UserSeed.Seed(builder);
            GroupSeed.Seed(builder);
            ExpenseSeed.Seed(builder);
            
        }
    }
}
