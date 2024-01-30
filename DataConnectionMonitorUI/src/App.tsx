import {
  disconnectionsData as disconnections,
} from "./disconnectionsData";
import { Disconnections } from "./components/Disconnections";

import "bootstrap/dist/css/bootstrap.min.css";
import "./App.css";

function App() {
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
          <Disconnections disconnections={disconnectionsByDate} />
        </main>
      </div>
    </>
  );
}


export default App;
