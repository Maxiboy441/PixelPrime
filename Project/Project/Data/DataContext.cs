using System;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data
{
	public class DataContext : DbContext
    {
        public DbSet<Recommendation> Recommendations { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }
    }
}

