using System.Security.Cryptography;
using PersonalDigitalVault.Interfaces;

namespace PersonalDigitalVault.Services
{
    public class FileEncryptionService : IFileEncryptionService
    {
        private readonly byte[] _key;

        public FileEncryptionService(
            IConfiguration configuration)
        {
            var key =
                configuration["Encryption:AesKey"];

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException(
                    "AES encryption key is not configured.");
            }

            _key = Convert.FromBase64String(key);

            if (_key.Length != 32)
            {
                throw new InvalidOperationException(
                    "AES key must be 32 bytes for AES-256.");
            }
        }

        public async Task EncryptAsync(
            Stream inputStream,
            Stream outputStream)
        {
            if (inputStream.CanSeek)
            {
                inputStream.Position = 0;
            }

            using var aes = Aes.Create();

            aes.Key = _key;
            aes.GenerateIV();

            await outputStream.WriteAsync(
                aes.IV,
                0,
                aes.IV.Length);

            using var encryptor =
                aes.CreateEncryptor(
                    aes.Key,
                    aes.IV);

            using var cryptoStream =
                new CryptoStream(
                    outputStream,
                    encryptor,
                    CryptoStreamMode.Write,
                    leaveOpen: true);

            await inputStream.CopyToAsync(
                cryptoStream);

            await cryptoStream.FlushFinalBlockAsync();
        }

        public async Task DecryptAsync(
            Stream inputStream,
            Stream outputStream)
        {
            if (inputStream.CanSeek)
            {
                inputStream.Position = 0;
            }

            using var aes = Aes.Create();

            aes.Key = _key;

            var iv =
                new byte[aes.BlockSize / 8];

            var bytesRead =
                await inputStream.ReadAsync(
                    iv,
                    0,
                    iv.Length);

            if (bytesRead != iv.Length)
            {
                throw new CryptographicException(
                    "Invalid encrypted file.");
            }

            aes.IV = iv;

            using var decryptor =
                aes.CreateDecryptor(
                    aes.Key,
                    aes.IV);

            using var cryptoStream =
                new CryptoStream(
                    inputStream,
                    decryptor,
                    CryptoStreamMode.Read,
                    leaveOpen: true);

            await cryptoStream.CopyToAsync(
                outputStream);
        }
    }
}