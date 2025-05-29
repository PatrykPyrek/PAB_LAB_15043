using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PPyrekBackend15043.Razor.Pages.Admin.RestProjects
{
    [Authorize(Roles = "User,Admin")]
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public EditModel(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        [BindProperty]
        public ProjectDto Project { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var client = _httpClientFactory.CreateClient("rest");
            var jwt = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(jwt))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var dto = await client.GetFromJsonAsync<ProjectDto>($"projects/{id}");
            if (dto == null) return RedirectToPage("Index");
            Project = dto;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var client = _httpClientFactory.CreateClient("rest");
            var jwt = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(jwt))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var response = await client.PutAsJsonAsync($"projects/{Project.Id}", Project);
            if (response.IsSuccessStatusCode) return RedirectToPage("Index");
            ModelState.AddModelError(string.Empty, "Failed to save.");
            return Page();
        }
    }
}
