using Microsoft.AspNetCore.SignalR;

namespace DataConnectionMonitorAPI
{

  public class LastSuccessfulConnectionService : BackgroundService
  {
    private readonly IHubContext<DisconnectionsHub> _hubContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LastSuccessfulConnectionService> _logger;
    private readonly FileSystemWatcher _watcher;

    private readonly string _lastSuccessfulConnectionFile;

    public LastSuccessfulConnectionService(IHubContext<DisconnectionsHub> hubContext, IConfiguration configuration, ILogger<LastSuccessfulConnectionService> logger)
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

    protected override Task<Task> ExecuteAsync(CancellationToken stoppingToken)
    {
      _watcher.EnableRaisingEvents = true;
      return Task.FromResult(Task.CompletedTask);
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
      _logger.LogInformation("File changed: {name} at {time}", e.Name, DateTime.Now.ToLongTimeString());
      _hubContext.Clients.All.SendAsync("ReceiveLastConnection", DateTime.Now);
    }
  }
}