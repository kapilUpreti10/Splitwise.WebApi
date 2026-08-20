using System.ComponentModel.DataAnnotations;

namespace Splitwise.Contracts.DTOs.Groups
{
    public class UpdateGroupDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
