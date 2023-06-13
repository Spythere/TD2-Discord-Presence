namespace TD2_Presence.Classes
{
    public class ActiveTrain
    {
        public int trainNo { get; set; }
        public int mass { get; set; }
        public int speed { get; set; }
        public int length { get; set; }
        public int distance { get; set; }
        public string stockString { get; set; }
        public string driverName { get; set; }
        public int driverId { get; set; }
        public bool driverIsSupporter { get; set; }
        public int driverLevel { get; set; }
        public string currentStationHash { get; set; }
        public string currentStationName { get; set; }
        public string signal { get; set; }
        public string connectedTrack { get; set; }
        public int online { get; set; }
        public object lastSeen { get; set; }
        public string region { get; set; }
        public bool isTimeout { get; set; }
        public ActiveTrainTimetable timetable { get; set; }
    }

    public class ActiveTrainTimetableStop
    {
        public string stopName { get; set; }
        public string stopNameRAW { get; set; }
        public string stopType { get; set; }
        public double stopDistance { get; set; }
        public string pointId { get; set; }
        public string comments { get; set; }
        public bool mainStop { get; set; }
        public string arrivalLine { get; set; }
        public long arrivalTimestamp { get; set; }
        public long arrivalRealTimestamp { get; set; }
        public int arrivalDelay { get; set; }
        public string departureLine { get; set; }
        public long departureTimestamp { get; set; }
        public long departureRealTimestamp { get; set; }
        public int departureDelay { get; set; }
        public bool beginsHere { get; set; }
        public bool terminatesHere { get; set; }
        public int confirmed { get; set; }
        public int stopped { get; set; }
        public int? stopTime { get; set; }
    }

    public class ActiveTrainTimetable
    {
        public bool SKR { get; set; }
        public bool TWR { get; set; }
        public string category { get; set; }
        public List<ActiveTrainTimetableStop> stopList { get; set; }
        public string route { get; set; }
        public int timetableId { get; set; }
        public List<string> sceneries { get; set; }
    }
}
