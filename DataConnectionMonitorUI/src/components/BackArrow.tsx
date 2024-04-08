import { ArrowLeft } from 'react-bootstrap-icons';
import { NavLink, useLocation } from 'react-router-dom';

import './BackArrow.css';

export const BackArrow = () => {

  const location = useLocation();

  if (location.pathname === '/') {
    return <div></div>;
  }

  return (
    <NavLink className='back-arrow' to={'/'}>
      <ArrowLeft />
    </NavLink>
  )
}