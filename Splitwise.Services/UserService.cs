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

            // as createAsync returns IdentityResult which contains options lik 
            // .Succeeded ,.Errors etc 
            if (!result.Succeeded)
                return (false, null, result.Errors.Select(e => e.Description));


            // if there is no role assigned in the dto then we will set defult role to user 
            // in frontend we dont provide the option to assign role to user so by default it will be user role
            // but since we have dto option to accept role so what if someone inject the role from sql query? 
            // so it may be the better pracatise to make soeme admin id initially by chaning code and making role =admin and later
            // we always make default role to user so that no new admin can be added 
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


            // this is custome function which is defined below but actually in 
            // _roleManager.RoleExistsAsync(role) we can check if role exists or not but
            // since we will be using this piece of code frequently so to avoid code duplication we have made a function

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


        // this is the private method to ensure that the role exists in the database before assigning it to a user.
        // since this piece of code will be used frequently so to reuse this code we have made the function
        private async Task EnsureRoleExistsAsync(string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
        }


        // this function is only accessible inside this class

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
