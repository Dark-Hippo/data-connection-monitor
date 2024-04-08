import { useTitle } from '../contexts/TitleContext';

import './Title.css';

export const Title = () => {
  const { title } = useTitle();

  return (
    <div className="disconnection-date">{title}</div>
  )
}