using System.ServiceModel;

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
