using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS
{
    class ImageProcessingReview
    {
        public Mat image { get; set; }

        int TagX;
        int TagY;
        int Grid = 0;
        int GridVal = 100;
        int ColorMap = 0;
        public void GridSet()
        {
            Grid++;
            if (Grid == 2)
            {
                Grid = 0;
            }
        }
        public void Colormap()
        {
            ColorMap++;
            if (ColorMap == 3)
            {
                ColorMap = 0;
            }
        }

        public Mat View()
        {
            Mat img;
            if (image != null)
            {
                img = image.Clone();
                img = ColormapView(img);
                img = GridView(img, GridVal);
            }
            else
            {
                img = null;
            }
            return img;
        }
        private Mat ColormapView(Mat src)
        {
            Mat img = src;
            if (ColorMap == 1)
            {
                Cv2.CvtColor(img, img, ColorConversionCodes.BGR2GRAY);
                Mat Not_image = new Mat();
                Cv2.BitwiseNot(img, Not_image);
                Cv2.ApplyColorMap(Not_image, img, ColormapTypes.Rainbow);
                return img;
            }
            else if (ColorMap == 2)
            {
                Cv2.CvtColor(img, img, ColorConversionCodes.BGR2GRAY);
                Cv2.BitwiseNot(img, img);
                return img;
            }
            else if (ColorMap == 0)
            {
                return img;
            }
            else
            {
                return img;
            }
        }
        private Mat GridView(Mat src, double x)
        {
            Mat img = src.Clone();
            if (Grid == 1)
            {
                double mm = x * 2048 / 9 / 100;
                Cv2.Line(img, 0, img.Height / 2, img.Width, img.Height / 2, Scalar.FromRgb(68, 0, 0), 2);
                Cv2.Line(img, img.Width / 2, 0, img.Width / 2, img.Height, Scalar.FromRgb(68, 0, 0), 2);
                for (int i = 1; img.Height / 2 - mm * (i - 1) > 0; i++)
                {
                    int dist = System.Convert.ToInt32(Math.Round(mm * i));
                    Cv2.Line(img, 0, img.Height / 2 - dist, img.Width, img.Height / 2 - dist, Scalar.FromRgb(68, 0, 0), 2);
                    Cv2.Line(img, 0, img.Height / 2 + dist, img.Width, img.Height / 2 + dist, Scalar.FromRgb(68, 0, 0), 2);
                }
                for (int i = 1; img.Width / 2 - mm * (i - 1) > 0; i++)
                {
                    int dist = System.Convert.ToInt32(Math.Round(mm * i));
                    Cv2.Line(img, img.Width / 2 - dist, 0, img.Width / 2 - dist, img.Height, Scalar.FromRgb(68, 0, 0), 2);
                    Cv2.Line(img, img.Width / 2 + dist, 0, img.Width / 2 + dist, img.Height, Scalar.FromRgb(68, 0, 0), 2);
                }
            }
            return img;
        }
        public void ImageTag(string tag)
        {
            string[] tagList = tag.Split(',');

            TagX = System.Convert.ToInt32(tagList[0]);
            TagY = System.Convert.ToInt32(tagList[1]);
        }
        public int GetX()
        {
            Console.WriteLine($"GetX : {TagX}");
            return TagX;
        }
        public int GetY()
        {
            Console.WriteLine($"GetY : {TagY}");
            return TagY;
        }
        public void ImageLeft()
        {
            if (TagY!=0)
            {
                TagY--;
            }
        }
        public void ImageRight(Mat[][] selctsource)
        {
            if(TagY != selctsource[TagX].Length-1)
            {
                TagY++;
            }
        }

    }
}
