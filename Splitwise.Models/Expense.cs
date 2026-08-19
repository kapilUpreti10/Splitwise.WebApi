using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Models
{
    public class Expense
    {

        [Key]
        public int Id { get; set; }

        public  int GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]

        public Group? GroupName { get; set; }

        public string PaidBy { get; set; } = string.Empty;

        [ForeignKey(nameof(PaidBy))]

        public ApplicationUser? PaidByUser { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }


        // since we have already linked expense and expense split by foregin key in expensesplit table as
        // it is one to many relationship so we write foreignkey in many side table 
        // here we are using navigation property ExpenseSplits which is list to get all 
        // expensesplits related to this expense 
        public ICollection<ExpenseSplit> ExpenseSplits { get; set; } = new List<ExpenseSplit>();
    }
}
