using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace eShop.ServiceDefaults.Extensions;

public static class OpenTelemetryExtensions
{
  public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
  {
    builder.Logging.AddOpenTelemetry(logging =>
    {
      logging.IncludeFormattedMessage = true;
      logging.IncludeScopes = true;
    });

    builder.Services.AddOpenTelemetry()
        .WithMetrics(metrics =>
        {
          metrics.AddAspNetCoreInstrumentation();
          metrics.AddHttpClientInstrumentation();
          metrics.AddRuntimeInstrumentation();
        })
        .WithTracing(tracing =>
        {
          if (builder.Environment.IsDevelopment())
            tracing.SetSampler(new AlwaysOffSampler());
          tracing.AddAspNetCoreInstrumentation();
          tracing.AddHttpClientInstrumentation();
        });

    var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

    if (useOtlpExporter)
      builder.Services.AddOpenTelemetry().UseOtlpExporter();

    return builder;
  }

}
