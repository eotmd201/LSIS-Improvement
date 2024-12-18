using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace LSIS
{
    /// <summary>
    /// RenderingViewWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RenderingViewWindow : Window
    {
        FlowDocument doc;
        List<System.Drawing.Bitmap> bitlist = new List<System.Drawing.Bitmap>();
        public event EventHandler<List<System.Drawing.Bitmap>> ChildWindowClosed;
        public RenderingViewWindow(FlowDocument doc)
        {
            InitializeComponent();
            this.doc = doc;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Width = 1122.52;
            Height = 793.70;
        }
        private async void Viewer_Loaded(object sender, RoutedEventArgs e)
        {
            viewer.Document = doc;
            // UI 엘리먼트의 렌더링이 완료될 때까지 대기 후 이미지 저장
            int count = await GetPageCountAsync();
            bitlist.Clear();
            for (int i = 0; i < count; i++)
            {
                int currentPage = i + 1;
                viewer.GoToPage(currentPage);
                // 페이지 전환을 기다립니다.
                viewer.UpdateLayout();
                await Task.Delay(1000);
                // UIElement를 이미지로 렌더링
                Size renderedSize = MeasureAndArrange(viewer);
                System.Drawing.Bitmap bit = RenderAndSaveImage(viewer, renderedSize);
                bitlist.Add(bit);
                Console.WriteLine($"Page {currentPage} saved successfully!");
            }
            Console.WriteLine("Image saved successfully!");
            this.DialogResult = true;
            System.Windows.Window.GetWindow(this).Close();
        }
        private Task<int> GetPageCountAsync()
        {
            var tcs = new TaskCompletionSource<int>();

            Dispatcher.InvokeAsync(() =>
            {
                tcs.SetResult(viewer.PageCount);
            }, DispatcherPriority.ApplicationIdle);

            return tcs.Task;
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
            /*// UIElement를 이미지로 렌더링
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

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(finalRtb));
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);// UIElement를 이미지로 렌더링
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

            }*/
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            ChildWindowClosed?.Invoke(this, bitlist);
        }
    }
}
