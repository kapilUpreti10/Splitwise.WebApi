using System.ComponentModel.DataAnnotations;

namespace Splitwise.Contracts.DTOs.Auth
{
    // Public self-registration. Always creates the caller as RoleNames.User —
    // unlike Admin.CreateUserDto, there is no Role field here, so no one can
    // hand themselves the Admin role through this endpoint.
    public class RegisterDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Address { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
