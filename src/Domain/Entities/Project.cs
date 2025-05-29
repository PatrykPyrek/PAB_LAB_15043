
namespace PPyrekBackend15043.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public List<TaskItem> Tasks { get; set; } = new();
}
