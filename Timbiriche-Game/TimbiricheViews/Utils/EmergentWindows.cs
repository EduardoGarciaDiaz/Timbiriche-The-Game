using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimbiricheViews.Components;
using TimbiricheViews.Components.Lobby;

namespace TimbiricheViews.Utils
{
    public static class EmergentWindows
    {
        public static void CreateEmergentWindow(string titleEmergentWindow, string descriptionEmergentWindow)
        {
            XAMLEmergentWindow emergentWindow = new XAMLEmergentWindow(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.ShowDialog();
        }

        public static void CreateEmergentWindowNoModal(string titleEmergentWindow, string descriptionEmergentWindow)
        {
            XAMLEmergentWindow emergentWindowNoModal = new XAMLEmergentWindow(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindowNoModal.Show();
        }

        public static void CreateConnectionFailedMessageWindow()
        {
            string titleEmergentWindow = Properties.Resources.lbConnectionFailed;
            string descriptionEmergentWindow = Properties.Resources.tbkConnectionFailedDetails;

            XAMLEmergentWindow emergentWindow = new XAMLEmergentWindow(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.ShowDialog();
        }

        public static void CreateTimeOutMessageWindow()
        {
            string titleEmergentWindow = Properties.Resources.lbTimeOutExceptionTitle;
            string descriptionEmergentWindow = Properties.Resources.tbkTimeOutExceptionDescription;

            XAMLEmergentWindow emergentWindow = new XAMLEmergentWindow(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.ShowDialog();
        }

        public static void CreateLobbyInvitationWindow(string lobbyCode)
        {
            XAMLLobbyInvitationComponent lobbyInvitationComponent = new XAMLLobbyInvitationComponent(lobbyCode);

            lobbyInvitationComponent.Show();
        }
    }
}
