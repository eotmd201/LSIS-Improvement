using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LSIS
{
    public class Login
    {
        public bool MasterCheck(string id, string password)
        {
            if(id == "s-onebio" && password == "1123")
            {
                return true;
            }
            else
            {
                return false;
            }
        } 
        public string AccountCheck(DB db, string id, string password)
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
                if (Login[i] == id && Login[i + 1] == password)
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
