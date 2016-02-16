using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AForge.Video;
using AForge.Video.DirectShow;
using Tesseract;

using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace BrewMonitor
{
    class Program
    {
        static void Main(string[] args)
        {            
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice );
        
                var videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString );

            videoSource.NewFrame += CaptureFrame;
            
            videoSource.Start();

            Thread.Sleep(10000);

            videoSource.Stop();

            using (var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(@"stc1000test.jpg"))
                {                   
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        Console.WriteLine(text);
                    }                    
                }
            }
        }

        private static void CaptureFrame(object sender, NewFrameEventArgs eventArgs)
        {
            var bitmap = (Bitmap)eventArgs.Frame.Clone();
            bitmap.Save(String.Format("stc1000_{0:yyyyMMdd}_{0:HHmmss}.jpg", DateTime.Now), ImageFormat.Jpeg);
        }
    }
}