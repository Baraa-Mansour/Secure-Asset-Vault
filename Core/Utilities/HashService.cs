using System.Security.Cryptography;

namespace Core.Utilities;

public interface IHashService
{
    public (byte[] Hash, byte[] Salt) Hash(string password);
    public bool Verify(string password, byte[] storedHash, byte[] storedSalt);
}

public class HashService : IHashService
{
    private const int _saltSize = 16;
    private const int _hashSize = 32;
    private const int _iterations = 100000;

    public (byte[] Hash , byte[] Salt) Hash(string password)
    {
        byte[] Salt = new byte[_saltSize];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(Salt);
        }

        byte[] Hash = _HashPasswordWithSalt(password , Salt);
        
        return (Hash , Salt);
    }

    public bool Verify(string password, byte[] storedHash, byte[] storedSalt)
    {
        byte[] computedHash = _HashPasswordWithSalt(password, storedSalt);

        // CRITICAL: Use constant-time comparison to prevent timing attacks
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }


    private byte[] _HashPasswordWithSalt(string password , byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                _iterations,
                HashAlgorithmName.SHA256
        );

        return pbkdf2.GetBytes(_hashSize);
    }
}
