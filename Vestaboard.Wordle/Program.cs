using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.FileProviders;
using Vestaboard.Common;
using Vestaboard.Wordle.Services;

namespace Vestaboard.Wordle;

internal static class Program
{
    private static void ConfigureJsonOptions(JsonSerializerOptions options)
    {
        options.DictionaryKeyPolicy = options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    }

    private static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder builder)
    {
        builder.ClearProviders().AddSimpleConsole(options => options.SingleLine = true);
        if (context.HostingEnvironment.IsDevelopment())
        {
            builder.AddDebug();
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSingleton<HttpMessageHandler>(new SocketsHttpHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })
            .AddSingleton<IBoardClient, BoardClient>()
            .AddSingleton<IWordRepository, WordRepository>()
            .AddSingleton<IGameService, GameService>()
            .AddHostedService<TitleScreenHostedService>();
    }

    private static void ConfigureWebHostDefaults(IWebHostBuilder builder)
    {
        builder
            .ConfigureKestrel(options =>
            {
                options.AddServerHeader = false;
                options.ListenAnyIP(builder.GetSetting("PORT") is { Length: > 0 } text ? int.Parse(text) : 81);
            })
            .ConfigureServices(services =>
            {
                services
                   .AddMvcCore(options =>
                   {
                       options.AllowEmptyInputInBodyModelBinding = true;
                       options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
                   })
                   .AddJsonOptions(options => Program.ConfigureJsonOptions(options.JsonSerializerOptions))
                   .AddCors(options => options.AddPolicy(nameof(CorsPolicy), builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
            })
            .Configure((context, builder) =>
            {
                PhysicalFileProvider fileProvider = new(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "fe/out")));
                builder
                    .UseExceptionHandler(builder => builder.Run(async context =>
                    {
                        Exception exception = context.Features.Get<IExceptionHandlerPathFeature>()!.Error;
                        await context.Response.WriteAsync(exception.Message, context.RequestAborted);
                    }))
                    .UseRouting()
                    .UseDefaultFiles(options: new() { FileProvider = fileProvider, DefaultFileNames = { "index.html" }, RequestPath = string.Empty })
                    .UseStaticFiles(options: new() { FileProvider = fileProvider, RequestPath = string.Empty })
                    .UseEndpoints(endpoints => endpoints.MapControllers())
                    .UseCors(nameof(CorsPolicy));
            });
    }

    private static async Task Main()
    {
        using IHost host = new HostBuilder()
            .ConfigureHostConfiguration(builder => builder.AddUserSecrets(typeof(Program).Assembly))
            .ConfigureWebHostDefaults(Program.ConfigureWebHostDefaults)
            .ConfigureLogging(Program.ConfigureLogging)
            .ConfigureServices(Program.ConfigureServices)
            .Build();
        await host.RunAsync().ConfigureAwait(false);
    }
}