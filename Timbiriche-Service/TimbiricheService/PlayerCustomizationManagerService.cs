using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool GetMyStyles(int idPlayer)
        {
            throw new NotImplementedException();
        }


    }
}
