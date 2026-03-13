using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrailerTrack.Domain.Entities;
using TrailerTrack.Infrastructure.Identity;
namespace TrailerTrack.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Asset> Assets{ get; set; }
    public DbSet<HireEvent> HireEvents{ get; set; }
    public DbSet<MaintenanceLog> MaintenanceLogs{ get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
