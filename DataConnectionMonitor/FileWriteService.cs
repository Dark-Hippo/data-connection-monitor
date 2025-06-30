namespace DataConnectionMonitor;

public class FileWriteService
{
    public void WriteFailureToFile(string disconnectionsFile, DateTime failureTime, double failureDuration)
    {
        var failureFile = disconnectionsFile;
        using var writer = new StreamWriter(failureFile, true);
        writer.WriteLine($"{failureTime:yyyy-MM-dd HH:mm:ss},{failureDuration}");
    }

    public void WriteSuccessToFile(string lastSuccessfulConnectionsFile)
    {
        var successFile = lastSuccessfulConnectionsFile;
        using var writer = new StreamWriter(successFile, false);
        writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    }

    public void WriteCurrentStatusToFile(string currentStatusFile, ConnectionState connectionState)
    {
        var statusFile = currentStatusFile;
        using var writer = new StreamWriter(statusFile, false);
        writer.WriteLine($"{connectionState}");
    }
}