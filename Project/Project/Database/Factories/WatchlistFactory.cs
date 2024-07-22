using Bogus;
using Project.Models;
using Project.Data;

namespace Project.Database.Factories
{
    public class WatchlistFactory
    {
        private readonly Faker<Watchlist> _faker;
        private readonly DataContext _context;

        public WatchlistFactory(DataContext context)
        {
            _context = context;

            _faker = new Faker<Watchlist>()
                .RuleFor(w => w.User_id, f => GetRandomUserId())
                .RuleFor(f => f.Movie_id, f => f.PickRandom(DummyData.movieIds))
                .RuleFor(w => w.Movie_title, f => f.Lorem.Words(3).Aggregate((a, b) => a + " " + b))
                .RuleFor(f => f.Movie_poster, f => f.PickRandom(DummyData.posters))
                .RuleFor(w => w.Created_at, f => f.Date.Past(2))
                .RuleFor(w => w.Updated_at, (f, w) => f.Date.Between(w.Created_at, DateTime.Now));
        }

        private int GetRandomUserId()
        {
            var userIds = _context.Set<User>().Select(u => u.Id).ToList();
            return userIds.Any() ? new Faker().PickRandom(userIds) : 1;
        }

        public Watchlist Create()
        {
            return _faker.Generate();
        }

        public Watchlist[] CreateMany(int count)
        {
            return _faker.Generate(count).ToArray();
        }

        public WatchlistFactory State(Action<Faker, Watchlist> action)
        {
            _faker.FinishWith(action);
            return this;
        }
    }
}