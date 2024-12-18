using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS
{
    public struct strutList
    {
        public List<double> listval;
        public string date;

        public strutList(List<double> listval, string date)
        {
            this.listval = listval;
            this.date = date;
        }

    }
    class ChartList
    {
        private List<double> chartval = new List<double>();
        private List<strutList> objectList = new List<strutList>();
        private MainWindow main;
        public ChartList(MainWindow main)
        {
            this.main = main;
        }
        public void Add(string path)
        {
            var m_pDicomFile = DicomFile.Open(path);
            DicomDataset ds = m_pDicomFile.Dataset;
            List<double> recent = new List<double> {
                double.Parse(ds.GetString(new DicomTag(0x0009, 0x0010))),
                double.Parse(ds.GetString(new DicomTag(0x0009, 0x0011))),
                double.Parse(ds.GetString(new DicomTag(0x0009, 0x0012))),
                double.Parse(ds.GetString(new DicomTag(0x0009, 0x0013))),
                double.Parse(ds.GetString(new DicomTag(0x0009, 0x0014))),
                double.Parse(ds.GetString(new DicomTag(0x0009, 0x0015))),
                double.Parse(ds.GetString(new DicomTag(0x0009, 0x0016))),
                double.Parse(ds.GetString(new DicomTag(0x0009, 0x0017))),
                double.Parse(ds.GetString(new DicomTag(0x0009, 0x0018))),
            };
            for (int idx = 0; idx < recent.Count; idx++)
            {
                recent[idx] = recent[idx] + 1;
            }
            objectList.Add(new strutList(recent, ds.GetString(DicomTag.StudyDate)));
        }
        public void Del()
        {
            if (objectList.Count != 0)
            {
                objectList.RemoveAt(objectList.Count - 1);
            }
        }
        public void Chartval()
        {
            List<double> recent = new List<double> {
                main.A_Hand_Pattern.SelectedIndex,
                main.A_Lower_Pattern.SelectedIndex,
                main.A_Upper_Pattern.SelectedIndex,
                main.P_Lower_Pattern.SelectedIndex,
                main.P_Upper_Pattern.SelectedIndex,
                main.M_Lower_Pattern.SelectedIndex,
                main.M_Upper_Pattern.SelectedIndex,
                main.L_Lower_Pattern.SelectedIndex,
                main.L_Upper_Pattern.SelectedIndex,
            };
            for (int idx = 0; idx < recent.Count; idx++)
            {
                recent[idx] = recent[idx] + 1;
            }
            chartval = recent;
        }
        public List<double> GetChartval()
        {
            return chartval;
        }
        public List<strutList> GetChart()
        {
            return objectList;
        }
        
    }
}
