type DisconnectionProps = {
  date: Date;
}

export const DisconnectionDate = (props: DisconnectionProps) => {
  const { date } = props;
  const formattedDate = new Intl.DateTimeFormat('en-GB', {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
    weekday: 'short',
  }).format(date);

  return (
    <div className="disconnection-date">{formattedDate}</div>
  )
}