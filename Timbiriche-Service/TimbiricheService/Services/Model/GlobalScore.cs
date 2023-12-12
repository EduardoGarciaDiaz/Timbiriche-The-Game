using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [DataContract]
    public class GlobalScore
    {
        private int _idGlobalScore;
        private int _idPlayer;
        private int _winsNumber;

        [DataMember]
        public int IdGlobalScore { get { return _idGlobalScore; } set { _idGlobalScore = value; } }

        [DataMember]
        public int IdPlayer { get { return _idPlayer; } set { _idPlayer = value; } }
        
        [DataMember]
        public int WinsNumber { get { return _winsNumber; } set { _winsNumber = value; } }
    }
}
