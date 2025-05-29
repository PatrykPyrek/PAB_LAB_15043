using Microsoft.EntityFrameworkCore;
using PPyrekBackend15043.Domain.Entities;
using PPyrekBackend15043.Infrastructure.Data;

namespace PPyrekBackend15043.Web.Endpoints;

public static class SubtasksEndpoints
{
    public static void MapSubtasks(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/projects/{projectId}/tasks/{taskId}/subtasks")
                       .WithTags("Subtasks");

        group.MapGet("/", async (ApplicationDbContext db, Guid projectId, Guid taskId) =>
        {
            var subtasks = await db.Subtasks
                .Where(s => s.TaskItemId == taskId)
                .ToListAsync();
            return Results.Ok(subtasks);
        }).RequireAuthorization("Subtasks.Read");

        group.MapGet("/{id}", async (ApplicationDbContext db, Guid projectId, Guid taskId, Guid id) =>
        {
            var subtask = await db.Subtasks
                .FirstOrDefaultAsync(s => s.Id == id && s.TaskItemId == taskId);
            return subtask is not null ? Results.Ok(subtask) : Results.NotFound();
        }).RequireAuthorization("Subtasks.Read");

        group.MapPost("/", async (ApplicationDbContext db, Guid projectId, Guid taskId, Subtask subtask) =>
        {
            subtask.TaskItemId = taskId;
            db.Subtasks.Add(subtask);
            await db.SaveChangesAsync();
            return Results.Created($"/projects/{projectId}/tasks/{taskId}/subtasks/{subtask.Id}", subtask);
        }).RequireAuthorization("Subtasks.Create");

        group.MapPut("/{id}", async (ApplicationDbContext db, Guid projectId, Guid taskId, Guid id, Subtask input) =>
        {
            var subtask = await db.Subtasks.FindAsync(id);
            if (subtask is null || subtask.TaskItemId != taskId) return Results.NotFound();

            subtask.Description = input.Description;
            await db.SaveChangesAsync();
            return Results.Ok(subtask);
        }).RequireAuthorization("Subtasks.Edit");

        group.MapDelete("/{id}", async (ApplicationDbContext db, Guid projectId, Guid taskId, Guid id) =>
        {
            var subtask = await db.Subtasks.FindAsync(id);
            if (subtask is null || subtask.TaskItemId != taskId) return Results.NotFound();

            db.Subtasks.Remove(subtask);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization("Subtasks.Delete");
    }
}
