namespace TcpClient;

using System.Net.Sockets;

public interface ISocketManagerService
{
    public Socket? Socket { get; set; }
}

public class SocketManagerService : ISocketManagerService
{
    public Socket? Socket { get; set; }
}
