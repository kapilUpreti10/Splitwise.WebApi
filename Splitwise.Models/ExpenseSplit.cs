

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Splitwise.Models
{
    public class ExpenseSplit

        // this represent in a group which member is responsible for how much amount of the expense 
    {

        [Key]
        //public Guid Id { get; set; } = Guid.NewGuid(); 
        // we cannot write Guid.NewGuid() here it is dynamic and while adding seed data this class will also be used so 

        public Guid Id { get; set; }

        public int ExpenseId { get; set; }

        [ForeignKey(nameof(ExpenseId))]

        public Expense? Expense { get; set; }

        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]

        public ApplicationUser? User { get; set; }

        [Required]
        public decimal IndivudialAmount { get; set; }



    }
}
