using System.ComponentModel.DataAnnotations;

namespace PersonalDigitalVault.DTOs
{
    public class RenameDocumentDto
    {
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;
    }
}