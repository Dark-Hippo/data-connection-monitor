# TODO List

## DataConnectionMonitor

- [x] Output folder location to environment variable / config file.
- [x] Other consts to config file.
- [x] DNS list to config file.
- [x] Add dockerfile with multi-stage build and proper build process.
- [x] Write connection status to file

## DataConnectionMonitorAPI

- [x] Add dockerfile with multi-stage build and proper build process.
- [x] Add endpoint for lastSuccessfulConnection endpoint.
- [ ] Read connection status from file, implement web socket to push status to UI.
- [ ] Read last successful connection from file, implement web socket to push status to UI.
- [ ] Look at authentication for API.
- [ ] Add readme for API.

## DataConnectionMonitorUI

- [ ] Add dockerfile with multi-stage build and proper build process (look at npm serve).
- [ ] Add nginx to docker compose.
- [ ] Change page title
- [ ] Add last successful connection endpoint to UI.
- [ ] Add connection status indicator
- [ ] Add authentication to communicate with API