import { useDisconnectionsData } from "./hooks/useDisconnectionsData";
import { DisconnectionDetails } from "./components/DisconnectionDetails";
import { Route, Routes } from "react-router-dom";
import { Container, Spinner } from "react-bootstrap";
import * as signalR from "@microsoft/signalr";
import { useTheme } from "./contexts/ThemeContext";

import "./App.css";
import { useEffect, useState } from "react";
import { CurrentConnectionStatus, ConnectionStatus } from "./components/CurrentConnectionStatus";
import { LastSuccessfulConnection } from "./components/LastSuccessfulConnection";
import { DisconnectionsList } from "./components/DisconnectionsList";
import { DisconnectionStats } from "./components/DisconnectionStats";
import { Header } from "./components/Header";
import { Footer } from "./components/Footer";

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

    connection.start()
      .catch((err) => {
        console.error("error connecting to disconnections hub", err);
      });

    connection.onclose(() => {
      setConnectionStatus(ConnectionStatus.Disconnected);
    });

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
      <Header />
      <main>
        <CurrentConnectionStatus connectionStatus={connectionStatus} />
        <LastSuccessfulConnection time={lastConnection} />
        <div className="title">Disconnections</div>
        <Routes>
          <Route
            path="/"
            element={<DisconnectionsList disconnections={groupedDisconnections} />}
          />
          <Route
            path="/:date"
            element={<DisconnectionDetails disconnections={disconnectionsByDate} />}
          />
          <Route
            path="/stats"
            element={<DisconnectionStats totalDisconnections={totalDisconnections} totalDowntime={totalDowntime} longestDisconnection={longestDisconnection} />}
          />
        </Routes>
      </main>
      <Footer />
    </Container>
  );
}

export default App;
