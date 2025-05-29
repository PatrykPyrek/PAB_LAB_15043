
namespace PPyrekBackend15043.Razor.Pages.Admin.RestProjects.Tasks.Subtasks
{
    public class SubtaskDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid TaskId { get; set; }
    }
}
