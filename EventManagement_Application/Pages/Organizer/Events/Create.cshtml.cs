using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EventManagement_Application.Pages.Organizer.Events
{
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
        public Event Event { get; set; }

        public IActionResult OnPost()
        {
            // Check if the user is authenticated
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                _logger.LogError("User is not authenticated.");
                ModelState.AddModelError(string.Empty, "You must be logged in to create an event.");
                return Page();
            }

            // Get User ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("User ID is null or empty.");
                ModelState.AddModelError(string.Empty, "Unable to identify the user. Please log in again.");
                return Page();
            }

            Event.OrganizerId = userId;

            _logger.LogInformation("Creating event with Title: {Title}, Date: {Date}, OrganizerId: {OrganizerId}",
                Event.Title, Event.Date, Event.OrganizerId);

            try
            {
                _context.Events.Add(Event);
                _context.SaveChanges();
                _logger.LogInformation("Event created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving event to the database.");
                ModelState.AddModelError(string.Empty, "An error occurred while saving the event.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
