using EventManagement_Application.Data;
using EventManagement_Application.Enums;
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
    public class ViewEventsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ViewEventsModel> _logger;

        public ViewEventsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<ViewEventsModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        //public List<Event> Events { get; set; }

        public List<int> FavoritedEventIds { get; set; } = new List<int>();
        public IPagedList<Event> Events { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 12;

        // Properties for search and filtering
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedCategory { get; set; }

        [BindProperty(SupportsGet = true)]
        public EventMode? SelectedMode { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? SearchDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedTicketType { get; set; }

        public List<string> TicketTypes { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch distinct Ticket Types for the dropdown
            TicketTypes = await _context.Tickets
                .Select(t => t.TicketType)
                .Distinct()
                .ToListAsync();

            // Base query for events
            var eventsQuery = _context.Events
                .OrderBy(e => e.EventDate); // Order by event date

            // Apply search and filters
            var query = eventsQuery.AsQueryable(); // Start with the base query

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(e =>
                    e.Title.Contains(SearchTerm) ||
                    e.Category.Contains(SearchTerm) ||
                    e.Location.Contains(SearchTerm));
            }

            if (!string.IsNullOrEmpty(SelectedCategory))
            {
                query = query.Where(e => e.Category == SelectedCategory);
            }

            if (SearchDate.HasValue)
            {
                query = query.Where(e => e.EventDate.Date == SearchDate.Value.Date);
            }

            if (SelectedMode.HasValue)
            {
                query = query.Where(e => e.Mode == SelectedMode);
            }

            if (!string.IsNullOrEmpty(SelectedTicketType))
            {
                query = query.Where(e => e.Tickets.Any(t => t.TicketType == SelectedTicketType));
            }

            // Paginate the filtered query
            Events = query.ToPagedList(PageNumber, PageSize);

            // Fetch favorited event IDs for the logged-in user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                FavoritedEventIds = await _context.Favorites
                    .Where(f => f.UserId == userId)
                    .Select(f => f.EventId)
                    .ToListAsync();
            }

            return Page();
        }
        public async Task<IActionResult> OnPostTrackQRCodeScanAsync(int eventId)
        {
            _logger.LogInformation("OnPostTrackQRCodeScanAsync: Event ID: {EventId}", eventId);
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound();
            }

            // Increment the QRScanCount
            eventItem.QRScanCount++;

            // Record the scan in the EventViewer table
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var eventViewer = new EventViewer
            {
                EventId = eventId,
                ViewerId = userId,
                ScanDate = DateTime.UtcNow
            };

            _context.EventViewers.Add(eventViewer);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, newCount = eventItem.QRScanCount });
        }
        public class QRScanRequest
        {
            public int EventId { get; set; }
        }

        public async Task<IActionResult> OnPostToggleFavoriteAsync([FromBody] QRScanRequest request)
        {
            _logger.LogInformation("call OnPostToggleFavoriteAsync ");
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogInformation("user_id: {userId}", userId);
                if (userId == null)
                {
                    _logger.LogWarning("User not authenticated.");
                    //return new JsonResult(new { success = false, message = "User not authenticated" });
                    return new JsonResult(new { success = false, message = "You need to log in to add events to favorites." });

                }
                _logger.LogInformation(" before existingFavorite: {userId}, {request.EventId}", userId, request.EventId);

                var existingFavorite = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.EventId == request.EventId);
                _logger.LogInformation("existingFavorite: {existingFavorite}", existingFavorite);
                if (existingFavorite != null)
                {
                    _logger.LogInformation("existingFavorite != null");
                    _context.Favorites.Remove(existingFavorite);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Event {EventId} removed from favorites by user {UserId}.", request.EventId, userId);
                    return new JsonResult(new { success = true, isFavorited = false });
                }
                else
                {
                    var favoriteEvent = new FavoriteEvent
                    {
                        UserId = userId,
                        EventId = request.EventId
                    };
                    _logger.LogInformation("add to Favorite:", userId, " event:", request.EventId);
                    _context.Favorites.Add(favoriteEvent);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Event {EventId} added to favorites by user {UserId}.", request.EventId, userId);
                    return new JsonResult(new { success = true, isFavorited = true });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling favorite for event {EventId}", request.EventId);
                return new JsonResult(new { success = false, message = "An error occurred" });
            }
        }


    }
}

