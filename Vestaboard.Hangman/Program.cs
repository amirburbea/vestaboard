using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Vestaboard.Common;
using Vestaboard.Hangman.Services;

namespace Vestaboard.Hangman;

internal static class Program
{
    private static void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSingleton<IGameService, GameService>()
            .AddHostedService<TitleScreenHostedService>();
    }

    private static async Task Main()
    {
        using IHost host = HostBuilderFactory.CreateHostBuilder()
            .ConfigureWebHostDefaults(builder => WebHostConfiguration.ConfigureWebHost(
                builder,
                Assembly.GetExecutingAssembly(),
                configureApplication: (context, builder) =>
                {
                    PhysicalFileProvider fileProvider = new(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "fe/out")));
                    builder
                        .UseDefaultFiles(options: new() { FileProvider = fileProvider, DefaultFileNames = { "index.html" }, RequestPath = string.Empty })
                        .UseStaticFiles(options: new() { FileProvider = fileProvider, RequestPath = string.Empty });
                }
            ))
            .ConfigureServices(Program.ConfigureServices)
            .Build();
        await host.RunAsync().ConfigureAwait(false);
    }
}
