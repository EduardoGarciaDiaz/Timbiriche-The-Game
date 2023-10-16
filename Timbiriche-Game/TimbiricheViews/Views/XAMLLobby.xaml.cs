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
using TimbiricheViews.Player;
using TimbiricheViews.Server;

namespace TimbiricheViews.Views
{
    public partial class XAMLLobby : Page
    {
        public XAMLLobby()
        {
            InitializeComponent();
            LoadDataPlayer();
        }

        private void LoadDataPlayer()
        {
            Server.Player playerLoggedIn = PlayerSingleton.player;
            string initialPlayerNameLetter = playerLoggedIn.username[0].ToString();
            lbUsername.Content = playerLoggedIn.username;
            lbCoins.Content = playerLoggedIn.coins;
            lbUserFaceBox.Content = initialPlayerNameLetter;
        }
    }
}
