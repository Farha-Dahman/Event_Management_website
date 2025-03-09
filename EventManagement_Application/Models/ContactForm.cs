using System.ComponentModel.DataAnnotations;

namespace EventManagement_Application.Models
{
    public class ContactForm
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public string UserEmail { get; set; }

        public string UserId { get; set; }

        public DateTime SubmittedAt { get; set; }
    }
}
