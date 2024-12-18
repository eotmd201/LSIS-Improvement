using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// ScanData.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ScanData : Window
    {
        /* ----------------------------------------------------------------------------------------------------
           * 최초작성자 : 조대승
           * 작성일자 : 2022.08.09
           * 최종변경일 : 2022.08.12
           * 목적 : 스캔 데이터 설정
           * 내용 : 스캔 데이터 설정
           * ----------------------------------------------------------------------------------------------------*/
        public MainWindow main;
        string[] CheckPosition = new string[3];
        private Data data;
        private ImageProcessing img;
        private Camera cam;
        private Serial serial_servo;
        public ScanData(MainWindow main, Data data, ImageProcessing img, Camera cam, Serial serial_servo)
        {
            InitializeComponent();
            this.data = data;
            this.img = img;
            this.cam = cam;
            this.main = main;
            this.serial_servo = serial_servo;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckPosition[0] = data.Position;
            CheckPosition[1] = data.Position1;
            CheckPosition[2] = data.Position2;
            PositionText();
            if (data.Position1 != null)
            {
                CheckBox postion = (CheckBox)this.FindName(data.Position1);
                postion.IsChecked = true;
            }
            if (data.Position2 != null)
            {
                CheckBox postion = (CheckBox)this.FindName(data.Position2);
                postion.IsChecked = true;
            }
            LoadText();
        }
        private void LoadText()
        {
            //회전각도 설정
            Auto_Angle_I.Text = 45 * (data.Auto_Rotation_Angle + 1) + "˚";
            Manual_Angle.Text = 22.5 * Math.Pow(2, data.Manual_Rotation_Angle) + "˚";

            //Extend 모드 설정
            /*if (data.Extended_Shot == 0)
            {
                Extended_Shot.Text = "Nomal Scan";
            }
            else if (data.Extended_Shot == 1)
            {
                Extended_Shot.Text = "Middle Scan";
            }
            else if (data.Extended_Shot == 2)
            {
                Extended_Shot.Text = "Full Scan";
            }*/

            // 사이즈 설정
            if (data.View_Range == 0)
            {
                View_Range.Text = "900 x 350";
            }
            else if (data.View_Range == 1)
            {
                View_Range.Text = "900 x 300";
            }
            else if (data.View_Range == 2)
            {
                View_Range.Text = "450 x 450";
            }
            else if (data.View_Range == 3)
            {
                View_Range.Text = "Full";
            }
            /*else if (data.View_Range == 3)
            {
                View_Range.Text = "700 x 300";
            }
            else if (data.View_Range == 4)
            {
                View_Range.Text = "600 x 400";
            }
            else if (data.View_Range == 5)
            {
                View_Range.Text = "600 x 300";
            }
            else if (data.View_Range == 6)
            {
                View_Range.Text = "500 x 400";
            }
            else if (data.View_Range == 7)
            {
                View_Range.Text = "400 x 400";
            }*/


            //카메라 설정
            Exposure_Time_I.Text = (data.Exposure_Time + 1) * 0.1 + "s";
            Gain_I.Text = data.Gain * 3 + "dB";
            Gamma_I.Text = (data.Gamma * 0.1 + 0.3).ToString();
        }
        private void Position_Click(object sender, RoutedEventArgs e)
        {
            CheckBox rdo = sender as CheckBox;
            if (CheckPosition[0] != rdo.Tag.ToString())
            {
                CheckPosition[0] = rdo.Tag.ToString();
                CheckPosition[1] = rdo.Name;
                CheckPosition[2] = null;
            }
            else
            {
                if (CheckPosition[1] == rdo.Name)
                {
                    CheckPosition[1] = CheckPosition[2];
                    CheckPosition[2] = null;

                }
                else if (CheckPosition[2] == rdo.Name)
                {
                    CheckPosition[2] = null;
                }
                else
                {
                    CheckPosition[2] = rdo.Name;
                }
                if (CheckPosition[1] == null && CheckPosition[2] == null)
                {
                    CheckPosition[0] = null;
                }
            }
            if (CheckPosition[0] == "1")
            {
                Check_Others.IsChecked = false;
                Check_RA.IsChecked = false;
                Check_LA.IsChecked = false;
                Check_RL.IsChecked = false;
                Check_LL.IsChecked = false;
            }
            else if (CheckPosition[0] == "2")
            {
                Check_FF.IsChecked = false;
                Check_RA.IsChecked = false;
                Check_LA.IsChecked = false;
                Check_RL.IsChecked = false;
                Check_LL.IsChecked = false;
            }
            else if (CheckPosition[0] == "3")
            {
                Check_FF.IsChecked = false;
                Check_Others.IsChecked = false;
                Check_RL.IsChecked = false;
                Check_LL.IsChecked = false;
            }
            else if (CheckPosition[0] == "4")
            {
                Check_FF.IsChecked = false;
                Check_Others.IsChecked = false;
                Check_RA.IsChecked = false;
                Check_LA.IsChecked = false;
            }
            PositionText();

        }
        private void PositionText()
        {
            if (CheckPosition[1] == null)
            {
                Postion1.Text = "N/A";
            }
            else
            {
                int underscoreIndex = CheckPosition[1].IndexOf('_');
                if (underscoreIndex != -1) // '_'가 문자열에 존재하는 경우
                {
                    Postion1.Text = CheckPosition[1].Substring(underscoreIndex + 1); // '_' 다음부터 문자열 끝까지 추출
                }

            }
            if (CheckPosition[2] == null)
            {
                Postion2.Text = "N/A";
            }
            else
            {
                int underscoreIndex = CheckPosition[2].IndexOf('_');
                if (underscoreIndex != -1) // '_'가 문자열에 존재하는 경우
                {
                    Postion2.Text = CheckPosition[2].Substring(underscoreIndex + 1); // '_' 다음부터 문자열 끝까지 추출
                }
            }
        }
        private void Auto_Angle_I_Click(object sender, MouseButtonEventArgs e)
        {
            if (Auto_Angle_I.Text == "45˚")
            {
                Auto_Angle_I.Text = "90˚";
            }
            else if (Auto_Angle_I.Text == "90˚")
            {
                Auto_Angle_I.Text = "45˚";
            }
        }
        private void Auto_Angle_V_Click(object sender, MouseButtonEventArgs e)
        {
            if (Auto_Angle_V.Text == "22.5˚")
            {
                Auto_Angle_V.Text = "45˚";
            }
            else if (Auto_Angle_V.Text == "45˚")
            {
                Auto_Angle_V.Text = "90˚";
            }
            else if (Auto_Angle_V.Text == "90˚")
            {
                Auto_Angle_V.Text = "22.5˚";
            }
        }
        private void Manual_Angle_Click(object sender, MouseButtonEventArgs e)
        {
            if (Manual_Angle.Text == "22.5˚")
            {
                Manual_Angle.Text = "45˚";
            }
            else if (Manual_Angle.Text == "45˚")
            {
                Manual_Angle.Text = "90˚";
            }
            else if (Manual_Angle.Text == "90˚")
            {
                Manual_Angle.Text = "22.5˚";
            }
        }
        private void View_Range_Click(object sender, MouseButtonEventArgs e)
        {
            if(img.GetVideo() != 1)
            {
                if (View_Range.Text == "900 x 350")
                {
                    View_Range.Text = "900 x 300";
                }
                else if (View_Range.Text == "900 x 300")
                {
                    View_Range.Text = "450 x 450";
                }
                else if (View_Range.Text == "450 x 450")
                {
                    View_Range.Text = "Full";
                }
                else if (View_Range.Text == "Full")
                {
                    View_Range.Text = "900 x 350";
                }
            }
            else
            {
                MessageBox.Show("영상 녹화를 종료해 주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            /*if (View_Range.Text == "800 x 400")
            {
                View_Range.Text = "800 x 300";
            }
            else if (View_Range.Text == "800 x 300")
            {
                View_Range.Text = "700 x 400";
            }
            else if (View_Range.Text == "700 x 400")
            {
                View_Range.Text = "700 x 300";
            }
            else if (View_Range.Text == "700 x 300")
            {
                View_Range.Text = "600 x 400";
            }
            else if (View_Range.Text == "600 x 400")
            {
                View_Range.Text = "600 x 300";
            }
            else if (View_Range.Text == "600 x 300")
            {
                View_Range.Text = "500 x 400";
            }
            else if (View_Range.Text == "500 x 400")
            {
                View_Range.Text = "400 x 400";
            }
            else if (View_Range.Text == "400 x 400")
            {
                View_Range.Text = "800 x 400";
            }*/
        }
        private void Exposure_Time_Click(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBox = sender as TextBlock;
            if (textBox.Text == "1s")
            {
                textBox.Text = "0.1s";
            }
            else
            {
                textBox.Text = Convert.ToDouble(textBox.Text.Replace("s", "")) + 0.1 + "s";
            }
        }
        private void Gain_Click(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBox = sender as TextBlock;
            if (textBox.Text == "45dB")
            {
                textBox.Text = "0dB";
            }
            else
            {
                textBox.Text = Convert.ToInt32(textBox.Text.Replace("dB", "")) + 3 + "dB";
            }

        }
        private void Gamma_Click(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBox = sender as TextBlock;
            if (textBox.Text == "1")
            {
                textBox.Text = "0.3";
            }
            else
            {
                textBox.Text = (Convert.ToDouble(textBox.Text) + 0.1).ToString();
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            data.Position_Now = 1;
            data.Position = CheckPosition[0];
            data.Position1 = CheckPosition[1];
            data.Position2 = CheckPosition[2];

            if (CheckPosition[1] != null)
            {
                if (serial_servo.IsOpen())
                {
                    main.AutoScan.IsEnabled = true;
                }
                main.ManualScan.IsEnabled = true;
                main.Position1.Background = Brushes.Yellow;
                main.Position2.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x7F, 0x7F, 0x7F));
                main.Position1.Foreground = Brushes.Red;
                main.Position2.Foreground = Brushes.White;
                main.Position1.IsEnabled = true;
            }
            else
            {
                main.Position1.Background = Brushes.Yellow;
                main.Position1.Foreground = Brushes.Red;
                main.AutoScan.IsEnabled = false;
                main.ManualScan.IsEnabled = false;
                main.Position1.IsEnabled = false;
            }
            if (CheckPosition[2] != null)
            {
                main.Position2.IsEnabled = true;
            }
            else
            {
                main.Position2.IsEnabled = false;
            }
            //회전각도 설정
            data.Auto_Rotation_Angle = Convert.ToInt32(Auto_Angle_I.Text.Replace("˚", "")) / 45 - 1;
            data.Manual_Rotation_Angle = (int)Math.Log(Convert.ToDouble(Manual_Angle.Text.Replace("˚", "")) / 22.5, 2);

            //Extend 모드 설정
            /*if (Extended_Shot.Text == "Nomal Scan")
            {
                data.Extended_Shot = 0;
            }
            else if (Extended_Shot.Text == "Middle Scan")
            {
                data.Extended_Shot = 1;
            }
            else if (Extended_Shot.Text == "Full Scan")
            {
                data.Extended_Shot = 2;
            }*/

            // 사이즈 설정
            if (View_Range.Text == "900 x 350")
            {
                data.View_Range = 0;
            }
            else if (View_Range.Text == "900 x 300")
            {
                data.View_Range = 1;
            }
            else if (View_Range.Text == "450 x 450")
            {
                data.View_Range = 2;
            }
            else if (View_Range.Text == "Full")
            {
                data.View_Range = 3;
            }
            /*else if (View_Range.Text == "700 x 300")
            {
                data.View_Range = 3;
            }
            else if (View_Range.Text == "600 x 400")
            {
                data.View_Range = 4;
            }
            else if (View_Range.Text == "600 x 300")
            {
                data.View_Range = 5;
            }
            else if (View_Range.Text == "500 x 400")
            {
                data.View_Range = 6;
            }
            else if (View_Range.Text == "400 x 400")
            {
                data.View_Range = 7;
            }*/



            //카메라 설정
            data.Exposure_Time = (int)(Convert.ToDouble(Exposure_Time_I.Text.Replace("s", "")) * 10 - 1);
            data.Gain = Convert.ToInt32(Gain_I.Text.Replace("dB", "")) / 3;
            data.Gamma = (int)Math.Round((Convert.ToDouble(Gamma_I.Text) - 0.3) * 10);

            //img.Image_Size(data.View_Range);
            cam.Setting(data.Exposure_Time, data.Gain, data.Gamma);
            if(data.Position1 !=null)
            {
                if(data.Position1 != "Check_FF" && data.Position1 != "Check_Others")
                {
                    data.ScanDataCheck = true;
                }
                else
                {
                    data.ScanDataCheck = false;
                }
                int underscoreIndex = data.Position1.IndexOf('_');
                if (underscoreIndex != -1) // '_'가 문자열에 존재하는 경우
                {
                    main.Position1.Content = data.Position1.Substring(underscoreIndex + 1); // '_' 다음부터 문자열 끝까지 추출
                }
            }
            else
            {
                data.ScanDataCheck = false;
                main.Position1.Content = data.Position1;
            }
            if (data.Position2 != null)
            {
                int underscoreIndex2 = data.Position2.IndexOf('_');
                if (underscoreIndex2 != -1) // '_'가 문자열에 존재하는 경우
                {
                    main.Position2.Content = data.Position2.Substring(underscoreIndex2 + 1); // '_' 다음부터 문자열 끝까지 추출
                }
            }
            else
            {
                main.Position2.Content = data.Position2;
            }
            

            
            main.AutoScan.IsEnabled = main.AutoscanEnabled();
            this.DialogResult = true;
            Window.GetWindow(this).Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
