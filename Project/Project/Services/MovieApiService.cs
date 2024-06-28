using System.Text.Json;
using Project.Models;

namespace Project.Services
{
    public static class MovieApiService
    {
        private static readonly HttpClient HttpClient = new();
        private const string BaseApiUrl = $"http://www.omdbapi.com/";
        private static readonly IConfigurationRoot Myconfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        private static readonly string? ApiKey = Myconfig.GetValue<string>("Api:MovieApi");


        public static async Task<Movie> GetMovieById(string id)
        {
            var url = $"{BaseApiUrl}?i={id}&apikey={ApiKey}";

            HttpResponseMessage response = await HttpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request to OMDB API failed with status code: {response.StatusCode}");
            }

            string content = await response.Content.ReadAsStringAsync();

            if (IsErrorResponse(content, out string? errorMessage))
            {
                throw new Exception(errorMessage);
            }

            Movie movie = MappJsonToMovie(content);
            return movie;
        }

        public static async Task<Movie> GetMovieByName(string name)
        {
            var url = $"{BaseApiUrl}?t={name}&apikey={ApiKey}";

            HttpResponseMessage response = await HttpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request to OMDB API failed with status code: {response.StatusCode}");
            }

            string content = await response.Content.ReadAsStringAsync();

            if (IsErrorResponse(content, out string? errorMessage))
            {
                throw new Exception(errorMessage);
            }

            Movie movie = MappJsonToMovie(content);
            return movie;
        }

        private static Movie MappJsonToMovie(string jsonString)
        {
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            JsonElement root = doc.RootElement;

            Movie movie = new Movie
            {
                Id = root.GetProperty("imdbID").GetString(), 
                Title = root.GetProperty("Title").GetString(),
                Year = root.GetProperty("Year").GetString(),
                Runtime = root.GetProperty("Runtime").GetString(),
                Genre = root.GetProperty("Genre").GetString(),
                Director = root.GetProperty("Director").GetString(),
                Writer = root.GetProperty("Writer").GetString(),
                Actors = root.GetProperty("Actors").GetString(),
                Plot = root.GetProperty("Plot").GetString(),
                Language = root.GetProperty("Language").GetString(),
                Country = root.GetProperty("Country").GetString(),
                Awards = root.GetProperty("Awards").GetString(),
                Poster = root.GetProperty("Poster").GetString(),
                /*TODO: Replace the PixelRating from the DB if it exists*/
                PixelRating = root.GetProperty("imdbRating").GetString()
            };

            return movie;
        }

        private static bool IsErrorResponse(string jsonString, out string? errorMessage)
        {
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("Response", out JsonElement responseElement) && responseElement.GetString() == "False")
            {
                errorMessage = root.GetProperty("Error").GetString();
                return true;
            }

            errorMessage = null;
            return false;
        }
    }
}
