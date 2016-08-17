using System.Collections.Generic;
using AForge.Imaging;
using AForge.Imaging.Filters;
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

        public IEnumerable<string> Colours  { get; set; }

        public double GetTemperature()
        {
            var temp = 0.0;

            var image = Image.FromFile(filename);

            var grayscale = new Grayscale(0.2125, 0.7154, 0.0721);
            image = grayscale.Apply(image);

            var invert = new Invert();
            image = invert.Apply(image);

            var stats = new ImageStatistics(image);
            var levelsLinear = new LevelsLinear
            {
                InGray = stats.Gray.GetRange(2.90)
            };

            image = levelsLinear.Apply(image);

            var contrast = new ContrastStretch();
            image = contrast.Apply(image);

            var erosion = new Erosion();
            image = erosion.Apply(image);

            var blur = new GaussianBlur(2, 3);
            image = blur.Apply(image);

            var threshold = new Threshold(79);
            image = threshold.Apply(image);

            image.Save(processedFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            image.Dispose();
            var text = Recognise();

            double.TryParse(text.Replace(',', '.'), out temp);

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
                //TODO: Add in exception logging
                return string.Empty;
            }            
        }
    }
}