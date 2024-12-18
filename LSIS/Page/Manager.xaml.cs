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
    /// Manager.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Manager : Window
    {
        private DB db;
        public Manager(DB db)
        {
            InitializeComponent();
            this.db = db;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ID.Focus();
        }
        private void ManagerID_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    Save();
                    e.Handled = true;
                    break;
                case Key.Space:
                    e.Handled = true;
                    break;
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }
        private void Save()
        {
            if (ID.Text == "" || Password.Password == "")
            {
                MessageBox.Show("ID와 Password를 확인해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if(Password.Password.Length < 8)
            {
                MessageBox.Show("8자리 이상의 비밀번호를 설정해주세요. ", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                string sql = "SELECT * FROM DB_Account";
                SQLiteDataReader rdr = db.Load(sql);
                List<string> Account = new List<string>();
                while (rdr.Read())
                {
                    Account.Add(rdr["ID"].ToString());
                    Account.Add(rdr["Password"].ToString());
                }
                rdr.Close();
                bool Check = false;
                for (int i = 0; i < Account.Count; i += 2)
                {
                    if (Account[i] == ID.Text && Account[i + 1] == Password.Password)
                    {
                        Check = true;
                    }
                }
                if (Check != true)
                {
                    sql = "update DB_Account set ID='" + ID.Text + "', Password ='" + Password.Password + "' where ID='" + Account[0] + "' AND Password='" + Account[1] + "'";
                    db.Save(sql);
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("중복된 ID 입니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            GetWindow(this).Close();
        }
    }
}
