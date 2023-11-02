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
    internal interface IPlayerCustomizationManager
    {
        [OperationContract]
        List<PlayerColor> GetMyColors(int idPlayer);
        [OperationContract]
        string GetHexadecimalColors(int idColor);
        [OperationContract]
        bool SelectMyColor(int idPlayer, int idColor);
        [OperationContract]
        bool GetMyStyles(int idPlayer);
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
