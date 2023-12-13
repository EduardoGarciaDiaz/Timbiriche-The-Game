using System.Collections.Generic;
using System.ServiceModel;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IScoreboardManager
    {
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        List<GlobalScore> GetGlobalScores();

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        int UpdateWins(int idPlayer);
    }
}