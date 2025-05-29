using System.Text.Json.Serialization;

namespace PPyrekBackend15043.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public Guid ProjectId { get; set; }

    [JsonIgnore]
    public Project? Project { get; set; }

    public List<Subtask> Subtasks { get; set; } = new();
}
