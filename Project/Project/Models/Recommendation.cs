using System;
namespace Project.Models
{
	public class Recommendation
	{
        public int Id { get; set; }
        // TODO: Add User_id as foreign key
        public int User_id { get; set; }
        public string Movie_id { get; set; }
    }
}

