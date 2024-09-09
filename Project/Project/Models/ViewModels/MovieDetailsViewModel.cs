namespace Project.Models.ViewModels
{
    public class MovieDetailsViewModel
    {
        public Movie? Movie { get; set; }
        public List<Review>? Reviews { get; set; }
        public string? AverageRating { get; set; }
        public bool HasAverageRating { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsWatchlist { get; set; }
        public bool IsRated { get; set; }
        public bool UserHasRating { get; set; }
        public string? CurrentUserRating { get; set; }
        public int? CurrentUserId { get; set; }
        public bool UserHasReview { get; set; }
        
        public string? MovieTrailer { get; set; }
    }
}