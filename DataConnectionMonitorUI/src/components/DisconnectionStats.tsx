import { ReactElement } from "react";

import './DisconnectionStats.css';

type DisconnectionStatsProps = {
  totalDisconnections: number;
  totalDowntime: number;
  longestDisconnection: DisconnectionData | undefined;
}

export const DisconnectionStats = (props: DisconnectionStatsProps): ReactElement => {
  const { totalDisconnections, totalDowntime, longestDisconnection } = props;

  const convertToDowntime = (time: string): string => {
    const timestring = time.toString();
    const hours = timestring.slice(0, 2);
    const minutes = timestring.slice(3, 5);
    const seconds = timestring.slice(6, 8);
    return `${hours}h ${minutes}m ${seconds}s`;
  }

  const longestDowntime = longestDisconnection
    ? convertToDowntime(longestDisconnection.downtime)
    : 'N/A';

  const totalDowntimeToString = (duration: number): string => {
    // convert total downtime to hours, minutes, seconds
    const totalDowntimeHours = Math.floor(duration / 3600);
    const totalDowntimeMinutes = Math.floor((duration % 3600) / 60);
    const totalDowntimeSeconds = (duration % 60).toFixed(0);

    return `${totalDowntimeHours}h ${totalDowntimeMinutes}m ${totalDowntimeSeconds}s`;
  }

  return (
    <div className="stats">
      <div className="details total-disconnections">
        <strong>{totalDisconnections}</strong><br />
        <span className="stats-text">Disconnections</span>
      </div>
      <div className="details total-downtime">
        <strong>{totalDowntimeToString(totalDowntime)}</strong><br />
        <span className="stats-text">Total downtime</span>
      </div>
      <div className="details longest-downtime">
        <strong>{longestDowntime}</strong><br />
        <span className="stats-text">Longest disconnection ({longestDisconnection?.start.toLocaleDateString('en-GB')})</span>
      </div>
    </div>
  )
}