using PPyrekBackend15043.Domain.Constants;
using PPyrekBackend15043.Domain.Entities;
using PPyrekBackend15043.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace PPyrekBackend15043.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public Task InitialiseAsync()
    {
        return Task.CompletedTask;
    }


    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {

        // Default Project/Task/Subtask
        if (!_context.Projects.Any())
        {
            var project = new Project
            {
                Name = "Demo Project",
                Tasks = new List<TaskItem>
            {
                new TaskItem
                {
                    Title = "Seeded Task",
                    Subtasks = new List<Subtask>
                    {
                        new Subtask { Description = "Seeded Subtask 1" },
                        new Subtask { Description = "Seeded Subtask 2" }
                    }
                }
            }
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }
    }

}
