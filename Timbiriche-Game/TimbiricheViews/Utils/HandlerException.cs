using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheViews.Utils
{
    public static class HandlerException
    {
        private static readonly ILogger _logger = LoggerManager.GetLogger();

        public static void HandleErrorException(Exception ex)
        {
            _logger.Error(ex.Message + "\n" + ex.StackTrace + "\n");
        }

        public static void HandleFatalException(Exception ex)
        {
            _logger.Fatal(ex.Message + "\n" + ex.StackTrace + "\n");
        }
    }
}
