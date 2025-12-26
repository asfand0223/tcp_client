namespace TcpClient;

using System.Text;
using Microsoft.Extensions.Logging;

public interface ISocketWriterService
{
    public Task RunAsync();
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

    public async Task RunAsync()
    {
        try
        {
            while (true)
            {
                Console.WriteLine("Type to enter a message:");
                var message = Console.ReadLine();

                if (message == "quit")
                {
                    Console.WriteLine("quitting");
                    break;
                }

                if (_socketManagerService.Socket is not null && message is not null)
                {
                    var messageWithPrependedLengthBytes = GetMessage(message);
                    await _socketManagerService.Socket.SendAsync(messageWithPrependedLengthBytes);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StartAsync");
        }
    }

    private byte[] GetMessage(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var messageLengthBytes = BitConverter.GetBytes(message.Length);
        var buffer = new byte[4 + messageBytes.Length];

        Array.Copy(messageLengthBytes, 0, buffer, 0, 4);
        Array.Copy(messageBytes, 0, buffer, 4, messageBytes.Length);

        return buffer;
    }
}
