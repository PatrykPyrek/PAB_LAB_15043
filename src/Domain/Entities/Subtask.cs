using System.Text.Json.Serialization;

namespace PPyrekBackend15043.Domain.Entities;

public class Subtask
{
    public Guid Id { get; set; }
    public string Description { get; set; } = default!;
    public Guid TaskItemId { get; set; }

    [JsonIgnore]
    public TaskItem? TaskItem { get; set; }
}
