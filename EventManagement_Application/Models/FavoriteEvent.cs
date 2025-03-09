using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventManagement_Application.Models
{
    public class FavoriteEvent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // The user who favorited the event

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public Event Event { get; set; }
    }
}
