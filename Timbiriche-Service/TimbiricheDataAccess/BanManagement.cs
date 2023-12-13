using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess.Exceptions;
using TimbiricheDataAccess.Utils;

namespace TimbiricheDataAccess
{
    public static class BanManagement
    {
        public static bool CreateReport(int idPlayerReported, int idPlayerReporter, DateTime reportDate)
        {
            int rowsAffected = -1;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var playerReported = context.Players.Find(idPlayerReported);
                    var playerReporter = context.Players.Find(idPlayerReporter);

                    if (playerReported != null && playerReporter != null)
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
                }
            }
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }

            return rowsAffected > 0;
        }

        public static bool VerifyUniqueReport(int idPlayerReported, int idPlayerReporter)
        {
            bool isUniqueReport = false;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var playerReported = context.Players.Find(idPlayerReported);
                    var playerReporter = context.Players.Find(idPlayerReporter);

                    if (playerReported != null && playerReporter != null)
                    {
                        var query = context.Reports
                            .Where(r => r.idPlayerReported == idPlayerReported)
                            .Where(r => r.idPlayerReporter == idPlayerReporter);

                        isUniqueReport = (query.Count() <= 1);
                    }

                    return isUniqueReport;
                }
            }
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }
        }

        public static int GetNumberOfReportsByIdPlayerReported(int idPlayerReported)
        {
            int numberOfReports = 0;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var playerReported = context.Players.Find(idPlayerReported);

                    if (playerReported != null)
                    {
                        numberOfReports = context.Reports
                            .Where(r => r.idPlayerReported == idPlayerReported)
                            .Count();
                    }

                    return numberOfReports;
                }
            }
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }
        }

        public static int GetNumberOfBansByIdPlayer(int idPlayer)
        {
            int numberOfBans = 0;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var player = context.Players.Find(idPlayer);

                    if (player != null)
                    {
                        numberOfBans = context.Bans
                            .Where(b => b.idBannedPlayer == idPlayer)
                            .Count();
                    }

                    return numberOfBans;
                }
            }
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }
        }

        public static bool CreateBan(int idPlayer, DateTime startDate, DateTime endDate)
        {
            int rowsAffected = 0;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var player = context.Players.Find(idPlayer);

                    if (player != null)
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
                }
            }
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }

            return rowsAffected > 0;
        }

        public static bool ClearReportsByIdPlayer(int idPlayer)
        {
            int rowsAffected = 0;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var player = context.Players.Find(idPlayer);

                    if (player != null)
                    {
                        var reportsToRemove = context.Reports
                                            .Where(r => r.idPlayerReported == idPlayer);

                        context.Reports.RemoveRange(reportsToRemove);
                        rowsAffected = context.SaveChanges();
                    }
                }
            }
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }

            return rowsAffected > 0;
        }

        public static bool UpdatePlayerStatus(int idPlayer, string status)
        {
            int rowsAffected = 0;

            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    var playerToUpdate = context.Players.Find(idPlayer);

                    if (playerToUpdate != null)
                    {
                        playerToUpdate.status = status;
                        rowsAffected = context.SaveChanges();
                    }
                }
            }
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }

            return rowsAffected > 0;
        }

        public static DateTime GetBanEndDateByIdPlayer(int idPlayer)
        {
            try
            {
                DateTime banEndDate = DateTime.MinValue;

                using (var context = new TimbiricheDBEntities())
                {
                    var playerToUpdate = context.Players.Find(idPlayer);

                    if (playerToUpdate != null)
                    {
                        var ban = context.Bans
                            .Where(b => b.idBannedPlayer == idPlayer)
                            .OrderByDescending(b => b.startDate)
                            .FirstOrDefault();

                        if(ban != null)
                        {
                            banEndDate = (DateTime)ban.endDate;

                        }
                    }

                    return banEndDate;
                }
            }
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }
        }

        public static string GetPlayerStatusByIdPlayer(int idPlayer)
        {
            try
            {
                using (var context = new TimbiricheDBEntities())
                {
                    string playerStatus = null;
                    var player = context.Players.Find(idPlayer);

                    if (player != null)
                    {
                        playerStatus = player.status;
                    }

                    return playerStatus;
                }
            }
            catch (EntityException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (SqlException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                throw new DataAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                HandlerExceptions.HandleFatalException(ex);
                throw new DataAccessException(ex.Message);
            }
        }
    }
}
