using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.IO.Buffer;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LSIS
{
    class DicomManager
    {
        private DicomDataset dataset;
        private DicomDataset videoDataset;
        private DicomDataset reportdataset;

        int OID = 200128;
        int ProductCode = 0001;
        string SOPClassUID = "1.2.840.10008.5.1.4.1.1.77.1.4.1";
        string SOPClassUIDVideo = "1.2.840.10008.5.1.4.1.1.7";
        string SOPClassUIDReport = "1.2.840.10008.5.1.4.1.1.7";
        string UID;

        public DicomManager(string id,string serialnumber)
        {
            dataset = new DicomDataset();
            videoDataset = new DicomDataset();
            reportdataset = new DicomDataset();
            UID = "1.2.410." + OID.ToString() + "." + ProductCode.ToString().TrimStart('0') + "." + serialnumber.ToString().TrimStart('0') + "." + id.TrimStart('0');
        }

        private void SetGeneralEquipment()
        {
            dataset.Add(DicomTag.Manufacturer, "S-ONE BIO");        // 	(0008,0070)
            dataset.Add(DicomTag.ManufacturerModelName, "LYMPOSCOPE ICG_A1.0"); //	(0008,1090)
        }
        public void SetPatient(string id, string name, string birthDate, string gender, string age)
        {
            dataset.Add(DicomTag.PatientID, id);         // (0010,0020)
            dataset.Add(DicomTag.SpecificCharacterSet, "ISO_IR 149"); //(0008,0005)
            dataset.Add(DicomTag.PatientName, name);   // 	(0010,0010)
            dataset.Add(DicomTag.PatientBirthDate, birthDate);     // 	(0010,0030)
            dataset.Add(DicomTag.PatientSex, gender);                  // 	(0010,0040)                        
            dataset.Add(DicomTag.PatientAge, age.PadLeft(3, '0') + "Y");   // 	(0010,1010)
            dataset.Add(DicomTag.AdmittingDiagnosesDescription, ""); //(0008, 1080)  //진단확인된 명칭
        }
        public void SetStudy(string studyID, string accessionNumber, string date, string time, string injection, string hospitalName, string description)
        {
            string studyInstanceUID = UID + "." + DateTime.Now.ToString("yyyyMMdd");
            // https://dicom.nema.org/dicom/2013/output/chtml/part05/chapter_9.html 
            dataset.Add(DicomTag.StudyInstanceUID, studyInstanceUID);   //(0020,000D)
            dataset.Add(DicomTag.StudyID, studyID);    //	(0020,0010)
            dataset.Add(DicomTag.AccessionNumber, accessionNumber); //(0040, 050A)

            dataset.Add(DicomTag.StudyDate, date);    //	(0008,0020)
            dataset.Add(DicomTag.ScheduledProcedureStepStartTime, injection); // 인젝션 시간
            dataset.Add(DicomTag.StudyTime, time);   //	(0008,0030)
            dataset.Add(DicomTag.InstitutionName, hospitalName); //(0008, 0080)
            //dataset.Add(DicomTag.StudyDescription, description);   //	(0008,1030)
        }
        public void SetSeries(string seriesNumber, string bodyPart, string date, string time)
        {
            string seriesInstanceUID = UID + "." + seriesNumber + "." + DateTime.Now.ToString("yyyyMMddHHmmss");
            dataset.Add(DicomTag.SeriesInstanceUID, seriesInstanceUID);  //(0020,000E)
            dataset.Add(DicomTag.SeriesNumber, seriesNumber);      //	(0020,0011)

            // https://dicom.nema.org/medical/dicom/current/output/chtml/part16/chapter_L.html#chapter_L
            dataset.Add(DicomTag.BodyPartExamined, bodyPart);    //(0018,0015)

            dataset.Add(DicomTag.SeriesDate, date);   //	(0008,0021)
            dataset.Add(DicomTag.SeriesTime, time); //	(0008,0031)
            dataset.Add(DicomTag.Modality, "XC");           // 	(0008,0060)
            dataset.Add(DicomTag.SmallestPixelValueInSeries, "0");  //	(0028,0108)
            dataset.Add(DicomTag.LargestPixelValueInSeries, "32767");   // 	(0028,0109)
        }
        public void SetContent(string seriesNumber, string date, string time, string instanceNumber)
        {
            //string SOPInstanceUID = ConvertGuidToUuidInteger();
            string SOPInstanceUID = UID + "." + seriesNumber + "." + instanceNumber + "." + DateTime.Now.ToString("yyyyMMddHHmmss");
            dataset.Add(DicomTag.SOPClassUID, SOPClassUID);
            dataset.Add(DicomTag.SOPInstanceUID, SOPInstanceUID);
            dataset.Add(DicomTag.ContentDate, date);  //	(0008,0023)
            dataset.Add(DicomTag.ContentTime, time);    //(0008,0033)
            dataset.Add(DicomTag.InstanceNumber, instanceNumber); //	(0020,0013)
        }
        public void SetPattern(List<int> pattern)
        {
            reportdataset.Add(new DicomTag(0x0009, 0x0010), pattern[0].ToString());
            reportdataset.Add(new DicomTag(0x0009, 0x0011), pattern[1].ToString());
            reportdataset.Add(new DicomTag(0x0009, 0x0012), pattern[2].ToString());
            reportdataset.Add(new DicomTag(0x0009, 0x0013), pattern[3].ToString());
            reportdataset.Add(new DicomTag(0x0009, 0x0014), pattern[4].ToString());
            reportdataset.Add(new DicomTag(0x0009, 0x0015), pattern[5].ToString());
            reportdataset.Add(new DicomTag(0x0009, 0x0016), pattern[6].ToString());
            reportdataset.Add(new DicomTag(0x0009, 0x0017), pattern[7].ToString());
            reportdataset.Add(new DicomTag(0x0009, 0x0018), pattern[8].ToString());
        }
        public void SetContentVideo(string seriesNumber, string date, string time)
        {
            string SOPInstanceUID = UID + "." + seriesNumber + "." + DateTime.Now.ToString("yyyyMMddHHmmss");
            videoDataset.Add(DicomTag.SOPClassUID, SOPClassUIDVideo);
            videoDataset.Add(DicomTag.SOPInstanceUID, SOPInstanceUID);
            videoDataset.Add(DicomTag.ContentDate, date);  //	(0008,0023)
            videoDataset.Add(DicomTag.ContentTime, time);    //(0008,0033)
        }
        public void SetPrivateDataElement(Data data, string shotmode)
        {
            List<double> CameraVal = data.GetCameraVal();
            dataset.Add<string>(new DicomTag(0x0009, 0x0010), CameraVal[0].ToString()); //카메라 노출값
            dataset.Add<string>(new DicomTag(0x0009, 0x0011), CameraVal[1].ToString()); //카메라 게인값
            dataset.Add<string>(new DicomTag(0x0009, 0x0012), CameraVal[2].ToString()); //카메라 감마값
            dataset.Add<string>(new DicomTag(0x0009, 0x0013), shotmode); //샷 모드
            dataset.Add<string>(new DicomTag(0x0009, 0x0014), data.shotposition); //샷 위치
        }
        private void ReportSetGeneralEquipment()
        {
            reportdataset.Add(DicomTag.Manufacturer, "S-ONE BIO");        // 	(0008,0070)
            reportdataset.Add(DicomTag.ManufacturerModelName, "LYMPOSCOPE ICG_A1.0"); //	(0008,1090)
            reportdataset.Add(DicomTag.ConversionType, "SI"); //(0008, 0064)
        }
        public void ReportSetPatient(string id, string name, string birthDate, string gender, string age)
        {
            reportdataset.Add(DicomTag.PatientID, id);         // (0010,0020)
            reportdataset.Add(DicomTag.SpecificCharacterSet, "ISO_IR 149"); //(0008,0005)
            reportdataset.Add(DicomTag.PatientName, name);   // 	(0010,0010)
            reportdataset.Add(DicomTag.PatientBirthDate, birthDate);     // 	(0010,0030)
            reportdataset.Add(DicomTag.PatientSex, gender);                  // 	(0010,0040)                        
            reportdataset.Add(DicomTag.PatientAge, age.PadLeft(3, '0') + "Y");   // 	(0010,1010)
        }
        public void ReportSetStudy(string studyID, string accessionNumber, string date, string time, string hospitalName, string description)
        {
            string studyInstanceUID = UID + "." + DateTime.Now.ToString("yyyyMMdd");
            // https://dicom.nema.org/dicom/2013/output/chtml/part05/chapter_9.html 
            reportdataset.Add(DicomTag.StudyInstanceUID, studyInstanceUID);   //(0020,000D)
            reportdataset.Add(DicomTag.StudyID, studyID);    //	(0020,0010)
            reportdataset.Add(DicomTag.AccessionNumber, accessionNumber); //(0040, 050A)
            reportdataset.Add(DicomTag.ReferringPhysicianName, description); //(0008,0090)
            reportdataset.Add(DicomTag.StudyDate, date);    //	(0008,0020)
            reportdataset.Add(DicomTag.StudyTime, time);   //	(0008,0030)
            reportdataset.Add(DicomTag.InstitutionName, hospitalName); //(0008, 0080)
            //dataset.Add(DicomTag.StudyDescription, description);   //	(0008,1030)
        }
        public void ReportSetSeries(string seriesNumber, string bodyPart, string date, string time)
        {
            string seriesInstanceUID = UID + "." + seriesNumber + "." + DateTime.Now.ToString("yyyyMMddHHmmss");
            reportdataset.Add(DicomTag.SeriesInstanceUID, seriesInstanceUID);  //(0020,000E)
            reportdataset.Add(DicomTag.SeriesNumber, seriesNumber);      //	(0020,0011)

            // https://dicom.nema.org/medical/dicom/current/output/chtml/part16/chapter_L.html#chapter_L
            reportdataset.Add(DicomTag.BodyPartExamined, bodyPart);    //(0018,0015)
            reportdataset.Add(DicomTag.SeriesDate, date);   //	(0008,0021)
            reportdataset.Add(DicomTag.SeriesTime, time); //	(0008,0031)
            reportdataset.Add(DicomTag.Modality, "OT");           // 	(0008,0060)
            reportdataset.Add(DicomTag.SmallestPixelValueInSeries, "0");  //	(0028,0108)
            reportdataset.Add(DicomTag.LargestPixelValueInSeries, "32767");   // 	(0028,0109)
        }
        public void ReportSetContent(string seriesNumber, string date, string time, string instanceNumber)
        {
            //string SOPInstanceUID = ConvertGuidToUuidInteger();
            string SOPInstanceUID = UID + "." + seriesNumber + "." + instanceNumber + "." + DateTime.Now.ToString("yyyyMMddHHmmss");
            reportdataset.Add(DicomTag.SOPClassUID, SOPClassUIDReport);
            reportdataset.Add(DicomTag.SOPInstanceUID, SOPInstanceUID);
            reportdataset.Add(DicomTag.ContentDate, date);  //	(0008,0023)
            reportdataset.Add(DicomTag.ContentTime, time);    //(0008,0033)
            reportdataset.Add(DicomTag.InstanceNumber, instanceNumber); //	(0020,0013)
        }
        public async Task<bool> SaveImageFile(string savePath, Bitmap resPath)
        {
            if (resPath == null)
            {
                return false;
            }

            bool result = false;
            await Task.Run(() =>
            {
                try
                {
                    SetGeneralEquipment();
                    SetImage(resPath);
                    DicomFile file = new DicomFile(dataset);
                    file.Save(savePath);
                    result = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });

            return result;
        }
        public async Task<bool> SaveVideoFile(string savePath, string resPath)
        {
            if (savePath.Length == 0 || resPath.Length == 0)
            {
                return false;
            }

            bool result = false;
            await Task.Run(() =>
            {
                try
                {
                    SetGeneralEquipment();
                    SetVideo(resPath);
                    DicomFile file = new DicomFile(dataset);
                    file.Save(savePath);
                    result = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
            Console.WriteLine("DICOM 성공");
            return result;
        }
        public async Task<bool> SaveReportFile(string savePath, Bitmap resPath)
        {
            if (resPath == null)
            {
                return false;
            }

            bool result = false;
            await Task.Run(() =>
            {
                try
                {
                    ReportSetGeneralEquipment();
                    SetReport(resPath);
                    DicomFile file = new DicomFile(reportdataset);
                    file.Save(savePath);
                    result = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });

            return result;
        }
        private void SetImage(Bitmap filePaths)
        {
            DicomDataset imgDataset = new DicomDataset();

            //Bitmap bitmap = new Bitmap(filePaths[0]);
            Bitmap bitmap = GetValidImage(filePaths);
            byte[] pixels = GetPixels(bitmap);
            MemoryByteBuffer buffer = new MemoryByteBuffer(pixels);

            imgDataset.Add(DicomTag.SamplesPerPixel, "3"); //	(0028,0002)
            imgDataset.Add(DicomTag.PhotometricInterpretation, PhotometricInterpretation.Rgb.Value); //	(0028,0004)
            //imgDataset.Add(DicomTag.PhotometricInterpretation, PhotometricInterpretation.Monochrome2.Value); //	(0028,0004)

            imgDataset.Add(DicomTag.Rows, (ushort)bitmap.Height);   //(0028,0010)
            imgDataset.Add(DicomTag.Columns, (ushort)bitmap.Width);    //	(0028,0011)

            imgDataset.Add(DicomTag.BitsAllocated, (ushort)8); //	(0028,0100)
            imgDataset.Add(DicomTag.BitsStored, (ushort)8);    //	(0028,0101)
            imgDataset.Add(DicomTag.HighBit, (ushort)7);       // (0028,0102) // BitsStored - 1
            imgDataset.Add(DicomTag.PixelRepresentation, (ushort)1);   // 	(0028,0103)
            imgDataset.Add(DicomTag.SmallestImagePixelValue, "0"); //	(0028,0106)
            imgDataset.Add(DicomTag.LargestImagePixelValue, "255");    //	(0028,0107)

            DicomPixelData dpData = DicomPixelData.Create(imgDataset, true);
            dpData.PixelRepresentation = 0;
            dpData.PlanarConfiguration = 0;
            dpData.AddFrame(buffer);
            dataset.Add(imgDataset);
        }
        private void SetVideo(string videoPath)
        {
            VideoCapture vCapture = new VideoCapture(videoPath);
            int frameCnt = vCapture.FrameCount;
            int fps = (int)vCapture.Fps;
            int duration = frameCnt / fps;

            Console.WriteLine("Frame Count : {0}, FPS : {1}, Duration : {2}", frameCnt, fps, duration);
            Console.WriteLine("Frame Width : {0}, Height : {1}", vCapture.FrameWidth, vCapture.FrameHeight);

            videoDataset.Add(DicomTag.SamplesPerPixel, "3"); //	(0028,0002)
            videoDataset.Add(DicomTag.PhotometricInterpretation, PhotometricInterpretation.Rgb.Value); //	(0028,0004)
            //imgDataset.Add(DicomTag.PhotometricInterpretation, PhotometricInterpretation.Monochrome2.Value); //	(0028,0004)

            videoDataset.Add(DicomTag.Rows, (ushort)vCapture.FrameHeight);   //(0028,0010)
            videoDataset.Add(DicomTag.Columns, (ushort)vCapture.FrameWidth);    //	(0028,0011)

            videoDataset.Add(DicomTag.BitsAllocated, (ushort)8); //	(0028,0100)
            videoDataset.Add(DicomTag.BitsStored, (ushort)8);    //	(0028,0101)
            videoDataset.Add(DicomTag.HighBit, (ushort)7);       // (0028,0102) // BitsStored - 1
            videoDataset.Add(DicomTag.PixelRepresentation, (ushort)1);   // 	(0028,0103)

            videoDataset.Add(DicomTag.StartTrim, "1");       //	(0008,2142)
            videoDataset.Add(DicomTag.StopTrim, frameCnt.ToString());      // 	(0008,2143)
            videoDataset.Add(DicomTag.CineRate, fps.ToString());        //	(0018,0040)       // FPS
            videoDataset.Add(DicomTag.EffectiveDuration, duration.ToString());  //	(0018,0072)   // Sec
            videoDataset.Add(DicomTag.NumberOfFrames, frameCnt.ToString());  // (0028,0008)  // Total Frame Size

            DicomPixelData dpData = DicomPixelData.Create(videoDataset, true);
            dpData.PixelRepresentation = 0;
            dpData.PlanarConfiguration = 0;

            using (Mat captureImg = new Mat())
            {
                while (true)
                {
                    vCapture.Read(captureImg);
                    if (captureImg.Empty())
                    {
                        break;
                    }
                    Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(captureImg);
                    bitmap = GetValidImage(bitmap);
                    byte[] pixels = GetPixels(bitmap);
                    MemoryByteBuffer buffer = new MemoryByteBuffer(pixels);
                    dpData.AddFrame(buffer);
                }
            }
            dataset.Add(videoDataset);
        }
        private void SetReport(Bitmap filePaths)
        {
            DicomDataset imgDataset = new DicomDataset();

            //Bitmap bitmap = new Bitmap(filePaths[0]);
            Bitmap bitmap = GetValidImage(filePaths);
            byte[] pixels = GetPixels(bitmap);
            MemoryByteBuffer buffer = new MemoryByteBuffer(pixels);

            imgDataset.Add(DicomTag.SamplesPerPixel, "3"); //	(0028,0002)
            imgDataset.Add(DicomTag.PhotometricInterpretation, PhotometricInterpretation.Rgb.Value); //	(0028,0004)
            //imgDataset.Add(DicomTag.PhotometricInterpretation, PhotometricInterpretation.Monochrome2.Value); //	(0028,0004)

            imgDataset.Add(DicomTag.Rows, (ushort)bitmap.Height);   //(0028,0010)
            imgDataset.Add(DicomTag.Columns, (ushort)bitmap.Width);    //	(0028,0011)

            imgDataset.Add(DicomTag.BitsAllocated, (ushort)8); //	(0028,0100)
            imgDataset.Add(DicomTag.BitsStored, (ushort)8);    //	(0028,0101)
            imgDataset.Add(DicomTag.HighBit, (ushort)7);       // (0028,0102) // BitsStored - 1
            imgDataset.Add(DicomTag.PixelRepresentation, (ushort)1);   // 	(0028,0103)
            imgDataset.Add(DicomTag.SmallestImagePixelValue, "0"); //	(0028,0106)
            imgDataset.Add(DicomTag.LargestImagePixelValue, "255");    //	(0028,0107)

            DicomPixelData dpData = DicomPixelData.Create(imgDataset, true);
            dpData.PixelRepresentation = 0;
            dpData.PlanarConfiguration = 0;
            dpData.AddFrame(buffer);
            reportdataset.Add(imgDataset);
        }
        private Bitmap GetValidImage(Bitmap bitmap)
        {
            if (bitmap.PixelFormat != PixelFormat.Format24bppRgb)
            {
                Bitmap old = bitmap;
                using (old)
                {
                    bitmap = new Bitmap(old.Width, old.Height, PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.DrawImage(old, 0, 0, old.Width, old.Height);
                    }
                }
            }
            return bitmap;
        }
        private byte[] GetPixels(Bitmap image)
        {
            int rows = image.Height;
            int columns = image.Width;
            if (rows % 2 != 0 && columns % 2 != 0) --columns;

            BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, columns, rows), ImageLockMode.ReadOnly, image.PixelFormat);
            IntPtr bmpData = data.Scan0;

            try
            {
                int stride = columns * 3;
                int size = rows * stride;
                byte[] pixelData = new byte[size];
                for (int i = 0; i < rows; ++i) Marshal.Copy(new IntPtr(bmpData.ToInt64() + i * data.Stride), pixelData, i * stride, stride);

                for (int i = 0; i < pixelData.Length; i += 3)
                {
                    byte temp = pixelData[i];
                    pixelData[i] = pixelData[i + 2];
                    pixelData[i + 2] = temp;
                }
                return pixelData;
            }
            finally
            {
                image.UnlockBits(data);
            }
        }
    }
}
