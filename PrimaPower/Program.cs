using ParserWorkerBase;
using PrimaPower;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<PrimaPowerWorker>();
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
