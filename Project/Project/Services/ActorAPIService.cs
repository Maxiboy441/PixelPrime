using System.Globalization;
using Newtonsoft.Json;
using Project.Data;
using Project.Models;

namespace Project.Services;

public class ActorAPIService
{
    private readonly string _apiKey;
    private readonly DataContext _context;
    private readonly HttpClient _httpClient;

    public ActorAPIService(HttpClient httpClient, IConfiguration config, DataContext context)
    {
        _httpClient = httpClient;
        _apiKey = config.GetValue<string>("Api:NinjaAPI") ?? throw new ArgumentNullException(nameof(_apiKey));
        _context = context;
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
    }

    public async Task<Actor> GetAndSaveActorAsync(string actorName)
    {
        var response =
            await _httpClient.GetAsync(
                $"https://api.api-ninjas.com/v1/celebrity?name={Uri.EscapeDataString(actorName)}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var celebrities = JsonConvert.DeserializeObject<List<CelebrityResponse>>(content);

        var actor = celebrities.FirstOrDefault(c => c.occupation.Contains("actor"));

        if (actor == null) throw new Exception($"No actor found with the name: {actorName}");

        var service = new WikipediaMediaAPIService();
        var imageUrl = await service.GetFirstImageUrlAsync(actorName);

        var newActor = new Actor
        {
            Name = ToTitleCase(actor.name),
            NetWorth = actor.net_worth,
            Gender = CapitalizeFirstLetter(actor.gender),
            Nationality = actor.nationality.ToUpper(),
            Height = (decimal)actor.height,
            Birthday = DateTime.Parse(actor.birthday),
            IsAlive = actor.is_alive,
            Occupations = string.Join(",", actor.occupation),
            Image = imageUrl
        };

        _context.Actors.Add(newActor);
        await _context.SaveChangesAsync();

        return newActor;
    }

    public static string ToTitleCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(input.ToLower());
    }
    
    public static string CapitalizeFirstLetter(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToUpper(value[0]) + value.Substring(1);
    }


    public class CelebrityResponse
    {
        public string name { get; set; }
        public long net_worth { get; set; }
        public string gender { get; set; }
        public string nationality { get; set; }
        public string[] occupation { get; set; }
        public double height { get; set; }
        public string birthday { get; set; }
        public int age { get; set; }
        public bool is_alive { get; set; }
    }
}