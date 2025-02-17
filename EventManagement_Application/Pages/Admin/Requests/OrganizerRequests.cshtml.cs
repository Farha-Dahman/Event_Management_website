using EventManagement_Application.Data;
using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EventManagement_Application.Pages.Admin.Requests
{
    [Authorize(Roles = "Admin")]
    public class OrganizerRequestsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public OrganizerRequestsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<OrganizerRequest> PendingRequests { get; set; }

        public async Task OnGetAsync()
        {
            // Fetch pending requests and include the User details
            PendingRequests = await _context.OrganizerRequests
                .Where(r => r.Status == "Pending")
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string userId, bool approve, bool reject)
        {
            var request = await _context.OrganizerRequests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.Status == "Pending");

            if (request == null)
            {
                return NotFound();
            }

            if (approve)  // Approve request
            {
                request.Status = "Approved";

                // Ensure "Organizer" role exists
                if (!await _roleManager.RoleExistsAsync("Organizer"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Organizer"));
                }

                await _userManager.AddToRoleAsync(request.User, "Organizer");
            }
            else if (reject)  // Reject request
            {
                request.Status = "Rejected";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
