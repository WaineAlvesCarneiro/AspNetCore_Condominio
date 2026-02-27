using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore_Condominio.Configurations.Configurations;

public static class HostExtensions
{
    public static IHostBuilder AddAppLogging(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Debug);
        });
    }
}
