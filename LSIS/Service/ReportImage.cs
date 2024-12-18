using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS
{
    class ReportImage
    {
        List<Mat> img = new List<Mat>();
        List<Mat> dst = new List<Mat>();
        List<Mat> era = new List<Mat>();

        int width;
        int height;
        string fun = "";
        List<int> LineCount = new List<int>();
        Point point = new Point();
        Mat LookupTable = new Mat(1, 256, MatType.CV_8UC3);
        Mat LookupTableG = new Mat(1, 256, MatType.CV_8UC3);
        List<Eraser> EraserList = new List<Eraser>();
        struct Eraser { public int tag; public Point point; }
        public ReportImage()
        {
            for (int i = 0; i < 256; i++)
            {
                var Matvec3b = LookupTable.At<Vec3b>(0, i);
                Matvec3b.Item0 = 0;
                Matvec3b.Item1 = 0;
                Matvec3b.Item2 = (byte)i;
                LookupTable.Set(0, i, Matvec3b);
            }
            for (int i = 0; i < 256; i++)
            {
                var Matvec3b = LookupTable.At<Vec3b>(0, i);
                Matvec3b.Item0 = 0;
                Matvec3b.Item1 = (byte)i;
                Matvec3b.Item2 = 0;
                LookupTableG.Set(0, i, Matvec3b);
            }
        }
        public void Add(System.Drawing.Bitmap bitmap)
        {
            img.Add(BitmapConverter.ToMat(bitmap));
            WH();
        }
        private void WH()
        {
            width = img[0].Width;
            height = img[0].Height;
        }
        public string Function(string fun)
        {
            if (fun == this.fun)
            {
                this.fun = "";

            }
            else
            {
                this.fun = fun;
                if (fun == "Line")
                {
                    LineCount.Clear();
                }
                else if (fun == "Eraser")
                {
                    EraserList.Clear();
                }
                DstClone();
            }

            return this.fun;
        }
        public string FunctionGet()
        {
            return fun;
        }
        public void LineXlistSet(int x)
        {
            if (LineCount.Count() >= 3)
            {
                LineCount.Clear();
                LineCount.Add(x);
            }
            else
            {
                LineCount.Add(x);
            }
            DstClone();
            LineDraw();
        }
        private void LineDraw()
        {
            for (int i = 0; i < LineCount.Count(); i++)
            {
                if (i == 2)
                {
                    Cv2.Rectangle(dst[0], new OpenCvSharp.Point(LineCount[2], 0), new OpenCvSharp.Point(dst[0].Width, dst[0].Height), Scalar.Black, -1);
                    Cv2.Rectangle(dst[1], new OpenCvSharp.Point(LineCount[2], 0), new OpenCvSharp.Point(dst[1].Width, dst[1].Height), Scalar.Black, -1);
                    Cv2.Rectangle(dst[2], new OpenCvSharp.Point(LineCount[2], 0), new OpenCvSharp.Point(dst[2].Width, dst[2].Height), Scalar.Black, -1);
                    Cv2.Rectangle(dst[3], new OpenCvSharp.Point(LineCount[2], 0), new OpenCvSharp.Point(dst[3].Width, dst[3].Height), Scalar.Black, -1);
                }
                Cv2.Line(dst[0], new OpenCvSharp.Point(LineCount[i], 0), new OpenCvSharp.Point(LineCount[i], dst[0].Height), Scalar.White, 3);
                if (i != 0)
                {
                    Cv2.Line(dst[1], new OpenCvSharp.Point(LineCount[i], 0), new OpenCvSharp.Point(LineCount[i], dst[1].Height), Scalar.White, 3);
                    Cv2.Line(dst[2], new OpenCvSharp.Point(LineCount[i], 0), new OpenCvSharp.Point(LineCount[i], dst[2].Height), Scalar.White, 3);
                    Cv2.Line(dst[3], new OpenCvSharp.Point(LineCount[i], 0), new OpenCvSharp.Point(LineCount[i], dst[3].Height), Scalar.White, 3);
                }
            }
        }
        public List<Mat> ReportImageLine()
        {
            List<Mat> originalList = img;
            List<Mat> ri = originalList.Select(mat => mat.Clone()).ToList();
            for (int i = 0; i < LineCount.Count(); i++)
            {
                Cv2.Line(ri[0], new OpenCvSharp.Point(LineCount[i], 0), new OpenCvSharp.Point(LineCount[i], dst[0].Height), Scalar.White, 3);
                if (i != 0)
                {
                    Cv2.Line(ri[1], new OpenCvSharp.Point(LineCount[i], 0), new OpenCvSharp.Point(LineCount[i], dst[1].Height), Scalar.White, 3);
                    Cv2.Line(ri[2], new OpenCvSharp.Point(LineCount[i], 0), new OpenCvSharp.Point(LineCount[i], dst[2].Height), Scalar.White, 3);
                    Cv2.Line(ri[3], new OpenCvSharp.Point(LineCount[i], 0), new OpenCvSharp.Point(LineCount[i], dst[3].Height), Scalar.White, 3);
                }
            }
            return ri;
        }
        public void ExtractionPointSet(OpenCvSharp.Point point, double val)
        {
            this.point = point;
            DstClone();
            ExtractionDraw(val);
        }
        private void ExtractionDraw(double val)
        {

            if (point != new OpenCvSharp.Point(0, 0))
            {
                List<Mat> Ext = new List<Mat>();
                List<Mat> Edg = new List<Mat>();
                Ext.Add(Extraction2(dst[0].Clone(), val, point));
                Ext.Add(Extraction2(dst[1].Clone(), val, point));
                Ext.Add(Extraction2(dst[2].Clone(), val, point));
                Ext.Add(Extraction2(dst[3].Clone(), val, point));

                Edg.Add(ColorEdge(dst[0].Clone(), val));
                Edg.Add(ColorEdge(dst[1].Clone(), val));
                Edg.Add(ColorEdge(dst[2].Clone(), val));
                Edg.Add(ColorEdge(dst[3].Clone(), val));
                Cv2.BitwiseOr(Ext[0], Edg[0], dst[0]);
                Cv2.BitwiseOr(Ext[1], Edg[1], dst[1]);
                Cv2.BitwiseOr(Ext[2], Edg[2], dst[2]);
                Cv2.BitwiseOr(Ext[3], Edg[3], dst[3]);
            }
            else
            {
                dst[0] = ColorEdge(dst[0].Clone(), val);
                dst[1] = ColorEdge(dst[1].Clone(), val);
                dst[2] = ColorEdge(dst[2].Clone(), val);
                dst[3] = ColorEdge(dst[3].Clone(), val);
            }
            Cv2.Circle(dst[0], point, 5, Scalar.Blue, -1);
            Cv2.Circle(dst[1], point, 5, Scalar.Blue, -1);
            Cv2.Circle(dst[2], point, 5, Scalar.Blue, -1);
            Cv2.Circle(dst[3], point, 5, Scalar.Blue, -1);
        }
        private Mat Extraction(Mat image, double Val, OpenCvSharp.Point point)
        {
            Mat Reportimage_binary = binary(image.Clone(), Convert.ToInt32(Val));
            Cv2.CvtColor(Reportimage_binary, Reportimage_binary, ColorConversionCodes.GRAY2BGR);
            Cv2.FloodFill(Reportimage_binary, point, Scalar.Red);
            Mat image_Extraction = new Mat();
            Cv2.InRange(Reportimage_binary, Scalar.Red, Scalar.Red, image_Extraction);
            Cv2.CvtColor(image_Extraction, image_Extraction, ColorConversionCodes.GRAY2BGR);
            Cv2.BitwiseAnd(image, image_Extraction, image);
            return image;
        }
        private Mat Extraction2(Mat image, double Val, OpenCvSharp.Point point)
        {
            Mat Reportimage_binary = binary(image.Clone(), Convert.ToInt32(Val));
            Cv2.CvtColor(Reportimage_binary, Reportimage_binary, ColorConversionCodes.GRAY2BGR);
            Cv2.FloodFill(Reportimage_binary, point, Scalar.Red);

            Mat image_Extraction = new Mat();
            Cv2.InRange(Reportimage_binary, Scalar.Red, Scalar.Red, image_Extraction);
            Cv2.CvtColor(image_Extraction, image_Extraction, ColorConversionCodes.GRAY2BGR);
            image = Edge(image_Extraction.Clone(), Convert.ToInt32(Val));
            Cv2.CvtColor(image, image, ColorConversionCodes.GRAY2BGR);
            Cv2.LUT(image, LookupTableG, image);
            //image = ColorEdge(EdgeImg, Val, "Green");
            //Cv2.BitwiseAnd(image, image_Extraction, image);

            return image;
        }
        public void EraserPointSet(int tag, Point point, double val)
        {
            DstClone();
            Eraser pit = new Eraser();
            pit.tag = tag;
            pit.point = point;
            EraserList.Add(pit);
            GetEraserList(false);
            ExtractionDraw(val);
            GetEraserList(true);

        }
        public void GetEraserList(bool Visibillity)
        {
            Scalar scalar;
            if (Visibillity)
            {
                scalar = Scalar.Green;
            }
            else
            {
                scalar = Scalar.Black;
            }
            //EraserList.Sort((s1, s2) => s1.tag.CompareTo(s2.tag));
            EraserList = EraserList.OrderBy(x => x.tag).ToList();
            int count = 0;
            for (int i = 0; i < EraserList.Count(); i++)
            {
                Cv2.Circle(dst[EraserList[i].tag], EraserList[i].point, 10, scalar, -1, LineTypes.Link8);
                if (count % 2 == 1)
                {
                    if (EraserList[i].tag == EraserList[i - 1].tag)
                    {
                        Cv2.Line(dst[EraserList[i].tag], EraserList[i - 1].point, EraserList[i].point, scalar, 10);
                    }
                }
                if (i != 0 && EraserList[i].tag != EraserList[i - 1].tag)
                {
                    count = 1;
                }
                else
                {
                    count++;
                }
            }

        }
        private Mat ColorEdge(Mat image, double Val)
        {
            Mat EdgeCutImage = EdgeCut(image.Clone(), Convert.ToInt32(Val));
            Mat Reportimage_Edge = Edge(image.Clone(), Convert.ToInt32(Val));
            Cv2.CvtColor(Reportimage_Edge, Reportimage_Edge, ColorConversionCodes.GRAY2BGR);
            Cv2.LUT(Reportimage_Edge, LookupTable, Reportimage_Edge);
            Cv2.BitwiseOr(EdgeCutImage, Reportimage_Edge, image);
            return image;
        }
        private Mat EdgeCut(Mat image, int i)
        {
            Mat gray = new Mat();
            Mat binary = new Mat();
            Mat canny = new Mat();

            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(gray, binary, i, 255, ThresholdTypes.Binary);
            Cv2.CvtColor(binary, binary, ColorConversionCodes.GRAY2BGR);
            Cv2.BitwiseAnd(image, binary, image);
            return image;
        }
        public Mat Edge(Mat image, int i)
        {
            Mat gray = new Mat();
            Mat binary = new Mat();
            Mat canny = new Mat();

            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(gray, binary, i, 255, ThresholdTypes.Binary);
            Cv2.Canny(binary, canny, i, 255, 3, true);
            return canny;
        }
        private Mat binary(Mat image, int i)
        {
            Mat gray = new Mat();
            Mat binary = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(gray, binary, i, 255, ThresholdTypes.Binary);
            return binary;
        }
        private void DstClone()
        {
            dst.Clear();
            dst.Add(img[0].Clone());
            dst.Add(img[1].Clone());
            dst.Add(img[2].Clone());
            dst.Add(img[3].Clone());
        }
        public void Save(double val)
        {
            DstClone();
            if (fun == "Line")
            {
                if (LineCount.Count() == 3)
                {
                    Cv2.Rectangle(img[0], new OpenCvSharp.Point(LineCount[2], 0), new OpenCvSharp.Point(width, height), Scalar.Black, -1);
                    Cv2.Rectangle(img[1], new OpenCvSharp.Point(LineCount[2], 0), new OpenCvSharp.Point(width, height), Scalar.Black, -1);
                    Cv2.Rectangle(img[2], new OpenCvSharp.Point(LineCount[2], 0), new OpenCvSharp.Point(width, height), Scalar.Black, -1);
                    Cv2.Rectangle(img[3], new OpenCvSharp.Point(LineCount[2], 0), new OpenCvSharp.Point(width, height), Scalar.Black, -1);
                }
                else
                {
                    System.Windows.MessageBox.Show("라인 클릭 갯수 부족", "Warning");
                }
            }
            else if (fun == "Extraction")
            {
                if (point == new OpenCvSharp.Point(0, 0))
                {
                    System.Windows.MessageBox.Show("전경 이미지를 선택해주세요");
                }
                else
                {
                    img[0] = Extraction(img[0], val, point);
                    img[1] = Extraction(img[1], val, point);
                    img[2] = Extraction(img[2], val, point);
                    img[3] = Extraction(img[3], val, point);
                }
            }
            else if (fun == "Eraser")
            {
                GetEraserList(false);
                img[0] = Extraction(dst[0], val, point);
                img[1] = Extraction(dst[1], val, point);
                img[2] = Extraction(dst[2], val, point);
                img[3] = Extraction(dst[3], val, point);
            }
            fun = "";

        }
        public List<double> ROIVal(Mat image, int mode)
        {
            List<double> ROIvalue = new List<double>();
            if (LineCount.Count >= 3)
            {
                if (mode == 0)
                {
                    Mat Hand = image.Clone().SubMat(new Rect(0, 0, LineCount[0], image.Height));
                    Mat Lower = image.Clone().SubMat(new Rect(LineCount[0], 0, LineCount[1] - LineCount[0], image.Height));
                    Mat Upper = image.Clone().SubMat(new Rect(LineCount[1], 0, LineCount[2] - LineCount[1], image.Height));
                    double Handval = Cv2.Sum(Hand)[0];
                    double Lowerval = Cv2.Sum(Lower)[0];
                    double Upperval = Cv2.Sum(Upper)[0];
                    Mat HandBinary = binary(Hand, 1);
                    Mat LowerBinary = binary(Lower, 1);
                    Mat UpperBinary = binary(Upper, 1);
                    double HandCount = Cv2.Sum(HandBinary)[0] / 255;
                    double LowerCount = Cv2.Sum(LowerBinary)[0] / 255;
                    double UpperCount = Cv2.Sum(UpperBinary)[0] / 255;
                    double HandMean = Math.Round(Handval / HandCount, 2);
                    double LowerMean = Math.Round(Lowerval / LowerCount, 2);
                    double UpperMean = Math.Round(Upperval / UpperCount, 2);
                    ROIvalue.Add(HandMean);
                    ROIvalue.Add(LowerMean);
                    ROIvalue.Add(UpperMean);
                }
                else
                {
                    Mat Lower = image.Clone().SubMat(new Rect(0, 0, LineCount[1] - LineCount[0], image.Height));
                    Mat Upper = image.Clone().SubMat(new Rect(LineCount[1], 0, LineCount[2] - LineCount[1], image.Height));
                    double Lowerval = Cv2.Sum(Lower)[0];
                    double Upperval = Cv2.Sum(Upper)[0];
                    Mat LowerBinary = binary(Lower, 1);
                    Mat UpperBinary = binary(Upper, 1);
                    double LowerCount = Cv2.Sum(LowerBinary)[0] / 255;
                    double UpperCount = Cv2.Sum(UpperBinary)[0] / 255;
                    double LowerMean = Math.Round(Lowerval / LowerCount, 2);
                    double UpperMean = Math.Round(Upperval / UpperCount, 2);
                    ROIvalue.Add(LowerMean);
                    ROIvalue.Add(UpperMean);
                }

            }
            return ROIvalue;
        }
        public List<Mat> PositionText()
        {
            List<Mat> originalList = ReportImageLine();
            List<Mat> ri = originalList.Select(mat => mat.Clone()).ToList();
            string[] position = new string[] { "A", "LA", "UA", "LM", "UM", "LP", "UP", "LL", "UL"};
            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                {
                    //Cv2.Line(ri[0], new OpenCvSharp.Point(LineCount[i], 0), new OpenCvSharp.Point(LineCount[i], ri[0].Height), Scalar.White, 3);
                    Cv2.PutText(ri[0], "A", new Point((int)LineCount[0] / 2 - 100, 100), HersheyFonts.HersheyComplex, 3, Scalar.White, 5);
                    Cv2.PutText(ri[0], "LA", new Point((int)LineCount[0] + ((LineCount[1] - LineCount[0]) / 2) - 100, 100), HersheyFonts.HersheyComplex, 3, Scalar.White, 5);
                    Cv2.PutText(ri[0], "UA", new Point((int)LineCount[1] + ((LineCount[2] - LineCount[1]) / 2) - 100, 100), HersheyFonts.HersheyComplex, 3, Scalar.White, 5);
                }
                else
                {
                    Cv2.PutText(ri[i], position[2 * i + 1], new Point((int)LineCount[0] + ((LineCount[1] - LineCount[0]) / 2) - 100, 100), HersheyFonts.HersheyComplex, 3, Scalar.White, 5);
                    Cv2.PutText(ri[i], position[2 * i + 2], new Point((int)LineCount[1] + ((LineCount[2] - LineCount[1]) / 2) - 100, 100), HersheyFonts.HersheyComplex, 3, Scalar.White, 5);
                }
            }
            return ri;
        }
        public List<Mat> Getimg()
        {
            return img;
        }
        public List<Mat> Getdst()
        {
            return dst;
        }
        public int GetWidth()
        {
            return width;
        }
        public int GetHeight()
        {
            return height;
        }
        public string Getfun()
        {
            return fun;
        }
        public Point Getpoint()
        {
            return point;
        }
        public int GetLineCount()
        {
            return LineCount.Count();
        }
    }
}
