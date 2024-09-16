using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project.Data;
using Project.Enums.Occupation;
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

    public async Task<Actor?> GetAndSaveActorAsync(string actorName)
    {
        var response =
            await _httpClient.GetAsync(
                $"https://api.api-ninjas.com/v1/celebrity?name={Uri.EscapeDataString(actorName)}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var celebrities = JsonConvert.DeserializeObject<List<CelebrityResponse>>(content);
    
        var service = new WikipediaMediaAPIService();
        var imageUrl = await service.GetFirstImageUrlAsync(actorName);

        if (celebrities == null || !celebrities.Any())
        {
            return null;
        }
        
        var celebrity = celebrities.FirstOrDefault(c => string.Equals(c.name, actorName, StringComparison.OrdinalIgnoreCase));

        if (celebrity == null)
        {
            celebrity = celebrities.First();
        }
        
        bool isOccupationInEnum = false;

        foreach (var occupation in celebrity.occupation)
        {
            if (OccupationEnumExtensions.GetOccupation(occupation) != null)
            {
                isOccupationInEnum = true;
                break;
            }
        }

        if (!isOccupationInEnum)
        {
            return null;
        }

        var newActor = new Actor
        {
            Name = ToTitleCase(celebrity.name),
            NetWorth = celebrity.net_worth,
            Gender = CapitalizeFirstLetter(celebrity.gender),
            Nationality = string.IsNullOrEmpty(celebrity.nationality) ? "NoN" : celebrity.nationality.ToUpper(),
            Height = (decimal)celebrity.height,
            Birthday = ParseBirthday(celebrity.birthday),
            IsAlive = celebrity.is_alive,
            Occupations = string.Join(",", celebrity.occupation),
            Image = imageUrl
        };

        _context.Actors.Add(newActor);

        if((await _context.Actors.FirstOrDefaultAsync(a => a.Name == newActor.Name)) == null)
        {
            await _context.SaveChangesAsync();
        }

        return newActor;
    }

    private DateTime ParseBirthday(string birthday)
    {
        if (DateTime.TryParse(birthday, out DateTime result))
        {
            return result;
        }

        return new DateTime(1001, 1, 1);
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