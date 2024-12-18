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
        public double Check()
        {
            DriveInfo cDrive = new DriveInfo("C");
            if (cDrive.IsReady)
            {
                capacity = (double)cDrive.AvailableFreeSpace / (1024 * 1024 * 1024);
            }
            capacity = Math.Round(capacity, 2);
            return capacity;
        }
    }
}
