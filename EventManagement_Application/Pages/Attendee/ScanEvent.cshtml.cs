using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

//namespace EventManagement_Application.Pages.Attendee
//{
//    public class ScanEventModel : PageModel
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly ILogger<ScanEventModel> _logger;

//        public ScanEventModel(ApplicationDbContext context, ILogger<ScanEventModel> logger)
//        {
//            _context = context;
//            _logger = logger;
//        }

//        public async Task<IActionResult> OnGetAsync(int eventId)
//        {
//            // Fetch the event
//            var eventItem = await _context.Events.FindAsync(eventId);
//            if (eventItem == null)
//            {
//                _logger.LogWarning("Event with ID {EventId} not found.", eventId);
//                return NotFound();
//            }

//            // Increment the QRScanCount
//            eventItem.QRScanCount++;

//            // Record the scan in the EventViewer table
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var eventViewer = new EventViewer
//            {
//                EventId = eventId,
//                ViewerId = userId,
//                ScanDate = DateTime.UtcNow
//            };

//            _context.EventViewers.Add(eventViewer);
//            await _context.SaveChangesAsync();

//            _logger.LogInformation("QR code scanned for event ID {EventId}. New scan count: {QRScanCount}", eventId, eventItem.QRScanCount);

//            // Redirect to the event details page
//            return RedirectToPage("/Attendee/EventDetails", new { eventId = eventId });
//        }
//    }
//}

using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EventManagement_Application.Pages.Attendee
{
    public class ScanEventModel : PageModel
    {
    private readonly ApplicationDbContext _context;
        private readonly ILogger<ScanEventModel> _logger;

        public ScanEventModel(ApplicationDbContext context, ILogger<ScanEventModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int eventId)
        {
            _logger.LogInformation("QR Code scanned for event ID: {EventId}", eventId);

            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound("Event not found");
            }

            // Increment QR Scan Count
            eventItem.QRScanCount++;

            // Track the scan in EventViewer table
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var eventViewer = new EventViewer
            {
                EventId = eventId,
                ViewerId = userId,
                ScanDate = DateTime.UtcNow
            };

            _context.EventViewers.Add(eventViewer);
            await _context.SaveChangesAsync();

            // Redirect to the event details page
            return Redirect($"/Attendee/EventDetails?eventId={eventId}");
        }
    }
}

