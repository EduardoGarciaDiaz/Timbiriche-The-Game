using System.Collections.Generic;
using System.ServiceModel;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IShopManager
    {
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        List<ShopColor> GetColors(int idPlayer);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        List<ShopStyle> GetStyles(int idPlayer);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        bool BuyColor(ShopColor color, int idPlayer);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        bool BuyStyle(ShopStyle style, int idPlayer);
    }
}
