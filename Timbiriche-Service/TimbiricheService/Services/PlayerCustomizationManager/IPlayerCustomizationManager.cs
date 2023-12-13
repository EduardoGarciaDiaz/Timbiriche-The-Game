using System.Collections.Generic;
using System.ServiceModel;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IPlayerCustomizationManager
    {
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        List<PlayerColor> GetMyColors(int idPlayer);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        string GetHexadecimalColors(int idColor);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        int SelectMyColor(int idPlayer, int idColor);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        bool CheckColorForPlayer(int idPlayer, int idColor);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        List<PlayerStyle> GetMyStyles(int idPlayer);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        string GetStylePath(int idStyle);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        int SelectMyStyle(int idPlayer, int idStyle);
    }
}
