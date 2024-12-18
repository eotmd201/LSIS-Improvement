using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS
{
    public class Serial
    {
        private SerialPort serial;
        public delegate void ReceiveMessageCallback(string msg);
        public event ReceiveMessageCallback ReceiveMessage;
        public event EventHandler<string> DataReceived;
        public SerialPort Getserial()
        {
            return serial;
        }
        public async Task<bool> Open(string comPort, int baudrate)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (serial != null && serial.IsOpen)
                        return;
                    serial = new SerialPort();
                    serial.PortName = comPort;
                    serial.BaudRate = baudrate;
                    //serial.ReadTimeout = 5000;
                    serial.DtrEnable = true;
                    serial.RtsEnable = true;
                    serial.Open();
                    //var type = serial.GetType();

                    if (serial.IsOpen)
                    {
                        /*if (Data == 1)
                        {
                            serial.DataReceived += Serial_DataReceived;
                        }*/
                        //serial.DataReceived += Serial_DataReceived;
                        //serial.DataReceived += Serial_DataReceived;
                        //serial.ErrorReceived += Serial_ErrorReceived;
                        //serial.Disposed += Serial_Disposed;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }

            });

            return serial.IsOpen;
        }
        private void Serial_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }
        public void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string recvData = serial.ReadLine().Trim();
                DataReceived?.Invoke(this, recvData);
            }
            catch (Exception ep)
            {
                Console.WriteLine(ep.StackTrace);
            }

        }
        public bool IsOpen()
        {
            if (serial == null)
                return false;
            return serial.IsOpen;
        }
        public void WriteMessage(string msg, string etx)
        {
            if (msg == null)
                return;
            if (serial == null || !serial.IsOpen)
                return;

            if (etx != null)
                msg += etx;
            serial.Write(msg);
        }
        public void Close()
        {
            if (serial == null || !serial.IsOpen)
                return;
            serial.Close();
        }
    }
}
