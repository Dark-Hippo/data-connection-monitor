import { useTitle } from '../contexts/TitleContext';

export const Title = () => {
  const { title } = useTitle();

  return (
    <div className="disconnection-date">{title}</div>
  )
}