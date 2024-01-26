# Simple data connection monitor

Simple command line tool to monitor the status of a data connection. Uses the `ping` command to check if a host is reachable. Randomly selects a DNS to ping from a list of 10 publically available DNS servers. Failures are written to a simple CSV file in for format `timestamp,downtime` (downtime is in seconds).

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)

## Installation

It's a simple dotnet core console application. Clone the repo, build it and run it. I have it running in a docker container running on a spare Raspberry Pi.

## Usage

Run the program, check the logs, do something with the data (I like to send the disconnection data occasionally to my ISP to get a refund on my bill)

## Contributing

If you have any ideas for improvements, feel free to open an issue or submit a pull request.
