using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models;

public class Rating
{   
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int User_id { get; set; }
    public string Movie_id { get; set; }
    public double Rating_value { get; set; }
    public string Movie_title { get; set; }
    public string Movie_poster { get; set; }
    public DateTime Created_at { get; set; }
    public DateTime Updated_at { get; set; }

    [ForeignKey("User_id")]
    public virtual User User { get; set; }
}