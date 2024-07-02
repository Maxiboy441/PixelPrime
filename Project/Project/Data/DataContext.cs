using System;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data
{
	public class DataContext : DbContext
    {
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }
        public DbSet<Favorites> Favorites { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recommendation>()
                .HasKey(p => p.Id);
        
            modelBuilder.Entity<Recommendation>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<Rating>()
                .HasKey(p => p.Id);
        
            modelBuilder.Entity<Rating>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<Review>()
                .HasKey(p => p.Id);
        
            modelBuilder.Entity<Review>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
        
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is Review || entry.Entity is Recommendation || entry.Entity is Rating)
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

