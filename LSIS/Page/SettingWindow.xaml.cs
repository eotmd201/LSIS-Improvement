using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
    /// SettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingWindow : Window
    {
        public MainWindow main;
        private Data data;
        private DB db;
        public SettingWindow(Data data, DB db)
        {
            InitializeComponent();
            this.data = data;
            this.db = db;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT * FROM DB_Setting";
            SQLiteDataReader rdr = db.Load(sql);
            while (rdr.Read())
            {
                Grid.SelectedIndex = Convert.ToInt32(rdr["Grid"].ToString());
                View_Range.SelectedIndex = Convert.ToInt32(rdr["View_Range"].ToString());
                Auto_Rotation_Angle.SelectedIndex = Convert.ToInt32(rdr["Auto_Rotation_Angle"].ToString());
                Manual_Rotation_Angle.SelectedIndex = Convert.ToInt32(rdr["Manual_Rotation_Angle"].ToString());
                Exposure_Time.SelectedIndex = Convert.ToInt32(rdr["Exposure_Time"].ToString());
                Gain.SelectedIndex = Convert.ToInt32(rdr["Gain"].ToString());
                Gamma.SelectedIndex = Convert.ToInt32(rdr["Gamma"].ToString());
                Filter.SelectedIndex = Convert.ToInt32(rdr["Filter"].ToString());
            }

        }
        private void Initialization_Click(object sender, RoutedEventArgs e)
        {
            //Circumference_Interval.SelectedIndex = 0;
            Grid.SelectedIndex = 1;
            View_Range.SelectedIndex = 0;
            //Extended_Shot.SelectedIndex = 0;
            Auto_Rotation_Angle.SelectedIndex = 1;
            Manual_Rotation_Angle.SelectedIndex = 1;
            Exposure_Time.SelectedIndex = 0;
            Gain.SelectedIndex = 9;
            Gamma.SelectedIndex = 7;
            Filter.SelectedIndex = 0;
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //data.Circumference_Interval = Circumference_Interval.SelectedIndex;
            data.Grid = Grid.SelectedIndex;
            data.View_Range = View_Range.SelectedIndex;
            //data.Extended_Shot = Extended_Shot.SelectedIndex;
            data.Auto_Rotation_Angle = Auto_Rotation_Angle.SelectedIndex;
            data.Manual_Rotation_Angle = Manual_Rotation_Angle.SelectedIndex;
            data.Exposure_Time = Exposure_Time.SelectedIndex;
            data.Gain = Gain.SelectedIndex;
            data.Gamma = Gamma.SelectedIndex;
            data.Filter = Filter.SelectedIndex;
            string sql =
                "Update DB_Setting Set Grid=" + Grid.SelectedIndex +
                ",View_Range=" + View_Range.SelectedIndex +
                ",Auto_Rotation_Angle=" + Auto_Rotation_Angle.SelectedIndex +
                ",Manual_Rotation_Angle=" + Manual_Rotation_Angle.SelectedIndex +
                ",Exposure_Time=" + Exposure_Time.SelectedIndex +
                ",Gain=" + Gain.SelectedIndex +
                ",Gamma=" + Gamma.SelectedIndex +
                ",Filter=" + Filter.SelectedIndex + "";
            db.Save(sql);
            this.DialogResult = true;
            Window.GetWindow(this).Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
