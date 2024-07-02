namespace Project.Models;

public class Rating
{
    public int Id { get; set; }
    public int User_id { get; set; }
    public string Movie_id { get; set; }
    public double Rating_value { get; set; }
}