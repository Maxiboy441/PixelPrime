# Technical Concept: PixelPrime

## Overview
The project uses an API to retrieve movies that users can rate and add to their favorites. Users can also add movies to a “watch list” and receive recommendations from our AI server.

## Features
- Search for movies
- Rate movies
- Top Rated Movies List
- Movie Recommendations
- User Profile
- Login/Register
- Statisics
- Watchlist
- Movie info page (+ movie trailer)
- Comments to movies

## Architecture
The project is made in C# MVC

### Front-end
- HTML
- CSS
- JS
- C#
- Bootstrap

### Back-end
- C# MVC

### Database
- MySql container

### Database Modell
![Alt text](./diagramms/erm.png)

### External Services/APIs
- Movie API: 
https://www.omdbapi.com/

## Security Considerations
- user auth
- server behind a firewall and revers proxs

## Deployment and Hosting
Hosted on [Maximilian Huber](https://github.com/maxiboy441)'s home server which is based on debian.

## Infrastructure diagramm 
![Alt text](./diagramms/infrastructure_diagram.png)


## Future Enhancements
- Use of AI features (recommendations with diffrent weighting) api: api.webai.maxih.de

### Used Packages
- ASP.NET Core Identity: For handling user authentication and authorization.
- Entity Framework Core (EF Core): For database interactions and ORM capabilities.



## TODO's
| Title  |
| -------- |
| ASP.NET Core Identity Installation   |
| Entity Framework Core |
| User Model  (if not done by package)|
| Reviews Model |
| Favorites Model   |
| Watchlist Model   |
| Ratings Model   |
| Home controller (show favorites) |
| User controller (if not done by package) |
| Reviews controller (CUD)|
| Movie controller (indexAllRatedMovies, show, addRating)|
| Favorites controller (index, addMovie, delete)|
| Watchlist controller  (index, addMovie, delete)|
| Navbar Component |
| Master layout |
| Purple button |
| Movies card component |
| Movie see button component |
| Login page |
| Register page |
| Home page |
| Movies page |
| Profile page |
| ReviewPopUp |
| MovieAPIService |
| AIAPIService |



