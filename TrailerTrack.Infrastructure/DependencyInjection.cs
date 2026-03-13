
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrailerTrack.Domain.Interfaces;
using TrailerTrack.Infrastructure.Identity;
using TrailerTrack.Infrastructure.Persistence;
using TrailerTrack.Infrastructure.Persistence.Repositories;

namespace TrailerTrack.Infrastructure;

public static class DependencyInjection
{

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IMaintenanceLogRepository, MaintenanceLogRepository>();
        services.AddScoped<IHireEventRepository, HireEventRepository>();


        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/login";
            options.AccessDeniedPath = "/access-denied";
        });

        return services;
    }
}