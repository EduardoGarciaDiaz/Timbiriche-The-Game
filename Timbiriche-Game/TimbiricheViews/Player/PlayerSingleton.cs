using System;

namespace TimbiricheViews.Player
{

    public static class PlayerSingleton
    {
        public static Server.Player Player { get; set; }

        public static void UpdatePlayerFromDataBase()
        {
            Server.UserManagerClient userManagerClient = new Server.UserManagerClient(); 
            Player = userManagerClient.GetPlayerByIdPlayer(Player.IdPlayer);   
        }
    }
}