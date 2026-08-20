using System.ComponentModel.DataAnnotations;

namespace Splitwise.Contracts.DTOs.Groups
{
    public class CreateGroupDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // TEMPORARY: until JWT auth exists, the caller tells us who they are.
        // Once auth is added, replace this with the authenticated user's id
        // taken from the token instead of trusting the request body.
        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        // Optional additional members to add at creation time (creator is always added).
        public List<string>? MemberUserIds { get; set; }
    }
}
