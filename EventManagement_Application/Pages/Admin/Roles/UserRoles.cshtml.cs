using EventManagement_Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EventManagement_Application.Pages.Admin.Roles
{
    public class UserRolesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<UserInfo> Users { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch all users first
            var users = await _userManager.Users.ToListAsync();

            // Create a list to store user info with roles
            Users = new List<UserInfo>();

            // Loop through each user to fetch their roles
            foreach (var user in users)
            {
                // Fetch roles for each user asynchronously
                var roles = await _userManager.GetRolesAsync(user);

                // Add the user info along with their roles
                Users.Add(new UserInfo
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return Page();
        }

        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByNameAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();

            var viewRoles = new UserRoles
            {
                UserId = userId,
                UserName = user.UserName,
                Roles = roles.Select(role => new Role
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = _userManager.IsInRoleAsync(user, role.Name).Result
                }).ToList()
            };
            return Page();  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(UserRoles model)
        {
            var user = await _userManager.FindByNameAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in model.Roles)
            {
                if (userRoles.Any(r => r == role.RoleName) && !role.IsSelected)
                    await _userManager.RemoveFromRoleAsync(user, role.RoleName);

                if (!userRoles.Any(r => r == role.RoleName) && role.IsSelected)
                    await _userManager.AddToRoleAsync(user, role.RoleName);
            }
            return RedirectToAction(nameof(ManageRoles));
        }
    }
}
