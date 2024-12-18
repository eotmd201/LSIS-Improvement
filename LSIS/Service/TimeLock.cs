using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LSIS
{
    public class TimeLock
    {
        DispatcherTimer inactivityTimer = new DispatcherTimer();
        DB db;
        MainWindow main;
        public TimeLock(DB db,MainWindow main)
        {
            inactivityTimer.Interval = TimeSpan.FromMinutes(60);
            inactivityTimer.Tick += InactivityTimer_Tick;
            this.db = db;
            this.main = main;
        }

        public void Start()
        {
            inactivityTimer.Start();
        }
        public void Stop()
        {
            if(inactivityTimer.IsEnabled)
            {
                inactivityTimer.Stop();
            }
        }
        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            main.LED_Off();
            LoginWindow login_Window = new LoginWindow(db,true);
            login_Window.ShowDialog();
        }
        public void UserActivityDetected(object sender, EventArgs e)
        {
            // 사용자의 활동이 감지되면 타이머를 재설정합니다.
            inactivityTimer.Stop();
            inactivityTimer.Start();
        }

    }

    public class EventSubscriber
    {
        private TimeLock _timeLock;

        public EventSubscriber(Window window, TimeLock timeLock)
        {
            _timeLock = timeLock;

            // 이벤트 구독
            window.MouseMove += UserActivityDetected;
            window.KeyDown += UserActivityDetected;
        }

        private void UserActivityDetected(object sender, EventArgs e)
        {
            _timeLock.UserActivityDetected(sender, e);
        }
    }
}
