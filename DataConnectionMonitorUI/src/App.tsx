import { useDisconnectionsData } from "./hooks/useDisconnectionsData";
import { Disconnections } from "./components/Disconnections";
import { Route, Routes } from "react-router-dom";
import { Container, Spinner } from "react-bootstrap";
import * as signalR from "@microsoft/signalr";
import { useTheme } from "./contexts/ThemeContext";

import "./App.css";
import { useEffect, useState } from "react";
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
    totalDowntime,
  } = useDisconnectionsData();

  const [connectionStatus, setConnectionStatus] = useState<ConnectionStatus>(ConnectionStatus.Disconnected);
  const [lastConnection, setLastConnection] = useState<string>();

  const { theme } = useTheme();

  useEffect(() => {
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

    connection.start();

    connection.onclose(() => {
      setConnectionStatus(ConnectionStatus.Disconnected);
    });

    // .catch((err) => {
    //   console.error("error connecting to disconnections hub", err);
    // });

  }, []);

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

  return (
    <Container fluid data-bs-theme={theme}>
      <header>
        <Routes>
          <Route path="/:date?" element={<BackArrow />} />
          <Route path="/:date?" element={<DisconnectionDate />} />
        </Routes>
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
            element={<DisconnectionStats totalDisconnections={totalDisconnections} totalDowntime={totalDowntime} longestDisconnection={longestDisconnection} />}
          />
        </Routes>
      </main>
    </Container>
  );
}

export default App;
