using System.ComponentModel.DataAnnotations;

namespace EventManagement_Application.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        public string? UserAvatar { get; set; } // Optional, default if not provided

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}
