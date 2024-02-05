import { useDisconnectionsData } from "./hooks/useDisconnectionsData";
import { Disconnections } from "./components/Disconnections";
import { Route, Routes } from "react-router-dom";
import { Container, Spinner, FormCheck } from "react-bootstrap";
import { useTheme } from "./contexts/ThemeContext";

import "./App.css";

function App() {
  const {
    loading,
    totalDisconnections,
    longestDisconnection,
    disconnectionsByDate,
  } = useDisconnectionsData();

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
