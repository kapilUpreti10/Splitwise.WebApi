using System.ComponentModel.DataAnnotations;

namespace Splitwise.Contracts.DTOs.Expenses
{
    // Input shape: what the client sends for each participant.
    // Amount is only used for SplitType.Exact, Percentage only for SplitType.Percentage —
    // the service validates the right field is present for the chosen SplitType.
    public class ExpenseSplitInputDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        public decimal? Amount { get; set; }

        public decimal? Percentage { get; set; }
    }
}
