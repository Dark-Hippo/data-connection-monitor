using Microsoft.AspNetCore.SignalR;

public class PingHub : Hub
{
    public async Task SendPing(string message)
    {
        Console.WriteLine("Received ping");
        await Clients.All.SendAsync("ReceivePing", message);
    }
}