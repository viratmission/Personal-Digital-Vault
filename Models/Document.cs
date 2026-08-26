using System.ComponentModel.DataAnnotations;

namespace PersonalDigitalVault.Models
{
    public class Document
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string StoredFileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public int FolderId { get; set; }

        public Folder Folder { get; set; } = null!;
    }
}