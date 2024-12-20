using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS.Service
{
    /// <summary>
    ///  용량 체크 서비스 클래스
    /// </summary>
    class CapacityService
    {
        double capacity;
        public double CheckCapacity()
        {
            DriveInfo cDrive = new DriveInfo("C");
            if (cDrive.IsReady)
            {
                capacity = (double)cDrive.AvailableFreeSpace / (1024 * 1024 * 1024);
            }
            capacity = Math.Round(capacity, 2);
            return capacity;
        }
        // 10GB 미만인지 여부 확인 메서드
        public bool IsBelowThreshold(double threshold = 10)
        {
            // 현재 용량 확인
            CheckCapacity();

            // threshold 값과 비교
            return capacity < threshold;
        }

    }
}
