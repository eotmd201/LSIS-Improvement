using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using LiveCharts.Wpf;
using System.Windows.Media.Imaging;

namespace LSIS
{
    class ReportDocument
    {
        MainWindow main;
        public FlowDocument CreateFlowDocument(MainWindow main, List<string> patientinformation, List<Image> imagesouce, List<string> PattenValue_A, List<string> PattenValue_P, List<string> PattenValue_M, List<string> PattenValue_L, List<Image> positionimagesouce, List<string> subdatatable, List<string> datatable, List<int> chatval, List<List<string>> comment, string summery)
        {
            this.main = main;
            FlowDocument doc = new FlowDocument(); // FlowDocument 만들기

            // 페이지 크기를 A4 가로 모드로 설정 (픽셀 단위)

            doc.PageWidth = 1122.52; // 페이지 너비 (A4 가로 모드, 픽셀 단위)
            doc.PageHeight = 793.70; // 페이지 높이 (A4 세로 모드, 픽셀 단위)

            Section sec = Page1(patientinformation, imagesouce, PattenValue_A, PattenValue_P, PattenValue_M, PattenValue_L); // 섹션 만들기
            doc.Blocks.Add(sec);
            sec.BreakPageBefore = true;

            Section sec2 = Page2(positionimagesouce, subdatatable, datatable, chatval, patientinformation[2]);
            doc.Blocks.Add(sec2);
            sec2.BreakPageBefore = true;

            Section sec3 = Page3(imagesouce, comment, summery);
            doc.Blocks.Add(sec3);
            sec3.BreakPageBefore = true;
            doc.ColumnWidth = 1064;

            return doc;
        }

        private Section Page1(List<string> patientinformation, List<Image> imagesouce, List<string> PattenValue_A, List<string> PattenValue_P, List<string> PattenValue_M, List<string> PattenValue_L)
        {
            Section sec = new Section();
            Paragraph p1 = new Paragraph(); // 첫 번째 단락 만들기       
            Bold bld = new Bold(); // 굵게
            bld.Inlines.Add(new Run("환자정보"));
            p1.Inlines.Add(bld); // 단락에 굵게, 기울임꼴, 밑줄 추가  
            sec.Blocks.Add(p1);

            Paragraph p2 = new Paragraph();  // 환자 정보 테이블
            p2.Inlines.Add(Patient_Information_Tabel(patientinformation));
            sec.Blocks.Add(p2);

            Paragraph p3 = new Paragraph();
            bld = new Bold();
            bld.Inlines.Add(new Run("Key Image"));
            p3.Inlines.Add(bld);
            sec.Blocks.Add(p3);

            Paragraph p4 = new Paragraph();
            p4.Inlines.Add(Key_Image_Table(imagesouce, PattenValue_A, PattenValue_P, PattenValue_M, PattenValue_L));
            sec.Blocks.Add(p4);
            return sec;
        }
        private Section Page2(List<Image> imagesouce, List<string> subdatatable, List<string> datatable, List<int> chatval, string date)
        {
            Section sec = new Section();
            Grid grid = new Grid();
            //grid.Width = 1064;
            //grid.Height = 776;

            grid.Width = 1064;
            grid.Height = 718;

            var columnDefinitions = grid.ColumnDefinitions;
            var RowDefinitions = grid.RowDefinitions;
            for (int x = 0; x < 2; x++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column);
                columnDefinitions[x].Width = new GridLength(1, GridUnitType.Star);
            }
            List<int> length = new List<int>() { 40, 1, 40, 40, 310 };
            List<GridUnitType> gridunittype = new List<GridUnitType>() { GridUnitType.Pixel, GridUnitType.Star, GridUnitType.Pixel, GridUnitType.Pixel, GridUnitType.Pixel };
            for (int x = 0; x < 5; x++)
            {
                RowDefinition Row = new RowDefinition();
                grid.RowDefinitions.Add(Row);
                RowDefinitions[x].Height = new GridLength(length[x], gridunittype[x]);
            }
            //grid.ShowGridLines = true;

            TextBlock p1 = new TextBlock();
            Bold bld = new Bold();
            bld.Inlines.Add(new Run("Key Image"));
            p1.Inlines.Add(bld);
            Grid.SetColumn(p1, 0);
            Grid.SetRow(p1, 0);
            grid.Children.Add(p1);

