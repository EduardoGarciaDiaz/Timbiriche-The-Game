using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess.Exceptions;
using TimbiricheDataAccess;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    public partial class UserManagerService : IBanVerifierManager
    {
        public BanInformation VerifyBanEndDate(int idPlayer)
        {
            try
            {
                DateTime endDate = BanManagement.GetBanEndDateByIdPlayer(idPlayer);
                DateTime currentDateTime = DateTime.Now;

                BanInformation banInformation = new BanInformation();
                banInformation.EndDate = endDate;
                banInformation.BanStatus = "Active";

                if (currentDateTime >= endDate)
                {
                    BanManagement.UpdatePlayerStatus(idPlayer, "Not-Banned");
                    banInformation.BanStatus = "Inactive";
                }

                return banInformation;
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        public bool VerifyPlayerIsBanned(int idPlayer)
        {
            try
            {
                string playerStatus = BanManagement.GetPlayerStatusByIdPlayer(idPlayer);

                return playerStatus.Equals("Banned");
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }
    }
}
