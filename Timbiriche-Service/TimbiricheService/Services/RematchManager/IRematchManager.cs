using System;
using System.ServiceModel;

namespace TimbiricheService
{
    [ServiceContract(CallbackContract = typeof(IRematchManagerCallback))]
    public interface IRematchManager
    {
        [OperationContract(IsOneWay = true)]
        void NotRematch(string lobbyCode);

        [OperationContract(IsOneWay = true)]
        void Rematch(String lobbyCode, string username);
    }

    [ServiceContract]
    public interface IRematchManagerCallback
    {
        [OperationContract]
        void NotifyRematch(string lobbyCode);

        [OperationContract]
        void NotifyHostOfRematch(string lobbyCode);
    }
}
