using Microsoft.EntityFrameworkCore;
using SofijaFesis_5DanaUOblacima.Models;

namespace SofijaFesis_5DanaUOblacima.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Canteen> Canteens { get; set; }
        public DbSet<WorkingHour> WorkingHours { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Student
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.IsAdmin).HasDefaultValue(false);
                
                entity.HasMany(s => s.Reservations)
                    .WithOne(r => r.Student)
                    .HasForeignKey(r => r.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Canteen>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Location).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Capacity).IsRequired().HasDefaultValue(50);
                
                entity.HasMany(c => c.WorkingHours)
                    .WithOne(w => w.Canteen)
                    .HasForeignKey(w => w.CanteenId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(c => c.Reservations)
                    .WithOne(r => r.Canteen)
                    .HasForeignKey(r => r.CanteenId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WorkingHour>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Meal).IsRequired().HasMaxLength(50);
                entity.Property(e => e.From).IsRequired().HasMaxLength(5);
                entity.Property(e => e.To).IsRequired().HasMaxLength(5);
                entity.Property(e => e.CanteenId).IsRequired();
            });
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Status).IsRequired().HasDefaultValue("Active");
                entity.Property(e => e.StudentId).IsRequired();
                entity.Property(e => e.CanteenId).IsRequired();
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Time).IsRequired().HasMaxLength(5);  // HH:mm
                entity.Property(e => e.Duration).IsRequired().HasDefaultValue(30);
                
                entity.HasIndex(e => new { e.CanteenId, e.Date, e.Time });
                entity.HasIndex(e => e.StudentId);
            });
        }
    }
}