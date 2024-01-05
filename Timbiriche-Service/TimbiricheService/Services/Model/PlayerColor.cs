using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
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
}
