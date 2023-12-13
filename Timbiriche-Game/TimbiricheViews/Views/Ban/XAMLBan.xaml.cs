using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLBan : Page
    {
        public XAMLBan(int idPlayerBanned)
        {
            InitializeComponent();
            VerifyBanEndDate(idPlayerBanned);
        }

        private void VerifyBanEndDate(int idPlayerBanned)
        {
            Server.BanVerifierManagerClient banManagerClient = new Server.BanVerifierManagerClient();
            string dateFormat = "dd MMMM yyyy HH:mm";
            string inactiveStatus = "Inactive";

            try
            {
                Server.BanInformation banInformation = banManagerClient.VerifyBanEndDate(idPlayerBanned);

                string formattedDateTime = banInformation.EndDate.ToString(dateFormat);
                lbBanEndDate.Content = formattedDateTime;

                if (banInformation.BanStatus.Equals(inactiveStatus))
                {
                    gridBanFinished.Visibility = Visibility.Visible;
                }
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException<TimbiricheServerException>)
            {
                EmergentWindows.CreateDataBaseErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleFatalException(ex, NavigationService);
            }
        }

        private void BtnGoToLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
