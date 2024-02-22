import { ReactElement } from "react";
import { useParams } from "react-router-dom";
import { ExclamationDiamond } from "react-bootstrap-icons";

type DisconnectionDetailsProps = {
  disconnections: {
    [date: string]: DisconnectionData[];
  };
};

const DisconnectionDetails = (props: DisconnectionDetailsProps): ReactElement => {
  const { disconnections } = props;
  const { date } = useParams();

  if (!date) {
    return (
      <>Invalid date</>
    )
  }

  const dateStr = new Date(date).toDateString();

  if (!disconnections[dateStr]) {
    return <div>No disconnections for this date</div>;
  }

  return (
    <div style={{ display: "flex", flexDirection: "column", gap: "1rem" }}>
      {disconnections[dateStr].map((disconnection) =>
        <div key={disconnection.start.toISOString()} className="connection-status">
          <div className='connection-status-icon'><ExclamationDiamond /></div>
          <div className='connection-status-text'>
            <strong>{disconnection.start.getHours().toString().padStart(2, '0')}:{disconnection.start.getMinutes().toString().padStart(2, '0')}:{disconnection.start.getSeconds().toString().padStart(2, '0')}</strong><br />
            Duration {disconnection.downtime}
          </div>
        </div>
      )}
    </div>
  )
};

export { DisconnectionDetails as Disconnections };
