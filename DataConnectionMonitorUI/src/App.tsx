import { useDisconnectionsData } from "./useDisconnectionsData";
import { Disconnections } from "./components/Disconnections";
import { Route, Routes } from "react-router-dom";
import { Spinner } from "react-bootstrap";

import "bootstrap/dist/css/bootstrap.min.css";
import "./App.css";

function App() {
  const {
    loading,
    totalDisconnections,
    longestDisconnection,
    disconnectionsByDate,
  } = useDisconnectionsData();

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
    <>
      <div className="container">
        <header>
          <h1>Disconnections</h1>
          <h3>
            Total disconnections since monitoring began: {totalDisconnections}
          </h3>
          <h3>Longest disconnection: {longestDisconnection}</h3>
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
      </div>
    </>
  );
}

export default App;
