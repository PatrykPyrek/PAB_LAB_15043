using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace PPyrekBackend15043.Razor.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IHttpClientFactory httpClientFactory,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty]
        public string UserName { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string? ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            var signInResult = await _signInManager
                .PasswordSignInAsync(UserName, Password, isPersistent: false, lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {
                ErrorMessage = "Invalid login attempt.";
                _logger.LogWarning("Login failed for user {UserName}.", UserName);
                return Page();
            }

            _logger.LogInformation("User {UserName} logged in.", UserName);

            var user = await _userManager.FindByNameAsync(UserName);
            if (user is null)
            {
                _logger.LogWarning("User not found after login.");
                return RedirectToPage("/Account/Login");
            }

            var existingToken = await _userManager.GetAuthenticationTokenAsync(
                user, "CustomJWT", "Token");

            if (!string.IsNullOrEmpty(existingToken))
            {
                HttpContext.Session.SetString("JwtToken", existingToken);
                _logger.LogInformation("Loaded existing token from store.");
                return LocalRedirect(returnUrl ?? "/");
            }

            var roles = await _userManager.GetRolesAsync(user);
            string[] permissions = roles.Contains("Admin")
                ? new[]
                {
                    "Projects.Read", "Projects.Create", "Projects.Edit", "Projects.Delete",
                    "Tasks.Read",    "Tasks.Create",    "Tasks.Edit",    "Tasks.Delete",
                    "Subtasks.Read", "Subtasks.Create", "Subtasks.Edit", "Subtasks.Delete"
                  }
                : new[]
                  {
                    "Projects.Read",
                    "Tasks.Read",
                    "Subtasks.Read"
                  };

            _logger.LogInformation(
                "Generating new token for {UserName} with perms: {Permissions}",
                UserName, string.Join(", ", permissions));

            var client = _httpClientFactory.CreateClient("rest");
            var response = await client.PostAsJsonAsync("auth/token", new
            {
                Username = UserName,
                Permissions = permissions
            });

            if (response.IsSuccessStatusCode)
            {
                var tokenResult = await response.Content
                                               .ReadFromJsonAsync<TokenResult>();
                if (tokenResult is not null && !string.IsNullOrEmpty(tokenResult.Token))
                {
                    HttpContext.Session.SetString("JwtToken", tokenResult.Token);
                    _logger.LogInformation("New token saved to session.");

                    await _userManager.SetAuthenticationTokenAsync(
                        user,
                        "CustomJWT",
                        "Token",
                        tokenResult.Token
                    );
                    _logger.LogInformation("New token persisted to Identity store.");
                }
                else
                {
                    _logger.LogWarning("Token endpoint returned empty token.");
                }
            }
            else
            {
                var err = await response.Content.ReadAsStringAsync();
                _logger.LogError("Token endpoint error {Status}: {Err}",
                                 response.StatusCode, err);
            }

            return LocalRedirect(returnUrl ?? "/");
        }

        public class TokenResult
        {
            public string Token { get; set; } = "";
        }
    }
}
