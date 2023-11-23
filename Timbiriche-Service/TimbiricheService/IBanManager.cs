using System;
using System.Collections.Generic;
using System.Linq;
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
        DateTime VerifyBanEndDate(int idPlayer);
    }
}
