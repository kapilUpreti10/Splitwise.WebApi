



    namespace Splitwise.Models
    {
        public class  GroupMember
        {

        // as by default the id created from identity is of type string 
        public string UserId { get; set; } = string.Empty;

            public ApplicationUser? User { get; set; }

            public int GroupId { get; set; }

            public Group? Group { get; set; }


        
        }
    }
