using CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Builder;


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddSingleton<MenuTableService>(serviceProvider =>
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? throw new InvalidOperationException("AzureWebJobsStorage connection string is missing.");

            return new MenuTableService(connectionString);
        });
        services.AddSingleton<StaffFileService>(serviceProvider =>
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? throw new InvalidOperationException("AzureWebJobsStorage connection string is missing.");

            return new StaffFileService(connectionString);
        });


    })
    .Build();

host.Run();



