using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;

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

// Path: DataConnectionMonitor/Program.cs
var ping = new Ping();

var random = new Random();
var randomIPAddress = IPAddresses[random.Next(IPAddresses.Count)];

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
  builder.AddSimpleConsole(options => {
    options.IncludeScopes = true;
    options.SingleLine = true;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss";
  });
});

ILogger logger = loggerFactory.CreateLogger("DataConnectionMonitor");

var failureTime = DateTime.MinValue;
ConnectionState connectionState = ConnectionState.Initialising;

while (true)
{
  var reply = ping.Send(randomIPAddress);

  // if unsuccessful ping and current status is not disconnected
  if(reply.Status != IPStatus.Success && connectionState != ConnectionState.Disconnected)
  {
    logger.LogWarning("Ping failure on {IPAddress}", randomIPAddress);

    // log failure time
    failureTime = DateTime.Now;

    // set status as retrying
    connectionState = ConnectionState.Retrying;

    // retry with a different IP address
    var retrySuccessful = RetryConnection(logger, IPAddresses);

    // if retry successful, continue
    if(retrySuccessful)
    {
      logger.LogInformation("Connection restored");
      connectionState = ConnectionState.Connected;
    }
    // else set status as disconnected
    else
    {
      logger.LogError("Internet failure occured");
      connectionState = ConnectionState.Disconnected;
    }
  }

  else
  {
    // if current status is failed
    // log failure time and duration
    // set status as connected
  }

  // Select a new random IP address
  var previousIPAddress = randomIPAddress;
  while (previousIPAddress == randomIPAddress)
  {
    randomIPAddress = IPAddresses[random.Next(IPAddresses.Count)];
  }

  Console.WriteLine();
  Thread.Sleep(5000);
}

/// <summary>
/// Pings a list of IP addresses until one of them responds
/// If none of them respond, returns false
/// If one of them responds, returns true
/// </summary>
bool RetryConnection(ILogger logger, List<string> IPAddresses)
{
  var count = 3;
  Random random = new();
  while (count > 1)
  {
    // Select a random IP address
    string randomIPAddress = IPAddresses[random.Next(IPAddresses.Count)];

    // Attempt to ping the IP address
    Ping pingSender = new();
    PingReply reply = pingSender.Send(randomIPAddress);

    // If the ping was successful, print a success message
    if (reply.Status == IPStatus.Success)
    {
      return true;
    }
    // If the ping failed, print a failure message and remove the IP address from the list
    else
    {
      logger.LogWarning("Additional failure on {IPAddress}", randomIPAddress);
      IPAddresses.Remove(randomIPAddress);
      count--;
    }
  }

  return false;
}

void WriteFailureToFile(DateTime failureTime, int failureDuration)
{
  var failureFile = "output/connectionFailures.csv";
    using var writer = new StreamWriter(failureFile, true);
    writer.WriteLine($"{failureTime:yyyy-MM-dd HH:mm:ss},{failureDuration}");
}

void WriteSuccessToFile()
{
  var successFile = "output/lastSuccessfulConnection.txt";
  using var writer = new StreamWriter(successFile, false);
  writer.WriteLine($"Last successful ping at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
}

void LogReconnectionTimeAndDowntime(ILogger logger, DateTime failureTime)
{
  var reconnectionTime = DateTime.Now;
  logger.LogInformation("Connection restored");
  var downtime = reconnectionTime.Subtract(failureTime);
  logger.LogInformation("Connection was down for a total of {downtime} seconds", downtime.TotalSeconds);
  WriteFailureToFile(failureTime, (int)downtime.TotalSeconds);
}