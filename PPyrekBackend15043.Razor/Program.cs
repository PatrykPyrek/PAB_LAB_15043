using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PPyrekBackend15043.Infrastructure.Data;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opts =>
{
    opts.IdleTimeout = TimeSpan.FromMinutes(30);
    opts.Cookie.HttpOnly = true;
    opts.Cookie.IsEssential = true;
});


builder.Services.AddDbContext<IdentityDbContext>(opt =>
    opt.UseInMemoryDatabase("IdentityDb"));


builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Account/Login";
    opts.AccessDeniedPath = "/Account/AccessDenied";
    opts.Cookie.HttpOnly = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddRazorPages();
builder.Services.AddHttpClient("rest", client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/");
});

builder.Services.AddHttpClient("graphql", c =>
    c.BaseAddress = new Uri("https://localhost:7047/graphql"));

var app = builder.Build();

app.UseSession();
app.UseStaticFiles();
app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax
});


app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    if (!await roleManager.RoleExistsAsync("User"))
        await roleManager.CreateAsync(new IdentityRole("User"));

    var admin = new IdentityUser
    {
        UserName = "admin@example.com",
        Email = "admin@example.com"
    };
    if (await userManager.FindByNameAsync(admin.UserName) == null)
    {
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }

    var user = new IdentityUser
    {
        UserName = "user@example.com",
        Email = "user@example.com"
    };
    if (await userManager.FindByNameAsync(user.UserName) == null)
    {
        await userManager.CreateAsync(user, "User123!");
        await userManager.AddToRoleAsync(user, "User");
    }
}

app.Run();
