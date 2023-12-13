using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [DataContract]
    public class ShopStyle
    {
        [DataMember]
        public int IdStyle { get; set; }

        [DataMember]
        public string StyleName { get; set; }

        [DataMember]
        public string StylePath { get; set; }

        [DataMember]
        public int StyleCost { get; set; }

        [DataMember]
        public bool OwnedStyle { get; set; }
    }
}
