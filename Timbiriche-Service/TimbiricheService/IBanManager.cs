using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [ServiceContract(CallbackContract = typeof(IBanManagerCallback))]
    public interface IBanManager
    {
        [OperationContract(IsOneWay = true)]
        void RegisterToBansNotifications(string lobbyCode, string username);
        [OperationContract(IsOneWay = true)]
        void ReportMessage(string lobbyCode, int idPlayerReported, int idPlayerReporter, string reporterUsername);
        [OperationContract(IsOneWay = true)]
        void ReportPlayer(string lobbyCode, int idPlayerReported, int idPlayerReporter, string reporterUsername);
    }

    [ServiceContract]
    public interface IBanManagerCallback
    {
        [OperationContract]
        void NotifyReportCompleted();
        [OperationContract]
        void NotifyPlayerAlreadyReported();
        [OperationContract]
        void NotifyPlayerBanned(int idPlayerBanned);
    }

    [ServiceContract]
    public interface IBanVerifierManager
    {
        [OperationContract]
        BanInformation VerifyBanEndDate(int idPlayer);
        [OperationContract]
        bool VerifyPlayerIsBanned(int idPlayer);
    }

    [DataContract]
    public class BanInformation
    {
        private DateTime _endDate;
        private string _banStatus;

        [DataMember]
        public DateTime EndDate { get { return _endDate; } set { _endDate = value; } }
        [DataMember]
        public string BanStatus { get { return _banStatus; } set { _banStatus = value; } }
    }
     
}
