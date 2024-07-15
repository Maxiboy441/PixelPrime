using Project.Models;
using Project.Database.Factories;
using Project.Data;

namespace Project.Database.Seeders
{
    public class ReviewSeeder
    {
        private readonly DataContext _context;
        private readonly ReviewFactory _reviewFactory;

        public ReviewSeeder(DataContext context)
        {
            _context = context;
            _reviewFactory = new ReviewFactory(context);
        }

        public void Seed(int count = 30)
        {
            if (!_context.Set<Review>().Any())
            {
                var reviews = _reviewFactory.CreateMany(count);

                _context.Set<Review>().AddRange(reviews);
                _context.SaveChanges();

                Console.WriteLine($"Seeded {reviews.Length} reviews.");
            }
            else
            {
                Console.WriteLine("Reviews table is not empty. Skipping seeding.");
            }
        }
    }
}