using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace DataConnectionMonitorAPI
{
  public class CurrentConnectionStatusService : BackgroundService
  {
    private readonly IHubContext<DisconnectionsHub> _hubContext;
    private readonly ILogger<CurrentConnectionStatusService> _logger;
    private readonly FileSystemWatcher _watcher;
    private readonly Config _configuration;

    private readonly string _currentStatusFile;

    public CurrentConnectionStatusService(IHubContext<DisconnectionsHub> hubContext, ILogger<CurrentConnectionStatusService> logger, IOptions<Config> configuration)
    {
      _hubContext = hubContext;
      _configuration = configuration.Value;
      _logger = logger;
      _currentStatusFile = _configuration.CurrentStatusFile ?? "";

      if (string.IsNullOrEmpty(_currentStatusFile))
      {
        _logger.LogError("CurrentStatusFile is not set");
        throw new InvalidOperationException("CurrentStatusFile is not set");
      }

      var path = _currentStatusFile[.._currentStatusFile.LastIndexOf('/')];
      var fileName = _currentStatusFile[(_currentStatusFile.LastIndexOf('/') + 1)..];

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
      try
      {
        var lastConnection = File.ReadAllText(_currentStatusFile);
        _hubContext.Clients.All.SendAsync("CurrentConnectionStatus", lastConnection);
      }
      catch (Exception)
      {
        // Log the error, but don't do anything else. Chances are the file is being written to 
        // and we'll get another event soon.
        _logger.LogError("Error reading file {file}", _currentStatusFile);
      }
    }
  }
}