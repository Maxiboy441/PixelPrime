namespace Project.Database.Factories;

public class DummyData
{
    public static string[] posters  { get; set; }
    public static string[] movieIds  { get; set; }

    static DummyData()
    {
        posters = new[]
        {
            "https://m.media-amazon.com/images/M/MV5BZWI1OWM3ZmEtNjQ2OS00NzI2LTgwNWMtZDAyMGI1OTM2MzJmXkEyXkFqcGdeQXVyNjc5NjEzNA@@._V1_SX300.jpg",
            "https://m.media-amazon.com/images/M/MV5BYmZkYWRlNWQtOGY0Zi00MWZkLWJiZTktNjRjMDY4MTU2YzAyXkEyXkFqcGdeQXVyMzYzNzc1NjY@._V1_SX300.jpg",
            "https://m.media-amazon.com/images/M/MV5BOTk5ODg0OTU5M15BMl5BanBnXkFtZTgwMDQ3MDY3NjM@._V1_SX300.jpg",
            "https://m.media-amazon.com/images/M/MV5BMjIxODgxNTE5N15BMl5BanBnXkFtZTcwODM0MTM3Mg@@._V1_SX300.jpg",
            "https://m.media-amazon.com/images/M/MV5BM2YwYTkwNjItNGQzNy00MWE1LWE1M2ItOTMzOGI1OWQyYjA0XkEyXkFqcGdeQXVyMTUzMTg2ODkz._V1_SX300.jpg",
            "https://m.media-amazon.com/images/M/MV5BNWIwODRlZTUtY2U3ZS00Yzg1LWJhNzYtMmZiYmEyNmU1NjMzXkEyXkFqcGdeQXVyMTQxNzMzNDI@._V1_SX300.jpg",
            "https://m.media-amazon.com/images/M/MV5BYjBkM2RjMzItM2M3Ni00N2NjLWE3NzMtMGY4MzE4MDAzMTRiXkEyXkFqcGdeQXVyNDUzOTQ5MjY@._V1_SX300.jpg",
            "https://m.media-amazon.com/images/M/MV5BY2Q0ODg4ZmItNDZiYi00ZWY5LTg2NzctNmYwZjA5OThmNzE1XkEyXkFqcGdeQXVyMjM4MzQ4OTQ@._V1_SX300.jpg",
            "https://m.media-amazon.com/images/M/MV5BYjU5YTA5OGItYzFmZi00YmFjLWJjNWEtZTk0MjExMTFjYzE2XkEyXkFqcGdeQXVyNjcwMzExMzU@._V1_SX300.jpg",
            "https://m.media-amazon.com/images/M/MV5BNzA5ZDNlZWMtM2NhNS00NDJjLTk4NDItYTRmY2EwMWZlMTY3XkEyXkFqcGdeQXVyNzkwMjQ5NzM@._V1_SX300.jpg"
        };
        
        movieIds = new[]
        {
            "tt0111161",
            "tt0068646",
            "tt0071562",
            "tt0468569",
            "tt0050083",
            "tt0108052",
            "tt0167260",
            "tt0110912",
            "tt0060196",
            "tt0137523",
            "tt0109830",
            "tt1375666",
            "tt0167261",
            "tt0133093",
            "tt0099685",
            "tt0073486",
            "tt0114369",
            "tt0047478",
            "tt0038650",
            "tt0102926",
            "tt0120815",
            "tt0080684",
            "tt0167260",
            "tt0137523",
            "tt1375666",
            "tt0109830",
            "tt0133093",
            "tt0114369",
            "tt0076759",
            "tt0253474" 
        };
    }
}