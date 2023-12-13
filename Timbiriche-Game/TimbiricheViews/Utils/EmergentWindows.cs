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

        public static void CreateDataBaseErrorMessageWindow()
        {
            string titleEmergentWindow = Properties.Resources.lbDataBaseExceptionTitle;
            string descriptionEmergentWindow = Properties.Resources.tbkDataBaseExceptionDescription;

            XAMLEmergentWindow emergentWindow = new XAMLEmergentWindow(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.ShowDialog();
        }

        public static void CreateServerErrorMessageWindow()
        {
            string titleEmergentWindow = Properties.Resources.lbServerExceptionTitle;
            string descriptionEmergentWindow = Properties.Resources.tbkServerExceptionDescription;

            XAMLEmergentWindow emergentWindow = new XAMLEmergentWindow(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.ShowDialog();
        }

        public static void CreateUnexpectedErrorMessageWindow()
        {
            string titleEmergentWindow = Properties.Resources.lbUnexpectedExceptionTitle;
            string descriptionEmergentWindow = Properties.Resources.tbkUnexpectedExceptionDescription;

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

        public static void CreateLobbyNotFoundMessageWindow()
        {
            string titleEmergentWindow = Properties.Resources.lbLobbyNotFound;
            string descriptionEmergentWindow = Properties.Resources.tbkLobbyNotFound;

            XAMLEmergentWindow emergentWindow = new XAMLEmergentWindow(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.ShowDialog();
        }

        public static void CreateSuccesfulReportMessageWindow()
        {
            string titleEmergentWindow = Properties.Resources.lbSuccesfulReportTitle;
            string descriptionEmergentWindow = Properties.Resources.tbkSuccesfulReportDescription;

            XAMLEmergentWindow emergentWindow = new XAMLEmergentWindow(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.ShowDialog();
        }

        public static void CreateReportedPlayerMessageWindow()
        {
            string titleEmergentWindow = Properties.Resources.lbReportedPlayerTitle;
            string descriptionEmergentWindow = Properties.Resources.tbkReportedPlayerDescription;

            XAMLEmergentWindow emergentWindow = new XAMLEmergentWindow(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.ShowDialog();
        }

        public static void CreateBannedPlayerMessageWindow()
        {
            string titleEmergentWindow = Properties.Resources.lbBannedPlayerTitle;
            string descriptionEmergentWindow = Properties.Resources.tbkBannedPlayerDescription;

            XAMLEmergentWindow emergentWindow = new XAMLEmergentWindow(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.ShowDialog();
        }

        public static void CreateHostLeftLobbyMessageWindow()
        {
            string titleEmergentWindow = Properties.Resources.lbHostLeftLobbyTitle;
            string descriptionEmergentWindow = Properties.Resources.tbkHostLeftLobbyDescription;

            XAMLEmergentWindow emergentWindow = new XAMLEmergentWindow(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.ShowDialog();
        }

        public static void CreateLobbyIsFullMessageWindow()
        {
            string titleEmergentWindow = Properties.Resources.lbLobbyIsFullTitle;
            string descriptionEmergentWindow = Properties.Resources.tbkLobbyIsFullDescription;

            XAMLEmergentWindow emergentWindow = new XAMLEmergentWindow(
                titleEmergentWindow,
                descriptionEmergentWindow
            );

            emergentWindow.ShowDialog();
        }

    }
}
