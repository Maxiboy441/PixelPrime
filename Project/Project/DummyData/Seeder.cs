/*Usage:

Add in the page
@{
var movies = Project.DummyData.Seeder.getMovies();

}
*/

namespace Project.DummyData
{
   public class Seeder
    {
        public static List<Movie> getMovies()
        {
            return new List<Movie>
            {
                new Movie
                {
                    Title = "Spider Man: Lost Cause",
                    Year = "2014",
                    Rated = "N/A",
                    Released = "26 Sep 2014",
                    Runtime = "140 min",
                    Genre = "Action, Adventure, Comedy",
                    Director = "Joey Lever",
                    Writer = "Steve Ditko, Stan Lee, Joey Lever",
                    Actors = "Joey Lever, Craig Ellis, Teravis Ward",
                    Plot = "Peter Parker a lone child discovers that his parents were in a horrifying plot to make mankind change. getting bitten by his fathers invention he develops super powers to tries to find answers to his whole life, try and juggle a r...",
                    Language = "English",
                    Country = "United Kingdom",
                    Awards = "N/A",
                    Poster = "https://m.media-amazon.com/images/M/MV5BYmZkYWRlNWQtOGY0Zi00MWZkLWJiZTktNjRjMDY4MTU2YzAyXkEyXkFqcGdeQXVyMzYzNzc1NjY@._V1_SX300.jpg",
                    Ratings = "6.0",
                    imdbID = "tt2803854"
                },
                new Movie
                {
                    Title = "Godzilla x Kong: The New Empire",
                    Year = "2024",
                    Rated = "PG-13",
                    Released = "29 Mar 2024",
                    Runtime = "115 min",
                    Genre = "Action, Adventure, Fantasy",
                    Director = "Adam Wingard",
                    Writer = "Terry Rossio, Adam Wingard, Simon Barrett",
                    Actors = "Rebecca Hall, Brian Tyree Henry, Dan Stevens",
                    Plot = "Two ancient titans, Godzilla and Kong, clash in an epic battle as humans unravel their intertwined origins and connection to Skull Island's mysteries.",
                    Language = "English",
                    Country = "United States, Australia",
                    Awards = "N/A",
                    Poster = "https://m.media-amazon.com/images/M/MV5BY2QwOGE2NGQtMWQwNi00M2IzLThlNWItYWMzNGQ5YWNiZDA4XkEyXkFqcGdeQXVyNTE1NjY5Mg@@._V1_SX300.jpg",
                    Ratings = "7.0",
                    imdbID = "tt14539740"
                },
                new Project.DummyData.Movie
                {
                    Title = "Harry Potter and the Deathly Hallows: Part 2",
                    Year = "2011",
                    Rated = "PG-13",
                    Released = "15 Jul 2011",
                    Runtime = "130 min",
                    Genre = "Adventure, Family, Fantasy",
                    Director = "David Yates",
                    Writer = "Steve Kloves, J.K. Rowling",
                    Actors = "Daniel Radcliffe, Emma Watson, Rupert Grint",
                    Plot = "Harry, Ron, and Hermione search for Voldemort's remaining Horcruxes in their effort to destroy the Dark Lord as the final battle rages on at Hogwarts.",
                    Language = "English, Latin",
                    Country = "United Kingdom, United States",
                    Awards = "Nominated for 3 Oscars. 48 wins & 95 nominations total",
                    Poster = "https://m.media-amazon.com/images/M/MV5BMGVmMWNiMDktYjQ0Mi00MWIxLTk0N2UtN2ZlYTdkN2IzNDNlXkEyXkFqcGdeQXVyODE5NzE3OTE@._V1_SX300.jpg",
                    Ratings = "8.1",
                    Metascore = "85",
                    imdbRating = "8.1",
                    imdbVotes = "953,417",
                    imdbID = "tt1201607",
                    Type = "movie",
                    DVD = "N/A",
                    BoxOffice = "$381,447,587",
                    Production = "N/A",
                    Website = "N/A",
                    Response = "True"
                },
                new Movie
                {
                    Title = "The Hunger Games",
                    Year = "2012",
                    Rated = "PG-13",
                    Released = "23 Mar 2012",
                    Runtime = "142 min",
                    Genre = "Action, Adventure, Sci-Fi",
                    Director = "Gary Ross",
                    Writer = "Gary Ross, Suzanne Collins, Billy Ray",
                    Actors = "Jennifer Lawrence, Josh Hutcherson, Liam Hemsworth",
                    Plot = "Katniss Everdeen voluntarily takes her younger sister's place in the Hunger Games: a televised competition in which two teenagers from each of the twelve Districts of Panem are chosen at random to fight to the death.",
                    Language = "English",
                    Country = "United States",
                    Awards = "Won 1 BAFTA Award, 34 wins & 49 nominations total",
                    Poster = "https://m.media-amazon.com/images/M/MV5BMjA4NDg3NzYxMF5BMl5BanBnXkFtZTcwNTgyNzkyNw@@._V1_SX300.jpg",
                    Ratings = "7.2",
                    Metascore = "68",
                    imdbRating = "7.2",
                    imdbVotes = "1,007,612",
                    imdbID = "tt1392170",
                    Type = "movie",
                    DVD = "N/A",
                    BoxOffice = "$408,010,692",
                    Production = "N/A",
                    Website = "N/A",
                    Response = "True"
                },
                new Movie
                {
                    Title = "The Avengers",
                    Year = "2012",
                    Rated = "PG-13",
                    Released = "04 May 2012",
                    Runtime = "143 min",
                    Genre = "Action, Sci-Fi",
                    Director = "Joss Whedon",
                    Writer = "Joss Whedon, Zak Penn",
                    Actors = "Robert Downey Jr., Chris Evans, Scarlett Johansson",
                    Plot = "Earth's mightiest heroes must come together and learn to fight as a team if they are going to stop the mischievous Loki and his alien army from enslaving humanity.",
                    Language = "English, Russian",
                    Country = "United States",
                    Awards = "Nominated for 1 Oscar. 39 wins & 81 nominations total",
                    Poster = "https://m.media-amazon.com/images/M/MV5BNDYxNjQyMjAtNTdiOS00NGYwLWFmNTAtNThmYjU5ZGI2YTI1XkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_SX300.jpg",
                    Ratings = "8.0",
                    Metascore = "69",
                    imdbRating = "8.0",
                    imdbVotes = "1,464,818",
                    imdbID = "tt0848228",
                    Type = "movie",
                    DVD = "N/A",
                    BoxOffice = "$623,357,910",
                    Production = "N/A",
                    Website = "N/A",
                    Response = "True"
                },
                new Movie
                {
                    Title = "Fantastic Beasts and Where to Find Them",
                    Year = "2016",
                    Rated = "PG-13",
                    Released = "18 Nov 2016",
                    Runtime = "132 min",
                    Genre = "Adventure, Family, Fantasy",
                    Director = "David Yates",
                    Writer = "J.K. Rowling",
                    Actors = "Eddie Redmayne, Katherine Waterston, Alison Sudol",
                    Plot = "In 1926, magizoologist Newt Scamander arrives in New York during his worldwide tour to research and rescue magical creatures as something mysterious leaves trails of destruction in the city, threatening to expose the wizarding world.",
                    Language = "English, Central Khmer",
                    Country = "United Kingdom, United States",
                    Awards = "Won 1 Oscar. 15 wins & 54 nominations total",
                    Poster = "https://m.media-amazon.com/images/M/MV5BMjMxOTM1OTI4MV5BMl5BanBnXkFtZTgwODE5OTYxMDI@._V1_SX300.jpg",
                    Ratings = "7.2",
                    Metascore = "66",
                    imdbRating = "7.2",
                    imdbVotes = "509,076",
                    imdbID = "tt3183660",
                    Type = "movie",
                    DVD = "N/A",
                    BoxOffice = "$234,037,575",
                    Production = "N/A",
                    Website = "N/A",
                    Response = "True"
                }

            };
        }
    }
}

