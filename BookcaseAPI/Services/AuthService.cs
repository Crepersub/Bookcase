using BookcaseAPI.Data;
using BookcaseAPI.DTOs;
using BookcaseAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BookcaseAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse?> Register(RegisterRequest request)
        {
            if (await _context.Clients.AnyAsync(c => c.Username == request.Username))
            {
                return null;
            }

            var client = new Client
            {
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                Role = request.Role == "Admin" ? "Admin" : "User"
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(client.Username, client.Role, client.Id);

            return new AuthResponse
            {
                Token = token,
                Username = client.Username,
                Role = client.Role
            };
        }

        public async Task<AuthResponse?> Login(LoginRequest request)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Username == request.Username);

            if (client == null || !VerifyPassword(request.Password, client.PasswordHash))
            {
                return null;
            }

            var token = GenerateJwtToken(client.Username, client.Role, client.Id);

            return new AuthResponse
            {
                Token = token,
                Username = client.Username,
                Role = client.Role
            };
        }

        public string GenerateJwtToken(string username, string role, int userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }
    }
}