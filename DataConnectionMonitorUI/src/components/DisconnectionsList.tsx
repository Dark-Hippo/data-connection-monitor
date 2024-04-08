import { ReactElement, useEffect } from "react";
import { DisconnectionsListItem } from "./DisconnectionsListItem";
import { useTitle } from "../contexts/TitleContext";

type DisconnectionsListProps = {
  disconnections: GroupedDisconnection[];
};

export const DisconnectionsList = (props: DisconnectionsListProps): ReactElement => {
  const { disconnections } = props;
  const { setTitle } = useTitle();

  useEffect(() => {
    setTitle("Connections Monitor");
  });

  // sort disconnections by date order
  disconnections.sort((a, b) => b.date.getTime() - a.date.getTime());

  return (
    <div>
      {disconnections.map((day) => {
        return (
          <DisconnectionsListItem
            key={day.date.getTime()}
            date={day.date}
            disconnectionCount={day.disconnections.length}
          />
        );
      })}
    </div>
  );
};
