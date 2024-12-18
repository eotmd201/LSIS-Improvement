using System;
using System.Collections.Generic;
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
    /// InjectionWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InjectionWindow : Window
    {
        public InjectionWindow()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            this.Tag = $"{hourTextBox.Text}{minuteTextBox.Text}00";
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void IncreaseHour_Click(object sender, RoutedEventArgs e)
        {
            int hour = int.Parse(hourTextBox.Text);
            hour = (hour + 1) % 24;
            hourTextBox.Text = hour.ToString("D2");
        }

        private void DecreaseHour_Click(object sender, RoutedEventArgs e)
        {
            int hour = int.Parse(hourTextBox.Text);
            hour = (hour - 1 + 24) % 24;
            hourTextBox.Text = hour.ToString("D2");
        }

        private void IncreaseMinute_Click(object sender, RoutedEventArgs e)
        {
            int minute = int.Parse(minuteTextBox.Text);
            minute = (minute + 1) % 60;
            minuteTextBox.Text = minute.ToString("D2");
        }

        private void DecreaseMinute_Click(object sender, RoutedEventArgs e)
        {
            int minute = int.Parse(minuteTextBox.Text);
            minute = (minute - 1 + 60) % 60;
            minuteTextBox.Text = minute.ToString("D2");
        }
    }
}
