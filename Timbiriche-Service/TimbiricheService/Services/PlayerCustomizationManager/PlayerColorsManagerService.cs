using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using TimbiricheDataAccess.Utils;

namespace TimbiricheService
{
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
                    HandlerExceptions.HandleErrorException(ex);
                    RemovePlayerAndDictionaryFromDefaultColors(lobbyCode, currentUserCallbackChannel);
                }
            }
        }

        public void RenewSubscriptionToColorsSelected(string lobbyCode, LobbyPlayer lobbyPlayer)
        {
            IPlayerColorsManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerColorsManagerCallback>();
            int idColor = lobbyPlayer.IdHexadecimalColor;

            if (idColor == DEFAULT_COLOR)
            {
                HandleDefaultColorSubscription(lobbyCode, currentUserCallbackChannel);
            }
            else if (!IsColorSelected(lobbyCode, idColor))
            {
                HandleNonDefaultColorSubscription(lobbyCode, lobbyPlayer, currentUserCallbackChannel);
            }

            InformDefaultColorSubscriptors(lobbyCode, lobbyPlayer, idColor);
        }

        private void HandleDefaultColorSubscription(string lobbyCode, IPlayerColorsManagerCallback currentUserCallbackChannel)
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

                foreach (var colorSelector in players.ToList())
                {
                    try
                    {
                        colorSelector.ColorCallbackChannel?.NotifyColorSelected(lobbyPlayer);
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerExceptions.HandleErrorException(ex);
                        RemovePlayerAndDictionaryFromDefaultColors(lobbyCode, currentUserCallbackChannel);
                    }
                }
            }
        }

        private void InformDefaultColorSubscriptors(string lobbyCode, LobbyPlayer lobbyPlayer, int idColor)
        {
            foreach (var callbackChannel in playersWithDefaultColorByLobby[lobbyCode].ToList())
            {
                if (idColor != DEFAULT_COLOR)
                {
                    try
                    {
                        callbackChannel?.NotifyColorSelected(lobbyPlayer);
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerExceptions.HandleErrorException(ex);
                        RemovePlayerAndDictionaryFromDefaultColors(lobbyCode, callbackChannel);
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

            InformColorUnselected(lobbyCode, idColor);

            RemovePlayerAndDictionaryFromDefaultColors(lobbyCode, currentUserCallbackChannel);
        }

        private void InformColorUnselected(string lobbyCode, int idColor)
        {
            InformColorUnselectedToDefaultColors(lobbyCode, idColor);
            InformColorUnselectedToNonDefaultColor(lobbyCode, idColor);
        }

        private void InformColorUnselectedToNonDefaultColor(string lobbyCode, int idColor)
        {
            if (LobbyExists(lobbyCode) && IsColorSelected(lobbyCode, idColor))
            {
                List<LobbyPlayer> lobbyPlayers = GetLobbyPlayersList(lobbyCode);

                foreach (var colorCallbackChannel in lobbyPlayers.Select(p => p.ColorCallbackChannel).ToList())
                {
                    try
                    {
                         colorCallbackChannel?.NotifyColorUnselected(idColor);
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerExceptions.HandleErrorException(ex);
                        RemovePlayerAndDictionaryFromDefaultColors(lobbyCode, colorCallbackChannel);
                    }
                }
            }
        }

        private void InformColorUnselectedToDefaultColors(string lobbyCode, int idColor)
        {
            foreach (var callbackPlayer in playersWithDefaultColorByLobby[lobbyCode].ToList())
            {
                try
                {
                    callbackPlayer.NotifyColorUnselected(idColor);
                }
                catch (CommunicationException ex)
                {
                    HandlerExceptions.HandleErrorException(ex);
                    RemovePlayerAndDictionaryFromDefaultColors(lobbyCode, callbackPlayer);
                }
            }
        }

        private void RemovePlayerAndDictionaryFromDefaultColors(string lobbyCode, IPlayerColorsManagerCallback currentUserCallbackChannel)
        {
            DeletePlayerFromDefaultColorsDictionary(lobbyCode, currentUserCallbackChannel);
            DeleteDefaultColorsDictionary(lobbyCode);
        }

        private void DeletePlayerFromDefaultColorsDictionary(string lobbyCode, IPlayerColorsManagerCallback currentUserCallbackChannel)
        {
            if (playersWithDefaultColorByLobby.ContainsKey(lobbyCode) && playersWithDefaultColorByLobby[lobbyCode].Contains(currentUserCallbackChannel))
            {
                playersWithDefaultColorByLobby[lobbyCode].Remove(currentUserCallbackChannel);
            }
        }

        private void DeleteDefaultColorsDictionary(string lobbyCode)
        {
            int emptyDictionaryCount = 0;

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
}
