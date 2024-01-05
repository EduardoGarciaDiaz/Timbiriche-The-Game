using System;
using System.ServiceModel;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Exceptions;
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
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }

        public bool VerifyPlayerIsBanned(int idPlayer)
        {
            try
            {
                bool isPlayerBanned = false;

                if(idPlayer > 0)
                {
                    string playerStatus = BanManagement.GetPlayerStatusByIdPlayer(idPlayer);
                    isPlayerBanned = playerStatus.Equals("Banned");
                }

                return isPlayerBanned;
            }
            catch (DataAccessException ex)
            {
                TimbiricheServerExceptions exceptionResponse = new TimbiricheServerExceptions
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerExceptions>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }
        }
    }
}
