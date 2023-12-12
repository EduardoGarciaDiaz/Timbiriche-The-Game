using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IPlayerCustomizationManager
    {
        [OperationContract]
        List<PlayerColor> GetMyColors(int idPlayer);
        
        [OperationContract]
        string GetHexadecimalColors(int idColor);
        
        [OperationContract]
        int SelectMyColor(int idPlayer, int idColor);
        
        [OperationContract]
        bool CheckColorForPlayer(int idPlayer, int idColor);
        
        [OperationContract]
        List<PlayerStyle> GetMyStyles(int idPlayer);
        
        [OperationContract]
        string GetStylePath(int idStyle);
        
        [OperationContract]
        int SelectMyStyle(int idPlayer, int idStyle);
    }
}
