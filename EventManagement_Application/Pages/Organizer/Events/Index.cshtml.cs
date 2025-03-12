using EventManagement_Application.Data;
using EventManagement_Application.Enums;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EventManagement_Application.Pages.Organizer.Events
{
    [Authorize(Roles = "Organizer")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Properties for pagination
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 6;

        public List<Event> Events { get; set; } = new();
        public List<string> Categories { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedCategory { get; set; }

        [BindProperty(SupportsGet = true)]
        public EventMode? SelectedMode { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? SearchDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedTicketType { get; set; }
        public List<string> TicketTypes { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Events = new List<Event>(); // No user logged in, return empty events
                return;
            }

            // Fetch distinct Ticket Types for the dropdown
            TicketTypes = await _context.Tickets
                .Select(t => t.TicketType)
                .Distinct()
                .ToListAsync();

            // Get only events created by the logged-in organizer
            IQueryable<Event> query = _context.Events
                .Where(e => e.OrganizerId == userId); // Filter by OrganizerId

            // Apply Search
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(e => e.Category.Contains(SearchTerm) || e.Location.Contains(SearchTerm));
            }

            // Apply Category search
            if (!string.IsNullOrEmpty(SelectedCategory))
            {
                query = query.Where(e => e.Category == SelectedCategory);
            }

            // Apply Date search
            if (SearchDate.HasValue)
            {
                DateTime selectedDate = SearchDate.Value.Date;
                query = query.Where(e => e.EventDate.Date == selectedDate);
            }

            // Apply Event Mode Filter (Online/In-Person)
            if (SelectedMode.HasValue)
            {
                query = query.Where(e => e.Mode == SelectedMode);
            }

            // Apply Ticket Type Filter
            if (!string.IsNullOrEmpty(SelectedTicketType))
            {
                query = query.Where(e => e.Tickets.Any(t => t.TicketType.ToString() == SelectedTicketType));
            }

            //Events = await query.ToListAsync();
            // Calculate total number of events and pages
            var totalEvents = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalEvents / (double)PageSize);

            // Paginate the events
            Events = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        [HttpPost] // Handle delete request
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);
            if (eventToDelete == null)
            {
                return NotFound();
            }

            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
    }
}
