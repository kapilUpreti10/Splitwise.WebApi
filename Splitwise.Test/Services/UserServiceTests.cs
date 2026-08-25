using Microsoft.AspNetCore.Identity;
using Moq;
using Splitwise.Contracts.DTOs.Users;
using Splitwise.Models;
using Splitwise.Services;
using Splitwise.Utils.Enums;

namespace Splitwise.Test.Services
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userManagerMock = TestHelpers.CreateMockUserManager();
            _roleManagerMock = TestHelpers.CreateMockRoleManager();
            _userService = new UserService(_userManagerMock.Object, _roleManagerMock.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", Email = "user1@test.com", Name = "User 1" },
                new ApplicationUser { Id = "2", Email = "user2@test.com", Name = "User 2" }
            };

            _userManagerMock.Setup(u => u.Users).Returns(users.AsQueryable());
            _userManagerMock.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("User 1", result[0].Name);
            Assert.Equal("User 2", result[1].Name);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenValid_ReturnsUserDto()
        {
            // Arrange
            var user = new ApplicationUser { Id = "test-id", Email = "test@test.com", Name = "Test User" };
            _userManagerMock.Setup(u => u.FindByIdAsync("test-id")).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _userService.GetUserByIdAsync("test-id");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test-id", result.Id);
            Assert.Equal("Test User", result.Name);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenInvalid_ReturnsNull()
        {
            // Arrange
            _userManagerMock.Setup(u => u.FindByIdAsync("invalid-id")).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _userService.GetUserByIdAsync("invalid-id");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUserAsync_WhenValid_ReturnsSuccess()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Name = "New User",
                Email = "new@test.com",
                Password = "Password123!",
                Address = "Test Address"
            };

            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(r => r.RoleExistsAsync(RoleNames.User)).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), RoleNames.User))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var (succeeded, userId, errors) = await _userService.CreateUserAsync(dto);

            // Assert
            Assert.True(succeeded);
            Assert.NotNull(userId);
            Assert.Empty(errors);
        }

        [Fact]
        public async Task CreateUserAsync_WhenInvalid_ReturnsFailure()
        {
            // Arrange
            var dto = new CreateUserDto { Name = "", Email = "invalid", Password = "weak" };
            var identityErrors = new[] { new IdentityError { Description = "Email is invalid" } };

            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Failed(identityErrors));

            // Act
            var (succeeded, userId, errors) = await _userService.CreateUserAsync(dto);

            // Assert
            Assert.False(succeeded);
            Assert.Null(userId);
            Assert.NotEmpty(errors);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenValid_ReturnsTrue()
        {
            // Arrange
            var user = new ApplicationUser { Id = "test-id", Name = "Old Name" };
            var dto = new UpdateUserDto { Name = "New Name", Address = "New Address" };

            _userManagerMock.Setup(u => u.FindByIdAsync("test-id")).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.UpdateUserAsync("test-id", dto);

            // Assert
            Assert.True(result);
            Assert.Equal("New Name", user.Name);
            Assert.Equal("New Address", user.Address);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenInvalid_ReturnsFalse()
        {
            // Arrange
            var dto = new UpdateUserDto { Name = "New Name" };
            _userManagerMock.Setup(u => u.FindByIdAsync("invalid-id")).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _userService.UpdateUserAsync("invalid-id", dto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenValid_ReturnsTrue()
        {
            // Arrange
            var user = new ApplicationUser { Id = "test-id" };
            _userManagerMock.Setup(u => u.FindByIdAsync("test-id")).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.DeleteUserAsync("test-id");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenInvalid_ReturnsFalse()
        {
            // Arrange
            _userManagerMock.Setup(u => u.FindByIdAsync("invalid-id")).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _userService.DeleteUserAsync("invalid-id");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AssignRoleAsync_WhenValid_ReturnsSuccess()
        {
            // Arrange
            var user = new ApplicationUser { Id = "test-id" };
            _userManagerMock.Setup(u => u.FindByIdAsync("test-id")).ReturnsAsync(user);
            _roleManagerMock.Setup(r => r.RoleExistsAsync("Admin")).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
            _userManagerMock.Setup(u => u.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.AddToRoleAsync(user, "Admin"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var (succeeded, errors) = await _userService.AssignRoleAsync("test-id", "Admin");

            // Assert
            Assert.True(succeeded);
            Assert.Empty(errors);
        }

        [Fact]
        public async Task AssignRoleAsync_WhenUserNotFound_ReturnsFailure()
        {
            // Arrange
            _userManagerMock.Setup(u => u.FindByIdAsync("invalid-id")).ReturnsAsync((ApplicationUser?)null);

            // Act
            var (succeeded, errors) = await _userService.AssignRoleAsync("invalid-id", "Admin");

            // Assert
            Assert.False(succeeded);
            Assert.Contains("User not found.", errors);
        }

        [Fact]
        public async Task GetUserRolesAsync_WhenValid_ReturnsRoles()
        {
            // Arrange
            var user = new ApplicationUser { Id = "test-id" };
            var roles = new List<string> { "User", "Admin" };
            _userManagerMock.Setup(u => u.FindByIdAsync("test-id")).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(roles);

            // Act
            var result = await _userService.GetUserRolesAsync("test-id");

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains("User", result);
            Assert.Contains("Admin", result);
        }

        [Fact]
        public async Task GetUserRolesAsync_WhenUserNotFound_ReturnsEmptyList()
        {
            // Arrange
            _userManagerMock.Setup(u => u.FindByIdAsync("invalid-id")).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _userService.GetUserRolesAsync("invalid-id");

            // Assert
            Assert.Empty(result);
        }
    }
}
