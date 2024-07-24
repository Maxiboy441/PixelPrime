using Project.Data;
using Project.Database.Factories;
using Project.Models;

namespace Project.Database.Seeders
{
    public class UserSeeder
    {
        private readonly DataContext _context;
        private readonly UserFactory _userFactory;

        public UserSeeder(DataContext context)
        {
            _context = context;
            _userFactory = new UserFactory();
        }

        public void Seed(int count = 10)
        {
            if (!_context.Set<User>().Any())
            {
                var users = _userFactory.CreateMany(count);

                var adminUser = _userFactory
                    .State((f, u) =>
                    {
                        u.Email = "admin@example.com";
                        u.Name = "Admin User";
                        u.Password = "adminpassword";
                        u.PasswordConfirmation = "adminpassword";
                    })
                    .Create();

                users = users.Concat(new[] { adminUser }).ToArray();

                _context.Set<User>().AddRange(users);
                _context.SaveChanges();

                Console.WriteLine($"Seeded {users.Length} users.");
            }
            else
            {
                Console.WriteLine("Users table is not empty. Skipping seeding.");
            }
        }
    }
}