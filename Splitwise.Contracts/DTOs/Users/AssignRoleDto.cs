using System.ComponentModel.DataAnnotations;

namespace Splitwise.Contracts.DTOs.Users
{
    public class AssignRoleDto
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
