namespace TcpClient.Services;

using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public interface IConsoleInputProcessorService
{
    public Task ProcessAsync(Socket socket, CancellationToken cancellationToken);
}

public class ConsoleInputProcessorService : IConsoleInputProcessorService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    private readonly ILogger<SocketWriterService> _logger;

    private readonly ISocketWriterService _socketWriterService;

    public ConsoleInputProcessorService(
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<SocketWriterService> logger,
        ISocketWriterService socketWriterService
    )
    {
        _hostApplicationLifetime = hostApplicationLifetime;

        _logger = logger;

        _socketWriterService = socketWriterService;
    }

    public async Task ProcessAsync(Socket socket, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Type to enter a message:");

                var message = Console.ReadLine();

                if (message is not null)
                {
                    if (message.ToLower() == "quit")
                    {
                        _hostApplicationLifetime.StopApplication();
                        break;
                    }

                    await _socketWriterService.WriteAsync(socket, message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StartAsync");
        }
    }
}
