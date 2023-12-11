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
        List<GlobalScore> GetGlobalScores(string username);
        [OperationContract]
        int UpdateWins(int idPlayer);
    }

    [ServiceContract(CallbackContract = typeof(IGlobalScoreManagerCallback))]
    public interface IGlobalScoreManager
    {
        [OperationContract(IsOneWay = true)]
        void SubscribeToGlobalScoreRealTime(string usernameCurrentPlayer);
        [OperationContract(IsOneWay = true)]
        void UnsubscribeToGlobalScoreRealTime(string usernameCurrentPlayer);
        [OperationContract(IsOneWay = true)]
        void UpdateGlobalScore();
    }

    [ServiceContract]
    public interface IGlobalScoreManagerCallback
    {
        [OperationContract]
        void NotifyGlobalScoreboardUpdated();
    }

    [DataContract]
    public class GlobalScore
    {
        private int _idGlobalScore;
        private int _idPlayer;
        private int _winsNumber;

        [DataMember]
        public int IdGlobalScore { get { return _idGlobalScore; } set { _idGlobalScore = value; } }
        [DataMember]
        public int IdPlayer { get { return _idPlayer; } set { _idPlayer = value; } }
        [DataMember]
        public int WinsNumber { get { return _winsNumber; } set { _winsNumber = value; } }
    }
}