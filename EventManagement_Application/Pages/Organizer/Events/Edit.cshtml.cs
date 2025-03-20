using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EventManagement_Application.Pages.Organizer.Events
{
    [Authorize(Roles = "Organizer")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ApplicationDbContext context, ILogger<EditModel> logger)
        {
            _context = context;
            _logger = logger;
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

        public IActionResult OnPost()
        {
            _logger.LogInformation("OnPost method called for event update.");

            //if (!ModelState.IsValid)
            //{
            //    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            //    {
            //        _logger.LogWarning("ModelState Error: {ErrorMessage}", error.ErrorMessage);
            //    }
            //    return Page();
            //}

            var existingEvent = _context.Events.Find(Event.Id);

            if (existingEvent == null)
            {
                _logger.LogWarning("Event with ID {EventId} not found in the database.", Event?.Id);
                return NotFound();
            }

            _logger.LogInformation("Updating event: {EventId}, Title: {Title}", existingEvent.Id, existingEvent.Title);

            try
            {
                // Update fields
                existingEvent.Title = Event.Title;
                existingEvent.Description = Event.Description;
                existingEvent.CreationDate = Event.CreationDate;
                existingEvent.EventImage = Event.EventImage;
                existingEvent.Location = Event.Location;
                existingEvent.Category = Event.Category;
                existingEvent.Mode = Event.Mode;

                _context.SaveChanges();

                _logger.LogInformation("Event with ID {EventId} updated successfully.", existingEvent.Id);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating event with ID {EventId}.", existingEvent.Id);
                ModelState.AddModelError("", "An error occurred while updating the event.");
                return Page();
            }
        }


    }
}
