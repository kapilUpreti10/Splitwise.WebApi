using Microsoft.AspNetCore.Mvc;
using Moq;
using Splitwise.Contracts.DTOs.Groups;
using Splitwise.Services.Interfaces;
using Splitwise.WebApi.Controllers.Admin;

namespace Splitwise.Test.Controllers.Admin
{
    public class AdminGroupsControllerTests
    {
        private readonly Mock<IGroupService> _groupServiceMock;
        private readonly GroupsController _controller;

        public AdminGroupsControllerTests()
        {
            _groupServiceMock = new Mock<IGroupService>();
            _controller = new GroupsController(_groupServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithAllGroups()
        {
            // Arrange
            var groups = new List<GroupDto>
            {
                new GroupDto { Id = 1, Name = "Group 1", MemberCount = 3 },
                new GroupDto { Id = 2, Name = "Group 2", MemberCount = 5 }
            };
            _groupServiceMock.Setup(s => s.GetAllGroupsAsync()).ReturnsAsync(groups);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(groups, okResult.Value);
        }

        [Fact]
        public async Task Delete_WhenGroupExists_ReturnsNoContent()
        {
            // Arrange
            _groupServiceMock.Setup(s => s.DeleteGroupAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WhenGroupNotFound_ReturnsNotFound()
        {
            // Arrange
            _groupServiceMock.Setup(s => s.DeleteGroupAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
