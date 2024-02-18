import { ReactElement } from "react";
import { DisconnectionsListItem } from "./DisconnectionsListItem";

type DisconnectionsListProps = {
  disconnections: {
    [date: string]: DisconnectionData[];
  };
};

export const DisconnectionsList = (props: DisconnectionsListProps): ReactElement => {
  const { disconnections } = props;

  return (
    <div>
      {Object.keys(disconnections).map((date) => {
        return (
          <DisconnectionsListItem
            key={date}
            date={date}
            disconnectionCount={disconnections[date].length}
          />
        );
      })}
    </div>
  );
};
