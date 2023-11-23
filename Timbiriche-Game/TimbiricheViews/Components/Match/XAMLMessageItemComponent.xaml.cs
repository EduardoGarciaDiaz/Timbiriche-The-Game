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
using TimbiricheViews.Player;

namespace TimbiricheViews.Components.Match
{
    public partial class XAMLMessageItemComponent : UserControl
    {
        private int _idSenderPlayer;

        public XAMLMessageItemComponent(string senderUsername, string message, bool isMessageReceived, int idSenderPlayer)
        {          
            InitializeComponent();
            tbkSenderUsername.Text = senderUsername;
            tbkMessage.Text = message;

            _idSenderPlayer = idSenderPlayer;

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
        public void NotifyPlayerAlreadyReported()
        {
            Utils.EmergentWindows.CreateEmergentWindow("Jugador ya reportado", "Ya has reportado a este jugador.");
        }

        public void NotifyReportCompleted()
        {
            Utils.EmergentWindows.CreateEmergentWindow("Reporte éxitoso", "El jugador ha sido reportado. Agradecemos tu apoyo.");

        }

        private void BtnReportMessage_Click(object sender, RoutedEventArgs e)
        {
            int idPlayerReporter = PlayerSingleton.Player.IdPlayer;
            DateTime currentDateTime = DateTime.Now;

            InstanceContext context = new InstanceContext(this);
            Server.BanManagerClient banManagerClient = new Server.BanManagerClient(context);
            banManagerClient.ReportMessage(_idSenderPlayer, idPlayerReporter, currentDateTime);
        }
    }

}
