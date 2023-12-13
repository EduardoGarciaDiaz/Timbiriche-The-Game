using System;

namespace TimbiricheViews.Player
{

    public static class PlayerSingleton
    {
        private static Server.Player _player;

        public static Server.Player Player
        {
            get
            {
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