import { ReactElement } from "react";
import { NavLink, useParams } from "react-router-dom";

type DisconnectionDetails = {
  disconnections: {
    [date: string]: DisconnectionData[];
  };
};

const Disconnections = (props: DisconnectionDetails): ReactElement => {
  const { disconnections } = props;
  const { date } = useParams();

  if (!date) {
    return (
      <>Invalid date</>
    )
  }

  if (!disconnections[date]) {
    return <div>No disconnections for this date</div>;
  }

  return (
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
  )
};

export { Disconnections };
