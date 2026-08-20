using Splitwise.Contracts.DTOs.Users;

namespace Splitwise.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<(bool Succeeded, string? UserId, IEnumerable<string> Errors)> CreateUserAsync(CreateUserDto dto);
        Task<bool> UpdateUserAsync(string id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(string id);
        Task<(bool Succeeded, IEnumerable<string> Errors)> AssignRoleAsync(string userId, string role);
        Task<IList<string>> GetUserRolesAsync(string userId);
    }
}
