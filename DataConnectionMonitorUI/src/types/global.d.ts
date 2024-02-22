type DisconnectionData = {
  start: Date;
  duration: number;
  downtime: string;
};

type GroupedDisconnection = {
  date: Date;
  disconnections: [
    {
      start: Date;
      downtime: number;
    },
  ];
  totalDowntime: number;
  averageDowntime: number;
};
