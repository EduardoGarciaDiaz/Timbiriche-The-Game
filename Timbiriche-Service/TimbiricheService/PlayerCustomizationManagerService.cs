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
        private static ConcurrentDictionary<int, IPlayerColorsManagerCallback> selectedColors = new ConcurrentDictionary<int, IPlayerColorsManagerCallback>();
        private const int DEFAULT_COLOR = 0;

        private static List<IPlayerColorsManagerCallback> playersWithDefaultColor = new List<IPlayerColorsManagerCallback>();


        public void SubscribeColorToColorsSelected()
        {
            IPlayerColorsManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerColorsManagerCallback>();
            List<int> occupiedColors = selectedColors.Keys.ToList();
            currentUserCallbackChannel.NotifyOccupiedColors(occupiedColors);
        }

        public void RenewSubscriptionToColorsSelected(int idColor)
        {
            IPlayerColorsManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerColorsManagerCallback>();

            if (idColor == DEFAULT_COLOR)
            {
                playersWithDefaultColor.Add(currentUserCallbackChannel);
                currentUserCallbackChannel?.NotifyColorSelected(idColor);
            }

            if (!selectedColors.ContainsKey(idColor) && idColor > DEFAULT_COLOR)
            {
                selectedColors.TryAdd(idColor, currentUserCallbackChannel);
                foreach (var colorSelector in selectedColors)
                {
                    colorSelector.Value.NotifyColorSelected(idColor);                    
                }
            }

            foreach(var callbackChannel in playersWithDefaultColor)
            {
                if (idColor != DEFAULT_COLOR)
                {
                    callbackChannel.NotifyColorSelected(idColor);
                }
            }
        }

        public void UnsubscribeColorToColorsSelected(int oldIdColor)
        {
            IPlayerColorsManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IPlayerColorsManagerCallback>();

            if (selectedColors.ContainsKey(oldIdColor))
            {
                foreach (var colorSelector in selectedColors)
                {
                    colorSelector.Value.NotifyColorUnselected(oldIdColor);
                }
                selectedColors.TryRemove(oldIdColor, out _);
            }

            if (playersWithDefaultColor.Contains(currentUserCallbackChannel))
            {
                playersWithDefaultColor.Remove(currentUserCallbackChannel);
            }

        }
    }
}
