// .NET Generic host https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;

using var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
        services.AddHostedService<ScheduledService>();
    })
    .Build();

await host.RunAsync();
