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
        void ReportMessage(int idPlayerReported, int idPlayerReporter); 
    }

    [ServiceContract]
    public interface IBanManagerCallback
    {
        [OperationContract]
        void NotifyReportCompleted();
        [OperationContract]
        void NotifyPlayerAlreadyReported();
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
        private String _banStatus;

        [DataMember]
        public DateTime EndDate { get { return _endDate; } set { _endDate = value; } }
        [DataMember]
        public string BanStatus { get { return _banStatus; } set { _banStatus = value; } }
    }
     
}
