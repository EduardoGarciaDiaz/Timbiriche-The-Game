using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
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
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerExceptions.HandleErrorException(ex, NavigationService);
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
                HandlerExceptions.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerExceptions.HandleFatalException(ex, NavigationService);
            }
        }

        private void BtnGoToLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
