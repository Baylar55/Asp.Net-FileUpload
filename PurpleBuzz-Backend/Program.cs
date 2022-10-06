using Microsoft.EntityFrameworkCore;
using PurpleBuzz_Backend.DAL;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connectionString));
var app = builder.Build();
app.MapDefaultControllerRoute();
app.UseStaticFiles();
app.Run();
