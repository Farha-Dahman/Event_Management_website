using EventManagement_Application.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement_Application.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required]
        [StringLength(300, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Event Date & Time")]
        public DateTime EventDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Event Creation Date")]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public byte[]? EventImage { get; set; } // Store image path or URL

        [Required]
        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters.")]
        public string Category { get; set; }

        [Required]
        [EnumDataType(typeof(EventMode))]
        [Display(Name = "Mode of Event")]
        public EventMode Mode { get; set; }

        [Required]
        public string OrganizerId { get; set; }

        [ForeignKey("OrganizerId")]
        public ApplicationUser Organizer { get; set; }

        public ICollection<Ticket>? Tickets { get; set; } = new List<Ticket>();
    }
}
