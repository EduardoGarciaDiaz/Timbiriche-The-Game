using System.Collections.Generic;
using System.ServiceModel;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IScoreboardManager
    {
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        List<GlobalScore> GetGlobalScores();

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        int UpdateWins(int idPlayer);
    }
}