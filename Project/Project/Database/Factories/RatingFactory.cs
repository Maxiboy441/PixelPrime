using Bogus;
using Project.Models;
using Project.Data;

namespace Project.Database.Factories
{
    public class RatingFactory
    {
        private readonly Faker<Rating> _faker;
        private readonly DataContext _context;
        private readonly string[] _posters;

        public RatingFactory(DataContext context)
        {
            _context = context;
            _posters = new[]
            {
                "https://m.media-amazon.com/images/M/MV5BZWI1OWM3ZmEtNjQ2OS00NzI2LTgwNWMtZDAyMGI1OTM2MzJmXkEyXkFqcGdeQXVyNjc5NjEzNA@@._V1_SX300.jpg",
                "https://m.media-amazon.com/images/M/MV5BYmZkYWRlNWQtOGY0Zi00MWZkLWJiZTktNjRjMDY4MTU2YzAyXkEyXkFqcGdeQXVyMzYzNzc1NjY@._V1_SX300.jpg",
                "https://m.media-amazon.com/images/M/MV5BOTk5ODg0OTU5M15BMl5BanBnXkFtZTgwMDQ3MDY3NjM@._V1_SX300.jpg",
                "https://m.media-amazon.com/images/M/MV5BMjIxODgxNTE5N15BMl5BanBnXkFtZTcwODM0MTM3Mg@@._V1_SX300.jpg",
                "https://m.media-amazon.com/images/M/MV5BM2YwYTkwNjItNGQzNy00MWE1LWE1M2ItOTMzOGI1OWQyYjA0XkEyXkFqcGdeQXVyMTUzMTg2ODkz._V1_SX300.jpg",
                "https://m.media-amazon.com/images/M/MV5BNWIwODRlZTUtY2U3ZS00Yzg1LWJhNzYtMmZiYmEyNmU1NjMzXkEyXkFqcGdeQXVyMTQxNzMzNDI@._V1_SX300.jpg",
                "https://m.media-amazon.com/images/M/MV5BYjBkM2RjMzItM2M3Ni00N2NjLWE3NzMtMGY4MzE4MDAzMTRiXkEyXkFqcGdeQXVyNDUzOTQ5MjY@._V1_SX300.jpg",
                "https://m.media-amazon.com/images/M/MV5BY2Q0ODg4ZmItNDZiYi00ZWY5LTg2NzctNmYwZjA5OThmNzE1XkEyXkFqcGdeQXVyMjM4MzQ4OTQ@._V1_SX300.jpg",
                "https://m.media-amazon.com/images/M/MV5BYjU5YTA5OGItYzFmZi00YmFjLWJjNWEtZTk0MjExMTFjYzE2XkEyXkFqcGdeQXVyNjcwMzExMzU@._V1_SX300.jpg",
                "https://m.media-amazon.com/images/M/MV5BNzA5ZDNlZWMtM2NhNS00NDJjLTk4NDItYTRmY2EwMWZlMTY3XkEyXkFqcGdeQXVyNzkwMjQ5NzM@._V1_SX300.jpg"
            };

            _faker = new Faker<Rating>()
                .RuleFor(r => r.User_id, f => GetRandomUserId())
                .RuleFor(r => r.Movie_id, f => f.Random.AlphaNumeric(10))
                .RuleFor(r => r.Rating_value, f => Math.Round(f.Random.Double(0.1, 10.0), 1))
                .RuleFor(r => r.Movie_title, f => f.Lorem.Words(3).Aggregate((a, b) => a + " " + b))
                .RuleFor(r => r.Movie_poster, f => f.PickRandom(_posters))
                .RuleFor(r => r.Created_at, f => f.Date.Past(2))
                .RuleFor(r => r.Updated_at, (f, r) => f.Date.Between(r.Created_at, DateTime.Now));
        }

        private int GetRandomUserId()
        {
            var userIds = _context.Set<User>().Select(u => u.Id).ToList();
            return userIds.Any() ? new Faker().PickRandom(userIds) : 1;
        }

        public Rating Create()
        {
            return _faker.Generate();
        }

        public Rating[] CreateMany(int count)
        {
            return _faker.Generate(count).ToArray();
        }

        public RatingFactory State(Action<Faker, Rating> action)
        {
            _faker.FinishWith(action);
            return this;
        }
    }
}