using Microsoft.EntityFrameworkCore;
using PPyrekBackend15043.Domain.Entities;
using PPyrekBackend15043.Infrastructure.Data;

namespace PPyrekBackend15043.Web.Endpoints;

public static class TasksEndpoints
{
    public static void MapTasks(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/projects/{projectId}/tasks")
                       .WithTags("Tasks");

        group.MapGet("/", async (ApplicationDbContext db, Guid projectId) =>
        {
            var tasks = await db.Tasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.Subtasks)
                .ToListAsync();
            return Results.Ok(tasks);
        }).RequireAuthorization("Tasks.Read");

        group.MapGet("/{id}", async (ApplicationDbContext db, Guid projectId, Guid id) =>
        {
            var task = await db.Tasks
                .Include(t => t.Subtasks)
                .FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId);
            return task is not null ? Results.Ok(task) : Results.NotFound();
        }).RequireAuthorization("Tasks.Read");

        group.MapPost("/", async (ApplicationDbContext db, Guid projectId, TaskItem task) =>
        {
            task.ProjectId = projectId;
            db.Tasks.Add(task);
            await db.SaveChangesAsync();
            return Results.Created($"/projects/{projectId}/tasks/{task.Id}", task);
        }).RequireAuthorization("Tasks.Create");

        group.MapPut("/{id}", async (ApplicationDbContext db, Guid projectId, Guid id, TaskItem input) =>
        {
            var task = await db.Tasks.FindAsync(id);
            if (task is null || task.ProjectId != projectId) return Results.NotFound();

            task.Title = input.Title;
            await db.SaveChangesAsync();
            return Results.Ok(task);
        }).RequireAuthorization("Tasks.Edit");

        group.MapDelete("/{id}", async (ApplicationDbContext db, Guid projectId, Guid id) =>
        {
            var task = await db.Tasks.FindAsync(id);
            if (task is null || task.ProjectId != projectId) return Results.NotFound();

            db.Tasks.Remove(task);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization("Tasks.Delete");
    }
}
