using System.Collections.Generic;
using System.ServiceModel;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IPlayerCustomizationManager
    {
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        List<PlayerColor> GetMyColors(int idPlayer);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        string GetHexadecimalColors(int idColor);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        int SelectMyColor(int idPlayer, int idColor);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        bool CheckColorForPlayer(int idPlayer, int idColor);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        List<PlayerStyle> GetMyStyles(int idPlayer);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        string GetStylePath(int idStyle);
        
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        int SelectMyStyle(int idPlayer, int idStyle);
    }
}
