using Microsoft.AspNetCore.Mvc;
using Moq;
using Splitwise.Contracts.DTOs.Groups;
using Splitwise.Services.Interfaces;
using Splitwise.WebApi.Controllers.User;

namespace Splitwise.Test.Controllers.User
{
    public class UserGroupsControllerTests
    {
        private readonly Mock<IGroupService> _groupServiceMock;
        private readonly GroupsController _controller;

        public UserGroupsControllerTests()
        {
            _groupServiceMock = new Mock<IGroupService>();
            _controller = new GroupsController(_groupServiceMock.Object);
        }

        [Fact]
        public async Task GetMyGroups_ReturnsOkWithUserGroups()
        {
            // Arrange
            var userId = "user-123";
            var groups = new List<GroupDto>
            {
                new GroupDto { Id = 1, Name = "Group 1" },
                new GroupDto { Id = 2, Name = "Group 2" }
            };
            _groupServiceMock.Setup(s => s.GetGroupsForUserAsync(userId)).ReturnsAsync(groups);

            // Act
            var result = await _controller.GetMyGroups(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(groups, okResult.Value);
        }

        [Fact]
        public async Task GetById_WhenGroupExists_ReturnsOkWithGroup()
        {
            // Arrange
            var group = new GroupDto { Id = 1, Name = "Test Group", MemberCount = 3 };
            _groupServiceMock.Setup(s => s.GetGroupByIdAsync(1)).ReturnsAsync(group);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(group, okResult.Value);
        }

        [Fact]
        public async Task GetById_WhenGroupNotFound_ReturnsNotFound()
        {
            // Arrange
            _groupServiceMock.Setup(s => s.GetGroupByIdAsync(999)).ReturnsAsync((GroupDto?)null);

            // Act
            var result = await _controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_WhenValid_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateGroupDto
            {
                Name = "New Group",
                Description = "Description",
                CreatedBy = "user-123"
            };
            var createdGroup = new GroupDto { Id = 1, Name = dto.Name, Description = dto.Description };

            _groupServiceMock.Setup(s => s.CreateGroupAsync(dto)).ReturnsAsync(createdGroup);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            Assert.Equal(createdGroup, createdResult.Value);
        }

        [Fact]
        public async Task Update_WhenGroupExists_ReturnsNoContent()
        {
            // Arrange
            var dto = new UpdateGroupDto { Name = "Updated Name", Description = "Updated" };
            _groupServiceMock.Setup(s => s.UpdateGroupAsync(1, dto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_WhenGroupNotFound_ReturnsNotFound()
        {
            // Arrange
            var dto = new UpdateGroupDto { Name = "Updated Name" };
            _groupServiceMock.Setup(s => s.UpdateGroupAsync(999, dto)).ReturnsAsync(false);

            // Act
            var result = await _controller.Update(999, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
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
