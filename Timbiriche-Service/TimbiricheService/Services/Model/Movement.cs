using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [DataContract]
    public class Movement
    {
        [DataMember]
        public string TypeLine { get; set; }

        [DataMember]
        public int Row { get; set; }
        
        [DataMember]
        public int Column { get; set; }
        
        [DataMember]
        public int EarnedPoints { get; set; }
        
        [DataMember]
        public string HexadecimalColor { get; set; }
        
        [DataMember]
        public string StylePath { get; set; }
        
        [DataMember]
        public string Username { get; set; }
    }
}
