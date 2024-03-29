﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vestaboard.Common;

namespace Vestaboard.Clock;

internal static class Program
{
    private static async Task Main()
    {
        using IHost host = new HostBuilder()
            .ConfigureHostConfiguration(builder => builder.AddUserSecrets(typeof(IBoardClient).Assembly))
            .ConfigureServices(Program.ConfigureServices)
            .ConfigureLogging(Program.ConfigureLogging)
            .Build();
        await host.RunAsync().ConfigureAwait(false);
    }

    private static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder builder)
    {
        builder.ClearProviders().AddSimpleConsole(options => options.SingleLine = true);
        if (context.HostingEnvironment.IsDevelopment())
        {
            builder.AddDebug();
        }
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services
            .AddSingleton<HttpMessageHandler, SocketsHttpHandler>()
            .AddSingleton<IClockRenderer, WordClockRenderer>()
            .AddSingleton<IBoardClient, BoardClient>()
            .AddHostedService<ClockService>();
    }
}