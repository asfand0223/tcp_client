using Microsoft.Extensions.Hosting;
using TcpClient.Extensions;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices().ConfigureLogging().Build().Run();