            Grid keyImage = KeyImage(imagesouce);
            Grid.SetColumn(keyImage, 0);
            Grid.SetRow(keyImage, 1);
            Grid.SetColumnSpan(keyImage, 2);
            grid.Children.Add(keyImage);

            TextBlock p2 = new TextBlock();
            bld = new Bold();
            bld.Inlines.Add(new Run("Data Table"));
            p2.Inlines.Add(bld);
            Grid.SetColumn(p2, 0);
            Grid.SetRow(p2, 2);

            grid.Children.Add(p2);

            TextBlock p3 = new TextBlock();
            bld = new Bold();
            bld.Inlines.Add(new Run("Dermal Backflow Pattern Chart"));
            p3.Inlines.Add(bld);
            Grid.SetColumn(p3, 1);
            Grid.SetRow(p3, 2);
            grid.Children.Add(p3);

            Grid subtable = SubDataTable(subdatatable);
            Grid.SetColumn(subtable, 0);
            Grid.SetRow(subtable, 3);
            grid.Children.Add(subtable);

            Grid maintable = DataTable(datatable);
            Grid.SetColumn(maintable, 0);
            Grid.SetRow(maintable, 4);
            grid.Children.Add(maintable);

            CartesianChart datachart = new CartesianChart();
            datachart.AxisX.Clear();
            datachart.AxisY.Clear();
            Axis AxisX = new Axis();
            LiveCharts.Wpf.Separator sep = new LiveCharts.Wpf.Separator();
            sep.IsEnabled = true;
            sep.Step = 1;
            AxisX.Separator = sep;
            AxisX.Labels = new string[] { "A", "LA", "UA", "LP", "UP", "LM", "UM", "LL", "UL" };
            datachart.AxisX.Add(AxisX);
            Axis AxisY = new Axis();
            LiveCharts.Wpf.Separator sep2 = new LiveCharts.Wpf.Separator();
            sep2.IsEnabled = true;
            sep2.Step = 1;
            AxisY.Separator = sep2;
            AxisY.Labels = new string[] { "", "Linear", "Splash", "Stardust", "Diffuse", "Non-Flow" };
            AxisY.MaxValue = 5;
            datachart.AxisY.Add(AxisY);
            datachart.LegendLocation = LiveCharts.LegendLocation.Bottom;
            /*datachart.Series.Clear();
             List<int> recent = chatval;
             for (int idx = 0; idx < recent.Count; idx++)
             {
                 recent[idx] = recent[idx] + 1;
             }
             datachart.Series.Add(new LiveCharts.Wpf.ColumnSeries
             {
                 //바빠서 임시 조취 반드시 수정필요
                 Title = date,
                 MaxColumnWidth = 10,
                 Values = new LiveCharts.ChartValues<double> {recent[0],recent[1],
                     recent[2],
                     recent[3],
                     recent[4],
                     recent[5],
                     recent[6],
                     recent[7],
                     recent[8],}
             });*/
            main.ChartView(datachart);
            datachart.DisableAnimations = true;
            Grid.SetColumn(datachart, 1);
            Grid.SetRow(datachart, 3);
            Grid.SetRowSpan(datachart, 2);

            grid.Children.Add(datachart);


            /*ChartToBitmapImage bti = new ChartToBitmapImage();
            Image datachart = new Image();
            datachart.Source = bti.ControlToImage(chart, chart.ActualWidth, chart.ActualHeight);
            datachart.Stretch = Stretch.Fill;
            datachart.Margin = new Thickness(5);
            Grid.SetColumn(datachart, 1);
            Grid.SetRow(datachart, 3);
            Grid.SetRowSpan(datachart, 2);
            grid.Children.Add(datachart);*/

            Paragraph p = new Paragraph();
            p.Inlines.Add(grid);
            sec.Blocks.Add(p);

            return sec;
        }
        private Section Page3(List<Image> imagesouce, List<List<string>> comment, string summery)
        {
            Section sec = new Section();
            Grid grid = new Grid();
            grid.Width = 1064;
            grid.Height = 718;
            var columnDefinitions = grid.ColumnDefinitions;
            var RowDefinitions = grid.RowDefinitions;
            for (int x = 0; x < 2; x++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column);
                columnDefinitions[x].Width = new GridLength(1, GridUnitType.Star);
            }
            List<int> length = new List<int>() { 40, 1, 40, 200 };
            List<GridUnitType> gridunittype = new List<GridUnitType>() { GridUnitType.Pixel, GridUnitType.Star, GridUnitType.Pixel, GridUnitType.Pixel };
            for (int x = 0; x < 4; x++)
            {
                RowDefinition Row = new RowDefinition();
                grid.RowDefinitions.Add(Row);
                RowDefinitions[x].Height = new GridLength(length[x], gridunittype[x]);
            }
            //grid.ShowGridLines = true;

