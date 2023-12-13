using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
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