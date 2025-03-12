using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using X.PagedList;
using X.PagedList.Extensions;

namespace EventManagement_Application.Pages.Attendee
{
    [Authorize] // Ensure only logged-in users can access this page
    public class FavouriteEventsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavouriteEventsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Properties for pagination
        public IPagedList<Event> FavoriteEvents { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 6;

        public async Task OnGetAsync()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                // Fetch the favorite events for the logged-in user
                var favoriteEventsQuery = _context.Favorites
                    .Where(f => f.UserId == userId)
                    .Include(f => f.Event)
                    .Select(f => f.Event);

                // Paginate the favorite events
                FavoriteEvents = favoriteEventsQuery.ToPagedList(PageNumber, PageSize);
            }
        }
    }
}
