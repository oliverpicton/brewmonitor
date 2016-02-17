using System;
using System.Drawing;
using System.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using Microsoft.ServiceBus.Messaging;
using System.Text;
using Newtonsoft.Json;
using System.Configuration;

namespace BrewMonitor
{
    class Program
    {
        private static VideoCaptureDevice videoSource;

        static void Main(string[] args)
        {

            while (true)
            {
                videoSource = new VideoCaptureDevice(new FilterInfoCollection(FilterCategory.VideoInputDevice)[0].MonikerString);
                videoSource.NewFrame += CaptureFrame;

                videoSource.Start();
                Thread.Sleep(1000);
                videoSource.Stop();

                var engine = new TesseractEngine(@"tessdata", "letsgodigital", EngineMode.Default);

                var image = new ElitechStc1000Image("stc-1000-real.jpg", engine);

                var temp = image.GetTemperature();

                Console.WriteLine(temp);

                var eventHubClient = EventHubClient.CreateFromConnectionString(ConfigurationManager.AppSettings["EventHubSendConnection"], "stc1000statsin");

                try
                {
                    var sensorEvent = new SensorEvent
                    {
                        Temperature = temp
                    };

                    var serializedString = JsonConvert.SerializeObject(sensorEvent);
                    var data = new EventData(Encoding.Unicode.GetBytes(serializedString))
                    {
                        PartitionKey = "temperature-sensor"
                    };

                    Console.WriteLine("{0} > Sending temperature: {1}", DateTime.Now, temp);
                    eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(serializedString)));
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                    Console.ResetColor();
                }

                Thread.Sleep(2000);
            }
        }

        private static void CaptureFrame(object sender, NewFrameEventArgs eventArgs)
        {
            var bitmap = (Bitmap)eventArgs.Frame.Clone();
            bitmap.Save(String.Format("stc1000_{0:yyyyMMdd}_{0:HHmmss}.jpg", DateTime.Now), ImageFormat.Jpeg);
        }
    }
}