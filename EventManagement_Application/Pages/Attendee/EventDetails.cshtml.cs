using Microsoft.EntityFrameworkCore;
using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EventManagement_Application.Pages.Attendee
{
    public class EventDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventDetailsModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventDetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<EventDetailsModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public string UserId { get; set; }

        public Event Event { get; set; }
        public ICollection<Ticket> Tickets { get; set; }

        public async Task<IActionResult> OnGetAsync(int eventId)
        {
            ViewData["NoContainer"] = true;
            Event = await _context.Events
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (Event == null)
            {
                return NotFound();
            }

            Tickets = Event.Tickets;

            return Page();
        }

        //public async Task<IActionResult> OnPostReserveTicketAsync(int TicketId, int TicketCount)
        public async Task<IActionResult> OnPostReserveTicketAsync([FromForm] int TicketId, [FromForm] int TicketCount)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == TicketId);

            if (ticket == null)
            {
                return NotFound();
            }

            // Check if the current date is within the open and close date range
            if (DateTime.Now < ticket.OpenDate)
            {
                ModelState.AddModelError(string.Empty, "The ticket is not available yet.");
                return Page();
            }

            if (DateTime.Now > ticket.CloseDate)
            {
                ModelState.AddModelError(string.Empty, "The ticket sales are closed.");
                return Page();
            }


            // Ensure the ticket count is valid (greater than 0)
            if (TicketCount <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please select a valid quantity.");
                return Page();
            }

            // If there aren't enough tickets available, show error
            if (ticket.AvailableTickets < TicketCount)
            {
                ModelState.AddModelError(string.Empty, "Not enough tickets available.");
                return Page();
            }

            _logger.LogInformation("TicketCount: {TicketCount}", TicketCount);

            // Reserve the tickets for the user and update available ticket count
            int reservedCount = await ReserveTicketAsync(ticket, TicketCount);

            // Deduct the reserved tickets from the available ticket count
            _logger.LogInformation("AvailableTickets: {AvailableTickets}, reservedCount: {reservedCount}",
                ticket.AvailableTickets, reservedCount);
            ticket.AvailableTickets -= reservedCount;
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            return RedirectToPage("EventDetails", new { eventId = ticket.EventId });
        }

        private async Task<int> ReserveTicketAsync(Ticket ticket, int ticketCount)
        {
            string userId = _userManager.GetUserId(User); // Get actual user ID

            // Check if the user already has a reservation for this ticket
            var existingReservation = await _context.TicketUsers
                .FirstOrDefaultAsync(tu => tu.TicketId == ticket.Id && tu.UserId == userId);

            if (existingReservation != null)
            {
                // Update existing reservation by incrementing the quantity by the selected ticket count
                _logger.LogInformation("Quantity: {Quantity}, ticketCount: {ticketCount}",
                existingReservation.Quantity, ticketCount);
                existingReservation.Quantity += ticketCount;
                _context.TicketUsers.Update(existingReservation);
            }
            else
            {
                // Create a new reservation if no existing record
                var ticketUser = new TicketUser
                {
                    TicketId = ticket.Id,
                    UserId = userId, // UserId from UserManager
                    Quantity = ticketCount,
                    RegistrationDate = DateTime.Now
                };
                _context.TicketUsers.Add(ticketUser);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the actual number of tickets reserved (the full ticketCount)
            return ticketCount;
        }

    }
}
