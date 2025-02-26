using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventManagement_Application.Pages.Admin.Organizers
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public List<ApplicationUser> Organizers { get; set; }

        public async Task OnGetAsync()
        {
            // Fetch all users with the "Organizer" role
            var organizerRoleUsers = await _userManager.GetUsersInRoleAsync("Organizer");
            Organizers = organizerRoleUsers.ToList();
        }
    }
}
