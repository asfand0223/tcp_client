namespace TcpClient;

using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

public interface ISocketWriterService
{
    public Task ReadInputAsync(CancellationToken cancellationToken);
}

public class SocketWriterService : ISocketWriterService
{
    private readonly ILogger<SocketWriterService> _logger;
    private readonly ISocketManagerService _socketManagerService;

    public SocketWriterService(
        ILogger<SocketWriterService> logger,
        ISocketManagerService socketManagerService
    )
    {
        _logger = logger;
        _socketManagerService = socketManagerService;
    }

    public async Task ReadInputAsync(CancellationToken cancellationToken)
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
                        Console.WriteLine("quitting");
                        try
                        {
                            _socketManagerService.Socket?.Shutdown(SocketShutdown.Both);
                            _socketManagerService.Socket?.Close();
                        }
                        catch { }
                        break;
                    }

                    if (_socketManagerService.Socket is not null)
                    {
                        var messageWithPrependedLengthBytes = GetMessageWithPrependedLengthBytes(
                            message
                        );
                        await _socketManagerService.Socket.SendAsync(
                            messageWithPrependedLengthBytes
                        );
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StartAsync");
        }
    }

    private byte[] GetMessageWithPrependedLengthBytes(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var messageLengthBytes = BitConverter.GetBytes(message.Length);
        var buffer = new byte[4 + messageBytes.Length];

        Array.Copy(messageLengthBytes, 0, buffer, 0, 4);
        Array.Copy(messageBytes, 0, buffer, 4, messageBytes.Length);

        return buffer;
    }
}
