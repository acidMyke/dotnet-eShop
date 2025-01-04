using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace eShop.ServiceDefaults.Extensions;

public static class HealthCheckExtensions
{

  public static IHostApplicationBuilder ConfigureHealthChecks(this IHostApplicationBuilder builder)
  {
    builder.Services.AddHealthChecks()
        // Add a default liveness check to ensure app is responsive
        .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

    return builder;
  }

  public static WebApplication MapDefaultEndpoints(this WebApplication app)
  {
    // Adding health checks endpoints to applications in non-development environments has security implications.
    // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
    if (app.Environment.IsDevelopment())
    {
      // All health checks must pass for app to be considered ready to accept traffic after starting
      app.MapHealthChecks("/health");

      // Only health checks tagged with the "live" tag must pass for app to be considered alive
      app.MapHealthChecks("/alive", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
      {
        Predicate = r => r.Tags.Contains("live")
      });
    }

    return app;
  }

}
