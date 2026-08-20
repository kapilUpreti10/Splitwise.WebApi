using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Splitwise.Contracts.DTOs.Groups;
using Splitwise.Services.Interfaces;
using Splitwise.Utils.Enums;

namespace Splitwise.WebApi.Controllers.User
{
    [Area("User")]
    [ApiController]
    [Route("api/groups")]
    [Authorize(Roles = RoleNames.User + "," + RoleNames.Admin)]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        // TEMPORARY: userId comes from the query string until JWT auth exists.
        // Once auth is wired up, read it from User.FindFirstValue(ClaimTypes.NameIdentifier)
        // instead of trusting the caller.
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyGroups([FromQuery] string userId)
        {
            return Ok(await _groupService.GetGroupsForUserAsync(userId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            return group == null ? NotFound() : Ok(group);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGroupDto dto)
        {
            var group = await _groupService.CreateGroupAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGroupDto dto)
        {
            var ok = await _groupService.UpdateGroupAsync(id, dto);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _groupService.DeleteGroupAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
