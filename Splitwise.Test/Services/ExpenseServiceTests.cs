using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Splitwise.Contracts.DTOs.Expenses;
using Splitwise.DataAccess;
using Splitwise.Models;
using Splitwise.Services;
using Splitwise.Utils.Enums;

namespace Splitwise.Test.Services
{
    public class ExpenseServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ExpenseService _expenseService;

        public ExpenseServiceTests()
        {
            _dbContext = TestHelpers.CreateInMemoryDbContext(Guid.NewGuid().ToString());
            _expenseService = new ExpenseService(_dbContext);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetExpensesForGroupAsync_ReturnsExpenses()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser();
            var group = TestHelpers.CreateTestGroup();
            var expense1 = TestHelpers.CreateTestExpense(1, 1, "user-id", 100m);
            var expense2 = TestHelpers.CreateTestExpense(2, 1, "user-id", 200m);

            _dbContext.Users.Add(user);
            _dbContext.Groups.Add(group);
            _dbContext.Expenses.AddRange(expense1, expense2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _expenseService.GetExpensesForGroupAsync(1);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetExpenseByIdAsync_WhenValid_ReturnsExpenseDto()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser();
            var group = TestHelpers.CreateTestGroup();
            var expense = TestHelpers.CreateTestExpense();

            _dbContext.Users.Add(user);
            _dbContext.Groups.Add(group);
            _dbContext.Expenses.Add(expense);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _expenseService.GetExpenseByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100m, result.TotalAmount);
        }

        [Fact]
        public async Task GetExpenseByIdAsync_WhenInvalid_ReturnsNull()
        {
            // Act
            var result = await _expenseService.GetExpenseByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateExpenseAsync_WhenValidEqualSplit_ReturnsSuccess()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser();
            var group = TestHelpers.CreateTestGroup();

            _dbContext.Users.Add(user);
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            var dto = new CreateExpenseDto
            {
                GroupId = 1,
                PaidBy = "user-id",
                TotalAmount = 100m,
                SplitType = SplitType.Equal,
                Splits = new List<ExpenseSplitInputDto>
                {
                    new ExpenseSplitInputDto { UserId = "user-id" },
                    new ExpenseSplitInputDto { UserId = "user-id-2" }
                }
            };

            // Act
            var (succeeded, expense, error) = await _expenseService.CreateExpenseAsync(dto);

            // Assert
            Assert.True(succeeded);
            Assert.NotNull(expense);
            Assert.Null(error);
            Assert.Equal(100m, expense.TotalAmount);
        }

        [Fact]
        public async Task CreateExpenseAsync_WhenValidExactSplit_ReturnsSuccess()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser();
            var group = TestHelpers.CreateTestGroup();

            _dbContext.Users.Add(user);
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            var dto = new CreateExpenseDto
            {
                GroupId = 1,
                PaidBy = "user-id",
                TotalAmount = 100m,
                SplitType = SplitType.Exact,
                Splits = new List<ExpenseSplitInputDto>
                {
                    new ExpenseSplitInputDto { UserId = "user-id", Amount = 60m },
                    new ExpenseSplitInputDto { UserId = "user-id-2", Amount = 40m }
                }
            };

            // Act
            var (succeeded, expense, error) = await _expenseService.CreateExpenseAsync(dto);

