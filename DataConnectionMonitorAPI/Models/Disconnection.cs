using CsvHelper.Configuration.Attributes;

namespace DataConnectionMonitorAPI
{

    public class Disconnection
    {
        [Index(0)]
        public DateTime Start { get; set; }
        [Index(1)]
        public double Duration { get; set; }

        public string Downtime
        {
            get
            {
                var downtime = TimeSpan.FromSeconds(Duration);
                return downtime.ToString(@"hh\:mm\:ss");
            }
        }
    }
}