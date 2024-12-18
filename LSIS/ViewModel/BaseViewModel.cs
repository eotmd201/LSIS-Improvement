using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LSIS.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<string> ShowMessageEvent; // 메시지 표시 이벤트
        public event Action OpenManagerWindowEvent; // Manager 창 열기 이벤트
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 이벤트를 호출하는 헬퍼 메서드
        protected void TriggerShowMessage(string message)
        {
            ShowMessageEvent?.Invoke(message);
        }

        protected void TriggerOpenManagerWindow()
        {
            OpenManagerWindowEvent?.Invoke();
        }
    }
}
