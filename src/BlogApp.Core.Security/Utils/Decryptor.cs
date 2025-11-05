using System.Security.Cryptography;
using BlogApp.Core.Security.Constants;
using BlogApp.Core.Security.Models;

namespace BlogApp.Core.Security.Utils;

public class Decryptor
{
    public static async Task<string> DecryptAsync(EncryptionResult result)
    {
        var key = Convert.FromBase64String(result.Key);
        var (iv, encryptedData) = result.GetIVAndData();

        using var aes = Aes.Create();
        aes.KeySize = Encryption.KeySize;
        aes.BlockSize = Encryption.BlockSize;
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        using var memoryStreamDecrypt = new MemoryStream(encryptedData);
        await using var cryptoStreamDecrypt = new CryptoStream(memoryStreamDecrypt, decryptor, CryptoStreamMode.Read);
        using var streamReaderDecrypt = new StreamReader(cryptoStreamDecrypt);

        try
        {
            return await streamReaderDecrypt.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            throw new CryptographicException("Decryption Failed", ex);
        }
    }
}