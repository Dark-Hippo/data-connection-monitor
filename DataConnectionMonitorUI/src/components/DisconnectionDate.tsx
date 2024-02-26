import { useParams } from "react-router-dom";

export const DisconnectionDate = () => {
  const { date } = useParams();

  if (!date) return (<div></div>);

  const dateStr = new Date(date);

  const formattedDate = new Intl.DateTimeFormat('en-GB', {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
    weekday: 'short',
  }).format(dateStr);

  return (
    <div className="disconnection-date">{formattedDate}</div>
  )
}