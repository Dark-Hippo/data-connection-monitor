using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

var IPAddresses = new List<string> {
  "8.8.8.8", // Google DNS
  "8.8.4.4", // Google DNS
  "1.1.1.1", // Cloudflare DNS
  "1.0.0.1", // Cloudflare DNS
  "9.9.9.9", // Quad9 DNS
  "208.67.222.222", // OpenDNS
  "208.67.220.220", // OpenDNS
  "4.2.2.2", // Level3 DNS
  "4.2.2.1", // Level3 DNS
  "192.0.43.10", // ICANN DNS
  // "123.123.123.123", // Invalid IP address
};

var builder = new ConfigurationBuilder()
  .SetBasePath(Directory.GetCurrentDirectory())
  .AddJsonFile("appsettings.json")
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

  logger.LogInformation("Pinging {IPAddress}", randomIPAddress);
  var reply = ping.Send(randomIPAddress);

  switch (connectionState)
  {
    case ConnectionState.Connected:
      if (reply.Status == IPStatus.Success)
      {
        logger.LogInformation("Successfully pinged {IPAddress}", randomIPAddress);
        WriteSuccessToFile();
        WriteCurrentStatusToFile(connectionState);
      }
      else
      {
        logger.LogWarning("Failed to ping {IPAddress}", randomIPAddress);
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
        logger.LogWarning("Failed to ping {IPAddress}", randomIPAddress);
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
        logger.LogWarning("Still unable to ping {IPAddress}", randomIPAddress);
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