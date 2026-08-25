using Microsoft.AspNetCore.Mvc;
using Moq;
using Splitwise.Contracts.DTOs.GroupMembers;
using Splitwise.Services.Interfaces;
using Splitwise.WebApi.Controllers.User;

namespace Splitwise.Test.Controllers.User
{
    public class UserGroupMembersControllerTests
    {
        private readonly Mock<IGroupService> _groupServiceMock;
        private readonly GroupMembersController _controller;

        public UserGroupMembersControllerTests()
        {
            _groupServiceMock = new Mock<IGroupService>();
            _controller = new GroupMembersController(_groupServiceMock.Object);
        }

        [Fact]
        public async Task GetMembers_ReturnsOkWithMembers()
        {
            // Arrange
            var members = new List<GroupMemberDto>
            {
                new GroupMemberDto { Id = 1, GroupId = 1, UserId = "user-1", UserName = "User 1" },
                new GroupMemberDto { Id = 2, GroupId = 1, UserId = "user-2", UserName = "User 2" }
            };
            _groupServiceMock.Setup(s => s.GetMembersAsync(1)).ReturnsAsync(members);

            // Act
            var result = await _controller.GetMembers(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(members, okResult.Value);
        }

        [Fact]
        public async Task AddMember_WhenValid_ReturnsOk()
        {
            // Arrange
            var dto = new AddGroupMemberDto { UserId = "user-3" };
            _groupServiceMock
                .Setup(s => s.AddMemberAsync(1, "user-3"))
                .ReturnsAsync((true, (string?)null));

            // Act
            var result = await _controller.AddMember(1, dto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddMember_WhenGroupNotFound_ReturnsBadRequest()
        {
            // Arrange
            var dto = new AddGroupMemberDto { UserId = "user-3" };
            _groupServiceMock
                .Setup(s => s.AddMemberAsync(999, "user-3"))
                .ReturnsAsync((false, "Group not found."));

            // Act
            var result = await _controller.AddMember(999, dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Group not found.", badRequestResult.Value);
        }

        [Fact]
        public async Task AddMember_WhenAlreadyMember_ReturnsBadRequest()
        {
            // Arrange
            var dto = new AddGroupMemberDto { UserId = "user-1" };
            _groupServiceMock
                .Setup(s => s.AddMemberAsync(1, "user-1"))
                .ReturnsAsync((false, "User is already a member of this group."));

            // Act
            var result = await _controller.AddMember(1, dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User is already a member of this group.", badRequestResult.Value);
        }

        [Fact]
        public async Task RemoveMember_WhenValid_ReturnsNoContent()
        {
            // Arrange
            _groupServiceMock.Setup(s => s.RemoveMemberAsync(1, "user-1")).ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveMember(1, "user-1");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RemoveMember_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            _groupServiceMock.Setup(s => s.RemoveMemberAsync(1, "invalid-user")).ReturnsAsync(false);

            // Act
            var result = await _controller.RemoveMember(1, "invalid-user");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
