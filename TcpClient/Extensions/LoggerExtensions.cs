namespace TcpClient.Extensions;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public static class HostBuilderLoggingExtensions
{
    public static IHostBuilder ConfigureLogging(this IHostBuilder builder)
    {
        ConfigureAppLogging(builder);

        return builder;
    }

    public static IHostBuilder ConfigureAppLogging(IHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });

        return builder;
    }
}
