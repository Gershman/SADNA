using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    class Logger
    {
        private string filePath;
        public Logger(string filePath)
        {
            this.filePath = filePath;
        }

        public void writeToLog(string message)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
            {
                file.WriteLine(message);
            }
        }
	
    }
}
