namespace TcpClient.Extensions;

using Microsoft.Extensions.Hosting;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureServices(this IHostBuilder builder)
    {
        return builder.ConfigureServices(
            (context, services) =>
            {
                ServiceCollectionExtensions.ConfigureServices(services);
            }
        );
    }
}
