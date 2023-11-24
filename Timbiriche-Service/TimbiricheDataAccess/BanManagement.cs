using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace TimbiricheDataAccess
{
    public static class BanManagement
    {
        public static bool CreateReport(int idPlayerReported, int idPlayerReporter, DateTime reportDate)
        {
            int rowsAffected = 0;

            using (var context = new TimbiricheDBEntities())
            {
                var newReport = new Reports
                {
                    idPlayerReported = idPlayerReported,
                    idPlayerReporter = idPlayerReporter,
                    reportDate = reportDate
                };

                context.Reports.Add(newReport);
                rowsAffected = context.SaveChanges();
            }

            return rowsAffected > 0;
        }

        public static bool VerifyUniqueReport(int idPlayerReported, int idPlayerReporter)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var query = context.Reports
                    .Where(r => r.idPlayerReported == idPlayerReported)
                    .Where(r => r.idPlayerReporter == idPlayerReporter);

                return query.Count() < 1;
            }
        }

        public static int GetNumberOfReportsByIdPlayerReported(int idPlayerReported)
        {
            using (var context = new TimbiricheDBEntities())
            {
                int numberOfReports = context.Reports
                    .Where(r => r.idPlayerReported == idPlayerReported)
                    .Count();

                return numberOfReports;
            }
        }

        public static int GetNumberOfBansByIdPlayer(int idPlayer)
        {
            using (var context = new TimbiricheDBEntities())
            {
                int numberOfBans = context.Bans
                    .Where(b => b.idBannedPlayer == idPlayer)
                    .Count();

                return numberOfBans;
            }
        }

        public static bool CreateBan(int idPlayer, DateTime startDate, DateTime endDate)
        {
            int rowsAffected = 0;

            using (var context = new TimbiricheDBEntities())
            {
                var newBan = new Bans
                {
                    idBannedPlayer = idPlayer,
                    startDate = startDate,
                    endDate = endDate
                };

                context.Bans.Add(newBan);
                rowsAffected = context.SaveChanges();
            }

            return rowsAffected > 0;
        }

        public static bool ClearReportsByIdPlayer(int idPlayer)
        {
            int rowsAffected = 0;

            using (var context = new TimbiricheDBEntities())
            {
                var reportsToRemove = context.Reports
                    .Where(r => r.idPlayerReported == idPlayer);

                context.Reports.RemoveRange(reportsToRemove);
                rowsAffected = context.SaveChanges();
            }

            return rowsAffected > 0;
        }

        public static bool UpdatePlayerStatus(int idPlayer, string status)
        {
            int rowsAffected = 0;

            using (var context = new TimbiricheDBEntities())
            {
                var playerToUpdate = context.Players.Find(idPlayer);

                if(playerToUpdate != null)
                {
                    playerToUpdate.status = status;
                    rowsAffected = context.SaveChanges();
                }
            }

            return rowsAffected > 0;
        }

        public static DateTime GetBanEndDateByIdPlayer(int idPlayer)
        {
            using (var context = new TimbiricheDBEntities())
            {
                var banEndDate = context.Bans
                    .Where(b => b.idBannedPlayer == idPlayer)
                    .OrderByDescending(b => b.startDate)
                    .FirstOrDefault();

                return (DateTime) banEndDate.endDate;
            }
        }

        public static string GetPlayerStatusByIdPlayer(int idPlayer)
        {
            using (var context = new TimbiricheDBEntities())
            {
                string playerStatus = null;
                var player = context.Players.Find(idPlayer);

                if(player != null)
                {
                    playerStatus = player.status;
                }

                return playerStatus;
            }
        }
    }
}
