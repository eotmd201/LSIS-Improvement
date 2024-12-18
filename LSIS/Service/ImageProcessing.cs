using LSIS.ViewModel;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace LSIS
{
    public class ImageProcessing
    {
        public event Action<TimeSpan> OnTimeChanged;
        List<int> ImageSize = new List<int>(new int[] { 0, 256, 2048, 1024 });

        Mat LookupTable = new Mat(1, 256, MatType.CV_8UC3);
        Mat LookupTable2 = new Mat(1, 256, MatType.CV_8UC3);
        Mat LookupTable3 = new Mat(1, 256, MatType.CV_8UC3);

        public List<Mat> Auto_ICG_Image = new List<Mat>();
        public List<Mat> Manual_ICG_Image = new List<Mat>();
        Mat ICG_Image = new Mat();
        Mat Color_Image = new Mat();
        Mat Merge_Image = new Mat();
        Mat Video_Image = new Mat();
        int View_Num = 0;
        int Merge = 0;
        int Grid = 0;
        int ReviewGrid = 0;
        int DicomVideo = 0;
        int Video = 0;
        int Diffusion = 0;
        VideoWriter OpenCV_video;

        private DispatcherTimer timer = new DispatcherTimer();
        Stopwatch stopwatch = new Stopwatch();
        double Frame = 0;

        string file_name = "";
        string strFile = "";
        public ImageProcessing()
        {
            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(10);
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
                var Matvec3b = LookupTable2.At<Vec3b>(0, i);
                Matvec3b.Item0 = (byte)i;
                Matvec3b.Item1 = (byte)i;
                Matvec3b.Item2 = (byte)i;
                LookupTable2.Set(0, i, Matvec3b);
            }
            for (int i = 0; i < 256; i++)
            {
                var Matvec3b = LookupTable3.At<Vec3b>(0, i);
                Matvec3b.Item0 = (byte)i;
                Matvec3b.Item1 = (byte)i;
                Matvec3b.Item2 = 0;
                LookupTable3.Set(0, i, Matvec3b);
            }
        }
        public Mat Cut_Image(Mat image)
        {
            /*Mat View_Image = new Mat();
            Mat gray = new Mat();
            Mat blur = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
            Scalar scalar = Cv2.Mean(gray, null);
            int value = Convert.ToInt32(scalar[0]);+
            Cv2.GaussianBlur(gray, blur, new OpenCvSharp.Size(3, 3), 1, 0, BorderTypes.Default);
            Cv2.Canny(gray,image, value/2, 256,3, true);*/
            ICG_Image = image.SubMat(new OpenCvSharp.Rect(ImageSize[0], ImageSize[1], ImageSize[2], ImageSize[3]));
            Cv2.Resize(ICG_Image, Video_Image, new OpenCvSharp.Size(ICG_Image.Width * 0.7, ICG_Image.Height * 0.7));
            return ICG_Image.Clone();
        }
        public void Image_Size(int View_Range)
        {
            if (View_Range == 0)
            {
                ImageSize = new List<int>(new int[] { 0, 370, 2048, 796 }); //900*350
            }
            else if (View_Range == 1)
            {
                ImageSize = new List<int>(new int[] { 0, 427, 2048, 682 }); //900*300
            }
            else if (View_Range == 2)
            {
                ImageSize = new List<int>(new int[] { 512, 256, 1024, 1024 });//450 *450
            }
            else if (View_Range == 3)
            {
                ImageSize = new List<int>(new int[] { 0, 0, 2048, 1536 });//450 *450
            }
            /*if (View_Range == 0)
            {
                ImageSize = new List<int>(new int[] { 0, 0, 2048, 1536 }); //900*675
            }
            else if (View_Range == 1)
            {
                ImageSize = new List<int>(new int[] { 0, 256, 2048, 1024 }); //900*450
            }
            else if (View_Range == 2)
            {
                ImageSize = new List<int>(new int[] { 0, 313, 2048, 910 }); //900*400
            }
            else if (View_Range == 3)
            {
                ImageSize = new List<int>(new int[] { 0, 370, 2048, 796 }); //900*350
            }
            else if (View_Range == 4)
            {
                ImageSize = new List<int>(new int[] { 0, 427, 2048, 682 }); //900*300
            }
            else if (View_Range == 5)
            {
                ImageSize = new List<int>(new int[] { 512, 256, 1024, 1024 });//450 *450
            }
            else if (View_Range == 6)
            {
                ImageSize = new List<int>(new int[] { 384, 256, 1280, 1024 });
            }
            else if (View_Range == 7)
            {
                ImageSize = new List<int>(new int[] { 512, 256, 1024, 1024 });
            }*/
        }
        public void GridVal()
        {
            Grid++;
            if (Grid == 2)
            {
                Grid = 0;
            }
        }
        public int GridGet()
        {
            return Grid;
        }
        public void GridView(int Gridsize, Mat image)
        {

            if (Grid == 0)
            {

            }
            else if (Grid == 1)
            {
                if (Gridsize == 0)
                {
                    GridSize(50, image);
                }
                else if (Gridsize == 1)
                {
                    GridSize(100, image);
                }
            }
        }
        private void GridSize(double x, Mat image)
        {
            double mm = x * 2048 / 9 / 100;
            Cv2.Line(image, 0, image.Height / 2, image.Width, image.Height / 2, Scalar.FromRgb(68, 0, 0), 2);
            Cv2.Line(image, image.Width / 2, 0, image.Width / 2, image.Height, Scalar.FromRgb(68, 0, 0), 2);
            for (int i = 1; image.Height / 2 - mm * (i - 1) > 0; i++)
            {
                int dist = Convert.ToInt32(Math.Round(mm * i));
                Cv2.Line(image, 0, image.Height / 2 - dist, image.Width, image.Height / 2 - dist, Scalar.FromRgb(68, 0, 0), 2);
                Cv2.Line(image, 0, image.Height / 2 + dist, image.Width, image.Height / 2 + dist, Scalar.FromRgb(68, 0, 0), 2);
            }
            for (int i = 1; image.Width / 2 - mm * (i - 1) > 0; i++)
            {
                int dist = Convert.ToInt32(Math.Round(mm * i));
                Cv2.Line(image, image.Width / 2 - dist, 0, image.Width / 2 - dist, image.Height, Scalar.FromRgb(68, 0, 0), 2);
                Cv2.Line(image, image.Width / 2 + dist, 0, image.Width / 2 + dist, image.Height, Scalar.FromRgb(68, 0, 0), 2);
            }
        }
        public void Screenshot()
        {
            Manual_ICG_Image.Add(ICG_Image.Clone());
        }
        public void AutoScreenshot()
        {
            Auto_ICG_Image.Add(ICG_Image.Clone());

        }
        public int Video_Click(string HID, Data data, SelectPatient slt, Equipment divice)
        {
            Video++;
            if (Video == 2)
            {
                Video = 0;
            }
            if (Video == 1)
            {
                VideoStart(HID, data, slt);
            }
            else
            {
                VideoStop(data, slt, divice, false);
            }
            return Video;
        }
        public int Dicom_Click(string HID, Data data, SelectPatient slt, Equipment divice)
        {
            DicomVideo++;
            if(DicomVideo == 2)
            {
                DicomVideo = 0;
            }
            if(DicomVideo == 1)
            {
                VideoStart(HID, data, slt);
            }
            else
            {
                VideoStop(data, slt, divice, true);
            }
            return DicomVideo;
        }
        public int GetVideo()
        {
            int getval = 0;
            if (Video == 1 || DicomVideo == 1)
            {
                getval = 1;
            }
            return getval;
        }
        private void VideoStart(string HID, Data data, SelectPatient slt)
        {
            strFile = @"Video";
            DirectoryInfo fileInfo = new DirectoryInfo(strFile);
            if (fileInfo.Exists)
            {

            }
            else
            {
                fileInfo.Create();
            }
            strFile = strFile + @"\" + slt.SelectName + "(" + slt.SelectHID + ")";
            fileInfo = new DirectoryInfo(strFile);
            if (fileInfo.Exists)
            {

            }
            else
            {
                fileInfo.Create();
            }
            string Date = DateTime.Now.ToString("yyyy/MM/dd");
            string Shot_Time = DateTime.Now.ToString("HH:mm");
            strFile = strFile + @"\" + DateTime.Now.ToLongDateString();
            fileInfo = new DirectoryInfo(strFile);
            if (fileInfo.Exists)
            {

            }
            else
            {
                fileInfo.Create();
            }
            string Address = strFile;
            int count = 0;
            string strFilecount;
            string SelecPostion;
            if (data.Position_Now == 1)
            {
                SelecPostion = data.Position1;
            }
            else if (data.Position_Now == 2)
            {
                SelecPostion = data.Position2;
            }
            else
            {
                SelecPostion = "";
            }
            while (true)
            {
                strFilecount = strFile + @"\" + $"Video_{HID}_{DateTime.Now.ToString("yyyyMMdd")}_{data.SelectPosition()}_{count}" + ".avi";
                FileInfo fileInfo2 = new FileInfo(strFilecount);
                if (fileInfo2.Exists)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            //OpenCV_video = new VideoWriter(@"Video" + count + ".avi", "MJPG", 5, ICG_Image.Size());
            //file_name = HID + "_" + DateTime.Now.ToString("yyMMdd") + "(" + count + ")";
            file_name = $"Video_{HID}_{DateTime.Now.ToString("yyyyMMdd")}_{data.SelectPosition()}_{count}";
            //OpenCV_video = new VideoWriter(strFile + @"\" + file_name + ".avi", "MJPG", 5, new OpenCvSharp.Size(ICG_Image.Width, ICG_Image.Height));
            OpenCV_video = new VideoWriter(strFile + @"\" + file_name + ".avi", VideoWriter.FourCC("MJPG"), 5, new OpenCvSharp.Size(ICG_Image.Width, ICG_Image.Height));
            stopwatch.Start();
            timer.Start();
        }
        public async void VideoStop(Data data, SelectPatient slt, Equipment divice, bool dicom)
        {
            DicomManager dm = new DicomManager(slt.SelectHID, divice.GetSerialnum());
            stopwatch.Stop();
            OpenCV_video.Release();
            timer.Stop();

            string source_file = strFile + @"\" + file_name + ".avi";
            string SavePath = @"DicomVideo\" + file_name + ".dcm";
            /*string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + strFile;
            DirectoryInfo folderInfo = new DirectoryInfo(desktopPath);
            if (folderInfo.Exists)
            {

            }
            else
            {
                folderInfo.Create();
            }*/
            DirectoryInfo fileInfo = new DirectoryInfo(@"DicomVideo");
            if (fileInfo.Exists)
            {

            }
            else
            {
                fileInfo.Create();
            }

            //Dicom Video 구현 주석 처리
            if(dicom)
            {
                //string dest_file = desktopPath + @"\" + file_name + ".avi";
                string seriesNumber = SeriesNumber(slt.SelectHID, data.SelectPosition());
                dm.SetPatient(slt.SelectHID, slt.SelectName, slt.SelectBirthday, slt.SelectSex, slt.SelectAge);
                dm.SetStudy(DateTime.Now.ToString("yyyyMMdd") + "0001", DateTime.Now.ToString("yyyyMMdd") + "0001", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"), slt.SelectInjectionTime, "PNUH", "TEST");
                dm.SetSeries(seriesNumber, data.SelectPosition(), DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"));
                dm.SetContentVideo(seriesNumber, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"));
                await dm.SaveVideoFile(SavePath, source_file);
            }
            //File.Copy(source_file, dest_file, true);
            stopwatch.Reset();
            Frame = 0;
        }
        public string SeriesNumber(string HID, string Position)
        {
            int seriesNumCount = 0;
            string path = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + @"\x64\Debug\DicomVideo";
            string strFilecount;
            while (true)
            {
                strFilecount = path + @"\" + $"Video_{HID}_{DateTime.Now.ToString("yyyyMMdd")}_{Position}_{seriesNumCount}" + ".dcm";
                FileInfo fileInfo = new FileInfo(strFilecount);
                if (fileInfo.Exists)
                {
                    seriesNumCount++;
                }
                else
                {
                    break;
                }
            }
            return seriesNumCount.ToString();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = stopwatch.Elapsed;
            OnTimeChanged?.Invoke(ts);
            if (stopwatch.ElapsedMilliseconds >= 200 * Frame)
            {
                Cv2.Resize(ICG_Image, Video_Image, new OpenCvSharp.Size(ICG_Image.Width, ICG_Image.Height));
                OpenCV_video.Write(Video_Image);
                Frame++;
            }
        }
        public Mat Edge(Mat image, int i)
        {
            Mat gray = new Mat();
            Mat binary = new Mat();
            Mat canny = new Mat();

            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(gray, binary, i, 255, ThresholdTypes.Binary);
            Cv2.Canny(binary, canny, i, 255, 3, true);

            //Cv2.MorphologyEx(binary, binary, MorphTypes.Open, k);
            //Cv2.CvtColor(binary, yellow, ColorConversionCodes.GRAY2RGB);
            //Cv2.FloodFill(yellow, new OpenCvSharp.Point(binary.Width / 2, binary.Height / 2), Scalar.Red);
            //Cv2.InRange(yellow, new Scalar(0, 0, 255), new Scalar(0, 0, 255), yellow);
            return canny;
        }
        public Mat binary(Mat image, int i)
        {
            Mat gray = new Mat();
            Mat binary = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(gray, binary, i, 255, ThresholdTypes.Binary);
            //Cv2.MorphologyEx(binary, binary, MorphTypes.Open, k);
            //Cv2.CvtColor(binary, yellow, ColorConversionCodes.GRAY2RGB);
            //Cv2.FloodFill(yellow, new OpenCvSharp.Point(binary.Width / 2, binary.Height / 2), Scalar.Red);
            //Cv2.InRange(yellow, new Scalar(0, 0, 255), new Scalar(0, 0, 255), yellow);
            return binary;
        }
        public Mat LUTView(Mat src, int colormap)
        {
            Mat dst = new Mat();
            if (colormap == 1)
            {
                OpenCvSharp.Cv2.LUT(src, LookupTable2, dst);
            }
            else if (colormap == 2)
            {
                OpenCvSharp.Cv2.LUT(src, LookupTable3, dst);
            }
            else if (colormap == 0)
            {
                OpenCvSharp.Cv2.LUT(src, LookupTable, dst);
            }
            return dst;
        }
    }
}
