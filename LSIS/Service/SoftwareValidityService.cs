using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS.Service
{
    public class SoftwareValidityService
    {
        /// <summary>
        /// 소프트웨어 시리얼 확인 서비스 클래스
        /// </summary>
        private readonly DB _db;
        public SoftwareValidityService(DB db)
        {
            _db = db;
        }

        public bool IsSoftwareExpired(out DateTime validityPeriod)
        {
            validityPeriod = DateTime.MinValue;
            string sql = "SELECT Validity_Period FROM DB_Master LIMIT 1";

            using (var reader = _db.Load(sql))
            {
                while (reader.Read())
                {
                    string dateTimeString = reader["Validity_Period"].ToString();
                    if (DateTime.TryParseExact(
                        dateTimeString,
                        "yyyy-MM-dd tt h:mm:ss",
                        new CultureInfo("ko-KR"),
                        DateTimeStyles.None,
                        out validityPeriod))
                    {
                        return validityPeriod <= DateTime.Now;
                    }
                }
            }
            return true; // 유효기간이 없으면 만료된 것으로 간주
        }
    }
}
