namespace BrewMonitor
{
    public interface ISensorSink
    {
        void Send(SensorEvent sensorEvent);
    }
}