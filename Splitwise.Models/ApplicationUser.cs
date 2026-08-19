
using Microsoft.AspNetCore.Identity;
using Splitwise.Models;


namespace Splitwise.Models
{
    public class ApplicationUser:IdentityUser
    {

        public string? Name { get; set; }

        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; }


        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
    }
}
