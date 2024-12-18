using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LSIS
{
    class Reviewtest : INotifyPropertyChanged
    {
        private System.Windows.Media.ImageSource _image;
        public System.Windows.Media.ImageSource Image
        {
            get { return _image;}
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void LoadImage(Mat mat)
        {
            Image = WriteableBitmapConverter.ToWriteableBitmap(mat);
        }
    }
}
