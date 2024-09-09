using System.Text.Json;
using Newtonsoft.Json;
using Project.Models;

namespace Project.Services
{
    public class MovieApiService
    {
        private static HttpClient _httpClient;
        private static string _baseApiUrl;
        private static string? _apiKey;

        public MovieApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = "http://www.omdbapi.com/";
            _apiKey = configuration.GetValue<string>("Api:MovieApi");
        }
        
        public async Task<string?> GetTrailerByImdb(string id)
        {
            var url = $"https://api.kinocheck.de/movies?imdb_id={id}&language=de&categories=Trailer";
            Console.WriteLine(url);
            HttpResponseMessage response;
        
            try 
            {
                response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
            } 
            catch (HttpRequestException e) 
            {
                Console.WriteLine($"Request to KinoCheck API failed: {e.Message}");
                return null;
            }
        
            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response: {content}");
        
            using JsonDocument doc = JsonDocument.Parse(content);
            JsonElement root = doc.RootElement;
        
            if (root.TryGetProperty("trailer", out JsonElement trailerElement) &&
                trailerElement.TryGetProperty("youtube_video_id", out JsonElement trailerYoutubeIdElement))
            {
                string? trailerYoutubeId = trailerYoutubeIdElement.GetString();
                if (!string.IsNullOrEmpty(trailerYoutubeId))
                {
                    return trailerYoutubeId;
                }
            }
        
            if (root.TryGetProperty("videos", out JsonElement videosElement) && videosElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var video in videosElement.EnumerateArray())
                {
                    if (video.TryGetProperty("youtube_video_id", out JsonElement videoYoutubeIdElement))
                    {
                        string? videoYoutubeId = videoYoutubeIdElement.GetString();
                        if (!string.IsNullOrEmpty(videoYoutubeId))
                        {
                            return videoYoutubeId;
                        }
                    }
                }
            }
        
            return null;
        }
        
        
        public async Task<Movie?> GetMovieById(string id)
        {
            var url = $"{_baseApiUrl}?i={id}&apikey={_apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request to OMDB API failed with status code: {response.StatusCode}");
            }

            string content = await response.Content.ReadAsStringAsync();

            if (IsErrorResponse(content, out string? errorMessage))
            {
                return null;
            }

            Movie movie = MappJsonToMovie(content);
            
            if (movie.Type != "movie" && movie.Type != "series")
            {
                return null;
            }
            return movie;
        }

        public async Task<Movie?> GetMovieByName(string name)
        {
            var url = $"{_baseApiUrl}?t={name}&apikey={_apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request to OMDB API failed with status code: {response.StatusCode}");
            }

            string content = await response.Content.ReadAsStringAsync();

            if (IsErrorResponse(content, out string? errorMessage))
            {
                return null;
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
                PixelRating = root.GetProperty("imdbRating").GetString(),
                Type = root.GetProperty("Type").GetString()
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