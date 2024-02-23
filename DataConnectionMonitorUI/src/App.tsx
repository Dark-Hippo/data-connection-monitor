import { useDisconnectionsData } from "./hooks/useDisconnectionsData";
import { Disconnections } from "./components/Disconnections";
import { Route, Routes } from "react-router-dom";
import { Container, Spinner } from "react-bootstrap";
import * as signalR from "@microsoft/signalr";
import { useTheme } from "./contexts/ThemeContext";

import "./App.css";
import { useState } from "react";
import { ThemeSwitcher } from "./components/ThemeSwitcher";
import { DisconnectionDate } from "./components/DisconnectionDate";
import { BackArrow } from "./components/BackArrow";
import { CurrentConnectionStatus, ConnectionStatus } from "./components/CurrentConnectionStatus";
import { LastSuccessfulConnection } from "./components/LastSuccessfulConnection";
import { DisconnectionsList } from "./components/DisconnectionsList";
import { DisconnectionStats } from "./components/DisconnectionStats";

function App() {
  const {
    loading,
    totalDisconnections,
    longestDisconnection,
    groupedDisconnections,
    disconnectionsByDate,
  } = useDisconnectionsData();

  const [connectionStatus, setConnectionStatus] = useState<ConnectionStatus>(ConnectionStatus.Disconnected);
  const [lastConnection, setLastConnection] = useState<string>();

  const { theme } = useTheme();

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center"
        style={{ height: "100vh" }}>
        <Spinner animation="border" role="status" variant="primary">
          <span className="visually-hidden">Loading...</span>
        </Spinner>
      </div>
    );
  }

  const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/disconnections-hub",
      {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
    .build();

  connection.on("LastSuccessfulConnection", (lastConnection: string) => {
    setLastConnection(lastConnection);
  });

  connection.on("CurrentConnectionStatus", (status: string) => {
    // need to trim() as the status file has a newline character at the end
    setConnectionStatus(status.trim() as ConnectionStatus);
  });

  connection.start().then(() => {
    console.log("connected to disconnections hub!");
  }).catch((err) => {
    console.log("error connecting to disconnections hub", err);
  });

  return (
    <Container fluid data-bs-theme={theme}>
      <header>
        <BackArrow />
        <DisconnectionDate date={new Date()} />
        <ThemeSwitcher />
      </header>
      <main>
        <h1>
          Connection Monitor
        </h1>
        <CurrentConnectionStatus connectionStatus={connectionStatus} />
        <LastSuccessfulConnection time={lastConnection} />

        <strong>Disconnections</strong>
        <Routes>
          <Route
            path="/"
            element={<DisconnectionsList disconnections={groupedDisconnections} />}
          />
          <Route
            path="/:date"
            element={<Disconnections disconnections={disconnectionsByDate} />}
          />
          <Route
            path="/stats"
            element={<DisconnectionStats totalDisconnections={totalDisconnections} totalDowntime={""} longestDisconnection={longestDisconnection} />}
          />
        </Routes>
      </main>
    </Container>
  );
}

export default App;
