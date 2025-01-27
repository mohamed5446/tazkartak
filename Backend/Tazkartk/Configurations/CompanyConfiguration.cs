using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tazkartk.Models;

namespace Tazkartk.Configurations
{
    public class CompanyConfiguration: IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.Property(c => c.Name).HasMaxLength(50);
            builder.Property(c => c.City).HasMaxLength(50);
            builder.Property(c => c.Street).HasMaxLength(50);


            //1 company->m Trips
            builder.HasMany(c => c.Trips)
                 .WithOne(t => t.company)
                 .HasForeignKey(t => t.CompanyId);





        }
    }
}
