using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        public List<Event> Events { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Events = await _context.Events.ToListAsync();
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

//[HttpPost("/Attendee/ViewEvents/TrackQRCodeScan")]
//public IActionResult TrackQRCodeScan([FromBody] int eventId)
//{
//    Console.WriteLine($"Received eventId: {eventId}"); // Log the received eventId
//    var eventItem = _context.Events.Find(eventId);
//    Console.WriteLine($"Received eventItem: {eventItem}"); // Log the received eventId

//    if (eventItem != null)
//    {
//        eventItem.QRScanCount++; // زيادة العداد
//        _context.SaveChanges(); // حفظ التغييرات في قاعدة البيانات
//        Console.WriteLine($"Updated QRScanCount for event {eventId} to {eventItem.QRScanCount}"); // Log the updated count
//        return new JsonResult(new { success = true, newCount = eventItem.QRScanCount });
//    }

//    Console.WriteLine("Event not found"); // Log if event is not found
//    return BadRequest("Event not found");
//}

//public async Task<IActionResult> OnPostTrackQRCodeScanAsync(int eventId)
//{
//    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//    _logger.LogInformation("User ID: {UserId}, Event ID: {EventId}", userId, eventId);

//    var eventItem = await _context.Events.FindAsync(eventId);
//    if (eventItem == null)
//    {
//        _logger.LogWarning("Event not found. Event ID: {EventId}", eventId);
//        return BadRequest("Event not found.");
//    }

//    _logger.LogInformation("Current QRScanCount for Event ID {EventId}: {QRScanCount}", eventId, eventItem.QRScanCount);

//    // Increment scan count
//    eventItem.QRScanCount++;

//    // Save scan details to EventViewers table
//    var eventViewer = new EventViewer
//    {
//        EventId = eventId,
//        ViewerId = userId,
//        ScanDate = DateTime.UtcNow
//    };

//    _context.EventViewers.Add(eventViewer);
//    await _context.SaveChangesAsync();

//    _logger.LogInformation("Updated QRScanCount for Event ID {EventId}: {NewQRScanCount}", eventId, eventItem.QRScanCount);

//    return new JsonResult(new { success = true, newCount = eventItem.QRScanCount });
//}

// raghad code:
public class QRScanRequest
{
    public int EventId { get; set; }
}

//public async Task<IActionResult> OnPostTrackQRCodeScanAsync([FromBody] QRScanRequest request)
//{
//    if (request == null || request.EventId == 0)
//    {
//        _logger.LogWarning("Invalid QR scan request received.");
//        return BadRequest("Invalid request.");
//    }

//    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//    _logger.LogInformation("User ID: {UserId}, Event ID: {EventId}", userId, request.EventId);

//    var eventItem = await _context.Events.FindAsync(request.EventId);
//    if (eventItem == null)
//    {
//        _logger.LogWarning("Event not found. Event ID: {EventId}", request.EventId);
//        return BadRequest("Event not found.");
//    }

//    // Increment scan count
//    eventItem.QRScanCount++;

//    // Save scan details to EventViewers table
//    var eventViewer = new EventViewer
//    {
//        EventId = request.EventId,
//        ViewerId = userId,
//        ScanDate = DateTime.UtcNow
//    };

//    _context.EventViewers.Add(eventViewer);
//    await _context.SaveChangesAsync();

//    _logger.LogInformation("Updated QRScanCount for Event ID {EventId}: {NewQRScanCount}", request.EventId, eventItem.QRScanCount);

//    return new JsonResult(new { success = true, newCount = eventItem.QRScanCount });
//}

//[HttpPost]
//public async Task<IActionResult> OnPostTrackQRCodeScanAsync([FromBody] QRScanRequest request)
//{
//    if (request == null || request.EventId <= 0)
//    {
//        return BadRequest(new { success = false, message = "Invalid event ID" });
//    }

//    var eventItem = await _context.Events.FindAsync(request.EventId);
//    if (eventItem == null)
//    {
//        _logger.LogWarning("Event not found. Event ID: {EventId}", request.EventId);
//        return NotFound(new { success = false, message = "Event not found" });
//    }

//    // Increment QRScanCount for the event
//    eventItem.QRScanCount++;
//    _context.Events.Update(eventItem);

//    // Store scan details in EventViewer table
//    var viewer = new EventViewer
//    {
//        EventId = request.EventId,
//        ScanDate = DateTime.UtcNow,
//        //IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
//    };

//    _context.EventViewers.Add(viewer);
//    await _context.SaveChangesAsync();

//    _logger.LogInformation("Updated QRScanCount for Event ID {EventId}: {NewQRScanCount}", request.EventId, eventItem.QRScanCount);
//    return new JsonResult(new { success = true, newCount = eventItem.QRScanCount });
//}

    }
}

