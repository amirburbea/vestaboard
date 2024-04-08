using System.Net.Http;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Vestaboard.Common;

public static class HostBuilderFactory
{
    public static IHostBuilder CreateHostBuilder()
    {
        return new HostBuilder()
            .ConfigureHostConfiguration(builder => builder.AddUserSecrets(typeof(IBoardClient).Assembly))
            .ConfigureLogging(HostBuilderFactory.ConfigureLogging)
            .ConfigureServices(HostBuilderFactory.ConfigureServices);
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
            .AddSingleton<IBoardClient, BoardClient>();
    }
}
