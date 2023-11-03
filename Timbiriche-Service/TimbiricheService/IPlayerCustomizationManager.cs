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
        bool GetMyStyles(int idPlayer);
    }

    [ServiceContract(CallbackContract = typeof(IPlayerColorsManagerCallback))]
    public interface IPlayerColorsManager
    {
        [OperationContract(IsOneWay = true)]
        void SubscribeColorToColorsSelected();
        [OperationContract(IsOneWay = true)]
        void RenewSubscriptionToColorsSelected(int idColor);
        [OperationContract(IsOneWay = true)]
        void UnsubscribeColorToColorsSelected(int oldIdColor);
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
}
