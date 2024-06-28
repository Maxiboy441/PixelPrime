using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Watchlist
    {
        [Key]
        public int WatchlistId { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public int MovieId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
