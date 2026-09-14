using System.Security.Cryptography;
using PersonalDigitalVault.Interfaces;

namespace PersonalDigitalVault.Services
{
    public class FileHashService : IFileHashService
    {
        public async Task<string> GenerateSha256Async(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                using var sha256 = SHA256.Create();

                var hashBytes =
                    await sha256.ComputeHashAsync(stream);

                return Convert.ToHexString(hashBytes);
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}