using PPyrekBackend15043.Application.Common.Interfaces;
using PPyrekBackend15043.Domain.Constants;
using PPyrekBackend15043.Infrastructure.Data;
using PPyrekBackend15043.Infrastructure.Data.Interceptors;
using PPyrekBackend15043.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
        {

            builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

            builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("InMemoryDb");
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            });

            builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

            builder.Services.AddScoped<ApplicationDbContextInitialiser>();

            builder.Services.AddAuthentication()
                   .AddBearerToken(IdentityConstants.BearerScheme);

            builder.Services.AddAuthorizationBuilder();

            builder.Services
                   .AddIdentityCore<ApplicationUser>()
                   .AddRoles<IdentityRole>()
                   .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddTransient<IIdentityService, IdentityService>();

            builder.Services.AddAuthorization(options =>
                options.AddPolicy(Policies.CanPurge, policy =>
                    policy.RequireRole(Roles.Administrator)));
        }
    }
}
