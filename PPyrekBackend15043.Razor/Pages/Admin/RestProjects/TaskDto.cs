namespace PPyrekBackend15043.Razor.Pages.Admin.RestProjects.Tasks
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
    }
}
