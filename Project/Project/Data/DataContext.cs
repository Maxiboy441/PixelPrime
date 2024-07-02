using System;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data
{
	public class DataContext : DbContext
    {
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rating>()
                .HasKey(p => p.Id);
        
            modelBuilder.Entity<Rating>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
        
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is Rating)
                {
                    var now = DateTime.UtcNow;
                    if (entry.State == EntityState.Added)
                    {
                        entry.Property("CreatedAt").CurrentValue = now;
                        entry.Property("UpdatedAt").CurrentValue = now;
                    }
                    if (entry.State == EntityState.Modified)
                    {
                        entry.Property("UpdatedAt").CurrentValue = now;
                    }
                }
            }
            return base.SaveChanges();
        }
    }
}

