using System.Text;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace BrewMonitor.Sinks
{
    public class AzuresStreamAnalytics : ISensorSink
    {
        public string ConnectionString { get; private set; }

        public void Send(SensorEvent sensorEvent)
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(ConnectionString);

            var serializedString = JsonConvert.SerializeObject(sensorEvent);

            eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(serializedString)));
        }

        public AzuresStreamAnalytics WithConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
            return this;
        }
    }
}