using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tazkartk.Domain.Models;

namespace Tazkartk.Infrastructure.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            // 1 booking -> m seat
            builder.HasMany(b => b.seats)
                .WithOne(s => s.booking)
                .HasForeignKey(s => s.bookingId);

            // 1 booking -> 1 payment 
            builder.HasOne(b => b.payment)
                .WithOne(p => p.booking)
                .HasForeignKey<Payment>(b => b.bookingId);
        }
    }
}
