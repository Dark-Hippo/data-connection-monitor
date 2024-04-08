import { ReactElement } from "react";
import { ArrowRight } from "react-bootstrap-icons";
import { NavLink } from "react-router-dom";

type DisconnectionListItemProps = {
  date: Date;
  disconnectionCount: number;
};

export const DisconnectionsListItem = (props: DisconnectionListItemProps): ReactElement => {
  const { date, disconnectionCount } = props;

  // url encode the date to use as a route
  const dateStr = date.toISOString().split('T')[0];

  return (
    <NavLink to={`/${dateStr}`} style={{ textDecoration: "none" }}>
      <div className='connection-status' style={{ justifyContent: "space-between" }}>
        <div className='connection-status-text'>
          <strong>{DayDateString(date)}</strong><br />
          {disconnectionCount}
        </div>
        <div className='connection-status-icon-right'><ArrowRight /></div>
      </div>
    </NavLink>
  )
}

const DayDateString = (date: Date): string => {
  const today = new Date();
  if (date.getDate() === today.getDate() && date.getMonth() === today.getMonth() && date.getFullYear() === today.getFullYear()) {
    return "Today";
  } else if (date.getDate() === today.getDate() - 1 && date.getMonth() === today.getMonth() && date.getFullYear() === today.getFullYear()) {
    return "Yesterday";
  } else {
    return date.toLocaleDateString('en-GB');
  }
}