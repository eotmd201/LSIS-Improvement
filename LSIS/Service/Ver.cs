using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS
{
    class Ver
    {
        public string _version;
        public string _buildDate;

        public Ver()
        {
            // Assembly 정보에서 버전 정보를 가져옵니다.
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            _version = $"버전 {version}";

            // 빌드 날짜를 계산합니다. (빌드 날짜를 포함하려는 경우)
            DateTime buildDate = new DateTime(2000, 1, 1)
                                 .AddDays(version.Build)
                                 .AddSeconds(version.Revision * 2);
            _buildDate = buildDate.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public string Version
        {
            get => _version;
            private set
            {
                _version = value;
                OnPropertyChanged(nameof(Version));
            }
        }
        public string BuildDate
        {
            get => _buildDate;
            set
            {
                _buildDate = value;
                OnPropertyChanged(nameof(BuildDate));
            }
        }

        // INotifyPropertyChanged 구현
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
