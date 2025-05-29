using PPyrekBackend15043.Domain.Entities;


namespace PPyrekBackend15043.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Project> Projects { get; }
    DbSet<TaskItem> Tasks { get; }
    DbSet<Subtask> Subtasks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
