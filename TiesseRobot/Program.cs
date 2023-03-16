using TiesseRobot;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<TiesseWorker>();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
