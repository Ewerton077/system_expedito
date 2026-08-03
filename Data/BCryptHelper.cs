using System.Security.Cryptography;
using System.Text;

namespace SantoExpedito.Data;

public static class BCryptHelper
{ 
    public static string HashPassword(string password)
    {
        return Convert.ToBase64String(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(password)
            )
        );
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        return HashPassword(password) == storedHash;
    }
}
