using LSIS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS.Service
{
    public class MasterService
    {
        /// <summary>
        /// 마스터 모드 데이터 서비스 클래스
        /// </summary>
        private readonly DB _db;
        public MasterService(DB db)
        {
            _db = db;
        }

        public MasterModel GetMasterData()
        {
            var model = new MasterModel();

            // SerialNumber 가져오기
            string sql = "SELECT SerialNum, AET, IP, Port FROM DB_Master LIMIT 1";
            using (var reader = _db.Load(sql))
            {
                while (reader.Read())
                {
                    model.SerialNumber = reader["SerialNum"].ToString();
                    model.AET = reader["AET"].ToString();
                    model.IP = reader["IP"].ToString();
                    model.Port = reader["Port"].ToString();
                }
            }

            // ManagerID와 Password 가져오기
            sql = "SELECT ID, Password FROM DB_Account LIMIT 1";
            using (var reader = _db.Load(sql))
            {
                while (reader.Read())
                {
                    model.ManagerID = reader["ID"].ToString();
                    model.ManagerPassword = reader["Password"].ToString();
                }
            }

            return model;
        }
    }
}
