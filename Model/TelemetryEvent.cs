using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class TelemetryEvent : EventBase
    {
        public new Telemetry Content { get; set; }

        public TelemetryEvent()
        {
            Event = "Telemetry";
            Version = "1.0";
        }
        
    }
}
