﻿namespace Project.Models.ViewModels
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public List<IMovieItem> Favorites { get; set; }
        public List<IMovieItem> Watchlist { get; set; }
        public List<IMovieItem> Recommendations { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Rating> Ratings { get; set; }
    }
}
