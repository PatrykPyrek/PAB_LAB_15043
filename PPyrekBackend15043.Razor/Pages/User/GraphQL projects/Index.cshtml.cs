using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PPyrekBackend15043.Razor.Pages.User.GraphqlProjects {

    [Authorize(Roles = "User,Admin")]
    public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _http;
    [BindProperty] public string NewName { get; set; } = "";
    public List<ProjectDto> Projects { get; set; } = new();

    public IndexModel(IHttpClientFactory http) => _http = http;

    public async Task OnGetAsync()
    {
        var client = _http.CreateClient("graphql");
        var token = HttpContext.Session.GetString("JWToken");
        if (token != null)
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

        var req = new { query = "{ projects { id name } }" };
        var resp = await client.PostAsJsonAsync("", req);
        var wrapper = await resp.Content
            .ReadFromJsonAsync<GraphQLResponse<ProjectsData>>();
        Projects = wrapper?.Data.Projects ?? new();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = _http.CreateClient("graphql");
        var token = HttpContext.Session.GetString("JWToken");
        if (token != null)
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

        var mutation = @"mutation($i:AddProjectInput!){
            addProject(input:$i){ id name }
        }";
        var req = new
        {
            query = mutation,
            variables = new { i = new { name = NewName } }
        };
        await client.PostAsJsonAsync("", req);
        return RedirectToPage();
    }

    public record ProjectDto(Guid Id, string Name);

    public class GraphQLResponse<T>
    {
        public T Data { get; set; } = default!;
    }
    public class ProjectsData
    {
        public List<ProjectDto> Projects { get; set; } = new();
    }
}
}
