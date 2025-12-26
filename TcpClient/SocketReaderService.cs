namespace TcpClient;

using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

public interface ISocketReaderService
{
    public Task RunAsync(Socket socket);
}

public class SocketReaderService : ISocketReaderService
{
    private const int HEADER_SIZE = 4;
    private readonly ILogger<ISocketReaderService> Logger;

    public SocketReaderService(ILogger<ISocketReaderService> logger)
    {
        Logger = logger;
    }

    public async Task RunAsync(Socket socket)
    {
        while (true)
        {
            var headerBuffer = await ReadExactAsync(socket, HEADER_SIZE);
            if (headerBuffer is null)
            {
                return;
            }

            int messageLength = BitConverter.ToInt32(headerBuffer, 0);

            var messageBuffer = await ReadExactAsync(socket, messageLength);
            if (messageBuffer is null)
            {
                return;
            }

            string message = Encoding.UTF8.GetString(messageBuffer, 0, messageBuffer.Length);
            Logger.LogInformation(message);
        }
    }

    private async Task<byte[]?> ReadExactAsync(Socket socket, int bytesToRead)
    {
        int bytesRead = 0;
        var buffer = new byte[bytesToRead];

        while (bytesRead < bytesToRead)
        {
            int bytesReceived = await socket.ReceiveAsync(
                buffer.AsMemory(bytesRead, bytesToRead - bytesRead),
                SocketFlags.None
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
