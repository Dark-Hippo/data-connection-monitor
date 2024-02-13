using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace DataConnectionMonitorAPI
{
  public class LastSuccessfulConnectionService : BackgroundService
  {
    private readonly IHubContext<DisconnectionsHub> _hubContext;
    private readonly ILogger<LastSuccessfulConnectionService> _logger;
    private readonly FileSystemWatcher _watcher;
    private readonly Config _configuration;

    private readonly string _lastSuccessfulConnectionFile;

    public LastSuccessfulConnectionService(IHubContext<DisconnectionsHub> hubContext, ILogger<LastSuccessfulConnectionService> logger, IOptions<Config> configuration)
    {
      _hubContext = hubContext;
      _logger = logger;
      _configuration = configuration.Value;
      _lastSuccessfulConnectionFile = _configuration.LastSuccessfulConnectionFile ?? "";

      if (string.IsNullOrEmpty(_lastSuccessfulConnectionFile))
      {
        _logger.LogError("LastSuccessfulConnectionFile is not set");
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
      try
      {
        var lastConnection = File.ReadAllText(_lastSuccessfulConnectionFile);
        _hubContext.Clients.All.SendAsync("LastSuccessfulConnection", lastConnection);
      }
      catch (Exception)
      {
        // Log the error, but don't do anything else. Chances are the file is being written to 
        // and we'll get another event soon.
        _logger.LogError("Error reading file {file}", _lastSuccessfulConnectionFile);
      }
    }
  }
}