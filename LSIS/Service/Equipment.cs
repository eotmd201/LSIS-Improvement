using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS.ViewModel
{
    public class Equipment
    {
        string serialnum;

        public void Load(DB db)
        {
            string sql = "SELECT SerialNum FROM DB_Master LIMIT 1";
            SQLiteDataReader rdr = db.Load(sql);
            while (rdr.Read())
            {
                serialnum = rdr["SerialNum"].ToString();
            }
            rdr.Close();
        }
        public void Load()
        {

        }
        public string GetSerialnum()
        {
            return serialnum;
        }

        public bool SerialCheck()
        {
            if (serialnum != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
