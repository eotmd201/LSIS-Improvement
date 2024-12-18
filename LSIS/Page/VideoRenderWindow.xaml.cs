using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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
    /// VideoRenderWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VideoRenderWindow : Window
    {
        public VideoRenderWindow(Image image, ListView listview)
        {
            InitializeComponent();
            RenderImage.Source = image.Source;
            RenderList.ItemsSource = listview.ItemsSource;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1000);
            Size renderedSize = MeasureAndArrange(DataRender);
            Console.WriteLine(renderedSize);
            System.Drawing.Bitmap bit = RenderAndSaveImage(DataRender, renderedSize);
            // SaveFileDialog 인스턴스 생성
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG 파일 (*.png)|*.png"; // 파일 형식 필터
            saveFileDialog.DefaultExt = "png"; // 기본 확장자
            saveFileDialog.AddExtension = true; // 파일 이름에 확장자 자동 추가
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                // 사용자가 선택한 파일 경로
                string filePath = saveFileDialog.FileName;

                // Bitmap 객체를 사용자가 선택한 경로에 PNG로 저장
                bit.Save(filePath, ImageFormat.Png);
            };
            System.Windows.Window.GetWindow(this).Close();
        }
        private Size MeasureAndArrange(UIElement uiElement)
        {
            // 엘리먼트를 측정하고 배치
            uiElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            uiElement.Arrange(new Rect(uiElement.DesiredSize));

            // 렌더링된 크기 반환
            return uiElement.RenderSize;
        }
        private System.Drawing.Bitmap RenderAndSaveImage(UIElement uiElement, Size renderedSize)
        {
            System.Drawing.Bitmap bitmap;
            // UIElement를 이미지로 렌더링
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)renderedSize.Width, (int)renderedSize.Height, 96, 96, PixelFormats.Default);
            rtb.Render(uiElement);

            // 투명 배경 대신에 흰색 배경을 채웁니다.
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, renderedSize.Width, renderedSize.Height));
                drawingContext.DrawImage(rtb, new Rect(renderedSize));
            }

            // 새로운 이미지를 파일로 저장
            RenderTargetBitmap finalRtb = new RenderTargetBitmap((int)renderedSize.Width, (int)renderedSize.Height, 96, 96, PixelFormats.Default);
            finalRtb.Render(drawingVisual);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(finalRtb));

            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Save(stream);
                bitmap = new System.Drawing.Bitmap(stream);
                // Now 'bitmap' is a System.Drawing.Bitmap
                // You can use 'bitmap' as needed
            }

            return bitmap;
        }
    }
}
