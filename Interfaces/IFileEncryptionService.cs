namespace PersonalDigitalVault.Interfaces
{
    public interface IFileEncryptionService
    {
        Task EncryptAsync(
            Stream inputStream,
            Stream outputStream);

        Task DecryptAsync(
            Stream inputStream,
            Stream outputStream);
    }
}