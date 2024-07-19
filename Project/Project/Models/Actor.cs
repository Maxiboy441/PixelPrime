using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Project.Models;

public class Actor
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Column(TypeName = "bigint")]
    public long NetWorth { get; set; }

    [StringLength(10)]
    public string Gender { get; set; }

    [StringLength(3)]
    public string Nationality { get; set; }

    [Column(TypeName = "decimal(3, 2)")]
    public decimal Height { get; set; }

    [Column(TypeName = "date")]
    public DateTime Birthday { get; set; }
    
    public bool IsAlive { get; set; }

    public string Occupations { get; set; }
    
    public string Image { get; set; }

}