            TextBlock p1 = new TextBlock();
            Bold bld = new Bold();
            bld.Inlines.Add(new Run("Comment"));
            p1.Inlines.Add(bld);
            Grid.SetColumn(p1, 0);
            Grid.SetRow(p1, 0);
            grid.Children.Add(p1);

            Grid cmt = Comment(imagesouce, comment);
            Grid.SetColumn(cmt, 0);
            Grid.SetRow(cmt, 1);
            Grid.SetColumnSpan(cmt, 2);
            grid.Children.Add(cmt);


            TextBlock p2 = new TextBlock();
            bld = new Bold();
            bld.Inlines.Add(new Run("Summary"));
            p2.Inlines.Add(bld);
            Grid.SetColumn(p2, 0);
            Grid.SetRow(p2, 2);
            grid.Children.Add(p2);

            Grid smr = Summary(summery);
            Grid.SetColumn(smr, 0);
            Grid.SetRow(smr, 3);
            Grid.SetColumnSpan(smr, 2);
            grid.Children.Add(smr);

            Paragraph p = new Paragraph();
            p.Inlines.Add(grid);
            sec.Blocks.Add(p);
            return sec;
        }
        //page1
        private Grid Patient_Information_Tabel(List<string> patientinformation)
        {
            Grid grid = new Grid();
            grid.Width = 1064;
            grid.Height = 60;
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            var columnDefinitions = grid.ColumnDefinitions;
            var RowDefinitions = grid.RowDefinitions;
            for (int x = 0; x < 8; x++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column);
                if (x % 2 == 0)
                {
                    columnDefinitions[x].Width = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    columnDefinitions[x].Width = new GridLength(2, GridUnitType.Star);
                }
            }
            for (int x = 0; x < 2; x++)
            {
                RowDefinition Row = new RowDefinition();
                grid.RowDefinitions.Add(Row);
                RowDefinitions[x].Height = new GridLength(1, GridUnitType.Star);
            }
            List<string> patient = new List<string>();
            patient.Add("이름");
            patient.Add(patientinformation[0]);
            patient.Add("생년월일");
            patient.Add(patientinformation[1]);
            patient.Add("촬영날짜");
            patient.Add(patientinformation[2]);
            patient.Add("의료진");
            patient.Add(patientinformation[3]);
            patient.Add("등록번호");
            patient.Add(patientinformation[4]);
            patient.Add("성별");
            patient.Add(patientinformation[5]);
            patient.Add("환부");
            patient.Add(patientinformation[6]);
            patient.Add("진단명");
            patient.Add(patientinformation[7]);

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Border border = new Border();
                    Label label = new Label();
                    label.FontFamily = new FontFamily("Century Gothic");
                    label.Content = patient[x * 8 + y];
                    label.FontSize = 14;
                    label.Foreground = Brushes.Black;
                    label.VerticalContentAlignment = VerticalAlignment.Center;
                    label.HorizontalContentAlignment = HorizontalAlignment.Center;
                    border.BorderThickness = new Thickness(1);
                    border.BorderBrush = Brushes.Black;
                    border.Child = label;

                    Grid.SetRow(border, x);
                    Grid.SetColumn(border, y);
                    grid.Children.Add(border);
                }
            }
            return grid;
        }
        private Grid Key_Image_Table(List<Image> imagesouce, List<string> PattenValue_A, List<string> PattenValue_P, List<string> PattenValue_M, List<string> PattenValue_L)
        {
            Grid grid = new Grid();
            grid.Width = 1064;
            grid.Height = 550;
            var columnDefinitions = grid.ColumnDefinitions;
            var RowDefinitions = grid.RowDefinitions;
            for (int x = 0; x < 4; x++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column);
                if (x % 2 == 0)
                {
                    columnDefinitions[x].Width = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    columnDefinitions[x].Width = new GridLength(19, GridUnitType.Star);
                }

            }
            for (int x = 0; x < 4; x++)
            {
                RowDefinition Row = new RowDefinition();
                grid.RowDefinitions.Add(Row);
                if (x % 2 == 0)
                {
                    RowDefinitions[x].Height = new GridLength(3, GridUnitType.Star);
                }
                else
                {
                    RowDefinitions[x].Height = new GridLength(1, GridUnitType.Star);
                }

            }
            List<string> posision = new List<string>() { "A", "P", "M", "L" };
            int count = 0;
            for (int x = 0; x < 4; x += 2)
            {
                for (int y = 0; y < 4; y += 2)
                {
                    Label label = new Label();
                    label.Content = posision[count];
                    label.FontSize = 14;
                    label.FontWeight = FontWeights.UltraBold;
                    label.Foreground = Brushes.Black;
                    label.VerticalContentAlignment = VerticalAlignment.Top;
                    label.HorizontalContentAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(label, x);
                    Grid.SetColumn(label, y);
                    Grid.SetRowSpan(label, 2);
                    grid.Children.Add(label);
                    count++;
                }
            }

            count = 0;
            for (int x = 0; x < 4; x += 2)
            {
                for (int y = 1; y < 5; y += 2)
                {
                    Image imageview = new Image();
                    imageview.Source = imagesouce[count].Source;
                    Grid.SetRow(imageview, x);
                    Grid.SetColumn(imageview, y);
                    grid.Children.Add(imageview);
                    count++;
                }
            }

            Grid A = Key_PattenValue_Table(PattenValue_A);
            Grid.SetRow(A, 1);
            Grid.SetColumn(A, 1);
            grid.Children.Add(A);

            Grid P = Key_PattenValue_Table(PattenValue_P);
            Grid.SetRow(P, 3);
            Grid.SetColumn(P, 1);
            grid.Children.Add(P);

            Grid M = Key_PattenValue_Table(PattenValue_M);
            Grid.SetRow(M, 1);
            Grid.SetColumn(M, 3);
            grid.Children.Add(M);

            Grid L = Key_PattenValue_Table(PattenValue_L);
            Grid.SetRow(L, 3);
            Grid.SetColumn(L, 3);
            grid.Children.Add(L);
            return grid;
        }
        private Grid Key_PattenValue_Table(List<string> PattenValue)
        {
            Grid grid = new Grid();
            var columnDefinitions = grid.ColumnDefinitions;
            var RowDefinitions = grid.RowDefinitions;
            if (PattenValue.Count() == 4 || PattenValue.Count() == 6)
            {
                for (int x = 0; x < PattenValue.Count(); x++)
                {
                    ColumnDefinition column = new ColumnDefinition();
                    grid.ColumnDefinitions.Add(column);
                    if (x % 2 == 0)
                    {
                        columnDefinitions[x].Width = new GridLength(1, GridUnitType.Star);
                    }
                    else
                    {
                        columnDefinitions[x].Width = new GridLength(2, GridUnitType.Star);
                    }

                }
                for (int x = 0; x < 2; x++)
                {
                    RowDefinition Row = new RowDefinition();
                    grid.RowDefinitions.Add(Row);
                    RowDefinitions[x].Height = new GridLength(1, GridUnitType.Star);
                }
                List<string> position = new List<string>() { };
                if (PattenValue.Count() == 6)
                {
                    position = new List<string>() { "Hand", "Lower", "Upper" };
                }
                else if (PattenValue.Count() == 4)
                {
                    position = new List<string>() { "Lower", "Upper" };
                }
                int count = 0;
                for (int x = 0; x < PattenValue.Count(); x += 2)
                {
                    Border border = new Border();
                    Label text = new Label();
                    text.FontFamily = new FontFamily("Century Gothic");
                    text.FontWeight = FontWeights.UltraBold;
                    text.Content = position[count];
                    text.Foreground = Brushes.Black;
                    text.FontSize = 14;
                    text.HorizontalContentAlignment = HorizontalAlignment.Center;
                    text.VerticalContentAlignment = VerticalAlignment.Center;
                    Grid.SetRow(border, 0);
                    Grid.SetColumn(border, x);
                    Grid.SetRowSpan(border, 2);
                    border.BorderThickness = new Thickness(1);
                    border.BorderBrush = Brushes.Black;
                    border.Child = text;
                    grid.Children.Add(border);
                    count++;
                }

                for (int x = 0; x < 2; x++)
                {
                    for (int y = 1; y < PattenValue.Count(); y += 2)
                    {
                        Border border = new Border();
                        Label text = new Label();
                        text.FontFamily = new FontFamily("Century Gothic");
                        text.Foreground = Brushes.Black;
                        text.FontWeight = FontWeights.UltraBold;
                        text.Content = PattenValue[x * PattenValue.Count() / 2 + y / 2];
                        text.FontSize = 14;
                        text.HorizontalContentAlignment = HorizontalAlignment.Center;
                        text.VerticalContentAlignment = VerticalAlignment.Center;
                        Grid.SetRow(border, x);
                        Grid.SetColumn(border, y);
                        border.BorderThickness = new Thickness(1);
                        border.BorderBrush = Brushes.Black;
                        border.Child = text;
                        grid.Children.Add(border);
                    }
                }
            }
            else
            {
                MessageBox.Show("Key_PattenValue_Table 데이터 오류", "Warning");
            }
            return grid;
        }

        //page2
        private Grid KeyImage(List<Image> imagesouce)
        {
            Grid grid = new Grid();
            var columnDefinitions = grid.ColumnDefinitions;
            var RowDefinitions = grid.RowDefinitions;
            for (int x = 0; x < 2; x++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column);
                columnDefinitions[x].Width = new GridLength(1, GridUnitType.Star);
            }
            for (int x = 0; x < 2; x++)
            {
                RowDefinition Row = new RowDefinition();
                grid.RowDefinitions.Add(Row);
                RowDefinitions[x].Height = new GridLength(1, GridUnitType.Star);
            }
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    Image imageview = new Image();
                    imageview.Stretch = Stretch.Fill;
                    imageview.Margin = new Thickness(5);
                    imageview.Source = imagesouce[x * 2 + y].Source;
                    Grid.SetRow(imageview, x);
                    Grid.SetColumn(imageview, y);
                    grid.Children.Add(imageview);
                }
            }
            return grid;
        }
        private Grid SubDataTable(List<string> subdatatable)
        {
            Grid grid = new Grid();
            grid.Margin = new Thickness(5);
            var columnDefinitions = grid.ColumnDefinitions;
            for (int x = 0; x < 6; x++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column);
                if (x % 2 == 0)
                {
                    columnDefinitions[x].Width = new GridLength(3, GridUnitType.Star);
                }
                else
                {
                    columnDefinitions[x].Width = new GridLength(2, GridUnitType.Star);
                }
            }

            List<string> subdata = new List<string>() { "시작시간", subdatatable[0], "데이터시간", subdatatable[1], "소요시간", subdatatable[2] };
            for (int x = 0; x < 6; x++)
            {
                Border border = new Border();
                Label label = new Label();
                label.FontFamily = new FontFamily("Century Gothic");
                label.Content = subdata[x];
                label.FontSize = 14;
                label.FontWeight = FontWeights.UltraBold;
                label.Foreground = Brushes.Black;
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = Brushes.Black;
                border.Child = label;

                Grid.SetColumn(border, x);
                Grid.SetRow(border, 0);
                grid.Children.Add(border);
            }
            return grid;
        }
        private Grid DataTable(List<string> datatable)
        {
            Grid grid = new Grid();
            grid.Margin = new Thickness(5);
            var columnDefinitions = grid.ColumnDefinitions;
            var RowDefinitions = grid.RowDefinitions;
            for (int x = 0; x < 4; x++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column);
                columnDefinitions[x].Width = new GridLength(1, GridUnitType.Star);
            }
            for (int x = 0; x < 10; x++)
            {
                RowDefinition Row = new RowDefinition();
                grid.RowDefinitions.Add(Row);
                RowDefinitions[x].Height = new GridLength(1, GridUnitType.Star);
            }
            List<string> data = new List<string>() {"Area","Pattern","Velocity","ROI",
                "A", datatable[0], datatable[1], datatable[2],
                "LA", datatable[3], datatable[4], datatable[5],
                "UA", datatable[6], datatable[7], datatable[8],
                "LP", datatable[9], datatable[10], datatable[11],
                "UP", datatable[12], datatable[13], datatable[14],
                "LM", datatable[15], datatable[16], datatable[17],
                "UM", datatable[18], datatable[19], datatable[20],
                "LL", datatable[21], datatable[22], datatable[23],
                "UL", datatable[24], datatable[25], datatable[26] };

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    Border border = new Border();
                    Label label = new Label();
                    label.FontFamily = new FontFamily("Century Gothic");
                    label.Content = data[x * 4 + y];
                    label.FontSize = 14;
                    label.FontWeight = FontWeights.UltraBold;
                    label.Foreground = Brushes.Black;
                    label.VerticalContentAlignment = VerticalAlignment.Center;
                    label.HorizontalContentAlignment = HorizontalAlignment.Center;
                    border.BorderThickness = new Thickness(1);
                    border.BorderBrush = Brushes.Black;
                    border.Child = label;

                    Grid.SetRow(border, x);
                    Grid.SetColumn(border, y);
                    grid.Children.Add(border);
                }
            }
            return grid;
        }
        //page3
        private Grid Comment(List<Image> imagesouce, List<List<string>> comment)
        {
            Grid grid = new Grid();
            var columnDefinitions = grid.ColumnDefinitions;
            var RowDefinitions = grid.RowDefinitions;
            for (int x = 0; x < 2; x++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column);
                columnDefinitions[x].Width = new GridLength(1, GridUnitType.Star);
            }
            for (int x = 0; x < 4; x++)
            {
                RowDefinition Row = new RowDefinition();
                grid.RowDefinitions.Add(Row);
                if (x % 2 == 0)
                {
                    RowDefinitions[x].Height = new GridLength(144, GridUnitType.Pixel);
                }
                else
                {
                    RowDefinitions[x].Height = new GridLength(1, GridUnitType.Star);
                }

            }
            for (int y = 0; y < 4; y++)
            {
                if (y % 2 == 0)
                {
                    for (int x = 0; x < 2; x++)
                    {
                        Image imageview = new Image();
                        imageview.Stretch = Stretch.Fill;
                        imageview.Margin = new Thickness(5);
                        imageview.Source = imagesouce[y + x].Source;
                        Grid.SetColumn(imageview, x);
                        Grid.SetRow(imageview, y);
                        grid.Children.Add(imageview);
                    }
                }
                else
                {
                    for (int x = 0; x < 2; x++)
                    {
                        Grid cmt = CommentText(comment[x + y - 1]);
                        cmt.Margin = new Thickness(5);
                        Grid.SetColumn(cmt, x);
                        Grid.SetRow(cmt, y);
                        grid.Children.Add(cmt);
                    }
                }
            }
            //grid.ShowGridLines = true;
            return grid;
        }
        private Grid CommentText(List<string> comment)
        {
            Grid grid = new Grid();
            var columnDefinitions = grid.ColumnDefinitions;
            for (int x = 0; x < 2; x++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column);
                if (x % 2 == 0)
                {
                    columnDefinitions[x].Width = new GridLength(120, GridUnitType.Pixel);
                }
                else
                {
                    columnDefinitions[x].Width = new GridLength(1, GridUnitType.Star);
                }
                Border border = new Border();
                TextBlock label = new TextBlock();
                label.Text = comment[x];
                label.FontSize = 14;
                label.FontWeight = FontWeights.UltraBold;
                label.TextWrapping = TextWrapping.Wrap;
                label.Foreground = Brushes.Black;
                label.Background = Brushes.White;
                if (x % 2 == 0)
                {
                    label.VerticalAlignment = VerticalAlignment.Center;
                    label.HorizontalAlignment = HorizontalAlignment.Center;
                }
                else
                {
                    label.VerticalAlignment = VerticalAlignment.Top;
                    label.HorizontalAlignment = HorizontalAlignment.Left;
                }
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = Brushes.Black;
                border.Child = label;
                Grid.SetColumn(border, x);
                grid.Children.Add(border);
            }
            return grid;
        }
        private Grid Summary(string summery)
        {
            Grid grid = new Grid();
            grid.Margin = new Thickness(5);
            Border border = new Border();
            TextBlock label = new TextBlock();
            label.Text = summery;
            label.FontSize = 14;
            label.Foreground = Brushes.Black;
            label.TextWrapping = TextWrapping.Wrap;
            label.VerticalAlignment = VerticalAlignment.Top;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Black;
            border.Child = label;
            grid.Children.Add(border);
            return grid;
        }
    }
}
