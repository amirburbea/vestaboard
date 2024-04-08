using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vestaboard.Common;

namespace Vestaboard.Clock;

internal static class Program
{
    private static async Task Main()
    {
        using IHost host = HostBuilderFactory.CreateHostBuilder()
            .ConfigureServices(Program.ConfigureServices)
            .Build();
        await host.RunAsync().ConfigureAwait(false);
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services
            .AddSingleton<IClockRenderer, WordClockRenderer>()
            .AddHostedService<ClockService>();
    }
}