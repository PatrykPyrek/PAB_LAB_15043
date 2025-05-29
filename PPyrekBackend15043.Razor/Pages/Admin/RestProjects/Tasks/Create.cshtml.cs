using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace PPyrekBackend15043.Razor.Pages.Admin.RestProjects.Tasks
{
    [Authorize(Roles = "User,Admin")]
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CreateModel(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        [BindProperty]
        public TaskDto NewTask { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public Guid ProjectId { get; set; }

        public void OnGet(Guid projectId) => ProjectId = projectId;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var client = _httpClientFactory.CreateClient("rest");
            var jwt = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(jwt))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            NewTask.ProjectId = ProjectId;
            var response = await client.PostAsJsonAsync($"projects/{ProjectId}/tasks", NewTask);
            if (response.IsSuccessStatusCode) return RedirectToPage("Index", new { projectId = ProjectId });
            ModelState.AddModelError(string.Empty, "Failed to create.");
            return Page();
        }
    }
}
