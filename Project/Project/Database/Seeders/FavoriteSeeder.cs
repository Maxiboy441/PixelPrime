using Project.Models;
using Project.Database.Factories;
using Project.Data;

namespace Project.Database.Seeders
{
    public class FavoriteSeeder
    {
        private readonly DataContext _context;
        private readonly FavoriteFactory _favoriteFactory;

        public FavoriteSeeder(DataContext context)
        {
            _context = context;
            _favoriteFactory = new FavoriteFactory(context);
        }

        public void Seed(int count = 15)
        {
            if (!_context.Set<Favorite>().Any())
            {
                var favorites = _favoriteFactory.CreateMany(count);

                _context.Set<Favorite>().AddRange(favorites);
                _context.SaveChanges();

                Console.WriteLine($"Seeded {favorites.Length} favorites.");
            }
            else
            {
                Console.WriteLine("Favorites table is not empty. Skipping seeding.");
            }
        }
    }
}