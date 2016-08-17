using System;

namespace BrewMonitor
{
    public class SensorEvent
    {
        public SensorEvent()
        {
            DateTime = DateTime.Now;
        }

        public DateTime DateTime { get; set; }

        public double Temperature { get; set; }
    }
}