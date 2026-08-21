using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Splitwise.Models;
using Splitwise.Services.Interfaces;

namespace Splitwise.Services
{
    // Reads the Jwt:* section from configuration (appsettings.json / user-secrets /
    // environment variables) and mints a signed, self-contained access token.
    // No database lookup is needed to validate a token later — the signature
    // and the claims embedded in it are enough. That's the whole point of JWTs.
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, DateTime ExpiresAtUtc) CreateToken(ApplicationUser user, IList<string> roles)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"]
                ?? throw new InvalidOperationException("Jwt:Key is not configured.");
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expiryMinutes = int.TryParse(jwtSection["ExpiryMinutes"], out var m) ? m : 60;

            var claims = new List<Claim>
            {
                // NameIdentifier is the standard claim controllers read to find
                // "the currently logged in user" via User.FindFirstValue(ClaimTypes.NameIdentifier).
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.Name, user.Name ?? user.UserName ?? string.Empty),
                // jti (unique token id) is useful later if you add token revocation/blacklisting.
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // One claim per role so [Authorize(Roles = "Admin")] can match on ClaimTypes.Role.
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenString, expires);
        }
    }
}
