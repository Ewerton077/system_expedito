using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace SantoExpedito.Data;

public interface ICpfEncryptionService
{
    string Encrypt(string cpf);
    string Decrypt(string encryptedCpf);
}

public class CpfEncryptionService : ICpfEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public CpfEncryptionService(IConfiguration config)
    {
        var keyBase64 = config["CpfEncryption:Key"]
            ?? Environment.GetEnvironmentVariable("CPF_ENCRYPTION_KEY")
            ?? throw new InvalidOperationException("CpfEncryption:Key não configurada");

        var ivBase64 = config["CpfEncryption:IV"]
            ?? Environment.GetEnvironmentVariable("CPF_ENCRYPTION_IV")
            ?? throw new InvalidOperationException("CpfEncryption:IV não configurada");

        _key = Convert.FromBase64String(keyBase64);
        _iv = Convert.FromBase64String(ivBase64);
    }

    public string Encrypt(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return cpf;

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var bytes = Encoding.UTF8.GetBytes(cpf);
        var encrypted = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
        return Convert.ToBase64String(encrypted);
    }

    public string Decrypt(string encryptedCpf)
    {
        if (string.IsNullOrWhiteSpace(encryptedCpf))
            return encryptedCpf;

        try
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var bytes = Convert.FromBase64String(encryptedCpf);
            var decrypted = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
        catch
        {
            return encryptedCpf;
        }
    }
}
