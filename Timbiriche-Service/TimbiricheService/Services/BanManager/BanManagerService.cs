using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Exceptions;
using TimbiricheDataAccess.Utils;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{
    public partial class UserManagerService : IBanManager
    {
        public void RegisterToBansNotifications(string lobbyCode, string username)
        {
            IBanManagerCallback currentBanUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IBanManagerCallback>();

            if (lobbies.ContainsKey(lobbyCode))
            {
                List<LobbyPlayer> playersInLobby = lobbies[lobbyCode].Item2;

                LobbyPlayer playerToUpdate = playersInLobby.Find(player => player.Username == username);

                if (playerToUpdate != null)
                {
                    playerToUpdate.BanManagerChannel = currentBanUserCallbackChannel;
                }
            }
        }

        public void ReportMessage(string lobbyCode, int idPlayerReported, int idPlayerReporter, string reporterUsername)
        {
            IBanManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IBanManagerCallback>();

            try
            {
                if (VerifyUniqueReport(idPlayerReported, idPlayerReporter))
                {
                    DateTime currentDateTime = DateTime.Now;
                    bool reportCreated = CreateReport(idPlayerReported, idPlayerReporter, currentDateTime);

                    if (reportCreated)
                    {
                        VerifyBanNeed(idPlayerReported, currentDateTime);
                        currentUserCallbackChannel.NotifyReportCompleted();
                    }
                }
                else
                {
                    currentUserCallbackChannel.NotifyPlayerAlreadyReported();
                }
            }
            catch (CommunicationException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                LeftMatch(lobbyCode, reporterUsername);
            }
            catch (TimeoutException ex)
            {
                HandlerExceptions.HandleErrorException(ex);
                LeftMatch(lobbyCode, reporterUsername);
            }
        }

        public void ReportPlayer(string lobbyCode, int idPlayerReported, int idPlayerReporter, string reporterUsername)
        {
            if (idPlayerReported > 0)
            {
                IBanManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IBanManagerCallback>();

                try
                {
                    if (VerifyUniqueReport(idPlayerReported, idPlayerReporter))
                    {
                        DateTime currentDateTime = DateTime.Now;
                        bool reportCreated = CreateReport(idPlayerReported, idPlayerReporter, currentDateTime);

                        if (reportCreated)
                        {
                            VerifyBanNeedFromLobby(lobbyCode, idPlayerReported, currentDateTime);
                            currentUserCallbackChannel.NotifyReportCompleted();
                        }
                    }
                    else
                    {
                        currentUserCallbackChannel.NotifyPlayerAlreadyReported();
                    }
                }
                catch (CommunicationException ex)
                {
                    HandlerExceptions.HandleErrorException(ex);
                    PerformExitLobby(lobbyCode, reporterUsername, false);
                }
            }
        }

        private bool CreateReport(int idPlayerReported, int idPlayerReporter, DateTime currentDateTime)
        {
            bool isCreatedReport = false;

            try
            {
                isCreatedReport = BanManagement.CreateReport(idPlayerReported, idPlayerReporter, currentDateTime);
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

            return isCreatedReport;
        }

        private void VerifyBanNeedFromLobby(string lobbyCode, int idPlayerReported, DateTime startDate)
        {
            try
            {
                int maximumNumberOfReportsOnLobby = 2;
                int numberOfReports = BanManagement.GetNumberOfReportsByIdPlayerReported(idPlayerReported);

                if (numberOfReports >= maximumNumberOfReportsOnLobby)
                {
                    BanPlayer(idPlayerReported, startDate);
                    List<LobbyPlayer> lobbyPlayers = lobbies[lobbyCode].Item2;

                    UserManagement dataAccess = new UserManagement();
                    string username = dataAccess.GetUsernameByIdPlayer(idPlayerReported);

                    NotifyPlayerBanned(lobbyCode, lobbyPlayers, username, idPlayerReported);
                }
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

        private void NotifyPlayerBanned(string lobbyCode, List<LobbyPlayer> lobbyPlayers, string username, int idPlayerReported)
        {
            foreach (LobbyPlayer player in lobbyPlayers.ToList())
            {
                if (player.Username == username)
                {
                    try
                    {
                        player.BanManagerChannel.NotifyPlayerBanned(idPlayerReported);
                        break;
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerExceptions.HandleErrorException(ex);
                        PerformExitLobby(lobbyCode, player.Username, false);
                    }
                }
            }            
        }

        private bool VerifyUniqueReport(int idPlayerReported, int idPlayerReporter)
        {
            try
            {
                return BanManagement.VerifyUniqueReport(idPlayerReported, idPlayerReporter);
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

        private void VerifyBanNeed(int idPlayerReported, DateTime startDate)
        {
            try
            {
                int maximumNumberOfReportsOnMatch = 3;
                int numberOfReports = BanManagement.GetNumberOfReportsByIdPlayerReported(idPlayerReported);

                if (numberOfReports >= maximumNumberOfReportsOnMatch)
                {
                    BanPlayer(idPlayerReported, startDate);
                }
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

        private void BanPlayer(int idPlayerReported, DateTime startDate)
        {
            try
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
