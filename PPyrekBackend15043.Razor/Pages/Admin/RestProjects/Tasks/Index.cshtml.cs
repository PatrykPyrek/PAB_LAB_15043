using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PPyrekBackend15043.Razor.Pages.Admin.RestProjects.Tasks
{
    [Authorize(Roles = "User,Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public List<TaskDto> Tasks { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; } = "";

        public IndexModel(IHttpClientFactory httpClientFactory)
            => _httpClientFactory = httpClientFactory;

        public async Task OnGetAsync(Guid projectId)
        {
            ProjectId = projectId;
            var client = _httpClientFactory.CreateClient("rest");
            var token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var tasksResp = await client.GetAsync($"projects/{ProjectId}/tasks");
            if (tasksResp.IsSuccessStatusCode)
                Tasks = await tasksResp.Content.ReadFromJsonAsync<List<TaskDto>>() ?? new();

            var projResp = await client.GetAsync($"projects/{ProjectId}");
            if (projResp.IsSuccessStatusCode)
            {
                var proj = await projResp.Content.ReadFromJsonAsync<ProjectDto>();
                ProjectName = proj?.Name ?? "";
            }
        }
    }
}
