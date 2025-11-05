using System.Buffers.Binary;
using System.Security.Cryptography;

namespace BlogApp.Core.Security.Utils;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int SubkeyLength = 32;
    private const int IterationCount = 100_000;
    private const uint Prf = 2u;

    private static readonly RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();

    public static string HashPassword(string password)
    {
        Span<byte> salt = stackalloc byte[SaltSize];
        _randomNumberGenerator.GetBytes(salt);

        var subkey = Rfc2898DeriveBytes.Pbkdf2(password.AsSpan(), salt, IterationCount, HashAlgorithmName.SHA512,
            SubkeyLength);

        const int length = 12;
        var totalLength = length + salt.Length + subkey.Length;
        var outputBytes = new byte[totalLength];

        BinaryPrimitives.WriteUInt32BigEndian(outputBytes.AsSpan(0), Prf);
        BinaryPrimitives.WriteUInt32BigEndian(outputBytes.AsSpan(4), IterationCount);
        BinaryPrimitives.WriteUInt32BigEndian(outputBytes.AsSpan(8), (uint)salt.Length);

        salt.CopyTo(outputBytes.AsSpan(length));
        subkey.AsSpan().CopyTo(outputBytes.AsSpan(length + salt.Length));

        return Convert.ToBase64String(outputBytes);
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var hashedPasswordAsArray = Convert.FromBase64String(hashedPassword);

        var prf = BinaryPrimitives.ReadUInt32BigEndian(hashedPasswordAsArray.AsSpan(0));
        var iterationCount = (int)BinaryPrimitives.ReadUInt32BigEndian(hashedPasswordAsArray.AsSpan(4));
        var saltLength = (int)BinaryPrimitives.ReadUInt32BigEndian(hashedPasswordAsArray.AsSpan(8));

        if (prf != Prf && (saltLength <= 0 || (12 + saltLength) > hashedPasswordAsArray.Length))
            return false;

        var salt = hashedPasswordAsArray.AsSpan(12, saltLength);
        var expectedSubkey = hashedPasswordAsArray.AsSpan(12 + saltLength);

        var actualSubkey = Rfc2898DeriveBytes.Pbkdf2(password.AsSpan(), salt, iterationCount, HashAlgorithmName.SHA512,
            expectedSubkey.Length);

        return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
    }
}