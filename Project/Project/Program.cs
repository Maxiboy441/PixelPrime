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
builder.Services.AddScoped<CacheService>();

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
builder.Services.AddMemoryCache();

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
    name: "content_storefavorite",
    pattern: "Content/StoreFavoriteMovie/{id?}",
    defaults: new { controller = "Movies", action = "StoreFavoriteMovie" });

app.MapControllerRoute(
    name: "content_destroyfavorite",
    pattern: "Content/DestroyFavoriteMovie/{id?}",
    defaults: new { controller = "Movies", action = "DestroyFavoriteMovie" });

app.MapControllerRoute(
    name: "content_destroyfavoritefromprofile",
    pattern: "Content/DestroyFavoriteMovieFromProfile/{id?}",
    defaults: new { controller = "Movies", action = "DestroyFavoriteMovieFromProfile" });

app.MapControllerRoute(
    name: "content_storetowatchlist",
    pattern: "Content/StoreWatchlistMovie/{id?}",
    defaults: new { controller = "Movies", action = "StoreWatchlistMovie" });

app.MapControllerRoute(
    name: "content_destroyfromwatchlist",
    pattern: "Content/DestroyWatchlistMovie/{id?}",
    defaults: new { controller = "Movies", action = "DestroyWatchlistMovie" });

app.MapControllerRoute(
    name: "content_destroyfromwatchlistfromprofile",
    pattern: "Content/DestroyWatchlistMovieFromProfile/{id?}",
    defaults: new { controller = "Movies", action = "DestroyWatchlistMovieFromProfile" });

app.MapControllerRoute(
    name: "content",
    pattern: "content/{id?}",
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
    name: "account",
    pattern: "user/account/",
    defaults: new { controller = "User", action = "Delete" });

app.MapControllerRoute(
    name: "actor",
    pattern: "actor/{name?}",
    defaults: new { controller = "Actor", action = "Show" });

app.MapControllerRoute(
    name: "store_review",
    pattern: "content/{movieId?}/review",
    defaults: new { controller = "Review", action = "Store" });

app.MapControllerRoute(
    name: "update_review",
    pattern: "content/{movieId?}/update-review",
    defaults: new { controller = "Review", action = "Update" });

app.MapControllerRoute(
    name: "delete_review",
    pattern: "content/{movieId?}/delete-review",
    defaults: new { controller = "Review", action = "Delete" });

app.MapControllerRoute(
    name: "how-to-use",
    pattern: "how-to-use/",
    defaults: new { controller = "HowToUse", action = "Index" });

app.Run();