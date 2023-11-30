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
        private int _idColor;
        private string _colorName;
        private string _hexadecimalCode;
        private int _colorCost;
        private bool _ownedColor;

        [DataMember]
        public int IdColor { get { return _idColor; } set { _idColor = value; } }
        [DataMember]
        public string ColorName { get { return _colorName; } set { _colorName = value; } }
        [DataMember]
        public string HexadecimalCode { get { return _hexadecimalCode; } set { _hexadecimalCode = value; } }
        [DataMember]
        public int ColorCost { get { return _colorCost; } set { _colorCost = value; } }
        [DataMember]
        public bool OwnedColor { get { return _ownedColor; } set { _ownedColor = value; } }
    }

    [DataContract]
    public class ShopStyle
    {
        private int _idStyle;
        private string _styleName;
        private string _stylePath;
        private int _styleCost;
        private bool _ownedStyle;

        [DataMember]
        public int IdStyle { get { return _idStyle; } set { _idStyle = value; } }
        [DataMember]
        public string StyleName { get { return _styleName; } set { _styleName = value; } }
        [DataMember]
        public string StylePath { get { return _stylePath; } set { _stylePath = value; } }
        [DataMember]
        public int StyleCost { get { return _styleCost; } set { _styleCost = value; } }
        [DataMember]
        public bool OwnedStyle { get { return _ownedStyle; } set { _ownedStyle = value;} }
    }

}
