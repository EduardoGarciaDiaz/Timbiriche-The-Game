using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IPlayerCustomizationManager
    {
        [OperationContract]
        List<PlayerColor> GetMyColors(int idPlayer);
        [OperationContract]
        string GetHexadecimalColors(int idColor);
        [OperationContract]
        int SelectMyColor(int idPlayer, int idColor);
        [OperationContract]
        bool CheckColorForPlayer(int idPlayer, int idColor);
        [OperationContract]
        List<PlayerStyle> GetMyStyles(int idPlayer);
        [OperationContract]
        string GetStylePath(int idStyle);
        [OperationContract]
        int SelectMyStyle(int idPlayer, int idStyle);
    }

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

    [DataContract]
    public class PlayerColor
    {
        [DataMember]
        public int IdPlayerColors { get; set; }

        [DataMember]
        public int IdPlayer { get; set; }

        [DataMember]
        public int IdColor { get; set; }
    }

    [DataContract]
    public class PlayerStyle
    {
        [DataMember]
        public int IdPlayerStyle { get; set; }

        [DataMember]
        public int IdPlayer { get; set; }

        [DataMember]
        public int IdStyle { get; set; }
    }
}
