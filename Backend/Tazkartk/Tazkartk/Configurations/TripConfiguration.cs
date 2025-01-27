using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tazkartk.Models;

namespace Tazkartk.Configurations
{
    public class TripConfiguration: IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.Property(t => t.From).HasMaxLength(50);
            builder.Property(t => t.To).HasMaxLength(50);
            builder.Property(t => t.Class).HasMaxLength(50);
            builder.Property(t=>t.Location).HasMaxLength(100);
            // trip code ?

            // 1 trip -> m seats 
            builder.HasMany(t => t.seats)
                .WithOne(s => s.trip)
                .HasForeignKey(s => s.TripId);
        }
    }
}
