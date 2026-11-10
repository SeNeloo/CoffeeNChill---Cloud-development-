using CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

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
            var connectionString =
    Environment.GetEnvironmentVariable("AzureWebJobsStorage")
    ?? configuration["Values:AzureWebJobsStorage"]
    ?? throw new InvalidOperationException("AzureWebJobsStorage connection string is missing.");

            var service = new StaffFileService(connectionString);

            service.InitializeAsync().GetAwaiter().GetResult();

            return service;
        });
        services.AddSingleton<OrderTableService>(serviceProvider =>
        {
            var connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? throw new InvalidOperationException(
                    "AzureWebJobsStorage connection string is missing.");

            return new OrderTableService(connectionString);
        });
        services.AddSingleton<UserTableService>(serviceProvider =>
        {
            var connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? throw new InvalidOperationException(
                    "AzureWebJobsStorage connection string is missing.");

            return new UserTableService(connectionString);
        });



    })
    .Build();

host.Run();



