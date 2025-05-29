using HotChocolate;
using PPyrekBackend15043.Domain.Entities;
using PPyrekBackend15043.Infrastructure.Data;
using System.Threading.Tasks;

public class Mutation
{
    public async Task<Project> AddProject(
        AddProjectInput input,
        [Service] ApplicationDbContext db)
    {
        var p = new Project { Name = input.Name };
        db.Projects.Add(p);
        await db.SaveChangesAsync();
        return p;
    }
}

public record AddProjectInput(string Name);
