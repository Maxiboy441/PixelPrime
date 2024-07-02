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

        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }
    }
}

