using EventManagement_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EventManagement_Application.Pages.Admin.Roles
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public List<IdentityRole> Roles { get; set; } = new();

        [BindProperty]
        public AddNewRole RoleForm { get; set; } = new(); // Ensure binding works

        public IndexModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task OnGetAsync()
        {
            Roles = await _roleManager.Roles.ToListAsync();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(RoleForm.Name))
            {
                ModelState.AddModelError("RoleForm.Name", "Role name is required!");
                Roles = await _roleManager.Roles.ToListAsync();
                return Page();
            }

            // Check if role already exists
            if (await _roleManager.RoleExistsAsync(RoleForm.Name.Trim()))
            {
                ModelState.AddModelError("RoleForm.Name", "Role already exists!");
                Roles = await _roleManager.Roles.ToListAsync();
                return Page();
            }

            // Create role
            var newRole = new IdentityRole(RoleForm.Name.Trim());
            var result = await _roleManager.CreateAsync(newRole);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Error creating role.");
                Roles = await _roleManager.Roles.ToListAsync();
                return Page();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetCheckRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return new JsonResult(new { exists = false });
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName.Trim());
            return new JsonResult(new { exists = roleExists });
        }
    }
}