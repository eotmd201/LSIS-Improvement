using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS
{
    public interface ILoginStrategy
    {
        string Login(string userId, string password);
    }

    public class MasterLoginStrategy : ILoginStrategy
    {
        public string Login(string userId, string password)
        {
            // 관리자 ID로 로그인 검증 로직 구현
            if(userId == "s-onebio" && password == "1123")
            {
                return "Master";
            }
            else
            {
                return "false";
            }
        }
    }

    public class AccountCheckLoginStrategy : ILoginStrategy
    {
        DB db;
        public AccountCheckLoginStrategy(DB db)
        {
            this.db = db;
        }
        public string Login(string userId, string password)
        {
            string sql = "SELECT * FROM DB_Account";
            SQLiteDataReader rdr = db.Load(sql);
            List<string> Login = new List<string>();
            while (rdr.Read())
            {
                Login.Add(rdr["ID"].ToString());
                Login.Add(rdr["Password"].ToString());
            }
            rdr.Close();

            for (int i = 0; i < Login.Count; i += 2)
            {
                if (Login[i] == userId && Login[i + 1] == password)
                {
                    if (i == 0)
                    {
                        if (Login[0] == "admin" && Login[1] == "1234")
                        {
                            return "First";
                        }
                        else
                        {
                            return "Manager";
                        }
                    }
                    else
                    {
                        return "User";
                    }
                }
            }
            return "false";
        }
    }
}
