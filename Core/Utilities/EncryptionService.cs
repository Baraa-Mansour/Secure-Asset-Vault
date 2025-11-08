
using System.Security.Cryptography;
using System.Text;

namespace Core.Utilities;

public interface IEncryptionService
{
    public (byte[] Key, byte[] IV , byte[] EncryptedData) Encrypt(string data);
    public string Decrypt(byte[] data , byte[] Key, byte[] IV);

    public byte[] EncryptKey(byte[] key, byte[] masterKey , byte[] IV);
    public byte[] DecryptKey(byte[] encryptKey, byte[] masterKey , byte[] IV); // i will use the same iv just for learn
}

public class EncryptionService : IEncryptionService
{
    public (byte[] Key, byte[] IV, byte[] EncryptedData) Encrypt(string data)
    {
        using (Aes aes = Aes.Create())
        {  
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(data);
                cs.Write(plainBytes, 0, plainBytes.Length);
                cs.FlushFinalBlock();

                return (aes.Key, aes.IV, ms.ToArray());
            }
        }
    }

    public string Decrypt(byte[] data, byte[] Key, byte[] IV)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.IV = IV;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(data))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }

    public byte[] EncryptKey(byte[] key, byte[] masterKey, byte[] IV)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = masterKey;
            aes.IV = IV;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(key, 0, key.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
    }

    public byte[] DecryptKey(byte[] encryptKey, byte[] masterKey, byte[] IV)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = masterKey;
            aes.IV = IV;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(encryptKey))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (MemoryStream output = new MemoryStream())
            {
                cs.CopyTo(output);
                return output.ToArray();
            }
        }
    }
}
