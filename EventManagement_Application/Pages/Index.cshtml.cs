using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EventManagement_Application.Pages
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;
        private readonly EmailService _emailService;

        public IndexModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<IndexModel> logger, EmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        public List<Event> LatestEvents { get; set; }
        public List<Event> UpcomingEvents { get; set; }
        [BindProperty]
        public ContactForm ContactForm { get; set; }
        public List<Feedback> RecentFeedbacks { get; set; }
        public async Task OnGetAsync()
        {
            //LatestEvents = await _context.Events
            //    .OrderByDescending(e => e.CreationDate) // Sort by event date (descending, newest first)
            //    .Take(8) // Fetch only the latest 6 events
            //    .ToListAsync();

            var today = DateTime.Now;
            var firstDayOfTwoMonthsAgo = new DateTime(today.Year, today.Month, 1).AddMonths(-2);
            var lastDayOfLastMonth = new DateTime(today.Year, today.Month, 1).AddDays(-1);

            // Fetch past two months' events sorted by event date (most recent past event first)
            LatestEvents = await _context.Events
                .Where(e => e.EventDate >= firstDayOfTwoMonthsAgo && e.EventDate <= lastDayOfLastMonth) // Filter last 2 months
                .OrderByDescending(e => e.EventDate) // Show most recent past events first
                .Take(8) // Fetch only 8 past events
                .ToListAsync();

            UpcomingEvents = await _context.Events
                         .Where(e => e.EventDate > DateTime.Now) // Only future events
                         .OrderBy(e => e.EventDate) // Closest first
                         .Take(8)
                         .ToListAsync();

            // Fetch the 5 most recent feedbacks
            RecentFeedbacks = await _context.Feedbacks
                                            .OrderByDescending(f => f.SubmittedAt)
                                            .Take(5)
                                            .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Received an organizer request.");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found.");
                return NotFound();
            }

            _logger.LogInformation("Processing request for user: {UserId}", user.Id);

            // Check if a request already exists
            var existingRequest = await _context.OrganizerRequests
                .FirstOrDefaultAsync(r => r.UserId == user.Id && r.Status == "Pending");

            if (existingRequest != null)
            {
                TempData["ErrorMessage"] = "You have already submitted a request.";
                return RedirectToPage("/Index");
            }

            // Create and save new request
            var newRequest = new OrganizerRequest
            {
                UserId = user.Id,
                Status = "Pending"
            };

            _context.OrganizerRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your request has been sent! We will review it soon.";
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostContactAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ContactForm.UserEmail = user.Email;
                ContactForm.UserId = user.Id;
            }

            ContactForm.SubmittedAt = DateTime.UtcNow; // Set the submission timestamp

            // Save to database
            //await SaveToDatabaseAsync(ContactForm);

            await _emailService.SendEmailAsync(ContactForm.UserEmail ?? "anonymous@example.com", ContactForm.Subject, ContactForm.Message);

            //TempData["SuccessMessage"] = "Your message has been sent!";
            //return RedirectToPage("/Index");

            // Return a JSON response indicating success
            return new JsonResult(new { success = true });
        }

        //private async Task SaveToDatabaseAsync(ContactForm contactForm)
        //{
        //    _context.ContactForms.Add(contactForm);
        //    await _context.SaveChangesAsync();
        //}

    }
}