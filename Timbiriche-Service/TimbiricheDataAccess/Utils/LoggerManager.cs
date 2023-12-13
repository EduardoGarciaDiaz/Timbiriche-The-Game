using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheDataAccess.Utils
{
    public static class LoggerManager
    {
        private static ILogger _logger;

        private static void ConfigureLogger(string logFilePath)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(@logFilePath)
                .CreateLogger();
        }

        private static string BuildLogFilePath()
        {
            const string DATE_FORMAT = "dd-MM-yyyy";
            const string ID_FILE_NAME = "Log";
            const string CHARACTER_SEPARATOR = "_";
            const string FILE_EXTENSION = ".txt";
            const string RELATIVE_LOG_FILE_PATH = "../../Logs\\";

            DateTime currentDate = DateTime.Today;
            string date = currentDate.ToString(DATE_FORMAT);

            string logFileName = ID_FILE_NAME + CHARACTER_SEPARATOR + date + FILE_EXTENSION;
            string absoluteLogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RELATIVE_LOG_FILE_PATH);
            string logPath = absoluteLogFilePath + logFileName;

            return logPath;
        }

        public static ILogger GetLogger()
        {
            if (_logger == null)
            {
                string logPath = BuildLogFilePath();
                ConfigureLogger(logPath);
            }
            _logger = Log.Logger;
            return _logger;
        }

        public static void CloseAndFlush()
        {
            (_logger as IDisposable)?.Dispose();
            Log.CloseAndFlush();
            _logger = null;
        }
    }
}
