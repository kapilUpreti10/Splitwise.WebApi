namespace Splitwise.Utils.Enums
{
    // Platform-wide identity roles (NOT the same as GroupMember roles, which
    // would be scoped to a single group if we add that concept later).
    public enum RoleType
    {

        // if we dont give value also behind the scene it will give 0 to first value and 1 to second value and so on
        User = 0,
        Admin = 1
    }
}
