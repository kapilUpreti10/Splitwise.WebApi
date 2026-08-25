using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Moq;
using Splitwise.Models;
using Splitwise.Services;

namespace Splitwise.Test.Services
{
    public class JwtTokenServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IConfigurationSection> _jwtSectionMock;

        public JwtTokenServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _jwtSectionMock = new Mock<IConfigurationSection>();
        }

        [Fact]
        public void CreateToken_WhenValid_ReturnsTokenAndExpiry()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "user-id",
                Email = "test@example.com",
                Name = "Test User",
                UserName = "testuser"
            };
            var roles = new List<string> { "User" };

            SetupJwtConfiguration("TestSecretKeyThatIsAtLeast32CharactersLong", "TestIssuer", "TestAudience", "60");

            var service = new JwtTokenService(_configurationMock.Object);

            // Act
            var (token, expiresAtUtc) = service.CreateToken(user, roles);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
            Assert.True(expiresAtUtc > DateTime.UtcNow);
        }

        [Fact]
        public void CreateToken_ContainsCorrectClaims()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "user-id",
                Email = "test@example.com",
                Name = "Test User",
                UserName = "testuser"
            };
            var roles = new List<string> { "User", "Admin" };

            SetupJwtConfiguration("TestSecretKeyThatIsAtLeast32CharactersLong", "TestIssuer", "TestAudience", "60");

            var service = new JwtTokenService(_configurationMock.Object);

            // Act
            var (token, _) = service.CreateToken(user, roles);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var claims = jwtToken.Claims.ToList();

            Assert.Contains(claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == "user-id");
            Assert.Contains(claims, c => c.Type == ClaimTypes.Email && c.Value == "test@example.com");
            Assert.Contains(claims, c => c.Type == ClaimTypes.Name && c.Value == "Test User");
            Assert.Contains(claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
            Assert.Contains(claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        }

        [Fact]
        public void CreateToken_WhenKeyNotConfigured_ThrowsInvalidOperationException()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user-id", Email = "test@example.com" };
            var roles = new List<string> { "User" };

            _jwtSectionMock.Setup(s => s["Key"]).Returns((string?)null);
            _configurationMock.Setup(c => c.GetSection("Jwt")).Returns(_jwtSectionMock.Object);

            var service = new JwtTokenService(_configurationMock.Object);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => service.CreateToken(user, roles));
        }

        [Fact]
        public void CreateToken_UsesConfiguredExpiry()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user-id", Email = "test@example.com" };
            var roles = new List<string> { "User" };
            var customExpiryMinutes = 120;

            SetupJwtConfiguration("TestSecretKeyThatIsAtLeast32CharactersLong", "TestIssuer", "TestAudience", customExpiryMinutes.ToString());

            var service = new JwtTokenService(_configurationMock.Object);
            var beforeCreate = DateTime.UtcNow;

            // Act
            var (_, expiresAtUtc) = service.CreateToken(user, roles);

            // Assert
            var expectedExpiry = beforeCreate.AddMinutes(customExpiryMinutes);
            var tolerance = TimeSpan.FromSeconds(5);
            Assert.True(Math.Abs((expiresAtUtc - expectedExpiry).TotalSeconds) < tolerance.TotalSeconds);
        }

        [Fact]
        public void CreateToken_UsesDefaultExpiryWhenNotConfigured()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user-id", Email = "test@example.com" };
            var roles = new List<string> { "User" };

            SetupJwtConfiguration("TestSecretKeyThatIsAtLeast32CharactersLong", "TestIssuer", "TestAudience", null);

            var service = new JwtTokenService(_configurationMock.Object);
            var beforeCreate = DateTime.UtcNow;

            // Act
            var (_, expiresAtUtc) = service.CreateToken(user, roles);

            // Assert
            var expectedExpiry = beforeCreate.AddMinutes(60); // Default 60 minutes
            var tolerance = TimeSpan.FromSeconds(5);
            Assert.True(Math.Abs((expiresAtUtc - expectedExpiry).TotalSeconds) < tolerance.TotalSeconds);
        }

        private void SetupJwtConfiguration(string key, string issuer, string audience, string? expiryMinutes)
        {
            _jwtSectionMock.Setup(s => s["Key"]).Returns(key);
            _jwtSectionMock.Setup(s => s["Issuer"]).Returns(issuer);
            _jwtSectionMock.Setup(s => s["Audience"]).Returns(audience);
            _jwtSectionMock.Setup(s => s["ExpiryMinutes"]).Returns(expiryMinutes);
            _configurationMock.Setup(c => c.GetSection("Jwt")).Returns(_jwtSectionMock.Object);
        }
    }
}
