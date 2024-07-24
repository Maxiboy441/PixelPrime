using Microsoft.EntityFrameworkCore;
using Project.Models;
using Project.Database.Factories;
using Project.Data;

namespace Project.Database.Seeders
{
    public class WatchlistSeeder
    {
        private readonly DataContext _context;
        private readonly WatchlistFactory _watchlistFactory;

        public WatchlistSeeder(DataContext context)
        {
            _context = context;
            _watchlistFactory = new WatchlistFactory(context);
        }

        public void Seed(int count = 20)
        {
            if (!_context.Set<Watchlist>().Any())
            {
                var watchlistItems = _watchlistFactory.CreateMany(count);

                _context.Set<Watchlist>().AddRange(watchlistItems);
                _context.SaveChanges();

                Console.WriteLine($"Seeded {watchlistItems.Length} watchlist items.");
            }
            else
            {
                Console.WriteLine("Watchlist table is not empty. Skipping seeding.");
            }
        }
    }
}