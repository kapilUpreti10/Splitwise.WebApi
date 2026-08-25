using Microsoft.AspNetCore.Mvc;
using Moq;
using Splitwise.Contracts.DTOs.Users;
using Splitwise.Services.Interfaces;
using Splitwise.WebApi.Controllers.Admin;

namespace Splitwise.Test.Controllers.Admin
{
    public class AdminUsersControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UsersController _controller;

        public AdminUsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UsersController(_userServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithAllUsers()
        {
            // Arrange
            var users = new List<UserDto>
            {
                new UserDto { Id = "1", Name = "User 1", Email = "user1@test.com" },
                new UserDto { Id = "2", Name = "User 2", Email = "user2@test.com" }
            };
            _userServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(users, okResult.Value);
        }

        [Fact]
        public async Task GetById_WhenUserExists_ReturnsOkWithUser()
        {
            // Arrange
            var user = new UserDto { Id = "test-id", Name = "Test User", Email = "test@test.com" };
            _userServiceMock.Setup(s => s.GetUserByIdAsync("test-id")).ReturnsAsync(user);

            // Act
            var result = await _controller.GetById("test-id");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task GetById_WhenUserNotFound_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(s => s.GetUserByIdAsync("invalid-id")).ReturnsAsync((UserDto?)null);

            // Act
            var result = await _controller.GetById("invalid-id");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_WhenValid_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Name = "New User",
                Email = "new@test.com",
                Password = "password123"
            };
            var createdUser = new UserDto { Id = "new-id", Name = dto.Name, Email = dto.Email };

            _userServiceMock
                .Setup(s => s.CreateUserAsync(dto))
                .ReturnsAsync((true, "new-id", Enumerable.Empty<string>()));
            _userServiceMock
                .Setup(s => s.GetUserByIdAsync("new-id"))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            Assert.Equal(createdUser, createdResult.Value);
        }

        [Fact]
        public async Task Create_WhenInvalid_ReturnsBadRequest()
        {
            // Arrange
            var dto = new CreateUserDto { Name = "", Email = "invalid", Password = "123" };
            var errors = new[] { "Email is invalid", "Password too short" };

            _userServiceMock
                .Setup(s => s.CreateUserAsync(dto))
                .ReturnsAsync((false, null, errors));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errors, badRequestResult.Value);
        }

        [Fact]
        public async Task Update_WhenUserExists_ReturnsNoContent()
        {
            // Arrange
            var dto = new UpdateUserDto { Name = "Updated Name" };
            _userServiceMock.Setup(s => s.UpdateUserAsync("test-id", dto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update("test-id", dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_WhenUserNotFound_ReturnsNotFound()
        {
            // Arrange
            var dto = new UpdateUserDto { Name = "Updated Name" };
            _userServiceMock.Setup(s => s.UpdateUserAsync("invalid-id", dto)).ReturnsAsync(false);

            // Act
            var result = await _controller.Update("invalid-id", dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_WhenUserExists_ReturnsNoContent()
        {
            // Arrange
            _userServiceMock.Setup(s => s.DeleteUserAsync("test-id")).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete("test-id");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WhenUserNotFound_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(s => s.DeleteUserAsync("invalid-id")).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete("invalid-id");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AssignRole_WhenValid_ReturnsNoContent()
        {
            // Arrange
            var dto = new AssignRoleDto { Role = "Admin" };
            _userServiceMock
                .Setup(s => s.AssignRoleAsync("test-id", "Admin"))
                .ReturnsAsync((true, Enumerable.Empty<string>()));

            // Act
            var result = await _controller.AssignRole("test-id", dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AssignRole_WhenInvalid_ReturnsBadRequest()
        {
            // Arrange
            var dto = new AssignRoleDto { Role = "InvalidRole" };
            var errors = new[] { "Role does not exist" };
            _userServiceMock
                .Setup(s => s.AssignRoleAsync("test-id", "InvalidRole"))
                .ReturnsAsync((false, errors));

            // Act
            var result = await _controller.AssignRole("test-id", dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errors, badRequestResult.Value);
        }

        [Fact]
        public async Task GetRoles_ReturnsOkWithRoles()
        {
            // Arrange
            var roles = new List<string> { "User", "Admin" };
            _userServiceMock.Setup(s => s.GetUserRolesAsync("test-id")).ReturnsAsync(roles);

            // Act
            var result = await _controller.GetRoles("test-id");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(roles, okResult.Value);
        }
    }
}
