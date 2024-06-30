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

        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }
    }
}

