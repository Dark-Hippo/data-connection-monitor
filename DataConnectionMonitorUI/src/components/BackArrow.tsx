import { ArrowLeft } from 'react-bootstrap-icons';
import { NavLink } from 'react-router-dom';

export const BackArrow = () => {
  return (
    <NavLink to={'/'}>
      <ArrowLeft style={{ width: "1em", height: "1em" }} />
    </NavLink>
  )
}