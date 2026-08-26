using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PersonalDigitalVault.DTOs
{
    public class UploadDocumentDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int FolderId { get; set; }
    }
}