using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess.Utils;

namespace TimbiricheService
{
    public partial class UserManagerService : IPlayerStylesManager
    {

        public void AddStyleCallbackToLobbiesList(string lobbyCode, LobbyPlayer lobbyPlayer)
        {
            if (lobbies.ContainsKey(lobbyCode))
            {
                IPlayerStylesManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerStylesManagerCallback>();
                LobbyPlayer auxiliarPlayer = GetLobbyPlayerByUsername(lobbyCode, lobbyPlayer.Username);

                if (auxiliarPlayer != null)
                {
                    auxiliarPlayer.StyleCallbackChannel = currentUserCallbackChannel;
                }
            }
        }

        public void ChooseStyle(string lobbyCode, LobbyPlayer lobbyPlayer)
        {
            if (lobbies.ContainsKey(lobbyCode))
            {
                LobbyPlayer auxiliarPlayer = GetLobbyPlayerByUsername(lobbyCode, lobbyPlayer.Username);

                if (auxiliarPlayer != null)
                {
                    auxiliarPlayer.IdStylePath = lobbyPlayer.IdStylePath;

                    foreach (var styleSelector in lobbies[lobbyCode].Item2.ToList())
                    {
                        try
                        {
                            styleSelector.StyleCallbackChannel.NotifyStyleSelected(lobbyPlayer);
                        }
                        catch (CommunicationException ex)
                        {
                            HandlerException.HandleErrorException(ex);
                            styleSelector.StyleCallbackChannel = null;
                        }
                    }
                }
            }
        }
    }
}
