using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS
{
    class Sensor
    {
        public string Mode { get; set;}
        public string Sensor1 { get; set; }
        public string Sensor2 { get; set; }
        public string GetMode()
        {
            return Mode;
        }
        public string GetSensor1()
        {
            return Sensor1;
        }
        public string GetSensor2()
        {
            return Sensor2;
        }
    }
}
