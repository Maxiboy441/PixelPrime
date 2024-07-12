using System;
using System.Linq;
using Project.Models;
using Bogus;

namespace Project.Database.Factories
{
    public class UserFactory
    {
        private readonly Faker<User> _faker;

        public UserFactory()
        {
            _faker = new Faker<User>()
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name))
                .RuleFor(u => u.Password, f => f.Internet.Password(8))
                .RuleFor(u => u.PasswordConfirmation, (f, u) => u.Password)
                .RuleFor(u => u.Description, f => f.Lorem.Paragraph())
                .RuleFor(u => u.Avatar, f => f.Internet.Avatar())
                .RuleFor(u => u.Created_at, f => f.Date.Past(2))
                .RuleFor(u => u.Updated_at, (f, u) => f.Date.Between(u.Created_at, DateTime.Now));
        }

        public User Create()
        {
            return _faker.Generate();
        }

        public User[] CreateMany(int count)
        {
            return _faker.Generate(count).ToArray();
        }

        public UserFactory State(Action<Faker, User> action)
        {
            _faker.FinishWith(action);
            return this;
        }
    }
}