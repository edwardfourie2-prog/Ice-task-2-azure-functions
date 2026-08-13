using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Registers a singleton TableServiceClient using the connection string
        // from AzureWebJobsStorage (local.settings.json locally / App Settings in Azure).
        services.AddSingleton(sp =>
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? throw new InvalidOperationException("AzureWebJobsStorage connection string is not configured.");
            return new TableServiceClient(connectionString);
        });

      
    })
    .Build();

host.Run();
