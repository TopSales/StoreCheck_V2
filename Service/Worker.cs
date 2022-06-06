using System.Net;
using ZPF.Chat;

namespace StoreCheck.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    ChatServer chatServer = null;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StoreCheck service starting at: {time}", DateTimeOffset.Now);

        // - - - init - - -
        //serverIpAddress.Text = MainViewModel.Current.Config.ServerIP;
        //serverPortValue.Text = MainViewModel.Current.Config.ServerPort;

        chatServer = (ChatServer)ChatServer.Current;
        chatServer.OnChatEvent += ServerViewModel.Current.ChatServer_OnChatEvent;

        // - - - start - - -

        IPAddress.TryParse(MainViewModel.Current.Config.ServerIP, out var ipAddress);
        var serverPort = ChatCore.ParseStringToInt(MainViewModel.Current.Config.ServerPort);

        await chatServer.Listener( ipAddress, serverPort, 1024);

        // - - -  - - -

        _logger.LogInformation("StoreCheck service running at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("StoreCheck service stops at: {time}", DateTimeOffset.Now);
    }
}
