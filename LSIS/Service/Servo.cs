using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LSIS
{
    class Servo
    {
        public event Action<bool> OnTimeChanged;
        SerialPort serial;
        MainWindow main;
        byte[] SvoOn = new byte[8] { 0x01, 0x05, 0x00, 0x60, 0xFF, 0x00, 0x8C, 0x24 };
        byte[] SvoOff = new byte[8] { 0x01, 0x05, 0x00, 0x60, 0x00, 0x00, 0xCD, 0xD4 };
        byte[] AlarmReset = new byte[8] { 0x01, 0x05, 0x00, 0x61, 0xFF, 0x00, 0xDD, 0xE4 };
        byte[] AlarmReset2 = new byte[8] { 0x01, 0x05, 0x00, 0x61, 0x00, 0x00, 0x9C, 0x14 };
        byte[] SvonmoveOn = new byte[8] { 0x01, 0x01, 0x00, 0xA2, 0x00, 0x01, 0x5C, 0x28 };
        byte[] ServoOnCheck = new byte[8] { 0x01, 0x01, 0x00, 0x60, 0x00, 0x01, 0xFD, 0xD4 };

        byte[] test = new byte[8] { 0x01, 0x03, 0x40, 0x20, 0x00, 0x02, 0xD0, 0x01 };
        byte[] test2 = new byte[8] { 0x01, 0x03, 0x60, 0x0F, 0x00, 0x02, 0xEA, 0x08 };

        byte[] Load = new byte[8] { 0x01, 0x03, 0x40, 0x0E, 0x00, 0x01, 0xF0, 0x09 };
        byte[] Reset = new byte[8] { 0x01, 0x06, 0x44, 0x14, 0x00, 0x05, 0X1D, 0x3D };
        byte[] Servo_22_5 = new byte[8] { 0x01, 0x06, 0x44, 0x14, 0x00, 0x01, 0x1C, 0xFE };
        byte[] Servo_r22_5 = new byte[8] { 0x01, 0x06, 0x44, 0x14, 0x00, 0x08, 0xDC, 0xF8 };
        byte[] Servo_45 = new byte[8] { 0x01, 0x06, 0x44, 0x14, 0x00, 0x00, 0xDD, 0x3E };
        byte[] Servo_r45 = new byte[8] { 0x01, 0x06, 0x44, 0x14, 0x00, 0x07, 0x9C, 0xFC };
        byte[] Servo_90 = new byte[8] { 0x01, 0x06, 0x44, 0x14, 0x00, 0x02, 0x5C, 0xFF };
        byte[] Servo_r90 = new byte[8] { 0x01, 0x06, 0x44, 0x14, 0x00, 0x03, 0x9D, 0x3F };
        byte[] Servo_360 = new byte[8] { 0x01, 0x06, 0x44, 0x14, 0x00, 0x0A, 0x5D, 0x39 };
        byte[] Servo_r360 = new byte[8] { 0x01, 0x06, 0x44, 0x14, 0x00, 0x0B, 0x9C, 0xF9 };
        byte[] TestCheck0 = new byte[8] { 0x01, 0x05, 0x01, 0x20, 0xFF, 0x00, 0x8C, 0x0C };
        byte[] TestCheck1 = new byte[8] { 0x01, 0x05, 0x01, 0x20, 0x00, 0x00, 0xCD, 0xFC };
        byte[] errcode = new byte[8] { 0x01, 0x03, 0x40, 0x01, 0x00, 0x01, 0xC0, 0x0A };
        byte[] TT = new byte[8] { 0x01, 0x03, 0x44, 0x0A, 0x00, 0x01, 0xB0, 0xF8 };
        /*byte[] TT = new byte[8] { 0x01, 0x03, 0x44, 0x0A, 0x00, 0xA1, 0x71, 0x40 };
        byte[] TT = new byte[8] { 0x01, 0x03, 0x44, 0x0A, 0x00, 0xAD, 0x71, 0x40 };
        byte[] TT = new byte[8] { 0x01, 0x03, 0x44, 0x0A, 0x00, 0xA3, 0x71, 0x40 };*/

        byte[] Srdy = new byte[8] { 0x01, 0x03, 0x00, 0xA0, 0x00, 0x01, 0x84, 0x28 };
        byte[] Aml = new byte[8] { 0x01, 0x03, 0x00, 0xA1, 0x00, 0x01, 0xD5, 0xE8 };
        byte[] Dbrk = new byte[8] { 0x01, 0x03, 0x00, 0xAD, 0x00, 0x01, 0x15, 0xEB };
        byte[] Brkoff = new byte[8] { 0x01, 0x03, 0x00, 0xA3, 0x00, 0x01, 0x74, 0x28 };

        /*byte[] Srdy = new byte[8] { 0x01, 0x03, 0x44, 0x0A, 0x00, 0x01, 0x71, 0x40 };
        byte[] Aml = new byte[8] { 0x01, 0x03, 0x44, 0x0A, 0x00, 0xA1, 0xB0, 0x80 };
        byte[] Dbrk = new byte[8] { 0x01, 0x03, 0x44, 0x0A, 0x00, 0xAD, 0xB0, 0x85 };
        byte[] Brkoff = new byte[8] { 0x01, 0x03, 0x44, 0x0A, 0x00, 0xA3, 0x31, 0x41 };*/


        public double Angle = 0;
        private int Preview = 0;
        public bool SaveMoveState = false;
        public bool LastMoveState = true;
        private bool timerMove;
        public bool TimerMove
        {
            get => timerMove;
            set
            {
                if (timerMove != value)
                {
                    timerMove = value;
                    OnTimeChanged?.Invoke(timerMove);
                }
            }
        }
        private string PositionLabel;
        DateTime LastMoveTime;
        private DispatcherTimer timer = new DispatcherTimer();
        private DispatcherTimer timer2 = new DispatcherTimer();
        public Servo()
        {
            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer2.Tick += timer_Tick2;
            timer2.Interval = TimeSpan.FromSeconds(5);
            
        }
        public double Test()
        {
            string hex = PanasonicServoCmd(test2);
            hex = hex.Substring(15, 2) + hex.Substring(18, 2) + hex.Substring(9, 2) + hex.Substring(12, 2);
            double hex_10 = Convert.ToInt32(hex, 16);
            hex_10 = hex_10 * 360 / 500000;
            return hex_10;
        }
        public string Testres()
        {
            string hex = PanasonicServoCmd(Load);
            return hex;
        }
        public void SetSerial(SerialPort serial)
        {
            this.serial = serial;
            timer2.Start();
        }
        public void mainUI(MainWindow main)
        {
            this.main = main;
        }
        public void LastMove(DateTime Time)
        {
            LastMoveTime = Time;
        }
        public DateTime GetLastMoveTime()
        {
            return LastMoveTime;
        }
        public double GetAngle()
        {
            return Angle;
        }
        void timer_Tick(object sender, EventArgs e)
        {
            if (PanasonicServoCmd(SvonmoveOn) == "01-01-01-01-90-48")
            {
                TimerMove = false;
                main.Preview.IsEnabled = true;
                if (Preview == 0)
                {
                    main.Reset.IsEnabled = true;
                    main.Reverse.IsEnabled = true;
                    main.Forward.IsEnabled = true;
                    main.AutoScan.IsEnabled = main.AutoscanEnabled();
                }
                timer.Stop();
            }
            else
            {
                TimerMove = true;
                main.Preview.IsEnabled = false;
                main.Reset.IsEnabled = false;
                main.Reverse.IsEnabled = false;
                main.Forward.IsEnabled = false;
                main.AutoScan.IsEnabled = main.AutoscanEnabled();
            }
        }
        void timer_Tick2(object sender, EventArgs e)
        {
            if(Cmd_ErrCheck()!="0.0"&& serial.IsOpen)
            {
                string err = Cmd_ErrCheck();
                if(err =="94.0")
                {
                    MessageBox.Show($"ErrCode : {err}\n비상정지 버튼 확인 후 확인을 누르세요.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Cmd_AlarmReset();
                    Cmd_SvoOn();
                }
                else if(err == "16.0")
                {
                    MessageBox.Show($"ErrCode : {err}\n모터 회전 반경내 정리 후 확인을 누르세요.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Cmd_AlarmReset();
                    Cmd_SvoOn();
                }
                else
                {
                    MessageBox.Show($"ErrCode : {err} 에러코드를 확인 후 제조사에 문의하세요.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        public bool MoveState()
        {
            if (PanasonicServoCmd(SvonmoveOn) == "01-01-01-01-90-48")
            {
                LastMoveState = true;
            }
            else
            {
                LastMoveState = false;
            }
            return LastMoveState;
        }
        public void PositionLabelValue(string SelectPosition)
        {
            if (GetAngle() == 0)
            {
                if (SelectPosition == "RA" || SelectPosition == "RL")
                {
                    PositionLabel = SelectPosition + " A ▼";
                }
                else if (SelectPosition == "LA" || SelectPosition == "LL")
                {
                    PositionLabel = SelectPosition + " A ▲";
                }
                else
                {
                    PositionLabel = "";
                }
            }
            else if (GetAngle() == 90)
            {
                if (SelectPosition == "RA" || SelectPosition == "RL")
                {
                    PositionLabel = SelectPosition + " L ●";
                }
                else if (SelectPosition == "LA" || SelectPosition == "LL")
                {
                    PositionLabel = SelectPosition + " M ◎";
                }
                else
                {
                    PositionLabel = "";
                }
            }
            else if (GetAngle() == 180)
            {
                if (SelectPosition == "RA" || SelectPosition == "RL")
                {
                    PositionLabel = SelectPosition + " P ▲";
                }
                else if (SelectPosition == "LA" || SelectPosition == "LL")
                {
                    PositionLabel = SelectPosition + " P ▼";
                }
                else
                {
                    PositionLabel = "";
                }
            }
            else if (GetAngle() == 270)
            {
                if (SelectPosition == "RA" || SelectPosition == "RL")
                {
                    PositionLabel = SelectPosition + " M ◎";
                }
                else if (SelectPosition == "LA" || SelectPosition == "LL")
                {
                    PositionLabel = SelectPosition + " L ●";
                }
                else
                {
                    PositionLabel = "";
                }
            }
        }
        public string GetPositionLabel()
        {
            return PositionLabel;
        }
        public void Cmd(string cmd, Data data)
        {
            if (MoveState())
            {
                timer.Start();
                double ChangeAngle = 22.5 * Math.Pow(2, data.Manual_Rotation_Angle);
                if (cmd == "Reset")
                {
                    Cmd_Reset(data.SelectPosition());
                }
                else if (cmd == "Forward")
                {
                    if (Angle + ChangeAngle < 360)
                    {
                        if (data.Manual_Rotation_Angle == 0)
                        {
                            Cmd_22_5(data.SelectPosition());
                        }
                        else if (data.Manual_Rotation_Angle == 1)
                        {
                            Cmd_45(data.SelectPosition());
                        }
                        else if (data.Manual_Rotation_Angle == 2)
                        {
                            Cmd_90(data.SelectPosition());
                        }
                    }
                    else
                    {
                        MessageBox.Show("구동 범위가 벗어나는 명령 입니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                }
                else if (cmd == "Reverse")
                {
                    if (Angle - ChangeAngle >= 0)
                    {
                        if (data.Manual_Rotation_Angle == 0)
                        {
                            Cmd_r22_5(data.SelectPosition());
                        }
                        else if (data.Manual_Rotation_Angle == 1)
                        {
                            Cmd_r45(data.SelectPosition());
                        }
                        else if (data.Manual_Rotation_Angle == 2)
                        {
                            Cmd_r90(data.SelectPosition());
                        }
                    }
                    else
                    {
                        MessageBox.Show("구동 범위가 벗어나는 명령 입니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else if (cmd == "Preview")
                {
                    if (Preview == 0)
                    {
                        if (Angle == 0)
                        {
                            Preview++;
                            data.Grid = 0;
                            Cmd_90(data.SelectPosition());
                        }
                        else
                        {
                            MessageBox.Show("리셋후 작동하세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else if (Preview == 1)
                    {
                        data.Grid = 1;
                        Preview++;
                    }
                    else if (Preview == 2)
                    {
                        Cmd_r90(data.SelectPosition());
                        Preview++;
                    }
                    if (Preview == 3)
                    {
                        Preview = 0;
                    }

                }
            }
            else
            {
                if(cmd != "Auto")
                {
                    MessageBox.Show($"모터가 동작 중입니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            if (cmd == "Auto")
            {
                if (data.Auto_Rotation_Angle == 0)
                {
                    Cmd_45(data.SelectPosition());
                }
                else if (data.Auto_Rotation_Angle == 1)
                {
                    Cmd_90(data.SelectPosition());
                }
            }
            /*if(Cmd_ErrCheck() == "0.0")
            {
                if (MoveState())
                {
                    double ChangeAngle = 22.5 * Math.Pow(2, data.Manual_Rotation_Angle);
                    if (cmd == "Reset")
                    {
                        main.Preview.IsEnabled = false;
                        main.Reset.IsEnabled = false;
                        main.Reverse.IsEnabled = false;
                        main.Forward.IsEnabled = false;
                        timerMove = true;
                        main.AutoScan.IsEnabled = main.AutoscanEnabled();
                        Cmd_Reset(data.SelectPosition());
                        timer.Start();
                    }
                    else if (cmd == "Forward")
                    {
                        if (Angle + ChangeAngle < 360)
                        {
                            if (data.Manual_Rotation_Angle == 0)
                            {
                                Cmd_22_5(data.SelectPosition());
                            }
                            else if (data.Manual_Rotation_Angle == 1)
                            {
                                Cmd_45(data.SelectPosition());
                            }
                            else if (data.Manual_Rotation_Angle == 2)
                            {
                                Cmd_90(data.SelectPosition());
                            }
                        }
                        else
                        {
                            MessageBox.Show("작동불가.");
                        }

                    }
                    else if (cmd == "Reverse")
                    {
                        if (Angle - ChangeAngle >= 0)
                        {
                            if (data.Manual_Rotation_Angle == 0)
                            {
                                Cmd_r22_5(data.SelectPosition());
                            }
                            else if (data.Manual_Rotation_Angle == 1)
                            {
                                Cmd_r45(data.SelectPosition());
                            }
                            else if (data.Manual_Rotation_Angle == 2)
                            {
                                Cmd_r90(data.SelectPosition());
                            }
                        }
                        else
                        {
                            MessageBox.Show("작동불가.");
                        }
                    }
                    else if (cmd == "Preview")
                    {

                        if (Preview == 0)
                        {
                            if (Angle == 0)
                            {
                                Preview++;
                                data.Grid = 0;
                                Cmd_90(data.SelectPosition());
                            }
                            else
                            {
                                MessageBox.Show("리셋후 작동하세요.");
                            }
                        }
                        else if (Preview == 1)
                        {
                            data.Grid = 1;
                            Preview++;
                        }
                        else if (Preview == 2)
                        {
                            Cmd_r90(data.SelectPosition());
                            Preview++;
                        }
                        if (Preview == 3)
                        {
                            Preview = 0;
                        }

                    }
                }
                else
                {
                    MessageBox.Show("동작중.");
                }
            }
            else
            {
                if (Cmd_ErrCheck() == "16.0")
                {
                    MessageBox.Show($"ErrCode : {Cmd_ErrCheck()}(과부화 발생)");
                }
                else if (Cmd_ErrCheck() == "94.0")
                {
                    MessageBox.Show($"ErrCode : {Cmd_ErrCheck()}(블록동작이상)");
                }
            }*/
        }
        public int Cmd(string cmd, ImageProcessing img,Data data)
        {
            if (MoveState())
            {
                if (cmd == "Preview")
                {
                    if (Preview == 0)
                    {
                        if (Angle == 0)
                        {
                            Preview++;
                            if (img.GridGet() == 0)
                            {
                                img.GridVal();
                            }
                        }
                        else
                        {
                            MessageBox.Show("리셋후 작동하세요.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else if (Preview == 1)
                    {
                        if (MessageBox.Show("모터를 작동시키겠습니까?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            timer.Start();
                            Cmd_90(data.SelectPosition());
                            Preview++;
                        }
                        else
                        {

                        }
                    }
                    else if (Preview == 2)
                    {
                        timer.Start();
                        if (img.GridGet() == 1)
                        {
                            img.GridVal();
                        }
                        Cmd_r90(data.SelectPosition());
                        Preview++;
                    }
                    if (Preview == 3)
                    {
                        Preview = 0;
                    }

                }

            }
            else
            {
                MessageBox.Show("모터가 동작 중입니다.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            /*if(Cmd_ErrCheck() == "0.0")
            {
                if (MoveState())
                {
                    if (cmd == "Preview")
                    {

                        if (Preview == 0)
                        {
                            if (Angle == 0)
                            {
                                Preview++;
                                if (img.GridGet() == 0)
                                {
                                    img.GridVal();
                                }
                            }
                            else
                            {
                                MessageBox.Show("리셋후 작동하세요.");
                            }
                        }
                        else if (Preview == 1)
                        {
                            if (MessageBox.Show("모터를 작동시키겠습니까?", "Yes-No", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                Cmd_90(data.SelectPosition());
                                Preview++;
                            }
                            else
                            {

                            }
                        }
                        else if (Preview == 2)
                        {
                            if (img.GridGet() == 1)
                            {
                                img.GridVal();
                            }
                            Cmd_r90(data.SelectPosition());
                            Preview++;
                        }
                        if (Preview == 3)
                        {
                            Preview = 0;
                        }

                    }

                }
                else
                {
                    MessageBox.Show("동작중.");
                }
            }
            else
            {
                if(Cmd_ErrCheck() == "16.0")
                {
                    MessageBox.Show($"ErrCode : {Cmd_ErrCheck()} 과부화 발생");
                }
                else if(Cmd_ErrCheck() == "94.0")
                {
                    MessageBox.Show($"ErrCode : {Cmd_ErrCheck()} 동작이상");
                }
            }*/
            return Preview;
        }
        private void Cmd_Reset(string SelectPosition)
        {
            Angle = 0;
            PanasonicServoCmd(Reset);
            PanasonicServoCmd(TestCheck0);
            PanasonicServoCmd(TestCheck1);
            PositionLabelValue(SelectPosition);
        }
        public void Cmd_22_5(string SelectPosition)
        {
            Angle = Angle + 22.5;
            PanasonicServoCmd(Servo_22_5);
            PanasonicServoCmd(TestCheck0);
            PanasonicServoCmd(TestCheck1);
            PositionLabelValue(SelectPosition);
        }
        public void Cmd_45(string SelectPosition)
        {
            Angle = Angle + 45;
            PanasonicServoCmd(Servo_45);
            PanasonicServoCmd(TestCheck0);
            PanasonicServoCmd(TestCheck1);
            PositionLabelValue(SelectPosition);
        }
        public void Cmd_90(string SelectPosition)
        {
            Angle = Angle + 90;
            PanasonicServoCmd(Servo_90);
            PanasonicServoCmd(TestCheck0);
            PanasonicServoCmd(TestCheck1);
            PositionLabelValue(SelectPosition);
        }
        public void Cmd_360(string SelectPosition)
        {
            Angle = Angle + 360;
            PanasonicServoCmd(Servo_360);
            PanasonicServoCmd(TestCheck0);
            PanasonicServoCmd(TestCheck1);
            PositionLabelValue(SelectPosition);
        }
        public void Cmd_r22_5(string SelectPosition)
        {
            Angle = Angle - 22.5;
            PanasonicServoCmd(Servo_r22_5);
            PanasonicServoCmd(TestCheck0);
            PanasonicServoCmd(TestCheck1);
            PositionLabelValue(SelectPosition);
        }
        public void Cmd_r45(string SelectPosition)
        {
            Angle = Angle - 45;
            PanasonicServoCmd(Servo_r45);
            PanasonicServoCmd(TestCheck0);
            PanasonicServoCmd(TestCheck1);
            PositionLabelValue(SelectPosition);
        }
        public void Cmd_r90(string SelectPosition)
        {
            Angle = Angle - 90;
            PanasonicServoCmd(Servo_r90);
            PanasonicServoCmd(TestCheck0);
            PanasonicServoCmd(TestCheck1);
            PositionLabelValue(SelectPosition);
        }
        public void Cmd_r360(string SelectPosition)
        {
            Angle = Angle - 360;
            PanasonicServoCmd(Servo_r360);
            PanasonicServoCmd(TestCheck0);
            PanasonicServoCmd(TestCheck1);
            PositionLabelValue(SelectPosition);
        }
        public string Cmd_Srdy()
        {
            return PanasonicServoCmd(Srdy);
        }
        public string Cmd_ALM()
        {
            return PanasonicServoCmd(Aml);
        }
        public string Cmd_Dbrk()
        {
            return PanasonicServoCmd(Dbrk);
        }
        public string Cmd_BrkOff()
        {
            return PanasonicServoCmd(Brkoff);
        }
        public string Cmd_TTD()
        {
            return PanasonicServoCmd(TT);
        }
        public void Cmd_AlarmReset()
        {
            PanasonicServoCmd(AlarmReset);
            Thread.Sleep(200);
            Cmd_SvoOn();
            Thread.Sleep(200);
            PanasonicServoCmd(AlarmReset2);
            

        }
        public string Cmd_ErrCheck()
        {
            string hex = PanasonicServoCmd(errcode);
            string err1 = hex.Substring(9, 2);
            string err2 = hex.Substring(12, 2);
            double hex_10err1 = Convert.ToInt32(err1, 16);
            double hex_10err2 = Convert.ToInt32(err2, 16);
            string errcheck = $"{hex_10err1}.{hex_10err2}";
            return errcheck;
        }
        public void Cmd_SvoOn()
        {
            PanasonicServoCmd(SvoOn);
        }
        public void Cmd_SvoOff()
        {
            PanasonicServoCmd(SvoOff);
        }
        private string PanasonicServoCmd(byte[] cmd)
        {
            if (serial.IsOpen)
            {
                serial.Write(cmd, 0, cmd.Length);
                DateTime dt = DateTime.Now;
                Thread.Sleep(30);
                while (serial.BytesToRead == 0)
                {
                    Thread.Sleep(10);
                    if (DateTime.Now.Subtract(dt).TotalMilliseconds > 1000)
                    {
                        Console.WriteLine("Time Out Error");
                        return "0";
                    }
                }
            }
            Thread.Sleep(20);
            try
            {
                byte[] ReceveData = new byte[serial.BytesToRead];
                serial.Read(ReceveData, 0, ReceveData.Length);
                serial.DiscardInBuffer();
                //string re = Encoding.Default.GetString(ReceveData) + "\r\n";
                string re = ByteToHexString(ReceveData);
                return re;
            }
            catch
            {
                return "0";
            }
        }
        private string ByteToHexString(byte[] input)
        {
            //return Convert.ToHexString(input);
            return BitConverter.ToString(input);
        }

    }
}
