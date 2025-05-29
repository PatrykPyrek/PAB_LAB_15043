using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace PPyrekBackend15043.Razor.Pages.Admin.RestProjects.Tasks.Subtasks
{
    [Authorize(Roles = "User,Admin")]
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DeleteModel(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        [BindProperty]
        public SubtaskDto Subtask { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public Guid ProjectId { get; set; }
        [BindProperty(SupportsGet = true)]
        public Guid TaskId { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid projectId, Guid taskId, Guid id)
        {
            ProjectId = projectId;
            TaskId = taskId;
            var client = _httpClientFactory.CreateClient("rest");
            var jwt = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(jwt))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var dto = await client.GetFromJsonAsync<SubtaskDto>($"projects/{ProjectId}/tasks/{TaskId}/subtasks/{id}");
            if (dto == null) return RedirectToPage("Index", new { projectId = ProjectId, taskId = TaskId });
            Subtask = dto;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("rest");
            var jwt = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(jwt))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            await client.DeleteAsync($"projects/{ProjectId}/tasks/{TaskId}/subtasks/{Subtask.Id}");
            return RedirectToPage("Index", new { projectId = ProjectId, taskId = TaskId });
        }
    }
}
