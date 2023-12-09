using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IShopManager
    {
        [OperationContract]
        List<ShopColor> GetColors(int idPlayer);
        [OperationContract]
        List<ShopStyle> GetStyles(int idPlayer);
        [OperationContract]
        bool BuyColor(ShopColor color, int idPlayer);
        [OperationContract]
        bool BuyStyle(ShopStyle style, int idPlayer);

    }

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
