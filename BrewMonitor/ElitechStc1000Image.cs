using AForge.Imaging;
using AForge.Imaging.Filters;
using Aspose.OCR;
using System.Threading;
using Tesseract;

namespace BrewMonitor
{
    public class ElitechStc1000Image
    {
        private string filename;

        private const string processedFileName = "stc-1000-1-processed.jpg";

        private TesseractEngine engine;

        public ElitechStc1000Image(string filename)
        {
            this.filename = filename;
        }

        public ElitechStc1000Image(string filename, TesseractEngine engine) : this(filename)
        {
            this.engine = engine;
        }

        public double GetTemperature()
        {
            var image = Image.FromFile(filename);
                        
            var grayscale = new Grayscale(0.2125, 0.7154, 0.0721);
            image = grayscale.Apply(image);

            var invert = new Invert();
            image = invert.Apply(image);

            var stats = new ImageStatistics(image);
            var levelsLinear = new LevelsLinear
            {
                InGray = stats.Gray.GetRange(0.90)
            };

            image = levelsLinear.Apply(image);

            var contrast = new ContrastStretch();
            image = contrast.Apply(image);

            var erosion = new Erosion();
            image = erosion.Apply(image);

            var blur = new GaussianBlur(4, 15);
            image = blur.Apply(image);

            var threshold = new Threshold(140);
            image = threshold.Apply(image);

            image.Save(processedFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            image.Dispose();
            Thread.Sleep(5000);
            var text = Recognise();

            var textArr = text.Split(',');
            var temp = 0d;

            if (textArr.Length == 2 && double.TryParse(textArr[1], out temp))
            {
                return temp;
            }

            return temp;
        }

        private string Recognise()
        {
            try
            {
                using (var img = Pix.LoadFromFile(processedFileName))
                {
                    using (var page = engine.Process(img))
                    {
                        return page.GetText();
                    }
                }
            }
            catch (System.Exception)
            {

                return string.Empty;
            }
            
            //return "Rubbish";          
            //OcrEngine ocrEngine = new OcrEngine();

            ////Set the Image property by loading the image from file path location or an instance of MemoryStream
            //ocrEngine.Image = ImageStream.FromFile(processedFileName);

            ////Process the image
            //if (ocrEngine.Process())
            //{
            //    //Display the recognized text
            //    return ocrEngine.Text.ToString();
            //}

            //return string.Empty;
        }
    }
}