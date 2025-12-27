namespace TcpClient;

using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

public interface ISocketReaderService
{
    public Task ReceiveAsync(Socket socket, CancellationToken cancellationToken);
}

public class SocketReaderService : ISocketReaderService
{
    private const int HEADER_SIZE = 4;
    private readonly ILogger<ISocketReaderService> Logger;

    public SocketReaderService(ILogger<ISocketReaderService> logger)
    {
        Logger = logger;
    }

    public async Task ReceiveAsync(Socket socket, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var headerBuffer = await ReceiveExactAsync(socket, HEADER_SIZE, cancellationToken);
            if (headerBuffer is null)
            {
                return;
            }

            int messageLength = BitConverter.ToInt32(headerBuffer, 0);

            var messageBuffer = await ReceiveExactAsync(socket, messageLength, cancellationToken);
            if (messageBuffer is null)
            {
                return;
            }

            string message = Encoding.UTF8.GetString(messageBuffer, 0, messageBuffer.Length);
            Logger.LogInformation(message);
        }
    }

    private async Task<byte[]?> ReceiveExactAsync(
        Socket socket,
        int bytesToRead,
        CancellationToken cancellationToken
    )
    {
        int bytesRead = 0;
        var buffer = new byte[bytesToRead];

        while (bytesRead < bytesToRead)
        {
            int bytesReceived = await socket.ReceiveAsync(
                buffer.AsMemory(bytesRead, bytesToRead - bytesRead),
                SocketFlags.None,
                cancellationToken
            );
            if (bytesReceived == 0)
            {
                return null;
            }

            bytesRead += bytesReceived;
        }

        return buffer;
    }
}
