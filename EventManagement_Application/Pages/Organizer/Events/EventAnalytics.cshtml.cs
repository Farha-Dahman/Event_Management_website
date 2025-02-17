using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EventManagement_Application.Pages.Organizer.Events
{
    [Authorize(Roles = "Organizer")]
    public class EventAnalyticsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EventAnalyticsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Event Event { get; set; }
        public int TotalTicketsSold { get; set; }
        public int TotalTicketsAvailable { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AttendanceRate { get; set; }

        public async Task<IActionResult> OnGet(int eventId)
        {
            Event = await _context.Events
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (Event == null)
            {
                return NotFound();
            }

            // Calculate ticket sales and revenue
            TotalTicketsSold = Event.Tickets.Sum(t => t.Quantity - t.AvailableTickets);
            TotalTicketsAvailable = Event.Tickets.Sum(t => t.Quantity);
            TotalRevenue = Event.Tickets.Sum(t => (t.Quantity - t.AvailableTickets) * t.Price);

            // Calculate attendance rate
            AttendanceRate = TotalTicketsAvailable > 0
                ? (double)TotalTicketsSold / TotalTicketsAvailable * 100
                : 0;

            return Page();
        }
    }
}
