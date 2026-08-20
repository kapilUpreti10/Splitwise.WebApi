using Microsoft.AspNetCore.Identity;
using Splitwise.Utils.Enums;

namespace Splitwise.WebApi.Startup
{
    // Ensures the "User" and "Admin" roles exist in AspNetRoles at app startup,
    // so [Authorize(Roles = RoleNames.Admin)] has something real to check against.
    // Call RoleSeeder.SeedAsync(app) once, right after building the app, before app.Run().
    public static class RoleSeeder
    {
        public static async Task SeedAsync(WebApplication app)
        {

            // note::
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var roleName in new[] { RoleNames.User, RoleNames.Admin })
            {
                if (!await roleManager.RoleExistsAsync(roleName))

                    // if in RoleManager<identityRole> if the role user ,admin doesnt exits then it creates the role in the db 
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
