using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ConsoleApplication1.Logging
{
    public class LoggerHandlerFile : ILoggerListener
    {
        private object lockFile = new object();
        private string filePath;
        private string baseFileName;
        private string dirPath;

        public LoggerHandlerFile(string filePath)
        {
            this.baseFileName = Path.GetFileNameWithoutExtension(filePath);
            this.dirPath = Path.GetDirectoryName(filePath);

            DateTime now = DateTime.Now;
            this.filePath = Path.Combine(this.dirPath, this.baseFileName + "_" + now.Year + "_" + now.Month + "_" + now.Day + ".log");

            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            else
            {
                foreach (string oldFile in Directory.GetFiles(this.dirPath, this.baseFileName + "*"))
                {
                    if (File.GetLastWriteTime(oldFile) < DateTime.Now - TimeSpan.FromDays(14))
                    {
                        File.Delete(oldFile);
                    }
                }
            }
        }

        public void LogError(DateTime now, string message, int? ident)
        {
            WriteLine("ERR", now, message, ident);
        }

        public void LogMessage(DateTime now, string message, int? ident)
        {
            WriteLine("MSG", now, message, ident);
        }

        public void LogWarning(DateTime now, string message, int? ident)
        {
            WriteLine("WRN", now, message, ident);
        }

        private void WriteLine(string type, DateTime now, string message, int? ident)
        {
            lock (this.lockFile)
            {
                using (StreamWriter writer = File.AppendText(this.filePath))
                {
                    writer.Write(type + " " + now + " ");

                    if (ident.HasValue)
                    {
                        for (int i = 0; i < ident.Value; i++)
                        {
                            writer.Write("    ");
                        }
                    }

                    writer.WriteLine(message);
                }
            }
        }

        public string FilePath
        {
            get
            {
                return this.filePath;
            }
            set
            {
                this.filePath = value;
            }
        }
    }
}
