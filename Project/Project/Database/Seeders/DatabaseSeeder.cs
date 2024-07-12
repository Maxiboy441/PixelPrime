using Project.Data;

namespace Project.Database.Seeders
{
    public class DatabaseSeeder
    {
        private readonly DataContext _context;

        public DatabaseSeeder(DataContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            SeedUsers();
            SeedRatings();
            SeedReviews();
            SeedWatchlists();
            SeedFavorites();
            SeedRecommendations();
        }

        private void SeedUsers()
        {
            var userSeeder = new UserSeeder(_context);
            userSeeder.Seed();
        }
        
        private void SeedRatings()
        {
            var userSeeder = new RatingSeeder(_context);
            userSeeder.Seed();
        }
        
        private void SeedReviews()
        {
            var userSeeder = new ReviewSeeder(_context);
            userSeeder.Seed();
        }
        
        private void SeedWatchlists()
        {
            var userSeeder = new WatchlistSeeder(_context);
            userSeeder.Seed();
        }
        
        private void SeedFavorites()
        {
            var userSeeder = new FavoriteSeeder(_context);
            userSeeder.Seed();
        }
        
        private void SeedRecommendations()
        {
            var userSeeder = new RecommendationSeeder(_context);
            userSeeder.Seed();
        }
    }
}