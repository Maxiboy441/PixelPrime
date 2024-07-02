using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class Watchlist
    {
        [Key]
        public int WatchlistId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Movie")]
        public string MovieId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
