using System;
using System.Collections.Generic;
using System.Linq;
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

namespace TimbiricheViews.Views
{
    public partial class XAMLBan : Page
    {
        public XAMLBan(int idPlayerBanned)
        {
            InitializeComponent();

            Server.BanVerifierManagerClient banManagerClient = new Server.BanVerifierManagerClient();
            Server.BanInformation banInformation = banManagerClient.VerifyBanEndDate(idPlayerBanned);

            string formattedDateTime = banInformation.EndDate.ToString("dd MMMM yyyy HH:mm");
            lbBanEndDate.Content = formattedDateTime;

            if (banInformation.BanStatus.Equals("Inactive"))
            {
                gridBanFinished.Visibility = Visibility.Visible;
            }
        }

        private void BtnGoToLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
