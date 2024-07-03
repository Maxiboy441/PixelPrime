using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models;

public class Watchlist
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    // TODO: Add User_id as foreign key
    public int User_id { get; set; }
    public string Movie_id { get; set; }
    public string movie_title { get; set; }
    public string movie_poster { get; set; }
    public DateTime Created_at { get; set; }
    public DateTime Updated_at { get; set; }
}