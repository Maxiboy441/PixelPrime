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
            var ratingSeeder = new RatingSeeder(_context);
            ratingSeeder.Seed();
        }
        
        private void SeedReviews()
        {
            var reviewSeeder = new ReviewSeeder(_context);
            reviewSeeder.Seed();
        }
        
        private void SeedWatchlists()
        {
            var watchlistSeeder = new WatchlistSeeder(_context);
            watchlistSeeder.Seed();
        }
        
        private void SeedFavorites()
        {
            var favoriteSeeder = new FavoriteSeeder(_context);
            favoriteSeeder.Seed();
        }
        
        private void SeedRecommendations()
        {
            var recommendationSeeder = new RecommendationSeeder(_context);
            recommendationSeeder.Seed();
        }
    }
}