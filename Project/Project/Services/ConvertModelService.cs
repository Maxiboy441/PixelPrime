using System.Globalization;
using Project.Models;

// Usage:
// var convertService = new ConvertModelService();
// List<Rating> ratings = (List<Rating>)ViewData["TopRated"];
// List<Movie> moviesFromReviews = convertService.ConvertToMovies(ratings);

namespace Project.Services;

public class ConvertModelService
{
    public List<Movie> ConvertToMovies<T>(List<T>? items) where T : class
    {
        return items.Select(ConvertToMovie).ToList();
    }

    private static Movie ConvertToMovie<T>(T item) where T : class
    {
        if (item is Review review)
        {
            return new Movie
            {
                Id = review.Movie_id,
                Title = review.Movie_title,
                Poster = review.Movie_poster
            };
        }
        else if (item is Rating rating)
        {
            return new Movie
            {
                Id = rating.Movie_id,
                Title = rating.Movie_title,
                Poster = rating.Movie_poster,
                PixelRating = rating.Rating_value.ToString(CultureInfo.CurrentCulture)
            };
        }
        else if (item is Recommendation recommendation)
        {
            return new Movie
            {
                Id = recommendation.Movie_id,
                Title = recommendation.Movie_title,
                Poster = recommendation.Movie_poster
            };
        }
        else if (item is Watchlist watchlist)
        {
            return new Movie
            {
                Id = watchlist.Movie_id,
                Title = watchlist.Movie_title,
                Poster = watchlist.Movie_poster
            };
        }
        else if (item is Favorite favorites)
        {
            return new Movie
            {
                Id = favorites.Movie_id,
                Title = favorites.Movie_title,
                Poster = favorites.Movie_poster
            };
        }
        else
        {
            throw new ArgumentException($"Unsupported type: {typeof(T).Name}");
        }
    }
}