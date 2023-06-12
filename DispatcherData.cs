using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD2_Presence
{
    public class DispatcherData
    {
        public int id { get; set; }
        public int currentDuration { get; set; }
        public int dispatcherId { get; set; }
        public string dispatcherName { get; set; }
        public bool isOnline { get; set; }
        public long lastOnlineTimestamp { get; set; }
        public string region { get; set; }
        public string stationHash { get; set; }
        public string stationName { get; set; }
        public long timestampFrom { get; set; }
        public long timestampTo { get; set; }
        public int dispatcherLevel { get; set; }
        public bool dispatcherIsSupporter { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int dispatcherStatus { get; set; }
        public int dispatcherRate { get; set; }
        public List<string> statusHistory { get; set; }
    }
}
