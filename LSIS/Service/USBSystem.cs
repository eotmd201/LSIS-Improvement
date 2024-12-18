using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace LSIS
{
    class USBSystem
    {
        public List<string> GetList()
        {
            string USB_Serial_Port = "";
            string USB_SERIAL_CH340 = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select Name from Win32_PnpEntity");
            foreach (ManagementObject devices in searcher.Get())
            {
                if (devices.GetPropertyValue("Name") != null)
                {
                    string name = devices.GetPropertyValue("Name").ToString();
                    if (name.Contains("(COM"))
                    {
                        if (name.Contains("Port"))
                        {
                            USB_Serial_Port = GetMiddleString(name, "(", ")");
                        }
                        else if (name.Contains("CH340"))
                        {
                            USB_SERIAL_CH340 = GetMiddleString(name, "(", ")");
                        }
                    }

                }
            }
            List<string> UsbList = new List<string>();
            UsbList.Add(USB_Serial_Port);
            UsbList.Add(USB_SERIAL_CH340);
            return UsbList;
        }

        private string GetMiddleString(string str, string begin, string end)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            string result = null;
            if (str.IndexOf(begin) > -1)
            {
                str = str.Substring(str.IndexOf(begin) + begin.Length);
                if (str.IndexOf(end) > -1) result = str.Substring(0, str.IndexOf(end));
                else result = str;
            }
            return result;
        }
    }
}
