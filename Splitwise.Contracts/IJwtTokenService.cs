using Splitwise.Models;

namespace Splitwise.Services.Interfaces
{
    public interface IJwtTokenService
    {
        // Builds a signed JWT for the given user + their current roles.
        // Returns the raw token string plus its UTC expiry, so callers
        // (AuthController) can hand both back to the client.
        (string Token, DateTime ExpiresAtUtc) CreateToken(ApplicationUser user, IList<string> roles);
    }
}
