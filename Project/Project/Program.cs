using Project.Data;
using Microsoft.EntityFrameworkCore;
using Project.Database.Seeders;
using Project.Services;
using Project.HostedServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<AiApiService>();
builder.Services.AddScoped<AiApiService>();
builder.Services.AddHttpClient<MovieApiService>();
builder.Services.AddScoped<MovieApiService>();
builder.Services.AddScoped<RecommendationService>();
builder.Services.AddScoped<BackgroundRecommendationService>();
builder.Services.AddHostedService<BackgroundRecommendationService>();
builder.Services.AddScoped<ActorAPIService>();
builder.Services.AddScoped<WikipediaMediaAPIService>();


builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DataContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(604800);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Seed the database in development environment
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var seeder = new DatabaseSeeder(context);
        seeder.Seed();
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "movies_storefavorite",
    pattern: "Movies/StoreFavoriteMovie/{id?}",
    defaults: new { controller = "Movies", action = "StoreFavoriteMovie" });

app.MapControllerRoute(
    name: "movies_destroyfavorite",
    pattern: "Movies/DestroyFavoriteMovie/{id?}",
    defaults: new { controller = "Movies", action = "DestroyFavoriteMovie" });

app.MapControllerRoute(
    name: "movies_destroyfavoritefromprofile",
    pattern: "Movies/DestroyFavoriteMovieFromProfile/{id?}",
    defaults: new { controller = "Movies", action = "DestroyFavoriteMovieFromProfile" });

app.MapControllerRoute(
    name: "movies_storetowatchlist",
    pattern: "Movies/StoreWatchlistMovie/{id?}",
    defaults: new { controller = "Movies", action = "StoreWatchlistMovie" });

app.MapControllerRoute(
    name: "movies_destroyfromwatchlist",
    pattern: "Movies/DestroyWatchlistMovie/{id?}",
    defaults: new { controller = "Movies", action = "DestroyWatchlistMovie" });

app.MapControllerRoute(
    name: "movies_destroyfromwatchlistfromprofile",
    pattern: "Movies/DestroyWatchlistMovieFromProfile/{id?}",
    defaults: new { controller = "Movies", action = "DestroyWatchlistMovieFromProfile" });

app.MapControllerRoute(
    name: "movies",
    pattern: "Movies/{id?}",
    defaults: new { controller = "Movies", action = "Show" });

app.MapControllerRoute(
    name: "auth",
    pattern: "{controller=Auth}/{action=Login}");

app.MapControllerRoute(
    name: "search",
    pattern: "Search",
    defaults: new { controller = "Api", action = "Search" });

app.MapControllerRoute(
    name: "searchbyid",
    pattern: "Searchbyid",
    defaults: new { controller = "Api", action = "SearchById" });

app.MapControllerRoute(
    name: "profile",
    pattern: "profile/",
    defaults: new { controller = "Profile", action = "Index" });

app.MapControllerRoute(
    name: "account",
    pattern: "user/account/",
    defaults: new { controller = "User", action = "Update" });

app.MapControllerRoute(
    name: "actor",
    pattern: "actor/{name?}",
    defaults: new { controller = "Actor", action = "Show" });

app.MapControllerRoute(
    name: "store_review",
    pattern: "movies/{movieId?}/review",
    defaults: new { controller = "Reviews", action = "Store" });

app.MapControllerRoute(
    name: "update_review",
    pattern: "movies/{movieId?}/update-review",
    defaults: new { controller = "Reviews", action = "Update" });

app.MapControllerRoute(
    name: "delete_review",
    pattern: "movies/{movieId?}/delete-review",
    defaults: new { controller = "Reviews", action = "Delete" });

app.Run();