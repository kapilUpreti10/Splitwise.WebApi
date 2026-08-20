namespace Splitwise.Utils.Enums
{
    // How an expense's total amount should be divided among participants.
    // Not persisted on the Expense entity itself — the caller specifies it
    // on create/update, the service resolves it into concrete ExpenseSplit
    // rows (always stored as final decimal amounts), and it's forgotten.
    public enum SplitType
    {
        Equal,
        Exact,
        Percentage
    }
}
