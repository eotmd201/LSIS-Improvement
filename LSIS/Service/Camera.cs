using SpinnakerNET;
using SpinnakerNET.GenApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS
{
    public class Camera
    {
        private ManagedSystem managedSystem;
        private bool camOpen = false;
        private bool camConnection = false;
        private static ManagedCameraList managedCameras;
        private List<BackgroundWorker> workers = new List<BackgroundWorker>();
        private List<string> deviceSerialNums = new List<string>();
        private List<string> deviceModelName = new List<string>();

        public delegate void CaptureImageEvent(string serialNum, IManagedImage managedImage);
        public event CaptureImageEvent CaptureImageHandler;

        public Camera()
        {
            managedSystem = new ManagedSystem();
            LibraryVersion spinVersion = managedSystem.GetLibraryVersion();
            Console.WriteLine(
                "## Spinnaker Lib Version: {0}.{1}.{2}.{3}\n",
                spinVersion.major,
                spinVersion.minor,
                spinVersion.type,
                spinVersion.build);
        }

        public async Task InitAsync()
        {
            camConnection = true;
            await Task.Run(() =>
            {
                foreach (IManagedCamera managedCamera in managedCameras)
                {
                    managedCamera.Init();
                }

            });
        }
        public bool IsOpen()
        {
            return camOpen;
        }
        public bool IsConnection()
        {
            return camConnection;
        }
        public void Dispose()
        {
            foreach (IManagedCamera managedCamera in managedCameras)
            {
                managedCamera.DeInit();
            }
            managedCameras.Clear();
            managedSystem.Dispose();
        }
        public void StartLiveView()
        {
            for (int i = 0; i < managedCameras.Count; i++)
            {
                try
                {
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += Worker_DoWork;
                    worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                    workers.Add(worker);
                    worker.RunWorkerAsync(argument: managedCameras[i]);
                    managedCameras[i].BeginAcquisition();
                }
                catch (SpinnakerException se)
                {
                    Console.WriteLine("[Err] BeginAcquisition {0} : {1}", deviceSerialNums[i], se.Message);
                }
            }
            camOpen = true;
        }
        public void StopLiveView()
        {
            if (camOpen == true)
            {
                for (int i = 0; i < managedCameras.Count; i++)
                {
                    managedCameras[i].EndAcquisition();
                }
            }
            camOpen = false;
        }
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            int idx = workers.IndexOf(worker);
            if (managedCameras[idx].IsStreaming())
            {
                worker.RunWorkerAsync(argument: managedCameras[idx]);
            }
            else
            {
                workers.Remove(worker);
                worker.Dispose();
                if (worker == null)
                    Console.WriteLine("> {0} Camera Stop..RunWorkerCompleted..", idx);
            }
        }
        private List<double> Camera_Set(int ExposureTime, int Gain, int Gamma)
        {
            List<double> set = new List<double>();
            set.Add(ExposureTime * 100000 + 100000);
            set.Add(Gain * 3);
            set.Add(Gamma * 0.1 + 0.3);
            return set;
        }
        public void Setting(int ExposureTime, int Gain, int Gamma)
        {
            List<double> set = Camera_Set(ExposureTime, Gain, Gamma);
            deviceModelName.Clear();
            for (int i = 0; i < managedCameras.Count; i++)
            {
                IManagedCamera managedCamera = managedSystem.GetCameras()[0];
                INodeMap nodeMap = managedCamera.GetTLDeviceNodeMap();
                IString iDeviceDeviceModelName = nodeMap.GetNode<IString>("DeviceModelName");
                deviceModelName.Add(iDeviceDeviceModelName.Value);
                nodeMap = managedCamera.GetNodeMap();

                IEnum iExposureAuto = nodeMap.GetNode<IEnum>("ExposureAuto");

                IEnumEntry iExposureAutoContinuous = iExposureAuto.GetEntryByName("Off");

                iExposureAuto.Value = iExposureAutoContinuous.Symbolic;

                IFloat iExposureTime = nodeMap.GetNode<IFloat>("ExposureTime");
                //Set gain to 0.6 dB
                iExposureTime.Value = set[0];//800000

                IEnum iGainAuto = nodeMap.GetNode<IEnum>("GainAuto");

                //EnumEntry node (always associated with an Enumeration node)
                IEnumEntry iGainAutoOff = iGainAuto.GetEntryByName("Off");

                //Turn off Auto Gain
                iGainAuto.Value = iGainAutoOff.Symbolic;

                // Float node
                IFloat iGain = nodeMap.GetNode<IFloat>("Gain");

                //Set gain to 0.6 dB
                iGain.Value = set[1]; //27

                // Float node
                IFloat iGamma = nodeMap.GetNode<IFloat>("Gamma");

                //Set gain to 0.6 dB
                iGamma.Value = set[2];//0.8
            }

        }
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            IManagedCamera managedCamera = (IManagedCamera)e.Argument;
            BackgroundWorker worker = (BackgroundWorker)sender;
            int idx = workers.IndexOf(worker);
            try
            {
                using (IManagedImage rawImage = managedCamera.GetNextImage(1000))
                {
                    if (!rawImage.IsIncomplete)
                    {
                        using (IManagedImage bgrImage = rawImage.Convert(PixelFormatEnums.BGR8))
                        {
                            if (CaptureImageHandler != null)
                                CaptureImageHandler(deviceSerialNums[idx], bgrImage);
                        }
                    }
                }
            }
            catch (SpinnakerException se)
            {
                Console.WriteLine("IManagedImage Error: {0}\n IsInitialized >> {1} // IsValid >> {2}", se.Message, managedCamera.IsInitialized(), managedCamera.IsValid());
            }
        }
        public List<string> GetSerialNumbers()
        {
            deviceSerialNums.Clear();

            if (managedSystem != null)
            {
                managedCameras = managedSystem.GetCameras();
                Console.WriteLine("> Camera Count: {0}", managedCameras.Count);

                for (int i = 0; i < managedCameras.Count; i++)
                {
                    IManagedCamera managedCamera = managedCameras[i];
                    INodeMap nodeMap = managedCamera.GetTLDeviceNodeMap();
                    IString iDeviceSerialNumber = nodeMap.GetNode<IString>("DeviceSerialNumber");
                    if (iDeviceSerialNumber != null && iDeviceSerialNumber.IsReadable)
                    {
                        deviceSerialNums.Add(iDeviceSerialNumber.Value);
                    }
                    PrintDeviceInfo(i, nodeMap);
                }
            }
            return deviceSerialNums;
        }
        private void PrintDeviceInfo(int pos, INodeMap nodeMap)
        {
            try
            {
                Console.WriteLine("## Camera Position {0} ##", pos);

                ICategory category = nodeMap.GetNode<ICategory>("DeviceInformation");
                if (category != null && category.IsReadable)
                {
                    for (int i = 0; i < category.Children.Length; i++)
                    {
                        Console.WriteLine(
                            "{0}: {1}",
                            category.Children[i].Name,
                            (category.Children[i].IsReadable ? category.Children[i].ToString()
                             : "Node not available"));
                    }
                }
                else
                {
                    Console.WriteLine("Device control information not available.");
                }
            }
            catch (SpinnakerException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }
    }
}
