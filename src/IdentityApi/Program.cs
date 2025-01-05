using eShop.IdentityApi.Data;
using eShop.ServiceDefaults;
using eShop.ServiceDefaults.Extensions;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.AddDefaultOpenAPI();

// Database
builder.AddNpgsqlDbContext<ApplicationDbContext>(ConfigKeys.IdentityDb);
builder.Services.AddDbContextMigration<ApplicationDbContext, DataSeed>();

// Auth
builder.Services
  .AddIdentity<IdentityUser, IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthorization();

var app = builder.Build();
await app.ApplyDbContextMigrationAsync<ApplicationDbContext>();

app.UseDefaultOpenAPI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.Run();