namespace BlogApp.Core.Security.Models;

public class EncryptionResult
{
    public string Data { get; init; } = null!;
    public string Key { get; private init; } = null!;

    public static EncryptionResult Create(byte[] data, byte[] iv, string key)
    {
        var combined = new byte[iv.Length + data.Length];
        Array.Copy(iv, 0, combined, 0, iv.Length);
        Array.Copy(data, 0, combined, iv.Length, data.Length);

        return new EncryptionResult
        {
            Data = Convert.ToBase64String(combined),
            Key = key
        };
    }

    public (byte[] iv, byte[] data) GetIVAndData()
    {
        const int length = 16;
        var combined = Convert.FromBase64String(Data);

        var iv = new byte[length];
        var encryptedData = new byte[combined.Length - length];

        Array.Copy(combined, 0, iv, 0, length);
        Array.Copy(combined, length, encryptedData, 0, encryptedData.Length);

        return (iv, encryptedData);
    }
}