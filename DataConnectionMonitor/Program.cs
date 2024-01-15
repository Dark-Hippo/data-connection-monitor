using System;
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

while (true)
{

  var reply = ping.Send(randomIPAddress);
  if (reply.Status == IPStatus.Success)
  {
    Console.WriteLine($"Success: {randomIPAddress}");
    if (failure)
    {
      failure = false;
      Console.WriteLine("connection restored at {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    }
  }
  else
  {
    Console.WriteLine($"Failure: {randomIPAddress}");
    var failureIPAddress = new List<string>();
    failureIPAddress.AddRange(IPAddresses);
    failureIPAddress.Remove(randomIPAddress);
    if (PingIPAddresses(failureIPAddress))
    {
      Console.WriteLine("Internet connection is working again!");
      if (failure)
      {
        failure = false;
        Console.WriteLine("connection restored at {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
      }
    }
    else
    {
      Console.WriteLine("Internet connection is still down!");
      Console.WriteLine("Internet failure at {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
      failure = true;
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
      Console.WriteLine($"Failure: {randomIPAddress}");
      IPAddresses.Remove(randomIPAddress);
      count--;
    }

    // Wait for 1 second before the next ping
    // Thread.Sleep(1000);
  }

  return false;
}
