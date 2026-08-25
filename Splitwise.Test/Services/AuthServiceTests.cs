using Microsoft.AspNetCore.Identity;
using Moq;
using Splitwise.Contracts.DTOs.Auth;
using Splitwise.Models;
using Splitwise.Services;
using Splitwise.Services.Interfaces;
using Splitwise.Utils.Enums;

namespace Splitwise.Test.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userManagerMock = TestHelpers.CreateMockUserManager();
            _roleManagerMock = TestHelpers.CreateMockRoleManager();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _authService = new AuthService(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _jwtTokenServiceMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_WhenValid_ReturnsSuccessWithAuthResponse()
        {
            // Arrange
            var dto = new RegisterDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "Password123!",
                Address = "Test Address"
            };

            _userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(r => r.RoleExistsAsync(RoleNames.User)).ReturnsAsync(false);
            _roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), RoleNames.User))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { RoleNames.User });
            _jwtTokenServiceMock.Setup(j => j.CreateToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
                .Returns(("test-token", DateTime.UtcNow.AddHours(1)));

            // Act
            var (succeeded, result, errors) = await _authService.RegisterAsync(dto);

            // Assert
            Assert.True(succeeded);
            Assert.NotNull(result);
            Assert.Equal("test-token", result.Token);
            Assert.Empty(errors);
        }

        [Fact]
        public async Task RegisterAsync_WhenDuplicateEmail_ReturnsFailure()
        {
            // Arrange
            var dto = new RegisterDto
            {
                Name = "Test User",
                Email = "existing@example.com",
                Password = "Password123!"
            };
            var existingUser = new ApplicationUser { Email = dto.Email };

            _userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email)).ReturnsAsync(existingUser);

            // Act
            var (succeeded, result, errors) = await _authService.RegisterAsync(dto);

            // Assert
            Assert.False(succeeded);
            Assert.Null(result);
            Assert.Contains("A user with this email already exists.", errors);
        }

        [Fact]
        public async Task RegisterAsync_WhenPasswordValidationFails_ReturnsFailure()
        {
            // Arrange
            var dto = new RegisterDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "weak"
            };
            var identityErrors = new[] { new IdentityError { Description = "Password is too short" } };

            _userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Failed(identityErrors));

            // Act
            var (succeeded, result, errors) = await _authService.RegisterAsync(dto);

            // Assert
            Assert.False(succeeded);
            Assert.Null(result);
            Assert.Single(errors);
        }

        [Fact]
        public async Task LoginAsync_WhenValid_ReturnsSuccessWithAuthResponse()
        {
            // Arrange
            var dto = new LoginDto
            {
                Email = "test@example.com",
                Password = "Password123!"
            };
            var user = new ApplicationUser
            {
                Id = "user-id",
                Email = dto.Email,
                Name = "Test User"
            };

            _userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { RoleNames.User });
            _jwtTokenServiceMock.Setup(j => j.CreateToken(user, It.IsAny<IList<string>>()))
                .Returns(("test-token", DateTime.UtcNow.AddHours(1)));

            // Act
            var (succeeded, result, error) = await _authService.LoginAsync(dto);

            // Assert
            Assert.True(succeeded);
            Assert.NotNull(result);
            Assert.Equal("test-token", result.Token);
            Assert.Null(error);
        }

        [Fact]
        public async Task LoginAsync_WhenUserNotFound_ReturnsFailure()
        {
            // Arrange
            var dto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            _userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);

            // Act
            var (succeeded, result, error) = await _authService.LoginAsync(dto);

            // Assert
            Assert.False(succeeded);
            Assert.Null(result);
            Assert.Equal("Invalid email or password.", error);
        }

        [Fact]
        public async Task LoginAsync_WhenInvalidPassword_ReturnsFailure()
        {
            // Arrange
            var dto = new LoginDto
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };
            var user = new ApplicationUser { Email = dto.Email };

            _userManagerMock.Setup(u => u.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(false);

            // Act
            var (succeeded, result, error) = await _authService.LoginAsync(dto);

            // Assert
            Assert.False(succeeded);
            Assert.Null(result);
            Assert.Equal("Invalid email or password.", error);
        }
    }
}
