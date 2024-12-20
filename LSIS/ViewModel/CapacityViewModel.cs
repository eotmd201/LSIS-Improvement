using LSIS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace LSIS.ViewModel
{
    public class CapacityViewModel : BaseViewModel
    {
        public event Action<string> MessageRequested;
        private Timer _timer;
        private readonly CapacityService _capacityService;

        private bool _driveCheck;
        private long _freeSpaceInGB;
        private bool _isWarningShown = false; // 경고창 표시 여부 추적

        public CapacityViewModel()
        {
            _capacityService = new CapacityService();
            // 주기적 체크 시작
            StartMonitoring();
        }

        private void StartMonitoring()
        {
            _timer = new Timer(60000); // 1분 간격
            _timer.Elapsed += (s, e) => CheckDrive();
            _timer.Start();
        }
        public void StopMonitoring()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // 가비지 컬렉터에서 Finalizer 호출 방지
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 타이머 해제
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }
        public void InitializeDriveStatus()
        {
            try
            {
                double freeSpace = _capacityService.CheckCapacity();

                if (_capacityService.IsBelowThreshold())
                {
                    DriveCheck = false;
                    if (!_isWarningShown)
                    {
                        MessageRequested?.Invoke($"드라이브 사용 가능 공간이 {freeSpace} GB 남았습니다.");
                        _isWarningShown = true;
                    }
                }
                else
                {
                    DriveCheck = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"드라이브 상태 초기화 중 오류: {ex.Message}");
            }
        }
        private void CheckDrive()
        {
            try
            {
                // 서비스에서 상태 확인
                DriveCheck = !_capacityService.IsBelowThreshold(); // 10GB 미만이면 false
            }
            catch (Exception ex)
            {
                Console.WriteLine($"드라이브 상태 업데이트 중 오류: {ex.Message}");
                DriveCheck = false; // 오류 시 기본값 처리
            }
        }
        public bool DriveCheck
        {
            get => _driveCheck;
            set
            {
                _driveCheck = value;
                OnPropertyChanged();
            }
        }

        public long FreeSpaceInGB
        {
            get => _freeSpaceInGB;
            set
            {
                _freeSpaceInGB = value;
                OnPropertyChanged();
            }
        }
    }
}
