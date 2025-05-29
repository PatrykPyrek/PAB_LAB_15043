using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PPyrekBackend15043.Razor.Pages.Admin.RestProjects
{
    [Authorize(Roles = "User,Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public List<ProjectDto> Projects { get; set; } = new();

        public IndexModel(IHttpClientFactory httpClientFactory)
            => _httpClientFactory = httpClientFactory;

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("rest");
            var token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("projects");
            if (response.IsSuccessStatusCode)
                Projects = await response.Content.ReadFromJsonAsync<List<ProjectDto>>() ?? new();
        }
    }
}
