namespace TcpClient;

using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class SocketService : BackgroundService
{
    private readonly ILogger<SocketService> _logger;
    private readonly ISocketReaderService _socketReaderService;
    private readonly ISocketManagerService _socketManagerService;

    private Socket _clientSocket;
    private IPEndPoint _endpoint;

    public SocketService(
        ILogger<SocketService> logger,
        ISocketReaderService socketReaderService,
        ISocketManagerService socketManagerService
    )
    {
        _logger = logger;
        _socketReaderService = socketReaderService;
        _socketManagerService = socketManagerService;

        _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _endpoint = new IPEndPoint(IPAddress.Loopback, 5000);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _clientSocket.ConnectAsync(_endpoint);

            _socketManagerService.Socket = _clientSocket;

            while (!cancellationToken.IsCancellationRequested)
            {
                await _socketReaderService.RunAsync(_clientSocket);
            }
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
        {
            _logger.LogInformation("Shutting down...");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StartAsync");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StopAsync");
        }
        finally
        {
            _clientSocket.Close();
        }

        await base.StopAsync(cancellationToken);
    }
}
