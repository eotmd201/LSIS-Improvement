using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LSIS
{
    public struct ReportDateList
    {
        public string date;
        public List<BitmapImage> reportimage;

        public ReportDateList(string date, List<BitmapImage> reportimage)
        {
            this.date = date;
            this.reportimage = reportimage;
        }
    }
}
