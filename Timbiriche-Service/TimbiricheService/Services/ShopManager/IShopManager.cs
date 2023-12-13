using System.Collections.Generic;
using System.ServiceModel;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IShopManager
    {
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        List<ShopColor> GetColors(int idPlayer);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        List<ShopStyle> GetStyles(int idPlayer);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        bool BuyColor(ShopColor color, int idPlayer);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        bool BuyStyle(ShopStyle style, int idPlayer);
    }
}
