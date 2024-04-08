import { useEffect, useState } from 'react';
import { Clock } from 'react-bootstrap-icons';

import './CurrentConnectionStatus.css';

type LastSuccessfulConnectionProps = {
  time?: string;
}

export const LastSuccessfulConnection = (props: LastSuccessfulConnectionProps) => {
  const { time } = props;
  const [displayTime, setDisplayTime] = useState<string>("");

  useEffect(() => {
    const newTime = time?.substring(time.length - 9, time.length) ?? "";
    setDisplayTime(newTime);
  }, [time]);

  return (
    <div className='connection-status'>
      <div className='connection-status-icon'><Clock /></div>
      <div className='connection-status-text'>
        <strong>Last successful test</strong><br />
        {displayTime}
      </div>
    </div>
  )
}