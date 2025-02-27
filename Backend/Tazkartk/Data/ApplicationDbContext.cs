using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;
using Tazkartk.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Tazkartk.Data
{
    public class ApplicationDbContext: IdentityDbContext<Account, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            SeedRoles(builder);
            builder.Entity<Account>().UseTptMappingStrategy()
                .ToTable("AspNetAccounts");
            base.OnModelCreating(builder);

        }
        private void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole<int>>().HasData(
            new IdentityRole<int> { Id = 1, Name = Tazkartk.Models.Enums.Roles.Admin.ToString(), NormalizedName = Tazkartk.Models.Enums.Roles.Admin.ToString().ToUpper() },
            new IdentityRole<int> { Id = 2, Name = Tazkartk.Models.Enums.Roles.User.ToString(), NormalizedName = Tazkartk.Models.Enums.Roles.User.ToString().ToUpper() },
                new IdentityRole<int> { Id = 3, Name = Tazkartk.Models.Enums.Roles.Company.ToString(), NormalizedName = Tazkartk.Models.Enums.Roles.Company.ToString().ToUpper() }
            );
        }
        public DbSet<User> Users {  get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Booking> bookings { get; set; }
       







    }
}
