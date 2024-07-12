using Project.Models;
using Project.Database.Factories;
using Project.Data;

namespace Project.Database.Seeders
{
    public class RecommendationSeeder
    {
        private readonly DataContext _context;
        private readonly RecommendationFactory _recommendationFactory;

        public RecommendationSeeder(DataContext context)
        {
            _context = context;
            _recommendationFactory = new RecommendationFactory(context);
        }

        public void Seed(int count = 25)
        {
            if (!_context.Set<Recommendation>().Any())
            {
                var recommendations = _recommendationFactory.CreateMany(count);

                _context.Set<Recommendation>().AddRange(recommendations);
                _context.SaveChanges();

                Console.WriteLine($"Seeded {recommendations.Length} recommendations.");
            }
            else
            {
                Console.WriteLine("Recommendations table is not empty. Skipping seeding.");
            }
        }
    }
}