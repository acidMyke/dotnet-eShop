using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace eShop.ServiceDefaults.Extensions;

public static class OpenApiExtensions
{
  public static IApplicationBuilder UseDefaultOpenAPI(this WebApplication app, Action<SwaggerUIOptions>? setupUiAction = null)
  {
    var openApiConfig = app.Configuration.GetSection("OpenApi");
    if (!openApiConfig.Exists()) return app;
    app.UseSwagger();
    if (!app.Environment.IsDevelopment()) return app;

    if (setupUiAction != null)
    {
      setupUiAction = uiOptions =>
      {
        var pathBase = app.Configuration["PATH_BASE"] ?? string.Empty;
        var authSection = openApiConfig.GetSection("Auth");
        var endpointSection = openApiConfig.GetRequiredSection("Endpoint");

        foreach (var description in app.DescribeApiVersions())
        {
          var name = description.GroupName;
          var url = endpointSection["Url"] ?? $"{pathBase}/swagger/{name}/swagger.json";
          uiOptions.SwaggerEndpoint(url, name);
        }

        if (authSection.Exists())
        {
          uiOptions.OAuthClientId(authSection.GetRequiredValue("ClientId"));
          uiOptions.OAuthAppName(authSection.GetRequiredValue("AppName"));
        }
      };
    }
    app.UseSwaggerUI(setupUiAction);
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
    return app;
  }

  public static IHostApplicationBuilder AddDefaultOpenAPI(this IHostApplicationBuilder builder, Action<SwaggerGenOptions>? setupAction = null)
  {
    var openApiConfig = builder.Configuration.GetSection("OpenApi");
    if (!openApiConfig.Exists()) return builder;
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(setupAction);
    return builder;
  }
}
