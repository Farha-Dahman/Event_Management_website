using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;

namespace EventManagement_Application.Pages.Organizer.Events
{
    [Authorize(Roles = "Organizer")]
    public class AttendeesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AttendeesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Event Event { get; set; }
        public List<TicketUser> Attendees { get; set; }

        public async Task<IActionResult> OnGet(int eventId)
        {
            Event = await _context.Events
                .Include(e => e.Tickets)
                .ThenInclude(t => t.TicketUsers)
                .ThenInclude(tu => tu.User)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (Event == null)
            {
                return NotFound();
            }

            Attendees = Event.Tickets
                .SelectMany(t => t.TicketUsers)
                .ToList();

            return Page();
        }

        //public async Task<IActionResult> OnPostRemoveAsync(int ticketId, string userId, int eventId)
        //{
        //    // Find the ticketUser based on TicketId and UserId
        //    var ticketUser = await _context.TicketUsers
        //        .FirstOrDefaultAsync(tu => tu.TicketId == ticketId && tu.UserId == userId);

        //    if (ticketUser != null)
        //    {
        //        // Remove the TicketUser from the context
        //        _context.TicketUsers.Remove(ticketUser);

        //        // Save changes asynchronously
        //        await _context.SaveChangesAsync();
        //    }

        //    // Redirect to the event page or the desired page with eventId
        //    return RedirectToPage(new { eventId });
        //}
    }

}
