import { ReactElement } from "react";

import "./disconnections.css";
import { Disconnection } from "./Disconnection";
import { NavLink, useParams } from "react-router-dom";

const Disconnections = (props: DisconnectionsProps): ReactElement => {
  const { disconnections } = props;
  const { date } = useParams();

  return date ? (
    <div>
      <hr />
      <NavLink to="/">Back to all disconnections</NavLink>
      <h4>{date}</h4>
      <div className="">
        {disconnections[date].map((disconnection) => 
          <div key={disconnection.start.toISOString()}>{disconnection.start.getHours().toString().padStart(2, '0')}:{disconnection.start.getMinutes().toString().padStart(2, '0')} - {disconnection.downtime}</div>
        )}
    </div>
    </div>
  ) : (
    <div className="d-flex flex-wrap disconnections-container">
      {Object.keys(disconnections).map((date) => {
        return (
          <Disconnection
            key={date}
            date={date}
            disconnections={disconnections[date]}
          />
        );
      })}
    </div>
  );
};

export { Disconnections };
