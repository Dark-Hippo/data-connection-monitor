using System;
using System.IO;
using System.Net.NetworkInformation;

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
var failure = false;
var failureTime = DateTime.MinValue;
var reconnectionTime = DateTime.MinValue;

while (true)
{
  Log("Last ping attempt at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), false);
  var reply = ping.Send(randomIPAddress);
  if (reply.Status == IPStatus.Success)
  {
    if (failure)
    {
      failure = false;
      LogReconnectionTimeAndDowntime(failureTime);
      failureTime = DateTime.MinValue;
    }
    else
    {
      Console.WriteLine($"Success: {randomIPAddress}");
    }
  }
  else
  {
    Console.WriteLine("Ping failure on {0}", randomIPAddress);
    var failureIPAddress = new List<string>();
    failureIPAddress.AddRange(IPAddresses);
    failureIPAddress.Remove(randomIPAddress);
    if (PingIPAddresses(failureIPAddress))
    {
      if (failure)
      {
        failure = false;
        LogReconnectionTimeAndDowntime(failureTime);
        failureTime = DateTime.MinValue;
      }
    }
    else
    {
      if (!failure)
      {
        failureTime = DateTime.Now;
        failure = true;
      }
      Log("Internet failure at " + failureTime.ToString("yyyy-MM-dd HH:mm:ss"));
    }

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
static bool PingIPAddresses(List<string> IPAddresses)
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
      Console.WriteLine($"Additional failure on: {randomIPAddress}");
      IPAddresses.Remove(randomIPAddress);
      count--;
    }
  }

  return false;
}

static void Log(string message, bool failure = true)
{

  var successFile = "output/success.txt";
  var failureFile = "output/failure.txt";

  if (failure)
  {
    if (!File.Exists(failureFile))
    {
      File.Create(failureFile).Close();
    }
    File.AppendAllLinesAsync(failureFile, [message]);
    Console.WriteLine(message);
  }
  else
  {
    // if success, write datetime to file
    File.WriteAllTextAsync(successFile, message);
    Console.WriteLine(message);
  }
}

static void LogReconnectionTimeAndDowntime(DateTime failureTime)
{
  var reconnectionTime = DateTime.Now;
  var message = "Connection restored at " + reconnectionTime.ToString("yyyy-MM-dd HH:mm:ss");
  Console.WriteLine(message);
  Log(message);
  var downtime = reconnectionTime.Subtract(failureTime);
  Log("Connection was down for a total of " + downtime.TotalSeconds + " seconds");
}