
using Microsoft.AspNetCore.Identity;
using Splitwise.Models;


namespace Splitwise.Models
{
    public class ApplicationUser:IdentityUser
    {

        public string? Name { get; set; }

        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; }



        // navigation property 
        // How ef core knows GroupMembers is navigation property as we havent mentioned any foreign key?

        // since it sees GroupMembers is List of GroupMember which is an entity ie our model class so it 
        // understands one Application user can be related to many GroupMember records
        //so it will create foreign key in GroupMember table with name ApplicationUserId
        public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();



        // generally for navigation property we use ICollection<T> instead of List<T>
        // because ICollection<T> is more flexible and allows for better abstraction.
        // It provides a more general interface for working with collections, allowing for different implementations (like List<T>, HashSet<T>, etc.)
        // to be used without changing the code that interacts with the collection.
        // This can make the code more maintainable and adaptable to future changes.
    }
}
