using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.Models;

public class Review
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    // TODO: Add User_id as foreign key
    public int User_id { get; set; }
    public string Movie_id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public string Movie_title { get; set; }
    public string Movie_poster { get; set; }
    public DateTime Created_at { get; set; }
    public DateTime Updated_at { get; set; }

}