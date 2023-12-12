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
}
