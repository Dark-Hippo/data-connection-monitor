using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
  .SetBasePath(Directory.GetCurrentDirectory())
  .AddJsonFile("appsettings.json", reloadOnChange: true, optional: false)
  .AddEnvironmentVariables();

var configuration = builder.Build();

// maximum retries after the first ping failure
bool success = int.TryParse(configuration["MaxRetries"], out int maxRetries);
if (!success && maxRetries == 0)
{
  throw new InvalidOperationException("Failed to parse number and maxRetries is not set");
}

// interval between ping attempts
success = int.TryParse(configuration["PingInterval"], out int pingInterval);
if (!success && pingInterval == 0)
{
  throw new InvalidOperationException("Failed to parse number and pingInterval is not set");
}

var disconnectionsFile = configuration["DisconnectionsFile"];
if (string.IsNullOrEmpty(disconnectionsFile))
{
  throw new InvalidOperationException("DisconnectionsFile is not set");
}

var lastSuccessfulConnectionsFile = configuration["LastSuccessfulConnectionFile"];
if (string.IsNullOrEmpty(lastSuccessfulConnectionsFile))
{
  throw new InvalidOperationException("LastSuccessfulConnectionFile is not set");
}

var currentStatusFile = configuration["CurrentStatusFile"];
if (string.IsNullOrEmpty(currentStatusFile))
{
  throw new InvalidOperationException("CurrentStatusFile is not set");
}

var IPAddresses = configuration.GetSection("IPAddresses").Get<List<IPAddress>>();
if(IPAddresses == null || IPAddresses.Count == 0)
{
  throw new InvalidOperationException("IPAddresses is not set");
}


var ping = new Ping();
var random = new Random();
var randomIPAddress = IPAddresses[random.Next(IPAddresses.Count)];

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
  builder.AddSimpleConsole(options =>
  {
    options.IncludeScopes = true;
    options.SingleLine = true;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss";
  });

});

ILogger logger = loggerFactory.CreateLogger("DataConnectionMonitor");

var failureTime = DateTime.MinValue;
ConnectionState connectionState = ConnectionState.Connected;

var retryCount = 0;

while (true)
{
  // Select a new random IP address
  var previousIPAddress = randomIPAddress;

  while (previousIPAddress == randomIPAddress)
  {
    randomIPAddress = IPAddresses[random.Next(IPAddresses.Count)];
  }

  logger.LogInformation("Pinging {IPAddress}", randomIPAddress.Address);
  var reply = ping.Send(randomIPAddress.Address);

  switch (connectionState)
  {
    case ConnectionState.Connected:
      if (reply.Status == IPStatus.Success)
      {
        logger.LogInformation("Successfully pinged {IPAddress}", randomIPAddress.Address);
        WriteSuccessToFile();
        WriteCurrentStatusToFile(connectionState);
      }
      else
      {
        logger.LogWarning("Failed to ping {IPAddress}", randomIPAddress.Address);
        failureTime = DateTime.Now;
        connectionState = ConnectionState.Retrying;
        WriteCurrentStatusToFile(connectionState);
      }
      break;
    case ConnectionState.Retrying:
      if (reply.Status == IPStatus.Success)
      {
        connectionState = ConnectionState.Connected;
        WriteCurrentStatusToFile(connectionState);
        retryCount = 0;
      }
      else
      {
        logger.LogWarning("Failed to ping {IPAddress}", randomIPAddress.Address);
        retryCount++;
        if (retryCount >= maxRetries)
        {
          connectionState = ConnectionState.Disconnected;
        WriteCurrentStatusToFile(connectionState);
          retryCount = 0;
        }
      }
      break;
    case ConnectionState.Disconnected:
      if (reply.Status == IPStatus.Success)
      {
        logger.LogInformation("Connection restored");
        var downtime = DateTime.Now.Subtract(failureTime);
        logger.LogInformation("Connection was down for a total of {downtime} seconds", downtime.TotalSeconds);
        WriteFailureToFile(failureTime, downtime.TotalSeconds);
        connectionState = ConnectionState.Connected;
        WriteCurrentStatusToFile(connectionState);
      }
      else
      {
        logger.LogWarning("Still unable to ping {IPAddress}", randomIPAddress.Address);
      }
      break;
  }

  logger.LogInformation("Current connection state: {connectionState}", connectionState);

  Thread.Sleep(pingInterval);
}

void WriteFailureToFile(DateTime failureTime, double failureDuration)
{
  var failureFile = disconnectionsFile;
  using var writer = new StreamWriter(failureFile, true);
  writer.WriteLine($"{failureTime:yyyy-MM-dd HH:mm:ss},{failureDuration}");
}

void WriteSuccessToFile()
{
  var successFile = lastSuccessfulConnectionsFile;
  using var writer = new StreamWriter(successFile, false);
  writer.WriteLine($"Last successful ping at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
}

void WriteCurrentStatusToFile(ConnectionState connectionState)
{
  var statusFile = currentStatusFile;
  using var writer = new StreamWriter(statusFile, false);
  writer.WriteLine($"{connectionState}");
}