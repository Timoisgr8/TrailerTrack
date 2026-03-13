using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrailerTrack.Domain.Entities;

namespace TrailerTrack.Infrastructure.Persistence.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("assets");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AssetCode)
            .IsRequired();

        builder.Property(a => a.Location)
            .IsRequired();

        builder.Property(a => a.LastServicedAt);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired();

        builder.HasMany(a => a.HireEvents)
            .WithOne(h => h.Asset)
            .HasForeignKey(h => h.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(a => a.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.HasMany(a => a.MaintenanceLogs)
            .WithOne(h => h.Asset)
            .HasForeignKey(h => h.AssetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
