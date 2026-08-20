using System.ComponentModel.DataAnnotations;

namespace Splitwise.Contracts.DTOs.GroupMembers
{
    public class AddGroupMemberDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}
