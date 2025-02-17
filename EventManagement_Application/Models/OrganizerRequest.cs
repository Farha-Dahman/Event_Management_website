using System.ComponentModel.DataAnnotations;

namespace EventManagement_Application.Models
{
    public class OrganizerRequest
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        [Required]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    }
}
