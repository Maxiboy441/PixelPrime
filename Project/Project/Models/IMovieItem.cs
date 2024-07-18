namespace Project.Models
{
    public interface IMovieItem
    {
        int Id { get; set; }
        int User_id { get; set; }
        string Movie_id { get; set; }
        string Movie_title { get; set; }
        string Movie_poster { get; set; }
        DateTime Created_at { get; set; }
        DateTime Updated_at { get; set; }
    }
}

