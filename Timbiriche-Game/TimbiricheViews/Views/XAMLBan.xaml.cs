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
            DateTime banEndDateTime = banManagerClient.VerifyBanEndDate(idPlayerBanned);


            string formattedDateTime = banEndDateTime.ToString("dd MMMM yyyy HH:mm");
            lbBanEndDate.Content = formattedDateTime;
        }
    }
}
