using System.ComponentModel.DataAnnotations;

namespace PersonalDigitalVault.DTOs
{
    public class CreateFolderDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
