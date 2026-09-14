namespace PersonalDigitalVault.Interfaces
{
    public interface IAesEncryptionService
    {
        string Encrypt(string plainText);

        string Decrypt(string encryptedText);
    }
}