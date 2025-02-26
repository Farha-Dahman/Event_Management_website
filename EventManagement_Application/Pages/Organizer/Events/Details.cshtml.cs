using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EventManagement_Application.Pages.Organizer.Events
{
    [Authorize(Roles = "Organizer")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Event Event { get; set; }
        public ICollection<Ticket> Tickets { get; set; }

        //public async Task<IActionResult> OnGet(int? id)
        //{
        //    // Fetch the event along with its associated tickets
        //    Event = await _context.Events
        //        .Include(e => e.Tickets)
        //        .FirstOrDefaultAsync(e => e.Id == id);

        //    if (Event == null)
        //    {
        //        return NotFound();
        //    }

        //    // Tickets will be automatically populated with the related tickets
        //    Tickets = Event.Tickets;  // You can explicitly assign tickets if needed

        //    return Page();
        //}
        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event = _context.Events.Include(e => e.Tickets).FirstOrDefault(e => e.Id == id);

            if (Event == null)
            {
                return NotFound();
            }

            Tickets = Event.Tickets.ToList();

            return Page();
        }
        public IActionResult OnPostDelete(int id)
        {
            var ticket = _context.Tickets.Find(id);

            if (ticket == null)
            {
                return NotFound();
            }

            _context.Tickets.Remove(ticket);
            _context.SaveChanges();

            return RedirectToPage("Details", new { id = ticket.EventId });
        }
    }
}
