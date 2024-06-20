using Order.Outbox.Table.Publisher.Jobs;
using Quartz;
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddQuartz(configurator =>
{
    JobKey jobkey = new("OrderOutboxPublishJob");
    configurator.AddJob<OrderOutboxPublishJob>(options => options.WithIdentity(jobkey));

    TriggerKey triggerKey = new("OrderOutboxPublishTrigger");
    configurator.AddTrigger(options => options.ForJob(jobkey)
    .WithIdentity(triggerKey)
    .StartAt(DateTime.UtcNow)
    .WithSimpleSchedule(builder => builder.WithIntervalInSeconds(5).RepeatForever()));
});


builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

var host = builder.Build();
host.Run();
