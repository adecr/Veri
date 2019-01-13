using System;

namespace Model
{
    public class Telemetry
    {
        public string BoilerType { get; set; }
        public string BoilerModel { get; set; }
        public string BoilerSerial { get; set; }
        public DateTime Timestamp { get; set; }
        public float TemperatureC { get; set; }
        public float PressureBar { get; set; }
    }
}