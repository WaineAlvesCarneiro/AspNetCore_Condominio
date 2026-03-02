using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore_Condominio.Configurations.Configurations;

public static class HostExtensions
{
    public static IHostBuilder AddAppLogging(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureLogging((context, logging) =>
        {
            logging.ClearProviders();
            logging.AddConsole();

            if (context.HostingEnvironment.IsDevelopment())
                logging.SetMinimumLevel(LogLevel.Debug);
            
            if (context.HostingEnvironment.IsProduction())
                logging.SetMinimumLevel(LogLevel.Information);
        });
    }
}