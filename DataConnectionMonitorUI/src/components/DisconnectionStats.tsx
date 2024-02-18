import { ReactElement } from "react";

type DisconnectionStatsProps = {
  totalDisconnections: number;
  totalDowntime: string;
  longestDisconnection: number;
}

export const DisconnectionStats = (props: DisconnectionStatsProps): ReactElement => {
  const { totalDisconnections, totalDowntime, longestDisconnection: longestDowntime } = props;

  return (
    <>
      <ul>
        <li>{totalDisconnections} total disconnections</li>
        <li>{totalDowntime} of total downtime</li>
        <li>Longest downtime of {longestDowntime}</li>
      </ul>
    </>
  )
}