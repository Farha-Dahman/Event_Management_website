using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EventManagement_Application.Pages.Organizer.Tickets
{
    [Authorize(Roles = "Organizer")]
    public class EditTicketModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditTicketModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; }

        public async Task<IActionResult> OnGetAsync(int ticketId)
        {
            Ticket = await _context.Tickets.Include(t => t.Event).FirstOrDefaultAsync(t => t.Id == ticketId);

            if (Ticket == null)
            {
                return NotFound("Ticket not found.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _context.Attach(Ticket).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(Ticket.Id))
                {
                    return NotFound("Ticket no longer exists.");
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("../Events/Details", new { id = Ticket.EventId });
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(t => t.Id == id);
        }
    }
}
