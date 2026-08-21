

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Splitwise.Models;


    namespace Splitwise.Models
    {
        public class Group
        {

            //[Required]
            // since int is already non nullable so making it required adds nothing 

            [Key]
            public int Id { get; set; }

            [Required]
            public string Name { get; set; } = string.Empty;


            public string? Description { get; set; }

        //[Required]
        public string CreatedBy { get; set; } = string.Empty;

            [ForeignKey(nameof(CreatedBy))]
            // this is for navigation property 


            public ApplicationUser? CreatedByUser { get; set; }


            public DateTime CreatedAt { get; set; }




        // this is also navigation property which is used to get the list of group members for this group
        public ICollection<GroupMember> GroupMembers = new List<GroupMember>();

        
        }
    }
