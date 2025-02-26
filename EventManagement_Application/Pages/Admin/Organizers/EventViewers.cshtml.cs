using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EventManagement_Application.Pages.Admin.Organizers
{
    [Authorize(Roles = "Admin")]
    public class EventViewersModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventViewersModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Event Event { get; set; }
        public List<EventViewer> Viewers { get; set; }

        public async Task<IActionResult> OnGetAsync(int eventId)
        {
            // Fetch the event details
            Event = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (Event == null)
            {
                return NotFound(); // Or redirect to an error page
            }

            // Fetch the list of viewers who viewed this event
            Viewers = await _context.EventViewers
                .Where(ev => ev.EventId == eventId)
                .ToListAsync() ?? new List<EventViewer>(); // Ensure Viewers is not null

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteViewerAsync(int viewerId)
        {
            var viewer = await _context.EventViewers
                .FirstOrDefaultAsync(v => v.Id == viewerId);

            if (viewer != null)
            {
                _context.EventViewers.Remove(viewer);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
