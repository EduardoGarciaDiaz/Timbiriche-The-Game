using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
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
}
