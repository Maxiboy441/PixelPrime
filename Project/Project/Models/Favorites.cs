using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class Favorites
    {
      //  [Key]
        public int Id { get; set; }
      //  [ForeignKey("user_id")]
        public int UserId { get; set; } = 0;

        public string MovieId { get; set; }

        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }
}
