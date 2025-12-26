using Microsoft.Extensions.Hosting;
using TcpClient.Extensions;

var builder = Host.CreateDefaultBuilder(args);

await builder.ConfigureServices().ConfigureLogging().Build().ConfigureHost();
