using Microsoft.AspNetCore.Identity;
using Splitwise.Contracts.DTOs.Users;
using Splitwise.Models;
using Splitwise.Services.Interfaces;
using Splitwise.Utils.Enums;

namespace Splitwise.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserDto>();

            foreach (var user in users)
                result.Add(await MapToDtoAsync(user));

            return result;
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user == null ? null : await MapToDtoAsync(user);
        }

        public async Task<(bool Succeeded, string? UserId, IEnumerable<string> Errors)> CreateUserAsync(CreateUserDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return (false, null, result.Errors.Select(e => e.Description));

            var role = string.IsNullOrWhiteSpace(dto.Role) ? RoleNames.User : dto.Role;
            await EnsureRoleExistsAsync(role);
            await _userManager.AddToRoleAsync(user, role);

            return (true, user.Id, Enumerable.Empty<string>());
        }

        public async Task<bool> UpdateUserAsync(string id, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            if (dto.Name != null) user.Name = dto.Name;
            if (dto.Address != null) user.Address = dto.Address;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> AssignRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, new[] { "User not found." });

            await EnsureRoleExistsAsync(role);

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Count > 0)
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, role);
            return (result.Succeeded, result.Errors.Select(e => e.Description));
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            return await _userManager.GetRolesAsync(user);
        }

        private async Task EnsureRoleExistsAsync(string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
        }

        private async Task<UserDto> MapToDtoAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                Address = user.Address,
                CreatedAt = user.CreatedAt,
                Roles = roles
            };
        }
    }
}
