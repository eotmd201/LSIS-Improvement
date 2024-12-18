using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : Window
    {
        DB db;
        Login login;
        bool close;
        public LoginWindow(DB db,bool cancel)
        {
            InitializeComponent();
            this.db = db;
            if(cancel)
            {
                LoginBtn.Margin = new Thickness(0,0,10,0);
                CancelBtn.Visibility = Visibility.Hidden;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textbox_ID.Focus();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }
        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    Login();
                    break;
            }
        }
        private void Login()
        {
            string sql = $"SELECT CASE WHEN EXISTS(SELECT 1 FROM DB_Account WHERE ID = '{textbox_ID.Text}' AND Password = '{textbox_Password.Password}') THEN 'true' ELSE 'false' END AS succes";
            SQLiteDataReader rdr = db.Load(sql);
            bool check = false;
            while (rdr.Read())
            {
                check = (string)rdr["succes"] == "true";
            }
            if (check)
            {
                close = true;
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("ID와 Password를 확인해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(close != true)
            {
                e.Cancel = true; // 창 닫기 취소             // 여기에 원하는 로직 추가, 예를 들어, 사용자에게 메시지 표시
            }
            
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            close = true;
            GetWindow(this).Close();
        }
    }
}
