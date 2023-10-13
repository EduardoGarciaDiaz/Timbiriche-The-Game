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
    /// <summary>
    /// Lógica de interacción para XAMLLobby.xaml
    /// </summary>
    public partial class XAMLLobby : Page
    {
        private Player player;
        public XAMLLobby()
        {
            InitializeComponent();
        }

        public XAMLLobby(Player playerLogged)
        {
            player = playerLogged;
            InitializeComponent();
            LoadDataPlayer();
        }

        private void LoadDataPlayer()
        {
            string initialPlayerNameLetter = player.username[0].ToString();
            lbUsername.Content = player.username;
            lbCoins.Content = player.coins;
            lbUserFaceBox.Content = initialPlayerNameLetter;
            Console.WriteLine(player.accountFK.name);
        }
    }
}
