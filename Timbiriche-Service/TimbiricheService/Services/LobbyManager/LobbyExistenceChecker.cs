namespace TimbiricheService
{
    public partial class UserManagerService : ILobbyExistenceChecker
    {
        public bool ExistLobbyCode(string lobbyCode)
        {
            bool existLobby = false;

            if (lobbies.ContainsKey(lobbyCode))
            {
                existLobby = true;
            }

            return existLobby;
        }
    }
}
