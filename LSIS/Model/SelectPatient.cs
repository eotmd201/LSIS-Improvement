using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS
{
    public class SelectPatient
    {
        public Mat[][] selctsource { get; set; }
        public string SelectName { get; set; }
        public string SelectHID { get; set; }
        public string SelectBirthday { get; set; }
        public string SelectAge { get; set; }
        public string SelectSex { get; set; }
        public string[][] filename { get; set; }
        public string Viewdata1 { get; set; }
        public string[][] Viewdata2 { get; set; }
        public string[][] Viewdata3 { get; set; }
        public string[][] Viewdata4 { get; set; }
        public string SelectInjectionTime { get; set; }
    }
}
