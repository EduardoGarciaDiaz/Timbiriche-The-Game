using System.ServiceModel;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IBanVerifierManager
    {
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        BanInformation VerifyBanEndDate(int idPlayer);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerExceptions))]
        bool VerifyPlayerIsBanned(int idPlayer);
    }
}
