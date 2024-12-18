using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS
{
    public class Data
    {
        public string Name { get; set; }
        public string HID { get; set; }
        public string SID { get; set; }
        public string Comment { get; set; }

        public string Num { get; set; }
        public string Time { get; set; }
        public string Section_Time { get; set; }
        public string Section_Dist { get; set; }
        public string Section_Speed { get; set; }
        public string total_Time { get; set; }
        public string total_Dist { get; set; }
        public string total_Speed { get; set; }
        public string ID { get; set; }
        public string Password { get; set; }

        public int Circumference_Interval { get; set; }
        public int Grid { get; set; }
        public int View_Range { get; set; }
        public int Extended_Shot { get; set; }
        public int Auto_Rotation_Angle { get; set; }
        public int Manual_Rotation_Angle { get; set; }
        public int Exposure_Time { get; set; }
        public int Gain { get; set; }
        public int Gamma { get; set; }
        public int Position_Now { get; set; }
        public string Position { get; set; }
        public string Position1 { get; set; }
        public string Position2 { get; set; }
        public int L1 { get; set; }
        public int L2 { get; set; }
        public int C1 { get; set; }
        public int C2 { get; set; }
        public string Position_Chose { get; set; }
        public int SliderVal { get; set; }
        public string Date { get; set; }
        public string Shot_Time { get; set; }
        public string Mode { get; set; }
        public string Sequences { get; set; }
        public string Address { get; set; }
        public bool ScanDataCheck { get; set; }

        public int UPDOWN { get; set; }
        public int PUSHPULL { get; set; }
        public int LED { get; set; }
        public int LED2 { get; set; }
        public int Laser { get; set; }
        public int Contour { get; set; }
        public int ColorMap { get; set; }
        public int flip { get; set; }

        public int Filter { get; set; }
        //public string SelectName { get; set; }
        //public string SelectHID { get; set; }
        //public string SelectSID { get; set; }
        //public string SelectSex { get; set; }
        //public string SelectAge { get; set; }



        public string shotposition { get; set; }

        public string Imagetemp { get; set; }
        public string ImageView1 { get; set; }
        public string ImageView2 { get; set; }
        public string ImageView3 { get; set; }
        public string ImageView4 { get; set; }

        public int ReviewGrid { get; set; }

        public int Screen { get; set; }

        public int startXPos { get; set; }
        public int endXPos { get; set; }
        public int brighteness_Limit { get; set; }
        public string FirstImage { get; set; }

        public List<double> Angle { get; set; } = new List<double>();

        public int View { get; set; }

        public int Marker { get; set; }

        public string ReportSelectDate { get; set; }
        public string ReportSelectPostion { get; set; }
        public string ReportSelectStart_Time { get; set; }
        public string ReportSelectShot_Time { get; set; }

        public string SelectPosition()
        {
            string SelecPostion;
            if (Position_Now == 1)
            {
                SelecPostion = Position1;
            }
            else if (Position_Now == 2)
            {
                SelecPostion = Position2;
            }
            else
            {
                SelecPostion = "";
            }
            if(SelecPostion != null)
            {
                SelecPostion = SelecPostion.Substring(SelecPostion.IndexOf("_") + 1, SelecPostion.Length - SelecPostion.IndexOf("_") - 1);
            }
            if (SelecPostion == "Others")
            {
                SelecPostion = "";
            }
            return SelecPostion;
        }
        public List<double> GetCameraVal()
        {
            List<double> cameraval = new List<double>();
            cameraval.Add(Exposure_Time * 100000 + 100000);
            cameraval.Add(Gain * 3);
            cameraval.Add(Gamma * 0.1 + 0.3);

            return cameraval;
        }
    }
}
