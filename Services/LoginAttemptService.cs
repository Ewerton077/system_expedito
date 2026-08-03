using Microsoft.Extensions.Caching.Memory;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Services;

public class LoginAttemptService : ILoginAttemptService
{
    private readonly IMemoryCache _cache;
    private const int MaxAttempts = 5;
    private const int BlockMinutes = 5;
    private const string Prefix = "login_attempts_";

    public LoginAttemptService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool IsBlocked(string ipAddress)
    {
        var key = Prefix + ipAddress;
        var attempts = _cache.Get<int>(key);
        if (attempts >= MaxAttempts)
        {
            if (_cache.TryGetValue(key + "_blocked_until", out DateTime blockedUntil))
            {
                if (DateTime.Now <= blockedUntil)
                    return true;
            }

            _cache.Remove(key);
            _cache.Remove(key + "_blocked_until");
            return false;
        }
        return false;
    }

    public void RecordFailedAttempt(string ipAddress)
    {
        var key = Prefix + ipAddress;
        var attempts = _cache.Get<int>(key);
        attempts++;
        _cache.Set(key, attempts, TimeSpan.FromMinutes(BlockMinutes));

        if (attempts >= MaxAttempts)
        {
            _cache.Set(key + "_blocked_until", DateTime.Now.AddMinutes(BlockMinutes), TimeSpan.FromMinutes(BlockMinutes));
        }
    }

    public void Reset(string ipAddress)
    {
        var key = Prefix + ipAddress;
        _cache.Remove(key);
        _cache.Remove(key + "_blocked_until");
    }
}
