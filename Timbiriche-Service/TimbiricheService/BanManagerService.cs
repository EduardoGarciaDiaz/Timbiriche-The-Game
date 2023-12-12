using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Utils;

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

                LobbyPlayer playerToUpdate = playersInLobby.FirstOrDefault(player => player.Username == username);

                if (playerToUpdate != null)
                {
                    playerToUpdate.BanManagerChannel = currentBanUserCallbackChannel;
                }
            }
        }

        public void ReportMessage(int idPlayerReported, int idPlayerReporter)
        {
            IBanManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IBanManagerCallback>();

            if (VerifyUniqueReport(idPlayerReported, idPlayerReporter))
            {
                DateTime currentDateTime = DateTime.Now;
                bool reportCreated = BanManagement.CreateReport(idPlayerReported, idPlayerReporter, currentDateTime);

                if (reportCreated)
                {
                    VerifyBanNeed(idPlayerReported, currentDateTime);
                    try
                    {
                        currentUserCallbackChannel.NotifyReportCompleted();
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerException.HandleErrorException(ex);
                        // TODO: Manage channels
                    }
                }
            }
            else
            {
                try
                {
                    currentUserCallbackChannel.NotifyPlayerAlreadyReported();
                }
                catch (CommunicationException ex)
                {
                    HandlerException.HandleErrorException(ex);
                    // TODO: Manage channels
                }
            }
        }

        public void ReportPlayer(string lobbyCode, int idPlayerReported, int idPlayerReporter)
        {
            if (idPlayerReported > 0)
            {
                IBanManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IBanManagerCallback>();
                if (VerifyUniqueReport(idPlayerReported, idPlayerReporter))
                {
                    DateTime currentDateTime = DateTime.Now;
                    bool reportCreated = BanManagement.CreateReport(idPlayerReported, idPlayerReporter, currentDateTime);

                    if (reportCreated)
                    {
                        VerifyBanNeedFromLobby(lobbyCode, idPlayerReported, currentDateTime);
                        try
                        {
                            currentUserCallbackChannel.NotifyReportCompleted();
                        }
                        catch (CommunicationException ex)
                        {
                            HandlerException.HandleErrorException(ex);
                            // TODO: Manage channels
                        }
                    }
                }
                else
                {
                    try
                    {
                        currentUserCallbackChannel.NotifyPlayerAlreadyReported();
                    }
                    catch (CommunicationException ex)
                    {
                        HandlerException.HandleErrorException(ex);
                        // TODO: Manage channels
                    }
                }
            }
        }

        private void VerifyBanNeedFromLobby(string lobbyCode, int idPlayerReported, DateTime startDate)
        {
            int maximumNumberOfReportsOnLobby = 2;
            int numberOfReports = BanManagement.GetNumberOfReportsByIdPlayerReported(idPlayerReported);

            if (numberOfReports >= maximumNumberOfReportsOnLobby)
            {
                BanPlayer(idPlayerReported, startDate);
                List<LobbyPlayer> lobbyPlayers = lobbies[lobbyCode].Item2;

                UserManagement dataAccess = new UserManagement();
                string username = dataAccess.GetUsernameByIdPlayer(idPlayerReported);

                foreach(LobbyPlayer player in lobbyPlayers)
                {
                    if (player.Username == username)
                    {
                        try
                        {
                            player.BanManagerChannel.NotifyPlayerBanned(idPlayerReported);
                            
                        }
                        catch (CommunicationException ex)
                        {
                            HandlerException.HandleErrorException(ex);
                            // TODO: Manage channels
                        }
                        break;
                    }
                }
            }
        }

        private bool VerifyUniqueReport(int idPlayerReported, int idPlayerReporter)
        {
            return BanManagement.VerifyUniqueReport(idPlayerReported, idPlayerReporter);
        }

        private void VerifyBanNeed(int idPlayerReported, DateTime startDate)
        {
            int maximumNumberOfReportsOnMatch = 3;
            int numberOfReports = BanManagement.GetNumberOfReportsByIdPlayerReported(idPlayerReported);

            if (numberOfReports >= maximumNumberOfReportsOnMatch)
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

            /*switch (numberOfBans)
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
            }*/

            switch (numberOfBans)
            {
                case 0:
                    endDate = startDate.AddMinutes(2); 
                    break;
                case 1:
                    endDate = startDate.AddMinutes(5);
                    break;
                case 3:
                    endDate = startDate.AddHours(876000);
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

    public partial class UserManagerService : IBanVerifierManager
    {
        public BanInformation VerifyBanEndDate(int idPlayer)
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

        public bool VerifyPlayerIsBanned(int idPlayer)
        {
            string playerStatus = BanManagement.GetPlayerStatusByIdPlayer(idPlayer);
            return playerStatus.Equals("Banned");
        }
    }
}
