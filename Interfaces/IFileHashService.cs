namespace PersonalDigitalVault.Interfaces
{
    public interface IFileHashService
    {
        Task<string> GenerateSha256Async(Stream stream);
    }
}