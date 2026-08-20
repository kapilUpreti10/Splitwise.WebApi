namespace Splitwise.Contracts.DTOs.GroupMembers
{
    public class GroupMemberDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Name { get; set; }
    }
}
