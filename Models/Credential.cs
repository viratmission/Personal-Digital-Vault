namespace PersonalDigitalVault.Models
{
    public class Credential
    {
        public int Id { get; set; }

        public string EncryptedValue { get; set; } = string.Empty;

        public int UserId { get; set; }

        public User? User { get; set; }
    }
}