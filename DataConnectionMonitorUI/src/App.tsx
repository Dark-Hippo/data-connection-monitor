import { useDisconnectionsData } from "./hooks/useDisconnectionsData";
import { Disconnections } from "./components/Disconnections";
import { Route, Routes } from "react-router-dom";
import { Container, Spinner, FormCheck } from "react-bootstrap";
import * as signalR from "@microsoft/signalr";
import { useTheme } from "./contexts/ThemeContext";

import "./App.css";
import { useRef } from "react";

function App() {
  const {
    loading,
    totalDisconnections,
    longestDisconnection,
    disconnectionsByDate,
  } = useDisconnectionsData();

  const currentConnectionStatus = useRef<HTMLSpanElement>(null);
  const lastSuccessfulConnection = useRef<HTMLSpanElement>(null);

  const {theme, toggleTheme} = useTheme();

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
    currentConnectionStatus.current!.innerText = status;
  });

  connection.start().then(() => {
    console.log("connected to disconnections hub!");
  }).catch((err) => {
    console.log("error connecting to disconnections hub", err);
  });

  return (
    <Container fluid data-bs-theme={theme}>
        <header>
        <h1>
          <FormCheck style={{fontSize: "1rem"}} type="switch" id="theme-toggle" label="Dark mode" onChange={toggleTheme} checked={theme === 'dark'} />
          Disconnections Monitor
        </h1>
          <h3>
            Total disconnections since monitoring began: <span className="primary-emphasis">{totalDisconnections}</span>
          </h3>
          <h3>Longest disconnection: <span className="primary-emphasis">{longestDisconnection}</span></h3>
          <h3>Last successful connection: <span className="primary-emphasis" ref={lastSuccessfulConnection}></span></h3>
          <h3>Current connection status: <span className="primary-emphasis" ref={currentConnectionStatus}></span></h3>
        </header>
        <main>
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
