using eShop.ServiceDefaults.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization();
builder.AddDefaultOpenAPI();

var app = builder.Build();

app.UseDefaultOpenAPI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.Run();