import { useEffect, useState } from "react";

export const useDisconnectionsData = () => {
  const [loading, setLoading] = useState(true);
  const [totalDisconnections, setTotalDisconnections] = useState(0);
  const [longestDisconnection, setLongestDisconnection] =
    useState<DisconnectionData>();
  const [disconnectionsByDate, setDisconnectionsByDate] = useState<{
    [date: string]: DisconnectionData[];
  }>({});
  const [groupedDisconnections, setGroupedDisconnections] = useState<
    GroupedDisconnection[]
  >([]);
  const [totalDowntime, setTotalDowntime] = useState(0);

  useEffect(() => {
    const fetchData = async () => {
      const response = await fetch("/api/disconnections");

      if (!response.ok) {
        console.error("Failed to fetch disconnections data", response);
        setLoading(false);
        return;
      }

      const data = await response.json();
      const dataWithDates = data.map((item: any) => ({
        ...item,
        start: new Date(item.start),
      }));

      setTotalDisconnections(dataWithDates.length);

      setLongestDisconnection(
        [...dataWithDates].sort((a, b) => b.duration - a.duration)[0]
      );

      const disconnectionsByDate = dataWithDates.reduce(
        (acc: any, curr: any) => {
          const date = curr.start.toDateString();
          if (acc[date]) {
            acc[date].push(curr);
          } else {
            acc[date] = [curr];
          }
          return acc;
        },
        {}
      );

      setDisconnectionsByDate(disconnectionsByDate);

      const grouped: GroupedDisconnection[] = [];
      for (const date in disconnectionsByDate) {
        const disconnections = disconnectionsByDate[date];
        const totalDowntime = disconnections.reduce(
          (acc: number, curr: any) => acc + curr.duration,
          0
        );
        const averageDowntime = totalDowntime / disconnections.length;
        grouped.push({
          date: new Date(date),
          disconnections: disconnections.map((disconnection: any) => ({
            start: disconnection.start,
            downtime: disconnection.downtime,
          })),
          totalDowntime,
          averageDowntime,
        });
      }
      setGroupedDisconnections(grouped);

      setTotalDowntime(
        grouped.reduce((acc, curr) => acc + curr.totalDowntime, 0)
      );

      setLoading(false);
    };
    fetchData();
  }, []);

  return {
    loading,
    totalDisconnections,
    longestDisconnection,
    disconnectionsByDate,
    groupedDisconnections,
    totalDowntime,
  };
};
