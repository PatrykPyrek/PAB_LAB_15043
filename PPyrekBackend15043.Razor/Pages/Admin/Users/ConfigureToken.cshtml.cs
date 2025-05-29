using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace PPyrekBackend15043.Razor.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class ConfigureTokenModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public ConfigureTokenModel(
            UserManager<IdentityUser> userManager,
            IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public string UserId { get; set; } = "";

        public IdentityUser? UserEntity { get; set; }

        public List<string> AvailablePermissions { get; } = new()
        {
            "Projects.Read",   "Projects.Create", "Projects.Edit",   "Projects.Delete",
            "Tasks.Read",      "Tasks.Create",    "Tasks.Edit",      "Tasks.Delete",
            "Subtasks.Read",   "Subtasks.Create", "Subtasks.Edit",   "Subtasks.Delete"
        };

        [BindProperty]
        public List<string> SelectedPermissions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            UserEntity = await _userManager.FindByIdAsync(userId);
            if (UserEntity is null) return NotFound();

            UserId = userId;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            UserEntity = await _userManager.FindByIdAsync(UserId);
            if (UserEntity is null) return NotFound();

            var client = _httpClientFactory.CreateClient("rest");
            var response = await client.PostAsJsonAsync("auth/token", new
            {
                Username = UserEntity.UserName,
                Permissions = SelectedPermissions
            });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Token generation failed");
                return Page();
            }

            var tokenObj = await response.Content
                                         .ReadFromJsonAsync<TokenResult>();
            if (tokenObj is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid token response");
                return Page();
            }

            await _userManager.SetAuthenticationTokenAsync(
                UserEntity,
                "CustomJWT",
                "Token",
                tokenObj.Token
            );

            HttpContext.Session.SetString($"UserJwt_{UserId}", tokenObj.Token);

            TempData["Message"] = $"Custom token granted to {UserEntity.UserName}";
            return RedirectToPage("Index");
        }

        private class TokenResult
        {
            public string Token { get; set; } = "";
        }
    }
}
