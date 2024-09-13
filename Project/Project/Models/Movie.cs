﻿namespace Project.Models
{
    public class Movie
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Year { get; set; }
        public string? Runtime { get; set; }
        public string? Genre { get; set; }
        public string? Director { get; set; }
        public string? Writer { get; set; }
        public string? Actors { get; set; }
        public string? Plot { get; set; }
        public string? Language { get; set; }
        public string? Country { get; set; }
        public string? Awards { get; set; }
        public string? Poster { get; set; }
        
        /* TODO: Change string to double when using our own rating*/
        public string? PixelRating { get; set; }
        
        public string? Type { get; set; }
    }
}

