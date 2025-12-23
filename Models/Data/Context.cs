using Microsoft.EntityFrameworkCore;
using FitTrack.Models;

namespace FitTrack.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Profiles> Profiles { get; set; }
        public DbSet<Exercises> Exercises { get; set; }
        public DbSet<UserMetrics> UserMetrics { get; set; }
        public DbSet<UserWorkout> UserWorkouts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .Property(u => u.creation_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Profiles>()
                .Property(u => u.creation_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Exercises>()
                .Property(u => u.creation_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<UserWorkout>()
                .Property(u => u.creation_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
