using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Splitwise.Services.Interfaces;
using Splitwise.Utils.Enums;

namespace Splitwise.WebApi.Controllers.Admin
{
    // Admin-only visibility across ALL groups (not just ones you belong to),
    // plus the ability to remove any group. Regular per-user group CRUD
    // lives in Controllers/User/GroupsController.cs.
    [Area("Admin")]
    [ApiController]
    [Route("api/admin/groups")]
    [Authorize(Roles = RoleNames.Admin)]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _groupService.GetAllGroupsAsync());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _groupService.DeleteGroupAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
