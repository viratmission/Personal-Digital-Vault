using System.Security.Cryptography;
using System.Text;
using PersonalDigitalVault.Interfaces;

namespace PersonalDigitalVault.Services
{
    public class AesEncryptionService : IAesEncryptionService
    {
        private readonly byte[] _key;

        public AesEncryptionService(IConfiguration configuration)
        {
            var key = configuration["Encryption:AesKey"];

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

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            using var aes = Aes.Create();

            aes.Key = _key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor(
                aes.Key,
                aes.IV);

            var plainBytes = Encoding.UTF8.GetBytes(plainText);

            var encryptedBytes = encryptor.TransformFinalBlock(
                plainBytes,
                0,
                plainBytes.Length);

            var combinedBytes =
                new byte[aes.IV.Length + encryptedBytes.Length];

            Buffer.BlockCopy(
                aes.IV,
                0,
                combinedBytes,
                0,
                aes.IV.Length);

            Buffer.BlockCopy(
                encryptedBytes,
                0,
                combinedBytes,
                aes.IV.Length,
                encryptedBytes.Length);

            return Convert.ToBase64String(combinedBytes);
        }

        public string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                return encryptedText;
            }

            var combinedBytes =
                Convert.FromBase64String(encryptedText);

            using var aes = Aes.Create();

            aes.Key = _key;

            var ivLength = aes.BlockSize / 8;

            if (combinedBytes.Length <= ivLength)
            {
                throw new CryptographicException(
                    "Invalid encrypted value.");
            }

            var iv = new byte[ivLength];

            var cipherBytes =
                new byte[combinedBytes.Length - ivLength];

            Buffer.BlockCopy(
                combinedBytes,
                0,
                iv,
                0,
                ivLength);

            Buffer.BlockCopy(
                combinedBytes,
                ivLength,
                cipherBytes,
                0,
                cipherBytes.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(
                aes.Key,
                aes.IV);

            var decryptedBytes = decryptor.TransformFinalBlock(
                cipherBytes,
                0,
                cipherBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}