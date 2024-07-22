using System;
using System.Linq;
using Bogus;
using Project.Models;
using Project.Data;

namespace Project.Database.Factories
{
    public class FavoriteFactory
    {
        private readonly Faker<Favorite> _faker;
        private readonly DataContext _context;

        public FavoriteFactory(DataContext context)
        {
            _context = context;

            _faker = new Faker<Favorite>()
                .RuleFor(f => f.User_id, f => GetRandomUserId())
                .RuleFor(f => f.Movie_id, f => f.PickRandom(DummyData.movieIds))
                .RuleFor(f => f.Movie_title, f => f.Lorem.Words(3).Aggregate((a, b) => a + " " + b))
                .RuleFor(f => f.Movie_poster, f => f.PickRandom(DummyData.posters))
                .RuleFor(f => f.Created_at, f => f.Date.Past(2))
                .RuleFor(f => f.Updated_at, (f, fav) => f.Date.Between(fav.Created_at, DateTime.Now));
        }

        private int GetRandomUserId()
        {
            var userIds = _context.Set<User>().Select(u => u.Id).ToList();
            return userIds.Any() ? new Faker().PickRandom(userIds) : 1;
        }

        public Favorite Create()
        {
            return _faker.Generate();
        }

        public Favorite[] CreateMany(int count)
        {
            return _faker.Generate(count).ToArray();
        }

        public FavoriteFactory State(Action<Faker, Favorite> action)
        {
            _faker.FinishWith(action);
            return this;
        }
    }
}