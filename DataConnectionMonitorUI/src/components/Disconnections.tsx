import { ReactElement } from "react";

import './disconnections.css';
import { Disconnection } from "./Disconnection";

const Disconnections = (props: DisconnectionsProps): ReactElement => {
  const { disconnections } = props;
  return (
    <div className="d-flex flex-wrap disconnections-container">
      {Object.keys(disconnections).map((date) => {
        return (
          <Disconnection key={date} date={date} disconnections={disconnections[date]} />
        );
      })}
    </div>
  );
};

export { Disconnections}