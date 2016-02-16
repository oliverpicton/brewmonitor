using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using AForge.Video;
using AForge.Video.DirectShow;

namespace BrewMonitor
{
    class Program
    {
        static void Main(string[] args)
        {            
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice );
        
            var videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString );

            videoSource.NewFrame += new NewFrameEventHandler( video_NewFrame );
            
            videoSource.Start();
        }

        private static void video_NewFrame(object sender,
        NewFrameEventArgs eventArgs)
        {
            var bitmap = (Bitmap)eventArgs.Frame.Clone();
            bitmap.Save(String.Format("stc1000_{0:yyyyMMdd}_{0:HHmmss}.jpg", DateTime.Now), ImageFormat.Jpeg);
        }
    }
}