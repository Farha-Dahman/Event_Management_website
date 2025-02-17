using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EventManagement_Application.Pages.Organizer.Events
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
        public Event Event { get; set; }

        [BindProperty]
        public List<Ticket> Tickets { get; set; } = new();

        public async Task<IActionResult> OnPost()
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

            // Process the uploaded file (EventImage) if it exists
            if (Request.Form.Files.Count > 0)  // Check if there are files uploaded
            {
                var file = Request.Form.Files[0];  // Get the first file (assuming only one file is uploaded)
                if (file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream); // Copy the file to the memory stream
                        Event.EventImage = memoryStream.ToArray(); // Convert memory stream to byte array
                    }
                }
            }

            _logger.LogInformation("Creating event with Title: {Title}, Date: {Date}, OrganizerId: {OrganizerId}",
                Event.Title, Event.CreationDate, Event.OrganizerId);

            try
            {
                // Save Event
                _context.Events.Add(Event);
                await _context.SaveChangesAsync();

                // Ensure Event.Id is populated
                var eventId = Event.Id;

                _logger.LogInformation("Event created successfully with tickets.");
                return RedirectToPage("../Tickets/Create", new { eventId = eventId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving event to the database.");
                ModelState.AddModelError(string.Empty, "An error occurred while saving the event.");
                return Page();
            }
        }

    }
}
