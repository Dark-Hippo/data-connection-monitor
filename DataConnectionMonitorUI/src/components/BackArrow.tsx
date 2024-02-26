import { ArrowLeft } from 'react-bootstrap-icons';
import { NavLink, useParams } from 'react-router-dom';

export const BackArrow = () => {

  const { date } = useParams();

  if (!date) return <div />;

  return (
    <NavLink to={'/'}>
      <ArrowLeft style={{ width: "1em", height: "1em" }} />
    </NavLink>
  )
}