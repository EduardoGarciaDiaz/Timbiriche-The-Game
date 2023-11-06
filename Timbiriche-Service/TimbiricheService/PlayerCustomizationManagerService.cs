using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;

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
        private static ConcurrentDictionary<string, ConcurrentDictionary<int, IPlayerColorsManagerCallback>> selectedColorsByLobby = new ConcurrentDictionary<string, ConcurrentDictionary<int, IPlayerColorsManagerCallback>>();
        private static Dictionary<string, List<IPlayerColorsManagerCallback>> playersWithDefaultColorByLobby = new Dictionary<string, List<IPlayerColorsManagerCallback>>();
        private const int DEFAULT_COLOR = 0;

        public void SubscribeColorToColorsSelected(string lobbyCode)
        {
            IPlayerColorsManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerColorsManagerCallback>();
            if (selectedColorsByLobby.ContainsKey(lobbyCode))
            {
                ConcurrentDictionary<int, IPlayerColorsManagerCallback> selectedColorsAuxiliar = selectedColorsByLobby[lobbyCode];
                List<int> occupiedColors = selectedColorsAuxiliar.Keys.ToList();
                currentUserCallbackChannel.NotifyOccupiedColors(occupiedColors);
            }
        }

        public void RenewSubscriptionToColorsSelected(string lobbyCode, int idColor)
        {
            IPlayerColorsManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerColorsManagerCallback>();

            if (idColor == DEFAULT_COLOR)
            {
                if (!playersWithDefaultColorByLobby.ContainsKey(lobbyCode))
                {
                    playersWithDefaultColorByLobby[lobbyCode] = new List<IPlayerColorsManagerCallback>();
                }
                playersWithDefaultColorByLobby[lobbyCode].Add(currentUserCallbackChannel);
                currentUserCallbackChannel?.NotifyColorSelected(idColor);
            } 
            else if (!IsColorSelected(lobbyCode, idColor))
            {
                if (!selectedColorsByLobby.ContainsKey(lobbyCode))
                {
                    selectedColorsByLobby[lobbyCode] = new ConcurrentDictionary<int, IPlayerColorsManagerCallback>();
                }

                selectedColorsByLobby[lobbyCode].TryAdd(idColor, currentUserCallbackChannel);
                foreach (var colorSelector in selectedColorsByLobby[lobbyCode])
                {
                    colorSelector.Value.NotifyColorSelected(idColor);
                }
            }
            
            foreach (var callbackChannel in playersWithDefaultColorByLobby[lobbyCode])
            {
                if (idColor != DEFAULT_COLOR)
                {
                    callbackChannel.NotifyColorSelected(idColor);
                }
            }
        }

        private bool IsColorSelected(string lobbyCode, int idColor)
        {
            bool isColorSelected = false;
            if (selectedColorsByLobby.ContainsKey(lobbyCode))
            {
                isColorSelected = selectedColorsByLobby[lobbyCode].ContainsKey(idColor);
            }
            return isColorSelected;
        }

        public void UnsubscribeColorToColorsSelected(string lobbyCode, int oldIdColor)
        {
            IPlayerColorsManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerColorsManagerCallback>();

            if (selectedColorsByLobby.ContainsKey(lobbyCode) && selectedColorsByLobby[lobbyCode].ContainsKey(oldIdColor))
            {
                foreach (var colorSelector in selectedColorsByLobby[lobbyCode])
                {
                    colorSelector.Value.NotifyColorUnselected(oldIdColor);
                }
                selectedColorsByLobby[lobbyCode].TryRemove(oldIdColor, out _);
            }

            if (playersWithDefaultColorByLobby.ContainsKey(lobbyCode) && playersWithDefaultColorByLobby[lobbyCode].Contains(currentUserCallbackChannel))
            {
                playersWithDefaultColorByLobby[lobbyCode].Remove(currentUserCallbackChannel);
            }
        }
    }
}
