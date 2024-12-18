using LSIS.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LSIS.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly Login _login;
        private readonly TabControlViewModel _tabControlViewModel;
        private readonly Equipment _divice;
        private readonly SoftwareValidityService _validityService;
        private readonly DB _db;
        public ICommand LoginCommand { get; }
        public LoginViewModel(Login login, DB db, Equipment divice, SoftwareValidityService validityService, TabControlViewModel tabControlViewModel)
        {
            _login = login;
            _divice = divice;
            _validityService = validityService;
            _db = db;
            _tabControlViewModel = tabControlViewModel;
            LoginCommand = new RelayCommand(LoginClick);
        }
        private void LoginClick(object parameter)
        {
            if (_login.MasterCheck(Username, Password))
            {
                // 탭 변경 알림
                _tabControlViewModel.MoveToTab("마스터 모드");
                return;
            }
            if (!_divice.SerialCheck())
            {
                TriggerShowMessage("Serial번호를 등록 후 사용하세요.");
                return;
            }
            else
            {
                if (_validityService.IsSoftwareExpired(out DateTime validityPeriod))
                {
                    TriggerShowMessage($"소프트웨어의 유효기간이 지났습니다. ({validityPeriod:yyyy-MM-dd})");
                    return;
                }
                string accountMode = _login.AccountCheck(_db, Username, Password);
                if (accountMode == "First")
                {
                    TriggerOpenManagerWindow();
                }
                else if (accountMode == "Manager")
                {
                    _tabControlViewModel.MoveToTab("계정 관리");
                }
                else if (accountMode == "User")
                {
                    _tabControlViewModel.MoveToTab("환자 관리");
                }
                else
                {
                    TriggerShowMessage("ID와 Password를 확인해주세요.");
                }
            }

        }
        public void ConfirmManagerAction()
        {
            _tabControlViewModel.MoveToTab("계정 관리");
            string sql = $"UPDATE DB_Master SET Validity_Period = '{DateTime.Now.AddYears(1)}'";
            _db.Save(sql);
        }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }


    }
}
