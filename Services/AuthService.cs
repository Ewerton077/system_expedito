using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SantoExpedito.Data;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ValidateUserAsync(string username, string password)
    {
        var admin = await _context.AdminUsers.FirstOrDefaultAsync(u => u.Username == username);
        if (admin == null) return false;
        return BCryptHelper.VerifyPassword(password, admin.PasswordHash);
    }

    public ClaimsPrincipal CreatePrincipal(string username)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "Cookies");
        return new ClaimsPrincipal(identity);
    }
}
