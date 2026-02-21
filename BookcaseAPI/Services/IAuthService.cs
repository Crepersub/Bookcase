using BookcaseAPI.DTOs;

namespace BookcaseAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> Register(RegisterRequest request);
        Task<AuthResponse?> Login(LoginRequest request);
        string GenerateJwtToken(string username, string role, int userId);
    }
}