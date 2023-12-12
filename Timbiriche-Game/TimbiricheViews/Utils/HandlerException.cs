using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using TimbiricheViews.Views;

namespace TimbiricheViews.Utils
{
    public static class HandlerException
    {
        private static readonly ILogger _logger = LoggerManager.GetLogger();

        public static void HandleErrorException(Exception ex, NavigationService navigationService)
        {
            _logger.Error(ex.Message + "\n" + ex.StackTrace + "\n");
            navigationService.Navigate(new XAMLLogin());
        }

        public static void HandleFatalException(Exception ex, NavigationService navigationService)
        {
            _logger.Fatal(ex.Message + "\n" + ex.StackTrace + "\n");
            navigationService.Navigate(new XAMLLogin());
        }

        public static void HandleComponentErrorException(Exception ex)
        {
            _logger.Error(ex.Message + "\n" + ex.StackTrace + "\n");
        }

        public static void HandleComponentFatalException(Exception ex)
        {
            _logger.Fatal(ex.Message + "\n" + ex.StackTrace + "\n");
        }
    }
}
