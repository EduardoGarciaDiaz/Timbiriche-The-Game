using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [ServiceContract(CallbackContract = typeof(IPlayerStylesManagerCallback))]
    public interface IPlayerStylesManager
    {
        [OperationContract(IsOneWay = true)]
        void AddStyleCallbackToLobbiesList(string lobbyCode, LobbyPlayer lobbyPlayer);
        
        [OperationContract(IsOneWay = true)]
        void ChooseStyle(string lobbyCode, LobbyPlayer lobbyPlayer);
    }

    [ServiceContract]
    public interface IPlayerStylesManagerCallback
    {
        [OperationContract]
        void NotifyStyleSelected(LobbyPlayer lobbyPlayer);
    }
}
