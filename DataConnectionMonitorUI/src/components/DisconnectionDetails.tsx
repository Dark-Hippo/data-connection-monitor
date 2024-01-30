type DisconnectionDetailsProps = {
    date: String,
    disconnectionDetails: DisconnectionData[]
}

export const DisconnectionDetails = (props: DisconnectionDetailsProps) => {
    const { date, disconnectionDetails } = props;
    return (
        <div>
            <h4>{date}</h4>
        </div>
    )
}