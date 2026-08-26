using System.ComponentModel.DataAnnotations;
namespace PersonalDigitalVault.DTOs
{
    public class RenameFolderDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
