using Project.Data;
using Microsoft.EntityFrameworkCore;
using Project.Database.Seeders;
using Project.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<AiApiService>();
builder.Services.AddScoped<AiApiService>();
builder.Services.AddHttpClient<MovieApiService>();
builder.Services.AddScoped<MovieApiService>();

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
    pattern: "Movies/StoreFavorite/{id?}",
    defaults: new { controller = "Movies", action = "StoreFavorite" });

app.MapControllerRoute(
    name: "movies_destroyfavorite",
    pattern: "Movies/DestroyFavorite/{id?}",
    defaults: new { controller = "Movies", action = "DestroyFavorite" });

app.MapControllerRoute(
    name: "movies_storetowatchlist",
    pattern: "Movies/StoreWatchlist/{id?}",
    defaults: new { controller = "Movies", action = "StoreWatchlist" });

app.MapControllerRoute(
    name: "movies_destroyfromwatchlist",
    pattern: "Movies/DestroyWatchlist/{id?}",
    defaults: new { controller = "Movies", action = "DestroyWatchlist" });

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

app.Run();