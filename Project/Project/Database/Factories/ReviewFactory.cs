using Bogus;
using Project.Models;
using Project.Data;

namespace Project.Database.Factories
{
    public class ReviewFactory
    {
        private readonly Faker<Review> _faker;
        private readonly DataContext _context;
        private readonly string[] _posters;

        public ReviewFactory(DataContext context)
        {
            _context = context;

            _faker = new Faker<Review>()
                .RuleFor(r => r.User_id, f => GetRandomUserId())
                .RuleFor(f => f.Movie_id, f => f.PickRandom(DummyData.movieIds))
                .RuleFor(r => r.Title, f => f.Lorem.Sentence())
                .RuleFor(r => r.Text, f => f.Lorem.Paragraphs(3))
                .RuleFor(r => r.Movie_title, f => f.Lorem.Words(3).Aggregate((a, b) => a + " " + b))
                .RuleFor(f => f.Movie_poster, f => f.PickRandom(DummyData.posters))
                .RuleFor(r => r.Created_at, f => f.Date.Past(2))
                .RuleFor(r => r.Updated_at, (f, r) => f.Date.Between(r.Created_at, DateTime.Now));
        }

        private int GetRandomUserId()
        {
            var userIds = _context.Set<User>().Select(u => u.Id).ToList();
            return userIds.Any() ? new Faker().PickRandom(userIds) : 1;
        }

        public Review Create()
        {
            return _faker.Generate();
        }

        public Review[] CreateMany(int count)
        {
            return _faker.Generate(count).ToArray();
        }

        public ReviewFactory State(Action<Faker, Review> action)
        {
            _faker.FinishWith(action);
            return this;
        }
    }
}