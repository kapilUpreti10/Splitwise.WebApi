using Splitwise.Contracts.DTOs.Auth;

namespace Splitwise.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Succeeded, AuthResponseDto? Result, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto);
        Task<(bool Succeeded, AuthResponseDto? Result, string? Error)> LoginAsync(LoginDto dto);
    }
}
