using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PPyrekBackend15043.Razor.Pages.Admin.RestProjects.Tasks
{
    [Authorize(Roles = "User,Admin")]
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DeleteModel(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        [BindProperty]
        public TaskDto CurrentTask { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public Guid ProjectId { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid projectId, Guid id)
        {
            ProjectId = projectId;
            var client = _httpClientFactory.CreateClient("rest");
            var jwt = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(jwt))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var dto = await client.GetFromJsonAsync<TaskDto>($"projects/{projectId}/tasks/{id}");
            if (dto == null) return RedirectToPage("Index", new { projectId });

            CurrentTask = dto;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("rest");
            var jwt = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(jwt))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            await client.DeleteAsync($"projects/{ProjectId}/tasks/{CurrentTask.Id}");
            return RedirectToPage("Index", new { projectId = ProjectId });
        }
    }
}
