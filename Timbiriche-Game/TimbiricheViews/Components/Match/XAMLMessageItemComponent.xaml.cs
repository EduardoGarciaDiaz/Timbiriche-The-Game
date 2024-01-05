using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TimbiricheViews.Player;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Components.Match
{
    public partial class XAMLMessageItemComponent : UserControl
    {
        private readonly int _idSenderPlayer;
        private readonly string _lobbyCode;

        public XAMLMessageItemComponent(string senderUsername, string message, bool isMessageReceived, int idSenderPlayer, string lobbyCode)
        {          
            InitializeComponent();

            tbkSenderUsername.Text = senderUsername;
            tbkMessage.Text = message;

            _idSenderPlayer = idSenderPlayer;
            _lobbyCode = lobbyCode;

            if (!isMessageReceived)
            {
                imgResportMessage.Visibility = Visibility.Collapsed;
            }
        }

        private void ImgReportMessage_MouseEnter(object sender, MouseEventArgs e)
        {
            imgResportMessage.Visibility = Visibility.Collapsed;
            gridReportMessage.Visibility = Visibility.Visible;
        }

        private void GridReportMessage_MouseLeave(object sender, MouseEventArgs e)
        {
            imgResportMessage.Visibility = Visibility.Visible;
            gridReportMessage.Visibility = Visibility.Collapsed;
        }
    }

    public partial class XAMLMessageItemComponent : Server.IBanManagerCallback
    {
        public void NotifyReportCompleted()
        {
            EmergentWindows.CreateSuccesfulReportMessageWindow();
        }

        public void NotifyPlayerAlreadyReported()
        {
            EmergentWindows.CreateReportedPlayerMessageWindow();
        }

        public void NotifyPlayerBanned(int idPlayerBanned)
        {
            EmergentWindows.CreateBannedPlayerMessageWindow();
        }

        private void BtnReportMessage_Click(object sender, RoutedEventArgs e)
        {
            int idPlayerReporter = PlayerSingleton.Player.IdPlayer;
            int idGuestIdPlayer = 0;

            if (idPlayerReporter > idGuestIdPlayer)
            {
                ReportMessage(idPlayerReporter);
            }
        }

        private void ReportMessage(int idPlayerReporter)
        {
            InstanceContext context = new InstanceContext(this);
            Server.BanManagerClient banManagerClient = new Server.BanManagerClient(context);

            try
            {
                string reporterUsername = PlayerSingleton.Player.Username;
                banManagerClient.ReportMessage(_lobbyCode, _idSenderPlayer, idPlayerReporter, reporterUsername);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerExceptions.HandleComponentErrorException(ex);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerExceptions.HandleComponentErrorException(ex);
            }
            catch (FaultException)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerExceptions.HandleComponentErrorException(ex);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerExceptions.HandleComponentFatalException(ex);
            }
        }
    }

}
