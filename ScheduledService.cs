using Microsoft.Extensions.Hosting;
using Quartz;

// https://www.quartz-scheduler.net/documentation/quartz-4.x/quick-start.html
// https://andrewlock.net/using-quartz-net-with-asp-net-core-and-worker-services/
// https://github.com/skiptirengu/Hosting.Extensions.Quartz/blob/master/src/HostBuilderExtensions.cs#L71

public class ScheduledService : BackgroundService
{
    private readonly Lazy<Task<IScheduler>> _scheduler;

    public ScheduledService(ISchedulerFactory factory) =>
        _scheduler = new Lazy<Task<IScheduler>>(() => factory.GetScheduler());

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Grab the Scheduler instance from the Factory
        var scheduler = await _scheduler.Value;

        // define the job and tie it to our HelloJob class
        var job = JobBuilder.Create<HelloJob>()
            .WithIdentity("job1", "group1")
            .Build();

        // Trigger the job to run now, and then repeat every 10 seconds
        var trigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartNow()
            .WithSimpleSchedule(s => s
                .WithIntervalInSeconds(1)
                .RepeatForever()
            )
            .Build();

        // Tell Quartz to schedule the job using our trigger
        await scheduler.ScheduleJob(job, trigger);

        // and start it off
        await scheduler.Start(cancellationToken);
        Console.WriteLine("A scheduler has been started.");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        var scheduler = await _scheduler.Value;
        await scheduler.Shutdown(cancellationToken);
        Console.WriteLine("A scheduler has been stopped.");
    }

    public class HelloJob : IJob
    {
        public async Task Execute(IJobExecutionContext context) =>
            await Console.Out.WriteLineAsync("Greetings from HelloJob!");
    }
}