            // Assert
            Assert.True(succeeded);
            Assert.NotNull(expense);
            Assert.Null(error);
        }

        [Fact]
        public async Task CreateExpenseAsync_WhenValidPercentageSplit_ReturnsSuccess()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser();
            var group = TestHelpers.CreateTestGroup();

            _dbContext.Users.Add(user);
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            var dto = new CreateExpenseDto
            {
                GroupId = 1,
                PaidBy = "user-id",
                TotalAmount = 100m,
                SplitType = SplitType.Percentage,
                Splits = new List<ExpenseSplitInputDto>
                {
                    new ExpenseSplitInputDto { UserId = "user-id", Percentage = 60m },
                    new ExpenseSplitInputDto { UserId = "user-id-2", Percentage = 40m }
                }
            };

            // Act
            var (succeeded, expense, error) = await _expenseService.CreateExpenseAsync(dto);

            // Assert
            Assert.True(succeeded);
            Assert.NotNull(expense);
            Assert.Null(error);
        }

        [Fact]
        public async Task CreateExpenseAsync_WhenGroupNotFound_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateExpenseDto
            {
                GroupId = 999,
                PaidBy = "user-id",
                TotalAmount = 100m,
                SplitType = SplitType.Equal,
                Splits = new List<ExpenseSplitInputDto> { new ExpenseSplitInputDto { UserId = "user-id" } }
            };

            // Act
            var (succeeded, expense, error) = await _expenseService.CreateExpenseAsync(dto);

            // Assert
            Assert.False(succeeded);
            Assert.Null(expense);
            Assert.Equal("Group not found.", error);
        }

        [Fact]
        public async Task CreateExpenseAsync_WhenEmptySplits_ReturnsFailure()
        {
            // Arrange
            var group = TestHelpers.CreateTestGroup();
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            var dto = new CreateExpenseDto
            {
                GroupId = 1,
                PaidBy = "user-id",
                TotalAmount = 100m,
                SplitType = SplitType.Equal,
                Splits = new List<ExpenseSplitInputDto>()
            };

            // Act
            var (succeeded, expense, error) = await _expenseService.CreateExpenseAsync(dto);

            // Assert
            Assert.False(succeeded);
            Assert.Null(expense);
            Assert.Equal("At least one split participant is required.", error);
        }

        [Fact]
        public async Task CreateExpenseAsync_WhenDuplicateUsersInSplit_ReturnsFailure()
        {
            // Arrange
            var group = TestHelpers.CreateTestGroup();
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            var dto = new CreateExpenseDto
            {
                GroupId = 1,
                PaidBy = "user-id",
                TotalAmount = 100m,
                SplitType = SplitType.Equal,
                Splits = new List<ExpenseSplitInputDto>
                {
                    new ExpenseSplitInputDto { UserId = "user-id" },
                    new ExpenseSplitInputDto { UserId = "user-id" }
                }
            };

            // Act
            var (succeeded, expense, error) = await _expenseService.CreateExpenseAsync(dto);

            // Assert
            Assert.False(succeeded);
            Assert.Null(expense);
            Assert.Equal("Duplicate users in split list.", error);
        }

        [Fact]
        public async Task CreateExpenseAsync_WhenExactAmountsDontMatchTotal_ReturnsFailure()
        {
            // Arrange
            var group = TestHelpers.CreateTestGroup();
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            var dto = new CreateExpenseDto
            {
                GroupId = 1,
                PaidBy = "user-id",
                TotalAmount = 100m,
                SplitType = SplitType.Exact,
                Splits = new List<ExpenseSplitInputDto>
                {
                    new ExpenseSplitInputDto { UserId = "user-id", Amount = 60m },
                    new ExpenseSplitInputDto { UserId = "user-id-2", Amount = 30m }
                }
            };

            // Act
            var (succeeded, expense, error) = await _expenseService.CreateExpenseAsync(dto);

            // Assert
            Assert.False(succeeded);
            Assert.Null(expense);
            Assert.Contains("must add up to the total amount", error);
        }

        [Fact]
        public async Task CreateExpenseAsync_WhenPercentagesDontSumTo100_ReturnsFailure()
        {
            // Arrange
            var group = TestHelpers.CreateTestGroup();
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();

            var dto = new CreateExpenseDto
            {
                GroupId = 1,
                PaidBy = "user-id",
                TotalAmount = 100m,
                SplitType = SplitType.Percentage,
                Splits = new List<ExpenseSplitInputDto>
                {
                    new ExpenseSplitInputDto { UserId = "user-id", Percentage = 60m },
                    new ExpenseSplitInputDto { UserId = "user-id-2", Percentage = 30m }
                }
            };

            // Act
            var (succeeded, expense, error) = await _expenseService.CreateExpenseAsync(dto);

            // Assert
            Assert.False(succeeded);
            Assert.Null(expense);
            Assert.Contains("must add up to 100", error);
        }

        [Fact]
        public async Task UpdateExpenseAsync_WhenValid_ReturnsSuccess()
        {
            // Arrange
            var user = TestHelpers.CreateTestUser();
            var group = TestHelpers.CreateTestGroup();
            var expense = TestHelpers.CreateTestExpense();
            var split = TestHelpers.CreateTestExpenseSplit();

            _dbContext.Users.Add(user);
            _dbContext.Groups.Add(group);
            _dbContext.Expenses.Add(expense);
            _dbContext.ExpenseSplits.Add(split);
            await _dbContext.SaveChangesAsync();

            var dto = new UpdateExpenseDto
            {
                PaidBy = "user-id",
                TotalAmount = 150m,
                SplitType = SplitType.Equal,
                Splits = new List<ExpenseSplitInputDto> { new ExpenseSplitInputDto { UserId = "user-id" } }
            };

            // Act
            var (succeeded, error) = await _expenseService.UpdateExpenseAsync(1, dto);

            // Assert
            Assert.True(succeeded);
            Assert.Null(error);
            var updated = await _dbContext.Expenses.FindAsync(1);
            Assert.Equal(150m, updated!.TotalAmount);
        }

        [Fact]
        public async Task UpdateExpenseAsync_WhenExpenseNotFound_ReturnsFailure()
        {
            // Arrange
            var dto = new UpdateExpenseDto
            {
                PaidBy = "user-id",
                TotalAmount = 150m,
                SplitType = SplitType.Equal,
                Splits = new List<ExpenseSplitInputDto> { new ExpenseSplitInputDto { UserId = "user-id" } }
            };

            // Act
            var (succeeded, error) = await _expenseService.UpdateExpenseAsync(999, dto);

            // Assert
            Assert.False(succeeded);
            Assert.Equal("Expense not found.", error);
        }

        [Fact]
        public async Task DeleteExpenseAsync_WhenValid_ReturnsTrue()
        {
            // Arrange
            var expense = TestHelpers.CreateTestExpense();
            _dbContext.Expenses.Add(expense);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _expenseService.DeleteExpenseAsync(1);

            // Assert
            Assert.True(result);
            Assert.Equal(0, await _dbContext.Expenses.CountAsync());
        }

        [Fact]
        public async Task DeleteExpenseAsync_WhenInvalid_ReturnsFalse()
        {
            // Act
            var result = await _expenseService.DeleteExpenseAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}
