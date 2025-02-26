using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EventManagement_Application.Pages.Admin.Organizers
{
    [Authorize(Roles = "Admin")]
    public class EventsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EventsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string OrganizerName { get; set; }
        public List<Event> Events { get; set; }

        public async Task<IActionResult> OnGetAsync(string organizerId)
        {
            // Fetch the organizer's name
            var organizer = await _context.Users.FindAsync(organizerId);
            if (organizer == null)
            {
                return NotFound();
            }
            OrganizerName = $"{organizer.FirstName} {organizer.LastName}";

            // Fetch all events for this organizer
            Events = await _context.Events
                .Where(e => e.OrganizerId == organizerId)
                .ToListAsync();

            return Page();
        }
    }
}
