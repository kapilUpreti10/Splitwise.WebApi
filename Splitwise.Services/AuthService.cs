using Microsoft.AspNetCore.Identity;
using Splitwise.Contracts.DTOs.Auth;
using Splitwise.Contracts.DTOs.Users;
using Splitwise.Models;
using Splitwise.Services.Interfaces;
using Splitwise.Utils.Enums;

namespace Splitwise.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<(bool Succeeded, AuthResponseDto? Result, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            
            if (existing != null)
                return (false, null, new[] { "A user with this email already exists." });

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            
            if (!createResult.Succeeded)
                return (false, null, createResult.Errors.Select(e => e.Description));

            // Self-registration is always a plain "User" — Admin can promote via
            // PUT /api/admin/users/{id}/role afterwards.
            if (!await _roleManager.RoleExistsAsync(RoleNames.User))
                // here RoleNames is enum and we are checking if the role of type User
                // exists or not if not create the role of type User
                await _roleManager.CreateAsync(new IdentityRole(RoleNames.User));
            await _userManager.AddToRoleAsync(user, RoleNames.User);


            // here we are building the auth response for the user
            // after sucessful registration and returning it to the client
            var authResponse = await BuildAuthResponseAsync(user);
            return (true, authResponse, Enumerable.Empty<string>());
        }

        public async Task<(bool Succeeded, AuthResponseDto? Result, string? Error)> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return (false, null, "Invalid email or password.");

            // CheckPasswordAsync compares against the ASP.NET Core Identity hash
            // (PBKDF2) stored on the user — never compare plaintext passwords yourself.
            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
                return (false, null, "Invalid email or password.");

            var authResponse = await BuildAuthResponseAsync(user);
            return (true, authResponse, null);
        }

        private async Task<AuthResponseDto> BuildAuthResponseAsync(ApplicationUser user)
        {

            // here since we are creating jwt token for login and register but generally it should be in login 
            // it is because of design choice since once register is successful we dont want user to login again 
            // so we provide token and assumes the user is logged in 



            var roles = await _userManager.GetRolesAsync(user);
            var (token, expiresAtUtc) = _jwtTokenService.CreateToken(user, roles);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    UserName = user.UserName,
                    Email = user.Email,
                    Address = user.Address,
                    CreatedAt = user.CreatedAt,
                    Roles = roles
                }
            };
        }
    }
}
