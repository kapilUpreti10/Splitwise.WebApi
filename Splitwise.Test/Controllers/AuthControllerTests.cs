using Microsoft.AspNetCore.Mvc;
using Moq;
using Splitwise.Contracts.DTOs.Auth;
using Splitwise.Contracts.DTOs.Users;
using Splitwise.Services.Interfaces;
using Splitwise.WebApi.Controllers;

namespace Splitwise.Test.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_WhenSuccessful_ReturnsOkWithAuthResponse()
        {
            // Arrange
            var dto = new RegisterDto
            {
                Name = "Test User",
                Email = "test@example.com",
                Password = "password123",
                Address = "Test Address"
            };
            var response = new AuthResponseDto
            {
                Token = "test-token",
                ExpiresAtUtc = DateTime.UtcNow.AddHours(1),
                User = new UserDto { Id = "user-id", Email = dto.Email, Name = dto.Name }
            };

            _authServiceMock
                .Setup(s => s.RegisterAsync(dto))
                .ReturnsAsync((true, response, Enumerable.Empty<string>()));

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
            _authServiceMock.Verify(s => s.RegisterAsync(dto), Times.Once);
        }

        [Fact]
        public async Task Register_WhenDuplicateEmail_ReturnsBadRequestWithErrors()
        {
            // Arrange
            var dto = new RegisterDto
            {
                Name = "Test User",
                Email = "existing@example.com",
                Password = "password123"
            };
            var errors = new[] { "A user with this email already exists." };

            _authServiceMock
                .Setup(s => s.RegisterAsync(dto))
                .ReturnsAsync((false, null, errors));

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errors, badRequestResult.Value);
            _authServiceMock.Verify(s => s.RegisterAsync(dto), Times.Once);
        }

        [Fact]
        public async Task Login_WhenSuccessful_ReturnsOkWithAuthResponse()
        {
            // Arrange
            var dto = new LoginDto
            {
                Email = "test@example.com",
                Password = "password123"
            };
            var response = new AuthResponseDto
            {
                Token = "test-token",
                ExpiresAtUtc = DateTime.UtcNow.AddHours(1),
                User = new UserDto { Id = "user-id", Email = dto.Email }
            };

            _authServiceMock
                .Setup(s => s.LoginAsync(dto))
                .ReturnsAsync((true, response, null));

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
            _authServiceMock.Verify(s => s.LoginAsync(dto), Times.Once);
        }

        [Fact]
        public async Task Login_WhenInvalidCredentials_ReturnsUnauthorizedWithMessage()
        {
            // Arrange
            var dto = new LoginDto
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };
            var error = "Invalid email or password.";

            _authServiceMock
                .Setup(s => s.LoginAsync(dto))
                .ReturnsAsync((false, null, error));

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult.Value);
            var messageProperty = unauthorizedResult.Value.GetType().GetProperty("message");
            Assert.NotNull(messageProperty);
            Assert.Equal(error, messageProperty.GetValue(unauthorizedResult.Value));
            _authServiceMock.Verify(s => s.LoginAsync(dto), Times.Once);
        }
    }
}
