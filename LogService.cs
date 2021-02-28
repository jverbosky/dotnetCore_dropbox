using System;
using System.IO;  // support for Directory.EnumerateFiles


namespace dotnetCore_dropbox
{
    public class LogService
    {
        public string LogFile { get; set; }


        // Constructor to setup logging properties
        public LogService(string logFile)
        {
            LogFile = logFile;
        }


        // Logs information to timestamped file in /logs directory
        public void LogActivity(string logEntry)
        {
            string lineSeparator = "--------------------------------------------------";
            
            try{
                using (System.IO.StreamWriter streamWriter = File.AppendText(LogFile))
                {
                    streamWriter.Write(logEntry + Environment.NewLine);
                    streamWriter.Write(lineSeparator + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("The file could not be opened:");
                Console.WriteLine(ex.Message);
            }
        }

    }
}