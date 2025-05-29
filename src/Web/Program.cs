using PPyrekBackend15043.Infrastructure.Data;
using PPyrekBackend15043.Web.Endpoints;
using PPyrekBackend15043.Web.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PPyrekBackend15043.Web.Endpoints.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();
builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});


builder.Services.AddAuthorization(options =>
{
    var permissions = new[]
    {
        "Projects.Read", "Projects.Create", "Projects.Edit", "Projects.Delete",
        "Tasks.Read", "Tasks.Create", "Tasks.Edit", "Tasks.Delete",
        "Subtasks.Read", "Subtasks.Create", "Subtasks.Edit", "Subtasks.Delete"
    };

    foreach (var permission in permissions)
    {
        options.AddPolicy(permission, policy =>
            policy.RequireClaim("permissions", permission));
    }
});



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseRequestHeaderLogging();
app.UseNightBlockMiddleware();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});
app.UseExceptionHandler(options => { });
app.Map("/", () => Results.Redirect("/api"));
app.UseOpenApi();
app.MapEndpoints();
app.MapProjects();
app.MapTasks();
app.MapSubtasks();
app.MapControllers();
app.Run();

public partial class Program { }
