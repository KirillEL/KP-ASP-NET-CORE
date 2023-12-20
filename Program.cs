using System.Text;
using dotnet_kp.Database;
using dotnet_kp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql("Host=shop_db;Port=5432;Database=my_shop;Username=postgres;Password=Kirik228;"));


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.Cookie.Name = "jwtToken";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("user_role", "admin"));
});



builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();


app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }

    var adminExists = context.Users.FirstOrDefault(u => u.UserName == "admin" && u.Password == "admin");
    if (adminExists == null)
    {
        var admin = new User
        {
            UserName = "admin",
            Email = "admin@example.com",
            Password = "admin",
            Role = "admin"
        };
        context.Users.Add(admin);
        context.SaveChanges();
    }
    var tvExists = context.Categories.FirstOrDefault(c => c.Name == "Телевизоры");
    if (tvExists == null)
    {
        var tv = new Category
        {
            Name = "Телевизоры",
            Description = "Телевизоры кайф"
        };
        context.Categories.Add(tv);
        context.SaveChanges();
    }
    var smartphoneExists = context.Categories.FirstOrDefault(c => c.Name == "Смартфоны");
    if (smartphoneExists == null)
    {
        var smt = new Category
        {
            Name = "Смартфоны",
            Description = "Смартфоны топ1"
        };
        context.Categories.Add(smt);
        context.SaveChanges();
    }
    var laptops = context.Categories.FirstOrDefault(c => c.Name == "Ноутбуки");
    if (laptops == null)
    {
        var laptop = new Category
        {
            Name = "Ноутбуки",
            Description = "Ноутбуки мощь!"
        };
        context.Categories.Add(laptop);
        context.SaveChanges();
    }

}

app.Run();