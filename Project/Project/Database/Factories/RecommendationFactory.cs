using Bogus;
using Project.Models;
using Project.Data;

namespace Project.Database.Factories
{
    public class RecommendationFactory
    {
        private readonly Faker<Recommendation> _faker;
        private readonly DataContext _context;

        public RecommendationFactory(DataContext context)
        {
            _context = context;

            _faker = new Faker<Recommendation>()
                .RuleFor(r => r.User_id, f => GetRandomUserId())
                .RuleFor(f => f.Movie_id, f => f.PickRandom(DummyData.movieIds))
                .RuleFor(r => r.Movie_title, f => f.Lorem.Words(3).Aggregate((a, b) => a + " " + b))
                .RuleFor(f => f.Movie_poster, f => f.PickRandom(DummyData.posters))
                .RuleFor(r => r.Created_at, f => f.Date.Past(1))
                .RuleFor(r => r.Updated_at, (f, r) => f.Date.Between(r.Created_at, DateTime.Now));
        }

        private int GetRandomUserId()
        {
            var userIds = _context.Set<User>().Select(u => u.Id).ToList();
            return userIds.Any() ? new Faker().PickRandom(userIds) : 1;
        }

        public Recommendation Create()
        {
            return _faker.Generate();
        }

        public Recommendation[] CreateMany(int count)
        {
            return _faker.Generate(count).ToArray();
        }

        public RecommendationFactory State(Action<Faker, Recommendation> action)
        {
            _faker.FinishWith(action);
            return this;
        }
    }
}