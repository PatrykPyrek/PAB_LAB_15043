using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PPyrekBackend15043.Razor.Pages.Admin.RestProjects.Tasks.Subtasks
{
    [Authorize(Roles = "User,Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public List<SubtaskDto> Subtasks { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public Guid ProjectId { get; set; }
        [BindProperty(SupportsGet = true)]
        public Guid TaskId { get; set; }

        public string TaskTitle { get; set; } = "";
        public string ProjectTitle { get; set; } = "";

        public IndexModel(IHttpClientFactory httpClientFactory)
            => _httpClientFactory = httpClientFactory;

        public async Task OnGetAsync(Guid projectId, Guid taskId)
        {
            ProjectId = projectId;
            TaskId = taskId;

            var client = _httpClientFactory.CreateClient("rest");
            var token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var subtasksResp = await client.GetAsync($"projects/{ProjectId}/tasks/{TaskId}/subtasks");
            if (subtasksResp.IsSuccessStatusCode)
                Subtasks = await subtasksResp.Content.ReadFromJsonAsync<List<SubtaskDto>>() ?? new();

            var taskResp = await client.GetAsync($"projects/{ProjectId}/tasks/{TaskId}");
            if (taskResp.IsSuccessStatusCode)
            {
                var t = await taskResp.Content.ReadFromJsonAsync<TaskDto>();
                TaskTitle = t?.Title ?? "";
            }

            var projResp = await client.GetAsync($"projects/{ProjectId}");
            if (projResp.IsSuccessStatusCode)
            {
                var p = await projResp.Content.ReadFromJsonAsync<ProjectDto>();
                ProjectTitle = p?.Name ?? "";
            }
        }
    }
}
