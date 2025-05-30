using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tazkartk.Domain.Models;

namespace Tazkartk.Infrastructure.Configurations
{
    public class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            // seat has composite key of seatid+tripid
            builder.HasKey(s => new { s.Number, s.TripId });

        }
    }
}
