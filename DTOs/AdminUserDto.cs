namespace PersonalDigitalVault.DTOs
{
    public class AdminUserDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }
    }
}