using System;
using System.Linq;
using Project.Models;
using Bogus;

namespace Project.Database.Factories
{
    public class ActorFactory
    {
        private readonly Faker<Actor> _faker;
        private readonly string[] _actorImages;

        public ActorFactory()
        {
            _actorImages = new[]
            {
                "https://m.media-amazon.com/images/M/MV5BMTQ2MjMwNDA3Nl5BMl5BanBnXkFtZTcwMTA2NDY3NQ@@._V1_UY317_CR2,0,214,317_AL_.jpg", // Tom Hanks
                "https://m.media-amazon.com/images/M/MV5BMjA1MTQ3NzU1MV5BMl5BanBnXkFtZTgwMDE3Mjg0MzE@._V1_UY317_CR52,0,214,317_AL_.jpg", // Meryl Streep
                "https://m.media-amazon.com/images/M/MV5BMjI0MTg3MzI0M15BMl5BanBnXkFtZTcwMzQyODU2Mw@@._V1_UY317_CR10,0,214,317_AL_.jpg", // Leonardo DiCaprio
                "https://m.media-amazon.com/images/M/MV5BODg3MzYwMjE4N15BMl5BanBnXkFtZTcwMjU5NzAzNw@@._V1_UY317_CR22,0,214,317_AL_.jpg", // Angelina Jolie
                "https://m.media-amazon.com/images/M/MV5BMTk1MjM3NTU5M15BMl5BanBnXkFtZTcwMTMyMjAyMg@@._V1_UY317_CR14,0,214,317_AL_.jpg", // Denzel Washington
                "https://m.media-amazon.com/images/M/MV5BMTc1MDI0MDg1NV5BMl5BanBnXkFtZTgwMDM3OTAzMTE@._V1_UY317_CR3,0,214,317_AL_.jpg", // Julia Roberts
                "https://m.media-amazon.com/images/M/MV5BMjExNzA4MDYxN15BMl5BanBnXkFtZTcwOTI1MDAxOQ@@._V1_UY317_CR7,0,214,317_AL_.jpg", // Matt Damon
                "https://m.media-amazon.com/images/M/MV5BMTkxMzk4MjQ4MF5BMl5BanBnXkFtZTcwMzExODQxOA@@._V1_UX214_CR0,0,214,317_AL_.jpg", // Charlize Theron
                "https://m.media-amazon.com/images/M/MV5BMTM2ODk0NDAwMF5BMl5BanBnXkFtZTcwNTM1MTc2Mw@@._V1_UY317_CR1,0,214,317_AL_.jpg", // Robert Downey Jr.
                "https://m.media-amazon.com/images/M/MV5BMjExNjY5NDY0MV5BMl5BanBnXkFtZTgwNjQ1Mjg1MTI@._V1_UY317_CR20,0,214,317_AL_.jpg"  // Emma Stone
            };

            _faker = new Faker<Actor>()
                .RuleFor(a => a.Name, f => f.Name.FullName())
                .RuleFor(a => a.NetWorth, f => f.Random.Long(100000, 1000000000))
                .RuleFor(a => a.Gender, f => f.PickRandom("Male", "Female", "Other"))
                .RuleFor(a => a.Nationality, f => f.Address.CountryCode())
                .RuleFor(a => a.Height, f => f.Random.Decimal(1.50m, 2.10m))
                .RuleFor(a => a.Birthday, f => f.Date.Past(80))
                .RuleFor(a => a.IsAlive, f => f.Random.Bool(0.9f))
                .RuleFor(a => a.Occupations, f => string.Join(",", f.Lorem.Words(f.Random.Int(1, 3))))
                .RuleFor(a => a.Image, f => f.PickRandom(_actorImages));
        }

        public Actor Create()
        {
            return _faker.Generate();
        }

        public Actor[] CreateMany(int count)
        {
            return _faker.Generate(count).ToArray();
        }

        public ActorFactory State(Action<Faker, Actor> action)
        {
            _faker.FinishWith(action);
            return this;
        }
    }
}