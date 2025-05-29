using HotChocolate;
using PPyrekBackend15043.Domain.Entities;
using PPyrekBackend15043.Infrastructure.Data;
using System.Linq;

public class Query
{
    public IQueryable<Project> GetProjects([Service] ApplicationDbContext db)
        => db.Projects;
}
