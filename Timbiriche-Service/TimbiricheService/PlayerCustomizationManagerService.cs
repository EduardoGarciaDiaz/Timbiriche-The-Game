using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Utils;

namespace TimbiricheService
{
    public partial class UserManagerService : IPlayerCustomizationManager
    {
        public List<PlayerColor> GetMyColors(int idPlayer)
        {
            List<PlayerColor> myColors = new List<PlayerColor>();
            PlayerCustomizationManagement dataAccess = new PlayerCustomizationManagement();
            List<PlayerColors> playerColorsDataAccess = dataAccess.GetMyColorsByIdPlayer(idPlayer);

            foreach(PlayerColors playerColor in playerColorsDataAccess)
            {
                PlayerColor playerColorAuxiliar = new PlayerColor();
                playerColorAuxiliar.IdPlayerColors = playerColor.idPlayerColors;
                playerColorAuxiliar.IdPlayer = (int)playerColor.idPlayer;
                playerColorAuxiliar.IdColor = (int)playerColor.idColor;

                myColors.Add(playerColorAuxiliar);
            }

            return myColors;
        }

        public string GetHexadecimalColors(int idColor)
        {
            PlayerCustomizationManagement dataAccess = new PlayerCustomizationManagement();
            string hexadecimalColor = dataAccess.GetHexadecimalColorByIdColor(idColor);    
            
            return hexadecimalColor;
        }

        public int SelectMyColor(int idPlayer, int idColor)
        {
            PlayerCustomizationManagement dataAccess = new PlayerCustomizationManagement();

            return dataAccess.UpdateMyColorSelected(idPlayer, idColor);
        }

        public bool CheckColorForPlayer(int idPlayer, int idColor)
        {
            PlayerCustomizationManagement dataAccess = new PlayerCustomizationManagement();

            return dataAccess.SearchInMyColors(idPlayer, idColor);
        }

        public List<PlayerStyle> GetMyStyles(int idPlayer)
        {
            List<PlayerStyle> myStyles = new List<PlayerStyle>();
            PlayerCustomizationManagement dataAccess = new PlayerCustomizationManagement();
            List<PlayerStyles> playerStylesDataAccess = dataAccess.GetMyStylesByIdPlayer(idPlayer);

            foreach(PlayerStyles playerStyle in playerStylesDataAccess)
            {
                PlayerStyle playerStyleAxiliar = new PlayerStyle();
                playerStyleAxiliar.IdPlayerStyle = playerStyle.idPlayerStyles;
                playerStyleAxiliar.IdPlayer = (int)playerStyle.idPlayer;
                playerStyleAxiliar.IdStyle = (int)playerStyle.idStyle;

                myStyles.Add(playerStyleAxiliar);
            }

            return myStyles;
        }

        public string GetStylePath(int idStyle)
        {
            PlayerCustomizationManagement dataAccess = new PlayerCustomizationManagement();
            string stylePath = dataAccess.GetStylePathByIdStyle(idStyle);

            return stylePath;
        }

        public int SelectMyStyle(int idPlayer, int idStyle)
        {
            PlayerCustomizationManagement dataAccess = new PlayerCustomizationManagement();

            return dataAccess.UpdateMyStyleSelected(idPlayer, idStyle);
        }
    }

    public partial class UserManagerService : IPlayerColorsManager
    {
        private const int DEFAULT_COLOR = 0;
        private static Dictionary<string, List<IPlayerColorsManagerCallback>> playersWithDefaultColorByLobby = new Dictionary<string, List<IPlayerColorsManagerCallback>>();

        public void SubscribeColorToColorsSelected(string lobbyCode)
        {
            IPlayerColorsManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerColorsManagerCallback>();

            if (LobbyExists(lobbyCode))
            {
                List<LobbyPlayer> playersColorSelection = lobbies[lobbyCode].Item2;
                try
                {
                    currentUserCallbackChannel.NotifyOccupiedColors(playersColorSelection);
                }
                catch (CommunicationException ex)
                {
                    HandlerException.HandleErrorException(ex);
                    // TODO: Manage channels
                }
            }
        }

        public void RenewSubscriptionToColorsSelected(string lobbyCode, LobbyPlayer lobbyPlayer)
        {
            IPlayerColorsManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerColorsManagerCallback>();
            int idColor = lobbyPlayer.IdHexadecimalColor;

            if (idColor == DEFAULT_COLOR)
            {
                HandleDefaultColorSubscription(lobbyCode, lobbyPlayer, currentUserCallbackChannel);
            }
            else if (!IsColorSelected(lobbyCode, idColor))
            {
                HandleNonDefaultColorSubscription(lobbyCode, lobbyPlayer, currentUserCallbackChannel);
            }

            InformDefaultColorSubscriptors(lobbyCode, lobbyPlayer, idColor);
        }

