using System.Security.Cryptography;
using GoodHamburger.Application.Auth;

namespace GoodHamburger.Infrastructure.Auth;

/// <summary>
/// PBKDF2-SHA256 hashing with a per-password random salt. Uses only the BCL,
/// and stores the iteration count with the hash so it can be raised later
/// without invalidating existing passwords.
/// Format: {iterations}.{salt-base64}.{hash-base64}
/// </summary>
public class Pbkdf2PasswordHasher : IPasswordHasher {

    private const int Iterations = 100_000;
    private const int SaltSize = 16;
    private const int KeySize = 32;

    public string Hash(string password) {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public bool Verify(string password, string passwordHash) {
        var parts = passwordHash.Split('.', 3);
        if (parts.Length != 3) return false;

        if (!int.TryParse(parts[0], out var iterations)) return false;

        byte[] salt, expectedKey;
        try {
            salt = Convert.FromBase64String(parts[1]);
            expectedKey = Convert.FromBase64String(parts[2]);
        } catch (FormatException) {
            return false;
        }

        var actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expectedKey.Length);
        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
    }
}
