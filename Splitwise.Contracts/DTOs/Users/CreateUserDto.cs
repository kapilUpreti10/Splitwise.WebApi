using System.ComponentModel.DataAnnotations;

namespace Splitwise.Contracts.DTOs.Users
{
    // Admin uses this to create a user directly (no self-registration/auth flow yet).
    public class CreateUserDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Address { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        // Optional — defaults to RoleNames.User if not supplied.
        public string? Role { get; set; }
    }
}
