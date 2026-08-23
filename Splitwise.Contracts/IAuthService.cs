using Splitwise.Contracts.DTOs.Auth;

namespace Splitwise.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Succeeded, AuthResponseDto? Result, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto);
        Task<(bool Succeeded, AuthResponseDto? Result, string? Error)> LoginAsync(LoginDto dto);
        // here we are using tuple to return multiple values from the method as 
        // generally from one function we can return only one value 
    }
}



