using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EventManagement_Application.Data;
using EventManagement_Application.Models;
using System.Linq;

namespace EventManagement_Application.Pages.Organizer.Events
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Event Event { get; set; }

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event = _context.Events.FirstOrDefault(e => e.Id == id);

            if (Event == null)
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event = _context.Events.Find(id);

            if (Event != null)
            {
                _context.Events.Remove(Event);
                _context.SaveChanges();
            }

            return RedirectToPage("./Index");
        }
    }
}