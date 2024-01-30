import {
  useDisconnectionsData
} from "./useDisconnectionsData";
import { Disconnections } from "./components/Disconnections";
import { Route, Routes } from "react-router-dom";

import "bootstrap/dist/css/bootstrap.min.css";
import "./App.css";

function App() {
  const {data: disconnections, loading} = useDisconnectionsData();
  
  if (loading) {
    return <div>Loading...</div>;
  }

  const disconnectionsByDate: { [date: string]: DisconnectionData[] } = {};

  disconnections.forEach((disconnection) => {
    const date = disconnection.start.toISOString().split("T")[0];

    if (!disconnectionsByDate[date]) {
      disconnectionsByDate[date] = [];
    }

    disconnectionsByDate[date].push(disconnection);
  });

  // sort disconnections by start date
  const disconnectionsData = disconnections.sort(
    (a, b) => a.start.getTime() - b.start.getTime()
  );

  const totalDisconnections = disconnectionsData.length;
  const longestDisconnection = [...disconnectionsData].sort(
    (a, b) => b.duration - a.duration
  )[0].downtime;

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
            <Route path="/" element={<Disconnections disconnections={disconnectionsByDate} />} />
            <Route path="/:date" element={<Disconnections disconnections={disconnectionsByDate} />} />
          </Routes>
        </main>
      </div>
    </>
  );
}


export default App;
