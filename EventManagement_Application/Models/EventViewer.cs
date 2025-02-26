using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventManagement_Application.Models
{
    public class EventViewer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Event")]
        public int EventId { get; set; }
        public Event Event { get; set; }  // Navigation property

        [Required]
        [ForeignKey("Viewer")]
        public string ViewerId { get; set; }
        public ApplicationUser Viewer { get; set; }  // Navigation property to Identity User

        [Required]
        public DateTime ScanDate { get; set; } = DateTime.UtcNow;
    }
}
