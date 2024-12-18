using LSIS.ViewModel;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LSIS
{
    /// <summary>
    /// ImageCheck.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageCheck : System.Windows.Window
    {
        private ImageProcessing img;
        private Data data;
        private MainWindow main;
        private DB db;
        private SelectPatient slt;
        private List<int> image_selection = new List<int>();
        private Equipment divice;
        public ImageCheck(MainWindow main, ImageProcessing img, Data data, DB db, SelectPatient slt, Equipment divice)
        {
            InitializeComponent();
            this.main = main;
            this.img = img;
            this.data = data;
            this.db = db;
            this.slt = slt;
            this.divice = divice;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Image_Count(ImageGrid_ICG, img.Auto_ICG_Image, 540, 220, 5, 3);
            Image_Count(ImageGrid_ICG_M, img.Manual_ICG_Image, 540, 220, 5, 3);
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border.BorderBrush == System.Windows.Media.Brushes.Yellow)
            {
                int del = 0;
                border.BorderBrush = System.Windows.Media.Brushes.Transparent;
                for (int x = 0; x < image_selection.Count(); x++)
                {
                    if (image_selection[x] == Convert.ToInt32(border.Tag))
                    {
                        del = x;
                    }
                }
                image_selection.RemoveAt(del);
            }
            else
            {
                border.BorderBrush = System.Windows.Media.Brushes.Yellow;
                image_selection.Add(Convert.ToInt32(border.Tag));
            }
            image_selection.Sort();
            image_selection.Reverse();
        }
        private void Image_Count(Grid grid, List<Mat> ImageList, int width, int height, int thickness, int gridx)
        {
            int count = 0;
            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
            image_selection.Clear();
            var columnDefinitions = grid.ColumnDefinitions;
            var RowDefinitions = grid.RowDefinitions;
            for (int x = 0; x < gridx; x++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column);
                columnDefinitions[x].Width = new GridLength(1, GridUnitType.Star);
            }
            for (int x = 0; x < (ImageList.Count + gridx - 1) / gridx; x++)
            {
                RowDefinition Row = new RowDefinition();
                grid.RowDefinitions.Add(Row);
                RowDefinitions[x].Height = new GridLength(height + 10);
            }
            for (int x = 0; x < (ImageList.Count + gridx - 1) / gridx; x++)
            {
                for (int y = 0; y < gridx; y++)
                {
                    Border border = new Border();
                    System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                    image.Source = WriteableBitmapConverter.ToWriteableBitmap(ImageList[count]);
                    border.MouseDown += new MouseButtonEventHandler(Border_MouseDown);
                    border.Tag = count;
                    border.Width = width; //540
                    border.Height = height; //288
                    border.BorderThickness = new Thickness(thickness);//5
                    border.Child = image;
                    Grid.SetRow(border, x);
                    Grid.SetColumn(border, y);
                    grid.Children.Add(border);
                    count++;
                    if (ImageList.Count == count)
                    {
                        break;
                    }
                }
            }
        }
        private void Image_View(Grid grid, int width, int height, int thickness)
        {
            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
            Border border = new Border();
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            if (grid == ImageGrid_ICG)
            {
                image.Source = WriteableBitmapConverter.ToWriteableBitmap(img.Auto_ICG_Image[image_selection[image_selection.Count() - 1]]);
            }
            else if (grid == ImageGrid_ICG_M)
            {
                image.Source = WriteableBitmapConverter.ToWriteableBitmap(img.Manual_ICG_Image[image_selection[image_selection.Count() - 1]]);
            }
            border.Width = width; //540
            border.Height = height; //288
            border.Background = System.Windows.Media.Brushes.Black;
            border.BorderThickness = new Thickness(thickness);//5
            image.Stretch = Stretch.Fill;
            border.Child = image;

            Grid.SetRow(border, 0);
            Grid.SetColumn(border, 0);
            if (grid == ImageGrid_ICG)
            {
                ImageGrid_ICG.Children.Add(border);
            }
            else if (grid == ImageGrid_ICG_M)
            {
                ImageGrid_ICG_M.Children.Add(border);
            }

        }
        public string SeriesNumber(string HID, string Position)
        {
            int seriesNumCount = 0;
            string path = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + @"\x64\Debug\Dicom";
            string strFilecount;
            while (true)
            {
                strFilecount = path + @"\" + HID + "_" + DateTime.Now.ToString("yyyyMMdd") + Position + "(#" + seriesNumCount + ")(0).dcm";
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
        private string SavePath(string Name, string HID, string Position, string seriesNumber, int x)
        {
            string strFile = @"Dicom";
            DirectoryInfo fileInfo = new DirectoryInfo(strFile);
            if (fileInfo.Exists)
            {

            }
            else
            {
                fileInfo.Create();
            }
            string path = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + @"\x64\Debug\Dicom\" + HID + "_" + DateTime.Now.ToString("yyyyMMdd") + Position + "(#" + Convert.ToInt32(seriesNumber) + ")" + "(" + x + ")" + ".dcm";
            Console.WriteLine(path);
            return path;
        }
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            List<Bitmap> Dicomimage = new List<Bitmap>();
            /*Mat image = Cv2.ImRead(@"ICG_A0.png");      //다이콤 파일 임시로 만들기
            Mat image2 = Cv2.ImRead(@"ICG_A1.png");
            Mat image3 = Cv2.ImRead(@"ICG_A2.png");
            Mat image4 = Cv2.ImRead(@"ICG_A3.png");
            OpenCvSharp.Cv2.PutText(image, $"{i}-1",
                            new OpenCvSharp.Point(image.Width / 2, image.Height / 2),
                            OpenCvSharp.HersheyFonts.Italic, 5, OpenCvSharp.Scalar.Red, 4, OpenCvSharp.LineTypes.AntiAlias);
            OpenCvSharp.Cv2.PutText(image2, $"{i}-2",
                            new OpenCvSharp.Point(image.Width / 2, image.Height / 2),
                            OpenCvSharp.HersheyFonts.Italic, 5, OpenCvSharp.Scalar.Red, 4, OpenCvSharp.LineTypes.AntiAlias);
            OpenCvSharp.Cv2.PutText(image3, $"{i}-3",
                            new OpenCvSharp.Point(image.Width / 2, image.Height / 2),
                            OpenCvSharp.HersheyFonts.Italic, 5, OpenCvSharp.Scalar.Red, 4, OpenCvSharp.LineTypes.AntiAlias);
            OpenCvSharp.Cv2.PutText(image4, $"{i}-4",
                            new OpenCvSharp.Point(image.Width / 2, image.Height / 2),
                            OpenCvSharp.HersheyFonts.Italic, 5, OpenCvSharp.Scalar.Red, 4, OpenCvSharp.LineTypes.AntiAlias);
            img.Auto_ICG_Image.Add(image);
            img.Auto_ICG_Image.Add(image2);
            img.Auto_ICG_Image.Add(image3);
            img.Auto_ICG_Image.Add(image4);*/
            for (int x = 0; x < img.Auto_ICG_Image.Count(); x++)
            {
                Dicomimage.Add(OpenCvSharp.Extensions.BitmapConverter.ToBitmap(img.Auto_ICG_Image[x]));
            }

            for (int x = 0; x < img.Manual_ICG_Image.Count(); x++)
            {
                Dicomimage.Add(OpenCvSharp.Extensions.BitmapConverter.ToBitmap(img.Manual_ICG_Image[x]));
            }
            string seriesNumber = SeriesNumber(slt.SelectHID, data.SelectPosition());
            for (int x = 0; x < img.Auto_ICG_Image.Count() + img.Manual_ICG_Image.Count(); x++)
            {
                DicomManager dm = new DicomManager(slt.SelectHID, divice.GetSerialnum());
                dm.SetPatient(slt.SelectHID, slt.SelectName, slt.SelectBirthday, slt.SelectSex, slt.SelectAge);
                // string SOPInstanceUID = "1.2.410."+ data.SelectHID.TrimStart('0')+"." + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + (x+1);
                dm.SetStudy(DateTime.Now.ToString("yyyyMMdd") + "0001", DateTime.Now.ToString("yyyyMMdd") + "0001", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"), slt.SelectInjectionTime, "PNUH", "TEST");
                dm.SetSeries(seriesNumber, data.SelectPosition(), DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"));
                dm.SetContent(seriesNumber, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"), x.ToString());
                string Shotmode = "";
                if (x < img.Auto_ICG_Image.Count())
                {
                    Shotmode = "Auto";
                }
                else
                {
                    Shotmode = "Manual";
                }
                dm.SetPrivateDataElement(data, Shotmode);
                //string path = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + @"\x64\Debug\" + DateTime.Now.ToString("MMddTHHmmss")+"("+x+")" + ".dcm";
                string path = SavePath(slt.SelectName, slt.SelectHID, data.SelectPosition(), seriesNumber, x);
                await dm.SaveImageFile(path, Dicomimage[x]);
            }

            img.Auto_ICG_Image.Clear();
            img.Manual_ICG_Image.Clear();

            this.DialogResult = true;
            System.Windows.Window.GetWindow(this).Close();
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {

            if (tabControl.SelectedIndex == 0)
            {
                if (MessageBox.Show("Auto이미지 전체가 삭제됩니다.\n삭제 하시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    img.Auto_ICG_Image.Clear();
                    Image_Count(ImageGrid_ICG, img.Auto_ICG_Image, 540, 220, 5, 3);
                    image_selection.Clear();
                }
            }
            else if (tabControl.SelectedIndex == 1)
            {
                if(image_selection.Count() == 0)
                {
                    MessageBox.Show("이미지를 선택해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (MessageBox.Show("선택된 이미지를 삭제 하시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        for (int x = 0; x < image_selection.Count(); x++)
                        {
                            img.Manual_ICG_Image.RemoveAt(image_selection[x]);
                        }
                        Image_Count(ImageGrid_ICG_M, img.Manual_ICG_Image, 540, 220, 5, 3);
                        image_selection.Clear();
                    }
                }
                
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window.GetWindow(this).Close();
        }
        private void View_Click(object sender, RoutedEventArgs e)
        {
            data.View++;
            if (data.View == 2)
            {
                data.View = 0;
            }
            if (data.View == 1)
            {
                if(image_selection.Count() == 0)
                {
                    MessageBox.Show("이미지를 선택해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    data.View = 0;
                }
                else if(image_selection.Count() >= 2)
                {
                    MessageBox.Show("이미지를 하나만 선택해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    data.View = 0;
                }
                else
                {
                    if (tabControl.SelectedIndex == 0)
                    {
                        Image_View(ImageGrid_ICG, 1600, 1000, 5);
                    }
                    else if (tabControl.SelectedIndex == 1)
                    {
                        Image_View(ImageGrid_ICG_M, 1600, 1000, 5);
                    }
                }
            }
            else
            {
                if (tabControl.SelectedIndex == 0)
                {
                    Image_Count(ImageGrid_ICG, img.Auto_ICG_Image, 540, 220, 5, 3);
                }
                else if (tabControl.SelectedIndex == 1)
                {
                    Image_Count(ImageGrid_ICG_M, img.Manual_ICG_Image, 540, 220, 5, 3);
                }
            }

            //View.Source = WriteableBitmapConverter.ToWriteableBitmap(img.Manual_ICG_Image[image_selection[image_selection.Count()-1]]);

        }
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 1)
            {
                Image_Count(ImageGrid_ICG, img.Auto_ICG_Image, 540, 220, 5, 3);
            }
            else if (tabControl.SelectedIndex == 0)
            {
                Image_Count(ImageGrid_ICG_M, img.Manual_ICG_Image, 540, 220, 5, 3);
            }
            image_selection.Clear();
        }
    }
}
