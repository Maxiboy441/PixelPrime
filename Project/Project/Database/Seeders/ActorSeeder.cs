using Project.Data;
using Project.Database.Factories;
using Project.Models;

namespace Project.Database.Seeders
{
    public class ActorSeeder
    {
        private readonly DataContext _context;
        private readonly ActorFactory _actorFactory;

        public ActorSeeder(DataContext context)
        {
            _context = context;
            _actorFactory = new ActorFactory();
        }

        public void Seed(int count = 20)
        {
            if (!_context.Set<Actor>().Any())
            {
                var actors = _actorFactory.CreateMany(count);

                var famousActor = _actorFactory
                    .State((f, a) =>
                    {
                        a.Name = "Tom Hanks";
                        a.NetWorth = 400000000;
                        a.Gender = "Male";
                        a.Nationality = "USA";
                        a.Height = 1.83m;
                        a.Birthday = new DateTime(1956, 7, 9);
                        a.IsAlive = true;
                        a.Occupations = "Actor,Director,Producer";
                        a.Image = "https://example.com/tom-hanks.jpg";
                    })
                    .Create();

                actors = actors.Concat(new[] { famousActor }).ToArray();

                _context.Set<Actor>().AddRange(actors);
                _context.SaveChanges();

                Console.WriteLine($"Seeded {actors.Length} actors.");
            }
            else
            {
                Console.WriteLine("Actors table is not empty. Skipping seeding.");
            }
        }
    }
}