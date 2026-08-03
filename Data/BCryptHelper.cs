using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace SantoExpedito.Data;

public static class BCryptHelper
{
    private const int WorkFactor = 12;

    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(storedHash))
            return false;

        if (storedHash.Length == 44 && !storedHash.Contains("$"))
        {
            return VerifyLegacySha256(password, storedHash);
        }

        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }

    private static bool VerifyLegacySha256(string password, string storedHash)
    {
        var sha256 = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        var currentHash = Convert.ToBase64String(sha256);
        return currentHash == storedHash;
    }
}
