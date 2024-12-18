using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS
{
    public class DB
    {
        public SQLiteConnection conn { get; private set; }
        public DB()
        {
            //데이터 베이스 생성
            string DBfile = "LSIS.sqlite";
            conn = new SQLiteConnection("Data Source=LSIS.sqlite;Version=3;");
            if (!System.IO.File.Exists(DBfile))
            {
                SQLiteConnection.CreateFile("LSIS.sqlite");

            }
            conn.Open();
            ForeignEnable();
            Create_DB_Account();
            Create_DB_Patient();
            Create_DB_ShotSave();
            Create_DB_Setting();
            Create_DB_Report();
            Create_DB_Master();
            Create_DB_Injection();
            //Create_DB_Video();

        }
        private void ForeignEnable()
        {
            string sql = "PRAGMA foreign_keys = ON";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader cnt = cmd.ExecuteReader();
        }
        private void Create_DB_Account()
        {
            //DB_Account 테이블 생성
            string sql = "create table if not exists DB_Account (ID string,Password char)";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();

            //DB_Account 기본값 입력 카운터로 DB_Account 값 존재 여부 확인후 값이 존재 하지않을때 기본값 대입
            sql = "INSERT INTO DB_Account (ID, Password) SELECT 'admin', '1234' WHERE(SELECT COUNT(*) FROM DB_Account) = 0";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
        }
        private void Create_DB_Patient()
        {
            //DB_Patient 테이블 생성
            string sql = "create table if not exists DB_Patient (Name to_char, HID to_char PRIMARY KEY, Birthday to_char, Age to_char, Sex to_char, Comment to_char, CreationDate to_char)";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
        }
        private void Create_DB_ShotSave()
        {
            //DB_ShotSave 테이블 생성
            string sql = "create table if not exists DB_ShotSave (HID to_char, Position to_char, Date to_char, Shot_Time to_char,Mode to_char,Sequences to_char,ImageNum to_char,CameraSet to_char,Comment to_char, FileName to_char GENERATED ALWAYS AS (HID || '_' || Date || Position || '(#' || Sequences || ')(' || ImageNum || ')') STORED,UNIQUE(HID,Position,Date,Mode,Sequences,ImageNum),  FOREIGN KEY(HID) REFERENCES DB_Patient(HID) ON UPDATE CASCADE ON DELETE CASCADE)";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
        }
        private void Create_DB_Report()
        {
            //DB_ShotSave 테이블 생성
            string sql = "create table if not exists DB_Report (HID to_char, Position to_char, Date to_char, Shot_Time to_char,Sequences to_char,ImageNum to_char,FileName to_char GENERATED ALWAYS AS ('Report_'||HID || '_' || Date || Position || '(#' || Sequences || ')(' || ImageNum || ')') STORED,UNIQUE(HID,Position,Date,Sequences,ImageNum),  FOREIGN KEY(HID) REFERENCES DB_Patient(HID) ON UPDATE CASCADE ON DELETE CASCADE)";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
        }
        private void Create_DB_Setting()
        {
            //DB_ShotSave 테이블 생성
            string sql = "create table if not exists DB_Setting (Circumference_Interval int, Grid int, View_Range int, Extended_Shot int, Auto_Rotation_Angle int, Manual_Rotation_Angle int, Exposure_Time int, Gain int, Gamma int,Filter int)";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();

            //DB_Setting 기본값 입력 카운터로 DB_Setting 값 존재 여부 확인후 값이 존재 하지않을때 기본값 대입
            sql = "INSERT INTO DB_Setting(Circumference_Interval, Grid, View_Range, Extended_Shot, Auto_Rotation_Angle, Manual_Rotation_Angle, Exposure_Time, Gain, Gamma, Filter) SELECT 0, 1, 0, 0, 1, 1, 0, 9, 7, 0 WHERE (SELECT COUNT(*) FROM DB_Setting WHERE Circumference_Interval = 0) = 0";
            Console.WriteLine(sql);
            command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
        }
        private void Create_DB_Master()
        {
            //DB_Account 테이블 생성
            string sql = "create table if not exists DB_Master (SerialNum to_char,Validity_Period to_char,AET to_char, IP to_char, Port to_char)";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader cnt = cmd.ExecuteReader();
        }
        private void Create_DB_Injection()
        {
            //DB_Account 테이블 생성
            string sql = "create table if not exists DB_Injection (HID to_char,Date to_char, Injection_Time to_char, FOREIGN KEY (HID) REFERENCES DB_Patient(HID) ON UPDATE CASCADE ON DELETE CASCADE)";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader cnt = cmd.ExecuteReader();
        }
        /*private void Create_DB_Video()
        {
            string sql = "create table if not exists DB_Video (HID to_char, Position to_char, Date to_char, Shot_Time to_char,Sequences to_char, FileName to_char GENERATED ALWAYS AS ('Video_'||HID || '_' || Date || Position || '(' || Sequences || ')') STORED,UNIQUE(HID,Position,Date,Sequences),  FOREIGN KEY(HID) REFERENCES DB_Patient(HID) ON UPDATE CASCADE ON DELETE CASCADE)";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader cnt = cmd.ExecuteReader();
        }*/
        public void Save(string sql)
        {
            //예제1 string sql = "insert into DB_Report values('" + data.SelectHID + "','" + data.ReportSelectDate + "','" + data.ReportSelectPostion + "')";
            //예제2 업데이트 string sql = "update DB_ShotSave set HID='" + ReHID + "'where HID='" + HID + "'";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
        }
        public SQLiteDataReader Load(string sql)
        {
            //예제1 일반로드 string sql =  "SELECT * FROM DB_Patient";
            //예제2 조건로드 string sql =  "SELECT * FROM DB_Patient where Name='" + Name + "'and HID='" + HID + "'and SID='" + SID + "'and Comment='" + Comment + "'";
            //예제3 오름차순 로드 SELECT *FROM DB_Patient ORDER BY SelectDate asc ; 내림차순 : desc
            //예제3 조건 오름차순 로드 SELECT *FROM (SELECT * FROM DB_Report Where HID='220330165') ORDER BY SelectDate asc ; 내림차순 : desc
            /* 사용예제
            while (rdr.Read())
            {
                pat_items.Insert(0, new Data()
                {
                    Name = rdr["Name"].ToString(),
                    HID = rdr["HID"].ToString(),
                    SID = rdr["SID"].ToString(),
                    Comment = rdr["Comment"].ToString()
                });
            }*/

            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            return rdr;
        }
    }
}
