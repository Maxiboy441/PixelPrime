using System;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data
{
	public class DataContext : DbContext
    {
        public DbSet<Recommendation> Recommendations { get; set; }
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
            
            modelBuilder.Entity<Review>()
                .HasKey(p => p.Id);
        
            modelBuilder.Entity<Review>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}

