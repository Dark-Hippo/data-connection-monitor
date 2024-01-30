type DisconnectionsProps = {
  disconnections: { 
    [date: string]: DisconnectionData[] 
  };
};

type DisconnectionData = {
  start: Date;
  duration: number;
  downtime: string;
};

type DisconnectionProps = {
  date: string;
  disconnections: any;
};
