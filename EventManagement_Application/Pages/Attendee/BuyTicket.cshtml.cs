using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EventManagement_Application.Pages.Attendee
{
    public class BuyTicketModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public BuyTicketModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int TicketId { get; set; }

        public Ticket Ticket { get; set; }
        public Event Event { get; set; }

        public async Task<IActionResult> OnGet(int ticketId)
        {
            // Fetch the ticket and the associated event
            Ticket = await _context.Tickets
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (Ticket == null || Ticket.AvailableTickets <= 0)
            {
                return NotFound();  // Or show "Out of Stock"
            }

            Event = Ticket.Event;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Find the ticket by Id
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == TicketId);

            if (ticket == null || ticket.AvailableTickets <= 0)
            {
                return NotFound();  // Or show "Out of Stock"
            }

            // Decrease available tickets
            ticket.AvailableTickets--;

            // Save changes to the database
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            // Optionally, you can redirect to a confirmation page or show a message
            return RedirectToPage("/Attendee/EventDetails", new { id = ticket.EventId });
        }
    }
}
