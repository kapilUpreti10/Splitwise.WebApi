using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Splitwise.Contracts.DTOs.Users;
using Splitwise.Services.Interfaces;
using Splitwise.Utils.Enums;

namespace Splitwise.WebApi.Controllers.Admin
{
    [Area("Admin")]
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = RoleNames.Admin)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userService.GetAllUsersAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            var (succeeded, userId, errors) = await _userService.CreateUserAsync(dto);
            if (!succeeded) return BadRequest(errors);

            var created = await _userService.GetUserByIdAsync(userId!);
            return CreatedAtAction(nameof(GetById), new { id = userId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
        {
            var ok = await _userService.UpdateUserAsync(id, dto);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _userService.DeleteUserAsync(id);
            return ok ? NoContent() : NotFound();
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> AssignRole(string id, [FromBody] AssignRoleDto dto)
        {
            var (succeeded, errors) = await _userService.AssignRoleAsync(id, dto.Role);
            return succeeded ? NoContent() : BadRequest(errors);
        }

        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetRoles(string id)
        {
            return Ok(await _userService.GetUserRolesAsync(id));
        }
    }
}
