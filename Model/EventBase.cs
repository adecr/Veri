using System;

namespace Model
{
    public class EventBase
    {
        public string DeviceID { get; set; }
        public string Event { get; set; }
        public string Version { get; set; }
        public object Content { get; set; }
    }
}
