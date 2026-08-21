namespace Splitwise.Contracts.DTOs.Groups
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CreatedBy { get; set; } = string.Empty;

        // this is the additional property we have added where we get value 
        // from the navigation property
        public string? CreatedByName { get; set; }

        // this is timestamp which db automatically adds and 
        // we are also returning it in the final op
        public DateTime CreatedAt { get; set; }

        // this is the additional property we have added
        public int MemberCount { get; set; }
    }
}
