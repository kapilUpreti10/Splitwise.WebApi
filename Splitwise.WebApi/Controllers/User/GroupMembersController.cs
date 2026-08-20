using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Splitwise.Contracts.DTOs.GroupMembers;
using Splitwise.Services.Interfaces;
using Splitwise.Utils.Enums;

namespace Splitwise.WebApi.Controllers.User
{
    [Area("User")]
    [ApiController]
    [Route("api/groups/{groupId}/members")]
    [Authorize(Roles = RoleNames.User + "," + RoleNames.Admin)]
    public class GroupMembersController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupMembersController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMembers(int groupId)
        {
            return Ok(await _groupService.GetMembersAsync(groupId));
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(int groupId, [FromBody] AddGroupMemberDto dto)
        {
            var (succeeded, error) = await _groupService.AddMemberAsync(groupId, dto.UserId);
            return succeeded ? Ok() : BadRequest(error);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveMember(int groupId, string userId)
        {
            var ok = await _groupService.RemoveMemberAsync(groupId, userId);
            return ok ? NoContent() : NotFound();
        }
    }
}
