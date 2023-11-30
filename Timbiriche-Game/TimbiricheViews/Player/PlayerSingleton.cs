using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheViews.Player
{

    public static class PlayerSingleton
    {
        private static Server.Player _player;

        public static Server.Player Player
        {
            get
            {
                if(_player == null)
                {
                    throw new ArgumentNullException("The Player instance has not been configured.");
                }

                return _player;
            }

            set
            {
                _player = value;
            }
        }

        public static void UpdatePlayerFromDataBase()
        {
            Server.UserManagerClient userManagerClient = new Server.UserManagerClient();
            _player = userManagerClient.GetPlayerByIdPlayer(_player.IdPlayer);
        }
    }
}