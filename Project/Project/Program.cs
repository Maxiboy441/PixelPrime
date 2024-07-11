using Project.Data;
using Microsoft.EntityFrameworkCore;
using Project.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<AiApiService>();
builder.Services.AddScoped<AiApiService>();
builder.Services.AddHttpClient<MovieApiService>();
builder.Services.AddScoped<MovieApiService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DataContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "auth",
    pattern: "{controller=Auth}/{action=Login}");

app.Run();