using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrailerTrack.Domain.Entities;

namespace TrailerTrack.Infrastructure.Persistence.Configurations;

public class MaintenanceLogConfiguration : IEntityTypeConfiguration<MaintenanceLog>
{
    public void Configure(EntityTypeBuilder<MaintenanceLog> builder)
    {
        builder.ToTable("maintenance_logs");

        builder.HasKey(ml => ml.Id);

        builder.Property(ml => ml.Description)
            .IsRequired();

        builder.HasOne(ml => ml.Asset)
            .WithMany(ml => ml.MaintenanceLogs)
            .HasForeignKey(ml => ml.AssetId);

        builder.Property(ml => ml.CreatedAt)
            .IsRequired();

        builder.Property(ml => ml.StartedAt)
            .IsRequired();

        builder.Property(ml => ml.ExpectedCompletionAt);

        builder.Property(ml => ml.CompletedAt);

        builder.Property(ml => ml.CompletedNotes);

        builder.Property(ml => ml.PerformedBy)
            .IsRequired();

        builder.Property(ml => ml.Cost);
    }
}
