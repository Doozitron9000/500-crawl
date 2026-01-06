using _500_crawl.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// this is all copied pretty closely from the microsoft website (e.g. I've just copied their exception string) with just the relevant vars changed.
// get the connections string for the db from appsettings.json and if it isnt found throw an exception using a ternary
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string"
        + "'DefaultConnection' not found.");

builder.Services.AddDbContext<SiteDbContext>(options =>
    options.UseSqlite(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

// add the password hasher
builder.Services.AddScoped<
    Microsoft.AspNetCore.Identity.IPasswordHasher<User>,
    Microsoft.AspNetCore.Identity.PasswordHasher<User>
>();

// Make our auth options class auto fill its props via the authentication section in appsettings
// and break the app if it isn't correctly configured so we can't start without our pepper
builder.Services.AddOptions<AuthenticationOptions>().Bind(builder.Configuration.GetSection("Authentication"))
    .Validate(
        o => !string.IsNullOrEmpty(o.PasswordPepper),
        "Password pepper must be configured!"
    ).ValidateOnStart();

// enable sessions and have them last 30mins for now
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".500Crawl.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// tell the app to use sessions
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
