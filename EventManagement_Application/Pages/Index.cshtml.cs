using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EventManagement_Application.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        //private readonly UserManager<ApplicationUser> _userManager;

        //public IndexModel(UserManager<ApplicationUser> userManager)
        //{
        //    _userManager = userManager;
        //}

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    user.OrganizerRequestPending = true;
        //    await _userManager.UpdateAsync(user);

        //    TempData["SuccessMessage"] = "Your request has been sent! We will review it soon.";
        //    return RedirectToPage("/Index");
        //}

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;   
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    _logger.LogInformation("hello world");

        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        _logger.LogInformation("user is null", user);
        //        return NotFound();
        //    }
        //    _logger.LogInformation("this is the user: ",user);

        //    // Check if a request already exists
        //    var existingRequest = _context.OrganizerRequests.FirstOrDefault(r => r.UserId == user.Id && r.Status == "Pending");
        //    if (existingRequest != null)
        //    {
        //        TempData["ErrorMessage"] = "You have already submitted a request.";
        //        return RedirectToPage("/Index");
        //    }

        //    // Create and save new request
        //    var newRequest = new OrganizerRequest
        //    {
        //        UserId = user.Id,
        //        Status = "Pending"
        //    };

        //    _context.OrganizerRequests.Add(newRequest);
        //    await _context.SaveChangesAsync();

        //    TempData["SuccessMessage"] = "Your request has been sent! We will review it soon.";
        //    return RedirectToPage("/Index");
        //}


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

    }
}
