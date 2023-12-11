using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TimbiricheDataAccess;
using TimbiricheDataAccess.Exceptions;
using TimbiricheService.Exceptions;

namespace TimbiricheService
{

    public partial class UserManagerService : IScoreboardManager
    {
        public List<GlobalScore> GetGlobalScores(string username)
        {
            GlobalScoresManagement dataAccess = new GlobalScoresManagement();
            List<GlobalScore> globalScores = new List<GlobalScore>();

            try
            {
                List<GlobalScores> scores = dataAccess.GetGlobalScores();

                foreach (GlobalScores score in scores)
                {
                    GlobalScore globalScore = new GlobalScore
                    {
                        IdGlobalScore = score.idGlobalScore,
                        IdPlayer = (int)score.idPlayer,
                        WinsNumber = (int)score.winsNumber
                    };
                    globalScores.Add(globalScore);
                }
            }
            catch (DataAccessException ex)
            {
                _logger.Error(ex.Source + " - " + ex.Message + "\n" + ex.StackTrace + "\n");

                globalScoreRealTime.Remove(username);
                UnregisterUserToOnlineUsers(username);

                TimbiricheServerException exceptionResponse = new TimbiricheServerException
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<TimbiricheServerException>(exceptionResponse, new FaultReason(exceptionResponse.Message));
            }

            return globalScores;
        }

        public int UpdateWins(int idPlayer)
        {
            GlobalScoresManagement dataAccess = new GlobalScoresManagement();
            int response = dataAccess.UpdateWinsPlayer(idPlayer);
            UpdateGlobalScore();
            return response;
        }
    }

    public partial class UserManagerService : IGlobalScoreManager
    {
        private static Dictionary<string, IGlobalScoreManagerCallback> globalScoreRealTime = new Dictionary<string, IGlobalScoreManagerCallback>();

        public void SubscribeToGlobalScoreRealTime(string usernameCurrentPlayer)
        {
            if (!globalScoreRealTime.ContainsKey(usernameCurrentPlayer))
            {
                IGlobalScoreManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IGlobalScoreManagerCallback>();
                globalScoreRealTime.Add(usernameCurrentPlayer, currentUserCallbackChannel);
                currentUserCallbackChannel.NotifyGlobalScoreboardUpdated();
            }
        }

        public void UnsubscribeToGlobalScoreRealTime(string usernameCurrentPlayer)
        {
            if (globalScoreRealTime.ContainsKey(usernameCurrentPlayer))
            {
                IGlobalScoreManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IGlobalScoreManagerCallback>();
                globalScoreRealTime.Remove(usernameCurrentPlayer);
            }
        }

        public void UpdateGlobalScore()
        {
            foreach (var user in globalScoreRealTime)
            {
                user.Value.NotifyGlobalScoreboardUpdated();
            }
        }
    }


}
