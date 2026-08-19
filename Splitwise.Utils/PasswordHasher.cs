using Microsoft.AspNetCore.Identity;


namespace Splitwise.Utils
{
    public static class PasswordHashGenerator
    {
        public static string Generate(string password)
        {
            var hasher = new PasswordHasher<IdentityUser>();

            var user = new IdentityUser();

            return hasher.HashPassword(user, password);
        }
        
    };

    
    }

