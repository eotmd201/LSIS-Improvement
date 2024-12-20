using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.IO.Buffer;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;
using FFmpeg.AutoGen;
using LiveCharts.Wpf;
using Microsoft.Win32;
using OpenCvSharp.Extensions;
using SpinnakerNET;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using LSIS.ViewModel;
using System.ComponentModel;
using LSIS.Service;

namespace LSIS
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private MainWindow main;
        Log logdata = new Log("log.txt");
        DB db = new DB();
        Dicom dm = new Dicom();
        USBSystem usb = new USBSystem();
        SelectPatient slt = new SelectPatient();
        private Serial serial_PCB = new Serial();
        private Serial serial_servo = new Serial();
        //private ImageProcessing img = new ImageProcessing();
        //private ImageProcessing img2 = new ImageProcessing();

        private ImageProcessingReview irv;
        private ImageProcessingReview irv2;
        private ImageProcessingReview irv3;
        private ImageProcessingReview irv4;
        private ReportDocument rd;
        private ImageProcessing imgSelect = new ImageProcessing();
        ReportSelect rptslt = new ReportSelect();
        ChartList chartlist;
        private Servo servo = new Servo();
        private Camera cam = new Camera();
        private Data data = new Data();
        private VideoPlayer vp = new VideoPlayer();
        private Sensor ssv = new Sensor();
        private Equipment divice;
        private SoftwareValidityService validityService;
        private Login login = new Login();
        private DispatcherTimer timer = new DispatcherTimer();
        private DispatcherTimer timer2 = new DispatcherTimer();
        private CapacityService cap = new CapacityService();
        private TimeLock timelock;
        private List<string> deviceNums;
        Label DB;
        DoubleAnimation animation;
        Stopwatch sw = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
            chartlist = new ChartList(this);
            validityService = new SoftwareValidityService(db);
            divice = new Equipment();
            timer.Tick += AutoTimer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer2.Tick += TimerTickHandler2;
            timer2.Interval = TimeSpan.FromMilliseconds(200);
            main = this;
            _viewModel = new MainViewModel(login, db, divice, validityService);
            DataContext = _viewModel;
            servo.OnTimeChanged += ServoMove_OnTimeChanged;

            _viewModel.ShowMessageEvent += ShowMessage;
            _viewModel.OpenManagerWindowEvent += OpenManagerWindow;
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void OpenManagerWindow()
        {
            var managerWindow = new Manager(db);
            managerWindow.ShowDialog();

        }
        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
            base.OnClosed(e);
        }
    }

    /// <summary>
    /// 로그인
    /// </summary>
    public partial class MainWindow : Window
    {

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
            /*string mode = log.LoginMode(ID_TextBox.Text, Password_TextBox.Password.ToString(), db);
            if (mode == "Master")
            {
                MoveToTab("마스터 모드");
                //tabControl.SelectedIndex = 10;
                MasterLoad();
            }
            else if (!divice.SerialCheck())
            {
                MessageBox.Show("Serial번호를 등록 후 사용하세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if(mode == "First")
            {
                Manager manager_Window = new Manager(db);
                manager_Window.ShowDialog();
                if (manager_Window.DialogResult.HasValue && manager_Window.DialogResult.Value)
                {
                    AccountLoad();
                    button_Setting.IsEnabled = true;
                    string sql = "update DB_Master set Validity_Period ='" + DateTime.Now.AddYears(1) + "'";
                    db.Save(sql);
                }
            }
            else if(mode == "Manager")
            {
                AccountLoad();
                //button_Setting.IsEnabled = true;
            }
            else if(mode == "User")
            {
                button_Patient.IsEnabled = true;
                button_Setting.IsEnabled = true;
                PatientLoad();
            }
            else
            {
                MessageBox.Show("ID와 Password를 확인해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            logdata.WriteLog($"로그인 모드 : {mode} // ID : {ID_TextBox.Text} ");
            
            if (log.GetLoginCheck()&& divice.SerialCheck())
            {
                timelock = new TimeLock(db,main);
                EventSubscriber _evertsubscriber = new EventSubscriber(this, timelock);
                timelock.Start();
            }*/
        }
    }

    /// <summary>
    /// 계정관리
    /// </summary>
    public partial class MainWindow : Window
    {
        string AccountFind = "";
        private void AccountLoad()
        {
            MoveToTab("계정 관리");
            //tabControl.SelectedIndex = 1;
        }
        private void AccountRefresh()
        {
            AccountFind = "";
            string sql = "SELECT * FROM DB_Account LIMIT (Select COUNT(*)-1 cnt FROM DB_Account) OFFSET 1";
            List_Account.ItemsSource = db.Load(sql);
            logdata.WriteLog("계정 검색 리셋");
        }
        private void AccountFindRefresh()
        {
            if (Account_Search.Text != "")
            {
                AccountFind = Account_Search.Text;
                string sql = "SELECT * FROM DB_Account where ID='" + Account_Search.Text + "'";
                List_Account.ItemsSource = db.Load(sql);
                logdata.WriteLog($"계정 검색 : {Account_Search.Text}");
            }
        }
        private void Account_Add_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            AccountWindow account_Window = new AccountWindow(db, btn.Content.ToString());
            account_Window.ShowDialog();
            if (account_Window.DialogResult.HasValue && account_Window.DialogResult.Value)
            {
                MessageBox.Show("사용자 계정 생성을 완료하였습니다.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                logdata.WriteLog("계정 추가");
                AccountRefresh();
            }
            /*Button btn = sender as Button;
            AccountWindow account_Window = new AccountWindow(db, btn.Content.ToString());
            account_Window.ShowDialog();
            if (account_Window.DialogResult.HasValue && account_Window.DialogResult.Value)
            {

            }
            var viewModel = DataContext as MainViewModel;
            var newAccount = new Account { ID = "New Name", Password = 30 };
            viewModel.AddAccount(newAccount);*/
        }
        private void Account_Change_Click(object sender, RoutedEventArgs e)
        {
            if (List_Account.SelectedItems.Count == 0)
            {
                MessageBox.Show("사용자 계정을 선택해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (List_Account.SelectedItems.Count >= 2)
            {
                MessageBox.Show("사용자 계정을 하나만 선택해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                int lastClickedIndex = 0;
                var selectedItems = List_Account.SelectedItems;
                if (selectedItems.Count > 0)
                {
                    var lastSelectedItem = selectedItems[selectedItems.Count - 1];
                    lastClickedIndex = List_Account.Items.IndexOf(lastSelectedItem);
                }
                Button btn = sender as Button;
                AccountWindow account_Window = new AccountWindow(db, btn.Content.ToString(), lastClickedIndex, AccountFind);
                account_Window.ShowDialog();
                if (account_Window.DialogResult.HasValue && account_Window.DialogResult.Value)
                {
                    logdata.WriteLog("계정 수정");
                    MessageBox.Show("사용자 계정 수정을 완료하였습니다.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    AccountRefresh();
                }
            }
        }
        private void Account_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (List_Account.SelectedItems.Count == 0)
            {
                MessageBox.Show("사용자 계정을 선택해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (List_Account.SelectedItems.Count >= 2)
            {
                MessageBox.Show("사용자 계정을 하나만 선택해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (MessageBox.Show("사용자 계정을 삭제 하시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    int lastClickedIndex = 0;
                    var selectedItems = List_Account.SelectedItems;
                    if (selectedItems.Count > 0)
                    {
                        var lastSelectedItem = selectedItems[selectedItems.Count - 1];
                        lastClickedIndex = List_Account.Items.IndexOf(lastSelectedItem);
                    }
                    string sql;
                    if (AccountFind == "")
                    {
                        sql = "Delete from DB_Account where (ID,Password) = (SELECT ID, Password FROM DB_Account LIMIT 1 OFFSET " + (lastClickedIndex + 1) + ")";
                    }
                    else
                    {
                        sql = "Delete from DB_Account where (ID,Password) = (SELECT ID, Password FROM DB_Account where ID = '" + AccountFind + "' LIMIT 1 OFFSET " + lastClickedIndex + ")";
                    }
                    db.Save(sql);
                    logdata.WriteLog("사용자 계정 삭제");
                    MessageBox.Show("사용자 계정 삭제를 완료하였습니다.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    AccountRefresh();
                    List_Account.Items.Refresh();
                }
            }

            /*var viewModel = DataContext as MainViewModel;
            if (viewModel.SelectedAccount != null)
            {
                viewModel?.DeleteSelectedAccount();
            }
            else
            {
                MessageBox.Show("삭제할 계정를 선택해주세요.", "Warning");
            }*/
        }
        private void Manager_Change(object sender, RoutedEventArgs e)
        {
            Manager manager_Window = new Manager(db);
            manager_Window.ShowDialog();
            if(manager_Window.DialogResult.HasValue && manager_Window.DialogResult.Value)
            {
                logdata.WriteLog("관리자 계정 수정");
                MessageBox.Show("관리자 계정 수정을 완료하였습니다.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void Account_Find_Click(object sender, RoutedEventArgs e)
        {
            AccountFindRefresh();
        }
        private void Account_Search_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    AccountFindRefresh();
                    break;
            }
        }
        private void Account_Reset_Click(object sender, RoutedEventArgs e)
        {
            AccountFind = "";
            Account_Search.Text = "";
            AccountRefresh();
        }
        private void PACSSet_Click(object sender, RoutedEventArgs e)
        {
            PACSSetWindow pacsset_Window = new PACSSetWindow(db);
            pacsset_Window.ShowDialog();
            if (pacsset_Window.DialogResult.HasValue && pacsset_Window.DialogResult.Value)
            {
                MessageBox.Show("PACS설정 완료.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    /// <summary>
    /// 환자관리
    /// </summary>
    public partial class MainWindow : Window
    {
        string PatientFind = "";
        private void PatientLoad()
        {
            logdata.WriteLog("환자관리창 로드");
            MoveToTab("환자 관리");
            //tabControl.SelectedIndex = 2;
        }
        private void Patient_Click(object sender, RoutedEventArgs e)
        {
            if (imgSelect.GetVideo() == 1)
            {
                MessageBox.Show("녹화를 종료하고 진행해 주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (imgSelect.Auto_ICG_Image.Count() != 0 || imgSelect.Manual_ICG_Image.Count() != 0)
                {
                    if (MessageBox.Show("저장되지 않은 이미지가 존재합니다.\n삭제하고 진행하시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        imgSelect.Auto_ICG_Image.Clear();
                        imgSelect.Manual_ICG_Image.Clear();
                        PatientLoad();
                    }
                }
                else
                {
                    PatientLoad();
                }
            }
            
        }

        private void PatientRefresh()
        {
            PatientFind = "";
            Patient_Search.Text = "";
            textblock_MainName.Text = "";
            textblock_MainHID.Text = "";
            string sql = "SELECT * FROM DB_Patient ORDER BY CreationDate DESC";
            List_Patient.ItemsSource = db.Load(sql);
            List_Patient.Items.Refresh();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Patient patient_Window = new Patient(db, btn.Content.ToString());
            patient_Window.ShowDialog();
            if (patient_Window.DialogResult.HasValue && patient_Window.DialogResult.Value)
            {
                MessageBox.Show("환자 데이터 생성을 완료하였습니다.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                logdata.WriteLog("환자 추가");
                PatientRefresh();
            }
        }
        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (List_Patient.SelectedItems.Count == 0)
            {
                MessageBox.Show("환자를 선택해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (List_Patient.SelectedItems.Count >= 2)
            {
                MessageBox.Show("환자 데이터를 하나만 선택해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                int lastClickedIndex = 0;
                var selectedItems = List_Patient.SelectedItems;
                if (selectedItems.Count > 0)
                {
                    var lastSelectedItem = selectedItems[selectedItems.Count - 1];
                    lastClickedIndex = List_Patient.Items.IndexOf(lastSelectedItem);
                }
                Button btn = sender as Button;
                Patient patient_Window = new Patient(db, dm, slt, btn.Content.ToString(), lastClickedIndex, PatientFind);
                patient_Window.ShowDialog();
                if (patient_Window.DialogResult.HasValue && patient_Window.DialogResult.Value)
                {
                    logdata.WriteLog("환자 변경");
                    MessageBox.Show("환자 데이터 수정을 완료하였습니다.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    PatientRefresh();
                }
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (List_Patient.SelectedItems.Count == 0)
            {
                MessageBox.Show("환자를 선택해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (List_Patient.SelectedItems.Count >= 2)
            {
                MessageBox.Show("환자 데이터를 하나만 선택해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (MessageBox.Show("환자 데이터를 삭제 하시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    LoginWindow login_Window = new LoginWindow(db, false);
                    login_Window.ShowDialog();
                    if (login_Window.DialogResult.HasValue && login_Window.DialogResult.Value)
                    {
                        string sql;
                        sql = "Delete from DB_Patient where HID = '" + slt.SelectHID + "'";
                        db.Save(sql);
                        textblock_MainName.Text = "";
                        textblock_MainHID.Text = "";
                        dm.Delete(slt.SelectHID);
                        dm.Deletevideo(slt.SelectHID);
                        dm.DeleteReport(slt.SelectHID);
                        // 폴더 경로 지정
                        string folderPath = @"video\" + $"{slt.SelectName}({slt.SelectHID})";

                        // 해당 폴더가 존재하는지 확인
                        if (Directory.Exists(folderPath))
                        {
                            // 폴더 삭제
                            Directory.Delete(folderPath, true); // 'true'는 폴더 내의 모든 내용을 포함하여 삭제합니다.
                        }
                        logdata.WriteLog("환자 삭제");
                        MessageBox.Show("환자 데이터 삭제를 완료하였습니다.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        PatientRefresh();
                        List_Patient.Items.Refresh();
                    }
                }
            }
        }
        private void Find_Click(object sender, RoutedEventArgs e)
        {
            PatientFindRefresh();
        }
        private void Search_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    PatientFindRefresh();
                    break;
            }
        }
        private void PatientFindRefresh()
        {
            if (Patient_Search.Text != "")
            {
                logdata.WriteLog($"환자 검색 : {Patient_Search.Text}");
                PatientFind = Patient_Search.Text;
                string sql = "SELECT * FROM DB_Patient where HID ='" + Patient_Search.Text + "' OR Name = '" + Patient_Search.Text + "' ORDER BY CreationDate DESC";
                List_Patient.ItemsSource = db.Load(sql);
                textblock_MainName.Text = "";
                textblock_MainHID.Text = "";

            }
        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("환자 리셋");
            PatientRefresh();
            PatientFind = "";
            Patient_Search.Text = "";
            textblock_MainName.Text = "";
            textblock_MainHID.Text = "";
        }
        private void List_Patient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (List_Patient.SelectedItems.Count == 1)
            {
                if (deviceNums.Count() != 0)
                {
                    button_Scan.IsEnabled = true;
                }
                button_Review.IsEnabled = true;
                string sql;
                if (PatientFind == "")
                {
                    sql = "SELECT * FROM DB_Patient ORDER BY CreationDate DESC LIMIT 1 OFFSET " + List_Patient.SelectedIndex + "";

                }
                else
                {
                    sql = "SELECT * FROM DB_Patient where Name='" + PatientFind + "' or HID='" + PatientFind + "' ORDER BY CreationDate DESC LIMIT 1 OFFSET " + List_Patient.SelectedIndex + "";
                }
                SQLiteDataReader rdr = db.Load(sql);
                if (rdr.Read())
                {
                    slt.SelectName = rdr["Name"].ToString();
                    slt.SelectHID = rdr["HID"].ToString();
                    slt.SelectBirthday = rdr["Birthday"].ToString();
                    slt.SelectAge = rdr["Age"].ToString();
                    slt.SelectSex = rdr["Sex"].ToString();
                }
                rdr.Close();
                /*ImageGrid.Children.Clear();
                ImageGrid.ColumnDefinitions.Clear();
                ImageGrid.RowDefinitions.Clear();*/
                logdata.WriteLog($"환자 선택 : {slt.SelectName}({slt.SelectHID})");
                textblock_MainName.Text = slt.SelectName + " " + slt.SelectSex + "/" + slt.SelectAge;
                textblock_MainHID.Text = slt.SelectHID;
                PatientSelectInit();
            }
            else
            {
                button_Scan.IsEnabled = false;
                button_Review.IsEnabled = false;
            }
        }
        private void PatientSelectInit()
        {
            Position1.Content = "";
            Position2.Content = "";
            Position1.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x7F, 0x7F, 0x7F));
            Position2.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x7F, 0x7F, 0x7F));
            Position1.IsEnabled = false;
            Position2.IsEnabled = false;
            AutoScan.IsEnabled = false;
            ManualScan.IsEnabled = false;
            data.ScanDataCheck = false;
            data.Position = null;
            data.Position1 = null;
            data.Position2 = null;
            string sql = "SELECT * FROM DB_Setting";
            SQLiteDataReader rdr = db.Load(sql);
            while (rdr.Read())
            {
                //setting.Add(rdr["Circumference_Interval"].ToString());
                data.Grid = Convert.ToInt32(rdr["Grid"].ToString());
                data.View_Range = Convert.ToInt32(rdr["View_Range"].ToString());
                data.Auto_Rotation_Angle = Convert.ToInt32(rdr["Auto_Rotation_Angle"].ToString());
                data.Manual_Rotation_Angle = Convert.ToInt32(rdr["Manual_Rotation_Angle"].ToString());
                data.Exposure_Time = Convert.ToInt32(rdr["Exposure_Time"].ToString());
                data.Gain = Convert.ToInt32(rdr["Gain"].ToString());
                data.Gamma = Convert.ToInt32(rdr["Gamma"].ToString());
                data.Filter = Convert.ToInt32(rdr["Filter"].ToString());
            }
            if (data.Position1 == "")
            {
                data.Position1 = null;
            }
            if (data.Position2 == "")
            {
                data.Position2 = null;
            }
        }
    }

    /// <summary>
    /// 스캔
    /// </summary>
    public partial class MainWindow : Window
    {
        private void ScanLoad()
        {
            MoveToTab("촬영");
            //tabControl.SelectedIndex = 3;
        }
        private void Scan_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("스캔 로드");
            ScanLoad();
            string sql = $"SELECT Count(*) AS Ck FROM DB_Injection WHERE HID = '{slt.SelectHID}' AND Date = '{DateTime.Now.ToString("yyyyMMdd")}'";
            SQLiteDataReader rdr = db.Load(sql);
            if (rdr.Read())
            {
                if (rdr["ck"].ToString() == "0")
                {
                    InjectionWindow injection_Window = new InjectionWindow();
                    injection_Window.ShowDialog();
                    if (injection_Window.DialogResult.HasValue && injection_Window.DialogResult.Value)
                    {
                        string returnedText = injection_Window.Tag as string;
                        slt.SelectInjectionTime = $"{returnedText}";
                        sql = $"insert into  DB_Injection values ('{slt.SelectHID}','{DateTime.Now.ToString("yyyyMMdd")}','{returnedText}')";
                        db.Save(sql);
                    }
                }
                else
                {
                    sql = $"SELECT Injection_Time From DB_Injection WHERE HID = '{slt.SelectHID}' AND Date = '{DateTime.Now.ToString("yyyyMMdd")}'";
                    SQLiteDataReader rdr2 = db.Load(sql);
                    if(rdr2.Read())
                    {
                        slt.SelectInjectionTime = rdr2["Injection_Time"].ToString();
                        Console.WriteLine($"slt.SelectInjectionTime : {slt.SelectInjectionTime}");
                    }
                    rdr2.Close();
                }
            }
            rdr.Close();
        }
        private void ScanInit()
        {
            if (!cam.IsOpen())
            {
                cam.Setting(data.Exposure_Time, data.Gain, data.Gamma);
                cam.StartLiveView();
                Gain.Content = data.Gain * 3 + "dB";
            }
        }
        private void shotposition_Loaded(object sender, RoutedEventArgs e)
        {
            shotposition.Content = data.shotposition;
        }
        private void Cam_CaptureImageHandler(string serialNum, IManagedImage managedImage)
        {
            ShowLiveView(serialNum, managedImage);
        }
        private void ShowLiveView(string serialNum, IManagedImage managedImage)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                imgSelect.Image_Size(data.View_Range);
                OpenCvSharp.Mat image = new OpenCvSharp.Mat((int)managedImage.Height, (int)managedImage.Width, OpenCvSharp.MatType.CV_8UC3, managedImage.DataPtr, managedImage.Stride);
                if (data.Filter == 0)
                {
                    OpenCvSharp.Cv2.GaussianBlur(image, image, new OpenCvSharp.Size(3, 3), 1, 0, OpenCvSharp.BorderTypes.Default);
                }
                OpenCvSharp.Mat View_image = imgSelect.Cut_Image(image.Clone());
                //View_image = OpenCvSharp.Cv2.ImRead(@"ICG_A0.png");
                OpenCvSharp.Mat View_image2 = imgSelect.binary(View_image, Convert.ToInt32(slidercount.Content));
                OpenCvSharp.Mat View_image3 = imgSelect.Edge(View_image, Convert.ToInt32(slidercount.Content));
                OpenCvSharp.Mat Contour_image = new OpenCvSharp.Mat();
                OpenCvSharp.Mat ColorMap_image = new OpenCvSharp.Mat();
                OpenCvSharp.Mat Not_image = new OpenCvSharp.Mat();
                OpenCvSharp.Mat View_image4 = View_image3.Clone();
                OpenCvSharp.Cv2.CvtColor(View_image4, View_image4, OpenCvSharp.ColorConversionCodes.GRAY2BGR);
                OpenCvSharp.Cv2.CvtColor(View_image3, View_image3, OpenCvSharp.ColorConversionCodes.GRAY2BGR);
                View_image4 = imgSelect.LUTView(View_image4, data.ColorMap);
                
                imgSelect.GridView(data.Grid, View_image);
                
                if (data.Contour == 1)
                {
                    OpenCvSharp.Mat img = new OpenCvSharp.Mat();
                    OpenCvSharp.Mat img2 = new OpenCvSharp.Mat();
                    OpenCvSharp.Cv2.BitwiseAnd(View_image4, View_image3, img);
                    OpenCvSharp.Cv2.BitwiseNot(View_image3, View_image3);
                    OpenCvSharp.Cv2.BitwiseAnd(View_image, View_image3, img2);
                    View_image = img + img2;
                }
                else
                {

                }
                
                if (data.ColorMap == 1)
                {
                    OpenCvSharp.Cv2.BitwiseNot(View_image, Not_image);
                    OpenCvSharp.Cv2.ApplyColorMap(Not_image, View_image, OpenCvSharp.ColormapTypes.Rainbow);
                }
                else if (data.ColorMap == 2)
                {
                    OpenCvSharp.Cv2.BitwiseNot(View_image, View_image);
                }

                
                MainImage.Source = WriteableBitmapConverter.ToWriteableBitmap(View_image);


            }));

        }
        private void MainImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point ClickPos = e.GetPosition((IInputElement)sender);
            int ClickY = (int)ClickPos.Y;
            if (ClickY >= MainImage.ActualHeight / 2)
            {
                data.Gain = data.Gain - 1;
            }
            else
            {
                data.Gain = data.Gain + 1;
            }
            if (data.Gain == -1)
            {
                data.Gain = 0;
            }
            else if (data.Gain == 16)
            {
                data.Gain = 15;
            }
            else
            {

            }

            cam.Setting(data.Exposure_Time, data.Gain, data.Gamma);


            if (DB != null)
            {
                DB.BeginAnimation(EffectProperty, null);
                MainGrid.Children.Remove(DB);
            }

            sw.Restart();
            DB = new Label();
            DB.FontSize = 50;
            DB.VerticalContentAlignment = VerticalAlignment.Center;
            DB.HorizontalContentAlignment = HorizontalAlignment.Center;
            DB.Content = data.Gain * 3 + "dB";
            DB.Foreground = Brushes.White;
            MainGrid.Children.Add(DB);
            animation = new DoubleAnimation(0, TimeSpan.FromSeconds(2));
            animation.Completed += Animation_Completed;
            // 블러 효과를 생성합니다.
            /*BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 2;*/

            // Label에 블러 효과를 적용합니다.
            /*DB.Effect = blurEffect;
            blurEffect.BeginAnimation(BlurEffect.RadiusProperty, animation);*/

            DB.BeginAnimation(Label.OpacityProperty, animation);
            Gain.Content = data.Gain * 3 + "dB";
        }
        private void Animation_Completed(object sender, EventArgs e)
        {
            sw.Stop();
            if (sw.ElapsedMilliseconds > 1800)
            {
                MainGrid.Children.Remove(DB);
            }
            else
            {
                sw.Start();
            }
        }
        private void Wave()
        {
            Ellipse ellipse = new Ellipse
            {
                Fill = System.Windows.Media.Brushes.White,
                Width = 0,
                Height = 0,
                Opacity = 0.5
            };
            ellipse.IsHitTestVisible = false;
            MainGrid.Children.Add(ellipse);
            ellipse.HorizontalAlignment = HorizontalAlignment.Center;
            ellipse.VerticalAlignment = VerticalAlignment.Center;
            double diagonalLength = Math.Sqrt(Math.Pow(SystemParameters.PrimaryScreenWidth, 2) + Math.Pow(SystemParameters.PrimaryScreenHeight, 2));
            DoubleAnimation widthAnimation = new DoubleAnimation
            {
                From = 0,
                To = diagonalLength * 2,
                Duration = TimeSpan.FromSeconds(1)
            };

            DoubleAnimation heightAnimation = new DoubleAnimation
            {
                From = 0,
                To = diagonalLength * 2,
                Duration = TimeSpan.FromSeconds(1)
            };

            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = 0.5,
                To = 0,
                Duration = TimeSpan.FromSeconds(1)
            };

            Storyboard.SetTarget(widthAnimation, ellipse);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(WidthProperty));

            Storyboard.SetTarget(heightAnimation, ellipse);
            Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(HeightProperty));

            Storyboard.SetTarget(opacityAnimation, ellipse);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(widthAnimation);
            storyboard.Children.Add(heightAnimation);
            storyboard.Children.Add(opacityAnimation);

            storyboard.Begin();
        }
        private void ScanData_Click(object sender, RoutedEventArgs e)
        {
            ScanData scandata_Window = new ScanData(main, data, imgSelect, cam, serial_servo);
            scandata_Window.ShowDialog();
            if (scandata_Window.DialogResult.HasValue && scandata_Window.DialogResult.Value)
            {
                logdata.WriteLog("스캔데이터 설정완료");
                serial_PCB.WriteMessage("SE", "\n");
                servo.PositionLabelValue(data.SelectPosition());
                PositionLabel.Content = servo.GetPositionLabel();
                Gain.Content = data.Gain * 3 + "dB";
            }
            
        }
        private void Servo_Preview_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("프리뷰 클릭");
            if (servo.Cmd("Preview", imgSelect, data) != 0)
            {
                Reset.IsEnabled = false;
                Reverse.IsEnabled = false;
                Forward.IsEnabled = false;
                AutoScan.IsEnabled = false;
            }
            PositionLabel.Content = servo.GetPositionLabel();
        }
        private void Servo_Reset_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("리셋 클릭");
            if (servo.MoveState())
            {
                Preview.IsEnabled = false;
                Reset.IsEnabled = false;
                Reverse.IsEnabled = false;
                Forward.IsEnabled = false;
                AutoScan.IsEnabled = AutoscanEnabled();
                servo.Cmd("Reset",data);
            }
            else
            {
                MessageBox.Show("모터가 동작 중입니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            PositionLabel.Content = servo.GetPositionLabel();
        }
        public bool AutoscanEnabled()
        {
            return serial_servo.IsOpen() && data.ScanDataCheck && !servo.TimerMove;
        }
        private void Servo_Reverse_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("역회전 클릭");
            servo.Cmd("Reverse", data);
            PositionLabel.Content = servo.GetPositionLabel();
        }
        private void Servo_Forward_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("정회전 클릭");
            servo.Cmd("Forward", data);
            PositionLabel.Content = servo.GetPositionLabel();
        }
        private void Up_Actuator(object sender, MouseButtonEventArgs e)
        {
            logdata.WriteLog("마우스 리니어 모터 UP 클릭");
            serial_PCB.WriteMessage("AL2", "\n");
        }
        private void Stop_Up_Actuator(object sender, MouseButtonEventArgs e)
        {
            logdata.WriteLog("마우스 리니어 모터 정지");
            serial_PCB.WriteMessage("AL0", "\n");
        }
        private void Down_Actuator(object sender, MouseButtonEventArgs e)
        {
            logdata.WriteLog("마우스 리니어 모터 Down 클릭");
            serial_PCB.WriteMessage("AL1", "\n");
        }
        private void Up_Actuator(object sender, TouchEventArgs e)
        {
            logdata.WriteLog("터치 리니어 모터 UP 클릭");
            serial_PCB.WriteMessage("AL2", "\n");
        }
        private void Stop_Up_Actuator(object sender, TouchEventArgs e)
        {
            logdata.WriteLog("터치 리니어 모터 정지");
            serial_PCB.WriteMessage("AL0", "\n");
        }
        private void Down_Actuator(object sender, TouchEventArgs e)
        {
            logdata.WriteLog("터치 리니어 모터 Down 클릭");
            serial_PCB.WriteMessage("AL1", "\n");
        }
        private void AutoScan_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("모터를 작동시키겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                logdata.WriteLog("오토 촬영");
                imgSelect.Auto_ICG_Image.Clear();
                servo.Cmd("Reset",data);
                PositionLabel.Content = servo.GetPositionLabel();
                if(data.LED == 0 && data.LED2 == 0)
                {
                    data.LED = 1;
                    LED.Foreground = Brushes.White;
                    LED2.Foreground = Brushes.Red;
                    serial_PCB.WriteMessage("LD1", "\n");
                    serial_PCB.WriteMessage("FA1", "\n");
                }
                timer.Start();
            }
        }
        private void AutoTimer_Tick(object sender, EventArgs e)
        {
            if (servo.MoveState())
            {

            }
            else
            {
                servo.LastMove(DateTime.Now);
            }
            if (DateTime.Now - servo.GetLastMoveTime() >= TimeSpan.FromTicks(6000000) && servo.LastMoveState == true)
            {
                if (servo.GetAngle() + (data.Auto_Rotation_Angle + 1) * 45 < 360)
                {
                    imgSelect.AutoScreenshot();
                    servo.Cmd("Auto", data);
                    /*if (data.Auto_Rotation_Angle == 0)
                    {
                        servo.Cmd_45(data.SelectPosition());
                    }
                    else if (data.Auto_Rotation_Angle == 1)
                    {
                        servo.Cmd_90(data.SelectPosition());
                    }*/
                    PositionLabel.Content = servo.GetPositionLabel();
                }
                else
                {
                    imgSelect.AutoScreenshot();
                    servo.Cmd("Reset",data);
                    timer.Stop();
                    PositionLabel.Content = servo.GetPositionLabel();
                    serial_PCB.WriteMessage("LD0", "\n");
                    if(data.LED != 0)
                    {
                        for (int i = 0; i < 4 - data.LED; i++)
                        {
                            serial_PCB.WriteMessage("LE", "\n");
                        }
                    }
                    data.LED = 0;
                    data.LED2 = 0;
                    serial_PCB.WriteMessage("FA0", "\n");
                    LED.Foreground = Brushes.White;
                    LED2.Foreground = Brushes.White;
                    logdata.WriteLog("오토 촬영 종료");
                }
                servo.LastMove(DateTime.Now);
            }
        }
        private void Screenshot_Click(object sender, RoutedEventArgs e)
        {
            imgSelect.Screenshot();
            Wave();
            logdata.WriteLog("메뉴얼 촬영");
        }
        private void Video_Click(object sender, RoutedEventArgs e)
        {
            VideoClick();
        }
        private void Dicom_Click(object sender, RoutedEventArgs e)
        {
            DicomClick();
        }
        private void VideoClick()
        {
            if (imgSelect.Video_Click(textblock_MainHID.Text, data, slt, divice) == 1)
            {
                logdata.WriteLog("비디오 촬영");
                Button_Video.Foreground = Brushes.Red;
                Dicom_Video.IsEnabled = false;
                REC.Visibility = Visibility.Visible;
                REC2.Visibility = Visibility.Visible;
                imgSelect.OnTimeChanged += TimerManager_OnTimeChanged;
            }
            else
            {
                logdata.WriteLog("비디오 촬영 종료");
                Button_Video.Foreground = Brushes.White;
                Dicom_Video.IsEnabled = true;
                REC.Visibility = Visibility.Hidden;
                REC2.Visibility = Visibility.Hidden;
                imgSelect.OnTimeChanged -= TimerManager_OnTimeChanged;
                MessageBox.Show("비디오가 저장되었습니다.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void DicomClick()
        {
            if (imgSelect.Dicom_Click(textblock_MainHID.Text, data, slt, divice) == 1)
            {
                logdata.WriteLog("다이콤비디오 촬영");
                Dicom_Video.Foreground = Brushes.Red;
                Button_Video.IsEnabled = false;
                REC.Visibility = Visibility.Visible;
                REC2.Visibility = Visibility.Visible;
                imgSelect.OnTimeChanged += DicomTimerManager_OnTimeChanged;
            }
            else
            {
                logdata.WriteLog("다이콤비디오 촬영 종료");
                Dicom_Video.Foreground = Brushes.White;
                Button_Video.IsEnabled = true;
                REC.Visibility = Visibility.Hidden;
                REC2.Visibility = Visibility.Hidden;
                imgSelect.OnTimeChanged -= DicomTimerManager_OnTimeChanged;
                MessageBox.Show("다이콤비디오가 저장되었습니다.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void TimerManager_OnTimeChanged(TimeSpan time)
        {
            // UI 스레드에서 Label 업데이트
            Dispatcher.Invoke(() =>
            {
                string elapsedTime = String.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
                REC2.Content = $"REC({elapsedTime})"; // TimeLabel은 XAML에서 정의된 Label의 이름입니다.
                if(time.Minutes >= 15)
                {
                    VideoClick();
                }
            });
        }
        private void DicomTimerManager_OnTimeChanged(TimeSpan time)
        {
            // UI 스레드에서 Label 업데이트
            Dispatcher.Invoke(() =>
            {
                string elapsedTime = String.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
                REC2.Content = $"REC({elapsedTime})"; // TimeLabel은 XAML에서 정의된 Label의 이름입니다.
                if (time.Minutes >= 1)
                {
                    DicomClick();
                }
            });
        }
        private void ServoMove_OnTimeChanged(bool state)
        {
            Dispatcher.Invoke(() =>
            {
                if(state)
                {
                    serial_PCB.WriteMessage("MO1", "\n");
                }
                else
                {
                    serial_PCB.WriteMessage("MO0", "\n");
                }
            });
        }
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            ImageCheck ImageCheck_Window = new ImageCheck(main, imgSelect, data, db, slt, divice);
            ImageCheck_Window.ShowDialog();
            if (ImageCheck_Window.DialogResult.HasValue && ImageCheck_Window.DialogResult.Value)
            {
                logdata.WriteLog("데이터 저장");
                MessageBox.Show("이미지가 저장되었습니다.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                dm.Load(db);
            }
                
        }
        private void Grid_Click(object sender, RoutedEventArgs e)
        {
            imgSelect.GridVal();
            logdata.WriteLog("그리드 클릭");
        }
        private void ColorMap_Click(object sender, RoutedEventArgs e)
        {
            data.ColorMap++;
            if (data.ColorMap == 3)
            {
                data.ColorMap = 0;
            }
            logdata.WriteLog("컬러맵 클릭");
        }
        private void Led2_Click(object sender, RoutedEventArgs e)
        {
            data.LED2++;
            if (data.LED2 == 2)
            {
                data.LED2 = 0;
            }
            if (data.LED2 == 0)
            {
                serial_PCB.WriteMessage("LD0", "\n");
                serial_PCB.WriteMessage("LE0", "\n");
                LED.Foreground = Brushes.White;
                LED2.Foreground = Brushes.White;
            }
            else if (data.LED2 == 1)
            {
                serial_PCB.WriteMessage("LD1", "\n");
                serial_PCB.WriteMessage("LE0", "\n");
                data.LED = 0;
                LED.Foreground = Brushes.White;
                LED2.Foreground = Brushes.Red;

            }
            FanControl();
            logdata.WriteLog("ICG LED 클릭");
        }
        private void Led_Click(object sender, RoutedEventArgs e)
        {
            /*data.LED++;
            serial_PCB.WriteMessage("LE", "\n");
            if (data.LED == 4)
            {
                data.LED = 0;
            }
            if(data.LED == 0)
            {
                LED.Foreground = Brushes.White;
            }
            else if (data.LED == 1)
            {
                LED.Foreground = Brushes.Yellow;
            }
            else if (data.LED == 2)
            {
                LED.Foreground = Brushes.Orange;
            }
            else if (data.LED == 3)
            {
                LED.Foreground = Brushes.Red;
            }
            FanControl();
            logdata.WriteLog("Dual LED 클릭");*/
            data.LED++;
            if (data.LED == 2)
            {
                data.LED = 0;
            }
            if (data.LED == 0)
            {
                serial_PCB.WriteMessage("LD0", "\n");
                serial_PCB.WriteMessage("LE0", "\n");
                LED.Foreground = Brushes.White;
                LED2.Foreground = Brushes.White;
            }
            else if (data.LED == 1)
            {
                serial_PCB.WriteMessage("LD1", "\n");
                serial_PCB.WriteMessage("LE1", "\n");
                data.LED2 = 0;
                LED.Foreground = Brushes.Red;
                LED2.Foreground = Brushes.White;
            }
            FanControl();
            logdata.WriteLog("Dual LED 클릭");
        }
        private void FanControl()
        {
            Console.WriteLine($"data.LED : {data.LED} data.LED2 : {data.LED2}");
            if(data.LED == 0 && data.LED2 == 0)
            {
                serial_PCB.WriteMessage("FA0", "\n");
            }
            else
            {
                serial_PCB.WriteMessage("FA1", "\n");
            }
        }
        private void Contour_Click(object sender, RoutedEventArgs e)
        {
            data.Contour++;
            if (data.Contour == 2)
            {
                data.Contour = 0;
            }
            if(data.Contour == 0)
            {
                Contour.Foreground = Brushes.White;
            }
            else
            {
                Contour.Foreground = Brushes.Red;
            }
            logdata.WriteLog("컨투어 클릭");
        }
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            data.SliderVal = (int)slider.Value;
            slidercount.Content = (int)slider.Value;
        }
        private void Serial_DataReceived(object sender, string e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e == "Short" || e == "Nomal" || e == "Long" || e == "Custom")
                {
                    ssv.Mode = e;
                    if (Position1.Content != null)
                    {
                        if (Position1.Content.ToString().Trim() == "Check_FF" || Position1.Content.ToString().Trim() == "Check_Others")
                        {
                            shotposition.Content = "Custom";
                            data.shotposition = "Custom";
                        }
                        else
                        {
                            if (e == "Short" || e == "Nomal" || e == "Long" || e == "Custom")
                            {
                                shotposition.Content = e;
                                data.shotposition = e;
                            }
                        }
                    }
                    else
                    {
                        shotposition.Content = "";
                        data.shotposition = "";
                    }
                    logdata.WriteLog($"수신 데이터 : {e}");
                }
                if(e == "0,0"|| e == "1,0" || e == "0,1" || e == "1,1")
                {
                    string[] sensorlist = e.Split(',');
                    ssv.Sensor1 = sensorlist[0];
                    ssv.Sensor2 = sensorlist[1];
                    logdata.WriteLog($"수신 데이터 : {e}");
                }
                

            });
        }
        private void Position_Click(object sender, RoutedEventArgs e)
        {
            if (imgSelect.Auto_ICG_Image.Count() != 0 || imgSelect.Manual_ICG_Image.Count() != 0)
            {
                if (MessageBox.Show("저장되지 않은 이미지가 존재합니다.\n삭제하고 진행하시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    imgSelect.Auto_ICG_Image.Clear();
                    imgSelect.Manual_ICG_Image.Clear();
                    var selectBtn = Grid_Color.Children.OfType<Button>().FirstOrDefault(r => r.Background == Brushes.Yellow);
                    if (selectBtn != null)
                    {
                        selectBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x7F, 0x7F, 0x7F));
                        selectBtn.Foreground = Brushes.White;
                        Button btn = sender as Button;
                        btn.Background = Brushes.Yellow;
                        btn.Foreground = Brushes.Red;
                        data.Position_Now = Convert.ToInt32(btn.Name.Substring(8, 1));
                    }
                }
            }
            else
            {
                var selectBtn = Grid_Color.Children.OfType<Button>().FirstOrDefault(r => r.Background == Brushes.Yellow);
                if (selectBtn != null)
                {
                    selectBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x7F, 0x7F, 0x7F));
                    selectBtn.Foreground = Brushes.White;
                    Button btn = sender as Button;
                    btn.Background = Brushes.Yellow;
                    btn.Foreground = Brushes.Red;
                    data.Position_Now = Convert.ToInt32(btn.Name.Substring(8, 1));
                }
            }
            servo.PositionLabelValue(data.SelectPosition());
            PositionLabel.Content = servo.GetPositionLabel();
        }
    }
    /// <summary>
    /// 이미지리뷰
    /// </summary>
    public partial class MainWindow : Window
    {
        private void Review_Click(object sender, RoutedEventArgs e)
        {
            if (imgSelect.GetVideo() == 1)
            {
                MessageBox.Show("녹화를 종료하고 진행해 주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                ReviewLoad();
            }
        }
        private void ReviewLoad()
        {
            logdata.WriteLog("리뷰 로드");
            MoveToTab("이미지 뷰어 리스트");
            //tabControl.SelectedIndex = 4;
        }
        private void Image_Review_Click(object sender, RoutedEventArgs e)
        {
            if (List_ImageReview.SelectedItems.Count != 0)
            {
                logdata.WriteLog("이미지리뷰 로드");
                ImageViewerLoad();
            }
            else
            {
                MessageBox.Show("이미지 리스트를 선택해 주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ReportSet_Click(object sender, RoutedEventArgs e)
        {
            if (List_ImageReview.SelectedItems.Count == 1)
            {
                if (modecheck())
                {
                    rpt = new ReportImage();
                    ReportSetLoad();
                }
                else
                {
                    MessageBox.Show("Auto 이미지리스트를 선택 해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (List_ImageReview.SelectedItems.Count == 0)
            {
                MessageBox.Show("이미지리스트를 선택 해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show("하나의 이미지리스트만 선택 해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private bool modecheck()
        {
            string sql = "SELECT DISTINCT Mode From DB_ShotSave WHERE(HID,Date, Position, Sequences, Mode)IN(SELECT DISTINCT HID,Date, Position, Sequences, Mode FROM DB_ShotSave where HID = '" + slt.SelectHID + "'ORDER BY Date DESC, CAST(Sequences AS INT) LIMIT 1 OFFSET " + List_ImageReview.SelectedIndex + ")";
            SQLiteDataReader rdr = db.Load(sql);
            bool check = false;
            while (rdr.Read())
            {
                if (rdr["Mode"].ToString() == "Auto")
                {
                    check = true;
                }
            }
            rdr.Close();
            return check;
        }
        private void List_ImageReview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (List_ImageReview.SelectedItems.Count != 0)
            {
                List<int> selectedIndexes = new List<int>();
                int lastClickedIndex = 0;
                var selectedItems = List_ImageReview.SelectedItems;
                if (selectedItems.Count > 0)
                {
                    var lastSelectedItem = selectedItems[selectedItems.Count - 1];
                    lastClickedIndex = List_ImageReview.Items.IndexOf(lastSelectedItem);
                    logdata.WriteLog($"이미지 선택 : {lastClickedIndex}");
                }
                string sql = "SELECT FileName,Comment From DB_ShotSave WHERE(HID,Date,Position, Sequences, Mode)IN(SELECT DISTINCT HID,Date,Position, Sequences, Mode FROM DB_ShotSave where HID = '" + slt.SelectHID + "'ORDER BY Date DESC,CAST(Sequences AS INT) LIMIT 1 OFFSET " + lastClickedIndex + ")";
                SQLiteDataReader rdr = db.Load(sql);
                List<string> path = new List<string>();
                List<BitmapImage> source = new List<BitmapImage>();
                while (rdr.Read())
                {
                    path.Add(@"dicom" + @"\" + rdr["FileName"].ToString() + ".dcm");
                    Comment.Text = rdr["Comment"].ToString();
                }
                rdr.Close();
                for (int i = 0; i < path.Count(); i++)
                {
                    new DicomSetupBuilder().RegisterServices(s => s.AddFellowOakDicom().AddImageManager<WinFormsImageManager>()).Build();
                    var m_pDicomFile = DicomFile.Open(path[i]);
                    var dicomImage = new DicomImage(m_pDicomFile.Dataset);
                    System.Drawing.Bitmap bitmap = dicomImage.RenderImage().As<System.Drawing.Bitmap>();
                    source.Add(ConvertBitmapToBitmapImage(bitmap));
                }
                Image_Count(ImageGrid, path.Count(), 500, 200, 5, 2, source);
            }
            else
            {
                ImageGrid.Children.Clear();
                Comment.Clear();
            }
        }
        private void Image_Count(Grid grid, int Count, int width, int height, int thickness, int gridx, List<BitmapImage> source)
        {
            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
            var columnDefinitions = grid.ColumnDefinitions;
            var RowDefinitions = grid.RowDefinitions;
            for (int x = 0; x < gridx; x++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column);
                columnDefinitions[x].Width = new GridLength(1, GridUnitType.Star);
            }
            for (int x = 0; x < (Count + gridx - 1) / gridx; x++)
            {
                RowDefinition Row = new RowDefinition();
                grid.RowDefinitions.Add(Row);
                RowDefinitions[x].Height = new GridLength(height + 10);
            }
            for (int x = 0; x < (Count + gridx - 1) / gridx; x++)
            {
                for (int y = 0; y < gridx; y++)
                {
                    if (x * 2 + y != Count)
                    {
                        Border border = new Border();
                        System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                        image.Source = source[x * 2 + y];
                        //image.Source = WriteableBitmapConverter.ToWriteableBitmap(ImageList[count]);
                        //border.MouseDown += new MouseButtonEventHandler(Border_MouseDown);
                        //border.Tag = count;
                        border.Width = width; //540
                        border.Height = height; //288
                        border.BorderThickness = new Thickness(thickness);//5
                        border.Child = image;
                        border.BorderBrush = System.Windows.Media.Brushes.Blue;
                        Grid.SetRow(border, x);
                        Grid.SetColumn(border, y);
                        grid.Children.Add(border);
                    }
                    //count++;
                }
            }
        }
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            if (List_ImageReview.SelectedItems.Count == 1)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Filter = "PNG 파일 (*.png)|*.png", // 파일 형식 필터
                    DefaultExt = ".png", // 기본 확장자
                    AddExtension = true // 파일 이름에 
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    // 사용자가 선택한 파일 경로
                    string filePath = saveFileDialog.FileName;
                    // Bitmap 객체를 사용자가 선택한 경로에 PNG로 저장
                    List<int> selectedIndexes = new List<int>();
                    int lastClickedIndex = 0;
                    var selectedItems = List_ImageReview.SelectedItems;
                    if (selectedItems.Count > 0)
                    {
                        var lastSelectedItem = selectedItems[selectedItems.Count - 1];
                        lastClickedIndex = List_ImageReview.Items.IndexOf(lastSelectedItem);
                        logdata.WriteLog($"이미지 선택 : {lastClickedIndex}");
                    }
                    string sql = "SELECT FileName From DB_ShotSave WHERE(HID, Date,Position, Sequences, Mode)IN(SELECT DISTINCT HID,Date,Position, Sequences, Mode FROM DB_ShotSave where HID = '" + slt.SelectHID + "'ORDER BY Date DESC,CAST(Sequences AS INT) LIMIT 1 OFFSET " + lastClickedIndex + ")";
                    SQLiteDataReader rdr = db.Load(sql);
                    List<string> path = new List<string>();
                    List<BitmapImage> source = new List<BitmapImage>();
                    while (rdr.Read())
                    {
                        path.Add(@"dicom" + @"\" + rdr["FileName"].ToString() + ".dcm");
                    }
                    rdr.Close();
                    for (int i = 0; i < path.Count(); i++)
                    {
                        new DicomSetupBuilder().RegisterServices(s => s.AddFellowOakDicom().AddImageManager<WinFormsImageManager>()).Build();
                        var m_pDicomFile = DicomFile.Open(path[i]);
                        var dicomImage = new DicomImage(m_pDicomFile.Dataset);
                        System.Drawing.Bitmap bitmap = dicomImage.RenderImage().As<System.Drawing.Bitmap>();
                        bitmap.Save($"{filePath.Replace(".png", "")}{i}.png", ImageFormat.Png);
                    }
                };
            }
            else if (List_ImageReview.SelectedItems.Count == 0)
            {
                MessageBox.Show("이미지 리스트를 선택해 주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show("하나의 이미지리스트만 선택 해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        private async void SendToPacs_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("Pacs 전송");
            if (List_ImageReview.SelectedItems.Count != 0)
            {
                var selectedItems = List_ImageReview.SelectedItems;
                List<string> selectedIndexes = new List<string>();
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    var select = selectedItems[i];
                    selectedIndexes.Add(List_ImageReview.Items.IndexOf(select).ToString());
                }
                selectedIndexes.Sort();
                for (int i = 0; i < selectedIndexes.Count(); i++)
                {
                    string sql = "SELECT FileName,Date,Position,Sequences,Mode From DB_ShotSave WHERE(HID,Date,Position, Sequences, Mode)IN(SELECT DISTINCT HID,Date,Position, Sequences, Mode FROM DB_ShotSave where HID = '" + slt.SelectHID + "'ORDER BY Date DESC, CAST(Sequences AS INT) LIMIT 1 OFFSET " + selectedIndexes[i] + ")";
                    SQLiteDataReader rdr = db.Load(sql);
                    List<string> path = new List<string>();
                    while (rdr.Read())
                    {
                        path.Add(@"dicom" + @"\" + rdr["FileName"].ToString() + ".dcm");
                    }
                    rdr.Close();
                    int successfulTransfers = 0;
                    for (int j = 0; j < path.Count(); j++)
                    {
                        //SendToPACS(path, "111222333", "200.200.1.16", 9592, "MAROTECH");
                        try
                        {
                            await SendToPacs(path[j], "111222333", IP.Text, int.Parse(Port.Text), AET.Text);
                            PacsDataSavebtn.IsEnabled = true;
                            successfulTransfers++;
                            if (successfulTransfers == path.Count())
                            {
                                MessageBox.Show("전송 완료", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                logdata.WriteLog("Pacs 전송 완료");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"전송 중 오류 발생: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            logdata.WriteLog($"Pacs 전송 실패 : {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("이미지리스트를 선택 해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CommentSave(object sender, RoutedEventArgs e)
        {
            if (List_ImageReview.SelectedItems.Count == 1)
            {
                int lastClickedIndex = 0;
                var selectedItems = List_ImageReview.SelectedItems;
                if (selectedItems.Count > 0)
                {
                    var lastSelectedItem = selectedItems[selectedItems.Count - 1];
                    lastClickedIndex = List_ImageReview.Items.IndexOf(lastSelectedItem);
                }
                string sql = "UPDATE DB_ShotSave SET Comment = '" + Comment.Text + "' WHERE(HID, Date, Position, Sequences, Mode)IN(SELECT DISTINCT HID, Date, Position, Sequences, Mode FROM DB_ShotSave where HID = '" + slt.SelectHID + "' ORDER BY Date DESC, CAST(Sequences AS INT) LIMIT 1 OFFSET " + lastClickedIndex + ")";
                db.Save(sql);
                MessageBox.Show("코멘트 저장 완료.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (List_ImageReview.SelectedItems.Count == 0)
            {
                MessageBox.Show("이미지리스트를 선택 해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show("하나의 이미지리스트만 선택 해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        /*public void SendToPACS(string dcmfile, string sourceAET, string targetIP, int targetPort, string targetAET)
        {
            var m_pDicomFile = DicomFile.Open(dcmfile);
            var client = DicomClientFactory.Create(targetIP, targetPort, false, sourceAET, targetAET);//("127.0.0.1", 12345, false, "SCU", "ANY-SCP")
            client.NegotiateAsyncOps();
            client.AddRequestAsync(new DicomCStoreRequest(m_pDicomFile));
            client.SendAsync();
        }*/

    }
    
    /// <summary>
    /// 이미지리뷰2
    /// </summary>
    public partial class MainWindow : Window
    {
        DivisionData dvs;
        private void ImageViewerLoad()
        {
            MoveToTab("이미지 뷰어");
            //tabControl.SelectedIndex = 5;
        }
        private void Image_Review_Select(Grid grid, int width, int height, int thickness, OpenCvSharp.Mat[][] source, List<string> SelectName)
        {
            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
            var columnDefinitions = grid.ColumnDefinitions;
            var RowDefinitions = grid.RowDefinitions;
            int maxy = 0;
            for (int x = 0; x < source.Length; x++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column);
                columnDefinitions[x].Width = new GridLength(width + 10);

                if (maxy < source[x].Count())
                {
                    maxy = source[x].Count();
                }
            }
            for (int x = 0; x < maxy + 1; x++)
            {
                RowDefinition Row = new RowDefinition();
                grid.RowDefinitions.Add(Row);
                if (x == 0)
                {
                    RowDefinitions[x].Height = new GridLength(50);
                }
                else
                {
                    RowDefinitions[x].Height = new GridLength(height + 10);
                }
            }

            for (int x = 0; x < source.Length; x++)
            {
                Label Label = new Label();
                Label.Content = SelectName[x];
                Label.FontSize = 20;
                Label.VerticalAlignment = VerticalAlignment.Center;
                Label.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(Label, 0);
                Grid.SetColumn(Label, x);
                grid.Children.Add(Label);
                for (int y = 0; y < source[x].Count(); y++)
                {
                    Border border = new Border();
                    System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                    image.Source = WriteableBitmapConverter.ToWriteableBitmap(source[x][y]);
                    //image.Source = WriteableBitmapConverter.ToWriteableBitmap(ImageList[count]);
                    //border.MouseDown += new MouseButtonEventHandler(Border_MouseDown);
                    //border.Tag = count;

                    image.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(image_MouseLeftButtonDown);
                    image.Tag = x + "," + y;
                    //image.DragEnter += new DragEventHandler(image_DragEnter);
                    border.Width = width; //540
                    border.Height = height; //288
                    border.BorderThickness = new Thickness(thickness);//5
                    border.Child = image;
                    border.BorderBrush = System.Windows.Media.Brushes.Blue;
                    Grid.SetRow(border, y + 1);
                    Grid.SetColumn(border, x);
                    grid.Children.Add(border);
                    //count++;
                }

            }
        }
        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image draggedImage = sender as System.Windows.Controls.Image;
            if (draggedImage != null)
            {
                DataObject dragData = new DataObject(draggedImage);
                DragDrop.DoDragDrop(draggedImage, dragData, DragDropEffects.Copy);
            }
        }
        private void img_View_Drop(object sender, DragEventArgs e)
        {
            System.Windows.Controls.Image droppedImage = e.Data.GetData(typeof(System.Windows.Controls.Image)) as System.Windows.Controls.Image;
            Border targetBorder = sender as Border;

            if (droppedImage != null && targetBorder != null)
            {
                logdata.WriteLog($"이미지 드롭 : {targetBorder}");
                // 드롭된 이미지를 해당 이미지 영역에 설정
                if (targetBorder == Border1)
                {
                    irv = new ImageProcessingReview();
                    img_View1.Source = droppedImage.Source;
                    irv.ImageTag(droppedImage.Tag.ToString());
                    irv.image = slt.selctsource[irv.GetX()][irv.GetY()];
                    information1_View1.Content = slt.Viewdata1;
                    information2_View1.Text = slt.Viewdata2[irv.GetX()][irv.GetY()];
                    information3_View1.Content = slt.Viewdata3[irv.GetX()][irv.GetY()];
                    information4_View1.Content = slt.Viewdata4[irv.GetX()][irv.GetY()];
                }
                else if (targetBorder == Border2)
                {
                    irv2 = new ImageProcessingReview();
                    img_View2.Source = droppedImage.Source;
                    irv2.ImageTag(droppedImage.Tag.ToString());
                    irv2.image = slt.selctsource[irv2.GetX()][irv2.GetY()];
                    information1_View2.Content = slt.Viewdata1;
                    information2_View2.Text = slt.Viewdata2[irv2.GetX()][irv2.GetY()];
                    information3_View2.Content = slt.Viewdata3[irv2.GetX()][irv2.GetY()];
                    information4_View2.Content = slt.Viewdata4[irv2.GetX()][irv2.GetY()];
                }
                else if (targetBorder == Border3)
                {
                    irv3 = new ImageProcessingReview();
                    img_View3.Source = droppedImage.Source;
                    irv3.ImageTag(droppedImage.Tag.ToString());
                    irv3.image = slt.selctsource[irv3.GetX()][irv3.GetY()];
                    information1_View3.Content = slt.Viewdata1;
                    information2_View3.Text = slt.Viewdata2[irv3.GetX()][irv3.GetY()];
                    information3_View3.Content = slt.Viewdata3[irv3.GetX()][irv3.GetY()];
                    information4_View3.Content = slt.Viewdata4[irv3.GetX()][irv3.GetY()];
                }
                else if (targetBorder == Border4)
                {
                    irv4 = new ImageProcessingReview();
                    img_View4.Source = droppedImage.Source;
                    irv4.ImageTag(droppedImage.Tag.ToString());
                    irv4.image = slt.selctsource[irv4.GetX()][irv4.GetY()];
                    information1_View4.Content = slt.Viewdata1;
                    information2_View4.Text = slt.Viewdata2[irv4.GetX()][irv4.GetY()];
                    information3_View4.Content = slt.Viewdata3[irv4.GetX()][irv4.GetY()];
                    information4_View4.Content = slt.Viewdata4[irv4.GetX()][irv4.GetY()];
                }
            }
            e.Handled = true;
        }
        private void ScrenDivision_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("ScrenDivision 클릭");
            Border border = FindAndProcessYellowBordersInGrid(ViewGrid);
            if (border != null)
            {
                if (SrcenDivision.Foreground == System.Windows.Media.Brushes.Red)
                {
                    SrcenDivision.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                    ScrenDivisionOff(border,dvs.SetRow(),dvs.SetColumn());
                }
                else
                {
                    dvs = new DivisionData();
                    dvs.row = Grid.GetRow(border);
                    dvs.column = Grid.GetColumn(border);
                    SrcenDivision.Foreground = System.Windows.Media.Brushes.Red;
                    ScrenDivisionOn(border);
                }
            }
            
            /*if (SrcenDivision.Foreground == System.Windows.Media.Brushes.Red)
            {
                SrcenDivision.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                Grid.SetColumnSpan(Border1, 1);
                Grid.SetRowSpan(Border1, 1);
                Border1.Visibility = Visibility.Visible;
                Border2.Visibility = Visibility.Visible;
                Border3.Visibility = Visibility.Visible;
                Border4.Visibility = Visibility.Visible;
            }
            else
            {
                SrcenDivision.Foreground = System.Windows.Media.Brushes.Red;
                Grid.SetColumnSpan(Border1, 2);
                Grid.SetRowSpan(Border1, 2);
                Border1.Visibility = Visibility.Visible;
                Border2.Visibility = Visibility.Hidden;
                Border3.Visibility = Visibility.Hidden;
                Border4.Visibility = Visibility.Hidden;
            }*/
        }
        private Border FindAndProcessYellowBordersInGrid(Grid grid)
        {
            List<Border> yellowBorders = new List<Border>();

            foreach (var child in grid.Children)
            {
                if (child is Border border && border.BorderBrush is SolidColorBrush brush && brush.Color == Colors.Yellow)
                {
                    yellowBorders.Add(border);
                }
            }

            switch (yellowBorders.Count)
            {
                case 0:
                    MessageBox.Show("이미지를 선택해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case 1:
                    return yellowBorders[0];
                default:
                    MessageBox.Show("이미지를 하나만 선택해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }

            return null;
        }
        private void ScrenDivisionOn(Border element)
        {
            Grid.SetRow(element, 0);  // 행 위치를 0으로 설정
            Grid.SetRowSpan(element, 2); // 행 범위를 2로 설정
            Grid.SetColumn(element, 0); // 열 위치를 0으로 설정
            Grid.SetColumnSpan(element, 2);
            Panel.SetZIndex(element, 1);
            foreach (var child in ViewGrid.Children)
            {
                if (child != element)
                {
                    Panel.SetZIndex((UIElement)child, 0);
                }
            }
        }
        private void ScrenDivisionOff(Border element, int row, int column)
        {
            Grid.SetRow(element, row);  // 행 위치를 0으로 설정
            Grid.SetRowSpan(element, 1); // 행 범위를 2로 설정
            Grid.SetColumn(element, column); // 열 위치를 0으로 설정
            Grid.SetColumnSpan(element, 1);
        }
        private void ReviewGrid_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("리뷰 Grid 클릭");
            if (Border1.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View1.Source != null)
            {
                irv.GridSet();
                img_View1.Source = WriteableBitmapConverter.ToWriteableBitmap(irv.View());
            }

            if (Border2.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View2.Source != null)
            {
                irv2.GridSet();
                img_View2.Source = WriteableBitmapConverter.ToWriteableBitmap(irv2.View());
            }
            if (Border3.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View3.Source != null)
            {
                irv3.GridSet();
                img_View3.Source = WriteableBitmapConverter.ToWriteableBitmap(irv3.View());
            }
            if (Border4.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View4.Source != null)
            {
                irv4.GridSet();
                img_View4.Source = WriteableBitmapConverter.ToWriteableBitmap(irv4.View());
            }
        }
        private void ReviewColorMap_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("리뷰 ColorMap 클릭");
            if (Border1.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View1.Source != null)
            {
                irv.Colormap();
                img_View1.Source = WriteableBitmapConverter.ToWriteableBitmap(irv.View());
            }
            if (Border2.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View2.Source != null)
            {
                irv2.Colormap();
                img_View2.Source = WriteableBitmapConverter.ToWriteableBitmap(irv2.View());
            }
            if (Border3.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View3.Source != null)
            {
                irv3.Colormap();
                img_View3.Source = WriteableBitmapConverter.ToWriteableBitmap(irv3.View());
            }
            if (Border4.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View4.Source != null)
            {
                irv4.Colormap();
                img_View4.Source = WriteableBitmapConverter.ToWriteableBitmap(irv4.View());
            }
        }
        private void Border_Click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point ClickPos = e.GetPosition((IInputElement)sender);
            int ClickX = (int)ClickPos.X;
            Border Border = sender as Border;
            if (Border.BorderBrush == System.Windows.Media.Brushes.Gray)
            {
                Border.BorderBrush = System.Windows.Media.Brushes.Yellow;
            }
            else
            {
                if (ClickX < Border.ActualWidth / 4)
                {
                    if (Border1.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View1.Source != null)
                    {
                        irv.ImageLeft();
                        irv.image = slt.selctsource[irv.GetX()][irv.GetY()];
                        information1_View1.Content = slt.Viewdata1;
                        information2_View1.Text = slt.Viewdata2[irv.GetX()][irv.GetY()];
                        information3_View1.Content = slt.Viewdata3[irv.GetX()][irv.GetY()];
                        information4_View1.Content = slt.Viewdata4[irv.GetX()][irv.GetY()];
                        img_View1.Source = WriteableBitmapConverter.ToWriteableBitmap(irv.View());
                       
                    }
                    if (Border2.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View2.Source != null)
                    {
                        irv2.ImageLeft();
                        irv2.image = slt.selctsource[irv2.GetX()][irv2.GetY()];
                        information1_View2.Content = slt.Viewdata1;
                        information2_View2.Text = slt.Viewdata2[irv2.GetX()][irv2.GetY()];
                        information3_View2.Content = slt.Viewdata3[irv2.GetX()][irv2.GetY()];
                        information4_View2.Content = slt.Viewdata4[irv2.GetX()][irv2.GetY()];
                        img_View2.Source = WriteableBitmapConverter.ToWriteableBitmap(irv2.View());
                    }
                    if (Border3.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View3.Source != null)
                    {
                        irv3.ImageLeft();
                        irv3.image = slt.selctsource[irv3.GetX()][irv3.GetY()];
                        information1_View3.Content = slt.Viewdata1;
                        information2_View3.Text = slt.Viewdata2[irv3.GetX()][irv3.GetY()];
                        information3_View3.Content = slt.Viewdata3[irv3.GetX()][irv3.GetY()];
                        information4_View3.Content = slt.Viewdata4[irv3.GetX()][irv3.GetY()];
                        img_View3.Source = WriteableBitmapConverter.ToWriteableBitmap(irv3.View());
                    }
                    if (Border4.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View4.Source != null)
                    {
                        irv4.ImageLeft();
                        irv4.image = slt.selctsource[irv4.GetX()][irv4.GetY()];
                        information1_View4.Content = slt.Viewdata1;
                        information2_View4.Text = slt.Viewdata2[irv4.GetX()][irv4.GetY()];
                        information3_View4.Content = slt.Viewdata3[irv4.GetX()][irv4.GetY()];
                        information4_View4.Content = slt.Viewdata4[irv4.GetX()][irv4.GetY()];
                        img_View4.Source = WriteableBitmapConverter.ToWriteableBitmap(irv4.View());
                    }
                }
                else if (ClickX > 3 * Border.ActualWidth / 4)
                {
                    if (Border1.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View1.Source != null)
                    {
                        irv.ImageRight(slt.selctsource);
                        irv.image = slt.selctsource[irv.GetX()][irv.GetY()];
                        information1_View1.Content = slt.Viewdata1;
                        information2_View1.Text = slt.Viewdata2[irv.GetX()][irv.GetY()];
                        information3_View1.Content = slt.Viewdata3[irv.GetX()][irv.GetY()];
                        information4_View1.Content = slt.Viewdata4[irv.GetX()][irv.GetY()];
                        img_View1.Source = WriteableBitmapConverter.ToWriteableBitmap(irv.View());
                    }
                    if (Border2.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View2.Source != null)
                    {
                        irv2.ImageRight(slt.selctsource);
                        irv2.image = slt.selctsource[irv2.GetX()][irv2.GetY()];
                        information2_View2.Text = slt.Viewdata2[irv2.GetX()][irv2.GetY()];
                        information3_View2.Content = slt.Viewdata3[irv2.GetX()][irv2.GetY()];
                        information4_View2.Content = slt.Viewdata4[irv2.GetX()][irv2.GetY()];
                        img_View2.Source = WriteableBitmapConverter.ToWriteableBitmap(irv2.View());
                    }
                    if (Border3.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View3.Source != null)
                    {
                        irv3.ImageRight(slt.selctsource);
                        irv3.image = slt.selctsource[irv3.GetX()][irv3.GetY()];
                        information1_View3.Content = slt.Viewdata1;
                        information2_View3.Text = slt.Viewdata2[irv3.GetX()][irv3.GetY()];
                        information3_View3.Content = slt.Viewdata3[irv3.GetX()][irv3.GetY()];
                        information4_View3.Content = slt.Viewdata4[irv3.GetX()][irv3.GetY()];
                        img_View3.Source = WriteableBitmapConverter.ToWriteableBitmap(irv3.View());
                    }
                    if (Border4.BorderBrush == System.Windows.Media.Brushes.Yellow && img_View4.Source != null)
                    {
                        irv4.ImageRight(slt.selctsource);
                        irv4.image = slt.selctsource[irv4.GetX()][irv4.GetY()];
                        information1_View4.Content = slt.Viewdata1;
                        information2_View4.Text = slt.Viewdata2[irv4.GetX()][irv4.GetY()];
                        information3_View4.Content = slt.Viewdata3[irv4.GetX()][irv4.GetY()];
                        information4_View4.Content = slt.Viewdata4[irv4.GetX()][irv4.GetY()];
                        img_View4.Source = WriteableBitmapConverter.ToWriteableBitmap(irv4.View());
                    }
                }
                else
                {
                    Border.BorderBrush = System.Windows.Media.Brushes.Gray;
                }
            }
        }
    }
    /// <summary>
    /// 비디오리스트
    /// </summary>
    public partial class MainWindow : Window
    {
        private void VideoView_Click(object sender, RoutedEventArgs e)
        {
            if (imgSelect.GetVideo() == 1)
            {
                MessageBox.Show("녹화를 종료하고 진행해 주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                VidoeListLoad();
            }
        }

        private void VidoeListLoad()
        {
            MoveToTab("비디오 뷰어 리스트");
        }

        private void List_VideoReview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (List_VideoReview.SelectedItems.Count != 0)
            {
                DataModel selectedItem = List_VideoReview.SelectedItem as DataModel;
                if (selectedItem != null)
                {
                    string absolutePath = System.IO.Path.GetFullPath(selectedItem.Path);
                    //Thumbnail.Source = new Uri(selectedItem.Path);
                    Thumbnail.Source = new Uri(absolutePath);
                    Thumbnail.SpeedRatio = 2.0; // 재생 속도 두 배로 설정
                    Thumbnail.Play();
                }
            }
            /*if (List_VideoReview.SelectedItems.Count != 0)
            {
                List<int> selectedIndexes = new List<int>();
                int lastClickedIndex = 0;
                var selectedItems = List_VideoReview.SelectedItems;
                if (selectedItems.Count > 0)
                {
                    var lastSelectedItem = selectedItems[selectedItems.Count - 1];
                    lastClickedIndex = List_VideoReview.Items.IndexOf(lastSelectedItem);
                    logdata.WriteLog($"비디오 선택 : {lastClickedIndex}");
                }
                string sql = "SELECT FileName From DB_Video WHERE(HID,Date,Position, Sequences)IN(SELECT DISTINCT HID,Date,Position, Sequences FROM DB_Video where HID = '" + slt.SelectHID + "'ORDER BY Date DESC,CAST(Sequences AS INT) LIMIT 1 OFFSET " + lastClickedIndex + ")";
                SQLiteDataReader rdr = db.Load(sql);
                List<string> path = new List<string>();
                List<BitmapImage> source = new List<BitmapImage>();
                string dicomFilePath = "";
                string filename = "";
                while (rdr.Read())
                {
                    filename = rdr["FileName"].ToString();
                }
                rdr.Close();
                dicomFilePath = @"DicomVideo" + @"\" + filename + ".dcm";
                var frames = ReadDicomFrames(dicomFilePath);
                VideoImage.Source = ConvertBitmapToBitmapImage(frames[0]);
                
            }*/
        }
        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            Thumbnail.Position = TimeSpan.Zero;
            Thumbnail.SpeedRatio = 2.0;
            Thumbnail.Play();
        }
        private void VideoExport_Click(object sender, RoutedEventArgs e)
        {
            if (List_VideoReview.SelectedItems.Count != 0)
            {
                List<int> selectedIndexes = new List<int>();
                int lastClickedIndex = 0;
                var selectedItems = List_VideoReview.SelectedItems;
                if (selectedItems.Count > 0)
                {
                    var lastSelectedItem = selectedItems[selectedItems.Count - 1];
                    lastClickedIndex = List_VideoReview.Items.IndexOf(lastSelectedItem);
                    logdata.WriteLog($"비디오 선택 : {lastClickedIndex}");
                }
                string sql = "SELECT FileName From DB_Video WHERE(HID,Date,Position, Sequences)IN(SELECT DISTINCT HID,Date,Position, Sequences FROM DB_Video where HID = '" + slt.SelectHID + "'ORDER BY Date DESC,CAST(Sequences AS INT) LIMIT 1 OFFSET " + lastClickedIndex + ")";
                SQLiteDataReader rdr = db.Load(sql);
                string dicomFilePath = "";
                string outputAviPath = "";
                string filename = "";
                while (rdr.Read())
                {
                    filename = rdr["FileName"].ToString();
                }
                dicomFilePath = @"DicomVideo" + @"\" + filename + ".dcm";
                outputAviPath = @"DicomVideo" + @"\" + filename + ".avi";

                ConvertToAviFile(dicomFilePath, outputAviPath);
                rdr.Close();
            }
        }
        public void ConvertToAviFile(string dicomFilePath, string outputAviPath)
        {
            //RegisterFFmpegBinaries();
            // DICOM 파일에서 프레임 읽기
            var frames = ReadDicomFrames(dicomFilePath);

            OpenCvSharp.VideoWriter OpenCV_video = new OpenCvSharp.VideoWriter(outputAviPath, "MJPG", 5, new OpenCvSharp.Size(frames[0].Width, frames[0].Height));
            for(int i = 0; i < frames.Count(); i++)
            {
                var mat = BitmapConverter.ToMat(frames[i]);
                OpenCV_video.Write(mat);
            }
            OpenCV_video.Release();
        }


        public List<System.Drawing.Bitmap> ReadDicomFrames(string dicomFilePath)
        {
            new DicomSetupBuilder().RegisterServices(s => s.AddFellowOakDicom().AddImageManager<WinFormsImageManager>()).Build();
            var dicomFile = DicomFile.Open(dicomFilePath);
            var dicomImage = new DicomImage(dicomFile.Dataset);
            List<System.Drawing.Bitmap> frames = new List<System.Drawing.Bitmap>();
            for (int i = 0; i < dicomImage.NumberOfFrames; i++)
            {
                using (var frame = dicomImage.RenderImage(i))
                {
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(dicomImage.RenderImage(i).As<System.Drawing.Bitmap>());
                    frames.Add(bitmap);
                }
            }
            return frames;
        }
        private async void VideoSendToPacs_Click(object sender, RoutedEventArgs e)
        {
            if(List_VideoReview.SelectedItems.Count != 0)
            {
                DataModel selectedItem = List_VideoReview.SelectedItem as DataModel;
                if (selectedItem != null && selectedItem.Dicom == "O")
                {
                    logdata.WriteLog("VideoPacs 전송 성공");
                    string dicomFolderPath = @"DicomVideo";
                    string filePath = System.IO.Path.Combine(dicomFolderPath, selectedItem.FileName.Replace("avi", "dcm"));
                    try
                    {
                        await SendToPacs(filePath, "111222333", IP.Text, int.Parse(Port.Text), AET.Text);
                        MessageBox.Show("전송 완료", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        logdata.WriteLog("Pacs 전송 완료");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"전송 중 오류 발생: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        logdata.WriteLog($"Pacs 전송 실패 : {ex.Message}");
                    }
                }
                else
                {
                    logdata.WriteLog("VideoPacs 전송 실패");
                    MessageBox.Show("전송가능한 파일을 선택하세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                logdata.WriteLog("VideoPacs 전송 실패");
                MessageBox.Show("파일을 선택하세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
           
            /*if (List_VideoReview.SelectedItems.Count != 0)
            {
                List<int> selectedIndexes = new List<int>();
                int lastClickedIndex = 0;
                var selectedItems = List_VideoReview.SelectedItems;
                if (selectedItems.Count > 0)
                {
                    var lastSelectedItem = selectedItems[selectedItems.Count - 1];
                    lastClickedIndex = List_VideoReview.Items.IndexOf(lastSelectedItem);
                    logdata.WriteLog($"비디오 선택 : {lastClickedIndex}");
                }
                string sql = "SELECT FileName From DB_Video WHERE(HID,Date,Position, Sequences)IN(SELECT DISTINCT HID,Date,Position, Sequences FROM DB_Video where HID = '" + slt.SelectHID + "'ORDER BY Date DESC,CAST(Sequences AS INT) LIMIT 1 OFFSET " + lastClickedIndex + ")";
                SQLiteDataReader rdr = db.Load(sql);
                string filename = "";
                while (rdr.Read())
                {
                    filename = @"DicomVideo" + @"\" + rdr["FileName"].ToString() + ".dcm";
                }
                rdr.Close();
                try
                {
                    await SendToPacs(filename, "111222333", IP.Text, int.Parse(Port.Text), AET.Text);
                    MessageBox.Show("전송 완료", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    logdata.WriteLog("Pacs 전송 완료");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"전송 중 오류 발생: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logdata.WriteLog($"Pacs 전송 실패 : {ex.Message}");
                }
            }*/
            /*if (List_ImageReview.SelectedItems.Count != 0)
            {
                var selectedItems = List_ImageReview.SelectedItems;
                List<string> selectedIndexes = new List<string>();
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    var select = selectedItems[i];
                    selectedIndexes.Add(List_ImageReview.Items.IndexOf(select).ToString());
                }
                selectedIndexes.Sort();
                for (int i = 0; i < selectedIndexes.Count(); i++)
                {
                    string sql = "SELECT FileName,Date,Position,Sequences,Mode From DB_ShotSave WHERE(Date,Position, Sequences, Mode)IN(SELECT DISTINCT Date,Position, Sequences, Mode FROM DB_ShotSave where HID = '" + slt.SelectHID + "'ORDER BY Date DESC, CAST(Sequences AS INT) LIMIT 1 OFFSET " + selectedIndexes[i] + ")";
                    SQLiteDataReader rdr = db.Load(sql);
                    List<string> path = new List<string>();
                    while (rdr.Read())
                    {
                        path.Add(@"dicom" + @"\" + rdr["FileName"].ToString() + ".dcm");
                    }
                    rdr.Close();
                    int successfulTransfers = 0;
                    for (int j = 0; j < path.Count(); j++)
                    {
                        //SendToPACS(path, "111222333", "200.200.1.16", 9592, "MAROTECH");
                        try
                        {
                            await SendToPacs(path[j], "111222333", IP.Text, int.Parse(Port.Text), AET.Text);
                            PacsDataSavebtn.IsEnabled = true;
                            successfulTransfers++;
                            if (successfulTransfers == path.Count())
                            {
                                MessageBox.Show("전송 완료", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                logdata.WriteLog("Pacs 전송 완료");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"전송 중 오류 발생: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            logdata.WriteLog($"Pacs 전송 실패 : {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("이미지리스트를 선택 해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }*/
        }
    }
    
    /// <summary>
    /// 비디오리뷰
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<PacketPoint> packetPoints = new List<PacketPoint>();
        List<PacketSpeed> List_Speed = new List<PacketSpeed>();
        
        private void VideoLoad()
        {
            //tabControl.SelectedIndex = 6;
            MoveToTab("비디오 뷰어");
            logdata.WriteLog("비디오 리뷰 로드");
            Video_View.Source = null;
        }
        private void Video_Review_Click(object sender, RoutedEventArgs e)
        {
            VideoLoad();
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            // 실행 파일(.exe)의 전체 경로 얻기
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            // 실행 파일이 있는 디렉토리 경로 얻기
            string exeDirectory = System.IO.Path.GetDirectoryName(exePath);

            // 'video' 폴더의 경로 만들기
            string videoDirectory = System.IO.Path.Combine(exeDirectory, "video");
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Title = "미디어소스",
                DefaultExt = ".avi",
                Filter = "미디어 파일|*.avi;*.wmv;*.mp4",
                Multiselect = false,
                InitialDirectory = videoDirectory
            };
            if (dlg.ShowDialog() == true)
            {
                Video_View.Source = new Uri(dlg.FileName);
                Video_View.SpeedRatio = vp.GetSpeed();
                DispatcherTimer timer3 = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromSeconds(0.2)
                };
                timer3.Tick += TimerTickHandler;
                timer3.Start();
                Video_View.Play();
                vp.Play(1);
                Video_Start.Content = "❚❚";
                logdata.WriteLog("비디오 오픈");
            }

        }
        private void Video_View_MediaOpened(object sender, RoutedEventArgs e)
        {
            PlayTime.Minimum = 0;
            PlayTime.Maximum = Video_View.NaturalDuration.TimeSpan.TotalSeconds;
            vp.Naturalwidth = Video_View.NaturalVideoWidth;
            vp.Naturalheight = Video_View.NaturalVideoHeight;
            double speedRatio = vp.SpeedInit();
            Video_View.SpeedRatio = speedRatio;
            Quick.Content = speedRatio;
        }
        private void Video_View_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Console.WriteLine("실패");
        }
        private void Video_View_MediaEnded(object sender, RoutedEventArgs e)
        {
            Video_View.Position = TimeSpan.FromMilliseconds(1);
            Video_Start.Content = "▶";
            vp.Play(0);
            double speedRatio = vp.SpeedInit();
            Video_View.SpeedRatio = speedRatio;
            Quick.Content = speedRatio;
            vp.sldrDragStart = false;
            Video_View.Pause();
        }
        private void Marker_Click(object sender, RoutedEventArgs e)
        {
            if (vp.MarkerVal() == 1)
            {
                logdata.WriteLog("마커 모드 온");
                Marker.Foreground = Brushes.Red;
            }
            else
            {
                logdata.WriteLog("마커 모드 오프");
                Marker.Foreground = Brushes.White;
            }
        }
        private void Video_View_Click(object sender, MouseButtonEventArgs e)
        {
            if (vp.GetMarker() == 1)
            {
                logdata.WriteLog("속도 마커 찍기");
                int x = (int)(e.GetPosition((MediaElement)sender).X * vp.GetWidth() / Video_View.ActualWidth);
                int y = (int)(e.GetPosition((MediaElement)sender).Y * vp.GetHight() / Video_View.ActualHeight);
                OpenCvSharp.Mat image = vp.GetRender(Video_View, 96);
                //Cv2.Circle(image, new OpenCvSharp.Point(x, y), 5, Scalar.Red, -1);
                Video_Canvas.Children.Clear();
                Ellipse myEllipse = new Ellipse();
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Color.FromArgb(255, 255, 0, 0);
                myEllipse.Fill = mySolidColorBrush;
                myEllipse.StrokeThickness = 2;
                myEllipse.Stroke = Brushes.Blue;
                myEllipse.Width = 10;
                myEllipse.Height = 10;
                Canvas.SetLeft(myEllipse, e.GetPosition((MediaElement)sender).X + ((Video_Canvas.ActualWidth - Video_View.ActualWidth) / 2) - 5);
                Canvas.SetTop(myEllipse, e.GetPosition((MediaElement)sender).Y + ((Video_Canvas.ActualHeight - Video_View.ActualHeight) / 2) - 5);
                Video_Canvas.Children.Add(myEllipse);

                int idx = packetPoints.FindIndex(obj => Math.Round(obj.videotimecount.TotalSeconds, 1) == Math.Round(Video_View.Position.TotalSeconds, 1));

                if (idx == -1)
                {
                    packetPoints.Add(new PacketPoint(Video_View.Position, x, y));
                }
                else
                {

                    packetPoints[idx].SetPoint(x, y);
                }
                packetPoints.Sort((videotimecountA, videotimecountB) => videotimecountA.videotimecount.CompareTo(videotimecountB.videotimecount));
                CalculatePacketPoints();
                DrawPacketData(image);

            }
        }
        private void CalculatePacketPoints()
        {
            // Packet Speed (mm/s)
            for (int i = 0; i < packetPoints.Count(); i++)
            {
                if (i == 0)
                {
                    PacketPoint newPoint = packetPoints[0];
                    TimeSpan nowtime = newPoint.videotimecount;
                    packetPoints[0].nowtime = nowtime.ToString(@"hh\:mm\:ss");
                }
                else
                {
                    PacketPoint newPoint = packetPoints[i];
                    PacketPoint prevPoint = packetPoints[i - 1];
                    double time = (newPoint.videotimecount - prevPoint.videotimecount).TotalMilliseconds / 1000.0; // Sec
                    float pWidth = newPoint.point.X - prevPoint.point.X;
                    float pHeight = newPoint.point.Y - prevPoint.point.Y;
                    double dist = Math.Sqrt(Math.Pow(pWidth, 2) + Math.Pow(pHeight, 2)) * 0.439453125;            //0.390625 mm
                    double speed = dist / time;
                    speed = Math.Round(speed, 2);
                    packetPoints[i].time = time;
                    packetPoints[i].range = dist;
                    packetPoints[i].speed = speed;

                    TimeSpan nowtime = newPoint.videotimecount;
                    double totaldist = 0;
                    double totaltime = (newPoint.videotimecount - packetPoints[0].videotimecount).TotalMilliseconds / 1000.0;

                    for (int j = 0; j < i + 1; j++)
                    {
                        totaldist += packetPoints[j].range;
                    }
                    double totalspeed = totaldist / totaltime;
                    totalspeed = Math.Round(totalspeed, 2);
                    packetPoints[i].totaldist = totaldist;
                    packetPoints[i].totaltime = totaltime;
                    packetPoints[i].totalspeed = totalspeed;
                    packetPoints[i].nowtime = nowtime.ToString(@"hh\:mm\:ss");
                }
            }
            List_Speed.Clear();
            for (int i = 0; i < packetPoints.Count(); i++)
            {
                List_Speed.Add(new PacketSpeed()
                {
                    Num = (i + 1).ToString(),
                    Time = packetPoints[i].nowtime.ToString(),
                    Section_Time = Math.Round(packetPoints[i].time, 2).ToString() + "s",
                    Section_Dist = Math.Round(packetPoints[i].range, 2).ToString() + "mm",
                    Section_Speed = packetPoints[i].speed.ToString() + " mm/s",
                    total_Time = Math.Round(packetPoints[i].totaltime, 2).ToString() + "s",
                    total_Dist = Math.Round(packetPoints[i].totaldist, 2).ToString() + "mm",
                    total_Speed = packetPoints[i].totalspeed.ToString() + " mm/s",
                });
                List_PackagesSpeed.ItemsSource = List_Speed;
            }
            List_PackagesSpeed.Items.Refresh();
        }
        private void DrawPacketData(OpenCvSharp.Mat image)
        {
            if (packetPoints.Count == 0)
            {
                return;
            }

            OpenCvSharp.Mat res = new OpenCvSharp.Mat();
            image.CopyTo(res);
            for (int i = 0; i < packetPoints.Count; i++)
            {
                OpenCvSharp.Cv2.Circle(res, packetPoints[i].point, 5, OpenCvSharp.Scalar.Red, -1);

                OpenCvSharp.Cv2.PutText(res, string.Format("{0}", i + 1),
                            new OpenCvSharp.Point(packetPoints[i].point.X - 35, packetPoints[i].point.Y - 20),
                            OpenCvSharp.HersheyFonts.Italic, 2, OpenCvSharp.Scalar.Red, 2, OpenCvSharp.LineTypes.AntiAlias);
            }
            MarkerImage.Source = WriteableBitmapConverter.ToWriteableBitmap(res);
        }
        private void Eraser_Click(object sender, RoutedEventArgs e)
        {
            if (List_PackagesSpeed.SelectedItem != null)
            {
                logdata.WriteLog("속도 측정 데이터 지우기");
                OpenCvSharp.Mat image = vp.GetRender(Video_View, 96);
                Video_Canvas.Children.Clear();
                packetPoints.RemoveAt(List_PackagesSpeed.SelectedIndex);
                packetPoints.Sort((videotimecountA, videotimecountB) => videotimecountA.videotimecount.CompareTo(videotimecountB.videotimecount));
                CalculatePacketPoints();
                DrawPacketData(image);
            }
        }
        private void AllEraser_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("속도 측정 데이터 전체지우기");
            packetPoints.Clear();
            List_Speed.Clear();
            List_PackagesSpeed.Items.Refresh();
            Video_Canvas.Children.Clear();
            MarkerImage.Source = null;
        }
        private void List_PackagesSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (List_PackagesSpeed.SelectedIndex != -1)
            {
                try
                {
                    // 시간 문자열 파싱
                    string time = packetPoints[List_PackagesSpeed.SelectedIndex].nowtime;
                    var timeParts = time.Split(':');

                    if (timeParts.Length == 3)
                    {
                        TimeSpan ts = new TimeSpan(int.Parse(timeParts[0]),    // hours
                                                   int.Parse(timeParts[1]),    // minutes
                                                   int.Parse(timeParts[2]));   // seconds

                        Video_Canvas.Children.Clear();

                        // 비동기적 대기를 위한 Task 실행
                        Task.Run(() =>
                        {
                            // UI 스레드에서 실행
                            Dispatcher.Invoke(() =>
                            {
                                // 필요한 경우만 Play 호출
                                if (Video_View.CanPause)
                                {
                                    Video_View.Play();
                                }

                                //Video_View.Position = TimeSpan.FromSeconds(ts.TotalSeconds);
                                Video_View.Position = packetPoints[List_PackagesSpeed.SelectedIndex].videotimecount;
                                Console.WriteLine($"Video_View.Position : {Video_View.Position}");
                                // 위치 변경 후 잠시 대기
                                //Task.Delay(100).Wait();  // 100ms 대기

                                Video_View.Pause();
                            });
                        });
                    }
                }
                catch (Exception ex)
                {
                    // 예외 처리: 오류 로깅, 사용자에게 알림 등
                    Console.WriteLine("Error changing video position: " + ex.Message);
                }
            }
        }
        private void Video_Start_Click(object sender, RoutedEventArgs e)
        {
            if (vp.PlayVal() == 1)
            {
                Video_View.SpeedRatio = vp.GetSpeed();
                Video_View.Play();
                Video_Start.Content = "❚❚";
                logdata.WriteLog("비디오 재생");
            }
            else
            {
                Video_View.SpeedRatio = 1;
                Video_View.Pause();
                Video_Start.Content = "▶";
                logdata.WriteLog("비디오 정지");
            }

        }
        private void SpeedUp_Click(object sender, RoutedEventArgs e)
        {
            double speedRatio = vp.SpeedUp();
            Quick.Content = speedRatio;
            Video_View.SpeedRatio = speedRatio;
            logdata.WriteLog($"비디오 배속 Up : {Video_View.SpeedRatio}");
        }
        private void SpeedDown_Click(object sender, RoutedEventArgs e)
        {
            double speedRatio = vp.SpeedDown();
            Quick.Content = speedRatio;
            Video_View.SpeedRatio = speedRatio;
            logdata.WriteLog($"비디오 배속 Down : {Video_View.SpeedRatio}");
        }
        private void PositionUp_Click(object sender, RoutedEventArgs e)
        {
            if (Video_View.Source != null)
            {
                if (Video_View.NaturalDuration.TimeSpan > Video_View.Position + TimeSpan.FromSeconds(1))
                {
                    Video_View.Position = Video_View.Position + TimeSpan.FromSeconds(1);
                    logdata.WriteLog("비디오 1초 앞으로");
                }
            }
        }
        private void Positiondown_Click(object sender, RoutedEventArgs e)
        {
            if (Video_View.Source != null)
            {
                Video_View.Position = Video_View.Position - TimeSpan.FromSeconds(1);
                logdata.WriteLog("비디오 1초 뒤로");
            }
        }
        private void PlayTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Video_View.Source == null)
            {
                return;
            }
            //Video_View.Position = TimeSpan.FromSeconds(PlayTime.Value);
            PlayTime_Label.Content = String.Format("{0} / {1}", Video_View.Position.ToString(@"hh\:mm\:ss"), Video_View.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss"));
        }
        private void PlayTime_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            vp.sldrDragStart = true;
            Video_View.Play();
            timer2.Start();
        }
        private void PlayTime_DrageCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Video_View.Position = TimeSpan.FromSeconds(PlayTime.Value);
            if (vp.GetPlay() == 1)
            {
                Video_View.Play();
                Video_Start.Content = "❚❚";
            }
            else
            {
                Video_View.Pause();
            }
            vp.sldrDragStart = false;
            timer2.Stop();
        }
        void TimerTickHandler(object sender, EventArgs e)
        {
            if (vp.sldrDragStart)
            {
                return;
            }
            if (Video_View.Source == null || !Video_View.NaturalDuration.HasTimeSpan)
            {
                PlayTime_Label.Content = "No file selected...";
                return;
            }

            PlayTime.Value = Video_View.Position.TotalSeconds;
        }
        private void TimerTickHandler2(object sender, EventArgs e)
        {
            if (Video_View.Source == null)
            {
                return;
            }
            Video_View.Position = TimeSpan.FromSeconds(PlayTime.Value);
        }
        private void VideoDataSave_Click(object sender, RoutedEventArgs e)
        {
            VideoRenderWindow videorender_window = new VideoRenderWindow(MarkerImage, List_PackagesSpeed);
            videorender_window.ShowDialog();
        }
    }

    /// <summary>
    /// 레포트이미지처리
    /// </summary>
    public partial class MainWindow : Window
    {
        ReportImage rpt;
        private void ReportSetLoad()
        {
            logdata.WriteLog("리포트 셋 로드");
            MoveToTab("리포트 이미지 편집");
            //tabControl.SelectedIndex = 7;
            FuntionColor("");
            ReportA.Source = null;
            ReportP.Source = null;
            ReportM.Source = null;
            ReportL.Source = null;
            string sql = "SELECT FileName, Date, Position,Shot_Time,Sequences From DB_ShotSave WHERE(HID,Date, Position, Sequences, Mode)IN(SELECT DISTINCT HID,Date, Position, Sequences, Mode FROM DB_ShotSave where HID = '" + slt.SelectHID + "'ORDER BY Date DESC,CAST(Sequences AS INT) LIMIT 1 OFFSET " + List_ImageReview.SelectedIndex + ")";
            sql += "AND (ImageNum = (SELECT count(*) From DB_ShotSave WHERE(HID,Date, Position, Sequences, Mode)IN(SELECT DISTINCT HID,Date, Position, Sequences, Mode FROM DB_ShotSave where HID = '" + slt.SelectHID + "'ORDER BY Date DESC,CAST(Sequences AS INT) LIMIT 1 OFFSET " + List_ImageReview.SelectedIndex + "))/4*0";
            sql += " OR ImageNum = (SELECT count(*) From DB_ShotSave WHERE(HID,Date, Position, Sequences, Mode)IN(SELECT DISTINCT HID,Date, Position, Sequences, Mode FROM DB_ShotSave where HID = '" + slt.SelectHID + "'ORDER BY Date DESC,CAST(Sequences AS INT) LIMIT 1 OFFSET " + List_ImageReview.SelectedIndex + "))/4*1";
            sql += " OR ImageNum = (SELECT count(*) From DB_ShotSave WHERE(HID,Date, Position, Sequences, Mode)IN(SELECT DISTINCT HID,Date, Position, Sequences, Mode FROM DB_ShotSave where HID = '" + slt.SelectHID + "'ORDER BY Date DESC,CAST(Sequences AS INT) LIMIT 1 OFFSET " + List_ImageReview.SelectedIndex + "))/4*2";
            sql += " OR ImageNum  = (SELECT count(*) From DB_ShotSave WHERE(HID,Date, Position, Sequences, Mode)IN(SELECT DISTINCT HID,Date, Position, Sequences, Mode FROM DB_ShotSave where HID = '" + slt.SelectHID + "'ORDER BY Date DESC,CAST(Sequences AS INT) LIMIT 1 OFFSET " + List_ImageReview.SelectedIndex + "))/4*3)";
            Console.WriteLine(sql);
            SQLiteDataReader rdr = db.Load(sql);
            List<string> path = new List<string>();
            while (rdr.Read())
            {
                path.Add(@"dicom" + @"\" + rdr["FileName"].ToString() + ".dcm");
                rptslt.ReportSelectDate = rdr["Date"].ToString();
                rptslt.ReportSelectPosition = rdr["Position"].ToString();
                rptslt.ReportSelectTime = rdr["Shot_Time"].ToString();
                rptslt.ReportSelectSequences = rdr["Sequences"].ToString();
            }
            rdr.Close();
            for (int i = 0; i < path.Count(); i++)
            {
                new DicomSetupBuilder().RegisterServices(s => s.AddFellowOakDicom().AddImageManager<WinFormsImageManager>()).Build();
                var m_pDicomFile = DicomFile.Open(path[i]);
                var dicomImage = new DicomImage(m_pDicomFile.Dataset);
                System.Drawing.Bitmap bitmap = dicomImage.RenderImage().As<System.Drawing.Bitmap>();
                rpt.Add(bitmap);
                //Count[j] = ConvertBitmapToBitmapImage(bitmap);
            }
            ReportImageView(true);
        }
        private void ReportImageView(bool mode)
        {
            if (mode)
            {
                ReportA.Source = WriteableBitmapConverter.ToWriteableBitmap(rpt.Getimg()[0]);
                ReportP.Source = WriteableBitmapConverter.ToWriteableBitmap(rpt.Getimg()[2]);
                ReportM.Source = WriteableBitmapConverter.ToWriteableBitmap(rpt.Getimg()[1]);
                ReportL.Source = WriteableBitmapConverter.ToWriteableBitmap(rpt.Getimg()[3]);
            }
            else
            {
                ReportA.Source = WriteableBitmapConverter.ToWriteableBitmap(rpt.Getdst()[0]);
                ReportP.Source = WriteableBitmapConverter.ToWriteableBitmap(rpt.Getdst()[2]);
                ReportM.Source = WriteableBitmapConverter.ToWriteableBitmap(rpt.Getdst()[1]);
                ReportL.Source = WriteableBitmapConverter.ToWriteableBitmap(rpt.Getdst()[3]);
            }
        }
        private void Report_Line_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("Line On");
            string fun = rpt.Function("Line");
            if (!FuntionColor(fun))
            {
                ReportImageView(true);
            }
            else
            {
                ReportImageView(false);
            }
        }
        private void Report_Extraction_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("Extraction On");
            string fun = rpt.Function("Extraction");
            if (FuntionColor(fun))
            {
                rpt.ExtractionPointSet(rpt.Getpoint(), Report_ICG_Val.Value);
                ReportImageView(false);
            }
            else
            {
                ReportImageView(true);
            }
        }
        private void Report_Eraser_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("Eraser On");
            string fun = rpt.Function("Eraser");
            if (FuntionColor(fun))
            {
                rpt.ExtractionPointSet(rpt.Getpoint(), Report_ICG_Val.Value);
                ReportImageView(false);
            }
            else
            {
                ReportImageView(true);
            }
        }
        private void Report_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (rpt.Getfun() == "Line")
            {
                int x = (int)(e.GetPosition((Image)sender).X * rpt.GetWidth() / image.ActualWidth);
                rpt.LineXlistSet(x);
                ReportImageView(false);

            }
            else if (rpt.Getfun() == "Extraction")
            {
                int x = (int)(e.GetPosition((Image)sender).X * rpt.GetWidth() / image.ActualWidth);
                int y = (int)(e.GetPosition((Image)sender).Y * rpt.GetHeight() / image.ActualHeight);
                rpt.ExtractionPointSet(new OpenCvSharp.Point(x, y), Report_ICG_Val.Value);
                ReportImageView(false);
            }
            else if (rpt.Getfun() == "Eraser")
            {
                int x = (int)(e.GetPosition((Image)sender).X * rpt.GetWidth() / image.ActualWidth);
                int y = (int)(e.GetPosition((Image)sender).Y * rpt.GetHeight() / image.ActualHeight);
                rpt.EraserPointSet(Convert.ToInt32(image.Tag), new OpenCvSharp.Point(x, y), Report_ICG_Val.Value);
                ReportImageView(false);
            }
        }
        private void Report_Save_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("Report Save");
            rpt.Save(Report_ICG_Val.Value);
            ReportImageView(true);
            FuntionColor("");
        }
        private void Report_ICG_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rpt.ExtractionPointSet(rpt.Getpoint(), Report_ICG_Val.Value);
            ReportImageView(false);
        }
        private bool FuntionColor(string fun)
        {
            bool Check = true;
            if (fun == "Line")
            {
                Report_Line_btn.Foreground = System.Windows.Media.Brushes.Red;
                Report_Extraction_btn.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                Report_Eraser_btn.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            }
            else if (fun == "Extraction")
            {
                Report_Line_btn.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                Report_Extraction_btn.Foreground = System.Windows.Media.Brushes.Red;
                Report_Eraser_btn.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            }
            else if (fun == "Eraser")
            {
                Report_Line_btn.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                Report_Extraction_btn.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                Report_Eraser_btn.Foreground = System.Windows.Media.Brushes.Red;
            }
            else
            {
                Report_Line_btn.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                Report_Extraction_btn.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                Report_Eraser_btn.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                Check = false;
            }
            return Check;
        }
        private void Report_Click(object sender, RoutedEventArgs e)
        {
            
            if(rpt.FunctionGet()=="")
            {
                logdata.WriteLog("Report 작성 이동");
                if (rpt.GetLineCount() == 3)
                {
                    //tabControl.SelectedIndex = 8;
                    MoveToTab("리포트 작성");
                    Report_Control_Init();
                    Report_Page1Data();
                    ReportDateInit();
                }
                else
                {
                    MessageBox.Show("라인기능을 완료 해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show($"{rpt.FunctionGet()}기능을 완료 해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }
        
    }

    /// <summary>
    /// 리포트
    /// </summary>
    public partial class MainWindow : Window
    {
        List<ReportDateList> ReportDateList = new List<ReportDateList>();
        int page = 0;
        int num = 0;
        private void Report_Control_Init()
        {
            logdata.WriteLog("리포트 Init");
            chartlist = new ChartList(this);
            //Page1
            //환자정보창 초기화
            Report_Name.Content = null;
            Report_Birthday.Content = null;
            Report_Shotdate.Content = null;
            MedicalStaff.Text = null;
            Report_HID.Content = null;
            Report_Sex.Content = null;
            Report_Position.Content = null;
            diagnosis.Text = null;

            //패턴입력 콤보박스 초기화
            A_Hand_Pattern.Text = null;
            A_Lower_Pattern.Text = null;
            A_Upper_Pattern.Text = null;
            P_Lower_Pattern.Text = null;
            P_Upper_Pattern.Text = null;
            M_Lower_Pattern.Text = null;
            M_Upper_Pattern.Text = null;
            L_Lower_Pattern.Text = null;
            L_Upper_Pattern.Text = null;

            //ROI 텍스트박스 초기화
            A_Hand_ROI.Content = null;
            A_Lower_ROI.Content = null;
            A_Upper_ROI.Content = null;
            P_Lower_ROI.Content = null;
            P_Upper_ROI.Content = null;
            M_Lower_ROI.Content = null;
            M_Upper_ROI.Content = null;
            L_Lower_ROI.Content = null;
            L_Upper_ROI.Content = null;

            //이미지 초기화
            ReportA_View.Source = null;
            ReportP_View.Source = null;
            ReportM_View.Source = null;
            ReportL_View.Source = null;

            //Page2
            //이미지 초기화
            Page2_ReportA_View.Source = null;
            Page2_ReportP_View.Source = null;
            Page2_ReportM_View.Source = null;
            Page2_ReportL_View.Source = null;

            //SubDatatable 초기화
            strattime.Content = null;
            datatime.Content = null;
            timetaken.Content = null;

            //Datatable 초기화
            PatternA.Content = null;
            PatternLA.Content = null;
            PatternUA.Content = null;
            PatternLP.Content = null;
            PatternUP.Content = null;
            PatternLM.Content = null;
            PatternUM.Content = null;
            PatternLL.Content = null;
            PatternUL.Content = null;

            VelocityA.Text = null;
            VelocityLA.Text = null;
            VelocityUA.Text = null;
            VelocityLP.Text = null;
            VelocityUP.Text = null;
            VelocityLM.Text = null;
            VelocityUM.Text = null;
            VelocityLL.Text = null;
            VelocityUL.Text = null;

            RoiA.Content = null;
            RoiLA.Content = null;
            RoiUA.Content = null;
            RoiLP.Content = null;
            RoiUP.Content = null;
            RoiLM.Content = null;
            RoiUM.Content = null;
            RoiLL.Content = null;
            RoiUL.Content = null;

            //Page3
            //이미지 초기화
            CommentA_View.Source = null;
            CommentP_View.Source = null;
            CommentM_View.Source = null;
            CommentL_View.Source = null;

            //Comment 초기화
            Anterior.Text = null;
            Posterior.Text = null;
            Medial.Text = null;
            Lateral.Text = null;
            Summary.Text = null;
            page = 0;
            num = 0;

            ChartList.Visibility = Visibility.Hidden;
            chart.Visibility = Visibility.Visible;
            ReportLoadImage.Visibility = Visibility.Hidden;
            Report_Page.Visibility = Visibility.Visible;
            Report_Page.SelectedIndex = 0;
            Report_Page1Data();
            Report_Page2Data();
            Report_Page3Data();
        }
        private void ReportDateInit()
        {
            string sql = "SELECT DISTINCT Date,Position FROM DB_Report where HID ='" + slt.SelectHID + "'ORDER BY Date DESC";
            SQLiteDataReader rdr = db.Load(sql);
            List<string> dateposition = new List<string>();
            while (rdr.Read())
            {
                string date = rdr["Date"].ToString();
                string position = rdr["Position"].ToString();
                string str = $"{date}({position})";
                dateposition.Add(str);
            }
            rdr.Close();
            sql = "SELECT FileName FROM DB_Report where HID ='" + slt.SelectHID + "'ORDER BY Date DESC";
            rdr = db.Load(sql);
            List<string> path = new List<string>();
            while (rdr.Read())
            {
                path.Add(@"Report" + @"\" + rdr["FileName"].ToString() + ".dcm");
            }
            rdr.Close();
            ReportDateList.Clear();
            for (int i = 0; i < dateposition.Count; i++)
            {
                List<BitmapImage> bitmapimage = new List<BitmapImage>();
                for (int j = 0; j < 3; j++)
                {
                    new DicomSetupBuilder().RegisterServices(s => s.AddFellowOakDicom().AddImageManager<WinFormsImageManager>()).Build();
                    var m_pDicomFile = DicomFile.Open(path[3 * i + j]);
                    var dicomImage = new DicomImage(m_pDicomFile.Dataset);
                    System.Drawing.Bitmap bitmap = dicomImage.RenderImage().As<System.Drawing.Bitmap>();
                    bitmapimage.Add(ConvertBitmapToBitmapImage(bitmap));
                }
                ReportDateList.Add(new ReportDateList(dateposition[i], bitmapimage));
            }
            ReportDateList.Sort((x, y) => y.date.CompareTo(x.date));
            //ReportDateList.Reverse();
            ContentControl[] reportDayLabels = new ContentControl[]
            {
                Report_day1,
                Report_day2,
                Report_day3,
                Report_day4,
                Report_day5,
                Report_day6,
                Report_day7,
            };

            for (int i = 0; i < ReportDateList.Count; i++)
            {
                reportDayLabels[i].Content = ReportDateList[i].date;
            }
            for (int i = ReportDateList.Count; i < 7; i++)
            {
                reportDayLabels[i].Visibility = Visibility.Hidden;
            }
        }
        private void Pageset(int page)
        {
            if (page == 0)
            {
                Report_Page1Data();
            }
            else if (page == 1)
            {
                Report_Page2Data();
            }
            else if (page == 2)
            {
                Report_Page3Data();
            }
        }
        //페이지 로드
        private void Report_Page1Data()
        {
            logdata.WriteLog("1페이지 로드");
            Report_Name.Content = slt.SelectName;
            Report_HID.Content = slt.SelectHID;
            Report_Birthday.Content = slt.SelectBirthday;
            Report_Sex.Content = slt.SelectSex;
            Report_Shotdate.Content = rptslt.ReportSelectDate;
            Report_Position.Content = rptslt.ReportSelectPosition;
            List<OpenCvSharp.Mat> ReportImage = rpt.ReportImageLine();
            ReportA_View.Source = WriteableBitmapConverter.ToWriteableBitmap(ReportImage[0]);
            ReportP_View.Source = WriteableBitmapConverter.ToWriteableBitmap(ReportImage[2]);
            ReportM_View.Source = WriteableBitmapConverter.ToWriteableBitmap(ReportImage[1]);
            ReportL_View.Source = WriteableBitmapConverter.ToWriteableBitmap(ReportImage[3]);
            HandOrFoot.Content = rptslt.ReportSelectPosition == "LA"|| rptslt.ReportSelectPosition == "RA" ? "Hand" : "Foot";
            List<double> ReportRoI = rpt.ROIVal(rpt.Getimg()[0], 0);
            A_Hand_ROI.Content = ReportRoI[0];
            A_Lower_ROI.Content = ReportRoI[1];
            A_Upper_ROI.Content = ReportRoI[2];
            ReportRoI = rpt.ROIVal(rpt.Getimg()[1], 1);
            P_Lower_ROI.Content = ReportRoI[0];
            P_Upper_ROI.Content = ReportRoI[1];
            ReportRoI = rpt.ROIVal(rpt.Getimg()[2], 1);
            M_Lower_ROI.Content = ReportRoI[0];
            M_Upper_ROI.Content = ReportRoI[1];
            ReportRoI = rpt.ROIVal(rpt.Getimg()[3], 1);
            L_Lower_ROI.Content = ReportRoI[0];
            L_Upper_ROI.Content = ReportRoI[1];
        }
        private void Report_Page2Data()
        {
            logdata.WriteLog("2페이지 로드");
            List<OpenCvSharp.Mat> img = rpt.PositionText();
            Page2_ReportA_View.Source = WriteableBitmapConverter.ToWriteableBitmap(img[0]);
            Page2_ReportP_View.Source = WriteableBitmapConverter.ToWriteableBitmap(img[2]);
            Page2_ReportM_View.Source = WriteableBitmapConverter.ToWriteableBitmap(img[1]);
            Page2_ReportL_View.Source = WriteableBitmapConverter.ToWriteableBitmap(img[3]);
            string sql = "SELECT Shot_Time From DB_ShotSave WHERE Date ='" + rptslt.ReportSelectDate + "'AND Sequences = 0 AND ImageNum = 0";
            SQLiteDataReader rdr = db.Load(sql);
            if (rdr.Read())
            {
                rptslt.ReportSelectStartTime = rdr["Shot_Time"].ToString();
            }
            strattime.Content = StringToDateTime(rptslt.ReportSelectStartTime);
            datatime.Content = StringToDateTime(rptslt.ReportSelectTime);
            timetaken.Content = (int)Math.Floor((Convert.ToDateTime(datatime.Content.ToString()) - Convert.ToDateTime(strattime.Content.ToString())).TotalMinutes) + "min";
            ChartSet();
        }
        private void Report_Page3Data()
        {
            logdata.WriteLog("3페이지 로드");
            List<OpenCvSharp.Mat> img = rpt.Getimg();
            CommentA_View.Source = WriteableBitmapConverter.ToWriteableBitmap(img[0]);
            CommentP_View.Source = WriteableBitmapConverter.ToWriteableBitmap(img[2]);
            CommentM_View.Source = WriteableBitmapConverter.ToWriteableBitmap(img[1]);
            CommentL_View.Source = WriteableBitmapConverter.ToWriteableBitmap(img[3]);
        }
        private string StringToDateTime(string str)
        {
            DateTime dateTime = DateTime.ParseExact(str, "HHmmss", null);
            string formattedTime = dateTime.ToString("HH:mm:ss");
            return formattedTime;
        }
        private void ChartSet()
        {
            chart.AxisX.Clear();
            chart.AxisY.Clear();
            Axis AxisX = new Axis();
            LiveCharts.Wpf.Separator sep = new LiveCharts.Wpf.Separator();
            sep.IsEnabled = true;
            sep.Step = 1;
            AxisX.Separator = sep;
            AxisX.Labels = new string[] { "A", "LA", "UA", "LP", "UP", "LM", "UM", "LL", "UL" };
            chart.AxisX.Add(AxisX);
            Axis AxisY = new Axis();
            LiveCharts.Wpf.Separator sep2 = new LiveCharts.Wpf.Separator();
            sep2.IsEnabled = true;
            sep2.Step = 1;
            AxisY.Separator = sep2;
            AxisY.Labels = new string[] { "", "Linear", "Splash", "Stardust", "Diffuse", "Non-Flow" };
            AxisY.MaxValue = 5;
            chart.AxisY.Add(AxisY);
            ChartView(chart);
        }
        private void ChartAddClick(object sender, RoutedEventArgs e)
        {
            if (chart.Visibility == Visibility.Visible)
            {
                chart.Visibility = Visibility.Hidden;
                ChartList.Visibility = Visibility.Visible;
                string sql = "SELECT DISTINCT Date,Position, Sequences FROM DB_Report where HID ='" + slt.SelectHID + "'ORDER BY Date DESC";
                ChartList.ItemsSource = db.Load(sql);
                ChartList.Items.Refresh();
                logdata.WriteLog("차트 추가");
            }
            else
            {
                ChartList.Visibility = Visibility.Hidden;
                chart.Visibility = Visibility.Visible;
            }
        }
        private void SelectChart(object sender, MouseButtonEventArgs e)
        {
            // 이벤트가 발생한 원본 요소를 찾습니다.
            var originalSource = e.OriginalSource as DependencyObject;

            // 원본 소스에서 부모 ListViewItem을 찾습니다.
            while (originalSource != null && !(originalSource is ListViewItem))
            {
                originalSource = VisualTreeHelper.GetParent(originalSource);
            }

            // ListViewItem이 발견되면 해당 아이템을 처리합니다.
            if (originalSource != null && originalSource is ListViewItem)
            {
                var listView = sender as ListView;
                if (listView != null)
                {
                    // 여기서 originalSource는 더블 클릭된 ListViewItem입니다.
                    // ListView에서 해당 ListViewItem의 인덱스를 찾습니다.
                    int select = listView.ItemContainerGenerator.IndexFromContainer(originalSource);
                    string sql = "SELECT FileName,Date From DB_Report WHERE(Date,Position, Sequences)IN(SELECT DISTINCT Date,Position, Sequences FROM DB_Report where HID = '" + slt.SelectHID + "'ORDER BY Date DESC LIMIT 1 OFFSET " + select + ") AND ImageNum = 0";
                    SQLiteDataReader rdr = db.Load(sql);
                    string path = "";
                    while (rdr.Read())
                    {
                        path = @"Report" + @"\" + rdr["FileName"].ToString() + ".dcm";
                    }
                    rdr.Close();
                    chartlist.Add(path);
                    ChartView(chart);
                    ChartList.Visibility = Visibility.Hidden;
                    chart.Visibility = Visibility.Visible;
                }
            }
            /*if (ChartList.SelectedItems.Count != 0)
            {
                logdata.WriteLog("차트 선택");
                int select = ChartList.SelectedIndex;
                string sql = "SELECT FileName,Date From DB_Report WHERE(Date,Position, Sequences)IN(SELECT DISTINCT Date,Position, Sequences FROM DB_Report where HID = '" + slt.SelectHID + "'ORDER BY Date DESC LIMIT 1 OFFSET " + select + ") AND ImageNum = 0";
                SQLiteDataReader rdr = db.Load(sql);
                string path = "";
                while (rdr.Read())
                {
                    path = @"Report" + @"\" + rdr["FileName"].ToString() + ".dcm";
                }
                rdr.Close();
                chartlist.Add(path);
                ChartView(chart);
                ChartList.Visibility = Visibility.Hidden;
                chart.Visibility = Visibility.Visible;
            }*/
        }
        private void ChartDelClick(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("차트 삭제");
            chartlist.Del();
            ChartView(chart);
        }
        public void ChartView(CartesianChart controlchart)
        {
            int select = ChartList.SelectedIndex;
            string sql = "SELECT FileName,Date From DB_Report WHERE(Date,Position, Sequences)IN(SELECT DISTINCT Date,Position, Sequences FROM DB_Report where HID = '" + slt.SelectHID + "'ORDER BY Date DESC LIMIT 1 OFFSET " + select + ") AND ImageNum = 0";
            SQLiteDataReader rdr = db.Load(sql);
            string date = "";
            while (rdr.Read())
            {
                date = rdr["Date"].ToString();
            }
            chartlist.Chartval();
            List<strutList> getchart = chartlist.GetChart();
            controlchart.Series.Clear();
            controlchart.Series.Add(new LiveCharts.Wpf.ColumnSeries
            {
                Title = rptslt.ReportSelectDate,
                MaxColumnWidth = 10,
                Values = new LiveCharts.ChartValues<double>(chartlist.GetChartval())
            });
            for (int i = 0; i < getchart.Count; i++)
            {
                controlchart.Series.Add(new LiveCharts.Wpf.ColumnSeries
                {
                    Title = getchart[i].date,
                    MaxColumnWidth = 10,
                    Values = new LiveCharts.ChartValues<double>(getchart[i].listval)
                });
            }
        }
        private void Left_page(object sender, RoutedEventArgs e)
        {
            if (page != 0)
            {
                page--;
                Report_Page.SelectedIndex = page;
                Pageset(page);
                if (num != 0)
                {
                    ReportLoadImage.Source = ReportDateList[num - 1].reportimage[page];
                }
                logdata.WriteLog($"왼쪽 페이지 클릭 : {page}Page");
                //DataTableView();
            }
        }
        private void Right_page(object sender, RoutedEventArgs e)
        {
            if (page != 2)
            {
                page++;
                Report_Page.SelectedIndex = page;
                Pageset(page);
                if (num != 0)
                {
                    ReportLoadImage.Source = ReportDateList[num - 1].reportimage[page];
                }
                logdata.WriteLog($"오른쪽 페이지 클릭 : {page}Page");
                //DataTableView();
            }
        }
        private void Report_day_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            num = int.Parse(button.Tag.ToString());
            logdata.WriteLog($"Report day 로드 : {num}");
            if (num == 0)
            {
                ReportLoadImage.Visibility = Visibility.Hidden;
                Report_Page.Visibility = Visibility.Visible;
            }
            else
            {
                ReportLoadImage.Visibility = Visibility.Visible;
                Report_Page.Visibility = Visibility.Hidden;
                ReportLoadImage.Source = ReportDateList[num - 1].reportimage[page];
            }

            //이동
        }
        private void ReportSave_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("리포트 저장");
            /*rd = new ReportDicom();
            FlowDocument doc = rd.CreateFlowDocument();
            DocumentPaginator paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;
            paginator.PageSize = new Size(400, 200);
            RenderTargetBitmap rtb = new RenderTargetBitmap(400, 200, 96, 96, PixelFormats.Default);*/
            // 텍스트를 포함한 이미지 생성
            /*FlowDocument doc = rd.CreateFlowDocument();
            RenderTargetBitmap rtb = new RenderTargetBitmap(400, 200, 96, 96, PixelFormats.Default);
            FixedDocument fixedDoc = new FixedDocument();
            PageContent pageContent = new PageContent();

            DocumentPage docPage = new DocumentPage(doc);
            docPage.Arrange(new Rect(0, 0, rtb.Width, rtb.Height));
            rtb.Render(docPage.Visual);*/
            /*DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                FormattedText text = new FormattedText(
                    "Hello, world!",
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    24,
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(visual).PixelsPerDip);

                context.DrawText(text, new Point(50, 50));
            }

            // 이미지를 렌더링하고 파일로 저장
            RenderTargetBitmap rtb = new RenderTargetBitmap(200, 100, 96, 96, PixelFormats.Default);
            rtb.Render(visual);

            // 이미지를 파일로 저장
            SaveAsImage(rtb, "output.png");

            Console.WriteLine("Image saved successfully!");*/
            // FlowDocument 생성
            // FlowDocument 생성
            //rd = new ReportDicom();
            //FlowDocument flowDoc = rd.CreateFlowDocument();
            List<string> patientinformation = new List<string>(new string[] {
                Report_Name.Content.ToString(),
                Report_Birthday.Content.ToString(),
                Report_Shotdate.Content.ToString(),
                MedicalStaff.Text,
                Report_HID.Content.ToString(),
                Report_Sex.Content.ToString(),
                Report_Position.Content.ToString(),
                diagnosis.Text});
            List<Image> imagesouce = new List<Image>(new Image[] { ReportA_View, ReportP_View, ReportM_View, ReportL_View });
            List<string> PattenValue_A = new List<string>(new string[] { A_Hand_Pattern.Text, A_Lower_Pattern.Text, A_Upper_Pattern.Text, A_Hand_ROI.Content.ToString(), A_Lower_ROI.Content.ToString(), A_Upper_ROI.Content.ToString() });
            List<string> PattenValue_P = new List<string>(new string[] { P_Lower_Pattern.Text, P_Upper_Pattern.Text, P_Lower_ROI.Content.ToString(), P_Upper_ROI.Content.ToString() });
            List<string> PattenValue_M = new List<string>(new string[] { M_Lower_Pattern.Text, M_Upper_Pattern.Text, M_Lower_ROI.Content.ToString(), M_Upper_ROI.Content.ToString() });
            List<string> PattenValue_L = new List<string>(new string[] { L_Lower_Pattern.Text, L_Upper_Pattern.Text, L_Lower_ROI.Content.ToString(), L_Upper_ROI.Content.ToString() });
            List<int> chatval = new List<int>(new int[] {
                A_Hand_Pattern.SelectedIndex,
                A_Lower_Pattern.SelectedIndex,
                A_Upper_Pattern.SelectedIndex,
                P_Lower_Pattern.SelectedIndex,
                P_Upper_Pattern.SelectedIndex,
                M_Lower_Pattern.SelectedIndex,
                M_Upper_Pattern.SelectedIndex,
                L_Lower_Pattern.SelectedIndex,
                L_Upper_Pattern.SelectedIndex});
            List<Image> positionimagesouce = new List<Image>(new Image[] { Page2_ReportA_View, Page2_ReportP_View, Page2_ReportM_View, Page2_ReportL_View });
            List<string> subdatatable = new List<string>(new string[] { strattime.Content.ToString(), datatime.Content.ToString(), timetaken.Content.ToString() });
            List<string> datatable = new List<string>(new string[] {
                A_Hand_Pattern.Text,VelocityA.Text,A_Hand_ROI.Content.ToString(),
                A_Lower_Pattern.Text,VelocityLA.Text,A_Lower_ROI.Content.ToString(),
                A_Upper_Pattern.Text,VelocityUA.Text,A_Upper_ROI.Content.ToString(),
                P_Lower_Pattern.Text,VelocityLP.Text,P_Lower_ROI.Content.ToString(),
                P_Upper_Pattern.Text,VelocityUP.Text,P_Upper_ROI.Content.ToString(),
                M_Lower_Pattern.Text,VelocityLM.Text,M_Lower_ROI.Content.ToString(),
                M_Upper_Pattern.Text,VelocityUM.Text,M_Upper_ROI.Content.ToString(),
                L_Lower_Pattern.Text,VelocityLL.Text,L_Lower_ROI.Content.ToString(),
                L_Upper_Pattern.Text,VelocityUL.Text,L_Upper_ROI.Content.ToString()
            });
            List<List<string>> comment = new List<List<string>>() { new List<string>() { "ANTERIOR", Anterior.Text }, new List<string>() { "POSTERIOR", Posterior.Text }, new List<string>() { "MEDIAL", Medial.Text }, new List<string>() { "LATERAL", Lateral.Text } };
            rd = new ReportDocument();
            /*for(int i = 0; i<3; i++)
            {
                FlowDocument doc = rd.CreateFlowDocument(patientinformation, imagesouce, PattenValue_A, PattenValue_P, PattenValue_M, PattenValue_L, positionimagesouce, subdatatable, datatable, comment, Summary.Text,i);
                RenderingView printchart_window = new RenderingView(doc,i);
                printchart_window.ShowDialog();
            }*/
            FlowDocument doc = rd.CreateFlowDocument(this, patientinformation, imagesouce, PattenValue_A, PattenValue_P, PattenValue_M, PattenValue_L, positionimagesouce, subdatatable, datatable, chatval, comment, Summary.Text);
            RenderingViewWindow printchart_window = new RenderingViewWindow(doc);
            printchart_window.ChildWindowClosed += ChildWindow_Closed;
            printchart_window.ShowDialog();
            MessageBox.Show("리포트가 저장되었습니다.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);


        }
        private async void ChildWindow_Closed(object sender, List<System.Drawing.Bitmap> bit)
        {
            List<int> chatval = new List<int>(new int[] {
                A_Hand_Pattern.SelectedIndex,
                A_Lower_Pattern.SelectedIndex,
                A_Upper_Pattern.SelectedIndex,
                P_Lower_Pattern.SelectedIndex,
                P_Upper_Pattern.SelectedIndex,
                M_Lower_Pattern.SelectedIndex,
                M_Upper_Pattern.SelectedIndex,
                L_Lower_Pattern.SelectedIndex,
                L_Upper_Pattern.SelectedIndex});

            for (int i = 0; i < bit.Count(); i++)
            {
                DicomManager dm = new DicomManager(slt.SelectHID, divice.GetSerialnum());
                dm.ReportSetPatient(slt.SelectHID, slt.SelectName, slt.SelectBirthday, slt.SelectSex, slt.SelectAge);
                dm.ReportSetStudy(rptslt.ReportSelectDate + "0001", rptslt.ReportSelectDate + "0001", rptslt.ReportSelectDate, rptslt.ReportSelectTime, "PNUH", "");
                dm.ReportSetSeries(rptslt.ReportSelectSequences, rptslt.ReportSelectPosition, rptslt.ReportSelectDate, rptslt.ReportSelectTime);
                dm.ReportSetContent(rptslt.ReportSelectSequences, rptslt.ReportSelectDate, rptslt.ReportSelectTime, i.ToString());
                dm.SetPattern(chatval);
                string path = SavePath(slt.SelectName, slt.SelectHID, rptslt.ReportSelectDate, rptslt.ReportSelectPosition, rptslt.ReportSelectSequences, i);
                Console.WriteLine(path);
                await dm.SaveReportFile(path, bit[i]);
            }                                                                                                                                                                                                                                                                                                                                                                                                                               
            logdata.WriteLog("리포트 저장 성공");
            dm.Load(db);

        }
        private string SavePath(string Name, string HID, string Date, string Position, string Sequences, int x)
        {
            string strFile = @"Report";
            DirectoryInfo fileInfo = new DirectoryInfo(strFile);
            if (fileInfo.Exists)
            {

            }
            else
            {
                fileInfo.Create();
            }
            string path = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + @"\x64\Debug\Report\" + "Report_" + HID + "_" + Date + Position + "(#" + Sequences + ")" + "(" + x + ")" + ".dcm";
            return path;
        }
    }

    /// <summary>
    /// 리포트 뷰어
    /// </summary>
    public partial class MainWindow : Window
    {
        List<BitmapImage> source = new List<BitmapImage>();
        int ReportPage = 0;
        private void ReportViewLoad()
        {
            logdata.WriteLog("리포트 뷰어 로드");
            MoveToTab("리포트 뷰어");
            //tabControl.SelectedIndex = 9;
        }
        private void button_ReportView_Click(object sender, RoutedEventArgs e)
        {
            ReportViewLoad();
        }
        private void List_ReportView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            source.Clear();
            if (List_ReportView.SelectedItems.Count != 0)
            {
                ReportPage = 0;
                int lastClickedIndex = 0;
                var selectedItems = List_ReportView.SelectedItems;
                if (selectedItems.Count > 0)
                {
                    var lastSelectedItem = selectedItems[selectedItems.Count - 1];
                    lastClickedIndex = List_ReportView.Items.IndexOf(lastSelectedItem);
                }
                string sql = "SELECT FileName From DB_Report WHERE(Date,Position, Sequences)IN(SELECT DISTINCT Date,Position, Sequences FROM DB_Report where HID = '" + slt.SelectHID + "'ORDER BY Date DESC, CAST(Sequences AS INT) LIMIT 1 OFFSET " + lastClickedIndex + ")";
                SQLiteDataReader rdr = db.Load(sql);
                Console.WriteLine(sql);
                List<string> path = new List<string>();

                while (rdr.Read())
                {
                    path.Add(@"Report" + @"\" + rdr["FileName"].ToString() + ".dcm");
                }
                rdr.Close();
                for (int i = 0; i < path.Count(); i++)
                {
                    new DicomSetupBuilder().RegisterServices(s => s.AddFellowOakDicom().AddImageManager<WinFormsImageManager>()).Build();
                    var m_pDicomFile = DicomFile.Open(path[i]);
                    var dicomImage = new DicomImage(m_pDicomFile.Dataset);
                    System.Drawing.Bitmap bitmap = dicomImage.RenderImage().As<System.Drawing.Bitmap>();
                    source.Add(ConvertBitmapToBitmapImage(bitmap));
                }
                ReportPageSet();
            }
        }
        private void Report_Left_page(object sender, RoutedEventArgs e)
        {
            if (ReportPage != 0)
            {
                ReportPage--;
                ReportPageSet();
                logdata.WriteLog($"리포트 왼쪽 페이지 클릭 : {ReportPage}Page");
            }
        }
        private void Report_Right_page(object sender, RoutedEventArgs e)
        {
            if (ReportPage != 2)
            {
                ReportPage++;
                ReportPageSet();
                logdata.WriteLog($"리포트 오른쪽 페이지 클릭 : {ReportPage}Page");
            }
        }
        private void ReportPageSet()
        {
            if (List_ReportView.SelectedItems.Count != 0)
            {
                ReportImage.Source = source[ReportPage];
            }
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (List_ReportView.SelectedItems.Count != 0)
            {
                
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    //printDialog.PrintTicket.PageOrientation = printDialog.PrintTicket.PageOrientation;
                    printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
                    FixedDocument fixedDoc = new FixedDocument();
                    for (int i = 0; i < source.Count; i++)
                    {

                        Image imageControl = new Image
                        {
                            Source = source[i],
                            Stretch = Stretch.Uniform,
                        };

                        // 이미지를 맞게 조절하기 위해 FixedPage의 크기를 조절
                        FixedPage fixedPage = new FixedPage();
                        fixedPage.Width = printDialog.PrintableAreaWidth;
                        fixedPage.Height = printDialog.PrintableAreaHeight;

                        // 이미지를 FixedPage에 추가
                        fixedPage.Children.Add(imageControl);

                        // FixedDocument 페이지에 추가
                        PageContent pageContent = new PageContent();
                        ((IAddChild)pageContent).AddChild(fixedPage);
                        fixedDoc.Pages.Add(pageContent);
                    }
                    // 프린트 작업 시작
                    printDialog.PrintDocument(fixedDoc.DocumentPaginator, "Printing Image");
                    logdata.WriteLog("프린트 성공");
                }
            }
            else
            {
                MessageBox.Show("프린트 하실 리포트를 선택하세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        private async void SendToPasc(object sender, RoutedEventArgs e)
        {
            if (List_ReportView.SelectedItems.Count != 0)
            {
                int lastClickedIndex = 0;
                var selectedItems = List_ReportView.SelectedItems;
                if (selectedItems.Count > 0)
                {
                    var lastSelectedItem = selectedItems[selectedItems.Count - 1];
                    lastClickedIndex = List_ReportView.Items.IndexOf(lastSelectedItem);
                }
                string sql = "SELECT FileName From DB_Report WHERE(Date,Position, Sequences)IN(SELECT DISTINCT Date,Position, Sequences FROM DB_Report where HID = '" + slt.SelectHID + "'ORDER BY Date DESC, CAST(Sequences AS INT) LIMIT 1 OFFSET " + lastClickedIndex + ")";
                SQLiteDataReader rdr = db.Load(sql);
                Console.WriteLine(sql);
                List<string> path = new List<string>();

                while (rdr.Read())
                {
                    path.Add(@"Report" + @"\" + rdr["FileName"].ToString() + ".dcm");
                }
                rdr.Close();
                for (int i = 0; i < path.Count(); i++)
                {
                    try
                    {
                        await SendToPacs(path[i], "111222333", IP.Text, int.Parse(Port.Text), AET.Text);
                        PacsDataSavebtn.IsEnabled = true;
                        MessageBox.Show("전송 완료", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        logdata.WriteLog("Pacs 전송 완료");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"전송 중 오류 발생: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        logdata.WriteLog($"Pacs 전송 실패 : {ex.Message}");
                    }
                }

            }
        }
    }

    /// <summary>
    /// 마스터
    /// </summary>
    public partial class MainWindow : Window
    {
        private void MasterLoad()
        {
            MasterSerialNumLoad();
            MasterManageLoad();
            MasterSensorLoad();
        }
        private void MasterSerialNumLoad()
        {
            string sql = "SELECT SerialNum FROM DB_Master LIMIT 1";
            SQLiteDataReader rdr = db.Load(sql);
            while (rdr.Read())
            {
                Serialnum.Text = rdr["SerialNum"].ToString();
            }
            rdr.Close();
        }
        private void MasterManageLoad()
        {
            string sql = "SELECT ID,Password FROM DB_Account LIMIT 1";
            SQLiteDataReader rdr = db.Load(sql);
            while (rdr.Read())
            {
                MI.Content = $"Manager ID : {rdr["ID"].ToString()}";
                MP.Content = $"Manager Password : {rdr["Password"].ToString()}";
            }

            rdr.Close();
        }
        private void MasterPacsDataLoad()
        {
            string sql = "SELECT AET,IP,Port FROM DB_Master LIMIT 1";
            SQLiteDataReader rdr = db.Load(sql);
            while (rdr.Read())
            {
                AET.Text = rdr["AET"].ToString();
                IP.Text = rdr["IP"].ToString();
                Port.Text = rdr["Port"].ToString();
            }

            rdr.Close();
        }
        private void MasterSensorLoad()
        {
            serial_PCB.WriteMessage("SE", "\n");
            serial_PCB.WriteMessage("LS", "\n");
            if (ssv != null)
            {
                string Mode = ssv.GetMode();
                mode.Content = $"mode : {Mode}";
                if (Mode == "Short")
                {
                    Sensor3.Content = "Sensor3 : 0";
                    Sensor4.Content = "Sensor4 : 0";
                }
                else if (Mode == "Nomal")
                {
                    Sensor3.Content = "Sensor3 : 1";
                    Sensor4.Content = "Sensor4 : 0";
                }
                else if (Mode == "Long")
                {
                    Sensor3.Content = "Sensor3 : 0";
                    Sensor4.Content = "Sensor4 : 1";
                }
                else if (Mode == "Custom")
                {
                    Sensor3.Content = "Sensor3 : 1";
                    Sensor4.Content = "Sensor4 : 1";
                }
                Sensor1.Content = $"Sensor1 : {ssv.GetSensor1()}";
                Sensor2.Content = $"Sensor2 : {ssv.GetSensor2()}";
                //else if(Mode == )
            }

        }
        private void SensorInit(object sender, RoutedEventArgs e)
        {
            MasterSensorLoad();
        }
        private void SerialSave(object sender, RoutedEventArgs e)
        {
            if (Serialnum.Text != "")
            {
                int check = CheckSerial();
                SetSerial(check);
                MessageBox.Show($"시리얼번호가 {Serialnum.Text}로 변경되었습니다.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Serialnum을 정확히 입력해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void PacsDataSave(object sender, RoutedEventArgs e)
        {
            if (AET.Text != "" && IP.Text != "" && Port.Text != "")
            {
                int check = CheckSerial();
                SetPacsData(check);
            }
            else
            {
                MessageBox.Show("Pacs Data를 정확히 입력해주세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private int CheckSerial()
        {
            string sql = "SELECT CASE WHEN EXISTS(SELECT 1 FROM DB_Master) THEN 1 ELSE 0 END As cnt";
            SQLiteDataReader cnt = db.Load(sql);
            string check = "";
            while (cnt.Read())
            {
                check = cnt["cnt"].ToString();
            }
            cnt.Close();
            return Convert.ToInt32(check);
        }
        private void SetSerial(int check)
        {
            string sql;
            if (check == 0)
            {
                
                sql = "insert into DB_Master (SerialNum) values('" + Serialnum.Text + "')";
            }
            else
            {
                sql = "update DB_Master set SerialNum='" + Serialnum.Text + "'";
            }
            db.Save(sql);
        }
        private void SetPacsData(int check)
        {
            string sql;
            if (check == 0)
            {
                sql = "insert into DB_Master (AET,IP,Port) values('" + AET.Text + "','" + IP.Text + "','" + Port.Text + "')";
            }
            else
            {
                sql = "update DB_Master set AET = '" + AET.Text + "', IP = '" + IP.Text + "', Port = '" + Port.Text + "'";
            }
            db.Save(sql);
        }
        private void ManagerInit(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("관리자 ID를 초기화 하시겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                string sql = "update DB_Account set ID = 'admin', Password = '1234' WHERE id = (SELECT id FROM DB_Account LIMIT 1)";
                db.Save(sql);
                MasterManageLoad();
                MessageBox.Show("관리자 ID가 초기화 완료 되었습니다.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async void PacsSendTest(object sender, RoutedEventArgs e)
        {
            try
            {
                await SendToPacs(@"TestDicom.dcm", "111222333", IP.Text, int.Parse(Port.Text), AET.Text);
                PacsDataSavebtn.IsEnabled = true;
                MessageBox.Show("전송 완료", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"전송 중 오류 발생: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ServoOn(object sender, RoutedEventArgs e)
        {
            servo.Cmd_SvoOn();
        }
        private void ServoOff(object sender, RoutedEventArgs e)
        {
            servo.Cmd_SvoOff();
        }
    }

    /// <summary>
    /// 설정
    /// </summary>
    public partial class MainWindow : Window
    {
        private void button_Setting_Click(object sender, RoutedEventArgs e)
        {
            logdata.WriteLog("설정 클릭");
            SettingWindow setting_Window = new SettingWindow(data, db);
            setting_Window.ShowDialog();
            if (setting_Window.DialogResult.HasValue && setting_Window.DialogResult.Value)
            {
                logdata.WriteLog($"설정 완료 : {data.Grid},{data.View_Range},{data.Auto_Rotation_Angle},{data.Manual_Rotation_Angle},{data.Exposure_Time},{data.Gain},{data.Gamma},{data.Filter}");
            }
        }
    }

    /// <summary>
    /// 기타구현
    /// </summary>
    public partial class MainWindow : Window
    {
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            HwndSource source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(new HwndSourceHook(this.WndProc));

            _viewModel.CapacityViewModel.InitializeDriveStatus();
            

            /*double drive = cap.CheckCapacity();
            if (drive < 10)
            {
                MessageBox.Show($"드라이브 사용 가능 공간이 {drive} GB 남았습니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }*/
            logdata.CleanupOldLogs(7); //일주일 지난 로그 지우기
            
            //logdata.WriteLog($"==============프로그램 로드 남은용량 : {drive}GB ==============");
            //db.Start();
            dm.Load(db);
            divice.Load(db);
            MasterPacsDataLoad();
            USBconnectionOpen();
            if (!cam.IsConnection())
            {
                deviceNums = cam.GetSerialNumbers();
                if (deviceNums.Count() != 0)
                {
                    string CameraNmae = "";
                    for (int i = 0; i < deviceNums.Count(); i++)
                    {
                        CameraNmae += deviceNums[i];
                    }
                    CameraCon.Header = "Camera(" + CameraNmae + ")";
                    await cam.InitAsync();
                    cam.CaptureImageHandler += Cam_CaptureImageHandler;
                    logdata.WriteLog("카메라 연결");
                }
                else
                {
                    MessageBox.Show("Camera connection failure", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logdata.WriteLog("카메라 연결실패");
                }

            }

            //ID_TextBox.Focus();

        }
        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(tabControl.SelectedIndex == 0)
            {
                UsernameTextBox.Focus();
            }
            else if(tabControl.SelectedIndex == 11)
            {
                Console.WriteLine("마스터 탭");
            }
            /*if (e.OriginalSource is TabControl)
            {
                if (tabControl.SelectedItem is TabItem selectedTab)
                {
                    WindowName.Text = selectedTab.Header.ToString();
                }
                if (tabControl.SelectedIndex == 1)
                {
                    string sql = "SELECT * FROM DB_Account LIMIT (Select COUNT(*)-1 cnt FROM DB_Account) OFFSET 1";
                    List_Account.ItemsSource = db.Load(sql);
                }
                else if (tabControl.SelectedIndex == 2)
                {
                    PatientRefresh();
                }
                else if (tabControl.SelectedIndex == 3)
                {
                    ScanInit();
                }
                else if (tabControl.SelectedIndex == 4)
                {
                    string sql = "SELECT DISTINCT Date,Position, Sequences, Mode, CameraSet FROM DB_ShotSave where HID ='" + slt.SelectHID + "'ORDER BY Date DESC, CAST(Sequences AS INT)";
                    ImageGrid.Children.Clear();
                    List_ImageReview.ItemsSource = db.Load(sql);
                    List_ImageReview.Items.Refresh();
                    SendToPacsbtnEN(SendPacsbtn);
                }
                else if (tabControl.SelectedIndex == 5)
                {
                    Image_Select.Children.Clear();
                    img_View1.Source = null;
                    img_View2.Source = null;
                    img_View3.Source = null;
                    img_View4.Source = null;
                    information1_View1.Content = null;
                    information2_View1.Text = null;
                    information3_View1.Content = null;
                    information4_View1.Content = null;
                    information1_View2.Content = null;
                    information2_View2.Text = null;
                    information3_View2.Content = null;
                    information4_View2.Content = null;
                    information1_View3.Content = null;
                    information2_View3.Text = null;
                    information3_View3.Content = null;
                    information4_View3.Content = null;
                    information1_View4.Content = null;
                    information2_View4.Text = null;
                    information3_View4.Content = null;
                    information4_View4.Content = null;
                    Border1.BorderBrush = System.Windows.Media.Brushes.Gray;
                    Border2.BorderBrush = System.Windows.Media.Brushes.Gray;
                    Border3.BorderBrush = System.Windows.Media.Brushes.Gray;
                    Border4.BorderBrush = System.Windows.Media.Brushes.Gray;
                    var selectedItems = List_ImageReview.SelectedItems;
                    List<string> selectedIndexes = new List<string>();
                    for (int i = 0; i < selectedItems.Count; i++)
                    {
                        var select = selectedItems[i];
                        selectedIndexes.Add(List_ImageReview.Items.IndexOf(select).ToString());
                    }
                    selectedIndexes.Sort();
                    OpenCvSharp.Mat[][] source = new OpenCvSharp.Mat[selectedIndexes.Count()][];
                    string[][] Viewdata2 = new string[selectedIndexes.Count()][];
                    string[][] Viewdata3 = new string[selectedIndexes.Count()][];
                    string[][] Viewdata4 = new string[selectedIndexes.Count()][];

                    List<string> SelectName = new List<string>();
                    for (int i = 0; i < selectedIndexes.Count(); i++)
                    {
                        //string sql = "SELECT FileName,Date,Position,Sequences,Mode,CameraSet,ImageNum From DB_ShotSave WHERE(HID,Date,Position, Sequences, Mode)IN(SELECT DISTINCT HID,Date,Position, Sequences, Mode FROM DB_ShotSave where HID = '" + slt.SelectHID + "'ORDER BY Date DESC,CAST(Sequences AS INT) LIMIT 1 OFFSET " + selectedIndexes[i] + ")";
                        string sql = $"SELECT FileName,DB_ShotSave.Date,Position,Sequences,Mode,CameraSet,ImageNum,Shot_Time,Injection_Time From DB_ShotSave LEFT JOIN DB_Injection ON DB_ShotSave.Date = DB_Injection.Date AND DB_ShotSave.HID = DB_Injection.HID WHERE(DB_ShotSave.HID,DB_ShotSave.Date,Position, Sequences, Mode) IN(SELECT DISTINCT DB_ShotSave.HID,DB_ShotSave.Date,Position, Sequences, Mode FROM DB_ShotSave where HID = '{slt.SelectHID}' ORDER BY DB_ShotSave.Date DESC,CAST(Sequences AS INT) LIMIT 1 OFFSET {selectedIndexes[i]})";
                        Console.WriteLine(sql);
                        SQLiteDataReader rdr = db.Load(sql);
                        List<string> path = new List<string>();
                        List<string> Shotdata = new List<string>();
                        List<string> Mode = new List<string>();
                        List<string> Cameradata = new List<string>();
                        List<string> InjectionTime = new List<string>();
                        List<string> ShotTime = new List<string>();
                        while (rdr.Read())
                        {
                            SelectName.Add(rdr["Date"].ToString() + "#" + rdr["Sequences"].ToString() + "/" + rdr["Mode"].ToString());
                            Shotdata.Add($"{rdr["Date"].ToString()}{rdr["Position"].ToString()}#{rdr["Sequences"].ToString()} {int.Parse(rdr["ImageNum"].ToString())+1}");
                            Mode.Add(rdr["Mode"].ToString());
                            Cameradata.Add(rdr["CameraSet"].ToString());
                            path.Add(@"dicom" + @"\" + rdr["FileName"].ToString() + ".dcm");
                            ShotTime.Add(rdr["Shot_Time"].ToString());
                            InjectionTime.Add(rdr["Injection_Time"].ToString());
                        }
                        rdr.Close();
                        source[i] = new OpenCvSharp.Mat[path.Count()];
                        Viewdata2[i] = new string[path.Count()];
                        Viewdata3[i] = new string[path.Count()];
                        Viewdata4[i] = new string[path.Count()];
                        System.Drawing.Bitmap[] selectbitmap = new System.Drawing.Bitmap[path.Count()];
                        for (int j = 0; j < path.Count(); j++)
                        {
                            new DicomSetupBuilder().RegisterServices(s => s.AddFellowOakDicom().AddImageManager<WinFormsImageManager>()).Build();
                            var m_pDicomFile = DicomFile.Open(path[j]);
                            var dicomImage = new DicomImage(m_pDicomFile.Dataset);
                            System.Drawing.Bitmap bitmap = dicomImage.RenderImage().As<System.Drawing.Bitmap>();
                            source[i][j] = BitmapConverter.ToMat(bitmap);
                            Viewdata2[i][j] = Shotdata[j] + "/" + path.Count() + "\n" + Mode[j];
                            if(InjectionTime[j] != "")
                            {
                                Viewdata3[i][j] = $"IT {InjectionTime[j].Substring(0, 2)}:{InjectionTime[j].Substring(2, 2)}:{InjectionTime[j].Substring(4, 2)}" + "\n" + $"ST {ShotTime[j].Substring(0, 2)}:{ShotTime[j].Substring(2, 2)}:{ShotTime[j].Substring(4, 2)}";
                            }
                            else
                            {
                                Viewdata3[i][j] = $"ST {ShotTime[j].Substring(0, 2)}:{ShotTime[j].Substring(2, 2)}:{ShotTime[j].Substring(4, 2)}";
                            }
                            Viewdata4[i][j] = Cameradata[j];
                        }
                    }
                    slt.selctsource = source;
                    slt.Viewdata1 = $"{slt.SelectName}({slt.SelectHID})\n{slt.SelectBirthday} - {slt.SelectSex}";
                    slt.Viewdata2 = Viewdata2;
                    slt.Viewdata3 = Viewdata3;
                    slt.Viewdata4 = Viewdata4;
                    SelectName = SelectName.Distinct().ToList();
                    Image_Review_Select(Image_Select, 300, 120, 2, source, SelectName);
                }
                else if(tabControl.SelectedIndex == 6)
                {
                    string folderPath = @"Video\" + $"{slt.SelectName}({slt.SelectHID})";
                    string dicomFolderPath = @"DicomVideo";
                    List<DataModel> dataList = new List<DataModel>();
                    // 지정된 폴더 내의 모든 파일명을 배열로 가져옴
                    if (Directory.Exists(folderPath))
                    {
                        string[] directories = Directory.GetDirectories(folderPath);
                        Array.Sort(directories);
                        Array.Reverse(directories);
                        foreach (string directory in directories)
                        {
                            // 전체 경로가 아니라 파일명만 출력하고 싶으면 Path.GetFileName 사용
                            string[] files = Directory.GetFiles(directory);
                            Console.WriteLine(directory);
                            foreach (string file in files)
                            {
                                string fileName = System.IO.Path.GetFileName(file);
                                string filePath = System.IO.Path.Combine(dicomFolderPath, fileName.Replace("avi", "dcm"));
                                string[] split = fileName.Split('_');
                                dataList.Add(new DataModel { Date = split[2], Position = split[3], Sequences = split[4].Replace(".avi", ""), Dicom = File.Exists(filePath) ? "O" : "X", FileName = fileName,  Path = $@"{directory}\{fileName}" });
                            }
                        }
                    }
                    List_VideoReview.ItemsSource = dataList;
                    //string sql = "SELECT DISTINCT Date,Position, Sequences FROM DB_Video where HID ='" + slt.SelectHID + "'ORDER BY Date DESC, CAST(Sequences AS INT)";
                    //List_VideoReview.ItemsSource = db.Load(sql);
                    //List_VideoReview.Items.Refresh();
                    Thumbnail.Source = null;
                    SendToPacsbtnEN(VideoSendPacsbtn);
                }
                else if (tabControl.SelectedIndex == 10)
                {
                    ReportImage.Source = null;
                    string sql = "SELECT DISTINCT Date,Position, Sequences FROM DB_Report where HID ='" + slt.SelectHID + "'ORDER BY Date DESC, CAST(Sequences AS INT)";
                    List_ReportView.ItemsSource = db.Load(sql);
                    List_ReportView.Items.Refresh();
                    SendToPacsbtnEN(ReportSendPacsbtn);
                }
                if (tabControl.SelectedIndex != 3)
                {
                    if (cam.IsOpen())
                    {
                        cam.StopLiveView();
                    }
                }
            }*/
        }
        public class DataModel
        {
            public string Date { get; set; }
            public string Position { get; set; }
            public string Sequences { get; set; }
            public string Dicom { get; set; }
            public string FileName { get; set; }
            public string Path { get; set; }
        }
        IntPtr WndProc(IntPtr hWnd, int nMsg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            UInt32 WM_DEVICECHANGE = 0x0219;
            //UInt32 DBT_DEVTUP_VOLUME = 0x02;
            UInt32 DBT_DEVICEARRIVAL = 0x8000;
            UInt32 DBT_DEVICEREMOVECOMPLETE = 0x8004;
            List<string> deviceNums;
            if (nMsg == WM_DEVICECHANGE)
            {
                deviceNums = cam.GetSerialNumbers();
                if (deviceNums.Count() != 0)
                {
                    Console.WriteLine("카메라인식");

                }
                else
                {
                    Console.WriteLine("카메라인식해제");
                }
            }
            //디바이스 연결시
            if ((nMsg == WM_DEVICECHANGE) && (wParam.ToInt32() == DBT_DEVICEARRIVAL))
            {
                Console.WriteLine("연결");
                USBconnectionOpen();
            }

            //디바이스 연결 해제시...
            if ((nMsg == WM_DEVICECHANGE) && (wParam.ToInt32() == DBT_DEVICEREMOVECOMPLETE))
            {
                Console.WriteLine("연결해제");
                USBconnectionClose();
            }

            return IntPtr.Zero;
        }
        private async void USBconnectionOpen()
        {
            string USB_Serial_Port = usb.GetList()[0];
            string USB_SERIAL_CH340 = usb.GetList()[1];
            await serial_PCB.Open(USB_SERIAL_CH340, 115200);
            await serial_servo.Open(USB_Serial_Port, 9600);
            if (serial_PCB.IsOpen())
            {
                Up.IsEnabled = true;
                Down.IsEnabled = true;
                LED.IsEnabled = true;
                LED2.IsEnabled = true;
                PCBCon.Header = "PCB(" + USB_SERIAL_CH340 + ")";
                serial_PCB.DataReceived += Serial_DataReceived;
                serial_PCB.Getserial().DataReceived += serial_PCB.Serial_DataReceived;
                logdata.WriteLog("PCB 연결");
            }
            else
            {
                Up.IsEnabled = false;
                Down.IsEnabled = false;
                LED.IsEnabled = false;
                LED2.IsEnabled = false;
                PCBCon.Header = "PCB(not connected)";
                logdata.WriteLog("PCB 연결실패");
                MessageBox.Show("PCB connection failure", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (serial_servo.IsOpen())
            {
                Preview.IsEnabled = true;
                Reset.IsEnabled = true;
                Reverse.IsEnabled = true;
                Forward.IsEnabled = true;
                ServoCon.Header = "Servo(" + USB_Serial_Port + ")";
                servo.SetSerial(serial_servo.Getserial());
                servo.mainUI(main);
                servo.Cmd_SvoOn();
                servo.Cmd("Reset", data);
                PositionLabel.Content = servo.GetPositionLabel();
                logdata.WriteLog("서보모터 연결");

            }
            else
            {
                Preview.IsEnabled = false;
                Reset.IsEnabled = false;
                Reverse.IsEnabled = false;
                Forward.IsEnabled = false;
                ServoCon.Header = "Servo(not connected)";
                logdata.WriteLog("서보모터 연결실패");
                MessageBox.Show("Servo Moter connection failure", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void USBconnectionClose()
        {
            if (serial_PCB.IsOpen())
            {
                serial_PCB.Close();
            }
            else
            {
                Up.IsEnabled = false;
                Down.IsEnabled = false;
                LED.IsEnabled = false;
                LED2.IsEnabled = false;
                PCBCon.Header = "PCB(not connected)";
            }
            if (serial_servo.IsOpen())
            {
                serial_servo.Close();
            }
            else
            {
                Preview.IsEnabled = false;
                Reset.IsEnabled = false;
                Reverse.IsEnabled = false;
                Forward.IsEnabled = false;
                ServoCon.Header = "Servo(not connected)";
            }

        }
        private BitmapImage ConvertBitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
        private void SendToPacsbtnEN(Button sendbtn)
        {
            string sql = "SELECT CASE WHEN AET IS NOT NULL AND IP IS NOT NULL AND Port IS NOT NULL THEN 'true' ELSE 'false' END AS result FROM DB_Master";
            SQLiteDataReader rdr = db.Load(sql);
            if (rdr.Read())
            {
                bool check = bool.Parse(rdr["result"].ToString());
                sendbtn.IsEnabled = check;
            }
        }
        public async Task SendToPacs(string dcmfile, string sourceAET, string targetIP, int targetPort, string targetAET)
        {
            var m_pDicomFile = DicomFile.Open(dcmfile);
            var client = DicomClientFactory.Create(targetIP, targetPort, false, sourceAET, targetAET);
            client.NegotiateAsyncOps();

            // DICOM C-STORE 요청 생성
            var request = new DicomCStoreRequest(m_pDicomFile);

            // 전송 성공 시 호출될 콜백
            request.OnResponseReceived += (req, response) =>
            {
                if (response.Status == DicomStatus.Success)
                {
                    Console.WriteLine("전송 성공");
                }
                else
                {
                    Console.WriteLine($"전송 실패: {response.Status}");
                }
            };

            // 요청 추가
            await client.AddRequestAsync(request);

            // 전송 시작
            await client.SendAsync();
        }
        private void ErrCheck(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"ErrCode : {servo.Cmd_ErrCheck()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void AlarmReset_Click(object sender, RoutedEventArgs e)
        {
            servo.Cmd_AlarmReset();
            servo.Cmd_SvoOn();
        }
        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("변경사항이 저장되지 않을 수 있습니다.\n프로그램을 종료하시겠습니까?", "프로그램 종료 확인", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                logdata.WriteLog("종료");
                if (serial_PCB.IsOpen())
                {
                    serial_PCB.WriteMessage("LD0", "\n");
                    /*if (data.LED != 0)
                    {
                        for (int i = 0; i < 4 - data.LED; i++)
                        {
                            serial_PCB.WriteMessage("LE", "\n");
                        }
                    }*/
                    serial_PCB.WriteMessage("LE0", "\n");
                    serial_PCB.WriteMessage("FA0", "\n");
                    serial_PCB.Close();
                }

                if (serial_servo.IsOpen())
                {
                    servo.Cmd_SvoOff();
                    serial_servo.Close();
                }
                //Environment.Exit(0);
                if (cam.IsOpen())
                {
                    cam.StopLiveView();
                    cam.Dispose();
                }
                Environment.Exit(0);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                this.Close();
            }
        }

        public void LED_Off()
        {
            if (serial_PCB.IsOpen())
            {
                data.LED = 0;
                data.LED2 = 0;
                serial_PCB.WriteMessage("LD0", "\n");
                serial_PCB.WriteMessage("LE0", "\n");
                serial_PCB.WriteMessage("FA0", "\n");
                LED.Foreground = Brushes.White;
                LED2.Foreground = Brushes.White;
            }
        }

        private void MoveToTab(string headerName)
        {
            /*foreach(TabItem tabItem in tabControl.Items)
            {
                if (tabItem.Header.ToString() == headerName)
                {
                    tabControl.SelectedItem = tabItem;
                    break;
                }
            }*/
        }
    }
}
