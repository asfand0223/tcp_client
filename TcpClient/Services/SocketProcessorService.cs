namespace TcpClient.Services;

using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class SocketProcessorService : BackgroundService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    private readonly ILogger<SocketProcessorService> _logger;

    private readonly IConsoleInputProcessorService _consoleInputProcessService;
    private readonly ISocketWriterService _socketWriterService;

    private Socket _socket;
    private IPEndPoint _endpoint;

    public SocketProcessorService(
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<SocketProcessorService> logger,
        IConsoleInputProcessorService consoleInputProcessorService,
        ISocketWriterService socketWriterService
    )
    {
        _hostApplicationLifetime = hostApplicationLifetime;

        _logger = logger;

        _consoleInputProcessService = consoleInputProcessorService;
        _socketWriterService = socketWriterService;

        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _endpoint = new IPEndPoint(IPAddress.Loopback, 5000);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _socket.ConnectAsync(_endpoint);

            await _consoleInputProcessService.ProcessAsync(_socket, cancellationToken);
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
        {
            _logger.LogInformation("Shutting down...");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ExecuteAsync");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _socket.Shutdown(SocketShutdown.Both);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StopAsync");
        }
        finally
        {
            _socket.Close();
        }

        await base.StopAsync(cancellationToken);
    }
}
