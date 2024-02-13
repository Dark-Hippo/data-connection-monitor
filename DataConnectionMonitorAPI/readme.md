# Data Connection Monitor API

This is the API for the Data Connection Monitor project. It is a .NET Core 8.0 Web API project that reads the status of the data connection from files and serves it to the UI via web sockets.

## Installation

It's a simple dotnet core web API. Clone the repo, build it and run it. I have it running in a docker container running on a spare Raspberry Pi.

## Usage

Run the program, call the endpoints and SignalR methods, do something with the data.

Requires a volume to be mapped to the container directory `/app/data` to provide the data files.

## API Endpoints

- `GET /disconnections`: Returns a list of disconnections in the format `timestamp,downtime` (downtime is in seconds).

## SignalR Hubs

### DisconnectionsHub

- `/disconnections-hub`: This hub is responsible for providing data on the last successful connection and current connection status.

#### Events

- `LastSuccessfulConnection`: This event is raised when the last successful connection changes.

- `CurrentConnectionStatus`: This event is raised when the current connection status changes.