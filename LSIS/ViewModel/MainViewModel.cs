using LSIS.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LSIS.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public event Action<string> ShowMessageEvent; // 메시지 표시 이벤트
        public TabControlViewModel TabControlViewModel { get; }
        public LoginViewModel LoginViewModel { get; }
        public CapacityViewModel CapacityViewModel { get; }
        public VersionViewModel VersionViewModel { get; }
        private readonly DB _db;

        
        public MainViewModel(Login login, DB db, Equipment divice, SoftwareValidityService validityService)
        {
            VersionViewModel = new VersionViewModel();
            TabControlViewModel = new TabControlViewModel();
            CapacityViewModel = new CapacityViewModel();
            LoginViewModel = new LoginViewModel(login, db, divice, validityService, TabControlViewModel);
            CapacityViewModel.MessageRequested += TriggerShowMessage;
            LoginViewModel.MessageRequested += TriggerShowMessage;
            _db = db;
            TabControlViewModel.MoveToTab("로그인");
        }

        // 이벤트를 호출하는 헬퍼 메서드
        protected void TriggerShowMessage(string message)
        {
            ShowMessageEvent?.Invoke(message);
        }
        ///*** 마스터 모드 ***///

        /*
        private Ver _ver;
        DB db;

        public MainViewModel(DB db)
        {
            _ver = new Ver();
            _ver.PropertyChanged += Ver_PropertyChanged;
            this.db = db;
            Accounts = AccountDataService.Instance(db).LoadAccounts();
            RefreshAccounts();
        }


        private ObservableCollection<Account> _accounts;
        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
            set
            {
                if (_accounts != value)
                {
                    _accounts = value;
                    OnPropertyChanged(nameof(Accounts));
                }
            }
        }

        public void RefreshAccounts()
        {
            var newAccounts = AccountDataService.Instance(db).LoadAccounts();
            Accounts = new ObservableCollection<Account>(newAccounts);
        }

        //버전 UI 처리
        public string Version => _ver.Version;
        public event PropertyChangedEventHandler PropertyChanged;

        private void Ver_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Ver.Version))
            {
                OnPropertyChanged(nameof(Version));
            }
        }

        //TabTable SelectedIndex 변경에따른 UI 처리
        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (_selectedTabIndex != value)
                {
                    _selectedTabIndex = value;
                    OnPropertyChanged(nameof(SelectedTabIndex));
                }
            }
        }

        //ListView Selected Accout 변경에 따른 UI 처리
        private Account _selectedAccount;
        public Account SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                OnPropertyChanged(nameof(SelectedAccount));
            }
        }

        // 항목 추가 메서드
        public void AddAccount(Account newAccount)
        {
            Accounts.Add(newAccount);
        }

        // 선택된 항목 수정 메서드
        public void UpdateSelectedAccount(Account updatedAccount)
        {
            if (SelectedAccount != null)
            {
                int index = Accounts.IndexOf(SelectedAccount);
                if (index != -1)
                {
                    Accounts[index] = updatedAccount;
                }
            }
        }
        // 선택된 항목 삭제 메서드
        public void DeleteSelectedAccount()
        {
            if (SelectedAccount != null)
            {
                Accounts.Remove(SelectedAccount);
                SelectedAccount = null; // 선택 상태 초기화
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
         */
    }
}
