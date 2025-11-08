
namespace Core.Utilities;

public interface IEncryptionService
{
    public (byte[] Key, byte[] IV , byte[] EncryptedData) Encrypt(string data);
    public string Decrypt(byte[] data , byte[] Key, byte[] IV);

    public byte[] EncryptKey(byte[] key, byte[] masterKey , byte[] IV);
    public byte[] DecryptKey(byte[] encryptKey, byte[] masterKey , byte[] IV); // i will use the same iv just for learn
}

public class EncryptionService
{

}
