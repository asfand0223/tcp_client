namespace TcpClient.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class HostExtensions
{
    public static async Task ConfigureHost(this IHost host)
    {
        await host.StartAsync();

        var writerService = host.Services.GetRequiredService<ISocketWriterService>();
        await writerService.RunAsync();

        await host.StopAsync();
    }
}
