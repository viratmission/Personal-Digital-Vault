using System.ComponentModel.DataAnnotations;
namespace PersonalDigitalVault.Models
{
    public class Folder
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; } = null !;
    }
}
