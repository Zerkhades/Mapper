using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Services.LoggerService
{
    public class LoggerService: ILogService
    {
        private string _folder = AppContext.BaseDirectory + "LogFiles";

        private string _file = AppContext.BaseDirectory + "LogFiles" + "\\" + DateTime.Now.ToString("yyyy-MM-dd") +
                                "\\log.log";
        private readonly object _sync = new();
        public LoggerService()
        {
            if (!File.Exists(_folder + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\log.log"))
            {
                Directory.CreateDirectory(_folder + "\\" + DateTime.Now.ToString("yyyy-MM-dd"));
                File.Create(_folder + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\log.log");
            }
        }

        public void Log(string message, string caller = "", string filePath = "", int lineNumber = 0)
        {
            lock (_sync)
            {
                using StreamWriter outputFile = new StreamWriter(_file, true);
                outputFile.WriteLine($@"[{DateTime.Now}] [Caller: {caller}] [Message: {message}] [FilePath: {filePath}]");
            }
        }
    }

}
