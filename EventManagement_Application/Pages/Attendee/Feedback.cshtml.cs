using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventManagement_Application.Pages.Attendee
{
    public class FeedbackModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        public FeedbackModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string UserName, int Rating, string Comment)
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Comment) || Rating < 1 || Rating > 5)
            {
                return Page();
            }
            var user = await _userManager.GetUserAsync(User);

            // Default avatar if user profile picture is null
            string userAvatar = "/images/default-avatar.jpg";

            if (user?.ProfilePicture != null && user.ProfilePicture.Length > 0)
            {
                string base64String = Convert.ToBase64String(user.ProfilePicture);
                userAvatar = $"data:image/png;base64,{base64String}";
            }

            var feedback = new Feedback
            {
                UserName = user?.UserName ?? "Anonymous",
                Rating = Rating,
                UserAvatar = userAvatar,
                Comment = Comment
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return RedirectToPage("Feedback");
        }
    }
}
