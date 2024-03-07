import { ReactElement } from "react";

import './DisconnectionStats.css';

type DisconnectionStatsProps = {
  totalDisconnections: number;
  totalDowntime: number;
  longestDisconnection: number;
}

export const DisconnectionStats = (props: DisconnectionStatsProps): ReactElement => {
  const { totalDisconnections, totalDowntime, longestDisconnection } = props;

  const convertToDowntime = (time: number): string => {
    const timestring = time.toString();
    const hours = timestring.slice(0, 2);
    const minutes = timestring.slice(3, 5);
    const seconds = timestring.slice(6, 8);
    return `${hours}h ${minutes}m ${seconds}s`;
  }

  const longestDowntime = convertToDowntime(longestDisconnection);

  return (
    <div className="stats">
      <div className="details total-disconnections">
        <strong>{totalDisconnections}</strong><br />
        <span className="stats-text">Disconnections</span>
      </div>
      <div className="details total-downtime">
        <strong>{totalDowntime}</strong><br />
        <span className="stats-text">Total downtime</span>
      </div>
      <div className="details longest-downtime">
        <strong>{longestDowntime}</strong><br />
        <span className="stats-text">Longest disconnection</span>
      </div>
    </div>
  )
}