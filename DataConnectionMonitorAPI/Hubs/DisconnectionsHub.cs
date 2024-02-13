using Microsoft.AspNetCore.SignalR;

namespace DataConnectionMonitorAPI
{
    public class DisconnectionsHub(ILogger<DisconnectionsHub> logger) : Hub
    {
        private readonly ILogger<DisconnectionsHub> _logger = logger;

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected {client}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Client disconnected {client}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}