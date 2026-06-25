using Microsoft.Extensions.Logging;
using DataConnectionMonitor.Models;

namespace DataConnectionMonitor.Services;

public class FileWriteService
{
  private readonly ILogger _logger;

  public FileWriteService(ILogger logger)
  {
    _logger = logger;
  }

  public void WriteFailureToFile(string disconnectionsFile, DateTime failureTime, double failureDuration)
  {
    if(string.IsNullOrEmpty(disconnectionsFile))
    {
      _logger.LogError("DisconnectionsFile is not set");
      throw new InvalidOperationException("DisconnectionsFile is not set");
    }

    var failureFile = disconnectionsFile;

    try {
      using var writer = new StreamWriter(failureFile, true);
      writer.WriteLine($"{failureTime:yyyy-MM-dd HH:mm:ss},{failureDuration}");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error writing to disconnections file");
      throw;
    }
  }

  public void WriteSuccessToFile(string lastSuccessfulConnectionsFile)
  {
    if(string.IsNullOrEmpty(lastSuccessfulConnectionsFile))
    {
      _logger.LogError("LastSuccessfulConnectionsFile is not set");
      throw new InvalidOperationException("LastSuccessfulConnectionsFile is not set");
    }

    var successFile = lastSuccessfulConnectionsFile;
    
    try {
      using var writer = new StreamWriter(successFile, false);
      writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error writing to last successful connections file");
      throw;
    }
  }

  public void WriteCurrentStatusToFile(string currentStatusFile, ConnectionState connectionState)
  {
    if(string.IsNullOrEmpty(currentStatusFile))
    {
      _logger.LogError("CurrentStatusFile is not set");
      throw new InvalidOperationException("CurrentStatusFile is not set");
    }

    var statusFile = currentStatusFile;
    
    try {
      using var writer = new StreamWriter(statusFile, false);
      writer.WriteLine($"{connectionState}");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error writing to current status file");
      throw;
    }
  }
}
