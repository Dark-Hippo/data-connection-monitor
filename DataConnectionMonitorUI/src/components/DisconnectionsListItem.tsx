import { ReactElement } from "react";
import { ArrowRight } from "react-bootstrap-icons";
import { NavLink } from "react-router-dom";

type DisconnectionListItemProps = {
  date: string;
  disconnectionCount: number;
};

export const DisconnectionsListItem = (props: DisconnectionListItemProps): ReactElement => {
  const { date, disconnectionCount } = props;

  return (
    <NavLink to={`/${date}`} style={{ textDecoration: "none" }}>
      <div className='connection-status' style={{ justifyContent: "space-between" }}>
        <div className='connection-status-text'>
          <strong>{date}</strong><br />
          {disconnectionCount}
        </div>
        <div className='connection-status-icon-right'><ArrowRight /></div>
      </div>
    </NavLink>
  )
}