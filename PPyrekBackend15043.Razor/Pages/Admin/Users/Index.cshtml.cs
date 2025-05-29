using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace PPyrekBackend15043.Razor.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(UserManager<IdentityUser> userManager,
                          IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _httpClientFactory = httpClientFactory;
        }

        public List<IdentityUser> Users { get; set; } = new();
        public Dictionary<string, IList<string>> UserRoles { get; set; } = new();

        public async Task OnGetAsync()
        {
            Users = _userManager.Users.ToList();
            foreach (var u in Users)
                UserRoles[u.Id] = await _userManager.GetRolesAsync(u);
        }

        public async Task<IActionResult> OnPostToggleRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                TempData["Message"] = $"Admin role removed from {user.UserName}";
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                TempData["Message"] = $"Admin role granted to {user.UserName}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRegenerateTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var perms = new[] { "Projects.*", "Tasks.*", "Subtasks.*" };

            var client = _httpClientFactory.CreateClient("rest");
            var resp = await client.PostAsJsonAsync("auth/token", new
            {
                Username = user.UserName,
                Permissions = perms
            });
            if (!resp.IsSuccessStatusCode)
                return BadRequest("Token generation failed");

            var tokenObj = await resp.Content.ReadFromJsonAsync<TokenResult>();
            if (tokenObj is null)
                return BadRequest("Invalid token response");

            HttpContext.Session.SetString($"UserJwt_{userId}", tokenObj.Token);

            TempData["Message"] = $"Admin-token granted to {user.UserName}";
            return RedirectToPage();
        }

        private class TokenResult { public string Token { get; set; } = ""; }
    }
}
