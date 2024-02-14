import { useEffect, useState } from 'react';
import { Wifi, WifiOff, ExclamationOctagon } from 'react-bootstrap-icons';

import './CurrentConnectionStatus.css';

type CurrentConnectionStatusProps = {
  connectionStatus: ConnectionStatus;
}

export enum ConnectionStatus {
  Connected = "Connected",
  Disconnected = "Disconnected",
  Retrying = "Retrying",
}

export const CurrentConnectionStatus = (props: CurrentConnectionStatusProps) => {
  const { connectionStatus } = props;
  const [statusElement, setStatusElement] = useState<JSX.Element>(<Wifi />);

  useEffect(() => {
    if (connectionStatus === ConnectionStatus.Connected) {
      setStatusElement(<Wifi />);
    } else if (connectionStatus === ConnectionStatus.Retrying) {
      setStatusElement(<ExclamationOctagon />);
    } else {
      setStatusElement(<WifiOff />);
    }
  }, [connectionStatus]);

  return (
    <div className='connection-status'>
      <div className='connection-status-icon'>{statusElement}</div>
      <div className='connection-status-text'>
        <strong>Connection Status</strong><br />
        {connectionStatus}
      </div>
    </div>
  )

}