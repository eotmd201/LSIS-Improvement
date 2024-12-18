using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LSIS
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        Mutex mutex = null;
        public App()
        {
            // 어플리케이션 이름 확인
            string applicationName = Process.GetCurrentProcess().ProcessName;
            Duplicate_execution(applicationName);

            // UI 스레드 예외 핸들러
            this.DispatcherUnhandledException += OnDispatcherUnhandledException;

            // 백그라운드 스레드 예외 핸들러
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
        }
        private void Duplicate_execution(string mutexName)
        {
            try
            {
                mutex = new Mutex(false, mutexName);
            }
            catch (Exception ex)
            {
                Application.Current.Shutdown();
            }
            if (mutex.WaitOne(0, false))
            {
                InitializeComponent();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // 로그 기록
            LogException(e.Exception);
            e.Handled = true;
        }

        private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // 로그 기록
            LogException(e.ExceptionObject as Exception);
        }

        private void LogException(Exception ex)
        {
            if (ex != null)
            {
                // 여기에 로그 기록 로직을 구현합니다.
                // 예: 파일에 기록, 로깅 라이브러리 사용 등
                File.AppendAllText("app.txt", ex.ToString());
            }
        }
    }
}
