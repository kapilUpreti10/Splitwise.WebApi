namespace Splitwise.Contracts.DTOs.Users
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
