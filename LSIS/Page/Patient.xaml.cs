using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
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
    /// Patient.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Patient : Window
    {
        private DB db;
        private Dicom dm;
        private SelectPatient slt;
        private string btnName;
        int index;
        string find = "";
        public Patient(DB db, string btn)
        {
            InitializeComponent();
            this.db = db;
            btnName = btn;
        }
        public Patient(DB db, Dicom dm, SelectPatient slt, string btn, int index, string find)
        {
            InitializeComponent();
            this.db = db;
            this.dm = dm;
            this.slt = slt;
            btnName = btn;
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
                    sql = "SELECT * FROM DB_Patient ORDER BY CreationDate DESC LIMIT 1 OFFSET " + index + "";
                }
                else
                {
                    sql = "SELECT * FROM DB_Patient Where Name = '" + find + "'OR HID ='" + find + "' ORDER BY CreationDate DESC LIMIT 1 OFFSET " + index + "";
                }
                Console.WriteLine(sql);
                SQLiteDataReader rdr = db.Load(sql);
                if (rdr.Read())
                {
                    textbox_Name.Text = rdr["Name"].ToString();
                    textbox_HID.Text = rdr["HID"].ToString();
                    textbox_Birthday.Text = rdr["Birthday"].ToString().Substring(0,8);
                    string Sex = rdr["Sex"].ToString();
                    if (Sex == "M")
                    {
                        combobox_Sex.SelectedIndex = 0;
                    }
                    else if(Sex == "F")
                    {
                        combobox_Sex.SelectedIndex = 1;
                    }
                    textbox_Comment.Text = rdr["Comment"].ToString();
                }
                rdr.Close();
                /*List<string> Patient = db.Account_Change_Load(main.List_Account.Items.Count, main.List_Account.SelectedIndex);
                textbox_ID.Text = Patient[0];
                textbox_Password.Text = Patient[1];*/
                Button_Add.Content = btnName;
            }
            textbox_Name.Focus();
            textbox_Name.Select(textbox_Name.Text.Length, 0);
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddBtn();
        }
        private void Patient_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    AddBtn();
                    break;
            }
        }
        private void AddBtn()
        {
            string sql = "SELECT * FROM DB_Patient ORDER BY CreationDate DESC";
            SQLiteDataReader rdr = db.Load(sql);
            List<string> Account = new List<string>();
            while (rdr.Read())
            {
                Account.Add(rdr["HID"].ToString());
            }
            bool Check = false;
            for (int i = 0; i < Account.Count; i ++)
            {
                /*if (Account[i] == textbox_HID.Text && Account[index] != textbox_HID.Text)
                {
                    Console.WriteLine("확인");
                    Check = true;
                }*/
                if(btnName == "Add")
                {
                    if (Account[i] == textbox_HID.Text)
                    {
                        Check = true;
                    }
                }
                else
                {
                    if (Account[i] == textbox_HID.Text && Account[index] != textbox_HID.Text)
                    {
                        Check = true;
                    }
                }
            }
            if (!Check)
            {
                if (textbox_Name.Text == "")
                {
                    MessageBox.Show("이름을 입력해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (textbox_HID.Text == "")
                {
                    MessageBox.Show("환자번호를 입력해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (textbox_Birthday.Text.Length != 8 || !DateTime.TryParseExact(textbox_Birthday.Text, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _))
                {
                    MessageBox.Show("생년월일 8자리를 확인해주세요 예) 19920101", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if(combobox_Sex.SelectedIndex == -1)
                {
                    MessageBox.Show("성별을 입력해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if(DateTime.ParseExact(textbox_Birthday.Text, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) > DateTime.Now)
                {
                    MessageBox.Show("생년월일 8자리를 확인해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (btnName == "Add")
                    {
                        Add();
                        this.DialogResult = true;
                        Window.GetWindow(this).Close();
                    }
                    else
                    {
                        LoginWindow login_Window = new LoginWindow(db,false);
                        login_Window.ShowDialog();
                        if (login_Window.DialogResult.HasValue && login_Window.DialogResult.Value)
                        {
                            Update();
                            this.DialogResult = true;
                            Window.GetWindow(this).Close();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("중복된 환자입니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void Add()
        {
            string AgeVal = Age(textbox_Birthday.Text);
            string SexVal = Sex(combobox_Sex.SelectedIndex);
            string sql = "insert into  DB_Patient  values ('" + textbox_Name.Text + "','" + textbox_HID.Text + "','" + textbox_Birthday.Text+ "','" + AgeVal + "','" + SexVal + "','" + textbox_Comment.Text + "','" + DateTime.Now.ToString("yyyyMMddHHmmss") + "')";
            db.Save(sql);
        }
        private void Update()
        {
            string AgeVal = Age(textbox_Birthday.Text);
            string SexVal = Sex(combobox_Sex.SelectedIndex);
            dm.Update(slt.SelectHID, textbox_HID.Text, textbox_Name.Text, textbox_Birthday.Text, SexVal, AgeVal);
            string sql;
            if (find == "")
            {
                sql = "update DB_Patient set Name='" + textbox_Name.Text + "', HID ='" + textbox_HID.Text + "', Birthday ='" + textbox_Birthday.Text + "', Age ='" + AgeVal + "', Sex ='" + SexVal + "', Comment ='" + textbox_Comment.Text + "' where (Name, HID, Birthday, Age, Sex, Comment) = (SELECT Name, HID, Birthday, Age, Sex, Comment FROM DB_Patient ORDER BY CreationDate DESC LIMIT 1 OFFSET " + index + ")";
                Console.WriteLine(sql);
            }
            else
            {
                sql = "update DB_Patient set Name='" + textbox_Name.Text + "', HID ='" + textbox_HID.Text + "' where (Name,HID) = (SELECT Name, HID FROM DB_Patient Where Name = '" + find + "'OR HID ='" + find + "'ORDER BY CreationDate DESC LIMIT 1 OFFSET " + index + ")";
                Console.WriteLine($"SQL : {sql}");
            }
            db.Save(sql);

        }
        private string Age(string Birthday)
        {

            int year= Convert.ToInt32(Birthday.Substring(0,4));
            int month = Convert.ToInt32(Birthday.Substring(4, 2));
            int day = Convert.ToInt32(Birthday.Substring(6, 2));
            DateTime dtNow = DateTime.Now;
            int now_year = dtNow.Year;
            int now_month = dtNow.Month;
            int now_day = dtNow.Day;
            int Age;
            if (month < now_month)
            {
                Age = now_year - year;
            }
            else if (month == now_month)
            {
                if (day <= now_day)
                {
                    Age = now_year - year;
                }
                else
                {
                    Age = now_year - year - 1;
                }
            }
            else
            {
                Age = now_year - year - 1;
            }
            return Age.ToString();
        }
        private string Sex(int SexNum)
        {
            string sex;
            if (SexNum == 0)
            {
                sex = "M";
            }
            else
            {
                sex = "F";
            }
            return sex;
        }
        private void textbox_Name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Z가-힣]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void textbox_HID_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void textbox_HID_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.ImeProcessed)
            {
                e.Handled = true;
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
