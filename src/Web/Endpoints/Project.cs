using Microsoft.EntityFrameworkCore;
using PPyrekBackend15043.Domain.Entities;
using PPyrekBackend15043.Infrastructure.Data;

namespace PPyrekBackend15043.Web.Endpoints;

public static class ProjectsEndpoints
{
    public static void MapProjects(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/projects")
                       .WithTags("Projects");

        group.MapGet("/", async (ApplicationDbContext db) =>
            await db.Projects
                .Include(p => p.Tasks)
                .ThenInclude(t => t.Subtasks)
                .ToListAsync())
            .RequireAuthorization("Projects.Read");

        group.MapGet("/{id}", async (ApplicationDbContext db, Guid id) =>
            await db.Projects
                .Include(p => p.Tasks)
                .ThenInclude(t => t.Subtasks)
                .FirstOrDefaultAsync(p => p.Id == id)
                is Project project
                    ? Results.Ok(project)
                    : Results.NotFound())
            .RequireAuthorization("Projects.Read");

        group.MapPost("/", async (ApplicationDbContext db, Project project) =>
        {
            db.Projects.Add(project);
            await db.SaveChangesAsync();
            return Results.Created($"/projects/{project.Id}", project);
        }).RequireAuthorization("Projects.Create");

        group.MapPut("/{id}", async (ApplicationDbContext db, Guid id, Project input) =>
        {
            var project = await db.Projects.FindAsync(id);
            if (project is null) return Results.NotFound();

            project.Name = input.Name;
            await db.SaveChangesAsync();
            return Results.Ok(project);
        }).RequireAuthorization("Projects.Edit");

        group.MapDelete("/{id}", async (ApplicationDbContext db, Guid id) =>
        {
            var project = await db.Projects.FindAsync(id);
            if (project is null) return Results.NotFound();

            db.Projects.Remove(project);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization("Projects.Delete");
    }
}
