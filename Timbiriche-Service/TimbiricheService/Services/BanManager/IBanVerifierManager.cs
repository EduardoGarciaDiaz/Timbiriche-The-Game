using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheService
{
    [ServiceContract]
    public interface IBanVerifierManager
    {
        [OperationContract]
        BanInformation VerifyBanEndDate(int idPlayer);

        [OperationContract]
        bool VerifyPlayerIsBanned(int idPlayer);
    }
}
