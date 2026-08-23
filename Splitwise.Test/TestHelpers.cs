using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Splitwise.DataAccess;
using Splitwise.Models;

namespace Splitwise.Test
{
    /// <summary>
    /// Helper methods for creating mock objects for UserManager, RoleManager, and DbContext
    /// </summary>
    public static class TestHelpers
    {
        /// <summary>
        /// Creates a mock UserManager with common setup methods
        /// </summary>
        public static Mock<UserManager<ApplicationUser>> CreateMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
            return mock;
        }

        /// <summary>
        /// Creates a mock RoleManager with common setup methods
        /// </summary>
        public static Mock<RoleManager<IdentityRole>> CreateMockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            var mock = new Mock<RoleManager<IdentityRole>>(
                store.Object, null, null, null, null);
            return mock;
        }

        /// <summary>
        /// Creates an InMemory database context for testing
        /// </summary>
        public static ApplicationDbContext CreateInMemoryDbContext(string dbName = "TestDb")
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// Creates a test ApplicationUser with specified properties
        /// </summary>
        public static ApplicationUser CreateTestUser(
            string id = "test-user-id",
            string email = "test@example.com",
            string name = "Test User")
        {
            return new ApplicationUser
            {
                Id = id,
                Email = email,
                UserName = email,
                Name = name,
                Address = "Test Address",
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
        }

        /// <summary>
        /// Creates a test Group with specified properties
        /// </summary>
        public static Group CreateTestGroup(
            int id = 1,
            string name = "Test Group",
            string createdBy = "test-user-id")
        {
            return new Group
            {
                Id = id,
                Name = name,
                Description = "Test Description",
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Creates a test Expense with specified properties
        /// </summary>
        public static Expense CreateTestExpense(
            int id = 1,
            int groupId = 1,
            string paidBy = "test-user-id",
            decimal totalAmount = 100.00m)
        {
            return new Expense
            {
                Id = id,
                GroupId = groupId,
                PaidBy = paidBy,
                TotalAmount = totalAmount,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Creates a test ExpenseSplit with specified properties
        /// </summary>
        public static ExpenseSplit CreateTestExpenseSplit(
            Guid? id = null,
            int expenseId = 1,
            string userId = "test-user-id",
            decimal amount = 50.00m)
        {
            return new ExpenseSplit
            {
                Id = id ?? Guid.NewGuid(),
                ExpenseId = expenseId,
                UserId = userId,
                IndivudialAmount = amount
            };
        }

        /// <summary>
        /// Creates a test GroupMember with specified properties
        /// </summary>
        public static GroupMember CreateTestGroupMember(
            int id = 1,
            int groupId = 1,
            string userId = "test-user-id")
        {
            return new GroupMember
            {
                Id = id,
                GroupId = groupId,
                UserId = userId
            };
        }
    }
}
