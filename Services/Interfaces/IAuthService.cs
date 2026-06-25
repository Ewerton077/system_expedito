using System.Security.Claims;

namespace SantoExpedito.Services.Interfaces;

public interface IAuthService
{
    Task<bool> ValidateUserAsync(string username, string password);
    ClaimsPrincipal CreatePrincipal(string username);
}
