using Microsoft.AspNetCore.Identity;
using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Infrastructure.Persistence;

namespace TrailerTrack.Infrastructure.Identity;

public static class DbSeeder
{
    public static async Task SeedAsync(UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager,
    AppDbContext db)
    {
        // Seed roles
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        if (!await roleManager.RoleExistsAsync("Staff"))
            await roleManager.CreateAsync(new IdentityRole("Staff"));

        // Seed admin user
        if (await userManager.FindByEmailAsync("admin@trailertrack.com") is null)
        {
            var admin = new AppUser
            {
                UserName = "admin@trailertrack.com",
                Email = "admin@trailertrack.com",
                FullName = "System Admin"
            };
            await userManager.CreateAsync(admin, "Admin1234!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Seed staff user
        if (await userManager.FindByEmailAsync("staff@trailertrack.com") is null)
        {
            var staff = new AppUser
            {
                UserName = "staff@trailertrack.com",
                Email = "staff@trailertrack.com",
                FullName = "Staff User"
            };
            await userManager.CreateAsync(staff, "Staff1234!");
            await userManager.AddToRoleAsync(staff, "Staff");
        }

        // Seed Assets
        if (!db.Assets.Any())
        {
            var assets = new List<Asset>
            {
                Asset.Create("TRL-001", "Adelaide CBD", AssetType.BoxTrailer),
                Asset.Create("TRL-002", "Gawler", AssetType.CageTrailer),
                Asset.Create("TRL-003", "Mount Barker", AssetType.CarTrailer),
                Asset.Create("TRL-004", "Port Adelaide", AssetType.BoatTrailer),
                Asset.Create("TRL-005", "Elizabeth", AssetType.PlantTrailer),
            };

            // Manually set different statuses for testing
            assets[1].UpdateStatus(AssetStatus.HiredOut);
            assets[2].UpdateStatus(AssetStatus.UnderMaintenance);
            assets[3].UpdateStatus(AssetStatus.Retired);

            db.Assets.AddRange(assets);
            await db.SaveChangesAsync();
        }
    }
}