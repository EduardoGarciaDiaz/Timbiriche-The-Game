using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;

namespace TimbiricheService
{
    public partial class UserManagerService : IBanManager
    {
        public void ReportMessage(int idPlayerReported, int idPlayerReporter, DateTime reportDate)
        {
            IBanManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IBanManagerCallback>();


            if (VerifyUniqueReport(idPlayerReported, idPlayerReporter))
            {
                bool reportCreated = BanManagement.CreateReport(idPlayerReported, idPlayerReporter, reportDate);

                if (reportCreated)
                {
                    VerifyBanNeed(idPlayerReported, reportDate);
                    currentUserCallbackChannel.NotifyReportCompleted();
                }
            }
            else
            {
                currentUserCallbackChannel.NotifyPlayerAlreadyReported();
            }
        }

        public DateTime VerifyBanEndDate(int idPlayer, DateTime currentDate)
        {
            DateTime endDate = BanManagement.GetBanEndDateByIdPlayer(idPlayer);

            if(endDate >= currentDate)
            {
                BanManagement.UpdatePlayerStatus(idPlayer, "Not-Banned");
            }

            return endDate;
        }

        private bool VerifyUniqueReport(int idPlayerReported, int idPlayerReporter)
        {
            return BanManagement.VerifyUniqueReport(idPlayerReported, idPlayerReporter);
        }

        private void VerifyBanNeed(int idPlayerReported, DateTime startDate)
        {
            int numberOfReports = BanManagement.GetNumberOfReportsByIdPlayerReported(idPlayerReported);

            if (numberOfReports >= 3)
            {
                BanPlayer(idPlayerReported, startDate);
            }

        }

        private void BanPlayer(int idPlayerReported, DateTime startDate)
        {
            int numberOfBans = BanManagement.GetNumberOfBansByIdPlayer(idPlayerReported);
            DateTime endDate = CalculateBanEndDate(startDate, numberOfBans);
            
            bool isPlayerBanned = BanManagement.CreateBan(idPlayerReported, startDate, endDate);

            if (isPlayerBanned)
            {
                BanManagement.ClearReportsByIdPlayer(idPlayerReported);
                BanManagement.UpdatePlayerStatus(idPlayerReported, "Banned");
            }
        }

        private DateTime CalculateBanEndDate(DateTime startDate, int numberOfBans)
        {
            DateTime endDate;

            switch (numberOfBans)
            {
                case 0:
                    endDate = startDate.AddHours(24); 
                    break;
                case 1:
                    endDate = startDate.AddHours(48);
                    break;
                case 3:
                    endDate = startDate.AddHours(72);
                    break;
                case 4:
                    endDate = startDate.AddHours(876000);
                    break;
                default:
                    endDate = startDate;
                    break;
            }

            return endDate;
        }
    }
}
