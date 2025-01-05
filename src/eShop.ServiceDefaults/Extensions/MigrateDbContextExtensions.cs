

using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eShop.ServiceDefaults.Extensions;

public static class MigrateDbContextExtensions
{
  private static readonly string ActivitySourceName = "DbMigrations";
  private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

  public static IServiceCollection AddDbContextMigration<TContext, TDbSeeder>(this IServiceCollection services)
    where TContext : DbContext
    where TDbSeeder : class, IDbSeeder<TContext>
  {
    services.AddScoped<IDbSeeder<TContext>, TDbSeeder>();
    return services.AddDbContextMigration();
  }

  public static IServiceCollection AddDbContextMigration(this IServiceCollection services)
  {
    services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource(ActivitySourceName));
    return services;
  }

  public static Task ApplyDbContextMigrationAsync<TContext>(this WebApplication app)
    where TContext : DbContext => app.Services.ApplyDbContextMigrationAsync<TContext>();

  public static async Task ApplyDbContextMigrationAsync<TContext>(this IServiceProvider services)
  where TContext : DbContext
  {
    using var scope = services.CreateScope();
    var scopeServices = scope.ServiceProvider;
    var logger = scopeServices.GetRequiredService<ILogger<TContext>>();
    var context = scopeServices.GetRequiredService<TContext>();
    var seeder = scopeServices.GetService<IDbSeeder<TContext>>();

    using var activity = ActivitySource.StartActivity($"Migration operation {typeof(TContext).Name}");

    try
    {
      logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);
      var strategy = context.Database.CreateExecutionStrategy();

      await strategy.ExecuteAsync(() =>
        context.Database
          .MigrateAsync()
          .ContinueWith((task) => seeder?.SeedAsync(context) ?? Task.CompletedTask)
      );
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
      activity?.SetExceptionTags(ex);
      throw;
    }

  }
}

public interface IDbSeeder<in TContext> where TContext : DbContext
{
  Task SeedAsync(TContext context);
}