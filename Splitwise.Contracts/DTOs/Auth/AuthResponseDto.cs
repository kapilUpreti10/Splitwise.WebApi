using Splitwise.Contracts.DTOs.Users;

namespace Splitwise.Contracts.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public UserDto User { get; set; } = null!;
    }
}
