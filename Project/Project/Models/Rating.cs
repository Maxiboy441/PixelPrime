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
}