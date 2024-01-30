import { useEffect, useState } from "react";

export const useDisconnectionsData = () => {
  const [data, setData] = useState<DisconnectionData[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      const response = await fetch('http://localhost:5000/disconnections');
      const data = await response.json();
      const dataWithDates = data.map((item: any) => ({
        ...item,
        start: new Date(item.start),

      }));
      setData(dataWithDates);
      setLoading(false);
    }
    fetchData();
  }, []);

  return {data, loading}
}