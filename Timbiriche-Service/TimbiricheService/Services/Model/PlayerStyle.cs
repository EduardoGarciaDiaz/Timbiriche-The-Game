using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
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
