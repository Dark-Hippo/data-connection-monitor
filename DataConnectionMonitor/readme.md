# Simple data connection monitor

Simple command line tool to monitor the status of a data connection. Uses the `ping` command to check if a host is reachable. Randomly selects a DNS to ping from a list of 10 publically available DNS servers. Failures are written to a simple CSV file in for format `timestamp,downtime` (downtime is in seconds), last successful ping time is also written to a text file in for format `timestamp`, and the current connection status is written to a text file as `Connected`, `Retrying` or `Disconnected`.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)

## Installation

It's a simple dotnet core console application. Clone the repo, build it and run it. I have it running in a docker container running on a spare Raspberry Pi.

## Usage

Run the program, check the logs, do something with the data (I like to send the disconnection data occasionally to my ISP to get a refund on my bill).

Requires a volume to be mapped to the container directory `/app/output` to store the output files.

### Evnironment variables

- `DisconnectionsFile`: The file to write disconnection data to in the format `timestamp,downtime` (downtime is in seconds). Defaults to 'output/connectionFailures.csv'
- `LastSuccessfulConnectionFile`: The file to write the timestamp of the last successful connection to. Defaults to 'output/lastSuccessfulConnection.txt'
- `CurrentStatusFile`: The file to write the current status to. Defaults to 'output/currentStatus.txt'
- `MaxRetries`: The number of times to retry a ping before considering it a failure. Defaults to 2 (i.e. 3 attempts in total)
- `PingInterval`: The interval in seconds between pings. Defaults to 3 seconds


## Contributing

If you have any ideas for improvements, feel free to open an issue or submit a pull request.
