using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Magidesk.Infrastructure.Security;

public class AesEncryptionService : IAesEncryptionService
{
    private readonly byte[] _key;

    public AesEncryptionService(IConfiguration configuration)
    {
        var keyString = configuration["Security:EncryptionKey"];
        
        // Fallback for development if not configured
        if (string.IsNullOrEmpty(keyString))
        {
            keyString = "MagideskDevKey_MustChangeInProd!"; // 32 characters for AES-256
        }

        // Ensure key is 32 bytes (256 bits)
        using var sha256 = SHA256.Create();
        _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(keyString));
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return plainText;

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV(); // Generate a new IV for each encryption

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var msEncrypt = new MemoryStream();
        // Write IV first
        msEncrypt.Write(aes.IV, 0, aes.IV.Length);

        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        var cipherBytes = msEncrypt.ToArray();
        return Convert.ToBase64String(cipherBytes);
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return cipherText;

        var fullCipher = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = _key;

        // Read IV from the beginning
        var iv = new byte[aes.BlockSize / 8];
        if (fullCipher.Length < iv.Length) throw new ArgumentException("Invalid cipher text");
        
        Array.Copy(fullCipher, 0, iv, 0, iv.Length);
        aes.IV = iv;

        // Check if there's actual content after IV
        if (fullCipher.Length == iv.Length) return string.Empty;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var msDecrypt = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        return srDecrypt.ReadToEnd();
    }
}
