using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Splitwise.Contracts.DTOs.Expenses;
using Splitwise.Services.Interfaces;
using Splitwise.Utils.Enums;

namespace Splitwise.WebApi.Controllers.User
{
    [Area("User")]
    [ApiController]
    [Route("api/groups/{groupId}/expenses")]
    [Authorize(Roles = RoleNames.User + "," + RoleNames.Admin)]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int groupId)
        {
            return Ok(await _expenseService.GetExpensesForGroupAsync(groupId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int groupId, int id)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            return expense == null ? NotFound() : Ok(expense);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int groupId, [FromBody] CreateExpenseDto dto)
        {
            dto.GroupId = groupId; // route is the source of truth, not the body
            var (succeeded, expense, error) = await _expenseService.CreateExpenseAsync(dto);
            if (!succeeded) return BadRequest(error);

            return CreatedAtAction(nameof(GetById), new { groupId, id = expense!.Id }, expense);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int groupId, int id, [FromBody] UpdateExpenseDto dto)
        {
            var (succeeded, error) = await _expenseService.UpdateExpenseAsync(id, dto);
            if (!succeeded) return error == "Expense not found." ? NotFound() : BadRequest(error);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int groupId, int id)
        {
            var ok = await _expenseService.DeleteExpenseAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
