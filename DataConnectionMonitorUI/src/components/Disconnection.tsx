import { NavLink } from "react-router-dom";

export const Disconnection = (props: DisconnectionProps) => {
  const { date, disconnections } = props;

  return (
    <NavLink to={`/${date}`} className="bg-light m-2 p-2 btn btn-light" >
      <div>
        {date} - {disconnections.length}
      </div>
    </NavLink>
  );
};
