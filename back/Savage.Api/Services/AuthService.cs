using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Savage.Api.DTOs;
using Savage.Api.Models;

namespace Savage.Api.Services;

/// <summary>
/// Implements JWT-based authentication with BCrypt password hashing.
/// Issues role-bearing tokens for Admin and Vendor users.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IMongoCollection<User> _users;
    private readonly IConfiguration _config;

    public AuthService(IMongoDatabase database, IConfiguration config)
    {
        _users = database.GetCollection<User>("Users");
        _config = config;
    }

    /// <summary>
    /// Authenticates a user by username/password and returns a JWT token.
    /// </summary>
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _users.Find(u => u.Username == request.Username).FirstOrDefaultAsync();
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return GenerateResponse(user);
    }

    /// <summary>
    /// Registers a new user, hashes their password, and returns a JWT token.
    /// </summary>
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var exists = await _users.Find(u => u.Username == request.Username).AnyAsync();
        if (exists) return null;

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            VendorId = request.VendorId
        };
        await _users.InsertOneAsync(user);
        return GenerateResponse(user);
    }

    /// <summary>
    /// Builds a JWT token containing the user's id, username, role, and optional vendorId.
    /// </summary>
    private AuthResponse GenerateResponse(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role)
        };
        if (user.VendorId != null)
            claims.Add(new Claim("vendorId", user.VendorId));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Username = user.Username,
            Role = user.Role,
            VendorId = user.VendorId
        };
    }
}
