namespace Project.Models
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public List<IMovieItem> Favorites { get; set; }
        public List<IMovieItem> Watchlist { get; set; }
        public List<IMovieItem> Recommendations { get; set; }
    }
}
