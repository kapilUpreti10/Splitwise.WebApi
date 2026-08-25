using Splitwise.Contracts.DTOs.Balances;
using Splitwise.DataAccess;
using Splitwise.Models;
using Splitwise.Services;

namespace Splitwise.Test.Services
{
    public class BalanceServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly BalanceService _balanceService;

        public BalanceServiceTests()
        {
            _dbContext = TestHelpers.CreateInMemoryDbContext(Guid.NewGuid().ToString());
            _balanceService = new BalanceService(_dbContext);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetGroupBalanceAsync_WhenGroupNotFound_ReturnsNull()
        {
            // Act
            var result = await _balanceService.GetGroupBalanceAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetGroupBalanceAsync_WhenNoExpenses_ReturnsZeroBalances()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser();
            var group = TestHelpers.CreateTestGroup();
            var member = TestHelpers.CreateTestGroupMember();

            _dbContext.Users.Add(user);
            _dbContext.Groups.Add(group);
            _dbContext.GroupMembers.Add(member);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _balanceService.GetGroupBalanceAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.GroupId);
            Assert.Single(result.Balances);
            Assert.Equal(0m, result.Balances[0].NetBalance);
        }

        [Fact]
        public async Task GetGroupBalanceAsync_WithExpenses_ReturnsCorrectBalances()
        {
            // Arrange
            var user1 = TestHelpers.CreateTestUser("user-1", "user1@test.com", "User 1");
            var user2 = TestHelpers.CreateTestUser("user-2", "user2@test.com", "User 2");
            var group = TestHelpers.CreateTestGroup();
            var member1 = new GroupMember { GroupId = 1, UserId = "user-1" };
            var member2 = new GroupMember { GroupId = 1, UserId = "user-2" };
            var expense = new Expense
            {
                Id = 1,
                GroupId = 1,
                PaidBy = "user-1",
                TotalAmount = 100m,
                CreatedAt = DateTime.UtcNow
            };
            var split1 = new ExpenseSplit
            {
                Id = Guid.NewGuid(),
                ExpenseId = 1,
                UserId = "user-1",
                IndivudialAmount = 50m
            };
            var split2 = new ExpenseSplit
            {
                Id = Guid.NewGuid(),
                ExpenseId = 1,
                UserId = "user-2",
                IndivudialAmount = 50m
            };

            _dbContext.Users.AddRange(user1, user2);
            _dbContext.Groups.Add(group);
            _dbContext.GroupMembers.AddRange(member1, member2);
            _dbContext.Expenses.Add(expense);
            _dbContext.ExpenseSplits.AddRange(split1, split2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _balanceService.GetGroupBalanceAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Balances.Count);

            // User1 paid 100, owes 50, so net = +50
            var user1Balance = result.Balances.First(b => b.UserId == "user-1");
            Assert.Equal(50m, user1Balance.NetBalance);

            // User2 paid 0, owes 50, so net = -50
            var user2Balance = result.Balances.First(b => b.UserId == "user-2");
            Assert.Equal(-50m, user2Balance.NetBalance);
        }

        [Fact]
        public async Task GetGroupBalanceAsync_WithSimplifiedDebts_ReturnsSettlementSuggestions()
        {
            // Arrange
            var user1 = TestHelpers.CreateTestUser("user-1", "user1@test.com", "User 1");
            var user2 = TestHelpers.CreateTestUser("user-2", "user2@test.com", "User 2");
            var group = TestHelpers.CreateTestGroup();
            var member1 = new GroupMember { GroupId = 1, UserId = "user-1" };
            var member2 = new GroupMember { GroupId = 1, UserId = "user-2" };
            var expense = new Expense
            {
                Id = 1,
                GroupId = 1,
                PaidBy = "user-1",
                TotalAmount = 100m,
                CreatedAt = DateTime.UtcNow
            };
            var split2 = new ExpenseSplit
            {
                Id = Guid.NewGuid(),
                ExpenseId = 1,
                UserId = "user-2",
                IndivudialAmount = 100m
            };

            _dbContext.Users.AddRange(user1, user2);
            _dbContext.Groups.Add(group);
            _dbContext.GroupMembers.AddRange(member1, member2);
            _dbContext.Expenses.Add(expense);
            _dbContext.ExpenseSplits.Add(split2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _balanceService.GetGroupBalanceAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.SimplifiedDebts);

            var suggestion = result.SimplifiedDebts.First();
            Assert.Equal("user-2", suggestion.FromUserId);
            Assert.Equal("user-1", suggestion.ToUserId);
            Assert.Equal(100m, suggestion.Amount);
        }

        [Fact]
        public async Task GetGroupBalanceAsync_WithMultipleCreditorsAndDebtors_SimplifiesCorrectly()
        {
            // Arrange
            var user1 = TestHelpers.CreateTestUser("user-1", "user1@test.com", "User 1");
            var user2 = TestHelpers.CreateTestUser("user-2", "user2@test.com", "User 2");
            var user3 = TestHelpers.CreateTestUser("user-3", "user3@test.com", "User 3");
            var group = TestHelpers.CreateTestGroup();

            _dbContext.Users.AddRange(user1, user2, user3);
            _dbContext.Groups.Add(group);
            _dbContext.GroupMembers.AddRange(
                new GroupMember { GroupId = 1, UserId = "user-1" },
                new GroupMember { GroupId = 1, UserId = "user-2" },
                new GroupMember { GroupId = 1, UserId = "user-3" }
            );

            // User1 paid 300, split equally among 3 users
            var expense = new Expense
            {
                Id = 1,
                GroupId = 1,
                PaidBy = "user-1",
                TotalAmount = 300m,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Expenses.Add(expense);
            _dbContext.ExpenseSplits.AddRange(
                new ExpenseSplit { Id = Guid.NewGuid(), ExpenseId = 1, UserId = "user-1", IndivudialAmount = 100m },
                new ExpenseSplit { Id = Guid.NewGuid(), ExpenseId = 1, UserId = "user-2", IndivudialAmount = 100m },
                new ExpenseSplit { Id = Guid.NewGuid(), ExpenseId = 1, UserId = "user-3", IndivudialAmount = 100m }
            );

            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _balanceService.GetGroupBalanceAsync(1);

            // Assert
            Assert.NotNull(result);

            // User1: paid 300, owes 100, net = +200
            // User2: paid 0, owes 100, net = -100
            // User3: paid 0, owes 100, net = -100

            var user1Balance = result.Balances.First(b => b.UserId == "user-1");
            var user2Balance = result.Balances.First(b => b.UserId == "user-2");
            var user3Balance = result.Balances.First(b => b.UserId == "user-3");

            Assert.Equal(200m, user1Balance.NetBalance);
            Assert.Equal(-100m, user2Balance.NetBalance);
            Assert.Equal(-100m, user3Balance.NetBalance);

            // Simplified debts: user2 pays user1 $100, user3 pays user1 $100
            Assert.Equal(2, result.SimplifiedDebts.Count);
        }
    }
}
