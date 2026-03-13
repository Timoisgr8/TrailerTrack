using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrailerTrack.Domain.Entities;

namespace TrailerTrack.Infrastructure.Persistence.Configurations;

public class HireEventConfiguration : IEntityTypeConfiguration<HireEvent>
{
    public void Configure(EntityTypeBuilder<HireEvent> builder)
    {
        builder.ToTable("hire_events");

        builder.HasKey(he => he.Id);

        builder.Property(he => he.AssetId)
            .IsRequired();

        builder.HasOne(he => he.Asset)
            .WithMany(a => a.HireEvents)
            .HasForeignKey(h => h.AssetId);

        builder.Property(he => he.EventType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(he => he.EventDate)
            .IsRequired();

        builder.Property(he => he.CreatedAt)
            .IsRequired();

        builder.Property(he => he.PerformedBy)
            .IsRequired();

        builder.Property(he => he.Customer)
            .IsRequired();

        builder.Property(he => he.CustomerContact)
            .IsRequired();

        builder.Property(he => he.Notes);
    }
}
