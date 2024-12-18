using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS
{
    class AccountDataService
    {
        private static AccountDataService _instance;
        private DB db;
        private AccountDataService(DB db)
        {
            this.db = db;
        }

        public static AccountDataService Instance(DB db)
        {
            if (_instance == null)
            {
                _instance = new AccountDataService(db);
            }
            return _instance;
        }

        public ObservableCollection<Account> LoadAccounts()
        {
            string sql = "SELECT * FROM DB_Account LIMIT (Select COUNT(*)-1 cnt FROM DB_Account) OFFSET 1";
            SQLiteDataReader rdr = db.Load(sql);
            List<Account> accounts = new List<Account>();
            while (rdr.Read())
            {
                var account = new Account
                {
                    // SQLiteDataReader에서 데이터를 읽어 Account 객체에 할당
                    ID = rdr["ID"].ToString(),
                    Password = rdr["Password"].ToString()
                    // 필요한 다른 필드들을 여기에 추가합니다.
                };
                accounts.Add(account);
            }
            rdr.Close();
            return new ObservableCollection<Account>(accounts);
        }
    }
}
