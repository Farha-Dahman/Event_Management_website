using EventManagement_Application.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement_Application.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public Event Event { get; set; }

        [Required]
        [EnumDataType(typeof(TicketType))]
        [Column(TypeName = "nvarchar(20)")]
        [Display(Name = "Ticket of Type")]
        public TicketType TicketType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Available tickets must be 0 or more.")]
        public int AvailableTickets { get; set; }

        public ICollection<TicketUser>? TicketUsers { get; set; } = new List<TicketUser>();
    }
}
