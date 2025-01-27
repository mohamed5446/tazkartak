using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tazkartk.Models;

namespace Tazkartk.Configurations
{
    public class UserConfiguration: IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {

            builder.Property(u => u.FirstName).HasMaxLength(50);
            builder.Property(u => u.LastName).HasMaxLength(50);

            // 1 user -> m bookings
            builder.HasMany(u => u.books)
                .WithOne(b => b.user)
                .HasForeignKey(b => b.UserId);
        }
    }
}
