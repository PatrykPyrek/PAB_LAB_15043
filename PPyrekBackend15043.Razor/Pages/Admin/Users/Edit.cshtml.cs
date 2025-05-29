using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PPyrekBackend15043.Razor.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(UserManager<IdentityUser> userManager)
            => _userManager = userManager;

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; } = "";

        [BindProperty]
        public List<string> SelectedRoles { get; set; } = new();

        public IList<string> AllRoles { get; set; } = new List<string>();
        public IdentityUser? UserEntity { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            UserEntity = await _userManager.FindByIdAsync(id);
            if (UserEntity is null) return NotFound();

            AllRoles = new List<string> { "User", "Admin" };
            var current = await _userManager.GetRolesAsync(UserEntity);
            SelectedRoles = current.ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            UserEntity = await _userManager.FindByIdAsync(Id);
            if (UserEntity is null) return NotFound();

            var current = await _userManager.GetRolesAsync(UserEntity);

            await _userManager.RemoveFromRolesAsync(UserEntity, current.Except(SelectedRoles));
            await _userManager.AddToRolesAsync(UserEntity, SelectedRoles.Except(current));

            TempData["Message"] = "Roles updated!";
            return RedirectToPage("Index");
        }
    }
}
