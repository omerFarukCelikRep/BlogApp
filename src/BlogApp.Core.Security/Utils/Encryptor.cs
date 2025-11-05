using System.Security.Cryptography;
using BlogApp.Core.Security.Constants;
using BlogApp.Core.Security.Models;

namespace BlogApp.Core.Security.Utils;

public class Encryptor
{
    public static async Task<EncryptionResult> EncryptAsync(string plainText)
    {
        using var aes = Aes.Create();
        aes.KeySize = Encryption.KeySize;
        aes.BlockSize = Encryption.BlockSize;

        aes.GenerateIV();
        aes.GenerateKey();

        using var encryptor = aes.CreateEncryptor();
        using var memoryStreamEncrypt = new MemoryStream();

        await using var cryptoStream = new CryptoStream(memoryStreamEncrypt, encryptor, CryptoStreamMode.Write);
        await using var streamWriterEncrypt = new StreamWriter(cryptoStream);
        await streamWriterEncrypt.WriteAsync(plainText);

        var encrypted = memoryStreamEncrypt.ToArray();

        return EncryptionResult.Create(encrypted, aes.IV, Convert.ToBase64String(aes.Key));
    }
}