        private void HandleDefaultColorSubscription(string lobbyCode, LobbyPlayer lobbyPlayer, IPlayerColorsManagerCallback currentUserCallbackChannel)
        {            
            if (!playersWithDefaultColorByLobby.ContainsKey(lobbyCode))
            {
                playersWithDefaultColorByLobby[lobbyCode] = new List<IPlayerColorsManagerCallback>();
            }

            playersWithDefaultColorByLobby[lobbyCode].Add(currentUserCallbackChannel);
        }

        private void HandleNonDefaultColorSubscription(string lobbyCode, LobbyPlayer lobbyPlayer, IPlayerColorsManagerCallback currentUserCallbackChannel)
        {
            if (lobbies.ContainsKey(lobbyCode))
            {
                LobbyPlayer auxiliarPlayer = GetLobbyPlayerByUsername(lobbyCode, lobbyPlayer.Username);
                List<LobbyPlayer> players = lobbies[lobbyCode].Item2;

                if (auxiliarPlayer != null)
                {
                    auxiliarPlayer.IdHexadecimalColor = lobbyPlayer.IdHexadecimalColor;
                    auxiliarPlayer.ColorCallbackChannel = currentUserCallbackChannel;
                }

                foreach (var colorSelector in players)
                {
                    try
                    {
                        colorSelector.ColorCallbackChannel?.NotifyColorSelected(lobbyPlayer);
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerException.HandleErrorException(ex);
                        // TODO: Manage channels
                    }
                }
            }
        }

        private void InformDefaultColorSubscriptors(string lobbyCode, LobbyPlayer lobbyPlayer, int idColor)
        {
            foreach (var callbackChannel in playersWithDefaultColorByLobby[lobbyCode])
            {
                if (idColor != DEFAULT_COLOR)
                {
                    try
                    {
                        callbackChannel?.NotifyColorSelected(lobbyPlayer);
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerException.HandleErrorException(ex);
                        // TODO: Manage channels
                    }
                }
            }
        }

        private LobbyPlayer GetLobbyPlayerByUsername(string lobbyCode, string username)
        {
            List<LobbyPlayer> lobbyPlayerList = GetLobbyPlayersList(lobbyCode);

            return lobbyPlayerList.Find(player => player.Username == username);
        }

        private List<LobbyPlayer> GetLobbyPlayersList(string lobbyCode)
        {
            return lobbies[lobbyCode].Item2;
        }

        public void UnsubscribeColorToColorsSelected(string lobbyCode, LobbyPlayer lobbyPlayer)
        {
            IPlayerColorsManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerColorsManagerCallback>();
            int idColor = lobbyPlayer.IdHexadecimalColor;
            int emptyDictionaryCount = 0;
            
            if (LobbyExists(lobbyCode) && IsColorSelected(lobbyCode, idColor))
            {
                List<LobbyPlayer> lobbyPlayers = GetLobbyPlayersList(lobbyCode);

                foreach (var player in lobbyPlayers)
                {
                    try
                    {
                        player.ColorCallbackChannel?.NotifyColorUnselected(idColor);
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerException.HandleErrorException(ex);
                        // TODO: Manage channels
                    }
                }
            }
            
            foreach(var callbackPlayer in playersWithDefaultColorByLobby[lobbyCode])
            {
                try
                {
                    callbackPlayer.NotifyColorUnselected(idColor);
                }
                catch (CommunicationException ex)
                {
                    HandlerException.HandleErrorException(ex);
                    // TODO: Manage channels
                }
            }

            if (playersWithDefaultColorByLobby.ContainsKey(lobbyCode) && playersWithDefaultColorByLobby[lobbyCode].Contains(currentUserCallbackChannel))
            {
                playersWithDefaultColorByLobby[lobbyCode].Remove(currentUserCallbackChannel);
            }

            if (playersWithDefaultColorByLobby[lobbyCode].Count == emptyDictionaryCount)
            {
                playersWithDefaultColorByLobby.Remove(lobbyCode);
            }
        }

        private bool IsColorSelected(string lobbyCode, int idColor)
        {
            bool isColorSelected = false;

            if (LobbyExists(lobbyCode))
            {
                LobbyPlayer playerHasColor = GetLobbyPlayersList(lobbyCode).Find(color => color.IdHexadecimalColor == idColor);
                
                if (playerHasColor != null)
                {
                    isColorSelected = true;
                }
            }

            return isColorSelected;
        }

        private bool LobbyExists(string lobbyCode)
        {
            bool doesLobbyExist = false;

            if (lobbies.ContainsKey(lobbyCode))
            {
                doesLobbyExist = true;
            }

            return doesLobbyExist;
        }
    }

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

                    foreach (var colorSelector in lobbies[lobbyCode].Item2)
                    {
                        try
                        {
                            colorSelector.StyleCallbackChannel.NotifyStyleSelected(lobbyPlayer);
                        }
                        catch (CommunicationException ex)
                        {
                            HandlerException.HandleErrorException(ex);
                            // TODO: Manage channels
                        }
                    }
                }
            }
        }
    }
}
