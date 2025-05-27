import { BarChartLine, BarChartLineFill, House, HouseFill } from "react-bootstrap-icons";
import { NavLink, useLocation } from "react-router-dom";

import "./Footer.css";

export const Footer = () => {
  const location = useLocation();

  return (
    <footer>
      <nav>
        <NavLink to='/'>
          {location.pathname === '/' ?
            <HouseFill className="icon" /> :
            <House className="icon" />
          }
          Home
        </NavLink>
        <NavLink to='/stats'>
          {location.pathname === '/stats' ?
            <BarChartLineFill className="icon" /> :
            <BarChartLine className="icon" />
          }
          Stats
        </NavLink>
        {/* <NavLink to='/settings'>
          {location.pathname === '/settings' ?
            <GearFill className="icon" /> :
            <Gear className="icon" />
          }
          Settings
        </NavLink> */}
      </nav>
    </footer>
  );
}