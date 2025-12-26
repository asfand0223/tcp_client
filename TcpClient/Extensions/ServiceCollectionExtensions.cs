namespace TcpClient.Extensions;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        ConfigureAppServices(services);

        return services;
    }

    private static IServiceCollection ConfigureAppServices(IServiceCollection services)
    {
        services.AddHostedService<SocketService>();

        services.AddSingleton<ISocketWriterService, SocketWriterService>();
        services.AddSingleton<ISocketReaderService, SocketReaderService>();
        services.AddSingleton<ISocketManagerService, SocketManagerService>();

        return services;
    }
}
