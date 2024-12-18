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
    /// AccountWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AccountWindow : Window
    {
        private DB db = new DB();
        private string btnName;
        int index;
        string find = "";
        public AccountWindow(DB db, string btn)
        {
            InitializeComponent();
            this.db = db;
            this.btnName = btn;
        }
        public AccountWindow(DB db, string btn, int index, string find)
        {
            InitializeComponent();
            this.db = db;
            this.btnName = btn;
            this.index = index;
            this.find = find;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (btnName == "Add")
            {
                Button_Add.Content = btnName;
            }
            else
            {
                string sql;
                if (find == "")
                {
                    sql = "SELECT * FROM DB_Account LIMIT 1 OFFSET " + (index + 1) + "";
                }
                else
                {
                    sql = "SELECT * FROM DB_Account Where ID='" + find + "' LIMIT 1 OFFSET " + index + "";
                }
                SQLiteDataReader rdr = db.Load(sql);
                if (rdr.Read())
                {
                    ID_TextBox.Text = rdr["ID"].ToString();
                }
                rdr.Close();
                /*List<string> Patient = db.Account_Change_Load(main.List_Account.Items.Count, main.List_Account.SelectedIndex);
                textbox_ID.Text = Patient[0];
                textbox_Password.Text = Patient[1];*/
                Button_Add.Content = btnName;
            }
            ID_TextBox.Focus();
            ID_TextBox.Select(ID_TextBox.Text.Length, 0);
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddBtn();
        }
        private void Account_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    AddBtn();
                    break;
                case Key.Space:
                    e.Handled = true;
                    break;
            }
        }
        private void AddBtn()
        {

            if(ID_TextBox.Text==""|| Password_TextBox.Password.ToString()=="")
            {
                MessageBox.Show("ID와 Password를 확인해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (Password_TextBox.Password.Length < 8)
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
                bool Check = false;
                for (int i = 0; i < Account.Count; i += 2)
                {
                    if (btnName == "Add")
                    {
                        if (Account[i] == ID_TextBox.Text)
                        {
                            Check = true;
                        }
                    }
                    else
                    {
                        if (Account[i] == ID_TextBox.Text && Account[2 * index + 2] != ID_TextBox.Text)
                        {
                            Check = true;
                        }
                    }
                }
                if (!Check)
                {
                    if (btnName == "Add")
                    {
                        Add();
                        this.DialogResult = true;
                        Window.GetWindow(this).Close();
                    }
                    else
                    {
                        Update();
                        this.DialogResult = true;
                        Window.GetWindow(this).Close();
                    }

                }
                else
                {
                    MessageBox.Show("중복된 ID 입니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            
        }
        private void Add()
        {
            string sql = "insert into  DB_Account  values ('" + ID_TextBox.Text + "','" + Password_TextBox.Password.ToString() + "')";
            db.Save(sql);
        }
        private void Update()
        {
            string sql;
            if (find == "")
            {
                sql = "update DB_Account set ID='" + ID_TextBox.Text + "', Password ='" + Password_TextBox.Password.ToString() + "' where (ID,Password) = (SELECT ID, Password FROM DB_Account LIMIT 1 OFFSET " + (index + 1) + ")";
            }
            else
            {
                sql = "update DB_Account set ID='" + ID_TextBox.Text + "', Password ='" + Password_TextBox.Password.ToString() + "' where (ID,Password) = (SELECT ID, Password FROM DB_Account where ID = '" + find + "' LIMIT 1 OFFSET " + index + ")";
            }
            Console.WriteLine(sql);
            db.Save(sql);
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
