using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

var keyString = "MagideskDevKey_MustChangeInProd!";
using var sha256 = SHA256.Create();
var key = sha256.ComputeHash(Encoding.UTF8.GetBytes(keyString));

string Encrypt(string plainText)
{
    if (string.IsNullOrEmpty(plainText)) return plainText;
    using var aes = Aes.Create();
    aes.Key = key;
    var iv = new byte[16];
    Array.Copy(key, 0, iv, 0, 16);
    aes.IV = iv;
    var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
    using var msEncrypt = new MemoryStream();
    msEncrypt.Write(aes.IV, 0, aes.IV.Length);
    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
    using (var swEncrypt = new StreamWriter(csEncrypt))
    {
        swEncrypt.Write(plainText);
    }
    return Convert.ToBase64String(msEncrypt.ToArray());
}

Console.WriteLine($"1111: {Encrypt("1111")}");
Console.WriteLine($"2222: {Encrypt("2222")}");
Console.WriteLine($"3333: {Encrypt("3333")}");
Console.WriteLine($"4444: {Encrypt("4444")}");
Console.WriteLine($"5555: {Encrypt("5555")}");
Console.WriteLine($"1234: {Encrypt("1234")}");
