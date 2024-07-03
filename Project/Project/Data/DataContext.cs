using System;
using Microsoft.EntityFrameworkCore;
using Project.Areas.Identity.Data;
using Project.Models;

namespace Project.Data
{
	public class DataContext : DbContext
    {
        public DbSet<User> User { get; set; }
        
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }

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
            
            modelBuilder.Entity<Favorite>()
                .HasKey(p => p.Id);
        
            modelBuilder.Entity<Favorite>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<Watchlist>()
                .HasKey(p => p.Id);
        
            modelBuilder.Entity<Watchlist>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
        
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is Review || entry.Entity is Recommendation || entry.Entity is Rating || entry.Entity is Favorite || entry.Entity is Watchlist)
                {
                    var now = DateTime.UtcNow;
                    if (entry.State == EntityState.Added)
                    {
                        entry.Property("Created_at").CurrentValue = now;
                        entry.Property("Updated_at").CurrentValue = now;
                    }
                    if (entry.State == EntityState.Modified)
                    {
                        entry.Property("Updated_at").CurrentValue = now;
                    }
                }
            }
            return base.SaveChanges();
        }
    }
}

