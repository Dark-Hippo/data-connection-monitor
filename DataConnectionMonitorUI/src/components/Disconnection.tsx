import { useState } from "react";
import { DisconnectionDetails } from "./DisconnectionDetails";

export const Disconnection = (props: DisconnectionProps) => {
  const { date, disconnections } = props;
  const [selectedDisconnection, setSelectedDisconnection] = useState(null);


  if(selectedDisconnection) {
    return <DisconnectionDetails date={date} disconnectionDetails={selectedDisconnection} />;
  } else {

  return (
    <div key={date} className="bg-light m-2 p-2" onClick={() => setSelectedDisconnection(disconnections)} >
      <div>
        {date} - {disconnections.length}
      </div>
    </div>
  );
  }
};
