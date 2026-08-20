using System.ComponentModel.DataAnnotations;
using Splitwise.Utils.Enums;

namespace Splitwise.Contracts.DTOs.Expenses
{
    public class CreateExpenseDto
    {
        // Set by the controller from the route (api/groups/{groupId}/expenses),
        // not required from the client body.
        public int GroupId { get; set; }

        [Required]
        public string PaidBy { get; set; } = string.Empty;

        [Required, Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required]
        public SplitType SplitType { get; set; } = SplitType.Equal;

        [Required, MinLength(1)]
        public List<ExpenseSplitInputDto> Splits { get; set; } = new();
    }
}
