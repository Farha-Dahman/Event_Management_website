using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EventManagement_Application.Pages.Attendee
{
    public class ViewEventsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ViewEventsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Event> Events { get; set; }

        // Method to load the event details based on its ID
        public async Task<IActionResult> OnGetAsync()
        {
            Events = await _context.Events.ToListAsync();
            return Page();
        }

    }
}

