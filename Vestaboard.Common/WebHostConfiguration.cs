using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace Vestaboard.Common;

/// <summary>
/// Common configuration for the web host for use with Vestaboard.
/// </summary>
public static class WebHostConfiguration
{
    public static void ConfigureWebHost(
        IWebHostBuilder builder,
        Assembly assembly,
        int defaultPort = 1234,
        Action<WebHostBuilderContext, IApplicationBuilder>? configureApplication = null
    )
    {
        builder
            .ConfigureKestrel(options =>
            {
                options.AddServerHeader = false;
                options.ListenAnyIP(builder.GetSetting("PORT") is { Length: > 0 } text ? int.Parse(text) : defaultPort);
            })
            .ConfigureServices(services =>
            {
                services
                   .AddMvcCore(options =>
                   {
                       options.AllowEmptyInputInBodyModelBinding = true;
                       options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
                   })
                   .AddApplicationPart(assembly)
                   .AddJsonOptions(options => WebHostConfiguration.ConfigureJsonOptions(options.JsonSerializerOptions))
                   .AddCors(options => options.AddPolicy(nameof(CorsPolicy), builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
            })
            .Configure((context, builder) =>
            {
                builder
                    .UseExceptionHandler(builder => builder.Run(async context =>
                    {
                        Exception exception = context.Features.Get<IExceptionHandlerPathFeature>()!.Error;
                        await context.Response.WriteAsync(exception.Message, context.RequestAborted);
                    }))
                    .UseRouting()
                    .UseEndpoints(endpoints => endpoints.MapControllers())
                    .UseCors(nameof(CorsPolicy));
                configureApplication?.Invoke(context, builder);
            });
    }

    private static void ConfigureJsonOptions(JsonSerializerOptions options)
    {
        options.DictionaryKeyPolicy = options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    }
}
