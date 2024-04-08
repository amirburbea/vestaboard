using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Vestaboard.Common;
using Vestaboard.Wordle.Services;

namespace Vestaboard.Wordle;

internal static class Program
{
    private static void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSingleton<IWordRepository, WordRepository>()
            .AddSingleton<IGameService, GameService>()
            .AddHostedService<TitleScreenHostedService>();
    }

    private static async Task Main()
    {
        using IHost host = HostBuilderFactory.CreateHostBuilder()
            .ConfigureWebHostDefaults(builder => WebHostConfiguration.ConfigureWebHost(
                builder,
                typeof(Program).Assembly,
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
