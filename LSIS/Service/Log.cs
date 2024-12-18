using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSIS
{
    class Log
    {
        private string _logFilePath;
        public Log(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public void WriteLog(string message)
        {
            File.AppendAllText("log.txt", $"{DateTime.Now} {message}\n");
        }

        public void CleanupOldLogs(int daysOld)
        {
            List<string> updatedLogLines = new List<string>();
            DateTime thresholdDate = DateTime.Now.AddDays(-daysOld);

            foreach (var line in File.ReadAllLines(_logFilePath))
            {
                try
                {
                    var dateTimeString = line.Split(new char[] { ' ' }, 2)[0];
                    var logDateTime = DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture);

                    if (logDateTime >= thresholdDate)
                    {
                        updatedLogLines.Add(line);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing date in line: {line}. Error: {ex.Message}");
                }
            }

            File.WriteAllLines(_logFilePath, updatedLogLines);
        }
    }
}
