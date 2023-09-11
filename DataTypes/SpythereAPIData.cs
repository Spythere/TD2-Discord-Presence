namespace TD2_Presence.Classes
{
    public class PlayerActivityData
    {
        public DriverData? driver { get; set; }
        public IList<DispatcherData>? dispatcher { get; set; }
        public long lastUpdateTimestamp { get; set; }
        public long nextUpdateTimestamp { get; set; }
    }
}
