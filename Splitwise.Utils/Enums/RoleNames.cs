namespace Splitwise.Utils.Enums
{
    // [Authorize(Roles = "...")] requires a compile-time constant string,
    // so we can't pass the RoleType enum directly into the attribute.
    // These consts mirror RoleType by name so every [Authorize] attribute
    // in the app references RoleNames.Admin / RoleNames.User instead of
    // a hardcoded "Admin" / "User" string.
    public static class RoleNames
    {
        public const string User = nameof(RoleType.User);
        public const string Admin = nameof(RoleType.Admin);
    }
}
