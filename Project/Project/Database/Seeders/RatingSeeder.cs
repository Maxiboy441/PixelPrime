using Project.Models;
using Project.Database.Factories;
using Project.Data;

namespace Project.Database.Seeders
{
    public class RatingSeeder
    {
        private readonly DataContext _context;
        private readonly RatingFactory _ratingFactory;

        public RatingSeeder(DataContext context)
        {
            _context = context;
            _ratingFactory = new RatingFactory(context);
        }

        public void Seed(int count = 50)
        {
            if (!_context.Set<Rating>().Any())
            {
                var ratings = _ratingFactory.CreateMany(count);

                _context.Set<Rating>().AddRange(ratings);
                _context.SaveChanges();

                Console.WriteLine($"Seeded {ratings.Length} ratings.");
            }
            else
            {
                Console.WriteLine("Ratings table is not empty. Skipping seeding.");
            }
        }
    }
}