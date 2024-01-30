import { useEffect, useState } from "react";

export const useDisconnectionsData = () => {
  const [disconnections, setDisconnections] = useState<DisconnectionData[]>([]);
  const [loading, setLoading] = useState(true);
  const [totalDisconnections, setTotalDisconnections] = useState(0);
  const [longestDisconnection, setLongestDisconnection] = useState(0);
  const [disconnectionsByDate, setDisconnectionsByDate] = useState<{ [date: string]: DisconnectionData[] }> ({});

  useEffect(() => {
    const fetchData = async () => {
      const response = await fetch("http://localhost:5000/disconnections");
      const data = await response.json();
      const dataWithDates = data.map((item: any) => ({
        ...item,
        start: new Date(item.start),
      }));
      setDisconnections(dataWithDates);
      setTotalDisconnections(dataWithDates.length);
      setLongestDisconnection(
        [...dataWithDates].sort((a, b) => b.duration - a.duration)[0].downtime
      );
      const disconnectionsByDate = dataWithDates.reduce((acc: any, curr: any) => {
        const date = curr.start.toDateString();
        if (acc[date]) {
          acc[date].push(curr);
        } else {
          acc[date] = [curr];
        }
        return acc;
      }, {});
      setDisconnectionsByDate(disconnectionsByDate);
      setLoading(false);
    };
    fetchData();
  }, []);

  return { disconnections, loading, totalDisconnections, longestDisconnection, disconnectionsByDate };
};
