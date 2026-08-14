using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataBaseContext
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Define DbSet properties for your entities
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Application> Applications { get; set; } = null!;
        public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

        // Fluent API configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Ssn).IsRequired();
                entity.HasIndex(b => b.Ssn).IsUnique(); // We can't have two customers with the same SSN
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Address).IsRequired();
                entity.Property(e => e.State).IsRequired().HasMaxLength(2);
                entity.Property(e => e.CompanyName).IsRequired();
            });

            modelBuilder.Entity<Application>(entity =>
            {
                entity.ToTable("Applications");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RequestedAmount).IsRequired();
                entity.HasOne(e => e.Customer)
                  .WithOne(c => c.Application)
                  .HasForeignKey<Application>(a => a.CustomerId) //A customer can have only one application
                  .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OutboxMessage>(entity =>
            {
                entity.ToTable("OutboxMessages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EventType).IsRequired();
                entity.Property(e => e.Payload).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }
    }
}
