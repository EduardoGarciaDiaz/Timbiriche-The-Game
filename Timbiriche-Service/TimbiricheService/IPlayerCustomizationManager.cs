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
        void RenewSubscriptionToColorsSelected(string lobbyCode, int idColor);
        [OperationContract(IsOneWay = true)]
        void UnsubscribeColorToColorsSelected(string lobbyCode, int oldIdColor);
    }

    [ServiceContract]
    public interface IPlayerColorsManagerCallback
    {
        [OperationContract]
        void NotifyColorSelected(int idSelectedColor);
        [OperationContract]
        void NotifyColorUnselected(int idUnselectedColor);
        [OperationContract]
        void NotifyOccupiedColors(List<int> occupiedColors);
    }



    [DataContract]
    public class PlayerColor
    {
        private int _idPlayerColors;
        private int _idPlayer;
        private int _idColor;

        [DataMember]
        public int IdPlayerColors { get { return _idPlayerColors; } set { _idPlayerColors = value; } }
        [DataMember]
        public int IdPlayer { get { return _idPlayer; } set { _idPlayer = value; } }
        [DataMember]
        public int IdColor { get { return _idColor; } set { _idColor = value; } }
    }

    [DataContract]
    public class PlayerStyle
    {
        private int _idPlayerStyle;
        private int _idPlayer;
        private int _idStyle;

        [DataMember]
        public int IdPlayerStyle { get { return _idPlayerStyle; } set { _idPlayerStyle = value; } }
        [DataMember]
        public int IdPlayer { get { return _idPlayer; } set { _idPlayer = value; } }
        [DataMember]
        public int IdStyle { get { return _idStyle; } set { _idStyle = value; } }
    }
}
