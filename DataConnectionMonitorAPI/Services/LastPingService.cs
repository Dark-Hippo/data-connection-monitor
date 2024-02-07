using Microsoft.AspNetCore.SignalR;

namespace DataConnectionMonitorAPI
{

  public class LastPingService : BackgroundService
  {
    private readonly IHubContext<DisconnectionsHub> _hubContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LastPingService> _logger;
    private readonly FileSystemWatcher _watcher;

    private readonly string _lastSuccessfulConnectionFile;

    public LastPingService(IHubContext<DisconnectionsHub> hubContext, IConfiguration configuration, ILogger<LastPingService> logger)
    {
      _hubContext = hubContext;
      _configuration = configuration;
      _logger = logger;
      _lastSuccessfulConnectionFile = _configuration["LastSuccessfulConnectionFile"] ?? "";
      if (string.IsNullOrEmpty(_lastSuccessfulConnectionFile))
      {
        throw new InvalidOperationException("LastSuccessfulConnectionFile is not set");
      }

      var path = _lastSuccessfulConnectionFile[.._lastSuccessfulConnectionFile.LastIndexOf('/')];
      var fileName = _lastSuccessfulConnectionFile[(_lastSuccessfulConnectionFile.LastIndexOf('/') + 1)..];

      _logger.LogInformation("Watching file {fileName} at {path}", fileName, path);

      _watcher = new FileSystemWatcher
      {
        Path = path,
        Filter = fileName,
        NotifyFilter = NotifyFilters.LastWrite
      };

      _watcher.Changed += OnChanged;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _watcher.EnableRaisingEvents = true;

      while (!stoppingToken.IsCancellationRequested)
      {
        var lastConnection = File.ReadAllText(_lastSuccessfulConnectionFile);
        _logger.LogInformation("Ping sent at {time}", DateTime.Now.ToString());
        await _hubContext.Clients.All.SendAsync("ReceiveLastConnection", lastConnection, cancellationToken: stoppingToken);
        await Task.Delay(1000, stoppingToken);
      }
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
      _logger.LogInformation("File changed: {name} at {time}", e.Name, DateTime.Now.ToLongTimeString());
    }
  }
}