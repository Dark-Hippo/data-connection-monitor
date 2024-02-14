import { useDisconnectionsData } from "./hooks/useDisconnectionsData";
import { Disconnections } from "./components/Disconnections";
import { Route, Routes } from "react-router-dom";
import { Container, Spinner } from "react-bootstrap";
import * as signalR from "@microsoft/signalr";
import { useTheme } from "./contexts/ThemeContext";

import "./App.css";
import { useRef, useState } from "react";
import { ThemeSwitcher } from "./components/ThemeSwitcher";
import { DisconnectionDate } from "./components/DisconnectionDate";
import { BackArrow } from "./components/BackArrow";
import { CurrentConnectionStatus, ConnectionStatus } from "./components/CurrentConnectionStatus";

function App() {
  const {
    loading,
    totalDisconnections,
    longestDisconnection,
    disconnectionsByDate,
  } = useDisconnectionsData();

  const currentConnectionStatus = useRef<HTMLSpanElement>(null);
  const lastSuccessfulConnection = useRef<HTMLSpanElement>(null);

  const [connectionStatus, setConnectionStatus] = useState<ConnectionStatus>(ConnectionStatus.Connected);

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
    lastSuccessfulConnection.current!.innerText = lastConnection;
  });

  connection.on("CurrentConnectionStatus", (status: string) => {
    setConnectionStatus(status as ConnectionStatus);
    const s = status as ConnectionStatus;
    console.log("connection status: ", s);
    s === ConnectionStatus.Connected ?
      currentConnectionStatus.current!.innerText = "True" :
      currentConnectionStatus.current!.innerText = "False";
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
        <h3>
          Total disconnections since monitoring began: <span className="primary-emphasis">{totalDisconnections}</span>
        </h3>
        <h3>Longest disconnection: <span className="primary-emphasis">{longestDisconnection}</span></h3>
        <h3>Last successful connection: <span className="primary-emphasis" ref={lastSuccessfulConnection}></span></h3>
        <h3>Current connection status: <span className="primary-emphasis" ref={currentConnectionStatus}></span></h3>
        <Routes>
          <Route
            path="/"
            element={<Disconnections disconnections={disconnectionsByDate} />}
          />
          <Route
            path="/:date"
            element={<Disconnections disconnections={disconnectionsByDate} />}
          />
        </Routes>
      </main>
    </Container>
  );
}

export default App;
