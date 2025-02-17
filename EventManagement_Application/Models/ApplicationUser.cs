using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EventManagement_Application.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public byte[]? ProfilePicture { get; set; }

        public bool OrganizerRequestPending { get; set; } = false;

        public ICollection<TicketUser>? TicketUsers { get; set; }
    }
}
