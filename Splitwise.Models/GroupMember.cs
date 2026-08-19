using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Splitwise.Models
{
    public class GroupMember
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        public int GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public Group? Group { get; set; }
    }
}