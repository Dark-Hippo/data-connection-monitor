
using Microsoft.AspNetCore.SignalR;

public class LastPingService : BackgroundService
{
    private readonly IHubContext<PingHub> _hubContext;

    public LastPingService(IHubContext<PingHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            // var lastConnection = File.ReadAllText(lastSuccessfulConnectionFile);
            Console.WriteLine("Sending last connection");
            var lastConnection = DateTime.Now.ToString();
            await _hubContext.Clients.All.SendAsync("ReceiveLastConnection", lastConnection);
            await Task.Delay(5000, stoppingToken);
        }
    }
}