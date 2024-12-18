using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LSIS.ViewModel
{
    public class TabControlViewModel : BaseViewModel
    {
        public void MoveToTab(string headerName)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                switch (headerName)
                {
                    case "로그인":
                        SelectedIndex = 0;
                        break;
                    case "계정 관리":
                        SelectedIndex = 1;
                        break;
                    case "환자 관리":
                        SelectedIndex = 2;
                        break;
                    case "촬영 관리":
                        SelectedIndex = 3;
                        break;
                    case "이미지 뷰어 리스트":
                        SelectedIndex = 4;
                        break;
                    case "이미지 뷰어":
                        SelectedIndex = 5;
                        break;
                    case "비디오 뷰어 리스트":
                        SelectedIndex = 6;
                        break;
                    case "비디오 뷰어":
                        SelectedIndex = 7;
                        break;
                    case "리포트 이미지 편집":
                        SelectedIndex = 8;
                        break;
                    case "리포트 작성":
                        SelectedIndex = 9;
                        break;
                    case "리포트 뷰어":
                        SelectedIndex = 10;
                        break;
                    case "마스터 모드":
                        SelectedIndex = 11;
                        break;
                }
            });
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }
    }
}
