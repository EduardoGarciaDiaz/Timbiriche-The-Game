using System.Collections.Generic;
using System.ServiceModel;

namespace TimbiricheService
{
    [ServiceContract(CallbackContract = typeof(IPlayerColorsManagerCallback))]
    public interface IPlayerColorsManager
    {
        [OperationContract(IsOneWay = true)]
        void SubscribeColorToColorsSelected(string lobbyCode);
        
        [OperationContract(IsOneWay = true)]
        void RenewSubscriptionToColorsSelected(string lobbyCode, LobbyPlayer lobbyPlayer);
        
        [OperationContract(IsOneWay = true)]
        void UnsubscribeColorToColorsSelected(string lobbyCode, LobbyPlayer lobbyPlayer);
    }

    [ServiceContract]
    public interface IPlayerColorsManagerCallback
    {
        [OperationContract]
        void NotifyColorSelected(LobbyPlayer lobbyPlayer);
        
        [OperationContract]
        void NotifyColorUnselected(int idUnselectedColor);
        
        [OperationContract]
        void NotifyOccupiedColors(List<LobbyPlayer> occupiedColors);
    }
}
