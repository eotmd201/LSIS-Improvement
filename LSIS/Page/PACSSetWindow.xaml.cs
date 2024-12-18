using FellowOakDicom;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;
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
    /// PACSSetWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PACSSetWindow : Window
    {
        DB db;
        public PACSSetWindow(DB db)
        {
            this.db = db;
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MasterPacsDataLoad();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            GetWindow(this).Close();
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
        private void PacsDataSave(object sender, RoutedEventArgs e)
        {
            if (AET.Text != "" && IP.Text != "" && Port.Text != "")
            {
                int check = CheckSerial();
                SetPacsData(check);
                this.DialogResult = true;
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

        
    }
}
