using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LSIS
{
    class VideoPlayer
    {
        private string TEMP_PNG_NAME = "Temp.png";
        private int marker = 0;
        private double speedRatio = 1;
        private int play = 0;
        public bool sldrDragStart = false;
        public int Naturalwidth = 0;
        public int Naturalheight = 0;

        public int MarkerVal()
        {
            marker++;
            if (marker == 2)
            {
                marker = 0;
            }
            return marker;
        }
        public int GetMarker()
        {
            return marker;
        }
        public double GetSpeed()
        {
            return speedRatio;
        }
        public double SpeedInit()
        {
            speedRatio = 1;
            return speedRatio;
        }
        public double SpeedUp()
        {
            if (speedRatio != 8)
            {
                speedRatio *= 2;
            }

            return speedRatio;
        }
        public double SpeedDown()
        {
            if (speedRatio != 0.25)
            {
                speedRatio /= 2;
            }

            return speedRatio;
        }
        public int PlayVal()
        {
            play++;
            if (play == 2)
            {
                play = 0;
            }
            return play;
        }
        public void Play(int i)
        {
            play = i;
        }
        public int GetPlay()
        {
            return play;
        }
        public int GetWidth()
        {
            return Naturalwidth;
        }
        public int GetHight()
        {
            return Naturalheight;
        }
        public Mat GetRender(UIElement source, double dpi)
        {
            System.Windows.Rect bounds = VisualTreeHelper.GetDescendantBounds(source);

            var scale = dpi / 96.0;
            var width = (bounds.Width + bounds.X) * scale;
            var height = (bounds.Height + bounds.Y) * scale;

            RenderTargetBitmap rtb =
                new RenderTargetBitmap((int)Math.Round(width, MidpointRounding.AwayFromZero),
                (int)Math.Round(height, MidpointRounding.AwayFromZero),
                dpi, dpi, PixelFormats.Pbgra32);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(source);
                ctx.DrawRectangle(vb, null,
                    new System.Windows.Rect(new System.Windows.Point(bounds.X, bounds.Y), new System.Windows.Point(width, height)));
            }

            rtb.Render(dv);
            PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
            pngBitmapEncoder.Frames.Add(BitmapFrame.Create(rtb));
            using (Stream stream = File.Create(TEMP_PNG_NAME))
            {
                pngBitmapEncoder.Save(stream);
            }
            Mat image = new Mat();
            Cv2.Resize(Cv2.ImRead(TEMP_PNG_NAME), image, new OpenCvSharp.Size(Naturalwidth, Naturalheight));
            return image;
        }
    }
}
