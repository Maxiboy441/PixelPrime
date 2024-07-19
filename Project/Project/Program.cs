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

app.Run();