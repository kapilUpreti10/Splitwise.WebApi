using Microsoft.AspNetCore.Mvc;
using Moq;
using Splitwise.Contracts.DTOs.Expenses;
using Splitwise.Services.Interfaces;
using Splitwise.Utils.Enums;
using Splitwise.WebApi.Controllers.User;

namespace Splitwise.Test.Controllers.User
{
    public class UserExpensesControllerTests
    {
        private readonly Mock<IExpenseService> _expenseServiceMock;
        private readonly ExpensesController _controller;

        public UserExpensesControllerTests()
        {
            _expenseServiceMock = new Mock<IExpenseService>();
            _controller = new ExpensesController(_expenseServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithExpenses()
        {
            // Arrange
            var expenses = new List<ExpenseDto>
            {
                new ExpenseDto { Id = 1, GroupId = 1, TotalAmount = 100m },
                new ExpenseDto { Id = 2, GroupId = 1, TotalAmount = 200m }
            };
            _expenseServiceMock.Setup(s => s.GetExpensesForGroupAsync(1)).ReturnsAsync(expenses);

            // Act
            var result = await _controller.GetAll(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expenses, okResult.Value);
        }

        [Fact]
        public async Task GetById_WhenExpenseExists_ReturnsOkWithExpense()
        {
            // Arrange
            var expense = new ExpenseDto { Id = 1, GroupId = 1, TotalAmount = 100m };
            _expenseServiceMock.Setup(s => s.GetExpenseByIdAsync(1)).ReturnsAsync(expense);

            // Act
            var result = await _controller.GetById(1, 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expense, okResult.Value);
        }

        [Fact]
        public async Task GetById_WhenExpenseNotFound_ReturnsNotFound()
        {
            // Arrange
            _expenseServiceMock.Setup(s => s.GetExpenseByIdAsync(999)).ReturnsAsync((ExpenseDto?)null);

            // Act
            var result = await _controller.GetById(1, 999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_WhenValid_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateExpenseDto
            {
                PaidBy = "user-1",
                TotalAmount = 100m,
                SplitType = SplitType.Equal,
                Splits = new List<ExpenseSplitInputDto>
                {
                    new ExpenseSplitInputDto { UserId = "user-1" },
                    new ExpenseSplitInputDto { UserId = "user-2" }
                }
            };
            var createdExpense = new ExpenseDto
            {
                Id = 1,
                GroupId = 1,
                PaidBy = dto.PaidBy,
                TotalAmount = dto.TotalAmount
            };

            _expenseServiceMock
                .Setup(s => s.CreateExpenseAsync(It.IsAny<CreateExpenseDto>()))
                .ReturnsAsync((true, createdExpense, (string?)null));

            // Act
            var result = await _controller.Create(1, dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            Assert.Equal(createdExpense, createdResult.Value);
            Assert.Equal(1, dto.GroupId); // GroupId should be set from route
        }

        [Fact]
        public async Task Create_WhenGroupNotFound_ReturnsBadRequest()
        {
            // Arrange
            var dto = new CreateExpenseDto
            {
                PaidBy = "user-1",
                TotalAmount = 100m,
                SplitType = SplitType.Equal,
                Splits = new List<ExpenseSplitInputDto> { new ExpenseSplitInputDto { UserId = "user-1" } }
            };

            _expenseServiceMock
                .Setup(s => s.CreateExpenseAsync(It.IsAny<CreateExpenseDto>()))
                .ReturnsAsync((false, (ExpenseDto?)null, "Group not found."));

            // Act
            var result = await _controller.Create(999, dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Group not found.", badRequestResult.Value);
        }

        [Fact]
        public async Task Create_WhenInvalidSplits_ReturnsBadRequest()
        {
            // Arrange
            var dto = new CreateExpenseDto
            {
                PaidBy = "user-1",
                TotalAmount = 100m,
                SplitType = SplitType.Percentage,
                Splits = new List<ExpenseSplitInputDto>
                {
                    new ExpenseSplitInputDto { UserId = "user-1", Percentage = 60 },
                    new ExpenseSplitInputDto { UserId = "user-2", Percentage = 30 }
                }
            };

            _expenseServiceMock
                .Setup(s => s.CreateExpenseAsync(It.IsAny<CreateExpenseDto>()))
                .ReturnsAsync((false, (ExpenseDto?)null, "Percentages must add up to 100."));

            // Act
            var result = await _controller.Create(1, dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Percentages must add up to 100.", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_WhenValid_ReturnsNoContent()
        {
            // Arrange
            var dto = new UpdateExpenseDto
            {
                PaidBy = "user-1",
                TotalAmount = 150m,
                SplitType = SplitType.Equal,
                Splits = new List<ExpenseSplitInputDto>
                {
                    new ExpenseSplitInputDto { UserId = "user-1" },
                    new ExpenseSplitInputDto { UserId = "user-2" }
                }
            };

            _expenseServiceMock
                .Setup(s => s.UpdateExpenseAsync(1, dto))
                .ReturnsAsync((true, (string?)null));

            // Act
            var result = await _controller.Update(1, 1, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_WhenExpenseNotFound_ReturnsNotFound()
        {
            // Arrange
            var dto = new UpdateExpenseDto
            {
                PaidBy = "user-1",
                TotalAmount = 150m,
                SplitType = SplitType.Equal,
                Splits = new List<ExpenseSplitInputDto> { new ExpenseSplitInputDto { UserId = "user-1" } }
            };

            _expenseServiceMock
                .Setup(s => s.UpdateExpenseAsync(999, dto))
                .ReturnsAsync((false, "Expense not found."));

            // Act
            var result = await _controller.Update(1, 999, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_WhenInvalidSplits_ReturnsBadRequest()
        {
            // Arrange
            var dto = new UpdateExpenseDto
            {
                PaidBy = "user-1",
                TotalAmount = 150m,
                SplitType = SplitType.Exact,
                Splits = new List<ExpenseSplitInputDto>
                {
                    new ExpenseSplitInputDto { UserId = "user-1", Amount = 100 },
                    new ExpenseSplitInputDto { UserId = "user-2", Amount = 100 }
                }
            };

            _expenseServiceMock
                .Setup(s => s.UpdateExpenseAsync(1, dto))
                .ReturnsAsync((false, "Split amounts must add up to total."));

            // Act
            var result = await _controller.Update(1, 1, dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Split amounts must add up to total.", badRequestResult.Value);
        }

        [Fact]
        public async Task Delete_WhenExpenseExists_ReturnsNoContent()
        {
            // Arrange
            _expenseServiceMock.Setup(s => s.DeleteExpenseAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1, 1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WhenExpenseNotFound_ReturnsNotFound()
        {
            // Arrange
            _expenseServiceMock.Setup(s => s.DeleteExpenseAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(1, 999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
