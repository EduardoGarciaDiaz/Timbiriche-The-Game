using System.ServiceModel;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IBanVerifierManager
    {
        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        BanInformation VerifyBanEndDate(int idPlayer);

        [OperationContract]
        [FaultContract(typeof(TimbiricheServerException))]
        bool VerifyPlayerIsBanned(int idPlayer);
    }
}
