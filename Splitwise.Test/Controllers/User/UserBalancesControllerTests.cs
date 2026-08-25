using Microsoft.AspNetCore.Mvc;
using Moq;
using Splitwise.Contracts.DTOs.Balances;
using Splitwise.Services.Interfaces;
using Splitwise.WebApi.Controllers.User;

namespace Splitwise.Test.Controllers.User
{
    public class UserBalancesControllerTests
    {
        private readonly Mock<IBalanceService> _balanceServiceMock;
        private readonly BalancesController _controller;

        public UserBalancesControllerTests()
        {
            _balanceServiceMock = new Mock<IBalanceService>();
            _controller = new BalancesController(_balanceServiceMock.Object);
        }

        [Fact]
        public async Task GetGroupBalance_WhenGroupExists_ReturnsOkWithBalance()
        {
            // Arrange
            var balance = new GroupBalanceDto
            {
                GroupId = 1,
                Balances = new List<UserBalanceDto>
                {
                    new UserBalanceDto { UserId = "user-1", NetBalance = 100.00m },
                    new UserBalanceDto { UserId = "user-2", NetBalance = -100.00m }
                },
                SimplifiedDebts = new List<SettlementSuggestionDto>()
            };
            _balanceServiceMock.Setup(s => s.GetGroupBalanceAsync(1)).ReturnsAsync(balance);

            // Act
            var result = await _controller.GetGroupBalance(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(balance, okResult.Value);
        }

        [Fact]
        public async Task GetGroupBalance_WhenGroupNotFound_ReturnsNotFound()
        {
            // Arrange
            _balanceServiceMock.Setup(s => s.GetGroupBalanceAsync(999)).ReturnsAsync((GroupBalanceDto?)null);

            // Act
            var result = await _controller.GetGroupBalance(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
