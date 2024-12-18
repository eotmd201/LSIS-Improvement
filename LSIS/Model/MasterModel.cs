using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS.Model
{
    public class MasterModel
    {
        // DB에서 가져오는 데이터
        public string SerialNumber { get; set; }
        public string ManagerID { get; set; }
        public string ManagerPassword { get; set; }
        public string AET { get; set; }
        public string IP { get; set; }
        public string Port { get; set; }
        public string Mode { get; set; }

        // PCB에서 가져오는 데이터
        public string Sensor1 { get; set; }
        public string Sensor2 { get; set; }
        public string Sensor3 { get; set; }
        public string Sensor4 { get; set; }
    }
}
