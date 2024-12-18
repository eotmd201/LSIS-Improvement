using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LSIS
{
    internal class PatientData //: IEquatable<PatientData>
    {
        public string Name { get; set; }
        public string HID { get; set; }
        public string SID { get; set; }
        public string Age { get; set; }
        public string Sex { get; set; }
        public string Date { get; set; }
        public string Sequence { get; set; }
        public string ShotMode { get; set; }
    }
    public class Dicom
    {
        List<PatientData> pat_items = new List<PatientData>();
        public void Load(DB db)
        {
            /*string patientInIt = "DELETE FROM DB_Patient";
            db.Save(patientInIt);
            string shotsaveInIt = "DELETE FROM DB_ShotSave";
            db.Save(shotsaveInIt);*/
            string FolderName = @"Dicom";
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(FolderName);
            foreach (System.IO.FileInfo File in di.GetFiles())
            {
                if (File.Extension.ToLower().CompareTo(".dcm") == 0)
                {
                    string FileNameOnly = File.Name.Substring(0, File.Name.Length - 4);
                    string FullFileName = File.FullName;
                    var m_pDicomFile = DicomFile.Open(@"Dicom\" + FileNameOnly + ".dcm");
                    DicomDataset ds = m_pDicomFile.Dataset;
                    string Age = ds.GetString(DicomTag.PatientAge).TrimStart('0').TrimEnd('Y');
                    string patient = "insert or ignore into DB_Patient values('" + ds.GetString(DicomTag.PatientName) + "','" + ds.GetString(DicomTag.PatientID) + "','" + ds.GetString(DicomTag.PatientBirthDate) + "','" + Age + "','" + ds.GetString(DicomTag.PatientSex) + "','" + /*ds.GetString(DicomTag.PatientComments)*/"" + "','" + DateTime.Now.ToString("yyyyMMddHHmmss") + "')";
                    db.Save(patient);
                    double ExposureTime = double.Parse(ds.GetString(new DicomTag(0x0009, 0x0010)))/ 1000000;
                    int GainValue = int.Parse(ds.GetString(new DicomTag(0x0009, 0x0011)));
                    double Gamma = double.Parse(ds.GetString(new DicomTag(0x0009, 0x0012)));
                    string cameraSet = $"{ExposureTime.ToString("0.0")}s/{GainValue}dB/{Gamma.ToString("0.0")}";
                    string comment = "";
                    if (ds.Contains(DicomTag.ImageComments))
                    {
                        comment = ds.GetString(DicomTag.ImageComments);
                    }
                    string shotsave = "insert or ignore into DB_ShotSave values('" + ds.GetString(DicomTag.PatientID) + "','" + ds.GetString(DicomTag.BodyPartExamined) + "','" + ds.GetString(DicomTag.StudyDate) + "','" + ds.GetString(DicomTag.StudyTime) + "','" + ds.GetString(new DicomTag(0x0009, 0x0013)) + "','" + ds.GetString(DicomTag.SeriesNumber) + "','" + ds.GetString(DicomTag.InstanceNumber) + "','" + cameraSet + "','" + comment /*+ "','" + FileNameOnly*/ + "')";
                    db.Save(shotsave);
                    pat_items.Add(new PatientData() { Name = ds.GetString(DicomTag.PatientName), HID = ds.GetString(DicomTag.PatientID), SID = ds.GetString(DicomTag.PatientBirthDate), Age = ds.GetString(DicomTag.PatientAge), Sex = ds.GetString(DicomTag.PatientSex), Date = ds.GetString(DicomTag.StudyDate), Sequence = ds.GetString(DicomTag.SeriesNumber), ShotMode = ds.GetString(new DicomTag(0x0009, 0x0013)) });
                }
            }
            string ReportFolderName = @"Report";
            System.IO.DirectoryInfo di2 = new System.IO.DirectoryInfo(ReportFolderName);
            foreach (System.IO.FileInfo File in di2.GetFiles())
            {
                if (File.Extension.ToLower().CompareTo(".dcm") == 0)
                {
                    string FileNameOnly = File.Name.Substring(0, File.Name.Length - 4);
                    var m_pDicomFile = DicomFile.Open(@"Report\" + FileNameOnly + ".dcm");
                    DicomDataset ds = m_pDicomFile.Dataset;
                    string Age = ds.GetString(DicomTag.PatientAge).TrimStart('0').TrimEnd('Y');
                    string patient = "insert or ignore into DB_Patient values('" + ds.GetString(DicomTag.PatientName) + "','" + ds.GetString(DicomTag.PatientID) + "','" + ds.GetString(DicomTag.PatientBirthDate) + "','" + Age + "','" + ds.GetString(DicomTag.PatientSex) + "','" + /*ds.GetString(DicomTag.PatientComments)*/"" + "','" + DateTime.Now.ToString("yyyyMMddHHmmss") + "')";
                    db.Save(patient);
                    string reportview = "insert or ignore into DB_Report values('" + ds.GetString(DicomTag.PatientID) + "','" + ds.GetString(DicomTag.BodyPartExamined) + "','" + ds.GetString(DicomTag.StudyDate) + "','" + ds.GetString(DicomTag.StudyTime) + "','" + ds.GetString(DicomTag.SeriesNumber) + "','" + ds.GetString(DicomTag.InstanceNumber) /*+ "','" + FileNameOnly*/ + "')";
                    db.Save(reportview);
                    //pat_items.Add(new PatientData() { Name = ds.GetString(DicomTag.PatientName), HID = ds.GetString(DicomTag.PatientID), SID = ds.GetString(DicomTag.PatientBirthDate), Age = ds.GetString(DicomTag.PatientAge), Sex = ds.GetString(DicomTag.PatientSex), Date = ds.GetString(DicomTag.StudyDate), Sequence = ds.GetString(DicomTag.SeriesNumber), ShotMode = ds.GetString(new DicomTag(0x0009, 0x0013)) });
                }
            }

            /*string VidoeFolderName = @"DicomVideo";
            System.IO.DirectoryInfo di3 = new System.IO.DirectoryInfo(VidoeFolderName);
            foreach (System.IO.FileInfo File in di3.GetFiles())
            {
                if (File.Extension.ToLower().CompareTo(".dcm") == 0)
                {
                    string FileNameOnly = File.Name.Substring(0, File.Name.Length - 4);
                    var m_pDicomFile = DicomFile.Open(@"DicomVideo\" + FileNameOnly + ".dcm");
                    DicomDataset ds = m_pDicomFile.Dataset;
                    string Age = ds.GetString(DicomTag.PatientAge).TrimStart('0').TrimEnd('Y');
                    string patient = "insert or ignore into DB_Patient values('" + ds.GetString(DicomTag.PatientName) + "','" + ds.GetString(DicomTag.PatientID) + "','" + ds.GetString(DicomTag.PatientBirthDate) + "','" + Age + "','" + ds.GetString(DicomTag.PatientSex) + "','" + "" + "','" + DateTime.Now.ToString("yyyyMMddHHmmss") + "')";
                    db.Save(patient);
                    string videoview = "insert or ignore into DB_Video values('" + ds.GetString(DicomTag.PatientID) + "','" + ds.GetString(DicomTag.BodyPartExamined) + "','" + ds.GetString(DicomTag.StudyDate) + "','" + ds.GetString(DicomTag.StudyTime) + "','" + ds.GetString(DicomTag.SeriesNumber) + "')";
                    db.Save(videoview);
                }
            }*/

        }
        public void Update(string SelectHID, string UpdateHID,string UpdateName, string UpdateBirthday, string UpdateSex, string UpdateAge)
        {
            string dicomFolderPath = @"Dicom"; // DICOM 파일들이 저장된 폴더 경로
            // 디렉터리에 있는 모든 DICOM 파일 가져오기
            string[] dicomFiles = Directory.GetFiles(dicomFolderPath, "*.dcm");

            foreach (var filePath in dicomFiles)
            {
                // DICOM 파일명에서 패턴을 찾기 (숫자_날짜+위치+순서+번호.dcm)
                Match match = Regex.Match(Path.GetFileName(filePath), @"^(\d+)_(\w+\(#\d+\)\(\d+\))\.dcm$");

                if (match.Success)
                {
                    string hid = match.Groups[1].Value; // DICOM 파일명에서 추출한 HID 값

                    // 선택된 HID와 일치하면 파일명 업데이트
                    if (hid.Equals(SelectHID))
                    {
                        string originalFileName = match.Groups[2].Value; // 날짜+위치+순서+번호.dcm
                        string updatedFileName = $"{UpdateHID}_{originalFileName}.dcm"; // 업데이트된 파일명
                        DicomUpdate(filePath, UpdateHID, updatedFileName, UpdateName, UpdateBirthday, UpdateSex, UpdateAge);
                        // 파일 이동 또는 복사 (원하는 동작 선택)
                        //File.Move(filePath, Path.Combine(dicomFolderPath, updatedFileName)); // 파일 이동
                        //File.Copy(filePath, Path.Combine(dicomFolderPath, updatedFileName)); // 파일 복사

                        Console.WriteLine($"파일 업데이트: {Path.GetFileName(filePath)} -> {updatedFileName}");
                    }
                }
            }
        }
        public void DicomUpdate(string Path,string UpdateHID,string updatedFileName, string UpdateName, string UpdateBirthday, string UpdateSex, string UpdateAge)
        {
            var dicomFile = DicomFile.Open(Path);
            DicomDataset ds = dicomFile.Dataset;
            ds.AddOrUpdate(DicomTag.PatientID, UpdateHID);
            ds.AddOrUpdate(DicomTag.PatientName, UpdateName);
            ds.AddOrUpdate(DicomTag.PatientBirthDate, UpdateBirthday);
            ds.AddOrUpdate(DicomTag.PatientSex, UpdateSex);
            ds.AddOrUpdate(DicomTag.PatientAge, UpdateAge.PadLeft(3, '0') + "Y");

            string[] SOPInstanceUID = ds.GetString(DicomTag.SOPInstanceUID).Split('.');
            string UpdateSOPInstanceUID =
                SOPInstanceUID[0] + "." +
                SOPInstanceUID[1] + "." +
                SOPInstanceUID[2] + "." +
                SOPInstanceUID[3] + "." +
                SOPInstanceUID[4] + "." +
                SOPInstanceUID[5] + "." +
                UpdateHID.TrimStart('0') + "." +
                SOPInstanceUID[7] + "." +
                SOPInstanceUID[8] + "." +
                SOPInstanceUID[9];
            ds.AddOrUpdate(DicomTag.SOPInstanceUID, UpdateSOPInstanceUID);
            string[] StudyInstanceUID =  ds.GetString(DicomTag.StudyInstanceUID).Split('.');
            string UpdateStudyInstanceUID =
                StudyInstanceUID[0] + "." +
                StudyInstanceUID[1] + "." +
                StudyInstanceUID[2] + "." +
                StudyInstanceUID[3] + "." +
                StudyInstanceUID[4] + "." +
                StudyInstanceUID[5] + "." +
                UpdateHID.TrimStart('0') + "." +
                StudyInstanceUID[7];
            ds.AddOrUpdate(DicomTag.StudyInstanceUID, UpdateStudyInstanceUID);
            string[] SeriesInstanceUID = ds.GetString(DicomTag.SeriesInstanceUID).Split('.');
            string UpdateSeriesInstanceUID =
                SeriesInstanceUID[0] + "." +
                SeriesInstanceUID[1] + "." +
                SeriesInstanceUID[2] + "." +
                SeriesInstanceUID[3] + "." +
                SeriesInstanceUID[4] + "." +
                SeriesInstanceUID[5] + "." +
                UpdateHID.TrimStart('0') + "." +
                SeriesInstanceUID[7] + "." +
                SeriesInstanceUID[8];
            ds.AddOrUpdate(DicomTag.SeriesInstanceUID, UpdateSeriesInstanceUID);
            

            DicomFile file = new DicomFile(ds);
            string tempFilePath = @"Dicom\s" + updatedFileName;
            file.Save(tempFilePath);
            File.Delete(Path);
            File.Move(tempFilePath, @"Dicom\"+ updatedFileName);
        }
        public void Delete(string SelectHID)
        {
            string dicomFolderPath = @"Dicom"; // DICOM 파일들이 저장된 폴더 경로


            // 디렉터리에 있는 모든 DICOM 파일 가져오기
            string[] dicomFiles = Directory.GetFiles(dicomFolderPath, "*.dcm");

            foreach (var filePath in dicomFiles)
            {
                // DICOM 파일명에서 패턴을 찾기 (숫자_날짜+위치+순서+번호.dcm)
                Match match = Regex.Match(Path.GetFileName(filePath), @"^(\d+)_(\w+\(#\d+\)\(\d+\))\.dcm$");

                if (match.Success)
                {
                    string hid = match.Groups[1].Value; // DICOM 파일명에서 추출한 HID 값

                    // 선택된 HID와 일치하면 파일명 업데이트
                    if (hid.Equals(SelectHID))
                    {
                        File.Delete(filePath);
                    }
                }
            }

        }
        public void Deletevideo(string SelectHID)
        {
            string dicomFolderPath = @"DicomVideo"; // DICOM 파일들이 저장된 폴더 경로


            // 디렉터리에 있는 모든 DICOM 파일 가져오기
            string[] dicomFiles = Directory.GetFiles(dicomFolderPath, "*.dcm");

            foreach (var filePath in dicomFiles)
            {
                // DICOM 파일명에서 패턴을 찾기 (숫자_날짜+위치+순서+번호.dcm)
                Match match = Regex.Match(Path.GetFileName(filePath), @"^Video_(\d+)_(\d{8})_(\w+)_(\d+)\.dcm$");

                if (match.Success)
                {
                    string hid = match.Groups[1].Value; // DICOM 파일명에서 추출한 HID 값

                    // 선택된 HID와 일치하면 파일명 업데이트
                    if (hid.Equals(SelectHID))
                    {
                        File.Delete(filePath);
                    }
                }
            }

        }
        public void DeleteReport(string SelectHID)
        {
            string dicomFolderPath = @"Report"; // DICOM 파일들이 저장된 폴더 경로


            // 디렉터리에 있는 모든 DICOM 파일 가져오기
            string[] dicomFiles = Directory.GetFiles(dicomFolderPath, "*.dcm");

            foreach (var filePath in dicomFiles)
            {
                // DICOM 파일명에서 패턴을 찾기 (숫자_날짜+위치+순서+번호.dcm)
                Match match = Regex.Match(Path.GetFileName(filePath), @"^Report_(\d+)_(\w+\(#\d+\)\(\d+\))\.dcm$");

                if (match.Success)
                {
                    string hid = match.Groups[1].Value; // DICOM 파일명에서 추출한 HID 값

                    // 선택된 HID와 일치하면 파일명 업데이트
                    if (hid.Equals(SelectHID))
                    {
                        File.Delete(filePath);
                    }
                }
            }

        }
    }
} 
