import { useEffect } from 'react';
import { Wifi, WifiOff, Exclamation } from 'react-bootstrap-icons';

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
  let element: JSX.Element = <Wifi />;

  useEffect(() => {
    if (connectionStatus === ConnectionStatus.Connected) {
      element = <Wifi />
    } else if (connectionStatus === ConnectionStatus.Retrying) {
      element = <Exclamation />
    } else {
      element = <WifiOff />
    }
  }, [connectionStatus]);

  return (
    <div className='connection-status'>
      <div className='connection-status-icon'>{element}</div>
      <div className='connection-status-text'>
        <strong>Connection Status</strong><br />
        {connectionStatus}
      </div>
    </div>
  )

}