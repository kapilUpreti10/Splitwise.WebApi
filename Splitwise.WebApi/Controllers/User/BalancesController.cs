using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Splitwise.Services.Interfaces;
using Splitwise.Utils.Enums;

namespace Splitwise.WebApi.Controllers.User
{
    [Area("User")]
    [ApiController]
    [Route("api/groups/{groupId}/balances")]
    [Authorize(Roles = RoleNames.User + "," + RoleNames.Admin)]
    public class BalancesController : ControllerBase
    {
        private readonly IBalanceService _balanceService;

        public BalancesController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupBalance(int groupId)
        {
            var balance = await _balanceService.GetGroupBalanceAsync(groupId);
            return balance == null ? NotFound() : Ok(balance);
        }
    }
}
