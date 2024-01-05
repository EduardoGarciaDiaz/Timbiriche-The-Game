using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [DataContract]
    public class ShopColor
    {
        [DataMember]
        public int IdColor { get; set; }

        [DataMember]
        public string ColorName { get; set; }

        [DataMember]
        public string HexadecimalCode { get; set; }

        [DataMember]
        public int ColorCost { get; set; }

        [DataMember]
        public bool OwnedColor { get; set; }
    }
}
