# TODO List

## DataConnectionMonitor

- [x] Output folder location to environment variable / config file.
- [x] Other consts to config file.
- [x] DNS list to config file.
- [ ] Add dockerfile with multi-stage build and proper build process.
- [x] Write connection status to file

## DataConnectionMonitorAPI

- [ ] Add dockerfile with multi-stage build and proper build process.
- [x] Add endpoint for lastSuccessfulConnection endpoint.
- [ ] Get UI address from environment variable for CORS
- [ ] Read connection status from file, implement web socket to push status to UI.
- [ ] Look at authentication for API.

## DataConnectionMonitorUI

- [ ] Add dockerfile with multi-stage build and proper build process (look at npm serve).
- [ ] Add nginx to docker compose.
- [ ] Change page title
- [ ] Add last successful connection endpoint to UI.
- [ ] Add connection status indicator
- [ ] Add authentication to communicate with API