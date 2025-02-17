using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventManagement_Application.Pages.Organizer.Tickets
{
    [Authorize(Roles = "Organizer")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ApplicationDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = new Ticket();

        [BindProperty]
        public Event Event { get; set; }

        public int EventId { get; set; }

        public async Task<IActionResult> OnGetAsync(int eventId)
        {
            if (eventId == 0)
            {
                _logger.LogError("eventId is 0 or missing.");
                return NotFound("Invalid event ID.");
            }

            // Load the event details using the eventId
            Event = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);

            if (Event == null)
            {
                _logger.LogError("Event not found for ID: {EventId}", eventId);
                return NotFound("Event not found.");
            }

            // Set the EventId property for use in the OnPostAsync method
            EventId = eventId;

            // Initialize the Ticket with default values (Free and 0 price)
            Ticket = new Ticket
            {
                EventId = eventId,
                Price = 0,
                TicketType = "Free",
                Quantity = 1,
                AvailableTickets = 1
            };

            _logger.LogInformation("Loading Create Ticket page for Event ID: {EventId}", eventId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Adding ticket to the database: Price: {TicketPrice}, Type: {TicketType}, Quantity: {TicketQuantity}, EventId: {EventId}",
                Ticket.Price, Ticket.TicketType, Ticket.Quantity, Ticket.EventId);

            // Set the ticket type based on the price value (if price is 0, it's Free)
            if (Ticket.Price == 0)
            {
                Ticket.TicketType = "Free";
            }

            _logger.LogInformation("Adding ticket to the database: Price: {TicketPrice}, Type: {TicketType}, Quantity: {TicketQuantity}, EventId: {EventId}",
                Ticket.Price, Ticket.TicketType, Ticket.Quantity, Ticket.EventId);

            // Add the ticket to the database
            _context.Tickets.Add(Ticket);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Reset the Ticket form after saving the ticket
            Ticket = new Ticket
            {
                EventId = EventId,
                Price = 0,
                TicketType = "Free",
                Quantity = 1,
                AvailableTickets = 1
            };

            _logger.LogInformation("New ticket created for Event ID: {EventId}", EventId);
            return Page();
        }
    }
}
